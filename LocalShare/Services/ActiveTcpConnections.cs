using LocalShare.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;

namespace LocalShare.Services
{
    public class ActiveTcpConnections
    {

        public ObservableCollection<TcpClientModel> Connections { get; private set; }

        public EventHandler<bool> AnyClientConnected;


        //private static readonly Lazy<ActiveTcpConnections> lazyInstance =
        //new Lazy<ActiveTcpConnections>(() => new ActiveTcpConnections());

        //public static ActiveTcpConnections Instance => lazyInstance.Value;

        public ActiveTcpConnections()
        {
            Connections = new();
            // PollClients();

        }


        public void AddConnection(TcpClientModel client)
        {
            Connections.Add(client);

            AnyClientConnected?.Invoke(EventArgs.Empty, true);

        }


        private void PollClients()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    if (Connections.Count == 0)
                    {
                        AnyClientConnected?.Invoke(EventArgs.Empty, false);
                        await Task.Delay(2000);
                        continue;
                    }

                    foreach (var client in Connections.ToList())
                    {
                        if ((client.TcpConnection.Client.Poll(1, SelectMode.SelectRead) && client.TcpConnection.Client.Available == 0))
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Connections.Remove(client);
                            });
                    }

                    await Task.Delay(1000);

                }

            });
        }

    }
}
