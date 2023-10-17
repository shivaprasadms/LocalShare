using LocalShare.Models;
using LocalShare.Utility;
using System;
using System.Threading.Tasks;

namespace LocalShare.Helpers
{
    internal class DisplayInfo
    {
        public static Task UpdateCurrentSendingFileInfo(TcpClientModel client, string fileName, long fileSizeInBytes)
        {
            var task = Task.Run(() =>
            {
                client.CurrentSendingFileName = fileName;
                client.CurrentSendingFileSize = FormatFileSize.GetSize(fileSizeInBytes);
            });

            return task;
        }

        public static Task UpdateCurrentReceivingFileInfo(TcpClientModel client, string fileName, long fileSizeInBytes)
        {
            var task = Task.Run(() =>
            {
                client.CurrentReceivingFileName = fileName;
                client.CurrentReceivingFileSize = FormatFileSize.GetSize(fileSizeInBytes);
            });

            return task;
        }

        public static async Task UpdateCurrentSendingFileProgress(TcpClientModel client, long bytesRead, long fileSizeInBytes, double timeInSeconds)
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
        }

        public static async Task UpdateCurrentReceivingFileProgress(TcpClientModel client, long bytesRead, long fileSizeInBytes, double timeInSeconds)
        {
            await Task.Run(() =>
            {
                double speed = (double)bytesRead / 1024.0 / 1024.0 / timeInSeconds;

                client.CurrentReceivingFileSpeed = $"{Math.Round(speed, 2)} MB/s";

                double progressPercentage = ((double)bytesRead / fileSizeInBytes) * 100;

                client.CurrentReceivingFilePercentage = progressPercentage;

                int timeLeftInSeconds = (int)((fileSizeInBytes - bytesRead) / (bytesRead / timeInSeconds));

                client.CurrentReceivingFileTimeLeft = $"{timeLeftInSeconds} seconds";

            });
        }


    }
}
