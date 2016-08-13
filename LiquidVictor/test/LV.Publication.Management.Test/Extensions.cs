using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test
{
    public static class Extensions
    {
        public static ILogger CreateLogger(this string logCategory)
        {
            var loggerFactory = new LoggerFactory().AddConsole();
            return loggerFactory.CreateLogger(logCategory);
        }
    }
}
