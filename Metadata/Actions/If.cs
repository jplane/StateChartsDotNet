﻿using Newtonsoft.Json;
using DSM.Common;
using DSM.Common.Model;
using DSM.Common.Model.Actions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSM.Metadata.Actions
{
    /// <summary>
    /// A conditional 'if' branch of an if-elseif-else control flow block.
    /// Can be used in conjuction with <see cref="ElseIf{TData}"/> and <see cref="Else{TData}"/>.
    /// </summary>
    /// <typeparam name="TData">The execution state of the state machine.</typeparam>
    [JsonObject(Id = "If",
                ItemNullValueHandling = NullValueHandling.Ignore,
                ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
    public sealed class If<TData> : Action<TData>, IIfMetadata
    {
        private readonly Lazy<Func<dynamic, bool>> _condition;

        private MetadataList<Action<TData>> _actions;
        private MetadataList<ElseIf<TData>> _elseIfs;
        private Else<TData> _else;

        public If()
        {
            this.Actions = new MetadataList<Action<TData>>();
            this.ElseIfs = new MetadataList<ElseIf<TData>>();

            _condition = new Lazy<Func<dynamic, bool>>(() =>
            {
                if (!string.IsNullOrWhiteSpace(this.ConditionExpression))
                {
                    return ExpressionCompiler.Compile<bool>(((IModelMetadata)this).MetadataId, this.ConditionExpression);
                }
                else if (this.ConditionFunction != null)
                {
                    return data => this.ConditionFunction((TData) data);
                }
                else
                {
                    return _ => throw new InvalidOperationException("Unable to resolve 'if' condition.");
                }
            });
        }

        /// <summary>
        /// Condition evaluated to determine if this <see cref="If{TData}"/> branch is executed.
        /// Execution state <typeparamref name="TData"/> can be used as part of the conditional logic.
        /// </summary>
        [JsonIgnore]
        public Func<TData, bool> ConditionFunction { get; set; }

        [JsonProperty("conditionexpression", Required = Required.Always)]
        private string ConditionExpression { get; set; }

        /// <summary>
        /// The set of actions executed for this <see cref="If{TData}"/> branch.
        /// </summary>
        [JsonProperty("actions", ItemConverterType = typeof(ActionConverter), Required = Required.Always)]
        public MetadataList<Action<TData>> Actions
        {
            get => _actions;

            private set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                if (_actions != null)
                {
                    _actions.MetadataIdResolver = null;
                }

                value.MetadataIdResolver = () => $"{this.MetadataIdResolver?.Invoke(this) ?? "if"}.actions";

                _actions = value;
            }
        }

        /// <summary>
        /// The set of <see cref="ElseIf{TData}"/> elements conditionally executed for this <see cref="If{TData}"/>.
        /// </summary>
        [JsonProperty("elseifs", Required = Required.DisallowNull)]
        public MetadataList<ElseIf<TData>> ElseIfs
        {
            get => _elseIfs;

            private set
            {
                if (value == null)
                {
                    throw new InvalidOperationException();
                }

                if (_elseIfs != null)
                {
                    _elseIfs.MetadataIdResolver = null;
                }

                value.MetadataIdResolver = () => $"{this.MetadataIdResolver?.Invoke(this) ?? "if"}.elseifs";

                _elseIfs = value;
            }
        }

        /// <summary>
        /// The <see cref="Else{TData}"/> element conditionally executed for this <see cref="If{TData}"/>.
        /// </summary>
        [JsonProperty("else", Required = Required.DisallowNull)]
        public Else<TData> Else
        {
            get => _else;

            set
            {
                if (_else != null)
                {
                    _else.MetadataIdResolver = null;
                }

                if (value != null)
                {
                    value.MetadataIdResolver = _ => $"{this.MetadataIdResolver?.Invoke(this) ?? "if"}.else";
                }

                _else = value;
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

            foreach (var elseif in this.ElseIfs)
            {
                elseif.Validate(errorMap);
            }

            this.Else?.Validate(errorMap);

            if (errors.Any())
            {
                errorMap.Add(((IModelMetadata)this).MetadataId, errors);
            }
        }

        bool IIfMetadata.EvalCondition(dynamic data) => _condition.Value(data);

        IEnumerable<IElseIfMetadata> IIfMetadata.GetElseIfs() => this.ElseIfs ?? Enumerable.Empty<IElseIfMetadata>();

        IEnumerable<IActionMetadata> IIfMetadata.GetActions() =>
            this.Actions ?? Enumerable.Empty<IActionMetadata>();

        IElseMetadata IIfMetadata.GetElse() => this.Else;
    }
}
