using LocalShare.Interfaces;
using LocalShare.ViewModels;
using System.Collections.Concurrent;
using System.Net.Sockets;

namespace LocalShare.Models
{
    public class TcpClientModel : ViewModel, ITcpClientModel
    {

        public string ClientName { get; set; }

        public string ClientIp { get; set; }

        public TcpClient TcpConnection { get; set; }

        private bool isSendingFile;

        public bool IsSendingFile
        {
            get { return isSendingFile; }
            set { SetProperty(ref isSendingFile, value, nameof(IsSendingFile)); }
        }


        private bool isReceivingFile;

        public bool IsReceivingFile
        {
            get { return isReceivingFile; }
            set { SetProperty(ref isReceivingFile, value, nameof(IsReceivingFile)); }
        }

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

        private string currentSendingFileSpeed;

        public string CurrentSendingFileSpeed
        {
            get { return currentSendingFileSpeed; }
            set
            {
                SetProperty(ref currentSendingFileSpeed, value, nameof(CurrentSendingFileSpeed));
            }
        }

        private double currentReceivingFileSpeed;

        public double CurrentReceivingFileSpeed
        {
            get { return currentReceivingFileSpeed; }
            set
            {
                SetProperty(ref currentReceivingFileSpeed, value, nameof(CurrentReceivingFileSpeed));
            }
        }

        private double currentSendingFilePercentage;

        public double CurrentSendingFilePercentage
        {
            get { return currentSendingFilePercentage; }
            set { SetProperty(ref currentSendingFilePercentage, value, nameof(CurrentSendingFilePercentage)); }
        }

        private int currentReceivingFilePercentage;

        public int CurrentReceivingFilePercentage
        {
            get { return currentReceivingFilePercentage; }
            set { SetProperty(ref currentReceivingFilePercentage, value, nameof(CurrentReceivingFilePercentage)); }
        }

        private string currentSendingFileTimeLeft;

        public string CurrentSendingFileTimeLeft
        {
            get { return currentSendingFileTimeLeft; }
            set { SetProperty(ref currentSendingFileTimeLeft, value, nameof(CurrentSendingFileTimeLeft)); }
        }

        private double currentReceivingFileTimeLeft;

        public double CurrentReceivingFileTimeLeft
        {
            get { return currentReceivingFileTimeLeft; }
            set { SetProperty(ref currentReceivingFileTimeLeft, value, nameof(CurrentReceivingFileTimeLeft)); }
        }







        private ConcurrentQueue<string> FilePathQueue { get; set; }

        public TcpClientModel(string Name, string IP, TcpClient Connection)
        {
            ClientName = Name;
            ClientIp = IP;
            TcpConnection = Connection;

            FilePathQueue = new();

        }

        public void AddFilesToQueue(string path)
        {
            FilePathQueue.Enqueue(path);
        }

        public string PopFileFromQueue()
        {
            string returnValue;
            FilePathQueue.TryDequeue(out returnValue);
            return returnValue;
        }

        public bool IsQueueEmpty()
        {
            return FilePathQueue.IsEmpty;
        }

        public void ResetProperties()
        {

            CurrentSendingFileName = "";
            CurrentSendingFilePercentage = 0;
            CurrentSendingFileName = "";
            CurrentSendingFileSize = "";
            CurrentSendingFileSpeed = "";
            CurrentSendingFileTimeLeft = "";
        }
    }
}
