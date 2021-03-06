﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSM.Common;
using DSM.Common.Model.States;
using System.Diagnostics;
using DSM.Common.Observability;

namespace DSM.FunctionClient
{
    /// <summary>
    /// Input type for state machine orchestrations.
    /// </summary>
    public sealed class StateMachinePayload
    {
        /// <summary>
        /// Provides optional observability information for the state machine runtime.
        /// </summary>
        [JsonProperty("observables")]
        public Instruction[] Observables { get; set; }

        /// <summary>
        /// Initial execution data for the state machine runtime.
        /// </summary>
        [JsonProperty("input")]
        public object Input { get; set; }

        /// <summary>
        /// Unique identifier for a state machine definition registered in the Durable Functions app.
        /// </summary>
        [JsonProperty("statemachineid")]
        public string StateMachineIdentifier { get; set; }

        [JsonProperty("parentstack")]
        internal string[] ParentInstanceStack { get; set; }

        internal object DeserializeInput(IStateMachineMetadata metadata)
        {
            metadata.CheckArgNull(nameof(metadata));

            if (this.Input == null)
            {
                return null;
            }

            Debug.Assert(this.Input is JObject);

            var metadataType = metadata.GetType();

            Debug.Assert(metadataType.IsGenericType);
            
            var genericArgs = metadataType.GetGenericArguments();

            Debug.Assert(genericArgs.Length == 1);

            return ((JObject) this.Input).ToObject(genericArgs[0]);
        }
    }
}
