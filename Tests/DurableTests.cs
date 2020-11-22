﻿using DurableTask.Emulator;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateChartsDotNet.CoreEngine.DurableTask;
using StateChartsDotNet.CoreEngine.ModelProvider.Fluent.States;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class DurableTests
    {
        private static ILogger _logger;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var loggerFactory = LoggerFactory.Create(
                                    builder => builder.AddFilter("DurableTests", level => true).AddDebug());

            _logger = loggerFactory.CreateLogger("DurableTests");
        }

        [TestMethod]
        public async Task SimpleTransition()
        {
            var x = 1;

            var machine = StateChart.Define("test")
                                    .AtomicState("state1")
                                        .OnEntry()
                                            .Execute(_ => x += 1)
                                            .Attach()
                                        .OnExit()
                                            .Execute(_ => x += 1)
                                            .Attach()
                                        .Transition()
                                            .Target("alldone")
                                            .Attach()
                                        .Attach()
                                    .FinalState("alldone")
                                        .Attach();

            var emulator = new LocalOrchestrationService();

            var service = new DurableStateChartService(machine, emulator, _logger);

            await service.StartAsync();

            var client = new DurableStateChartClient(emulator);

            await client.StartAsync();

            await client.WaitForCompletionAsync(TimeSpan.FromSeconds(60));

            await service.StopAsync();

            Assert.AreEqual(3, x);
        }
    }
}
