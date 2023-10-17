using LocalShare.Interfaces;
using LocalShare.ViewModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;

namespace LocalShare.Models
{
    public class TcpClientModel : ViewModel, ITcpClientModel
    {

        private readonly object _lock = new object();

        #region "Properties for UI"

        public string ClientName { get; set; }

        public string ClientIp { get; set; }

        public TcpClient TcpConnection { get; set; }

        private bool isSendingFile;

        public bool IsSendingFile
        {
            get
            {
                lock (_lock)
                {
                    return isSendingFile;
                }
            }
            set
            {
                lock (_lock)
                {
                    SetProperty(ref isSendingFile, value, nameof(IsSendingFile));
                }
            }
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

        private string currentReceivingFileSpeed;

        public string CurrentReceivingFileSpeed
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

        private double currentReceivingFilePercentage;

        public double CurrentReceivingFilePercentage
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

        private string currentReceivingFileTimeLeft;

        public string CurrentReceivingFileTimeLeft
        {
            get { return currentReceivingFileTimeLeft; }
            set { SetProperty(ref currentReceivingFileTimeLeft, value, nameof(CurrentReceivingFileTimeLeft)); }
        }


        #endregion

        private ConcurrentQueue<Tuple<string, string[]>> FilePathQueue { get; set; }



        public TcpClientModel(string Name, string IP, TcpClient Connection)
        {
            ClientName = Name;
            ClientIp = IP;
            TcpConnection = Connection;

            FilePathQueue = new();

        }

        public void AddFilesToQueue(List<Tuple<string, string[]>> fileList)
        {

            foreach (var path in fileList)
                FilePathQueue.Enqueue(path);
        }

        public Tuple<string, string[]> PopFileFromQueue()
        {
            Tuple<string, string[]> returnValue;
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
