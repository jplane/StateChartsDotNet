﻿using System;
using System.Collections.Generic;
using StateChartsDotNet.Common.Model.Execution;

namespace StateChartsDotNet.Common.Model.States
{
    public interface IInvokeStateChartMetadata : IModelMetadata
    {
        string IdLocation { get; }
        ChildStateChartExecutionMode ExecutionMode { get; }
        bool Autoforward { get; }
        string RemoteUri { get; }

        IStateChartMetadata GetRoot();
        IReadOnlyDictionary<string, object> GetParams(dynamic data);
        IEnumerable<IExecutableContentMetadata> GetFinalizeExecutableContent();
    }

    public enum ChildStateChartExecutionMode
    {
        Inline = 1,
        Isolated,
        Remote
    }

    public static class InvokeStateChartMetadataExtensions
    {
        public static void Validate(this IInvokeStateChartMetadata metadata, Dictionary<IModelMetadata, List<string>> errors)
        {
            ((IModelMetadata) metadata).Validate(errors);

            var msgs = new List<string>();

            var root = metadata.GetRoot();

            if (root == null)
            {
                msgs.Add("InvokeStateChart must specify a child statechart definition.");
            }
            else
            {
                root.Validate(errors);
            }

            if (msgs.Count > 0)
            {
                errors.Add(metadata, msgs);
            }

            foreach (var executableContent in metadata.GetFinalizeExecutableContent())
            {
                executableContent.Validate(errors);
            }
        }
    }
}
