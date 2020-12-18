﻿using Newtonsoft.Json.Linq;
using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Exceptions;
using StateChartsDotNet.Common.Model;
using StateChartsDotNet.Common.Model.Execution;
using StateChartsDotNet.Common.Model.States;
using StateChartsDotNet.Metadata.Json.Data;
using StateChartsDotNet.Metadata.Json.Execution;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;


namespace StateChartsDotNet.Metadata.Json.States
{
    public class InvokeStateChartMetadata : IInvokeStateChartMetadata
    {
        private readonly JObject _element;
        private readonly Lazy<string> _uniqueId;

        internal InvokeStateChartMetadata(JObject element)
        {
            _element = element;

            _uniqueId = new Lazy<string>(() =>
            {
                return element.GetUniqueElementPath();
            });
        }

        public bool Autoforward
        {
            get
            {
                var afattr = _element.Property("autoforward");

                if (afattr != null && bool.TryParse(afattr.Value.Value<string>(), out bool result))
                {
                    return result;
                }
                else
                {
                    return false;
                }
            }
        }

        public string UniqueId => _uniqueId.Value;

        public virtual bool Validate(Dictionary<IModelMetadata, List<string>> errors)
        {
            return true;
        }

        public string Id => _element.Property("id")?.Value.Value<string>() ?? string.Empty;

        public string IdLocation => _element.Property("idlocation")?.Value.Value<string>() ?? string.Empty;

        private IEnumerable<string> Namelist
        {
            get
            {
                var names = _element.Property("namelist")?.Value.Value<string>();

                if (string.IsNullOrWhiteSpace(names))
                {
                    return Enumerable.Empty<string>();
                }
                else
                {
                    return names.Split(" ");
                }
            }
        }

        public IEnumerable<IExecutableContentMetadata> GetFinalizeExecutableContent()
        {
            var content = new List<IExecutableContentMetadata>();

            var elements = _element.Property("finalize");

            if (elements != null)
            {
                foreach (var node in elements.Value.Values<JObject>())
                {
                    content.Add(ExecutableContentMetadata.Create(node));
                }
            }

            return content;
        }

        public IStateChartMetadata GetRoot()
        {
            var node = _element.Property("content");

            if (node != null)
            {
                var xobj = node.Value.Value<JObject>();

                Debug.Assert(xobj != null);

                return new StateChart(xobj);
            }

            return null;
        }

        public IReadOnlyDictionary<string, object> GetParams(dynamic data)
        {
            var parmProp = _element.Property("params");

            if (this.Namelist.Any() && parmProp != null)
            {
                throw new MetadataValidationException("Only one of service namelist and params can be specified.");
            }

            IEnumerable<ParamMetadata> parms;

            if (this.Namelist.Any())
            {
                parms = this.Namelist.Select(n => new ParamMetadata(n));
            }
            else
            {
                parms = parmProp.Value.Values<JObject>().Select(n => new ParamMetadata(n));
            }

            return new ReadOnlyDictionary<string, object>(parms.ToDictionary(p => p.Name, p => p.GetValue(data)));
        }
    }
}