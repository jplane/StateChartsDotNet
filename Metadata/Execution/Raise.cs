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
    public class Raise : ExecutableContent, IRaiseMetadata
    {
        private Lazy<Func<dynamic, string>> _messageGetter;

        public Raise()
        {
            _messageGetter = new Lazy<Func<dynamic, string>>(() =>
            {
                if (!string.IsNullOrWhiteSpace(this.Location))
                {
                    return data => data[this.Location];
                }
                else if (!string.IsNullOrWhiteSpace(this.MessageExpression))
                {
                    return ExpressionCompiler.Compile<string>(this.MessageExpression);
                }
                else if (this.MessageFunction != null)
                {
                    var func = this.MessageFunction.Compile();

                    Debug.Assert(func != null);

                    return data => func((IDictionary<string, object>)data);
                }
                else
                {
                    return _ => this.Message;
                }
            });
        }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("messagefunction", ItemConverterType = typeof(ExpressionTreeConverter))]
        public Expression<Func<IDictionary<string, object>, string>> MessageFunction { get; set; }

        [JsonProperty("messageexpression")]
        public string MessageExpression { get; set; }

        string IRaiseMetadata.GetMessage(dynamic data) => _messageGetter.Value(data);

        internal override void Validate(IDictionary<string, List<string>> errorMap)
        {
            Debug.Assert(errorMap != null);

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(this.MessageExpression) &&
                this.MessageFunction == null &&
                this.Message == null)
            {
                errors.Add("One of Message, MessageExpression, or MessageFunction must be set.");
            }

            if (string.IsNullOrWhiteSpace(this.Location))
            {
                errors.Add("Location is invalid.");
            }

            if (errors.Any())
            {
                errorMap.Add(((IModelMetadata)this).MetadataId, errors);
            }
        }
    }
}
