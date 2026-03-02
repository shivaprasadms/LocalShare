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

           
            await Task.Run(async () =>
            {
                while (client.TcpConnection.Connected)
                {
                    try
                    {
                        // Read the length of the file info buffer size string (1 byte)
                        byte[] fileInfoStringLengthBuffer = new byte[1];
                        int bytesRead = await stream.ReadAsync(fileInfoStringLengthBuffer, 0, 1);

                        if (bytesRead == 0)
                        {
                            _logger.LogInformation("Client disconnected");
                            break;
                        }

                        // Parse how many digits the file info size has
                        int fileInfoSizeLength = int.Parse(Encoding.UTF8.GetString(fileInfoStringLengthBuffer));

                        // Read the actual file info size string
                        byte[] fileInfoBufferSize = new byte[fileInfoSizeLength];
                        await ReadExactAsync(stream, fileInfoBufferSize, 0, fileInfoSizeLength);

                        // Parse the file info size
                        int fileInfoSize = int.Parse(Encoding.UTF8.GetString(fileInfoBufferSize));

                        // Read the file info string
                        byte[] fileInfoStringBuffer = new byte[fileInfoSize];
                        await ReadExactAsync(stream, fileInfoStringBuffer, 0, fileInfoSize);

                        client.IsReceivingFile = true;

                        string fileInfo = Encoding.UTF8.GetString(fileInfoStringBuffer);
                        var fileInfoArray = fileInfo.Split(':');

                        string fileName = fileInfoArray[0];
                        string fileSize = fileInfoArray[1];
                        string fileType = fileInfoArray[2] == "/" ? "" : fileInfoArray[2];

                        long fileSizeInBytes = long.Parse(fileSize);

                        var updateFileInfoToClient = DisplayInfo.UpdateCurrentReceivingFileInfo(client, fileName, fileSizeInBytes);

                        long completed = 0;
                        long fs = fileSizeInBytes;

                        Stopwatch stopwatch = new Stopwatch();
                        Timer timer = new Timer(300);

                        timer.Elapsed += async (sender, e) =>
                            await DisplayInfo.UpdateCurrentReceivingFileProgress(client, completed, fs, stopwatch.Elapsed.TotalSeconds);

                        await updateFileInfoToClient;

                        // Fixed: Use Downloads folder instead of Music
                        var basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        var fileSavePath = string.IsNullOrEmpty(fileType)
                            ? basePath
                            : Path.Combine(basePath, fileType);

                        if (!string.IsNullOrEmpty(fileType))
                        {
                            Directory.CreateDirectory(fileSavePath);
                        }

                        using (FileStream fileStream = new FileStream(Path.Combine(fileSavePath, fileName), FileMode.Create))
                        {
                            byte[] buffer = new byte[8192];

                            timer.Start();
                            stopwatch.Start();

                            long remainingBytes = fileSizeInBytes;

                            while (remainingBytes > 0)
                            {
                                int toRead = (int)Math.Min(buffer.Length, remainingBytes);
                                int read = await stream.ReadAsync(buffer, 0, toRead);

                                if (read == 0)
                                {
                                    _logger.LogError("Connection lost while receiving file");
                                    break;
                                }

                                await fileStream.WriteAsync(buffer, 0, read);

                                remainingBytes -= read;
                                completed += read;
                            }
                        }

                        // Fixed: Stop timer before disposing
                        timer.Stop();
                        timer.Dispose();

                        client.IsReceivingFile = false;

                        _logger.LogInformation($"File received successfully: {fileName}");
                    }
                    catch (IOException ioEx)
                    {
                        _logger.LogError(ioEx, "Network error during file reception");
                        break; // Exit on network errors
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error receiving file: {Message}", ex.Message);
                        client.IsReceivingFile = false;
                        break; // Exit on unexpected errors
                    }
                }
            });
        }

        // Helper method to ensure we read exactly the number of bytes requested
        private async Task ReadExactAsync(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int totalRead = 0;
            while (totalRead < count)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset + totalRead, count - totalRead);
                if (bytesRead == 0)
                {
                    throw new IOException("Connection closed before all data was received");
                }
                totalRead += bytesRead;
            }
        }
    }
}