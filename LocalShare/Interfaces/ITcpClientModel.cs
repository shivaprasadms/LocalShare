using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        Queue<string> FilePathQueue { get; set; }
    }
}
