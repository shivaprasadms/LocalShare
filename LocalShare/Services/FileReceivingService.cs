using LocalShare.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalShare.Services
{
    public class FileReceivingService
    {

        public static async Task ReceiveFromClient(TcpClientModel client)
        {

            NetworkStream stream = client.TcpConnection.GetStream();

            await Task.Factory.StartNew(async () =>
            {

                while (true)
                {
                    try
                    {


                        byte[] filelen = new byte[1];

                        await stream.ReadAsync(filelen, 0, 1);


                        byte[] fileInfoBufferSize = new byte[int.Parse(Encoding.UTF8.GetString(filelen))];

                        await stream.ReadAsync(fileInfoBufferSize, 0, fileInfoBufferSize.Length);

                        string size = Encoding.UTF8.GetString(fileInfoBufferSize);

                        byte[] fileInfoBuffer = new byte[int.Parse(size)];

                        await stream.ReadAsync(fileInfoBuffer, 0, fileInfoBuffer.Length);

                        client.IsReceivingFile = true;

                        string fileInfo = Encoding.UTF8.GetString(fileInfoBuffer);

                        var fileInfoArray = fileInfo.Split(':');

                        string fileName = fileInfoArray[0];

                        string fileSize = fileInfoArray[1];

                        long fileSizeInBytes = long.Parse(fileSize);

                        client.CurrentReceivingFileName = fileName;

                        client.CurrentReceivingFileSize = fileSize;

                        long completed = 0;
                        long fs = fileSizeInBytes;


                        var fileSavePath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), fileName);


                        using (FileStream fileStream = new FileStream(fileSavePath, FileMode.Create))
                        {
                            byte[] buffer = new byte[8192];

                            int bytesRead;


                            while ((bytesRead = await stream.ReadAsync(buffer, 0, (int)(fileSizeInBytes > 8192 ? 8192 : fileSizeInBytes))) > 0)
                            {

                                await fileStream.WriteAsync(buffer, 0, bytesRead);

                                fileSizeInBytes -= bytesRead;
                                completed += bytesRead;
                                if (fileSizeInBytes <= 0) break;

                                client.CurrentReceivingFilePercentage = ((double)completed / fs) * 100;
                            }


                            fileStream.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                    // await stream.WriteAsync(Encoding.UTF8.GetBytes("DONE"), 0, 3);  // signal to sync end of file
                }

            }, TaskCreationOptions.LongRunning);





        }


    }
}
