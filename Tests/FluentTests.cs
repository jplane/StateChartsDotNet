﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateChartsDotNet;
using StateChartsDotNet.Metadata.Fluent.States;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class FluentTests
    {
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

            var context = new ExecutionContext(machine);

            var interpreter = new Interpreter();

            await interpreter.RunAsync(context);

            Assert.AreEqual(3, x);
        }
    }
}