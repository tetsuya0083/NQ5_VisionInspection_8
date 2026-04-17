
namespace DataManager
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolderOriginal = new System.Windows.Forms.TextBox();
            this.btnBrowseDataFolder = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFolderResult = new System.Windows.Forms.TextBox();
            this.btnBrowseSaveFolder = new System.Windows.Forms.Button();
            this.listDrives = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDayDeleteOriginal = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSetDayDeleteOriginal = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDayDeleteResult = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSetDayDeleteResult = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnSetInterval = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.timerWork = new System.Windows.Forms.Timer(this.components);
            this.timerTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // notifyIcon1
            //
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "DataManager";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            //
            // label1
            //
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 30);
            this.label1.TabIndex = 1;
            this.label1.Text = "Original Pictures Folder";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtFolderOriginal
            //
            this.txtFolderOriginal.Enabled = false;
            this.txtFolderOriginal.Location = new System.Drawing.Point(16, 42);
            this.txtFolderOriginal.Name = "txtFolderOriginal";
            this.txtFolderOriginal.Size = new System.Drawing.Size(428, 36);
            this.txtFolderOriginal.TabIndex = 4;
            //
            // btnBrowseDataFolder
            //
            this.btnBrowseDataFolder.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnBrowseDataFolder.Location = new System.Drawing.Point(459, 41);
            this.btnBrowseDataFolder.Name = "btnBrowseDataFolder";
            this.btnBrowseDataFolder.Size = new System.Drawing.Size(100, 40);
            this.btnBrowseDataFolder.TabIndex = 22;
            this.btnBrowseDataFolder.Text = "Setting";
            this.btnBrowseDataFolder.UseVisualStyleBackColor = true;
            this.btnBrowseDataFolder.Click += new System.EventHandler(this.btnBrowseDataFolder_Click);
            //
            // label2
            //
            this.label2.Location = new System.Drawing.Point(14, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(297, 30);
            this.label2.TabIndex = 1;
            this.label2.Text = "Result Images Folder";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtFolderResult
            //
            this.txtFolderResult.Enabled = false;
            this.txtFolderResult.Location = new System.Drawing.Point(18, 189);
            this.txtFolderResult.Name = "txtFolderResult";
            this.txtFolderResult.Size = new System.Drawing.Size(426, 36);
            this.txtFolderResult.TabIndex = 4;
            //
            // btnBrowseSaveFolder
            //
            this.btnBrowseSaveFolder.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnBrowseSaveFolder.Location = new System.Drawing.Point(459, 189);
            this.btnBrowseSaveFolder.Name = "btnBrowseSaveFolder";
            this.btnBrowseSaveFolder.Size = new System.Drawing.Size(100, 40);
            this.btnBrowseSaveFolder.TabIndex = 23;
            this.btnBrowseSaveFolder.Text = "Setting";
            this.btnBrowseSaveFolder.UseVisualStyleBackColor = true;
            this.btnBrowseSaveFolder.Click += new System.EventHandler(this.btnBrowseSaveFolder_Click);
            //
            // listDrives
            //
            this.listDrives.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3});
            this.listDrives.FullRowSelect = true;
            this.listDrives.GridLines = true;
            this.listDrives.HideSelection = false;
            this.listDrives.Location = new System.Drawing.Point(18, 336);
            this.listDrives.MultiSelect = false;
            this.listDrives.Name = "listDrives";
            this.listDrives.Size = new System.Drawing.Size(428, 124);
            this.listDrives.TabIndex = 5;
            this.listDrives.UseCompatibleStateImageBehavior = false;
            this.listDrives.View = System.Windows.Forms.View.Details;
            //
            // columnHeader1
            //
            this.columnHeader1.Text = "Drive Name";
            this.columnHeader1.Width = 200;
            //
            // columnHeader3
            //
            this.columnHeader3.Text = "Space Available (%)";
            this.columnHeader3.Width = 200;
            //
            // label3
            //
            this.label3.Location = new System.Drawing.Point(13, 303);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(297, 30);
            this.label3.TabIndex = 1;
            this.label3.Text = "Result Images Folder";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // label5
            //
            this.label5.Location = new System.Drawing.Point(14, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(278, 36);
            this.label5.TabIndex = 6;
            this.label5.Text = "Delete Original Picture";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDayDeleteOriginal
            //
            this.txtDayDeleteOriginal.Enabled = false;
            this.txtDayDeleteOriginal.Location = new System.Drawing.Point(294, 83);
            this.txtDayDeleteOriginal.Name = "txtDayDeleteOriginal";
            this.txtDayDeleteOriginal.Size = new System.Drawing.Size(80, 36);
            this.txtDayDeleteOriginal.TabIndex = 7;
            this.txtDayDeleteOriginal.Text = "365";
            this.txtDayDeleteOriginal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label4
            //
            this.label4.Location = new System.Drawing.Point(380, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 36);
            this.label4.TabIndex = 8;
            this.label4.Text = "DAYS";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnSetDayDeleteOriginal
            //
            this.btnSetDayDeleteOriginal.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnSetDayDeleteOriginal.Location = new System.Drawing.Point(459, 83);
            this.btnSetDayDeleteOriginal.Name = "btnSetDayDeleteOriginal";
            this.btnSetDayDeleteOriginal.Size = new System.Drawing.Size(100, 40);
            this.btnSetDayDeleteOriginal.TabIndex = 24;
            this.btnSetDayDeleteOriginal.Text = "Setting";
            this.btnSetDayDeleteOriginal.UseVisualStyleBackColor = true;
            this.btnSetDayDeleteOriginal.Click += new System.EventHandler(this.btnSetDayDeleteOriginal_Click);
            //
            // label6
            //
            this.label6.Location = new System.Drawing.Point(12, 235);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(280, 27);
            this.label6.TabIndex = 6;
            this.label6.Text = "Delete Result Image";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtDayDeleteResult
            //
            this.txtDayDeleteResult.Enabled = false;
            this.txtDayDeleteResult.Location = new System.Drawing.Point(294, 235);
            this.txtDayDeleteResult.Name = "txtDayDeleteResult";
            this.txtDayDeleteResult.Size = new System.Drawing.Size(80, 36);
            this.txtDayDeleteResult.TabIndex = 9;
            this.txtDayDeleteResult.Text = "365";
            this.txtDayDeleteResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label7
            //
            this.label7.Location = new System.Drawing.Point(380, 235);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 42);
            this.label7.TabIndex = 10;
            this.label7.Text = "DAYS";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnSetDayDeleteResult
            //
            this.btnSetDayDeleteResult.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnSetDayDeleteResult.Location = new System.Drawing.Point(459, 235);
            this.btnSetDayDeleteResult.Name = "btnSetDayDeleteResult";
            this.btnSetDayDeleteResult.Size = new System.Drawing.Size(100, 40);
            this.btnSetDayDeleteResult.TabIndex = 25;
            this.btnSetDayDeleteResult.Text = "Setting";
            this.btnSetDayDeleteResult.UseVisualStyleBackColor = true;
            this.btnSetDayDeleteResult.Click += new System.EventHandler(this.btnSetDayDeleteResult_Click);
            //
            // label10
            //
            this.label10.Location = new System.Drawing.Point(14, 484);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(278, 27);
            this.label10.TabIndex = 17;
            this.label10.Text = "Data Manager Interval";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // txtInterval
            //
            this.txtInterval.Enabled = false;
            this.txtInterval.Location = new System.Drawing.Point(294, 478);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(80, 36);
            this.txtInterval.TabIndex = 18;
            this.txtInterval.Text = "10";
            this.txtInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // label11
            //
            this.label11.Location = new System.Drawing.Point(380, 486);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 27);
            this.label11.TabIndex = 19;
            this.label11.Text = "SEC";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // btnSetInterval
            //
            this.btnSetInterval.Font = new System.Drawing.Font("Calibri", 12F);
            this.btnSetInterval.Location = new System.Drawing.Point(459, 484);
            this.btnSetInterval.Name = "btnSetInterval";
            this.btnSetInterval.Size = new System.Drawing.Size(100, 30);
            this.btnSetInterval.TabIndex = 26;
            this.btnSetInterval.Text = "Setting";
            this.btnSetInterval.UseVisualStyleBackColor = true;
            this.btnSetInterval.Click += new System.EventHandler(this.btnSetInterval_Click);
            //
            // btnStart
            //
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStart.Location = new System.Drawing.Point(16, 558);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(286, 38);
            this.btnStart.TabIndex = 20;
            this.btnStart.Text = "START";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(423, 558);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(136, 38);
            this.btnClose.TabIndex = 21;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // timerWork
            //
            this.timerWork.Tick += new System.EventHandler(this.timerWork_Tick);
            //
            // timerTimer
            //
            this.timerTimer.Enabled = true;
            this.timerTimer.Interval = 1000;
            this.timerTimer.Tick += new System.EventHandler(this.timerTimer_Tick);
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 608);
            this.Controls.Add(this.btnSetInterval);
            this.Controls.Add(this.btnSetDayDeleteResult);
            this.Controls.Add(this.btnSetDayDeleteOriginal);
            this.Controls.Add(this.btnBrowseSaveFolder);
            this.Controls.Add(this.btnBrowseDataFolder);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.txtInterval);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtDayDeleteResult);
            this.Controls.Add(this.txtDayDeleteOriginal);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.listDrives);
            this.Controls.Add(this.txtFolderResult);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFolderOriginal);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.Text = "Vision Data Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFolderOriginal;
        private System.Windows.Forms.Button btnBrowseDataFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFolderResult;
        private System.Windows.Forms.Button btnBrowseSaveFolder;
        private System.Windows.Forms.ListView listDrives;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDayDeleteOriginal;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSetDayDeleteOriginal;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDayDeleteResult;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnSetDayDeleteResult;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnSetInterval;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Timer timerWork;
        private System.Windows.Forms.Timer timerTimer;
    }
}
