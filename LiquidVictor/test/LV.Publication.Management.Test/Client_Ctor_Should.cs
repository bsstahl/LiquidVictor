using LV.Publication.Interfaces;
using LV.Publication.Management.Test.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Management.Test
{
    public class Client_Ctor_Should
    {
        public Client_Ctor_Should()
        {
        }

        [Fact]
        public static void ExecutesSuccessfullyWithNullLogger()
        {
            ILogger logger = null;
            var target = (null as Client).Create(logger);
            target.Process();
        }

        [Fact]
        public static void ExecutesSuccessfullyWithLogger()
        {
            var target = (null as Client).Create();
            target.Process();
        }

        [Fact]
        public static void ThrowsArgumentNullExceptionIfConfigRepoNotProvided()
        {
            IConfigRepository configRepo = null;
            Assert.Throws(typeof(ArgumentNullException), () => (null as Client).Create(configRepo));
        }

        [Fact]
        public static void ThrowsArgumentNullExceptionIfSourceProcessorFactoryNotProvided()
        {
            IConfigRepository configRepo = new Mocks.MockConfigRepository();
            ISourceProcessorFactory factory = null;
            Assert.Throws(typeof(ArgumentNullException), () => (null as Client).Create(configRepo, factory));
        }
    }
}
