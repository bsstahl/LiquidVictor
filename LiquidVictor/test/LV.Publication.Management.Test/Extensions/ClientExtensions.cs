using LV.Publication.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Extensions
{
    public static class ClientExtensions
    {
        public static Client Create(this Client ignore)
        {
            var logger = "TestLogs".CreateLogger();
            return ignore.Create(logger);
        }

        public static Client Create(this Client ignore, ILogger logger)
        {
            var configRepo = new Mocks.MockConfigRepository();
            return ignore.Create(logger, configRepo);
        }

        public static Client Create(this Client ignore, IConfigRepository configRepo)
        {
            var logger = "TestLogs".CreateLogger();
            return ignore.Create(logger, configRepo);
        }

        public static Client Create(this Client ignore, ILogger logger, IConfigRepository configRepo)
        {
            return new Client(logger, configRepo);
        }
    }
}
