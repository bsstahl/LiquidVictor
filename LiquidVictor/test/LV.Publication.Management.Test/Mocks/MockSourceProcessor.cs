using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessor : ISourceProcessor
    {
        #region ISourceProcessor Methods

        public void Start()
        {
            this.StartCalled = true;
        }

        #endregion

        #region Mock Information

        public bool StartCalled { get; private set; }

        #endregion

    }
}
