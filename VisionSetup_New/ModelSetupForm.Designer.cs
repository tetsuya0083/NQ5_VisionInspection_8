
namespace VisionSetup
{
    partial class ModelSetupForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelSetupForm));
            this.menuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuModelAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.menuModelRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuModelDuplicate = new System.Windows.Forms.ToolStripMenuItem();
            this.imgTree = new System.Windows.Forms.ImageList(this.components);
            this.ofd = new System.Windows.Forms.OpenFileDialog();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.canvasPanel = new System.Windows.Forms.Panel();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.treeModel = new System.Windows.Forms.TreeView();
            this.panel7 = new System.Windows.Forms.Panel();
            this.txtServerModelName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtPLCModelNumber = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnChangeImage = new System.Windows.Forms.Button();
            this.btnDeleteImage = new System.Windows.Forms.Button();
            this.lblModelTree = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSystemSetup = new System.Windows.Forms.Button();
            this.btnCloseModel = new System.Windows.Forms.Button();
            this.menuStepToolsPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTree.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.canvasPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.panel4.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuTree
            // 
            this.menuTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.menuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuModelAdd,
            this.menuModelRemove,
            this.toolStripMenuItem3,
            this.menuModelDuplicate});
            this.menuTree.Name = "menuTree";
            this.menuTree.Size = new System.Drawing.Size(178, 76);
            this.menuTree.Opening += new System.ComponentModel.CancelEventHandler(this.menuTree_Opening);
            // 
            // menuModelAdd
            // 
            this.menuModelAdd.Name = "menuModelAdd";
            this.menuModelAdd.Size = new System.Drawing.Size(177, 22);
            this.menuModelAdd.Text = "Add Model";
            this.menuModelAdd.Click += new System.EventHandler(this.menuModelAdd_Click);
            // 
            // menuModelRemove
            // 
            this.menuModelRemove.Name = "menuModelRemove";
            this.menuModelRemove.Size = new System.Drawing.Size(177, 22);
            this.menuModelRemove.Text = "Remove Model";
            this.menuModelRemove.Click += new System.EventHandler(this.menuModelRemove_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(174, 6);
            // 
            // menuModelDuplicate
            // 
            this.menuModelDuplicate.Name = "menuModelDuplicate";
            this.menuModelDuplicate.Size = new System.Drawing.Size(177, 22);
            this.menuModelDuplicate.Text = "Duplicate Model";
            this.menuModelDuplicate.Click += new System.EventHandler(this.menuModelDuplicate_Click);
            // 
            // imgTree
            // 
            this.imgTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTree.ImageStream")));
            this.imgTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTree.Images.SetKeyName(0, "ModelOn.bmp");
            this.imgTree.Images.SetKeyName(1, "Camera.png");
            this.imgTree.Images.SetKeyName(2, "StepOn.bmp");
            // 
            // ofd
            // 
            this.ofd.FileName = "openFileDialog1";
            // 
            // btnMinimize
            // 
            this.btnMinimize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinimize.BackColor = System.Drawing.Color.White;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.Location = new System.Drawing.Point(1160, 12);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(51, 49);
            this.btnMinimize.TabIndex = 0;
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.Location = new System.Drawing.Point(1217, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(51, 49);
            this.btnClose.TabIndex = 1;
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.pictureBox1);
            this.panel6.Controls.Add(this.lblTitle);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1280, 82);
            this.panel6.TabIndex = 31;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(198, 58);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 27;
            this.pictureBox1.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.MidnightBlue;
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 30F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(1280, 82);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblTitle_MouseDown);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 82);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1280, 902);
            this.panel1.TabIndex = 32;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.canvasPanel);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1278, 842);
            this.panel3.TabIndex = 21;
            // 
            // canvasPanel
            // 
            this.canvasPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.canvasPanel.Controls.Add(this.picImage);
            this.canvasPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvasPanel.Location = new System.Drawing.Point(279, 0);
            this.canvasPanel.Name = "canvasPanel";
            this.canvasPanel.Size = new System.Drawing.Size(999, 842);
            this.canvasPanel.TabIndex = 18;
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.Color.White;
            this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picImage.Location = new System.Drawing.Point(0, 0);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(997, 840);
            this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picImage.TabIndex = 23;
            this.picImage.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.treeModel);
            this.panel4.Controls.Add(this.panel7);
            this.panel4.Controls.Add(this.lblModelTree);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(279, 842);
            this.panel4.TabIndex = 17;
            // 
            // treeModel
            // 
            this.treeModel.ContextMenuStrip = this.menuTree;
            this.treeModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.treeModel.HideSelection = false;
            this.treeModel.ImageIndex = 0;
            this.treeModel.ImageList = this.imgTree;
            this.treeModel.Location = new System.Drawing.Point(0, 59);
            this.treeModel.Name = "treeModel";
            this.treeModel.SelectedImageIndex = 0;
            this.treeModel.Size = new System.Drawing.Size(277, 589);
            this.treeModel.TabIndex = 3;
            this.treeModel.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeModel_AfterSelect);
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.txtServerModelName);
            this.panel7.Controls.Add(this.label3);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Controls.Add(this.txtPLCModelNumber);
            this.panel7.Controls.Add(this.btnApply);
            this.panel7.Controls.Add(this.label4);
            this.panel7.Controls.Add(this.btnChangeImage);
            this.panel7.Controls.Add(this.btnDeleteImage);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 648);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(277, 192);
            this.panel7.TabIndex = 2;
            // 
            // txtServerModelName
            // 
            this.txtServerModelName.Location = new System.Drawing.Point(81, 5);
            this.txtServerModelName.Name = "txtServerModelName";
            this.txtServerModelName.Size = new System.Drawing.Size(190, 23);
            this.txtServerModelName.TabIndex = 1;
            this.txtServerModelName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(6, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Name";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(6, 34);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(99, 23);
            this.label9.TabIndex = 2;
            this.label9.Text = "PLC Value";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPLCModelNumber
            // 
            this.txtPLCModelNumber.Location = new System.Drawing.Point(111, 34);
            this.txtPLCModelNumber.Name = "txtPLCModelNumber";
            this.txtPLCModelNumber.Size = new System.Drawing.Size(160, 23);
            this.txtPLCModelNumber.TabIndex = 3;
            this.txtPLCModelNumber.Text = " ";
            this.txtPLCModelNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Location = new System.Drawing.Point(6, 156);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(265, 30);
            this.btnApply.TabIndex = 14;
            this.btnApply.Text = "APPLY";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(6, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 61);
            this.label4.TabIndex = 11;
            this.label4.Text = "Model Image";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnChangeImage
            // 
            this.btnChangeImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeImage.Location = new System.Drawing.Point(111, 63);
            this.btnChangeImage.Name = "btnChangeImage";
            this.btnChangeImage.Size = new System.Drawing.Size(160, 27);
            this.btnChangeImage.TabIndex = 12;
            this.btnChangeImage.Text = "Change";
            this.btnChangeImage.UseVisualStyleBackColor = true;
            this.btnChangeImage.Click += new System.EventHandler(this.btnChangeImage_Click);
            // 
            // btnDeleteImage
            // 
            this.btnDeleteImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteImage.Location = new System.Drawing.Point(111, 96);
            this.btnDeleteImage.Name = "btnDeleteImage";
            this.btnDeleteImage.Size = new System.Drawing.Size(160, 27);
            this.btnDeleteImage.TabIndex = 13;
            this.btnDeleteImage.Text = "Delete";
            this.btnDeleteImage.UseVisualStyleBackColor = true;
            this.btnDeleteImage.Click += new System.EventHandler(this.btnDeleteImage_Click);
            // 
            // lblModelTree
            // 
            this.lblModelTree.BackColor = System.Drawing.Color.White;
            this.lblModelTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblModelTree.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblModelTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblModelTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblModelTree.Location = new System.Drawing.Point(0, 0);
            this.lblModelTree.Name = "lblModelTree";
            this.lblModelTree.Size = new System.Drawing.Size(277, 59);
            this.lblModelTree.TabIndex = 0;
            this.lblModelTree.Text = "MODEL LIST";
            this.lblModelTree.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblModelTree.Click += new System.EventHandler(this.lblModelTree_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSystemSetup);
            this.panel2.Controls.Add(this.btnCloseModel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 842);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1278, 58);
            this.panel2.TabIndex = 17;
            this.panel2.Visible = false;
            // 
            // btnSystemSetup
            // 
            this.btnSystemSetup.Location = new System.Drawing.Point(11, 17);
            this.btnSystemSetup.Name = "btnSystemSetup";
            this.btnSystemSetup.Size = new System.Drawing.Size(251, 30);
            this.btnSystemSetup.TabIndex = 0;
            this.btnSystemSetup.Text = "System Configuration";
            this.btnSystemSetup.UseVisualStyleBackColor = true;
            this.btnSystemSetup.Visible = false;
            this.btnSystemSetup.Click += new System.EventHandler(this.btnSystemSetup_Click);
            // 
            // btnCloseModel
            // 
            this.btnCloseModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseModel.Location = new System.Drawing.Point(1057, 17);
            this.btnCloseModel.Name = "btnCloseModel";
            this.btnCloseModel.Size = new System.Drawing.Size(210, 30);
            this.btnCloseModel.TabIndex = 1;
            this.btnCloseModel.Text = ":: CLOSE ::";
            this.btnCloseModel.UseVisualStyleBackColor = true;
            this.btnCloseModel.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // menuStepToolsPaste
            // 
            this.menuStepToolsPaste.Enabled = false;
            this.menuStepToolsPaste.Name = "menuStepToolsPaste";
            this.menuStepToolsPaste.Size = new System.Drawing.Size(214, 22);
            this.menuStepToolsPaste.Text = "Paste Step Properties";
            // 
            // ModelSetupForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1280, 984);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnMinimize);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel6);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelSetupForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.ModelSetupForm_Load);
            this.menuTree.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.canvasPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip menuTree;
        private System.Windows.Forms.ToolStripMenuItem menuModelAdd;
        private System.Windows.Forms.ToolStripMenuItem menuModelRemove;
        private System.Windows.Forms.ImageList imgTree;
        private System.Windows.Forms.OpenFileDialog ofd;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel canvasPanel;
        private System.Windows.Forms.Button btnDeleteImage;
        private System.Windows.Forms.Button btnChangeImage;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPLCModelNumber;
        private System.Windows.Forms.TextBox txtServerModelName;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblModelTree;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSystemSetup;
        private System.Windows.Forms.Button btnCloseModel;
        private System.Windows.Forms.ToolStripMenuItem menuModelDuplicate;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.TreeView treeModel;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.PictureBox picImage;
        private System.Windows.Forms.ToolStripMenuItem menuStepToolsPaste;
    }
}