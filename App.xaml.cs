using System;
using System.IO;
using System.Windows;
using Serilog;

namespace CSVReader
{
    /// <summary>
    /// Application startup and configuration
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Runs when application starts - sets up logging to Desktop and Files folder
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var desktopLog = Path.Combine(desktop, "CSVReaderApp.log");     

            if (File.Exists(desktopLog))
                File.Delete(desktopLog);       

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(desktopLog)
       
                .CreateLogger();

            Log.Information("Application started");
        }

        /// <summary>
        /// Runs when application closes - flushes and closes the log files
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Application closing");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
