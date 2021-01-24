﻿using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Data;
using StateChartsDotNet.Common.Model.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StateChartsDotNet.Metadata.Fluent.States
{
    public abstract class StateMetadata : IStateMetadata
    {
        private readonly string _id;

        internal StateMetadata(string id)
        {
            _id = id;
        }

        internal virtual void Serialize(BinaryWriter writer)
        {
            writer.CheckArgNull(nameof(writer));

            writer.WriteNullableString(this.GetType().AssemblyQualifiedName);
            writer.WriteNullableString(this.Id);
            writer.WriteNullableString(this.MetadataId);
        }

        internal static StateMetadata _Deserialize(BinaryReader reader)
        {
            reader.CheckArgNull(nameof(reader));

            var aqtn = reader.ReadNullableString();

            Debug.Assert(!string.IsNullOrWhiteSpace(aqtn));

            var type = System.Type.GetType(aqtn);

            Debug.Assert(type != null);

            var deserializeMethod = type.GetMethod("Deserialize", BindingFlags.NonPublic | BindingFlags.Static);

            Debug.Assert(deserializeMethod != null);

            return (StateMetadata) deserializeMethod.Invoke(null, new[] { reader });
        }

        public abstract StateType Type { get; }

        protected abstract IStateMetadata _Parent { get; }

        internal virtual IEnumerable<string> StateNames
        {
            get => new[] { _id };
        }

        public string Id => _id;

        public virtual string MetadataId { get; set; }

        IReadOnlyDictionary<string, object> IModelMetadata.DebuggerInfo
        {
            get
            {
                var json = new Dictionary<string, object>();

                json["id"] = this.Id;
                json["metadataId"] = this.MetadataId;

                return json;
            }
        }


        int IStateMetadata.DepthFirstCompare(IStateMetadata metadata)
        {
            int Compare(string x, string y)
            {
                return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
            }

            if (Compare(((IStateMetadata) this).Id, metadata.Id) == 0)
            {
                return 0;
            }

            var current = this;
            var parent = this._Parent;

            while (parent != null)
            {
                Debug.Assert(parent is StateMetadata);

                current = (StateMetadata) parent;

                parent = current._Parent;
            }

            Debug.Assert(current is StateChart);

            var stateNames = ((StateChart)current).StateNames.ToArray();

            foreach (var name in stateNames)
            {
                if (Compare(((IStateMetadata)this).Id, name) == 0)
                {
                    return 1;
                }
                else if (Compare(metadata.Id, name) == 0)
                {
                    return -1;
                }
            }

            Debug.Fail("Unexpected result.");

            return 0;
        }

        protected virtual IDatamodelMetadata GetDatamodel() => null;

        IDatamodelMetadata IStateMetadata.GetDatamodel()
        {
            return GetDatamodel();
        }

        protected virtual IOnEntryExitMetadata GetOnEntry() => null;

        IOnEntryExitMetadata IStateMetadata.GetOnEntry()
        {
            return GetOnEntry();
        }

        protected virtual IOnEntryExitMetadata GetOnExit() => null;

        IOnEntryExitMetadata IStateMetadata.GetOnExit()
        {
            return GetOnExit();
        }

        protected virtual IEnumerable<IInvokeStateChartMetadata> GetStateChartInvokes() => Enumerable.Empty<IInvokeStateChartMetadata>();

        IEnumerable<IInvokeStateChartMetadata> IStateMetadata.GetStateChartInvokes()
        {
            return GetStateChartInvokes();
        }

        protected virtual IEnumerable<ITransitionMetadata> GetTransitions() => Enumerable.Empty<ITransitionMetadata>();

        IEnumerable<ITransitionMetadata> IStateMetadata.GetTransitions()
        {
            return GetTransitions();
        }

        protected virtual ITransitionMetadata GetInitialTransition() => null;

        ITransitionMetadata IStateMetadata.GetInitialTransition()
        {
            return GetInitialTransition();
        }

        protected virtual IEnumerable<IStateMetadata> GetStates() => Enumerable.Empty<IStateMetadata>();

        IEnumerable<IStateMetadata> IStateMetadata.GetStates()
        {
            return GetStates();
        }

        bool IStateMetadata.IsDescendentOf(IStateMetadata state)
        {
            var parent = this._Parent;

            while (parent != null)
            {
                Debug.Assert(parent is StateMetadata);

                if (parent == state)
                {
                    return true;
                }

                parent = ((StateMetadata) parent)._Parent;
            }

            return false;
        }
    }
}