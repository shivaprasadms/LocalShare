using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalShare.Utility
{
    class LocalNetworkUtility
    {
        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        //public static IPEndPoint CreateLocalIPEndPoint(int portNumber)
        //{
        //    IPAddress localIpAddress = GetLocalIpAddress();

        //    if (localIpAddress == null)
        //    {
        //        throw new InvalidOperationException("Local IP address not found.");
        //    }

        //    return new IPEndPoint(localIpAddress, portNumber);
        //}
    }
}
