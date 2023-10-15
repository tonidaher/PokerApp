using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace PokerApp.Data.Logging
{
    public class PokerLogger
    {

        public ILogger GetLogger()
        {
            var log = new LoggerConfiguration()
                           .WriteTo.Console()
                           .WriteTo.File("logs\\myapp.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                           .MinimumLevel.Information()
                           .CreateLogger();

            return log;
        }
    }
}
