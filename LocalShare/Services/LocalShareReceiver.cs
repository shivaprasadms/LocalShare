using LocalShare.Helpers;
using LocalShare.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LocalShare.Services
{
    public class LocalShareReceiver
    {
        private readonly ILogger<LocalShareReceiver> _logger;

        public LocalShareReceiver(ILogger<LocalShareReceiver> logger)
        {
            _logger = logger;
        }

        public async Task ReceiveFromClient(TcpClientModel client)
        {

            NetworkStream stream = client.TcpConnection.GetStream();

            await Task.Factory.StartNew(async () =>
            {

                while (true)
                {
                    try
                    {
                        byte[] fileInfoStringLengthBuffer = new byte[1];

                        await stream.ReadAsync(fileInfoStringLengthBuffer, 0, 1);

                        byte[] fileInfoBufferSize = new byte[int.Parse(Encoding.UTF8.GetString(fileInfoStringLengthBuffer))];

                        await stream.ReadAsync(fileInfoBufferSize, 0, fileInfoBufferSize.Length);

                        byte[] fileInfoStringBuffer = new byte[int.Parse(Encoding.UTF8.GetString(fileInfoBufferSize))];

                        await stream.ReadAsync(fileInfoStringBuffer, 0, fileInfoStringBuffer.Length);

                        client.IsReceivingFile = true;

                        string fileInfo = Encoding.UTF8.GetString(fileInfoStringBuffer);

                        var fileInfoArray = fileInfo.Split(':');

                        string fileName = fileInfoArray[0];

                        string fileSize = fileInfoArray[1];

                        string fileType = fileInfoArray[2] == "/" ? "" : fileInfoArray[2]; // refactor this

                        long fileSizeInBytes = long.Parse(fileSize);

                        var updateFileInfoToClient = DisplayInfo.UpdateCurrentReceivingFileInfo(client, fileName, fileSizeInBytes);

                        int bytesRead = 0;
                        long completed = 0;
                        long fs = fileSizeInBytes;

                        Stopwatch stopwatch = new Stopwatch();
                        Timer timer = new Timer(300);

                        timer.Elapsed += async (sender, e) =>
                        await DisplayInfo.UpdateCurrentReceivingFileProgress(client, completed, fs, stopwatch.Elapsed.TotalSeconds);
                        await updateFileInfoToClient;

                        var fileSavePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fileType);

                        if (!string.IsNullOrEmpty(fileType))
                        {
                            Directory.CreateDirectory(fileSavePath);
                        }


                        using (FileStream fileStream = new FileStream($"{fileSavePath}/{fileName}", FileMode.Create))
                        {
                            byte[] buffer = new byte[8192];


                            timer.Start();
                            stopwatch.Start();

                            while ((bytesRead = await stream.ReadAsync(buffer, 0, (int)(fileSizeInBytes > 8192 ? 8192 : fileSizeInBytes))) > 0)
                            {

                                await fileStream.WriteAsync(buffer, 0, bytesRead);

                                fileSizeInBytes -= bytesRead;
                                completed += bytesRead;
                                if (fileSizeInBytes <= 0) break;

                            }
                        }

                        timer.Dispose();
                        client.IsReceivingFile = false;

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }

                }

            }, TaskCreationOptions.LongRunning);





        }
    }
}
