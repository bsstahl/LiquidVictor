using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;
using Microsoft.Extensions.Logging;

namespace LV.Publication.Management
{
    public class SourceProcessorCollection : List<ISourceProcessor>
    {
        object _threadMonitor = new object();
        ISourceProcessorFactory _sourceProcessorFactory;
        ILogger _logger;

        public int ActiveProcessorCount
        {
            get
            {
                lock (_threadMonitor)
                    return this.Count(p => p.IsActive);
            }
        }


        public SourceProcessorCollection(ILogger logger, ISourceProcessorFactory sourceProcessorFactory, IEnumerable<Source> sources)
        {
            _logger = logger;
            _sourceProcessorFactory = sourceProcessorFactory;
            Load(sources);
        }

        private void Load(IEnumerable<Source> sources)
        {
            lock (_threadMonitor)
            {
                foreach (var source in sources)
                    this.Add(_sourceProcessorFactory.Create(source));
            }
        }

        internal void Start()
        {
            lock (_threadMonitor)
            {
                foreach (var source in this)
                    source.Start();
            }
        }

        internal void Stop()
        {
            lock (_threadMonitor)
            {
                foreach (var source in this)
                    source.Stop();
            }
        }

        internal void Pause(Guid processorId)
        {
            lock (_threadMonitor)
            {
                var processor = this.Where(p => p.Id == processorId).SingleOrDefault();
                if (processor != null)
                {
                    processor.Pause();
                    _logger.LogInformation("Processor {0} paused", processor.Id);
                }
            }
        }

        internal void KillProcessorsPastTimeout()
        {
            var trouble = new List<ISourceProcessor>();

            _logger.LogInformation("Checking process timeouts");
            lock (_threadMonitor)
            {
                foreach (var processor in this)
                {
                    if (processor.IsActive && processor.LastAttempt.AddMilliseconds(processor.AttemptTimeoutMs) < DateTime.Now)
                    {
                        processor.Stop();
                        trouble.Add(processor);
                        _logger.LogWarning("Processor {0} stopped due to no activity since {1}", processor.Id, processor.LastAttempt);
                    }
                }

                foreach (var processor in trouble)
                {
                    var source = _sourceProcessorFactory.Create(processor.Config);
                    this.Add(source);
                    source.Start();
                    _logger.LogInformation("Processor {0} started to replace processor {1}", source.Id, processor.Id);
                }

                _logger.LogInformation("Process timeout check completed");
            }
        }
    }
}
