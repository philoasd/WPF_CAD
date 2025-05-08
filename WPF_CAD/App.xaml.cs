using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using System.Configuration;
using System.Data;
using System.IO;
using System.Threading;
using System.Windows;
using TouchSocket.Core;
using LogLib;
using WPF_CAD.ViewModes;
using WPF_CAD.Modes;
using WPF_CAD.Views;

namespace WPF_CAD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        private static Mutex? Mutex { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check if another instance of the application is already running
            var appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Mutex = new Mutex(true, appName, out bool createdNew);
            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                Current.Shutdown();
            }

            // Load NLog configuration from nlog.config file
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "nlog.config");
            LogManager.Configuration = new XmlLoggingConfiguration(logPath);

            // Check and create log folder if it doesn't exist
            string logFolderPath = "./Log/";
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
            else
            {
                // keep 7 days ago log files
                DirectoryInfo di = new DirectoryInfo(logFolderPath);
                List<FileInfo> files = di.GetFiles().OrderBy(p => p.CreationTime).ToList();
                int maxLogFiles = 7;
                if (files.Count > maxLogFiles)
                {
                    for (int i = 0; i < files.Count - maxLogFiles; i++)
                    {
                        files[i].Delete();
                    }
                }
            }

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            //var loginWindow = ServiceProvider.GetRequiredService<LoginWinodw>();
            //loginWindow.ShowDialog();

            //if (loginWindow.IsLoginSuccess)
            //{
            ServiceProvider.GetRequiredService<MainWindow>().ShowDialog();
            //}
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindow>();
            services.AddTransient<MainWindowViewMode>();
            
            services.AddSingleton<ProcessMode>();

            services.AddTransient<DrawingPropertiesWindowViewMode>();
            services.AddTransient<DrawingPropertiesWindow>();
            //services.AddSingleton<LoginWinodw>();
        }

        #region Exception Handling
        public App()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // 自定义处理UI线程未捕获的异常
            Log.LogError("未捕获到的UI线程异常!" + e.Exception);
            e.Handled = true;
            MessageBox.Show("An error occurred, please check the log file for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 自定义处理非UI线程未捕获的异常
            Log.LogError("未捕获到的非UI线程异常!" + (System.Exception)e.ExceptionObject);
            MessageBox.Show("An error occurred, please check the log file for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            // 自定义处理Task任务异常
            Log.LogError("未捕获到的Task任务异常!" + e.Exception);
            e.SetObserved();
            MessageBox.Show("An error occurred, please check the log file for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
        }
        #endregion
    }

}
