﻿using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Debugger;
using StateChartsDotNet.Common.Model.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace StateChartsDotNet.DurableFunction.Client
{
    public static class StateMachineExtensions
    {
        public static Task<string> StartNewStateMachineAsync(this IDurableClient client,
                                                             IStateChartMetadata definition,
                                                             DebuggerInfo debugInfo = null)
        {
            return StartNewStateMachineAsync(client, definition, null, debugInfo);
        }

        public static Task<string> StartNewStateMachineAsync(this IDurableClient client,
                                                             IStateChartMetadata definition,
                                                             IDictionary<string, object> arguments,
                                                             DebuggerInfo debugInfo = null)
        {
            client.CheckArgNull(nameof(client));
            definition.CheckArgNull(nameof(definition));

            var payload = GetPayload(definition, arguments, debugInfo);

            Debug.Assert(payload != null);

            return client.StartNewAsync("statemachine-orchestration", payload);
        }

        internal static StateMachineRequestPayload GetPayload(IStateChartMetadata definition,
                                                              IDictionary<string, object> arguments,
                                                              DebuggerInfo debugInfo)
        {
            Debug.Assert(definition != null);

            var format = StateMachineDefinitionFormat.Fluent;

            if (definition is Metadata.Json.States.StateChart)
            {
                format = StateMachineDefinitionFormat.Json;
            }

            return new StateMachineRequestPayload
            {
                StateMachineDefinition = definition.ToJson(),
                Format = format,
                Arguments = arguments == null ? null : new Dictionary<string, object>(arguments),
                DebugInfo = debugInfo
            };
        }
    }
}