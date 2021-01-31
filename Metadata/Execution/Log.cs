﻿using Newtonsoft.Json;
using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Execution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StateChartsDotNet.Metadata.Execution
{
    public class Log<TData> : ExecutableContent<TData>, ILogMetadata
    {
        private Lazy<Func<dynamic, string>> _messageGetter;

        public Log()
        {
            _messageGetter = new Lazy<Func<dynamic, string>>(() =>
            {
                if (!string.IsNullOrWhiteSpace(this.MessageExpression))
                {
                    return ExpressionCompiler.Compile<string>(this.MessageExpression);
                }
                else if (this.MessageFunction != null)
                {
                    return data => this.MessageFunction((TData) data);
                }
                else
                {
                    return _ => this.Message;
                }
            });
        }

        [JsonProperty("message")]
        public string Message { get; set; }

        public Func<TData, string> MessageFunction { get; set; }

        [JsonProperty("messageexpression")]
        private string MessageExpression { get; set; }

        public string GetMessage(dynamic data) => _messageGetter.Value(data);

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

            if (errors.Any())
            {
                errorMap.Add(((IModelMetadata)this).MetadataId, errors);
            }
        }
    }
}
