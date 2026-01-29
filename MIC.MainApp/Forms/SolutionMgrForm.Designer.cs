namespace MIC.MainApp.Forms
{
    partial class SolutionMgrForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.lstWorkflows = new System.Windows.Forms.ListBox();
            this.btnAddWorkflow = new System.Windows.Forms.Button();
            this.btnEditWorkflow = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(154, 76);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 21);
            this.txtName.TabIndex = 0;
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(291, 75);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(100, 21);
            this.txtDesc.TabIndex = 1;
            // 
            // lstWorkflows
            // 
            this.lstWorkflows.FormattingEnabled = true;
            this.lstWorkflows.ItemHeight = 12;
            this.lstWorkflows.Location = new System.Drawing.Point(154, 126);
            this.lstWorkflows.Name = "lstWorkflows";
            this.lstWorkflows.Size = new System.Drawing.Size(120, 88);
            this.lstWorkflows.TabIndex = 2;
            // 
            // btnAddWorkflow
            // 
            this.btnAddWorkflow.Location = new System.Drawing.Point(154, 256);
            this.btnAddWorkflow.Name = "btnAddWorkflow";
            this.btnAddWorkflow.Size = new System.Drawing.Size(75, 23);
            this.btnAddWorkflow.TabIndex = 3;
            this.btnAddWorkflow.Text = "button1";
            this.btnAddWorkflow.UseVisualStyleBackColor = true;
            // 
            // btnEditWorkflow
            // 
            this.btnEditWorkflow.Location = new System.Drawing.Point(254, 256);
            this.btnEditWorkflow.Name = "btnEditWorkflow";
            this.btnEditWorkflow.Size = new System.Drawing.Size(75, 23);
            this.btnEditWorkflow.TabIndex = 4;
            this.btnEditWorkflow.Text = "button2";
            this.btnEditWorkflow.UseVisualStyleBackColor = true;
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.Location = new System.Drawing.Point(356, 256);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAll.TabIndex = 5;
            this.btnSaveAll.Text = "button3";
            this.btnSaveAll.UseVisualStyleBackColor = true;
            // 
            // SolutionMgrForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.btnEditWorkflow);
            this.Controls.Add(this.btnAddWorkflow);
            this.Controls.Add(this.lstWorkflows);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.txtName);
            this.Name = "SolutionMgrForm";
            this.Text = "SolutionMgrForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.ListBox lstWorkflows;
        private System.Windows.Forms.Button btnAddWorkflow;
        private System.Windows.Forms.Button btnEditWorkflow;
        private System.Windows.Forms.Button btnSaveAll;
    }
}