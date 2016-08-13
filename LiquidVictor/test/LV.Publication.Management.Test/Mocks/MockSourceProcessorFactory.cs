using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessorFactory: ISourceProcessorFactory
    {
        #region Mock Information

        public int TimesCreateCalled { get; private set; }

        private List<ISourceProcessor> _sourceProcessors = new List<ISourceProcessor>();
        public IEnumerable<ISourceProcessor> SourceProcessorsCreated
        {
            get { return _sourceProcessors; }
        }

        #endregion

        #region ISourceProcessorFactory Methods

        public ISourceProcessor GetSource()
        {
            this.TimesCreateCalled++;
            var processor = new MockSourceProcessor();
            _sourceProcessors.Add(processor);
            return processor;
        }

        #endregion
    }
}
