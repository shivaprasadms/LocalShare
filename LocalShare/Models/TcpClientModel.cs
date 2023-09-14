using LocalShare.Interfaces;
using LocalShare.ViewModels;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LocalShare.Models
{
    public class TcpClientModel : ViewModel, ITcpClientModel
    {

        public string ClientName { get; set; }

        public string ClientIp { get; set; }

        public TcpClient TcpConnection { get; set; }

        //public System.Windows.Visibility IsSendingFile { get; set; } = System.Windows.Visibility.Visible;

        //public System.Windows.Visibility IsReceivingFile { get; set; } = System.Windows.Visibility.Visible;

        public bool IsSendingFile { get; set; } = true;

        public bool IsReceivingFile { get; set; } = true;

        private string currentSendingFileName = "";

        public string CurrentSendingFileName
        {
            get { return currentSendingFileName; }
            set
            {
                if (currentSendingFileName != value)
                {
                    SetProperty(ref currentSendingFileName, value, nameof(CurrentSendingFileName));
                }
            }
        }

        private string currentReceivingFileName = "";

        public string CurrentReceivingFileName
        {
            get { return currentReceivingFileName; }
            set
            {
                if (currentReceivingFileName != value)
                {
                    SetProperty(ref currentReceivingFileName, value, nameof(CurrentReceivingFileName));
                }
            }
        }

        private string currentSendingFileSize = "";

        public string CurrentSendingFileSize
        {
            get { return currentSendingFileSize; }
            set
            {
                if (currentSendingFileSize != value)
                {
                    SetProperty(ref currentSendingFileSize, value, nameof(CurrentSendingFileSize));
                }
            }
        }


        private string currentReceivingFileSize = "";

        public string CurrentReceivingFileSize
        {
            get { return currentReceivingFileSize; }
            set
            {
                if (currentReceivingFileSize != value)
                {
                    SetProperty(ref currentReceivingFileSize, value, nameof(CurrentReceivingFileSize));
                }
            }
        }



        public Queue<string> FilePathQueue { get; set; }

        public TcpClientModel(string Name, string IP, TcpClient Connection)
        {
            ClientName = Name;
            ClientIp = IP;
            TcpConnection = Connection;

        }


    }
}
