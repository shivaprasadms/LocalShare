using LocalShare.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LocalShare.Services
{
    public class FileTransferService
    {

        public static async Task SendToClient(TcpClientModel client, string path)
        {
            //var client = ActiveTcpConnections.Instance.Connections.FirstOrDefault(conn => conn.ClientIp == clientip);

            /*

             1. get the client object
             2. push the file path to the queue
             3. check if any file is being transferred
             4. if yes push to queue 
            5 if not just start transfer


             */

            try
            {

                NetworkStream stream = client.TcpConnection.GetStream();

                string fileName = Path.GetFileName(path);

                FileInfo fileInfo = new FileInfo(path);

                string fileSize = fileInfo.Length.ToString();

                string fileInfoString = $"{fileName}:{fileSize}:";


                byte[] fileInfobuffer = new byte[Encoding.UTF8.GetByteCount(fileInfoString)];


                Encoding.UTF8.GetBytes(fileInfoString, 0, fileInfoString.Length, fileInfobuffer, 0);


                await stream.WriteAsync(fileInfobuffer, 0, fileInfobuffer.Length);

                client.CurrentSendingFileName = fileName;
                client.CurrentSendingFileSize = fileSize;




                await Task.Delay(2000);

                using (FileStream fileStream = File.OpenRead(path))
                {
                    byte[] buffer = new byte[8192]; // 8 KB buffer




                    //int byt = 0;
                    //byte[] signal = new byte[6];
                    //await stream.ReadAsync(signal, 0, byt);

                    int bytesRead = 0;
                    while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        await stream.WriteAsync(buffer, 0, bytesRead);
                    }

                }

                client.CurrentSendingFileName = " ";


            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

        }

    }
}