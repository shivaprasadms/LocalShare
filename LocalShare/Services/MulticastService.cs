using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LocalShare.Services
{
    class MulticastService
    {

        
        public static void Broadcast(int tcpPort)
        {
            Task.Factory.StartNew(async () =>
            {
                using (UdpClient udpClient = new UdpClient(42345))
                {
                    IPAddress MulticastAddress = IPAddress.Parse("239.0.0.1");
                    udpClient.JoinMulticastGroup(MulticastAddress);
                    IPEndPoint endPoint = new IPEndPoint(MulticastAddress, 42345);

                    while (true)
                    {
                        string message = $"LocalShare TCP Port:{tcpPort}";

                        byte[] data = Encoding.UTF8.GetBytes(message);
                        udpClient.Send(data, data.Length, endPoint);
                        await Task.Delay(2000);
                    }

                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
