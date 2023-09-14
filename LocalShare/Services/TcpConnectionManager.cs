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


            TcpClient client;

            try
            {
                client = await TcpListener.AcceptTcpClientAsync();


                await HandleClientAsync(client);




            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.Message);
            }





        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                var obj = new TcpClientModel("PIXEL", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), client);
                connections.AddConnection(obj);

                await FileReceivingService.ReceiveFromClient(obj);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Client handling error: {ex.Message}");
            }
        }


    }
}
