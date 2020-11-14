﻿using StateChartsDotNet.CoreEngine;
using System;
using System.Xml.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StateChartsDotNet.CoreEngine.ModelProvider.Xml;
using StateChartsDotNet.CoreEngine.Abstractions;
using System.Collections.Generic;

namespace ConsoleRunner
{
    public class Foo
    {
        public dynamic data;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(
                    builder => builder.AddFilter("Default", LogLevel.Information)
                                      .AddConsole());

            var logger = loggerFactory.CreateLogger("StateChart");

            Task task;

            using (var scope = logger.BeginScope(""))
            {
                //task = RunMicrowave(logger);

                task = RunForeach(logger);
            }

            Task.WaitAll(Task.Delay(5000), task);
        }

        static Task RunForeach(ILogger logger)
        {
            return Run("foreach.xml", logger);
        }

        static Task RunMicrowave(ILogger logger)
        {
            return Run("microwave.xml", logger, async queue =>
            {
                queue.Enqueue(new Message("turn.on"));
                await Task.Delay(500);

                for (var i = 0; i < 5; i++)
                {
                    queue.Enqueue(new Message("time"));
                    await Task.Delay(500);
                }

                queue.Enqueue(new Message("cancel"));
                await Task.Delay(500);
            });
        }

        static Task Run(string xmldoc, ILogger logger, Func<Queue<Message>, Task> action = null)
        {
            var metadata = new XmlModelMetadata(XDocument.Load(xmldoc));

            var queue = new Queue<Message>();

            var runTask = Interpreter.Run(metadata, queue, logger);

            var actionTask = Task.CompletedTask;

            if (action != null)
            {
                actionTask = action.Invoke(queue);
            }

            return Task.WhenAll(runTask, actionTask);
        }
    }
}
