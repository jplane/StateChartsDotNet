﻿using Newtonsoft.Json.Linq;
using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Exceptions;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Execution;
using StateChartsDotNet.Metadata.Json.Queries;
using StateChartsDotNet.Metadata.Json.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace StateChartsDotNet.Metadata.Json.Execution
{
    public abstract class ExecutableContentMetadata : IExecutableContentMetadata
    {
        private readonly string _metadataId;
        protected readonly JObject _element;

        protected ExecutableContentMetadata(JObject element)
        {
            _element = element;
            _metadataId = element.GetUniqueElementPath();
        }

        public string MetadataId => _metadataId;

        public static IExecutableContentMetadata Create(JObject element)
        {
            element.CheckArgNull(nameof(element));

            var resolvers = new Func<JObject, IExecutableContentMetadata>[]
            {
                ServiceResolver.Resolve,
                QueryResolver.Resolve 
            };

            var type = element.Property("type").Value.Value<string>();

            var content = type switch
            {
                "if" => new IfMetadata(element),
                "raise" => new RaiseMetadata(element),
                "script" => new ScriptMetadata(element),
                "foreach" => new ForeachMetadata(element),
                "log" => new LogMetadata(element),
                "cancel" => new CancelMetadata(element),
                "assign" => new AssignMetadata(element),
                _ => resolvers.Select(func => func(element)).FirstOrDefault(result => result != null)
            };

            if (content == null)
            {
                throw new InvalidOperationException("Unable to resolve executable content type: " + type);
            }

            return content;
        }
    }
}
