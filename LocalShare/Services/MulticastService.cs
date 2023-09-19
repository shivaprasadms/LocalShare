using System;
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
                using (UdpClient udpClient = new UdpClient(52345))
                {
                    IPAddress MulticastAddress = IPAddress.Parse("226.1.1.1");

                    udpClient.EnableBroadcast = true;

                    IPEndPoint endPoint = new IPEndPoint(MulticastAddress, 52345);

                    udpClient.JoinMulticastGroup(MulticastAddress);

                    while (true)
                    {
                        string message = $"LocalShare TCP Port:{tcpPort}";

                        byte[] data = Encoding.UTF8.GetBytes(message);
                        udpClient.Send(data, data.Length, endPoint);
                        await Task.Delay(1000);
                    }

                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
