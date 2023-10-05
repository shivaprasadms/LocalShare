using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LocalShare.Interfaces
{
    interface ITcpClientModel
    {
        string ClientName { get; set; }

        string ClientIp { get; set; }

        TcpClient TcpConnection { get; set; }

        bool IsSendingFile { get; set; }

        bool IsReceivingFile { get; set; }

        string CurrentSendingFileName { get; set; }

        string CurrentReceivingFileName { get; set; }

        string CurrentSendingFileSize { get; set; }

        string CurrentReceivingFileSize { get; set; }

        //ConcurrentQueue<Tuple<string, string[]>> FilePathQueue { get; set; }

        void AddFilesToQueue(List<Tuple<string, string[]>> path);

        void ResetProperties();

        Tuple<string, string[]> PopFileFromQueue();


    }
}
