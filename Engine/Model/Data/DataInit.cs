﻿using StateChartsDotNet.Common.Model.Data;
using System;

namespace StateChartsDotNet.Model.Data
{
    internal class DataInit
    {
        private readonly IDataInitMetadata _metadata;

        public DataInit(IDataInitMetadata metadata)
        {
            metadata.CheckArgNull(nameof(metadata));

            _metadata = metadata;
        }

        public void Init(ExecutionContext context)
        {
            context.CheckArgNull(nameof(context));

            context.LogInformationAsync("Start: DataInit");

            try
            {
                var value = _metadata.GetValue(context.ScriptData);

                context.SetDataValue(_metadata.Id, value);

                context.LogDebugAsync($"Set {_metadata.Id} = {value}");
            }
            catch (Exception ex)
            {
                context.EnqueueExecutionError(ex);
            }
            finally
            {
                context.LogInformationAsync("End: DataInit");
            }
        }
    }
}