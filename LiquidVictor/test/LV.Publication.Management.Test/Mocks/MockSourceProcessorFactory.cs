using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Management.Test.Extensions;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessorFactory : ISourceProcessorFactory
    {
        #region Mock Information

        object _threadMonitor = new object();

        public int TimesCreateCalled { get; private set; }

        private List<ISourceProcessor> _sourceProcessors = new List<ISourceProcessor>();

        public IEnumerable<ISourceProcessor> GetUnstartedProcessors()
        {
            lock (_threadMonitor)
            {
                return _sourceProcessors.Select(s => (Mocks.MockSourceProcessor)s).Where(s => !s.IsActive).ToList();
            }
        }

        public ISourceProcessor GetFirstProcessor()
        {
            lock (_threadMonitor)
            {
                return _sourceProcessors.FirstOrDefault();
            }
        }

        public ISourceProcessor GetSecondProcessor()
        {
            lock (_threadMonitor)
            {
                return _sourceProcessors.Skip(1).Take(1).SingleOrDefault();
            }
        }

        public IEnumerable<ISourceProcessor> GetActiveProcessors()
        {
            lock (_threadMonitor)
            {
                return _sourceProcessors.Where(p => p.IsActive).ToList();
            }
        }


        public ISourceProcessor GetProcessorById(Guid id)
        {
            lock (_threadMonitor)
            {
                return _sourceProcessors.Where(p => p.Id == id).SingleOrDefault();
            }
        }

        public void ChangeRandomProcessorTimeouts(int timeoutMs)
        {
            lock (_threadMonitor)
            {
                foreach (var processor in _sourceProcessors)
                {
                    if (processor.Id.IsEven())
                        processor.AttemptTimeoutMs = timeoutMs;
                }
            }
        }

        #endregion

        #region ISourceProcessorFactory Methods

        public ISourceProcessor GetSource(Entities.Source source)
        {
            ISourceProcessor processor;
            lock (_threadMonitor)
            {
                this.TimesCreateCalled++;
                processor = new MockSourceProcessor();
                _sourceProcessors.Add(processor);
            }
            return processor;
        }

        #endregion
    }
}
