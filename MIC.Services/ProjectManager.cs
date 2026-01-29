using MIC.Infrastructure.Config;
using MIC.Core.Interfaces;
using MIC.Models.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MIC.Services
{
    public class ProjectManager
    {
        private readonly ILoggerService _logger;
        // 方案根目录：/bin/Debug/Solutions/
        private readonly string _solutionsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Solutions");

        public SolutionProject ActiveProject { get; private set; }
        // 内存中缓存当前加载的所有流程
        public List<WorkflowDefine> ActiveWorkflows { get; private set; } = new List<WorkflowDefine>();
        public DashboardConfig ActiveDashboard { get; private set; } // 新增：当前方案的UI配置

        public ProjectManager(ILoggerService logger)
        {
            _logger = logger;
            if (!Directory.Exists(_solutionsRoot)) Directory.CreateDirectory(_solutionsRoot);
        }

        /// <summary>
        /// 加载指定方案
        /// </summary>
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

            // 3. 加载流程文件
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
        /// 保存当前方案及所有流程
        /// </summary>
        public void SaveActiveProject()
        {
            if (ActiveProject == null) return;

            string projectDir = Path.Combine(_solutionsRoot, ActiveProject.Name);
            if (!Directory.Exists(projectDir)) Directory.CreateDirectory(projectDir);

            // 1. 保存 project.json
            string projFile = Path.Combine(projectDir, "project.json");
            JsonConfigHelper.SaveConfig(projFile, ActiveProject);

            // 2.保存 Dashboard 文件
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
        /// 新建一个流程（仅内存操作，需调用 Save 生效）
        /// </summary>
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
        /// 删除流程
        /// </summary>
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
        /// 创建新方案结构
        /// </summary>
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