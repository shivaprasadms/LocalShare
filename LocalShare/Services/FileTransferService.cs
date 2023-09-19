using LocalShare.Models;
using LocalShare.Utility;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace LocalShare.Services
{
    public class FileTransferService
    {

        private static Object LOCK = new Object();

        public static async Task SendToClient(TcpClientModel client, Tuple<string, string[]> filePath, bool isFolder)
        {

            lock (LOCK)
            {
                client.AddFilesToQueue(filePath);

                if (client.IsSendingFile) return;

            }


            await Task.Factory.StartNew(async () =>
             {

                 try
                 {
                     client.IsSendingFile = true;

                     NetworkStream stream = client.TcpConnection.GetStream();

                     while (!client.IsQueueEmpty())
                     {

                         var fileTuple = client.PopFileFromQueue();

                         foreach (var path in fileTuple.Item2)
                         {

                             string fileName = Path.GetFileName(path);

                             FileInfo fileInfo = new FileInfo(path);

                             string fileSize = fileInfo.Length.ToString();

                             long fileSizeInBytes = long.Parse(fileSize);

                             long copy = fileSizeInBytes;

                             string fileInfoString = $"{fileName}:{fileSize}:{fileTuple.Item1}:"; // <300 length

                             int length = 0;

                             if (fileInfoString.Length > 0 && fileInfoString.Length < 9)
                             {
                                 length = 1;
                             }
                             else if (fileInfoString.Length > 10 && fileInfoString.Length < 99)
                             {
                                 length = 2;
                             }
                             else
                             {
                                 length = 3;
                             }

                             string len = length.ToString();


                             int fileInfoStringByteCount = Encoding.UTF8.GetByteCount(fileInfoString);

                             byte[] fileSizeHeader = new byte[fileInfoStringByteCount];
                             fileSizeHeader = Encoding.UTF8.GetBytes(fileInfoString.Length.ToString());

                             await stream.WriteAsync(Encoding.UTF8.GetBytes(len), 0, len.Length);

                             await stream.WriteAsync(fileSizeHeader, 0, fileSizeHeader.Length);



                             byte[] fileInfobuffer = new byte[Encoding.UTF8.GetByteCount(fileInfoString)];

                             //                             byte[] fileInfobuffer = new byte[275];


                             Encoding.UTF8.GetBytes(fileInfoString, 0, fileInfoString.Length, fileInfobuffer, 0);


                             await stream.WriteAsync(fileInfobuffer, 0, fileInfobuffer.Length);


                             client.CurrentSendingFileName = fileName;
                             client.CurrentSendingFileSize = FormatFileSize.GetSize(fileSizeInBytes);



                             int bytesRead = 0;
                             long completed = 0;
                             Stopwatch stopwatch = new Stopwatch();

                             Timer timer = new Timer(300);
                             timer.Elapsed += async (sender, e) => await UpdateUI(client, completed, fileSizeInBytes, stopwatch.Elapsed.TotalSeconds);

                             long pg = fileSizeInBytes;


                             using (FileStream fileStream = File.OpenRead(path))
                             {
                                 byte[] buffer = new byte[8192];

                                 timer.Start();
                                 stopwatch.Start();

                                 while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                 {
                                     await stream.WriteAsync(buffer, 0, bytesRead);

                                     pg -= bytesRead;
                                     if (pg < 0) break;

                                     completed += bytesRead;

                                 }



                             }

                             //  await Task.Delay(1500);

                             // byte[] signal = new byte[4];

                             // await stream.ReadAsync(signal, 0, 4);

                             timer.Dispose();
                         }

                     }

                     client.IsSendingFile = false;
                     client.ResetProperties();




                 }
                 catch (Exception ex)
                 {

                     MessageBox.Show(ex.Message);
                 }
             }, TaskCreationOptions.LongRunning);


        }




        private static async Task UpdateUI(TcpClientModel client, long bytesRead, long fileSizeInBytes, double timeInSeconds)
        {
            await Task.Run(() =>
             {

                 double speed = (double)bytesRead / 1024.0 / 1024.0 / timeInSeconds;

                 client.CurrentSendingFileSpeed = $"{Math.Round(speed, 2)} MB/s";

                 double progressPercentage = ((double)bytesRead / fileSizeInBytes) * 100;

                 client.CurrentSendingFilePercentage = progressPercentage;

                 int timeLeftInSeconds = (int)((fileSizeInBytes - bytesRead) / (bytesRead / timeInSeconds));


                 client.CurrentSendingFileTimeLeft = $"{timeLeftInSeconds} seconds";

             });


            //Math.Round((double)bytesRead / 1048576.0, 2)}MB of {Math.Round((double)fileSizeInBytes / 1048576.0, 2)





        }

    }
}