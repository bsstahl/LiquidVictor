using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Management.Test.Extensions;
using TestHelperExtensions;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessorFactory : ISourceProcessorFactory
    {
        #region Mock Information

        object _threadMonitor = new object();

        public int TimesCreateCalled { get; private set; }

        // private List<ISourceProcessor> _sourceProcessors = new List<ISourceProcessor>();

        #endregion

        #region ISourceProcessorFactory Methods

        public ISourceProcessor Create(Entities.Source source)
        {
            ISourceProcessor processor;
            lock (_threadMonitor)
            {
                this.TimesCreateCalled++;
                processor = new MockSourceProcessor(source);
            }

            return processor;
        }

        #endregion
    }
}
