﻿using Newtonsoft.Json.Linq;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Execution;
using StateChartsDotNet.Common.Model.States;
using StateChartsDotNet.Metadata.Json.Execution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateChartsDotNet.Metadata.Json.States
{
    public class StateChart : StateMetadata, IStateChartMetadata
    {
        private readonly string _name;

        public StateChart(JObject document)
            : base(document)
        {
            _name = _element.Value<string>("name");

            document.InitDocumentPosition();
        }

        public override string Id => _name ?? this.UniqueId;

        public override string UniqueId => "scxml[0]";

        public bool FailFast
        {
            get => bool.Parse(_element.Value<string>("failfast") ?? "false");
        }

        public Databinding Databinding
        {
            get => (Databinding) Enum.Parse(typeof(Databinding),
                                            _element.Value<string>("binding") ?? "early",
                                            true);
        }

        public ITransitionMetadata GetInitialTransition()
        {
            var attr = _element.Value<string>("initial");

            if (attr != null)
            {
                return new TransitionMetadata(attr);
            }
            else
            {
                var firstChild = GetStates().FirstOrDefault(sm => sm is IAtomicStateMetadata ||
                                                                  sm is ISequentialStateMetadata ||
                                                                  sm is IParallelStateMetadata ||
                                                                  sm is IFinalStateMetadata);

                return firstChild == null ? null : new TransitionMetadata(firstChild.Id);
            }
        }

        public IScriptMetadata GetScript()
        {
            var node = _element.Property("script")?.Value.Value<JObject>();

            return node == null ? null : (IScriptMetadata) new ScriptMetadata(node);
        }

        public IEnumerable<IStateMetadata> GetStates()
        {
            var node = _element.Property("states");

            var states = new List<IStateMetadata>();

            foreach (var el in node.Value.Values<JObject>())
            {
                var type = el.Property("type")?.Value.Value<string>();

                if (type == null)
                {
                    states.Add(new AtomicStateMetadata(el));
                }
                else if (type == "sequential")
                {
                    states.Add(new SequentialStateMetadata(el));
                }
                else if (type == "parallel")
                {
                    states.Add(new ParallelStateMetadata(el));
                }
                else if (type == "final")
                {
                    states.Add(new FinalStateMetadata(el));
                }
            }

            return states.AsEnumerable();
        }
    }
}