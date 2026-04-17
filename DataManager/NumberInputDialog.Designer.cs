namespace DataManager
{
    partial class NumberInputDialog
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblMinimum = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtValue
            // 
            this.txtValue.Location = new System.Drawing.Point(16, 25);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(303, 25);
            this.txtValue.TabIndex = 0;
            this.txtValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtValue_KeyPress);
            // 
            // lblMinimum
            // 
            this.lblMinimum.Font = new System.Drawing.Font("±¼¸²", 14F);
            this.lblMinimum.ForeColor = System.Drawing.Color.Gray;
            this.lblMinimum.Location = new System.Drawing.Point(12, 59);
            this.lblMinimum.Name = "lblMinimum";
            this.lblMinimum.Size = new System.Drawing.Size(307, 41);
            this.lblMinimum.TabIndex = 1;
            this.lblMinimum.Text = "Minimum: 1";
            this.lblMinimum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("±¼¸²", 12F);
            this.btnOK.Location = new System.Drawing.Point(16, 107);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(130, 40);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("±¼¸²", 12F);
            this.btnCancel.Location = new System.Drawing.Point(189, 107);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 40);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // NumberInputDialog
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(344, 175);
            this.Controls.Add(this.txtValue);
            this.Controls.Add(this.lblMinimum);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NumberInputDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Number Input";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblMinimum;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}
