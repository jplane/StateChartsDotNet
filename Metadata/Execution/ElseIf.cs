﻿using Newtonsoft.Json;
using StateChartsDotNet.Common;
using StateChartsDotNet.Common.ExpressionTrees;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace StateChartsDotNet.Metadata.Execution
{
    public class ElseIf : ExecutableContent, IElseIfMetadata
    {
        private readonly Lazy<Func<dynamic, bool>> _condition;

        private MetadataList<ExecutableContent> _actions;

        public ElseIf()
        {
            this.Actions = new MetadataList<ExecutableContent>();

            _condition = new Lazy<Func<dynamic, bool>>(() =>
            {
                if (!string.IsNullOrWhiteSpace(this.ConditionExpression))
                {
                    return ExpressionCompiler.Compile<bool>(this.ConditionExpression);
                }
                else if (this.ConditionFunction != null)
                {
                    var func = this.ConditionFunction.Compile();

                    Debug.Assert(func != null);

                    return data => func((IDictionary<string, object>)data);
                }
                else
                {
                    return _ => throw new InvalidOperationException("Unable to resolve 'elseif' condition.");
                }
            });
        }

        [JsonProperty("conditionfunction", ItemConverterType = typeof(ExpressionTreeConverter))]
        public Expression<Func<IDictionary<string, object>, bool>> ConditionFunction { get; set; }

        [JsonProperty("conditionexpression")]
        public string ConditionExpression { get; set; }

        [JsonProperty("actions", ItemConverterType = typeof(ExecutableContentConverter))]
        public MetadataList<ExecutableContent> Actions
        {
            get => _actions;

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                if (_actions != null)
                {
                    _actions.MetadataIdResolver = null;
                }

                value.MetadataIdResolver = () => $"{this.MetadataIdResolver?.Invoke(this) ?? "elseif"}.actions";

                _actions = value;
            }
        }

        internal override void Validate(IDictionary<string, List<string>> errorMap)
        {
            Debug.Assert(errorMap != null);

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.ConditionExpression) && this.ConditionFunction == null)
            {
                errors.Add("One of ConditionExpression or ConditionFunction must be set.");
            }

            foreach (var action in this.Actions)
            {
                action.Validate(errorMap);
            }

            if (errors.Any())
            {
                errorMap.Add(((IModelMetadata)this).MetadataId, errors);
            }
        }

        bool IElseIfMetadata.EvalCondition(dynamic data) => _condition.Value(data);

        IEnumerable<IExecutableContentMetadata> IElseIfMetadata.GetExecutableContent() =>
            this.Actions ?? Enumerable.Empty<IExecutableContentMetadata>();
    }
}