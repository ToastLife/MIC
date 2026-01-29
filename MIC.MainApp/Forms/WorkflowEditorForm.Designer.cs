namespace MIC.MainApp.Forms
{
    partial class WorkflowEditorForm
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
            this.dgvSteps = new System.Windows.Forms.DataGridView();
            this.btnAddStep = new System.Windows.Forms.Button();
            this.btnRemoveStep = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSteps)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSteps
            // 
            this.dgvSteps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSteps.Location = new System.Drawing.Point(12, 12);
            this.dgvSteps.Name = "dgvSteps";
            this.dgvSteps.RowTemplate.Height = 23;
            this.dgvSteps.Size = new System.Drawing.Size(376, 335);
            this.dgvSteps.TabIndex = 0;
            // 
            // btnAddStep
            // 
            this.btnAddStep.Location = new System.Drawing.Point(54, 387);
            this.btnAddStep.Name = "btnAddStep";
            this.btnAddStep.Size = new System.Drawing.Size(75, 23);
            this.btnAddStep.TabIndex = 1;
            this.btnAddStep.Text = "button1";
            this.btnAddStep.UseVisualStyleBackColor = true;
            // 
            // btnRemoveStep
            // 
            this.btnRemoveStep.Location = new System.Drawing.Point(184, 387);
            this.btnRemoveStep.Name = "btnRemoveStep";
            this.btnRemoveStep.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveStep.TabIndex = 2;
            this.btnRemoveStep.Text = "button2";
            this.btnRemoveStep.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(313, 387);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "button3";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // WorkflowEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRemoveStep);
            this.Controls.Add(this.btnAddStep);
            this.Controls.Add(this.dgvSteps);
            this.Name = "WorkflowEditorForm";
            this.Text = "WorkflowEditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvSteps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvSteps;
        private System.Windows.Forms.Button btnAddStep;
        private System.Windows.Forms.Button btnRemoveStep;
        private System.Windows.Forms.Button btnOK;
    }
}