using MIC.Models.DTOs;
using MIC.Infrastructure.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIC.Services
{
    // 负责磁盘 IO，实现方案的一键切换。
    public class SolutionManager
    {
        private readonly string _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions");
        public SolutionProject CurrentSolution { get; private set; }
        // 存储当前方案下加载的所有流程内存对象
        public Dictionary<string, WorkflowDefine> WorkflowCache { get; } = new Dictionary<string, WorkflowDefine>();

        public void LoadSolution(string solutionName)
        {
            string baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions", solutionName);

            // 1. 加载主方案配置
            CurrentSolution = JsonConfigHelper.LoadConfig<SolutionProject>(Path.Combine(baseDir, "project.json"));

            // 2. 预加载所有流程到内存（高性能切换的基础）
            WorkflowCache.Clear();
            foreach (var wfFile in CurrentSolution.WorkflowFiles)
            {
                var wf = JsonConfigHelper.LoadConfig<WorkflowDefine>(Path.Combine(baseDir, "Workflows", wfFile));
                WorkflowCache.Add(wf.Name, wf);
            }
        }

        public void SaveCurrentSolution()
        {
            // 序列化 CurrentSolution 和所有 Workflows 到对应的目录
        }
    }
}
