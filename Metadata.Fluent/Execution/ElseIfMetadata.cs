﻿using StateChartsDotNet.Common;
using StateChartsDotNet.Common.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace StateChartsDotNet.Metadata.Fluent.Execution
{
    public sealed class ElseIfMetadata<TParent> : ExecutableContentMetadata where TParent : IModelMetadata
    {
        private readonly List<ExecutableContentMetadata> _executableContent;
        private Func<dynamic, bool> _eval;

        internal ElseIfMetadata()
        {
            _executableContent = new List<ExecutableContentMetadata>();
        }

        internal override void Serialize(BinaryWriter writer)
        {
            writer.CheckArgNull(nameof(writer));

            base.Serialize(writer);

            writer.Write(_eval);

            writer.WriteMany(_executableContent, (o, w) => o.Serialize(w));
        }

        internal static ElseIfMetadata<TParent> Deserialize(BinaryReader reader)
        {
            reader.CheckArgNull(nameof(reader));

            var metadata = new ElseIfMetadata<TParent>();

            metadata.MetadataId = reader.ReadNullableString();
            metadata._eval = reader.Read<Func<dynamic, bool>>();
            metadata._executableContent.AddRange(ExecutableContentMetadata.DeserializeMany(reader, metadata));

            return metadata;
        }

        internal TParent Parent { get; set; }

        internal IEnumerable<ExecutableContentMetadata> GetExecutableContent() => _executableContent;

        internal Func<dynamic, bool> GetEvalCondition() => _eval;

        public TParent _ => this.Parent;

        public ElseIfMetadata<TParent> Condition(Func<dynamic, bool> condition)
        {
            _eval = condition;
            return this;
        }

        public ElseIfMetadata<TParent> Assign(string location, object value)
        {
            var ec = new AssignMetadata<ElseIfMetadata<TParent>>();

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            ec.Location(location).Value(value);

            return this;
        }

        public ElseIfMetadata<TParent> Assign(string location, Func<dynamic, object> getter)
        {
            var ec = new AssignMetadata<ElseIfMetadata<TParent>>();

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            ec.Location(location).Value(getter);

            return this;
        }

        public CancelMetadata<ElseIfMetadata<TParent>> Cancel
        {
            get
            {
                var ec = new CancelMetadata<ElseIfMetadata<TParent>>();

                _executableContent.Add(ec);

                ec.Parent = this;

                ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

                return ec;
            }
        }

        public ForeachMetadata<ElseIfMetadata<TParent>> Foreach
        {
            get
            {
                var ec = new ForeachMetadata<ElseIfMetadata<TParent>>();

                _executableContent.Add(ec);

                ec.Parent = this;

                ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

                return ec;
            }
        }

        public IfMetadata<ElseIfMetadata<TParent>> If
        {
            get
            {
                var ec = new IfMetadata<ElseIfMetadata<TParent>>();

                _executableContent.Add(ec);

                ec.Parent = this;

                ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

                return ec;
            }
        }

        public ElseIfMetadata<TParent> Log(string message)
        {
            var ec = new LogMetadata<ElseIfMetadata<TParent>>();

            ec.Message(message);

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            return this;
        }

        public ElseIfMetadata<TParent> Log(Func<dynamic, string> getter)
        {
            var ec = new LogMetadata<ElseIfMetadata<TParent>>();

            ec.Message(getter);

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            return this;
        }

        public ElseIfMetadata<TParent> Raise(string messageName)
        {
            var ec = new RaiseMetadata<ElseIfMetadata<TParent>>();

            ec.MessageName(messageName);

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            return this;
        }

        public ElseIfMetadata<TParent> Execute(Action<dynamic> action)
        {
            var ec = new ScriptMetadata<ElseIfMetadata<TParent>>();

            ec.Action(action);

            _executableContent.Add(ec);

            ec.Parent = this;

            ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

            return this;
        }

        internal SendMessageMetadata<ElseIfMetadata<TParent>> SendMessage
        {
            get
            {
                var ec = new SendMessageMetadata<ElseIfMetadata<TParent>>();

                _executableContent.Add(ec);

                ec.Parent = this;

                ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

                return ec;
            }
        }

        internal QueryMetadata<ElseIfMetadata<TParent>> Query
        {
            get
            {
                var ec = new QueryMetadata<ElseIfMetadata<TParent>>();

                _executableContent.Add(ec);

                ec.Parent = this;

                ec.MetadataId = $"{((IModelMetadata)this).MetadataId}.ExecutableContent[{_executableContent.Count}]";

                return ec;
            }
        }
    }
}
