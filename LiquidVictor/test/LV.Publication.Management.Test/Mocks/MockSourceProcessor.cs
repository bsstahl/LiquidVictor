using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessor : ISourceProcessor
    {
        private bool StopRequested { get; set; }

        #region ISourceProcessor Methods

        public void Start()
        {
            this.StartCalled = true;
            this.StopRequested = false;
            Task.Factory.StartNew(() => Process());
        }

        private void Process()
        {
            while (!this.StopRequested)
            { }
        }

        public void Stop()
        {
            this.StopRequested = true;
        }

        #endregion

        #region Mock Information

        public bool StartCalled { get; private set; }

        #endregion

    }
}
