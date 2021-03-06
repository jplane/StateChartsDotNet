﻿using DSM.Common.Messages;
using System;
using System.Collections.Generic;

namespace DSM.Engine
{
    public sealed class InternalMessage : Message
    {
        internal InternalMessage()
        {
        }

        public bool IsError => this.Content != null && this.Content is Exception;

        public override MessageType Type => MessageType.Internal;

        public object Content { get; set; }
    }
}
