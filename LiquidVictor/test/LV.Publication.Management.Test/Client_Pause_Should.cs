using LV.Publication.Management.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using TestHelperExtensions;

namespace LV.Publication.Management.Test
{
    public class Client_Pause_Should
    {
        const long _defaultTimeout = 30000;


        [Fact]
        public static void MakeThePausedProcessorInactive()
        {
            int processorCount = 5;
            var configRepo = new Mocks.MockConfigRepository(processorCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(100);
            target.Start();

            var processor = factory.GetRandomProcessor();
            System.Diagnostics.Debug.Assert(processor.IsActive);

            try
            {
                target.Pause(processor.Id);
                Assert.False(processor.IsActive);
            }
            finally
            {
                target.Stop();
            }
        }

        [Fact]
        public static void ReduceTheNumberOfActiveProcessors()
        {
            int startCount = 10.GetRandom(5);
            int expectedCount = startCount - 1;
            var configRepo = new Mocks.MockConfigRepository(startCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(100);
            target.Start();

            var processor = factory.GetRandomProcessor();
            System.Diagnostics.Debug.Assert(processor.IsActive);

            try
            {
                target.Pause(processor.Id);
                Assert.Equal(expectedCount, target.ActiveProcessorCount);
            }
            finally
            {
                target.Stop();
            }
        }
    }
}
