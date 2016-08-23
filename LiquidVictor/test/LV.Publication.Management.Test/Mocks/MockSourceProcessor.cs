using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LV.Publication.Entities;

namespace LV.Publication.Management.Test.Mocks
{
    public class MockSourceProcessor : SourceProcessorBase
    {

        public MockSourceProcessor(Entities.Source source):base(source) { }

        public override Enumerations.ProcessorResult DoWork(Source source)
        {
            // Place any work the processor has to do here
            Task.WaitAll(Task.Delay(30));
            return Enumerations.ProcessorResult.FailAndContinue;
        }

    }
}
