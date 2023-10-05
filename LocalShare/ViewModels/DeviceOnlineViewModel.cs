using LocalShare.Models;
using LocalShare.Services;
using LocalShare.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
            Clients.AnyClientConnected += SetSearchingSpinnerView;

            //Clients.Connections.Add(new TcpClientModel("Pixel 6", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            //Clients.Connections.Add(new TcpClientModel("Pixel 7", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            //Clients.Connections.Add(new TcpClientModel("Pixel 8", "192.168.1.1", new System.Net.Sockets.TcpClient()));

            SendFileCommand = new RelayCommand(ExecuteSendFileCommand);

            SendFolderCommand = new RelayCommand(ExecuteSendFolderCommand);



        }

        private bool searchingSpinner = true;

        public bool SearchingSpinner
        {
            get { return searchingSpinner; }
            set { SetProperty(ref searchingSpinner, value); }
        }


        private async void ExecuteSendFileCommand(object sender)
        {

            var openFileDialog = new OpenFileDialog
            {
                Title = "LocalShare Send File",
                Filter = "All Files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                Multiselect = true
            };


            if (openFileDialog.ShowDialog() == true)
            {

                string[] selectedFilePath = openFileDialog.FileNames;

                var client = (sender as TcpClientModel);

                await FileTransferService.SendToClient(client, new List<Tuple<string, string[]>>() { new Tuple<string, string[]>("/", selectedFilePath) }, false);

            }
        }

        private async void ExecuteSendFolderCommand(object sender)
        {

            var client = (sender as TcpClientModel);

            using (var fbd = new Forms.FolderBrowserDialog())
            {
                Forms.DialogResult result = fbd.ShowDialog();

                var selectedPath = fbd.SelectedPath;

                if (result == Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(selectedPath))
                {

                    string[] rootDirFiles = Directory.GetFiles(selectedPath);

                    List<Tuple<string, string[]>> folder = new();

                    folder.Add(new Tuple<string, string[]>("/" + System.IO.Path.GetFileName(selectedPath), rootDirFiles));

                    var nestedDirectories = Directory.GetDirectories(selectedPath, "*", System.IO.SearchOption.AllDirectories);

                    foreach (var dir in nestedDirectories)
                    {
                        string[] nestedDirFiles = Directory.GetFiles(dir);
                        folder.Add(new Tuple<string, string[]>(dir, nestedDirFiles));

                    }


                    await FileTransferService.SendToClient(client, folder, true);

                }
            }

        }

        private void SetSearchingSpinnerView(object _, bool status)
        {
            SearchingSpinner = !status;
        }




        // test -> NF -> [nes,demo.txt] -> nes -> dem0.txt




    }
}
