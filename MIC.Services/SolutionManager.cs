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
    /// <summary>
    /// 方案管理器。负责方案的磁盘 IO、加载和保存，实现方案的快速切换。
    /// 将工作流预加载到内存以提高性能
    /// </summary>
    public class SolutionManager
    {
        /// <summary>
        /// 解决方案根目录路径
        /// </summary>
        private readonly string _rootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions");
        
        /// <summary>
        /// 当前已加载的方案
        /// </summary>
        public SolutionProject CurrentSolution { get; private set; }
        
        /// <summary>
        /// 工作流内存缓存。Key: 工作流名称，Value: 工作流定义对象
        /// 预加载工作流到内存以支持高速切换
        /// </summary>
        public Dictionary<string, WorkflowDefine> WorkflowCache { get; } = new Dictionary<string, WorkflowDefine>();

        /// <summary>
        /// 加载指定的方案。同时将所有工作流预加载到内存缓存中
        /// </summary>
        /// <param name="solutionName">方案名称</param>
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

        /// <summary>
        /// 保存当前方案及其所有工作流到磁盘
        /// </summary>
        public void SaveCurrentSolution()
        {
            // 序列化 CurrentSolution 和所有 Workflows 到对应的目录
        }
    }
}
