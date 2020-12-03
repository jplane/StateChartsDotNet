﻿using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StateChartsDotNet.Metadata.Xml.States;
using StateChartsDotNet.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StateChartsDotNet.Tests
{
    [TestClass]
    public class XmlTests
    {
        private static ILogger _logger;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            var loggerFactory = LoggerFactory.Create(
                                    builder => builder.AddFilter("XmlTests", level => true).AddDebug());

            _logger = loggerFactory.CreateLogger("XmlTests");
        }

        [TestMethod]
        [TestScaffold]
        public async Task Foreach(ScaffoldFactoryDelegate factory, string _)
        {
            var xmldoc = @"<?xml version='1.0'?>
                           <scxml xmlns='http://www.w3.org/2005/07/scxml'
                                  version='1.0'
                                  datamodel='csharp'
                                  initial='loop'>
                               <datamodel>
                                   <data id='items' expr='new [] { 1, 2, 3, 4, 5 }' />
                                   <data id='sum' expr='0' />
                               </datamodel>
                               <state id='loop'>
                                   <onentry>
                                       <foreach array='items' item='arrayItem'>
                                           <assign location='sum' expr='sum + arrayItem' />
                                           <log expr='&quot;item = &quot; + arrayItem' />
                                       </foreach>
                                   </onentry>
                                   <transition cond='sum &gt;= 15' target='done' />
                               </state>
                               <final id='done'>
                                   <onentry>
                                       <log expr='&quot;sum = &quot; + sum' />
                                   </onentry>
                               </final>
                           </scxml>";

            var machine = new StateChart(XDocument.Parse(xmldoc));

            var scaffold = factory(machine, CancellationToken.None, null);

            var context = scaffold.Item1;

            await scaffold.Item2();

            Assert.AreEqual(15, Convert.ToInt32(context["sum"]));
        }

        [TestMethod]
        [TestScaffold]
        public async Task HttpPost(ScaffoldFactoryDelegate factory, string _)
        {
            var listenerTask = Task.Run(() => InProcWebServer.EchoAsync("http://localhost:4444/"));

            var xmldoc = @"<?xml version='1.0'?>
                           <scxml xmlns='http://www.w3.org/2005/07/scxml'
                                  version='1.0'
                                  datamodel='csharp'>
                               <state id='state1'>
                                   <onentry>
                                       <http-post>
                                           <url>http://localhost:4444/</url>
                                           <body>
                                             { value: 5 }
                                           </body>
                                       </http-post>
                                   </onentry>
                                   <transition target='alldone' />
                               </state>
                               <final id='alldone' />
                           </scxml>";

            var machine = new StateChart(XDocument.Parse(xmldoc));

            var scaffold = factory(machine, CancellationToken.None, _logger);

            var context = scaffold.Item1;

            await scaffold.Item2();

            var json = await listenerTask;

            var content = JsonConvert.DeserializeAnonymousType(json, new { value = default(int) });

            Assert.AreEqual(5, content.value);
        }

        [TestMethod]
        [TestScaffold]
        public async Task HttpGet(ScaffoldFactoryDelegate factory, string _)
        {
            var uri = "http://localhost:4444/";

            var listenerTask = Task.Run(() => InProcWebServer.JsonResultAsync(uri, new { value = 43 }));

            var xmldoc = @"<?xml version='1.0'?>
                           <scxml xmlns='http://www.w3.org/2005/07/scxml'
                                  version='1.0'
                                  datamodel='csharp'>
                               <state id='state1'>
                                   <onentry>
                                       <http-get resultlocation='x'>
                                           <url>http://localhost:4444/</url>
                                       </http-get>
                                   </onentry>
                                   <transition target='alldone' />
                               </state>
                               <final id='alldone' />
                           </scxml>";

            var machine = new StateChart(XDocument.Parse(xmldoc));

            var scaffold = factory(machine, CancellationToken.None, _logger);

            var context = scaffold.Item1;

            await Task.WhenAll(scaffold.Item2(), listenerTask);

            var json = (string)context["x"];

            Assert.IsNotNull(json);

            var content = JsonConvert.DeserializeAnonymousType(json, new { value = default(int) });

            Assert.AreEqual(43, content.value);
        }

        [TestMethod]
        [TestScaffold]
        public async Task Microwave(ScaffoldFactoryDelegate factory, string _)
        {
            var xmldoc = @"<?xml version='1.0'?>
                           <scxml xmlns='http://www.w3.org/2005/07/scxml'
                                  version='1.0'
                                  datamodel='csharp'
                                  initial='off'>

                               <!--  trivial 5 second microwave oven example -->
                               <datamodel>
                                   <data id='cook_time' expr='5'/>
                                   <data id='door_closed' expr='true'/>
                                   <data id='timer' expr='0'/>
                               </datamodel>

                               <state id='off'>
                                   <transition event='turn.on' target='on'/>
                               </state>

                               <state id='on'>
                                   <initial>
                                       <transition target='idle'/>
                                   </initial>

                                   <transition event='turn.off' target='off'/>
                                   <transition cond='timer &gt;= cook_time' target='off'/>

                                   <state id='idle'>
                                       <transition cond='door_closed' target='cooking'/>
                                       <transition event='door.close' target='cooking'>
                                           <assign location='door_closed' expr='true'/>
                                       </transition>
                                   </state>

                                   <state id='cooking'>
                                       <transition event='door.open' target='idle'>
                                           <assign location='door_closed' expr='false'/>
                                       </transition>
                                       <transition event='time'>
                                           <assign location='timer' expr='timer + 1'/>
                                       </transition>
                                   </state>

                               </state>
                           </scxml>";

            var machine = new StateChart(XDocument.Parse(xmldoc));

            var scaffold = factory(machine, CancellationToken.None, _logger);

            var context = scaffold.Item1;

            var task = scaffold.Item2();

            await Task.Delay(1000);

            await context.SendAsync("turn.on");

            for (var i = 0; i < 5; i++)
            {
                await context.SendAsync("time");
                await Task.Delay(200);
            }

            await context.StopAsync();

            await task;

            Assert.AreEqual(5, Convert.ToInt32(context["timer"]));
        }

        [TestMethod]
        [TestScaffold]
        public async Task SimpleParentChild(ScaffoldFactoryDelegate factory, string _)
        {
            var xmldoc = @"<?xml version='1.0'?>
                           <scxml xmlns='http://www.w3.org/2005/07/scxml'
                                  version='1.0'
                                  name='outer'
                                  datamodel='csharp'>
                               <state id='outerState1'>
                                   <invoke>
                                       <param name='x' location='x' />
                                       <content>
                                           <scxml version='1.0'
                                                  name='inner'
                                                  datamodel='csharp'>
                                               <state id='innerState1'>
                                                   <onentry>
                                                       <assign location='x' expr='x * 2' />
                                                   </onentry>
                                                   <transition target='alldone' />
                                               </state>
                                               <final id='alldone'>
                                                   <donedata>
                                                       <param name='innerX' location='x' />
                                                   </donedata>
                                               </final>
                                           </scxml>
                                       </content>
                                       <finalize>
                                           <assign location='innerX' expr='_event.Parameters[&quot;innerX&quot;]' />
                                       </finalize>
                                   </invoke>
                                   <transition event='done.invoke.*' target='alldone' />
                               </state>
                               <final id='alldone' />
                           </scxml>";

            var machine = new StateChart(XDocument.Parse(xmldoc));

            var scaffold = factory(machine, CancellationToken.None, _logger);

            var context = scaffold.Item1;

            context["x"] = 5;

            await scaffold.Item2();

            var x = Convert.ToInt32(context["x"]);

            Assert.AreEqual(5, x);

            var innerX = Convert.ToInt32(context["innerX"]);

            Assert.AreEqual(10, innerX);
        }
    }
}
