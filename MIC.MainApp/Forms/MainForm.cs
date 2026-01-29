using MIC.Models.DTOs;
using MIC.Infrastructure.Config;
using MIC.Models.Entities;
using MIC.Plugin.Modbus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIC.MainApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.Logger.Info("System Starting...");

                // 1. 读取配置文件
                string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "devices.json");
                if (!File.Exists(configPath))
                {
                    MessageBox.Show("Config file not found!");
                    return;
                }

                // 4. 连接所有设备
                await GlobalContext.DeviceManager.ConnectAllAsync();

                GlobalContext.Logger.Info("System Startup Complete.");
            }
            catch (Exception ex)
            {
                GlobalContext.Logger.Error("Startup Fatal Error", ex);
                MessageBox.Show("Startup Error: " + ex.Message);
            }
        }


        //private async void MainForm_Shown(object sender, EventArgs e)
        //{
        //    // 使用 SplashScreen 或 等待动画
        //    await Task.Run(() =>
        //    {
        //        // 1. 初始化所有驱动
        //        LoadDriversFromConfig();

        //        // 2. 预热数据库连接
        //        GlobalContext.DbHelper.ExecuteAsync("SELECT 1");
        //    });

        //    // 3. 绑定采集引擎事件
        //    GlobalContext.PollingService.DataReceived += (devId, addr, val) =>
        //    {
        //        // 在 UI 线程更新卡片数据
        //        this.BeginInvoke(new Action(() => {
        //            UpdateDeviceUI(devId, addr, val);
        //        }));
        //    };

        //    // 4. 自动启动采集 (根据配置)
        //    GlobalContext.PollingService.Start();
        //}

        private void MainTimer_Tick(object sender, EventArgs e)
        {
            // 定时刷新UI数据
            // 实际项目中建议使用观察者模式或事件，而不是Timer轮询
        }

        //public void SwitchProject(string newProjectName)
        //{
        //    // 1. 停止业务
        //    GlobalContext.PollingService.Stop();

        //    // 2. 清空当前内存
        //    ActiveProject = null;
        //    ActiveWorkflows.Clear();

        //    // 3. 从新路径加载
        //    LoadProject(newProjectName); // 这里会填充 ActiveProject 和 ActiveWorkflows

        //    // 4. 通知 UI 刷新
        //    // ...
        //}


        //// 订阅引擎事件
        //private void BindEngineEvents()
        //{
        //    // 性能优化：使用 Action 降低开销，使用 BeginInvoke 确保 UI 线程不卡死逻辑线程
        //    GlobalContext.WorkflowEngine.StepChanged += (s, e) => {
        //        this.BeginInvoke(new Action(() => {
        //            // 仅更新状态栏或特定的行，不要刷新整个 Grid
        //            txtCurrentStep.Text = $"步骤: {e.StepId} - {e.Message}";
        //            UpdateGridRowHighlight(e.StepId);
        //        }));
        //    };

        //    GlobalContext.WorkflowEngine.WorkflowFinished += (msg) => {
        //        this.BeginInvoke(new Action(() => {
        //            MessageBox.Show(msg, "流程反馈");
        //            btnStart.Enabled = true;
        //        }));
        //    };
        //}

        //// 方案切换时的 UI 处理
        //private void OnSolutionSwitched()
        //{
        //    // 1. 清空旧 UI
        //    flowPanelWidgets.Controls.Clear();
        //    treeViewWorkflows.Nodes.Clear();

        //    // 2. 加载新流程列表到 TreeView
        //    foreach (var wfName in GlobalContext.SolutionManager.WorkflowCache.Keys)
        //    {
        //        treeViewWorkflows.Nodes.Add(new TreeNode(wfName));
        //    }

        //    // 3. 根据新方案的 dashboard.json 重新生成实时监控卡片
        //    GenerateDynamicUI();
        //}

        //private void SubscribeEvents()
        //{
        //    // 订阅引擎事件
        //    GlobalContext.WorkflowEngine.StepChanged += (s, e) => {
        //        // 使用 BeginInvoke 异步更新 UI 线程，防止 UI 渲染拖慢执行引擎
        //        this.BeginInvoke(new Action(() => {
        //            UpdateGridHighlight(e.StepId);
        //            lblStatus.Text = $"当前步骤: {e.Message}";
        //        }));
        //    };
        //}

        // 在主窗体中，你可以根据 ActiveDashboard 动态创建控件。这保证了**“方案切换 = 界面彻底变样”**。
        //private void GenerateDashboardUI()
        //{
        //    panelMain.Controls.Clear(); // 清空旧界面

        //    foreach (var config in GlobalContext.ProjectManager.ActiveDashboard.Widgets)
        //    {
        //        Control widget = null;
        //        switch (config.Type)
        //        {
        //            case WidgetType.ValueDisplay:
        //                widget = new Label { Text = "0.00", BorderStyle = BorderStyle.FixedSingle };
        //                // 这里可以利用 Tag 绑定设备信息，由 PollingService 自动更新
        //                widget.Tag = $"{config.DeviceId}.{config.Address}";
        //                break;
        //            case WidgetType.ActionButton:
        //                widget = new Button { Text = config.Text };
        //                widget.Click += (s, e) => GlobalContext.WorkflowEngine.QuickWrite(config.DeviceId, config.Address, "1");
        //                break;
        //                // ... 其他类型 ...
        //        }

        //        if (widget != null)
        //        {
        //            widget.SetBounds(config.X * 50, config.Y * 50, config.Width * 50, config.Height * 50);
        //            panelMain.Controls.Add(widget);
        //        }
        //    }
        //}

        //private void UpdateGridHighlight(int stepId)
        //{
        //    // 性能优化：只改变变动行的颜色，不使用 dgv.Refresh()
        //    foreach (DataGridViewRow row in dgvSteps.Rows)
        //    {
        //        var item = row.DataBoundItem as WorkflowStep;
        //        if (item?.Id == stepId)
        //        {
        //            row.DefaultCellStyle.BackColor = Color.LightSkyBlue;
        //            // 自动滚动到当前行
        //            dgvSteps.FirstDisplayedScrollingRowIndex = row.Index;
        //        }
        //        else
        //        {
        //            row.DefaultCellStyle.BackColor = Color.White;
        //        }
        //    }
        //}

        //// 切换方案的 UI 触发
        //private async void cmbProjects_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    string selectedProject = cmbProjects.SelectedItem.ToString();

        //    // 1. 停止当前所有作业
        //    GlobalContext.WorkflowEngine.Stop();
        //    GlobalContext.PollingService.Stop();

        //    // 2. 加载新方案 (ProjectManager)
        //    GlobalContext.ProjectManager.LoadProject(selectedProject);

        //    // 3. 刷新流程列表 (UI)
        //    lstWorkflows.DataSource = null;
        //    lstWorkflows.DataSource = GlobalContext.ProjectManager.ActiveWorkflows;
        //    lstWorkflows.DisplayMember = "Name";

        //    // 4. 重载设备驱动 (根据新方案的 devices.json)
        //    await ReinitDevicesAsync(GlobalContext.ProjectManager.ActiveProject.DeviceConfigFile);

        //    GlobalContext.Logger.Info($"方案已切换至: {selectedProject}");
        //}
    }
}