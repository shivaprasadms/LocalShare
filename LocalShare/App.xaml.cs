using LocalShare.Interfaces;
using LocalShare.Services;
using LocalShare.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace LocalShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private TcpConnectionManager? TcpManager;


        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<ActiveTcpConnections>();
                services.AddSingleton<TcpConnectionManager>();
                services.AddSingleton<DeviceOnlineViewModel>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));

                services.AddSingleton<MainWindow>(serviceProvider => new MainWindow
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>()
                }); ;


            }).Build();
        }



        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            // startupForm.DataContext = AppHost.Services.GetRequiredService<DeviceOnlineViewModel>();
            startupForm.Show();

            base.OnStartup(e);

            TcpManager = AppHost.Services.GetRequiredService<TcpConnectionManager>(); // error here because this constructor needs activetcpconnection

            MulticastService.Broadcast(TcpManager.GetPort());

            await TcpManager.StartListening();



        }





    }
}
