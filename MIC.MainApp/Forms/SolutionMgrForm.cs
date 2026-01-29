using MIC.Models.DTOs;
using MIC.Services;
using System;
using System.Windows.Forms;

namespace MIC.MainApp.Forms
{
// 方案管理窗体(SolutionMgrForm.cs)

//界面建议：

//TextBox: txtName(方案名), txtDesc(描述)

//ListBox: lstWorkflows(显示所有流程)

//Buttons: btnAddWorkflow, btnEditWorkflow, btnSaveAll
    public partial class SolutionMgrForm : Form
    {
        private ProjectManager _mgr => GlobalContext.ProjectManager;

        public SolutionMgrForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            if (_mgr.ActiveProject == null) return;

            txtName.Text = _mgr.ActiveProject.Name;
            txtDesc.Text = _mgr.ActiveProject.Description;

            BindWorkflowList();
        }

        private void BindWorkflowList()
        {
            lstWorkflows.DataSource = null;
            lstWorkflows.DataSource = _mgr.ActiveWorkflows;
            lstWorkflows.DisplayMember = "Name";
        }

        private void btnAddWorkflow_Click(object sender, EventArgs e)
        {
            string name = Microsoft.VisualBasic.Interaction.InputBox("请输入流程名称:", "新建流程", "NewFlow");
            if (string.IsNullOrWhiteSpace(name)) return;

            try
            {
                _mgr.CreateWorkflow(name);
                BindWorkflowList();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void btnEditWorkflow_Click(object sender, EventArgs e)
        {
            var selectedWf = lstWorkflows.SelectedItem as WorkflowDefine;
            if (selectedWf == null) return;

            // 打开流程编辑器
            using (var frm = new WorkflowEditorForm(selectedWf))
            {
                frm.ShowDialog();
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            _mgr.ActiveProject.Description = txtDesc.Text;
            // 注意：Name通常作为文件夹名，修改比较麻烦，此处暂不演示改名

            _mgr.SaveActiveProject();
            MessageBox.Show("方案及所有流程已保存！");
        }
    }
}
