using MIC.Infrastructure.Config;
using MIC.Core.Interfaces;
using MIC.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MIC.Services
{
    /// <summary>
    /// 项目/方案管理器。负责加载、保存、创建工作流和其他配置。
    /// 方案存储在 /bin/Debug/Solutions/ 目录下，每个方案包含 project.json、devices.json、dashboard.json 和 Workflows/ 目录
    /// </summary>
    public class ProjectManager
    {
        private readonly ILoggerService _logger;
        /// <summary>
        /// 方案根目录：/bin/Debug/Solutions/
        /// </summary>
        private readonly string _solutionsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions");

        /// <summary>
        /// 当前活跃的项目/方案
        /// </summary>
        public SolutionProject ActiveProject { get; private set; }
        
        /// <summary>
        /// 内存中缓存当前加载的所有工作流
        /// </summary>
        public List<WorkflowDefine> ActiveWorkflows { get; private set; } = new List<WorkflowDefine>();
        
        /// <summary>
        /// 当前方案的 UI 配置（Dashboard 配置）
        /// </summary>
        public DashboardConfig ActiveDashboard { get; private set; }

        /// <summary>
        /// 初始化项目管理器。如果解决方案根目录不存在则创建
        /// </summary>
        public ProjectManager(ILoggerService logger)
        {
            _logger = logger;
            if (!Directory.Exists(_solutionsRoot)) Directory.CreateDirectory(_solutionsRoot);
        }

        /// <summary>
        /// 加载指定的项目/方案。包括加载主配置、Dashboard 配置和所有工作流
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <exception cref="DirectoryNotFoundException">当项目目录不存在时抛出</exception>
        public void LoadProject(string projectName)
        {
            string projectDir = Path.Combine(_solutionsRoot, projectName);
            if (!Directory.Exists(projectDir)) throw new DirectoryNotFoundException($"方案目录不存在: {projectName}");

            // 1. 加载主配置
            string projFile = Path.Combine(projectDir, "project.json");
            ActiveProject = JsonConfigHelper.LoadConfig<SolutionProject>(projFile);

            // 2. 加载 Dashboard 配置
            string dbFile = Path.Combine(projectDir, ActiveProject.UIConfigFile); // 默认 dashboard.json
            if (File.Exists(dbFile))
            {
                ActiveDashboard = JsonConfigHelper.LoadConfig<DashboardConfig>(dbFile);
            }
            else
            {
                ActiveDashboard = new DashboardConfig { Title = "Default Dashboard" };
            }

            // 3. 加载工作流文件
            ActiveWorkflows.Clear();
            string wfDir = Path.Combine(projectDir, "Workflows");
            if (Directory.Exists(wfDir))
            {
                foreach (var wfFile in ActiveProject.WorkflowFiles)
                {
                    string fullPath = Path.Combine(wfDir, wfFile);
                    if (File.Exists(fullPath))
                    {
                        var wf = JsonConfigHelper.LoadConfig<WorkflowDefine>(fullPath);
                        if (wf != null) ActiveWorkflows.Add(wf);
                    }
                }
            }
            _logger.Info($"方案 [{projectName}] 加载完成");
        }

        /// <summary>
        /// 保存当前活跃的项目及所有工作流到磁盘。将内存中的配置写入相应的 JSON 文件
        /// </summary>
        public void SaveActiveProject()
        {
            if (ActiveProject == null) return;

            string projectDir = Path.Combine(_solutionsRoot, ActiveProject.Name);
            if (!Directory.Exists(projectDir)) Directory.CreateDirectory(projectDir);

            // 1. 保存 project.json
            string projFile = Path.Combine(projectDir, "project.json");
            JsonConfigHelper.SaveConfig(projFile, ActiveProject);

            // 2. 保存 Dashboard 文件
            string dbFile = Path.Combine(projectDir, ActiveProject.UIConfigFile);
            JsonConfigHelper.SaveConfig(dbFile, ActiveDashboard);

            // 3. 保存 Workflows
            string wfDir = Path.Combine(projectDir, "Workflows");
            if (!Directory.Exists(wfDir)) Directory.CreateDirectory(wfDir);

            // 清理旧文件逻辑可在此处添加（略）

            foreach (var wf in ActiveWorkflows)
            {
                // 确保文件名以 .json 结尾
                string fileName = wf.Name.EndsWith(".json") ? wf.Name : $"{wf.Name}.json";
                string fullPath = Path.Combine(wfDir, fileName);
                JsonConfigHelper.SaveConfig(fullPath, wf);
            }

            _logger.Info("方案已保存到磁盘");
        }

        /// <summary>
        /// 新建一个工作流（仅在内存中操作，需调用 SaveActiveProject 才能生效）
        /// </summary>
        /// <param name="name">工作流名称</param>
        /// <returns>创建的新工作流对象</returns>
        /// <exception cref="Exception">当工作流名称已存在时抛出</exception>
        public WorkflowDefine CreateWorkflow(string name)
        {
            if (ActiveWorkflows.Any(w => w.Name == name))
                throw new Exception("流程名称已存在");

            var newWf = new WorkflowDefine { Name = name };
            ActiveWorkflows.Add(newWf);

            // 更新主配置引用
            string fileName = $"{name}.json";
            if (!ActiveProject.WorkflowFiles.Contains(fileName))
            {
                ActiveProject.WorkflowFiles.Add(fileName);
            }
            return newWf;
        }

        /// <summary>
        /// 删除工作流。同时从内存、配置和磁盘中删除
        /// </summary>
        /// <param name="wf">要删除的工作流对象</param>
        public void RemoveWorkflow(WorkflowDefine wf)
        {
            if (ActiveWorkflows.Contains(wf))
            {
                ActiveWorkflows.Remove(wf);
                string fileName = $"{wf.Name}.json";
                ActiveProject.WorkflowFiles.Remove(fileName);

                // 可选：同时删除物理文件
                string path = Path.Combine(_solutionsRoot, ActiveProject.Name, "Workflows", fileName);
                if (File.Exists(path)) File.Delete(path);
            }
        }

        /// <summary>
        /// 创建新方案的完整目录结构。包括项目根目录、Workflows 子目录和默认配置文件
        /// </summary>
        /// <param name="projectName">新方案的名称</param>
        /// <exception cref="Exception">当方案已存在时抛出</exception>
        public void CreateNewSolutionStructure(string projectName)
        {
            string dir = Path.Combine(_solutionsRoot, projectName);
            if (Directory.Exists(dir)) throw new Exception("方案已存在");

            Directory.CreateDirectory(dir);
            Directory.CreateDirectory(Path.Combine(dir, "Workflows"));

            // 创建默认配置
            var proj = new SolutionProject
            {
                Name = projectName,
                Description = "新建方案",
                WorkflowFiles = new List<string>()
            };
            JsonConfigHelper.SaveConfig(Path.Combine(dir, "project.json"), proj);

            // 创建空的 devices.json 和 dashboard.json
            File.WriteAllText(Path.Combine(dir, "devices.json"), "[]");
            File.WriteAllText(Path.Combine(dir, "dashboard.json"), "{}");
        }
    }
}