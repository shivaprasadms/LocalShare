using LocalShare.Models;
using LocalShare.Utility;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LocalShare.Services
{
    class TcpConnectionManager
    {
        private TcpListener TcpListener;

        private ActiveTcpConnections connections;

        public TcpConnectionManager(ActiveTcpConnections clients)
        {
            connections = clients;
            TcpListener = new TcpListener(LocalNetworkUtility.GetLocalIpAddress(), 0);
            TcpListener.Start();

        }

        public int GetPort()
        {
            IPEndPoint endPoint = TcpListener.LocalEndpoint as IPEndPoint;
            return endPoint.Port;
        }



        public async Task StartListening()
        {

            try
            {
                while (true)
                {
                    TcpClient client = await TcpListener.AcceptTcpClientAsync();

                    HandleClient(client);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private async Task HandleClient(TcpClient client)
        {
            try
            {
                var clientModel = new TcpClientModel("PIXEL", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), client);
                connections.AddConnection(clientModel);



                await FileReceivingService.ReceiveFromClient(clientModel);




            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client handling error: {ex.Message}");
            }
        }


    }
}
