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
        ISourceProcessorFactory _sourceProcessorFactory;

        SourceProcessorCollection _processors;

        bool _stopCalled = false;

        public int ActiveProcessorCount
        {
            get { return _processors.ActiveProcessorCount; }
        }

        public Client(ILogger logger, IConfigRepository configRepo, ISourceProcessorFactory sourceProcessorFactory)
        {
            if (configRepo == null)
                throw new ArgumentNullException(nameof(configRepo));

            if (sourceProcessorFactory == null)
                throw new ArgumentNullException(nameof(sourceProcessorFactory));

            if (logger == null)
                logger = new NullLogger();

            _logger = logger;
            _configRepo = configRepo;
            _sourceProcessorFactory = sourceProcessorFactory;
        }

        public void Start()
        {
            _logger.LogInformation("Begin Process");

            _logger.LogInformation("Fetching Configuration");
            var config = _configRepo.GetConfig();
            _logger.LogInformation("Configuration retrieved");

            _logger.LogInformation("Creating source processors");
            _processors = new SourceProcessorCollection(_sourceProcessorFactory, config.Sources);
            _logger.LogInformation("Created {0} source processors", config.Sources.Count());
            _processors.Start();
            _logger.LogInformation("Source processors started");

            Task.Factory.StartNew(() => Monitor());
        }

        public void Stop()
        {
            _stopCalled = true;
            _processors.Stop();
        }

        private void Monitor()
        {
            while (!_stopCalled)
            { }

            _logger.LogInformation("End Process");
        }
    }
}
