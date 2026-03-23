
namespace VisionSetup
{
    partial class MainForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCameraDelete = new System.Windows.Forms.Button();
            this.btnCameraModify = new System.Windows.Forms.Button();
            this.btnCameraAdd = new System.Windows.Forms.Button();
            this.listCamera = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtPLCPort = new System.Windows.Forms.TextBox();
            this.btnPLCConnect = new System.Windows.Forms.Button();
            this.lblPLCStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPLCIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.btnWriteResult = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.txtDevTypeResult = new System.Windows.Forms.TextBox();
            this.lblReadResult = new System.Windows.Forms.Label();
            this.btnReadResult = new System.Windows.Forms.Button();
            this.txtAddressResult = new System.Windows.Forms.TextBox();
            this.txtWriteResult = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnWriteModel = new System.Windows.Forms.Button();
            this.lblReadModel = new System.Windows.Forms.Label();
            this.btnReadModel = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDevTypeModel = new System.Windows.Forms.TextBox();
            this.txtWriteModel = new System.Windows.Forms.TextBox();
            this.txtAddressModel = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.btnWriteFinalStep = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtDevTypeFinalStep = new System.Windows.Forms.TextBox();
            this.lblReadFinalStep = new System.Windows.Forms.Label();
            this.btnReadFinalStep = new System.Windows.Forms.Button();
            this.txtAddressFinalStep = new System.Windows.Forms.TextBox();
            this.txtWriteFinalStep = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btnWriteTrigger = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblReadTrigger = new System.Windows.Forms.Label();
            this.btnReadTrigger = new System.Windows.Forms.Button();
            this.txtDevTypeTrigger = new System.Windows.Forms.TextBox();
            this.txtAddressTrigger = new System.Windows.Forms.TextBox();
            this.txtWriteTrigger = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnWriteStep = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.lblReadStep = new System.Windows.Forms.Label();
            this.btnReadStep = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtDevTypeStep = new System.Windows.Forms.TextBox();
            this.txtAddressStep = new System.Windows.Forms.TextBox();
            this.txtWriteStep = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.txtDBTable = new System.Windows.Forms.TextBox();
            this.txtDBName = new System.Windows.Forms.TextBox();
            this.txtDBPassword = new System.Windows.Forms.TextBox();
            this.txtDBPort = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btnConnectDB = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDBUser = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtDBIP = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtGeneralDataFolder = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.nmImageSize = new System.Windows.Forms.NumericUpDown();
            this.cboEquipment = new System.Windows.Forms.ComboBox();
            this.btnSetEquipment = new System.Windows.Forms.Button();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.chkApplyAllCamera = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtTitleSetup = new System.Windows.Forms.TextBox();
            this.txtTitleInspection = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.btnChangeEquipCode = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmImageSize)).BeginInit();
            this.groupBox10.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCameraDelete);
            this.groupBox1.Controls.Add(this.btnCameraModify);
            this.groupBox1.Controls.Add(this.btnCameraAdd);
            this.groupBox1.Controls.Add(this.listCamera);
            this.groupBox1.Location = new System.Drawing.Point(27, 442);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(262, 210);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Setting";
            // 
            // btnCameraDelete
            // 
            this.btnCameraDelete.Enabled = false;
            this.btnCameraDelete.Location = new System.Drawing.Point(173, 164);
            this.btnCameraDelete.Name = "btnCameraDelete";
            this.btnCameraDelete.Size = new System.Drawing.Size(78, 35);
            this.btnCameraDelete.TabIndex = 3;
            this.btnCameraDelete.Text = "Delete";
            this.btnCameraDelete.UseVisualStyleBackColor = true;
            this.btnCameraDelete.Click += new System.EventHandler(this.btnCameraDelete_Click);
            // 
            // btnCameraModify
            // 
            this.btnCameraModify.Enabled = false;
            this.btnCameraModify.Location = new System.Drawing.Point(90, 164);
            this.btnCameraModify.Name = "btnCameraModify";
            this.btnCameraModify.Size = new System.Drawing.Size(78, 35);
            this.btnCameraModify.TabIndex = 2;
            this.btnCameraModify.Text = "Modify";
            this.btnCameraModify.UseVisualStyleBackColor = true;
            this.btnCameraModify.Click += new System.EventHandler(this.btnCameraModify_Click);
            // 
            // btnCameraAdd
            // 
            this.btnCameraAdd.Location = new System.Drawing.Point(6, 164);
            this.btnCameraAdd.Name = "btnCameraAdd";
            this.btnCameraAdd.Size = new System.Drawing.Size(78, 35);
            this.btnCameraAdd.TabIndex = 1;
            this.btnCameraAdd.Text = "Add";
            this.btnCameraAdd.UseVisualStyleBackColor = true;
            this.btnCameraAdd.Click += new System.EventHandler(this.btnCameraAdd_Click);
            // 
            // listCamera
            // 
            this.listCamera.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.listCamera.FullRowSelect = true;
            this.listCamera.GridLines = true;
            this.listCamera.HideSelection = false;
            this.listCamera.Location = new System.Drawing.Point(6, 25);
            this.listCamera.MultiSelect = false;
            this.listCamera.Name = "listCamera";
            this.listCamera.Size = new System.Drawing.Size(245, 133);
            this.listCamera.TabIndex = 0;
            this.listCamera.UseCompatibleStateImageBehavior = false;
            this.listCamera.View = System.Windows.Forms.View.Details;
            this.listCamera.SelectedIndexChanged += new System.EventHandler(this.listCamera_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "No.";
            this.columnHeader3.Width = 42;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "IP Address";
            this.columnHeader1.Width = 124;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Port";
            this.columnHeader2.Width = 61;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPLCPort);
            this.groupBox2.Controls.Add(this.btnPLCConnect);
            this.groupBox2.Controls.Add(this.lblPLCStatus);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtPLCIP);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(27, 234);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(262, 175);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PLC Communication Setting";
            // 
            // txtPLCPort
            // 
            this.txtPLCPort.Location = new System.Drawing.Point(117, 54);
            this.txtPLCPort.Name = "txtPLCPort";
            this.txtPLCPort.Size = new System.Drawing.Size(134, 23);
            this.txtPLCPort.TabIndex = 3;
            this.txtPLCPort.Text = " ";
            // 
            // btnPLCConnect
            // 
            this.btnPLCConnect.Location = new System.Drawing.Point(6, 83);
            this.btnPLCConnect.Name = "btnPLCConnect";
            this.btnPLCConnect.Size = new System.Drawing.Size(245, 30);
            this.btnPLCConnect.TabIndex = 4;
            this.btnPLCConnect.Text = "Connection Test";
            this.btnPLCConnect.UseVisualStyleBackColor = true;
            this.btnPLCConnect.Click += new System.EventHandler(this.btnPLCConnect_Click);
            // 
            // lblPLCStatus
            // 
            this.lblPLCStatus.BackColor = System.Drawing.Color.Red;
            this.lblPLCStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPLCStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.lblPLCStatus.Location = new System.Drawing.Point(6, 116);
            this.lblPLCStatus.Name = "lblPLCStatus";
            this.lblPLCStatus.Size = new System.Drawing.Size(245, 43);
            this.lblPLCStatus.TabIndex = 5;
            this.lblPLCStatus.Text = "Disconnected";
            this.lblPLCStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Location = new System.Drawing.Point(6, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port No.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPLCIP
            // 
            this.txtPLCIP.Location = new System.Drawing.Point(117, 25);
            this.txtPLCIP.Name = "txtPLCIP";
            this.txtPLCIP.Size = new System.Drawing.Size(134, 23);
            this.txtPLCIP.TabIndex = 1;
            this.txtPLCIP.Text = " ";
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox9);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Location = new System.Drawing.Point(295, 234);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(399, 202);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "PLC Common Data";
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.btnWriteResult);
            this.groupBox9.Controls.Add(this.label21);
            this.groupBox9.Controls.Add(this.label22);
            this.groupBox9.Controls.Add(this.txtDevTypeResult);
            this.groupBox9.Controls.Add(this.lblReadResult);
            this.groupBox9.Controls.Add(this.btnReadResult);
            this.groupBox9.Controls.Add(this.txtAddressResult);
            this.groupBox9.Controls.Add(this.txtWriteResult);
            this.groupBox9.Location = new System.Drawing.Point(12, 111);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(374, 83);
            this.groupBox9.TabIndex = 1;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "RESULT";
            // 
            // btnWriteResult
            // 
            this.btnWriteResult.Location = new System.Drawing.Point(286, 45);
            this.btnWriteResult.Name = "btnWriteResult";
            this.btnWriteResult.Size = new System.Drawing.Size(75, 23);
            this.btnWriteResult.TabIndex = 7;
            this.btnWriteResult.Text = "Write";
            this.btnWriteResult.UseVisualStyleBackColor = true;
            this.btnWriteResult.Click += new System.EventHandler(this.btnWriteResult_Click);
            // 
            // label21
            // 
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label21.Location = new System.Drawing.Point(6, 19);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(138, 23);
            this.label21.TabIndex = 0;
            this.label21.Text = "Device Type";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label22
            // 
            this.label22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label22.Location = new System.Drawing.Point(6, 48);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(138, 23);
            this.label22.TabIndex = 4;
            this.label22.Text = "Address";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDevTypeResult
            // 
            this.txtDevTypeResult.Location = new System.Drawing.Point(150, 19);
            this.txtDevTypeResult.Name = "txtDevTypeResult";
            this.txtDevTypeResult.Size = new System.Drawing.Size(49, 23);
            this.txtDevTypeResult.TabIndex = 1;
            this.txtDevTypeResult.Text = " ";
            // 
            // lblReadResult
            // 
            this.lblReadResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblReadResult.Location = new System.Drawing.Point(286, 19);
            this.lblReadResult.Name = "lblReadResult";
            this.lblReadResult.Size = new System.Drawing.Size(75, 23);
            this.lblReadResult.TabIndex = 3;
            this.lblReadResult.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadResult
            // 
            this.btnReadResult.Location = new System.Drawing.Point(205, 19);
            this.btnReadResult.Name = "btnReadResult";
            this.btnReadResult.Size = new System.Drawing.Size(75, 23);
            this.btnReadResult.TabIndex = 2;
            this.btnReadResult.Text = "Read";
            this.btnReadResult.UseVisualStyleBackColor = true;
            this.btnReadResult.Click += new System.EventHandler(this.btnReadResult_Click);
            // 
            // txtAddressResult
            // 
            this.txtAddressResult.Location = new System.Drawing.Point(150, 48);
            this.txtAddressResult.Name = "txtAddressResult";
            this.txtAddressResult.Size = new System.Drawing.Size(49, 23);
            this.txtAddressResult.TabIndex = 5;
            this.txtAddressResult.Text = " ";
            // 
            // txtWriteResult
            // 
            this.txtWriteResult.Location = new System.Drawing.Point(205, 47);
            this.txtWriteResult.Name = "txtWriteResult";
            this.txtWriteResult.Size = new System.Drawing.Size(75, 23);
            this.txtWriteResult.TabIndex = 6;
            this.txtWriteResult.Text = " ";
            this.txtWriteResult.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnWriteModel);
            this.groupBox4.Controls.Add(this.lblReadModel);
            this.groupBox4.Controls.Add(this.btnReadModel);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtDevTypeModel);
            this.groupBox4.Controls.Add(this.txtWriteModel);
            this.groupBox4.Controls.Add(this.txtAddressModel);
            this.groupBox4.Location = new System.Drawing.Point(12, 22);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(374, 83);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "MODEL";
            // 
            // btnWriteModel
            // 
            this.btnWriteModel.Location = new System.Drawing.Point(286, 45);
            this.btnWriteModel.Name = "btnWriteModel";
            this.btnWriteModel.Size = new System.Drawing.Size(75, 23);
            this.btnWriteModel.TabIndex = 7;
            this.btnWriteModel.Text = "Write";
            this.btnWriteModel.UseVisualStyleBackColor = true;
            this.btnWriteModel.Click += new System.EventHandler(this.btnWriteModel_Click);
            // 
            // lblReadModel
            // 
            this.lblReadModel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblReadModel.Location = new System.Drawing.Point(286, 19);
            this.lblReadModel.Name = "lblReadModel";
            this.lblReadModel.Size = new System.Drawing.Size(75, 23);
            this.lblReadModel.TabIndex = 3;
            this.lblReadModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadModel
            // 
            this.btnReadModel.Location = new System.Drawing.Point(205, 19);
            this.btnReadModel.Name = "btnReadModel";
            this.btnReadModel.Size = new System.Drawing.Size(75, 23);
            this.btnReadModel.TabIndex = 2;
            this.btnReadModel.Text = "Read";
            this.btnReadModel.UseVisualStyleBackColor = true;
            this.btnReadModel.Click += new System.EventHandler(this.btnReadModel_Click);
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(6, 19);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(138, 23);
            this.label9.TabIndex = 0;
            this.label9.Text = "Device Type";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label7.Location = new System.Drawing.Point(6, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(138, 23);
            this.label7.TabIndex = 4;
            this.label7.Text = "Address";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDevTypeModel
            // 
            this.txtDevTypeModel.Location = new System.Drawing.Point(150, 19);
            this.txtDevTypeModel.Name = "txtDevTypeModel";
            this.txtDevTypeModel.Size = new System.Drawing.Size(49, 23);
            this.txtDevTypeModel.TabIndex = 1;
            // 
            // txtWriteModel
            // 
            this.txtWriteModel.Location = new System.Drawing.Point(205, 47);
            this.txtWriteModel.Name = "txtWriteModel";
            this.txtWriteModel.Size = new System.Drawing.Size(75, 23);
            this.txtWriteModel.TabIndex = 6;
            this.txtWriteModel.Text = " ";
            this.txtWriteModel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtAddressModel
            // 
            this.txtAddressModel.Location = new System.Drawing.Point(150, 48);
            this.txtAddressModel.Name = "txtAddressModel";
            this.txtAddressModel.Size = new System.Drawing.Size(49, 23);
            this.txtAddressModel.TabIndex = 5;
            this.txtAddressModel.Text = " ";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.btnWriteFinalStep);
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.label10);
            this.groupBox7.Controls.Add(this.txtDevTypeFinalStep);
            this.groupBox7.Controls.Add(this.lblReadFinalStep);
            this.groupBox7.Controls.Add(this.btnReadFinalStep);
            this.groupBox7.Controls.Add(this.txtAddressFinalStep);
            this.groupBox7.Controls.Add(this.txtWriteFinalStep);
            this.groupBox7.Location = new System.Drawing.Point(12, 227);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(374, 83);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "FINAL STEP";
            // 
            // btnWriteFinalStep
            // 
            this.btnWriteFinalStep.Location = new System.Drawing.Point(286, 45);
            this.btnWriteFinalStep.Name = "btnWriteFinalStep";
            this.btnWriteFinalStep.Size = new System.Drawing.Size(75, 23);
            this.btnWriteFinalStep.TabIndex = 7;
            this.btnWriteFinalStep.Text = "Write";
            this.btnWriteFinalStep.UseVisualStyleBackColor = true;
            this.btnWriteFinalStep.Click += new System.EventHandler(this.btnWriteFinalStep_Click);
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label8.Location = new System.Drawing.Point(6, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(138, 23);
            this.label8.TabIndex = 0;
            this.label8.Text = "Device Type";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label10.Location = new System.Drawing.Point(6, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(138, 23);
            this.label10.TabIndex = 4;
            this.label10.Text = "Address";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDevTypeFinalStep
            // 
            this.txtDevTypeFinalStep.Location = new System.Drawing.Point(150, 19);
            this.txtDevTypeFinalStep.Name = "txtDevTypeFinalStep";
            this.txtDevTypeFinalStep.Size = new System.Drawing.Size(49, 23);
            this.txtDevTypeFinalStep.TabIndex = 1;
            this.txtDevTypeFinalStep.Text = " ";
            // 
            // lblReadFinalStep
            // 
            this.lblReadFinalStep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblReadFinalStep.Location = new System.Drawing.Point(286, 19);
            this.lblReadFinalStep.Name = "lblReadFinalStep";
            this.lblReadFinalStep.Size = new System.Drawing.Size(75, 23);
            this.lblReadFinalStep.TabIndex = 3;
            this.lblReadFinalStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadFinalStep
            // 
            this.btnReadFinalStep.Location = new System.Drawing.Point(205, 19);
            this.btnReadFinalStep.Name = "btnReadFinalStep";
            this.btnReadFinalStep.Size = new System.Drawing.Size(75, 23);
            this.btnReadFinalStep.TabIndex = 2;
            this.btnReadFinalStep.Text = "Read";
            this.btnReadFinalStep.UseVisualStyleBackColor = true;
            this.btnReadFinalStep.Click += new System.EventHandler(this.btnReadFinalStep_Click);
            // 
            // txtAddressFinalStep
            // 
            this.txtAddressFinalStep.Location = new System.Drawing.Point(150, 48);
            this.txtAddressFinalStep.Name = "txtAddressFinalStep";
            this.txtAddressFinalStep.Size = new System.Drawing.Size(49, 23);
            this.txtAddressFinalStep.TabIndex = 5;
            this.txtAddressFinalStep.Text = " ";
            // 
            // txtWriteFinalStep
            // 
            this.txtWriteFinalStep.Location = new System.Drawing.Point(205, 47);
            this.txtWriteFinalStep.Name = "txtWriteFinalStep";
            this.txtWriteFinalStep.Size = new System.Drawing.Size(75, 23);
            this.txtWriteFinalStep.TabIndex = 6;
            this.txtWriteFinalStep.Text = " ";
            this.txtWriteFinalStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.btnWriteTrigger);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.label4);
            this.groupBox6.Controls.Add(this.lblReadTrigger);
            this.groupBox6.Controls.Add(this.btnReadTrigger);
            this.groupBox6.Controls.Add(this.txtDevTypeTrigger);
            this.groupBox6.Controls.Add(this.txtAddressTrigger);
            this.groupBox6.Controls.Add(this.txtWriteTrigger);
            this.groupBox6.Location = new System.Drawing.Point(12, 138);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(374, 83);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "TRIGGER";
            // 
            // btnWriteTrigger
            // 
            this.btnWriteTrigger.Location = new System.Drawing.Point(286, 45);
            this.btnWriteTrigger.Name = "btnWriteTrigger";
            this.btnWriteTrigger.Size = new System.Drawing.Size(75, 23);
            this.btnWriteTrigger.TabIndex = 7;
            this.btnWriteTrigger.Text = "Write";
            this.btnWriteTrigger.UseVisualStyleBackColor = true;
            this.btnWriteTrigger.Click += new System.EventHandler(this.btnWriteTrigger_Click);
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Location = new System.Drawing.Point(6, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 23);
            this.label3.TabIndex = 0;
            this.label3.Text = "Device Type";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(6, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 23);
            this.label4.TabIndex = 4;
            this.label4.Text = "Address";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReadTrigger
            // 
            this.lblReadTrigger.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblReadTrigger.Location = new System.Drawing.Point(286, 19);
            this.lblReadTrigger.Name = "lblReadTrigger";
            this.lblReadTrigger.Size = new System.Drawing.Size(75, 23);
            this.lblReadTrigger.TabIndex = 3;
            this.lblReadTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadTrigger
            // 
            this.btnReadTrigger.Location = new System.Drawing.Point(205, 19);
            this.btnReadTrigger.Name = "btnReadTrigger";
            this.btnReadTrigger.Size = new System.Drawing.Size(75, 23);
            this.btnReadTrigger.TabIndex = 2;
            this.btnReadTrigger.Text = "Read";
            this.btnReadTrigger.UseVisualStyleBackColor = true;
            this.btnReadTrigger.Click += new System.EventHandler(this.btnReadTrigger_Click);
            // 
            // txtDevTypeTrigger
            // 
            this.txtDevTypeTrigger.Location = new System.Drawing.Point(150, 19);
            this.txtDevTypeTrigger.Name = "txtDevTypeTrigger";
            this.txtDevTypeTrigger.Size = new System.Drawing.Size(49, 23);
            this.txtDevTypeTrigger.TabIndex = 1;
            this.txtDevTypeTrigger.Text = " ";
            // 
            // txtAddressTrigger
            // 
            this.txtAddressTrigger.Location = new System.Drawing.Point(150, 48);
            this.txtAddressTrigger.Name = "txtAddressTrigger";
            this.txtAddressTrigger.Size = new System.Drawing.Size(49, 23);
            this.txtAddressTrigger.TabIndex = 5;
            this.txtAddressTrigger.Text = " ";
            // 
            // txtWriteTrigger
            // 
            this.txtWriteTrigger.Location = new System.Drawing.Point(205, 47);
            this.txtWriteTrigger.Name = "txtWriteTrigger";
            this.txtWriteTrigger.Size = new System.Drawing.Size(75, 23);
            this.txtWriteTrigger.TabIndex = 6;
            this.txtWriteTrigger.Text = " ";
            this.txtWriteTrigger.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnWriteStep);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.lblReadStep);
            this.groupBox5.Controls.Add(this.btnReadStep);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.txtDevTypeStep);
            this.groupBox5.Controls.Add(this.txtAddressStep);
            this.groupBox5.Controls.Add(this.txtWriteStep);
            this.groupBox5.Location = new System.Drawing.Point(12, 49);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(374, 83);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "PROGRAM NUMBER";
            // 
            // btnWriteStep
            // 
            this.btnWriteStep.Location = new System.Drawing.Point(286, 45);
            this.btnWriteStep.Name = "btnWriteStep";
            this.btnWriteStep.Size = new System.Drawing.Size(75, 23);
            this.btnWriteStep.TabIndex = 7;
            this.btnWriteStep.Text = "Write";
            this.btnWriteStep.UseVisualStyleBackColor = true;
            this.btnWriteStep.Click += new System.EventHandler(this.btnWriteStep_Click);
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label11.Location = new System.Drawing.Point(6, 19);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(138, 23);
            this.label11.TabIndex = 0;
            this.label11.Text = "Device Type";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblReadStep
            // 
            this.lblReadStep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblReadStep.Location = new System.Drawing.Point(286, 19);
            this.lblReadStep.Name = "lblReadStep";
            this.lblReadStep.Size = new System.Drawing.Size(75, 23);
            this.lblReadStep.TabIndex = 3;
            this.lblReadStep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnReadStep
            // 
            this.btnReadStep.Location = new System.Drawing.Point(205, 19);
            this.btnReadStep.Name = "btnReadStep";
            this.btnReadStep.Size = new System.Drawing.Size(75, 23);
            this.btnReadStep.TabIndex = 2;
            this.btnReadStep.Text = "Read";
            this.btnReadStep.UseVisualStyleBackColor = true;
            this.btnReadStep.Click += new System.EventHandler(this.btnReadStep_Click);
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label12.Location = new System.Drawing.Point(6, 48);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(138, 23);
            this.label12.TabIndex = 4;
            this.label12.Text = "Address";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDevTypeStep
            // 
            this.txtDevTypeStep.Location = new System.Drawing.Point(150, 19);
            this.txtDevTypeStep.Name = "txtDevTypeStep";
            this.txtDevTypeStep.Size = new System.Drawing.Size(49, 23);
            this.txtDevTypeStep.TabIndex = 1;
            this.txtDevTypeStep.Text = " ";
            // 
            // txtAddressStep
            // 
            this.txtAddressStep.Location = new System.Drawing.Point(150, 48);
            this.txtAddressStep.Name = "txtAddressStep";
            this.txtAddressStep.Size = new System.Drawing.Size(49, 23);
            this.txtAddressStep.TabIndex = 5;
            this.txtAddressStep.Text = " ";
            // 
            // txtWriteStep
            // 
            this.txtWriteStep.Location = new System.Drawing.Point(205, 47);
            this.txtWriteStep.Name = "txtWriteStep";
            this.txtWriteStep.Size = new System.Drawing.Size(75, 23);
            this.txtWriteStep.TabIndex = 6;
            this.txtWriteStep.Text = " ";
            this.txtWriteStep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(748, 721);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(145, 38);
            this.btnClose.TabIndex = 19;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(748, 677);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(145, 38);
            this.btnApply.TabIndex = 18;
            this.btnApply.Text = "SAVE";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.txtDBTable);
            this.groupBox8.Controls.Add(this.txtDBName);
            this.groupBox8.Controls.Add(this.txtDBPassword);
            this.groupBox8.Controls.Add(this.txtDBPort);
            this.groupBox8.Controls.Add(this.label17);
            this.groupBox8.Controls.Add(this.btnConnectDB);
            this.groupBox8.Controls.Add(this.label16);
            this.groupBox8.Controls.Add(this.label15);
            this.groupBox8.Controls.Add(this.label6);
            this.groupBox8.Controls.Add(this.txtDBUser);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Controls.Add(this.txtDBIP);
            this.groupBox8.Controls.Add(this.label13);
            this.groupBox8.Location = new System.Drawing.Point(343, 12);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(551, 149);
            this.groupBox8.TabIndex = 15;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "MES Database Setting";
            // 
            // txtDBTable
            // 
            this.txtDBTable.Location = new System.Drawing.Point(391, 54);
            this.txtDBTable.Name = "txtDBTable";
            this.txtDBTable.Size = new System.Drawing.Size(134, 23);
            this.txtDBTable.TabIndex = 11;
            this.txtDBTable.Text = " ";
            // 
            // txtDBName
            // 
            this.txtDBName.Location = new System.Drawing.Point(391, 25);
            this.txtDBName.Name = "txtDBName";
            this.txtDBName.Size = new System.Drawing.Size(134, 23);
            this.txtDBName.TabIndex = 9;
            this.txtDBName.Text = " ";
            // 
            // txtDBPassword
            // 
            this.txtDBPassword.Location = new System.Drawing.Point(117, 112);
            this.txtDBPassword.Name = "txtDBPassword";
            this.txtDBPassword.PasswordChar = '*';
            this.txtDBPassword.Size = new System.Drawing.Size(134, 23);
            this.txtDBPassword.TabIndex = 7;
            this.txtDBPassword.Text = " ";
            // 
            // txtDBPort
            // 
            this.txtDBPort.Location = new System.Drawing.Point(117, 54);
            this.txtDBPort.Name = "txtDBPort";
            this.txtDBPort.Size = new System.Drawing.Size(134, 23);
            this.txtDBPort.TabIndex = 3;
            this.txtDBPort.Text = " ";
            // 
            // label17
            // 
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label17.Location = new System.Drawing.Point(257, 54);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(128, 23);
            this.label17.TabIndex = 10;
            this.label17.Text = "Table Name";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnConnectDB
            // 
            this.btnConnectDB.Location = new System.Drawing.Point(257, 83);
            this.btnConnectDB.Name = "btnConnectDB";
            this.btnConnectDB.Size = new System.Drawing.Size(268, 52);
            this.btnConnectDB.TabIndex = 12;
            this.btnConnectDB.Text = "Connection Test";
            this.btnConnectDB.UseVisualStyleBackColor = true;
            this.btnConnectDB.Click += new System.EventHandler(this.btnConnectDB_Click);
            // 
            // label16
            // 
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label16.Location = new System.Drawing.Point(257, 25);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(128, 23);
            this.label16.TabIndex = 8;
            this.label16.Text = "Database Name";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label15.Location = new System.Drawing.Point(6, 112);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(105, 23);
            this.label15.TabIndex = 6;
            this.label15.Text = "Password";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label6.Location = new System.Drawing.Point(6, 54);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 23);
            this.label6.TabIndex = 2;
            this.label6.Text = "Port No.";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDBUser
            // 
            this.txtDBUser.Location = new System.Drawing.Point(117, 83);
            this.txtDBUser.Name = "txtDBUser";
            this.txtDBUser.Size = new System.Drawing.Size(134, 23);
            this.txtDBUser.TabIndex = 5;
            this.txtDBUser.Text = " ";
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label14.Location = new System.Drawing.Point(6, 83);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(105, 23);
            this.label14.TabIndex = 4;
            this.label14.Text = "User Name";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDBIP
            // 
            this.txtDBIP.Location = new System.Drawing.Point(117, 25);
            this.txtDBIP.Name = "txtDBIP";
            this.txtDBIP.Size = new System.Drawing.Size(134, 23);
            this.txtDBIP.TabIndex = 1;
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label13.Location = new System.Drawing.Point(6, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(105, 23);
            this.label13.TabIndex = 0;
            this.label13.Text = "IP Address";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(27, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 23);
            this.label5.TabIndex = 0;
            this.label5.Text = "Equipment Code";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label18.Location = new System.Drawing.Point(28, 109);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(123, 23);
            this.label18.TabIndex = 4;
            this.label18.Text = "Data Folder";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtGeneralDataFolder
            // 
            this.txtGeneralDataFolder.Location = new System.Drawing.Point(156, 109);
            this.txtGeneralDataFolder.Name = "txtGeneralDataFolder";
            this.txtGeneralDataFolder.Size = new System.Drawing.Size(163, 23);
            this.txtGeneralDataFolder.TabIndex = 5;
            this.txtGeneralDataFolder.Text = " ";
            // 
            // label19
            // 
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label19.Location = new System.Drawing.Point(28, 138);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(123, 23);
            this.label19.TabIndex = 6;
            this.label19.Text = "Save Image Size";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(284, 135);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(34, 23);
            this.label20.TabIndex = 8;
            this.label20.Text = "%";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // nmImageSize
            // 
            this.nmImageSize.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmImageSize.Location = new System.Drawing.Point(156, 138);
            this.nmImageSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmImageSize.Name = "nmImageSize";
            this.nmImageSize.Size = new System.Drawing.Size(123, 23);
            this.nmImageSize.TabIndex = 7;
            this.nmImageSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nmImageSize.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // cboEquipment
            // 
            this.cboEquipment.FormattingEnabled = true;
            this.cboEquipment.Location = new System.Drawing.Point(156, 46);
            this.cboEquipment.Name = "cboEquipment";
            this.cboEquipment.Size = new System.Drawing.Size(90, 24);
            this.cboEquipment.TabIndex = 1;
            this.cboEquipment.SelectedIndexChanged += new System.EventHandler(this.cboEquipment_SelectedIndexChanged);
            // 
            // btnSetEquipment
            // 
            this.btnSetEquipment.Location = new System.Drawing.Point(252, 46);
            this.btnSetEquipment.Name = "btnSetEquipment";
            this.btnSetEquipment.Size = new System.Drawing.Size(66, 24);
            this.btnSetEquipment.TabIndex = 2;
            this.btnSetEquipment.Text = "APPLY";
            this.btnSetEquipment.UseVisualStyleBackColor = true;
            this.btnSetEquipment.Click += new System.EventHandler(this.btnSetEquipment_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.chkApplyAllCamera);
            this.groupBox10.Controls.Add(this.groupBox6);
            this.groupBox10.Controls.Add(this.groupBox5);
            this.groupBox10.Controls.Add(this.groupBox7);
            this.groupBox10.Location = new System.Drawing.Point(295, 442);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(399, 321);
            this.groupBox10.TabIndex = 17;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "PLC Camera Data";
            // 
            // chkApplyAllCamera
            // 
            this.chkApplyAllCamera.AutoSize = true;
            this.chkApplyAllCamera.Location = new System.Drawing.Point(18, 22);
            this.chkApplyAllCamera.Name = "chkApplyAllCamera";
            this.chkApplyAllCamera.Size = new System.Drawing.Size(161, 21);
            this.chkApplyAllCamera.TabIndex = 0;
            this.chkApplyAllCamera.Text = "APPLY ALL CAMERA";
            this.chkApplyAllCamera.UseVisualStyleBackColor = true;
            // 
            // label23
            // 
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label23.Location = new System.Drawing.Point(28, 167);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(123, 23);
            this.label23.TabIndex = 9;
            this.label23.Text = "Setup Title";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtTitleSetup
            // 
            this.txtTitleSetup.Location = new System.Drawing.Point(156, 167);
            this.txtTitleSetup.Name = "txtTitleSetup";
            this.txtTitleSetup.Size = new System.Drawing.Size(396, 23);
            this.txtTitleSetup.TabIndex = 10;
            this.txtTitleSetup.Text = " ";
            // 
            // txtTitleInspection
            // 
            this.txtTitleInspection.Location = new System.Drawing.Point(156, 196);
            this.txtTitleInspection.Name = "txtTitleInspection";
            this.txtTitleInspection.Size = new System.Drawing.Size(396, 23);
            this.txtTitleInspection.TabIndex = 12;
            this.txtTitleInspection.Text = " ";
            // 
            // label24
            // 
            this.label24.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label24.Location = new System.Drawing.Point(28, 196);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(123, 23);
            this.label24.TabIndex = 11;
            this.label24.Text = "Inspection Title";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnChangeEquipCode
            // 
            this.btnChangeEquipCode.Location = new System.Drawing.Point(27, 76);
            this.btnChangeEquipCode.Name = "btnChangeEquipCode";
            this.btnChangeEquipCode.Size = new System.Drawing.Size(291, 27);
            this.btnChangeEquipCode.TabIndex = 3;
            this.btnChangeEquipCode.Text = "Change Equipment Code";
            this.btnChangeEquipCode.UseVisualStyleBackColor = true;
            this.btnChangeEquipCode.Click += new System.EventHandler(this.btnChangeEquipCode_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(905, 771);
            this.Controls.Add(this.btnChangeEquipCode);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.btnSetEquipment);
            this.Controls.Add(this.cboEquipment);
            this.Controls.Add(this.nmImageSize);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txtTitleInspection);
            this.Controls.Add(this.txtTitleSetup);
            this.Controls.Add(this.txtGeneralDataFolder);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox8);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Vision System Configuration";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nmImageSize)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCameraDelete;
        private System.Windows.Forms.Button btnCameraModify;
        private System.Windows.Forms.Button btnCameraAdd;
        private System.Windows.Forms.ListView listCamera;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPLCIP;
        private System.Windows.Forms.TextBox txtPLCPort;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPLCConnect;
        private System.Windows.Forms.Label lblPLCStatus;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAddressModel;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtAddressStep;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAddressTrigger;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtAddressFinalStep;
        private System.Windows.Forms.TextBox txtDevTypeFinalStep;
        private System.Windows.Forms.TextBox txtDevTypeTrigger;
        private System.Windows.Forms.TextBox txtDevTypeStep;
        private System.Windows.Forms.TextBox txtDevTypeModel;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.TextBox txtDBName;
        private System.Windows.Forms.TextBox txtDBPassword;
        private System.Windows.Forms.TextBox txtDBPort;
        private System.Windows.Forms.Button btnConnectDB;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDBUser;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtDBIP;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDBTable;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtGeneralDataFolder;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.NumericUpDown nmImageSize;
        private System.Windows.Forms.Button btnWriteFinalStep;
        private System.Windows.Forms.Label lblReadFinalStep;
        private System.Windows.Forms.Button btnReadFinalStep;
        private System.Windows.Forms.TextBox txtWriteFinalStep;
        private System.Windows.Forms.Button btnWriteTrigger;
        private System.Windows.Forms.Label lblReadTrigger;
        private System.Windows.Forms.Button btnReadTrigger;
        private System.Windows.Forms.TextBox txtWriteTrigger;
        private System.Windows.Forms.Button btnWriteStep;
        private System.Windows.Forms.Label lblReadStep;
        private System.Windows.Forms.Button btnReadStep;
        private System.Windows.Forms.TextBox txtWriteStep;
        private System.Windows.Forms.Button btnWriteModel;
        private System.Windows.Forms.Label lblReadModel;
        private System.Windows.Forms.Button btnReadModel;
        private System.Windows.Forms.TextBox txtWriteModel;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btnWriteResult;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtDevTypeResult;
        private System.Windows.Forms.Label lblReadResult;
        private System.Windows.Forms.Button btnReadResult;
        private System.Windows.Forms.TextBox txtAddressResult;
        private System.Windows.Forms.TextBox txtWriteResult;
        private System.Windows.Forms.ComboBox cboEquipment;
        private System.Windows.Forms.Button btnSetEquipment;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.CheckBox chkApplyAllCamera;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtTitleSetup;
        private System.Windows.Forms.TextBox txtTitleInspection;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button btnChangeEquipCode;
    }
}

