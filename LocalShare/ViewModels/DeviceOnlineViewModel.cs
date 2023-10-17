using LocalShare.Interfaces;
using LocalShare.Models;
using LocalShare.Services;
using LocalShare.Utility;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace LocalShare.ViewModels
{
    public class DeviceOnlineViewModel : ViewModel
    {

        public ActiveTcpConnections Clients { get; set; }
        public ICommand SendFileCommand { get; set; }
        public ICommand SendFolderCommand { get; set; }
        private readonly ILocalShareTransferService _localShareTransfer;
        private readonly IDialogService _dialogService;

        private bool searchingSpinner = true;


        public bool SearchingSpinner
        {
            get { return searchingSpinner; }
            set { SetProperty(ref searchingSpinner, value); }
        }


        public DeviceOnlineViewModel(ActiveTcpConnections clients, ILocalShareTransferService localShareTransfer, IDialogService dialogService)
        {
            Clients = clients;
            _localShareTransfer = localShareTransfer;
            _dialogService = dialogService;
            Clients.AnyClientConnected += SetSearchingSpinnerView;

            // Clients.Connections.Add(new TcpClientModel("Pixel 6", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            //Clients.Connections.Add(new TcpClientModel("Pixel 7", "192.168.1.1", new System.Net.Sockets.TcpClient()));
            //Clients.Connections.Add(new TcpClientModel("Pixel 8", "192.168.1.1", new System.Net.Sockets.TcpClient()));

            SendFileCommand = new RelayCommand(ExecuteSendFileCommand);
            SendFolderCommand = new RelayCommand(ExecuteSendFolderCommand);

        }




        private async void ExecuteSendFileCommand(object sender)
        {

            var openFileDialog = new Microsoft.Win32.OpenFileDialog
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

                await _localShareTransfer.SendFilesToClient(client, selectedFilePath);

            }
        }

        private async void ExecuteSendFolderCommand(object sender)
        {

            var client = (sender as TcpClientModel);

            try
            {

                var folderBrowser = new OpenFolderDialog()
                {
                    Title = "LocalShare Send Folder",
                    InitialFolder = "C:\\",
                    Multiselect = true,
                };


                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    var selectedFolders = folderBrowser.FoldersPaths;

                    await _localShareTransfer.SendFolderToClient(client, selectedFolders.ToArray());
                }

            }
            catch (Exception ex)
            {
                _dialogService.ShowMessage(ex.Message);
            }

        }

        private void SetSearchingSpinnerView(object _, bool status)
        {
            SearchingSpinner = !status;

        }



    }
}
