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

        #endregion

        #region ISourceProcessorFactory Methods

        public ISourceProcessor GetSource()
        {
            this.TimesCreateCalled++;
            return new MockSourceProcessor();
        }

        #endregion
    }
}
