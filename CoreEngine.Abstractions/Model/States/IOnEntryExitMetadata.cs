﻿using StateChartsDotNet.CoreEngine.Abstractions.Model.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StateChartsDotNet.CoreEngine.Abstractions.Model.States
{
    public interface IOnEntryExitMetadata
    {
        bool IsEntry { get; }

        Task<IEnumerable<IExecutableContentMetadata>> GetExecutableContent();
    }
}