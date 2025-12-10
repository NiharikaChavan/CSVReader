using System;
using Serilog;

namespace CSVReader.Services
{
    /// <summary>
    /// Very simple logging service.
    /// Single Log(...) method: logs Error when an exception is provided, otherwise Information.
    /// Assumes Serilog is configured elsewhere.
    /// </summary>
    public class LogService : IDisposable
    {
        public void Log(string message, Exception? ex = null)
        {
            if (string.IsNullOrEmpty(message) && ex is null) return;

            try
            {
                if (ex is not null)
                    Serilog.Log.Error(ex, "{Message}", message ?? ex.Message);
                else
                    Serilog.Log.Information("{Message}", message ?? string.Empty);
            }
            catch
            {
                // swallow — logging must not crash the app
            }
        }

        // Do NOT call Log.CloseAndFlush() here — App.OnExit handles that.
        public void Dispose() { }
    }
}

