using MIC.Core.Entities; // 引用本项目内的命名空间
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIC.Core.Interfaces
{
    public interface ILogQueryService
    {
        // 这里的 SystemLog 现在来自 MIC.Core.Entities
        Task<IEnumerable<SystemLog>> GetLogsAsync(DateTime start, DateTime end, string level, string keyword);
    }
}