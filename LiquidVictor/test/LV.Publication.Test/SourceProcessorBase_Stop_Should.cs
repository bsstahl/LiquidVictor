using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Test
{
    public class SourceProcessorBase_Stop_Should
    {
        const long _defaultTimeout = 30000;

        [Fact]
        public static void InactivateTheProcessor()
        {
            var source = new Entities.Source(_defaultTimeout);
            var target = new Mocks.MockSourceProcessor(source);

            target.Start();
            System.Diagnostics.Debug.Assert(target.IsActive);

            target.Stop();
            Assert.False(target.IsActive);
        }

        [Fact]
        public static void RasiseTheStoppingEvent()
        {
            var source = new Entities.Source(_defaultTimeout);
            var target = new Mocks.MockSourceProcessor(source);

            target.Start();
            System.Diagnostics.Debug.Assert(target.IsActive);

            target.Stop();
            Assert.True(target.StoppingCalled);
        }

        [Fact]
        public static void RasiseTheStoppedEvent()
        {
            var source = new Entities.Source(_defaultTimeout);
            var target = new Mocks.MockSourceProcessor(source);

            target.Start();
            System.Diagnostics.Debug.Assert(target.IsActive);

            target.Stop();
            Assert.True(target.StoppedCalled);
        }

    }
}
