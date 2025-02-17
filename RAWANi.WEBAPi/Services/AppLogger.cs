using RAWANi.WEBAPi.Application.Services;
using Serilog;

namespace RAWANi.WEBAPi.Services
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private readonly Serilog.ILogger _logger;

        public AppLogger()
        {
            // Automatically enrich logs with the SourceContext property for class name
            _logger = Log.ForContext<T>();
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger.Information(message, args);
        }


        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
        public void LogWarning(string message, params object[] args)
        {
            _logger.Warning(message, args);
        }
        public void LogWarning(string message, Exception ex = null)
        {
            if (ex == null)
                _logger.Warning(message);
            else
                _logger.Warning(ex, message);
        }




        public void LogError(string message, Exception ex = null)
        {
            if (ex == null)
                _logger.Error(message);
            else
                _logger.Error(ex, message);
        }

        

        public void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger?.Debug(message, args);
        }
    }
}
