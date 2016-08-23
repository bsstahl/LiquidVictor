using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Test.Mocks
{
    public class MockSourceProcessor : SourceProcessorBase
    {
        public bool StartingCalled { get; set; }

        public bool StartedCalled { get; private set; }

        public bool StoppingCalled { get; set; }

        public bool StoppedCalled { get; set; }



        public MockSourceProcessor(Entities.Source source) : base(source)
        {
            this.StartingCalled = false;
            this.StartedCalled = false;
            this.StoppingCalled = false;
            this.StoppedCalled = false;

            base.Started += SourceProcessor_Started;
            base.Starting += SourceProcessor_Starting;
            base.Stopping += SourceProcessor_Stopping;
            base.Stopped += SourceProcessor_Stopped;
        }

        private void SourceProcessor_Stopped(object sender, EventArgs args)
        {
            this.StoppedCalled = true;
        }

        private void SourceProcessor_Stopping(object sender, EventArgs args)
        {
            this.StoppingCalled = true;
        }

        private void SourceProcessor_Starting(object sender, EventArgs args)
        {
            this.StartingCalled = true;
        }

        public void SourceProcessor_Started(object source, EventArgs e)
        {
            this.StartedCalled = true;
        }


        public override Enumerations.ProcessorResult DoWork(Source source)
        {
            // Place any work the processor has to do here
            Task.WaitAll(Task.Delay(100));
            return Enumerations.ProcessorResult.FailAndContinue;
        }

    }
}
