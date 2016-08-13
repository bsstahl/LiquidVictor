using LV.Publication.Management.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Management.Test
{
    public class Client_Process_Should
    {
        [Fact]
        public static void RetrieveItsConfigurationFromTheConfigStore()
        {
            var configRepo = new Mocks.MockConfigRepository();
            var target = (null as Client).Create(configRepo);
            target.Process();
            Assert.True(configRepo.GetConfigCalled);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(17)]
        public static void CallSourceProcessorFactoryOnceForEveryConfiguredSource(int sourceCount)
        {
            var configRepo = new Mocks.MockConfigRepository();
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);
            target.Process();
            Assert.Equal(sourceCount, factory.TimesCreateCalled);
        }
    }
}
