using LV.Publication.Management.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TestHelperExtensions;
using LV.Publication.Interfaces;

namespace LV.Publication.Management.Test
{
    public class Client_Unpause_Should
    {
        const long _defaultTimeout = 30000;


        [Fact]
        public static void MakeTheUnpausedProcessorActive()
        {
            int processorCount = 5;
            var configRepo = new Mocks.MockConfigRepository(processorCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(100);
            target.Start();

            var processor = target.GetRandomProcessor();
            System.Diagnostics.Debug.Assert(processor.IsActive);

            try
            {
                target.Pause(processor.Id);
                System.Diagnostics.Debug.Assert(!processor.IsActive);
                target.Unpause(processor.Id);
                Assert.True(processor.IsActive);
            }
            finally
            {
                target.Stop();
            }
        }

        [Fact]
        public static void IncreaseTheNumberOfActiveProcessors()
        {
            int startCount = 15.GetRandom(10);
            int intermediateCount = startCount - 2;
            int expectedCount = startCount - 1;
            var configRepo = new Mocks.MockConfigRepository(startCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(100);
            target.Start();

            var processor1 = target.GetRandomProcessor();
            ISourceProcessor processor2;
            do
                processor2 = target.GetRandomProcessor();
            while (processor1.Equals(processor2));

            if (!(processor1.Pause() && processor2.Pause()))
                System.Diagnostics.Debug.Assert(false, "Could not pause the processors");

            System.Diagnostics.Debug.Assert(target.ActiveProcessorCount == intermediateCount, "The correct # of processors are not active");

            try
            {
                target.Unpause(processor2.Id);
                Assert.Equal(expectedCount, target.ActiveProcessorCount);
            }
            finally
            {
                target.Stop();
            }
        }
    }
}
