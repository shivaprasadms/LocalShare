using LocalShare.Configuration;
using LocalShare.Interfaces;
using LocalShare.Services;
using LocalShare.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace LocalShare
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private TcpConnectionManager? TcpConnManager;
        readonly ILogger<App> _logger;


        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                       .ConfigureServices((hostContext, services) => RegisterServices(hostContext, services))
                       .Build();

            TcpConnManager = AppHost.Services.GetRequiredService<TcpConnectionManager>();
            _logger = AppHost.Services.GetRequiredService<ILogger<App>>();
        }

        private void RegisterServices(HostBuilderContext hostContext, IServiceCollection services)
        {

            services.AddSingleton<AppConfiguration>();
            services.AddSingleton<ActiveTcpConnections>();
            services.AddSingleton<TcpConnectionManager>();
            services.AddSingleton<DeviceOnlineViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFile("app.log", append: true);
                loggingBuilder.SetMinimumLevel(LogLevel.Information);

            });
            services.AddSingleton(serviceProvider => new MainWindow()
            {
                DataContext = serviceProvider.GetRequiredService<MainWindowViewModel>(),
            });

        }



        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            await AppHost!.StartAsync();

            var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
            startupForm.Show();



            await InitiateAsync();





            //ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;

            await TcpConnManager.StartListening();

        }


        private async Task InitiateAsync()
        {
            ManageUnhandledExceptions();
            MulticastService.Broadcast(TcpConnManager.GetPort());


        }

        private void ManageUnhandledExceptions()
        {
            DispatcherUnhandledException += (_, e) => _logger.LogError(e.Exception, e.Exception.Message);
            TaskScheduler.UnobservedTaskException += (_, e) => _logger.LogError(e.Exception, e.Exception.Message);
            AppDomain.CurrentDomain.UnhandledException += (_, e) => _logger.LogError(e.ExceptionObject.ToString(), e.IsTerminating);
        }




    }
}
