using MIC.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIC.Core.Interfaces
{
    public interface ILogQueryService
    {
        Task<IEnumerable<SystemLog>> GetLogsAsync(DateTime start, DateTime end, string level, string keyword);
    }
}