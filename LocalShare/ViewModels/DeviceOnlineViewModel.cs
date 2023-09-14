﻿using LocalShare.Models;
using LocalShare.Services;
using LocalShare.Utility;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Forms = System.Windows.Forms;

namespace LocalShare.ViewModels
{
    class DeviceOnlineViewModel : ViewModel
    {

        public ActiveTcpConnections Clients { get; set; }

        public ICommand SendFileCommand { get; set; }

        public ICommand SendFolderCommand { get; set; }


        public DeviceOnlineViewModel(ActiveTcpConnections clients)
        {
            Clients = clients;

            Clients.Connections.Add(new TcpClientModel("Pixel 6", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            Clients.Connections.Add(new TcpClientModel("Pixel 7", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            Clients.Connections.Add(new TcpClientModel("Pixel 8", "192.168.1.1", new System.Net.Sockets.TcpClient()));

            SendFileCommand = new RelayCommand(ExecuteSendFileCommand);

            SendFolderCommand = new RelayCommand(ExecuteSendFolderCommand);



        }

        private bool searchingSpinner = true;

        public bool SearchingSpinner
        {
            get { return searchingSpinner; }
            set { SetProperty(ref searchingSpinner, value, nameof(SearchingSpinner)); }
        }


        private async void ExecuteSendFileCommand(object sender)
        {

            var openFileDialog = new OpenFileDialog
            {
                Title = "LocalShare Send File",
                Filter = "All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer)
            };


            if (openFileDialog.ShowDialog() == true)
            {

                string selectedFilePath = openFileDialog.FileName;

                var clientIP = (sender as TcpClientModel).ClientIp;

                var client = Clients.Connections.FirstOrDefault(client => client.ClientIp == clientIP);


                await FileTransferService.SendToClient(client, selectedFilePath);



            }
        }

        private async void ExecuteSendFolderCommand(object obj)
        {

            using (var fbd = new Forms.FolderBrowserDialog())
            {
                Forms.DialogResult result = fbd.ShowDialog();

                if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string[] files = Directory.GetFiles(fbd.SelectedPath);

                }
            }

        }








    }
}