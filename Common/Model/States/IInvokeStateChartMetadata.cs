﻿using System;
using System.Collections.Generic;
using StateChartsDotNet.Common.Model.Execution;

namespace StateChartsDotNet.Common.Model.States
{
    public interface IInvokeStateChartMetadata : IModelMetadata
    {
        bool Autoforward { get; }
        string Id { get; }
        string IdLocation { get; }

        string GetType(dynamic data);
        object GetContent(dynamic data);
        IReadOnlyDictionary<string, object> GetParams(dynamic data);
        IEnumerable<IExecutableContentMetadata> GetFinalizeExecutableContent();
     }
}
