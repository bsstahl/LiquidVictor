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
        const long _defaultTimeout = 30000;
        const int _additionalDelayMs = 5;

        [Fact]
        public static void RetrieveItsConfigurationFromTheConfigStore()
        {
            var configRepo = new Mocks.MockConfigRepository(0);
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
            target.Start();
            var unstartedProcessors = target.GetInactiveProcessors();
            try
            {
                Assert.False(unstartedProcessors.Any());
            }
            finally
            {
                target.Stop();
            }
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
            var timeouts = new long[] { timeoutMs };
            var configRepo = new Mocks.MockConfigRepository(timeouts);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            var originalProcessor = target.GetActiveProcessors().Single();
            var originalProcessorId = originalProcessor.Id;

            Task.WaitAll(Task.Delay(timeoutMs + _additionalDelayMs));
            var finalProcessorId = target.GetActiveProcessors().Single().Id;

            target.Stop();

            Assert.NotEqual(originalProcessorId, finalProcessorId);
        }

        [Fact]
        public static void StopAProcessorPastItsTimeout()
        {
            int timeoutMs = 30;
            var timeouts = new long[] { _defaultTimeout, timeoutMs, _defaultTimeout };
            var configRepo = new Mocks.MockConfigRepository(timeouts);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();
            var originalProcessor = target.GetActiveProcessorWithTimeout(timeoutMs).Single();
            Task.WaitAll(Task.Delay(timeoutMs + _additionalDelayMs));
            var processorWithOriginalId = target.GetProcessorById(originalProcessor.Id);

            try
            {
                Assert.False(processorWithOriginalId.IsActive);
            }
            finally
            {
                target.Stop();
            }
        }

        [Fact]
        public static void MaintainsTheCorrectNumberOfProcessors()
        {
            int timeoutMs = 30;
            var timeouts = new long[] { _defaultTimeout, timeoutMs, _defaultTimeout, timeoutMs, _defaultTimeout, timeoutMs, _defaultTimeout };
            var configRepo = new Mocks.MockConfigRepository(timeouts);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            target.Start();

            Task.WaitAll(Task.Delay(timeoutMs + _additionalDelayMs));
            var actualProcessorCount = target.ActiveProcessorCount;

            target.Stop();

            Assert.Equal(timeouts.Count(), actualProcessorCount);
        }

    }
}
