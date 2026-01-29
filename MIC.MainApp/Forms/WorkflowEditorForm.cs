using MIC.Models.DTOs;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace MIC.MainApp.Forms
{
    public partial class WorkflowEditorForm : Form
    {
        private WorkflowDefine _workflow;
        private BindingList<WorkflowStep> _bindingSteps;

        public WorkflowEditorForm(WorkflowDefine workflow)
        {
            InitializeComponent();
            _workflow = workflow;

            this.Text = $"编辑流程 - {_workflow.Name}";
            InitGrid();
        }

        private void InitGrid()
        {
            // 使用 BindingList 实现双向绑定
            _bindingSteps = new BindingList<WorkflowStep>(_workflow.Steps);
            dgvSteps.DataSource = _bindingSteps;
            dgvSteps.AutoGenerateColumns = false;

            // 1. ID列
            AddTextCol("Id", "ID", 40);
            // 2. 名称
            AddTextCol("Name", "步骤名称", 100);

            // 3. 动作 (ComboBox)
            var colAction = new DataGridViewComboBoxColumn();
            colAction.DataPropertyName = "Action";
            colAction.HeaderText = "动作类型";
            colAction.DataSource = Enum.GetValues(typeof(ActionType));
            dgvSteps.Columns.Add(colAction);

            // 4. 设备 (ComboBox - 动态加载)
            var colDev = new DataGridViewComboBoxColumn();
            colDev.DataPropertyName = "DeviceId";
            colDev.HeaderText = "设备ID";
            // 从 GlobalContext 获取当前已加载的设备
            colDev.DataSource = GlobalContext.DeviceManager.GetAllDevices().Select(d => d.DeviceId).ToList();
            dgvSteps.Columns.Add(colDev);

            // 5. 其他参数
            AddTextCol("Address", "地址", 80);
            AddTextCol("TargetValue", "数值/参数", 80);
            AddTextCol("TimeoutMs", "超时(ms)", 60);
            AddTextCol("NextStepId", "下一步ID", 60);
            AddTextCol("ErrorStepId", "错误跳至", 60);
        }

        private void AddTextCol(string prop, string header, int width)
        {
            dgvSteps.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop,
                HeaderText = header,
                Width = width
            });
        }

        private void btnAddStep_Click(object sender, EventArgs e)
        {
            int nextId = _bindingSteps.Any() ? _bindingSteps.Max(s => s.Id) + 10 : 10;
            _bindingSteps.Add(new WorkflowStep
            {
                Id = nextId,
                Name = "新步骤",
                Action = ActionType.Write,
                TimeoutMs = 1000,
                NextStepId = nextId + 10
            });
        }

        private void btnRemoveStep_Click(object sender, EventArgs e)
        {
            if (dgvSteps.CurrentRow?.DataBoundItem is WorkflowStep step)
            {
                _bindingSteps.Remove(step);
            }
        }

        // 关闭窗口时，因为是引用传递，修改已经反映在 _workflow 对象上了
        // 不需要额外的 Save 逻辑，只要父窗体调用 ProjectManager.SaveActiveProject() 即可
    }
}