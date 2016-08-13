using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LV.Publication.Management
{
    public class Client
    {
        ILogger _logger;

        public Client(ILogger logger)
        {
            if (logger == null)
                logger = new NullLogger();
            _logger = logger;
        }

        public void Process()
        {
            _logger.LogInformation("Begin Process");




            _logger.LogInformation("End Process");
        }

    }
}
