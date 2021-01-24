﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace StateChartsDotNet.Common.Model.Execution
{
    public interface IElseIfMetadata : IExecutableContentMetadata
    {
        bool EvalCondition(dynamic data);

        IEnumerable<IExecutableContentMetadata> GetExecutableContent();
    }
}
