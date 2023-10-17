using LocalShare.Models;
using LocalShare.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LocalShare.Services
{
    class TcpConnectionManager
    {
        private TcpListener TcpListener;

        private ActiveTcpConnections connections;
        private readonly LocalShareReceiver _localShareReceiver;
        private readonly ILogger<TcpConnectionManager> _logger;

        public TcpConnectionManager(ActiveTcpConnections clients, LocalShareReceiver localShareReceiver, ILogger<TcpConnectionManager> logger)
        {
            connections = clients;
            _localShareReceiver = localShareReceiver;
            _logger = logger;
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
                _logger.LogError(ex, ex.Message);
            }

        }

        private async Task HandleClient(TcpClient client)
        {
            var clientModel = new TcpClientModel("PIXEL", ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), client);
            connections.AddConnection(clientModel);

            await _localShareReceiver.ReceiveFromClient(clientModel);

        }


    }
}
