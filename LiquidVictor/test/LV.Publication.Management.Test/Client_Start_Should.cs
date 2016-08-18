using LV.Publication.Management.Test.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LV.Publication.Management.Test
{
    public class Client_Start_Should
    {
        [Fact]
        public static void RetrieveItsConfigurationFromTheConfigStore()
        {
            var configRepo = new Mocks.MockConfigRepository();
            var target = (null as Client).Create(configRepo);
            target.ExecuteToCompletion();
            Assert.True(configRepo.GetConfigCalled);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(17)]
        public static void CallSourceProcessorFactoryOnceForEveryConfiguredSource(int sourceCount)
        {
            var configRepo = new Mocks.MockConfigRepository(sourceCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);
            target.ExecuteToCompletion();
            target.Stop();
            Assert.Equal(sourceCount, factory.TimesCreateCalled);
        }

        [Fact]
        public static void StartEachOfTheSourceProcessors()
        {
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);
            target.ExecuteToCompletion();
            var sourceProcessors = factory.SourceProcessors.Select(s => (Mocks.MockSourceProcessor)s);
            Assert.False(sourceProcessors.Any(p => !p.StartCalled));
        }

        [Fact]
        public static void ContinueWhileStopNotCalled()
        {
            int processorCount = 3;
            var configRepo = new Mocks.MockConfigRepository(processorCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(100);
            target.Start();
            Task.WaitAll(timeoutTask);

            try
            {
                Assert.Equal(processorCount, target.ActiveProcessorCount);
            }
            finally
            {
                target.Stop();
            }
        }

        [Fact]
        public static void ShutdownAllProcessorsOnceStopIsCalled()
        {
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            target.Stop();

            Assert.Equal(0, target.ActiveProcessorCount);
        }

        [Fact]
        public static void RestartTheOnlyProcessorPastItsTimeout()
        {
            int timeoutMs = 30;
            var configRepo = new Mocks.MockConfigRepository(1);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            var originalProcessor = factory.SourceProcessors.First();
            originalProcessor.AttemptTimeoutMs = timeoutMs;
            Task.WaitAll(Task.Delay(timeoutMs));
            var finalProcessor = factory.SourceProcessors.First();

            target.Stop();

            Assert.NotEqual(originalProcessor.Id, finalProcessor.Id);
        }

        [Fact]
        public static void RestartAProcessorPastItsTimeout()
        {
            int timeoutMs = 30;
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            var originalProcessor = factory.SourceProcessors.Skip(1).Take(1).SingleOrDefault();
            originalProcessor.AttemptTimeoutMs = timeoutMs;
            Task.WaitAll(Task.Delay(timeoutMs));
            var processorWithOriginalId = factory.GetProcessorById(originalProcessor.Id);

            target.Stop();

            Assert.Null(processorWithOriginalId);
        }

        [Fact]
        public static void MaintainsTheCorrectNumberOfProcessors()
        {
            int processorCount = 7;
            int timeoutMs = 30;
            var configRepo = new Mocks.MockConfigRepository(processorCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            foreach (var processor in factory.SourceProcessors)
            {
                if (processor.Id.IsEven())
                    processor.AttemptTimeoutMs = timeoutMs;
            }

            Task.WaitAll(Task.Delay(timeoutMs));
            target.Stop();

            Assert.Equal(processorCount, factory.SourceProcessors.Count());
        }

    }
}
