using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using LV.Publication.Interfaces;

namespace LV.Publication.Management
{
    public class Client
    {
        ILogger _logger;
        IConfigRepository _configRepo;

        public Client(ILogger logger, IConfigRepository configRepo)
        {
            if (configRepo == null)
                throw new ArgumentNullException(nameof(configRepo));

            if (logger == null)
                logger = new NullLogger();

            _logger = logger;
            _configRepo = configRepo;
        }

        public void Process()
        {
            _logger.LogInformation("Begin Process");

            _logger.LogInformation("Fetching Configuration");
            var config = _configRepo.GetConfig();
            _logger.LogInformation("Configuration retrieved");

            _logger.LogInformation("End Process");
        }

    }
}
