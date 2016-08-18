using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessor : ISourceProcessor
    {
        private bool StopRequested { get; set; }

        #region Constructors

        public MockSourceProcessor()
        {
            this.Id = Guid.NewGuid();
            this.LastAttempt = DateTime.Now;
            this.AttemptTimeoutMs = 30000;
            this.IsActive = false;
        }

        #endregion  

        #region ISourceProcessor Methods

        public Guid Id { get; private set; }

        public DateTime LastAttempt { get; private set; }

        public long AttemptTimeoutMs { get; set; }

        public bool IsActive { get; private set; }

        public Source Config { get; private set; }


        public void Start()
        {
            this.StartCalled = true;
            this.StopRequested = false;
            this.IsActive = true;
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
            this.IsActive = false;
        }

        #endregion

        #region Mock Information

        public bool StartCalled { get; private set; }

        #endregion

    }
}
