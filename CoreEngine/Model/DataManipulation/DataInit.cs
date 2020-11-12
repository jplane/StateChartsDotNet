﻿using CoreEngine.Abstractions.Model.DataManipulation.Metadata;
using System;
using System.Threading.Tasks;

namespace CoreEngine.Model.DataManipulation
{
    internal class DataInit
    {
        private readonly IDataInitMetadata _metadata;

        public DataInit(IDataInitMetadata metadata)
        {
            metadata.CheckArgNull(nameof(metadata));

            _metadata = metadata;
        }

        public async Task Init(ExecutionContext context)
        {
            context.CheckArgNull(nameof(context));

            context.LogInformation("Start: DataInit");

            try
            {
                var value = await _metadata.GetValue(context.ScriptData);

                context.SetDataValue(_metadata.Id, value);

                context.LogDebug($"Set {_metadata.Id} = {value}");
            }
            catch (Exception ex)
            {
                context.EnqueueExecutionError(ex);
            }
            finally
            {
                context.LogInformation("End: DataInit");
            }
        }
    }
}