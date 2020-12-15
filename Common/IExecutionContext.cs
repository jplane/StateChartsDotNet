﻿using StateChartsDotNet.Common.Messages;
using StateChartsDotNet.Common.Model.States;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StateChartsDotNet.Common
{
    public interface IExecutionContext
    {
        IDictionary<string, object> Data { get; }

        Task SendStopMessageAsync();
        Task SendMessageAsync(string message, object content = null, IReadOnlyDictionary<string, object> parameters = null);
    }
}