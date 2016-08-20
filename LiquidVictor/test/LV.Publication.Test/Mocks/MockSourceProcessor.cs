using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Test.Mocks
{
    public class MockSourceProcessor : SourceProcessorBase
    {
        public bool StartCalled { get; private set; }

        public MockSourceProcessor(Entities.Source source) : base(source)
        {
            this.StartCalled = false;
            base.Started += new StartEventHandler(OnProcessorStart);
        }

        public void OnProcessorStart(object source, EventArgs e)
        {
            this.StartCalled = true;
        }

        public override void DoWork(Source source)
        {
            // Place any work the processor has to do here
        }

    }
}
