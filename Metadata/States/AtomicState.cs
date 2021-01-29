﻿using Newtonsoft.Json;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StateChartsDotNet.Metadata.States
{
    public class AtomicState : State, IStateMetadata
    {
        private OnEntryExit _onEntry;
        private OnEntryExit _onExit;
        private MetadataList<Transition> _transitions;
        private MetadataList<InvokeStateChart> _invokes;

        public AtomicState()
        {
            this.Transitions = new MetadataList<Transition>();
            this.Invokes = new MetadataList<InvokeStateChart>();
        }

        [JsonProperty("onentry")]
        public OnEntryExit OnEntry
        {
            get => _onEntry;

            set
            {
                if (_onEntry != null)
                {
                    _onEntry.MetadataIdResolver = null;
                }

                if (value != null)
                {
                    value.MetadataIdResolver = _ => $"{this.MetadataIdResolver?.Invoke(this) ?? "atomicstate"}.onentry";
                    value.IsEntry = true;
                }

                _onEntry = value;
            }
        }

        [JsonProperty("onexit")]
        public OnEntryExit OnExit
        {
            get => _onExit;

            set
            {
                if (_onExit != null)
                {
                    _onExit.MetadataIdResolver = null;
                }

                if (value != null)
                {
                    value.MetadataIdResolver = _ => $"{this.MetadataIdResolver?.Invoke(this) ?? "atomicstate"}.onexit";
                    value.IsEntry = false;
                }

                _onExit = value;
            }
        }

        [JsonProperty("transitions")]
        public MetadataList<Transition> Transitions
        {
            get => _transitions;

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                if (_transitions != null)
                {
                    _transitions.MetadataIdResolver = null;
                }

                value.MetadataIdResolver = () => $"{this.MetadataIdResolver?.Invoke(this) ?? "atomicstate"}.transitions";

                _transitions = value;
            }
        }

        [JsonProperty("childinvocations")]
        public MetadataList<InvokeStateChart> Invokes
        {
            get => _invokes;

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                if (_invokes != null)
                {
                    _invokes.MetadataIdResolver = null;
                }

                value.MetadataIdResolver = () => $"{this.MetadataIdResolver?.Invoke(this) ?? "atomicstate"}.invokes";

                _invokes = value;
            }
        }

        internal override void Validate(IDictionary<string, List<string>> errorMap)
        {
            Debug.Assert(errorMap != null);

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.Id))
            {
                errors.Add("Id is invalid.");
            }

            foreach (var transition in this.Transitions)
            {
                transition.Validate(errorMap);
            }

            foreach (var invoke in this.Invokes)
            {
                invoke.Validate(errorMap);
            }

            this.OnEntry?.Validate(errorMap);

            this.OnExit?.Validate(errorMap);

            if (errors.Any())
            {
                errorMap.Add(((IModelMetadata)this).MetadataId, errors);
            }
        }

        StateType IStateMetadata.Type => StateType.Atomic;

        bool IStateMetadata.IsDescendentOf(IStateMetadata state)
        {
            return this.IsDescendentOf(state);
        }

        int IStateMetadata.GetDocumentOrder()
        {
            return this.GetDocumentOrder();
        }

        IOnEntryExitMetadata IStateMetadata.GetOnEntry() => this.OnEntry;

        IOnEntryExitMetadata IStateMetadata.GetOnExit() => this.OnExit;

        IEnumerable<ITransitionMetadata> IStateMetadata.GetTransitions() =>
            this.Transitions ?? Enumerable.Empty<ITransitionMetadata>();

        IEnumerable<IInvokeStateChartMetadata> IStateMetadata.GetStateChartInvokes() =>
            this.Invokes ?? Enumerable.Empty<IInvokeStateChartMetadata>();

        ITransitionMetadata IStateMetadata.GetInitialTransition() => throw new NotSupportedException();

        IEnumerable<IStateMetadata> IStateMetadata.GetStates() => Enumerable.Empty<IStateMetadata>();
    }
}