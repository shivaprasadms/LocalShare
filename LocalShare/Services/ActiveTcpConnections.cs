using LocalShare.Models;
using System.Collections.ObjectModel;

namespace LocalShare.Services
{
    public class ActiveTcpConnections
    {

        public ObservableCollection<TcpClientModel> Connections { get; private set; }

        //private static readonly Lazy<ActiveTcpConnections> lazyInstance =
        //new Lazy<ActiveTcpConnections>(() => new ActiveTcpConnections());

        //public static ActiveTcpConnections Instance => lazyInstance.Value;

        public ActiveTcpConnections()
        {
            Connections = new();
        }


        public void AddConnection(TcpClientModel client)
        {
            Connections.Add(client);

        }













    }
}
