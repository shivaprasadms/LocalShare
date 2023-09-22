﻿using LocalShare.Configuration;
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

        private TcpConnectionManager? TcpConnManager;


        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureServices((hostContext, services) =>
            {

                services.AddSingleton<AppSettingsManager>();
                services.AddSingleton<ActiveTcpConnections>();
                services.AddSingleton<TcpConnectionManager>();
                services.AddSingleton<DeviceOnlineViewModel>();
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));


                services.AddSingleton(serviceProvider => new MainWindow()
                {
                    DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
                }); ;


            }).Build();
        }



        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();

            startupForm.Show();

            base.OnStartup(e);

            TcpConnManager = AppHost.Services.GetRequiredService<TcpConnectionManager>();

            MulticastService.Broadcast(TcpConnManager.GetPort());

            //ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;

            await TcpConnManager.StartListening();

        }




    }
}
