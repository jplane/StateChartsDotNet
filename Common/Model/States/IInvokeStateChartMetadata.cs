﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using StateChartsDotNet.Common.Model.Execution;

namespace StateChartsDotNet.Common.Model.States
{
    public interface IInvokeStateChartMetadata : IModelMetadata
    {
        string Id { get; }
        string ResultLocation { get; }
        ChildStateChartExecutionMode ExecutionMode { get; }
        string RemoteUri { get; }

        IStateChartMetadata GetRoot();
        string GetRootIdentifier();
        object GetData(dynamic data);
        IEnumerable<IExecutableContentMetadata> GetFinalizeExecutableContent();
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChildStateChartExecutionMode
    {
        Inline = 1,
        Remote
    }
}
