﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DSM.Common.Model.Execution
{
    public interface IForeachMetadata : IActionMetadata
    {
        (string, MemberInfo) Item { get; }
        (string, MemberInfo) Index { get; }

        IEnumerable GetArray(dynamic data);
        IEnumerable<IActionMetadata> GetActions();
    }
}
