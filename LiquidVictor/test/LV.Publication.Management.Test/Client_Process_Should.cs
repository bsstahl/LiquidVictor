﻿using LV.Publication.Management.Test.Extensions;
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
            var configRepo = new Mocks.MockConfigRepository(sourceCount);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);
            target.Process();
            Assert.Equal(sourceCount, factory.TimesCreateCalled);
        }

        [Fact]
        public static void StartEachOfTheSourceProcessors()
        {
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);
            target.Process();
            var sourceProcessors = factory.SourceProcessorsCreated.Select(s => (Mocks.MockSourceProcessor)s);
            Assert.False(sourceProcessors.Any(p => !p.StartCalled));
        }

        [Fact]
        public static void ContinueWhileStopNotCalled()
        {
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var timeoutTask = Task.Delay(250);
            var testTask = Task.Factory.StartNew(() => target.Process());

            Task.WaitAny(testTask, timeoutTask);
            Assert.False(testTask.IsCompleted);
        }

        [Fact]
        public static void ExitOnceStopIsCalled()
        {
            var configRepo = new Mocks.MockConfigRepository(3);
            var factory = new Mocks.MockSourceProcessorFactory();
            var target = (null as Client).Create(configRepo, factory);

            var testTask = Task.Factory.StartNew(() => target.Process());
            var timeoutTask = Task.Delay(250);
            target.Stop();

            Task.WaitAny(testTask, timeoutTask);
            Assert.True(testTask.IsCompleted);
        }
    }
}
