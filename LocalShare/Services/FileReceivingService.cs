using LocalShare.Models;
using System;
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
            // TcpClientModel client = ActiveTcpConnections.Instance.Connections.First();

            NetworkStream stream = client.TcpConnection.GetStream();



            byte[] fileInfoBuffer = new byte[128];

            await stream.ReadAsync(fileInfoBuffer, 0, fileInfoBuffer.Length);



            string fileInfo = Encoding.UTF8.GetString(fileInfoBuffer);

            var fileInfoArray = fileInfo.Split(':');

            string fileName = fileInfoArray[0];

            string fileSize = fileInfoArray[1];

            long fileSizeInBytes = long.Parse(fileSize);




            client.CurrentReceivingFileName = fileName;
            client.CurrentReceivingFileSize = fileSize;


            var save = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);


            using (FileStream fileStream = new FileStream(save, FileMode.Create))
            {
                byte[] buffer = new byte[8192]; // You can adjust the buffer size as needed

                int bytesRead;


                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // Write the received bytes to the file
                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    fileSizeInBytes -= bytesRead;
                    if (fileSizeInBytes <= 0) break;
                }

                // Close the file stream when done
                fileStream.Close();
            }





        }


    }
}
