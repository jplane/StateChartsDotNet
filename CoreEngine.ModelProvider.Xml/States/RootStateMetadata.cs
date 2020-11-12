﻿using CoreEngine.Abstractions.Model;
using CoreEngine.Abstractions.Model.Execution.Metadata;
using CoreEngine.Abstractions.Model.States.Metadata;
using CoreEngine.ModelProvider.Xml.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace CoreEngine.ModelProvider.Xml.States
{
    public class RootStateMetadata : StateMetadata, IRootStateMetadata
    {
        public RootStateMetadata(XElement element)
            : base(element)
        {
        }

        public override string Id => _element.Attribute("name")?.Value ?? string.Empty;

        public Databinding Databinding
        {
            get => (Databinding) Enum.Parse(typeof(Databinding),
                                            _element.Attribute("binding")?.Value ?? "early",
                                            true);
        }

        public async Task<ITransitionMetadata> GetInitialTransition()
        {
            var attr = _element.Attribute("initial");

            if (attr != null)
            {
                return new TransitionMetadata(attr);
            }
            else
            {
                var firstChild = (await GetStates()).FirstOrDefault(sm => sm is IAtomicStateMetadata ||
                                                                          sm is ISequentialStateMetadata ||
                                                                          sm is IParallelStateMetadata ||
                                                                          sm is IFinalizeMetadata);

                return firstChild == null ? null : new TransitionMetadata(firstChild.Id);
            }
        }

        public Task<IScriptMetadata> GetScript()
        {
            var node = _element.ScxmlElement("script");

            return Task.FromResult(node == null ? null : (IScriptMetadata) new ScriptMetadata(node));
        }

        public Task<IEnumerable<IStateMetadata>> GetStates()
        {
            var states = new List<IStateMetadata>();

            bool IsCompoundState(XElement el)
            {
                return el.ScxmlNameEquals("state") &&
                       el.Elements().Any(ce => ce.ScxmlNameIn("state", "parallel", "final"));
            }

            foreach (var el in _element.Elements())
            {
                if (IsCompoundState(el))
                {
                    states.Add(new SequentialStateMetadata(el));
                }
                else if (el.ScxmlNameEquals("parallel"))
                {
                    states.Add(new ParallelStateMetadata(el));
                }
                else if (el.ScxmlNameEquals("final"))
                {
                    states.Add(new FinalStateMetadata(el));
                }
                else if (el.ScxmlNameEquals("state"))
                {
                    states.Add(new AtomicStateMetadata(el));
                }
            }

            return Task.FromResult(states.AsEnumerable());
        }
    }
}