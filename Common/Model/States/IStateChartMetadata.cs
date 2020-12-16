﻿using StateChartsDotNet.Common.Model.Execution;
using System.Collections.Generic;

namespace StateChartsDotNet.Common.Model.States
{
    public interface IStateChartMetadata : IStateMetadata
    {
        bool FailFast { get; }
        Databinding Databinding { get; }

        IEnumerable<IStateMetadata> GetStates();
        ITransitionMetadata GetInitialTransition();
        IScriptMetadata GetScript();
    }
}