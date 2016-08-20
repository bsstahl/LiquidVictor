using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Test
{
    public class SourceProcessorBase_Start_Should
    {
        const long _defaultTimeout = 30000;

        [Fact]
        public static void RasiseTheOnStartEvent()
        {
            var source = new Entities.Source(_defaultTimeout);
            var target = new Mocks.MockSourceProcessor(source);
            target.Start();

            try
            {
                Assert.True(target.StartCalled);
            }
            finally
            {
                target.Stop();
            }

        }

    }
}
