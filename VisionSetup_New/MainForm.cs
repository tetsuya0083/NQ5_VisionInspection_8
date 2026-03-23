using Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
#if NET8_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XGCommLibDemo;
using YONGSAN_CPAD_VISION;

namespace VisionSetup
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cboEquipment_SelectedIndexChanged(object sender, EventArgs e)
        {
            //btnSetEquipment_Click(null, null);
        }

        private void btnSetEquipment_Click(object sender, EventArgs e)
        {
            SaveGeneralConfiguration();

            if (cboEquipment.Text.Trim() == string.Empty)
                return;

            string equi = cboEquipment.Text.ToUpper().Trim();
            if (equi == Program.EQUIPMENT)
                return;

            Util.SetIniFileString(Program.iniEquiptment, "Equipment", "Code", equi);
            int index = cboEquipment.Items.IndexOf(equi);
            // 새 행목 추가
            if (index < 0)
                Program.AddOrUpdateEquipmentInfo(equi);

            Program.ReloadConfiguration();

            LoadConfiguration();
        }

        private void btnChangeEquipCode_Click(object sender, EventArgs e)
        {
            if (cboEquipment.SelectedIndex < 0)
                return;

            string rename = Interaction.InputBox("Input New Equipment Code", "Change Equipment Code").ToUpper();
            int index = cboEquipment.Items.IndexOf(rename);
            if (index != -1)
            {
                MessageBox.Show(rename + " is already exist");
                return;
            }

            // 새로운 이름일 경우
            if(!Program.ChangeEquipmentCode(cboEquipment.Text, rename, out string err))
            {
                MessageBox.Show(err);
                return;
            }

            LoadConfiguration();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            cboEquipment.Items.Clear();
            Dictionary<string, string> equips = Util.GetIniFileSection(Program.iniEquiptment, "List", out bool result, out string err);
            foreach (var eq in equips)
                cboEquipment.Items.Add(eq.Value.ToUpper());

            int index = cboEquipment.Items.IndexOf(Program.EQUIPMENT);
            if (index != -1)
                cboEquipment.SelectedIndex = index;

            LoadGeneralConfiguration();

            LoadMESConfiguration();

            LoadCameraConfiguration();

            LoadPLCConfiguration();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            SaveGeneralConfiguration();

            SaveMESConfiguration();

            SaveCameraConfiguration();

            SavePLCConfiguration();

            MessageBox.Show("Succeed");
        }


        private void LoadGeneralConfiguration()
        {
            txtGeneralDataFolder.Text = Util.GetIniFileString(Program.iniSetup, "Setup", "DataFolder", "C:\\TP");
            if (Int32.TryParse(Util.GetIniFileString(Program.iniSetup, "Setup", "ImageCompressionRatio", "100"), out int imageSize))
                nmImageSize.Value = imageSize;

            txtTitleSetup.Text = Util.GetIniFileString(Program.iniSetup, "Title", "Setup", "");
            txtTitleInspection.Text = Util.GetIniFileString(Program.iniSetup, "Title", "Inspection", "");
        }

        private void SaveGeneralConfiguration()
        {
            if (txtGeneralDataFolder.Text.Length > 0 && txtGeneralDataFolder.Text[txtGeneralDataFolder.Text.Length - 1] == '\\')
                txtGeneralDataFolder.Text = txtGeneralDataFolder.Text.Substring(0, txtGeneralDataFolder.Text.Length - 1);
            if (!Directory.Exists(txtGeneralDataFolder.Text))
                MessageBox.Show("Data folder is not exist");

            Util.SetIniFileString(Program.iniSetup, "Setup", "DataFolder", txtGeneralDataFolder.Text.Trim());
            Util.SetIniFileString(Program.iniSetup, "Setup", "ImageCompressionRatio", nmImageSize.Value.ToString());
            Util.SetIniFileString(Program.iniSetup, "Title", "Setup", txtTitleSetup.Text.Trim());
            Util.SetIniFileString(Program.iniSetup, "Title", "Inspection", txtTitleInspection.Text.Trim());
        }

        private void LoadMESConfiguration()
        {
            string dataSource = Util.GetIniFileString(Program.iniServer, "Server", "Data Source", "");
            string[] split = dataSource.Split(',');
            if (split.Length > 0)
                txtDBIP.Text = split[0];
            if (split.Length > 1)
                txtDBPort.Text = split[1];
            txtDBUser.Text = Util.GetIniFileString(Program.iniServer, "Server", "User ID", "");
            txtDBPassword.Text = Util.GetIniFileString(Program.iniServer, "Server", "Password", "");
            txtDBName.Text = Util.GetIniFileString(Program.iniServer, "Server", "Initial Catalog", "");
            txtDBTable.Text = Util.GetIniFileString(Program.iniServer, "Server", "TableName", "");
        }

        private void SaveMESConfiguration()
        {
            string dataSource = txtDBIP.Text.Trim() + "," + txtDBPort.Text.Trim();
            Util.SetIniFileString(Program.iniServer, "Server", "Data Source", dataSource);
            Util.SetIniFileString(Program.iniServer, "Server", "User ID", txtDBUser.Text.Trim());
            Util.SetIniFileString(Program.iniServer, "Server", "Password", txtDBPassword.Text.Trim());
            Util.SetIniFileString(Program.iniServer, "Server", "Initial Catalog", txtDBName.Text.Trim());
            Util.SetIniFileString(Program.iniServer, "Server", "TableName", txtDBTable.Text.Trim());
        }

        private void btnConnectDB_Click(object sender, EventArgs e)
        {
            SaveMESConfiguration();
            string connString = CONFIG.GetConnectionString(Program.iniServer, out string ip);
            if (ip == string.Empty)
                return;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                return ;
            }

            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connString);
                string cmd = "SELECT * FROM " + txtDBTable.Text.Trim();
                SqlCommand qry = new SqlCommand(cmd, conn);
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {

                }
                btnConnectDB.BackColor = Color.Lime;
            }
            catch (Exception ex)
            {
                btnConnectDB.BackColor = Color.Red;
                MessageBox.Show("MES Connection Error. " + ex.Message);
            }
            finally
            {
                if (conn != null && conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public string CameraIPAddressDefault = "100.100.100.100";
        private void LoadCameraConfiguration()
        {
            listCamera.Items.Clear();
            btnCameraModify.Enabled = btnCameraDelete.Enabled = false;

            int count = Util.GetIniFileInt(Program.iniCamera, "Camera", "Count", 0);
            for (int i = 0; i < count; i++)
            {
                int idx = i + 1;
                ListViewItem item = listCamera.Items.Add(idx.ToString());
                item.SubItems.Add(Util.GetIniFileString(Program.iniCamera, "Camera", "IP" + idx.ToString(), ""));
                item.SubItems.Add(Util.GetIniFileString(Program.iniCamera, "Camera", "Port" + idx.ToString(), ""));
            }
        }

        private void SaveCameraConfiguration()
        {
            Util.SetIniFileNull(Program.iniCamera, "Camera");
            Util.SetIniFileString(Program.iniCamera, "Camera", "Count", listCamera.Items.Count.ToString());
            for (int i = 0; i < listCamera.Items.Count; i++)
            {
                ListViewItem item = listCamera.Items[i];
                int no = item.Index + 1;
                Util.SetIniFileString(Program.iniCamera, "Camera", "IP" + no.ToString(), item.SubItems[1].Text);
                Util.SetIniFileString(Program.iniCamera, "Camera", "Port" + no.ToString(), item.SubItems[2].Text);
            }
        }

        private int GetNewCameraNumber()
        {
            int count = Util.GetIniFileInt(Program.iniCamera, "Camera", "Count", 0);
            return count + 1;
        }

        private void btnCameraAdd_Click(object sender, EventArgs e)
        {
            int no = GetNewCameraNumber();
            string ip = CameraIPAddressDefault;
            int port = 8500;
            CameraForm form = new CameraForm(this, no, ip, port);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Util.SetIniFileString(Program.iniCamera, "Camera", "Count", no.ToString());
            Util.SetIniFileString(Program.iniCamera, "Camera", "IP" + no.ToString(), form.IPAddress);
            Util.SetIniFileString(Program.iniCamera, "Camera", "Port" + no.ToString(), form.CommPort.ToString());

            LoadCameraConfiguration();
        }

        private void btnCameraModify_Click(object sender, EventArgs e)
        {
            ListViewItem item = listCamera.SelectedItems[0];
            int no = item.Index + 1;
            string ip = item.SubItems[1].Text;
            int port = Int32.Parse(item.SubItems[2].Text);
            CameraForm form = new CameraForm(this, no, ip, port);
            if (form.ShowDialog() != DialogResult.OK)
                return;

            Util.SetIniFileString(Program.iniCamera, "Camera", "IP" + no.ToString(), form.IPAddress);
            Util.SetIniFileString(Program.iniCamera, "Camera", "Port" + no.ToString(), form.CommPort.ToString());

            LoadCameraConfiguration();
        }

        private void btnCameraDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to delete the selected camera?", "Delete Camera", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                return;

            listCamera.Items.Remove(listCamera.SelectedItems[0]);
            SaveCameraConfiguration();
            LoadCameraConfiguration();
        }

        private void LoadPLCConfiguration()
        {
            // Communication
            txtPLCIP.Text = Util.GetIniFileString(Program.iniPLC, "PLC", "IP", "");
            txtPLCPort.Text = Util.GetIniFileString(Program.iniPLC, "PLC", "Port", "2004");

            // Model
            txtDevTypeModel.Text = Util.GetIniFileString(Program.iniPLC, "Model", "DeviceType", "");
            txtAddressModel.Text = Util.GetIniFileString(Program.iniPLC, "Model", "Address", "");
            // Result
            txtDevTypeResult.Text = Util.GetIniFileString(Program.iniPLC, "Result", "DeviceType", "");
            txtAddressResult.Text = Util.GetIniFileString(Program.iniPLC, "Result", "Address", "");

            txtDevTypeStep.Text = txtAddressStep.Text = txtWriteStep.Text = lblReadStep.Text = string.Empty;
            txtDevTypeTrigger.Text = txtAddressTrigger.Text = txtWriteTrigger.Text = lblReadTrigger.Text = string.Empty;
            txtDevTypeFinalStep.Text = txtAddressFinalStep.Text = txtWriteFinalStep.Text = lblReadFinalStep.Text = string.Empty;

            if (listCamera.SelectedItems.Count < 1)
                return;

            if (Int32.TryParse(listCamera.SelectedItems[0].Text, out int index))
                LoadCameraPLCConfiguration(index);
        }

        private void listCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnCameraModify.Enabled = btnCameraDelete.Enabled = (listCamera.SelectedItems.Count > 0);

            LoadPLCConfiguration();
        }

        private void LoadCameraPLCConfiguration(int index)
        {
            string category = "CAMERA" + index.ToString();
            // Trigger
            txtDevTypeTrigger.Text = Util.GetIniFileString(Program.iniPLC, category + "Trigger", "DeviceType", "");
            txtAddressTrigger.Text = Util.GetIniFileString(Program.iniPLC, category + "Trigger", "Address", "");
            // Step
            txtDevTypeStep.Text = Util.GetIniFileString(Program.iniPLC, category + "Step", "DeviceType", "");
            txtAddressStep.Text = Util.GetIniFileString(Program.iniPLC, category + "Step", "Address", "");
            // Final Step
            txtDevTypeFinalStep.Text = Util.GetIniFileString(Program.iniPLC, category + "Finish", "DeviceType", "");
            txtAddressFinalStep.Text = Util.GetIniFileString(Program.iniPLC, category + "Finish", "Address", "");
        }

        private void SavePLCConfiguration()
        {
            // Communication
            Util.SetIniFileString(Program.iniPLC, "PLC", "IP", txtPLCIP.Text);
            Util.SetIniFileString(Program.iniPLC, "PLC", "Port", txtPLCPort.Text);

            // Model
            Util.SetIniFileString(Program.iniPLC, "Model", "DeviceType", txtDevTypeModel.Text);
            Util.SetIniFileString(Program.iniPLC, "Model", "Address", txtAddressModel.Text);

            // Result
            Util.SetIniFileString(Program.iniPLC, "Result", "DeviceType", txtDevTypeResult.Text);
            Util.SetIniFileString(Program.iniPLC, "Result", "Address", txtAddressResult.Text);

            if (!chkApplyAllCamera.Checked && listCamera.SelectedItems.Count < 1)
                return;

            for (int i = 0; i < listCamera.Items.Count; i++)
            {
                // 모두 적용하거나 선택된 아이템이면
                if (chkApplyAllCamera.Checked || (listCamera.SelectedItems.Count > 0 && i == listCamera.SelectedItems[0].Index))
                {
                    if (Int32.TryParse(listCamera.Items[i].Text, out int index))
                        SaveCameraPLCConfiguration(index);
                }
            }
        }

        private void SaveCameraPLCConfiguration(int index)
        {
            string category = "CAMERA" + index.ToString();
            // Trigger
            Util.SetIniFileString(Program.iniPLC, category + "Trigger", "DeviceType", txtDevTypeTrigger.Text);
            Util.SetIniFileString(Program.iniPLC, category + "Trigger", "Address", txtAddressTrigger.Text);
            // Step
            Util.SetIniFileString(Program.iniPLC, category + "Step", "DeviceType", txtDevTypeStep.Text);
            Util.SetIniFileString(Program.iniPLC, category + "Step", "Address", txtAddressStep.Text);
            // Final Step
            Util.SetIniFileString(Program.iniPLC, category + "Finish", "DeviceType", txtDevTypeFinalStep.Text);
            Util.SetIniFileString(Program.iniPLC, category + "Finish", "Address", txtAddressFinalStep.Text);
        }

        private XGCommSocket XGComm = new XGCommSocket();
        private void btnPLCConnect_Click(object sender, EventArgs e)
        {
            ConnectPLC();
        }
        private void btnModelSetup_Click(object sender, EventArgs e)
        {
            ModelSetupForm form = new ModelSetupForm();
            form.ShowDialog();
        }

        private void btnReadModel_Click(object sender, EventArgs e)
        {
            ReadPLCData("MODEL", txtDevTypeModel.Text, txtAddressModel.Text, lblReadModel);
        }

        private void btnReadStep_Click(object sender, EventArgs e)
        {
            ReadPLCData("STEP", txtDevTypeStep.Text, txtAddressStep.Text, lblReadStep);
        }

        private void btnReadTrigger_Click(object sender, EventArgs e)
        {
            ReadPLCData("TRIGGER", txtDevTypeTrigger.Text, txtAddressTrigger.Text, lblReadTrigger);
        }

        private void btnReadFinalStep_Click(object sender, EventArgs e)
        {
            ReadPLCData("FINISH", txtDevTypeFinalStep.Text, txtAddressFinalStep.Text, lblReadFinalStep);
        }

        private void btnReadResult_Click(object sender, EventArgs e)
        {
            ReadPLCData("RESULT", txtDevTypeResult.Text, txtAddressResult.Text, lblReadResult);
        }

        private void btnWriteModel_Click(object sender, EventArgs e)
        {
            WritePLCData("MODEL", txtDevTypeModel.Text, txtAddressModel.Text, txtWriteModel.Text);
        }

        private void btnWriteStep_Click(object sender, EventArgs e)
        {
            WritePLCData("STEP", txtDevTypeStep.Text, txtAddressStep.Text, txtWriteStep.Text);
        }

        private void btnWriteTrigger_Click(object sender, EventArgs e)
        {
            WritePLCData("TRIGGER", txtDevTypeTrigger.Text, txtAddressTrigger.Text, txtWriteTrigger.Text);
        }

        private void btnWriteFinalStep_Click(object sender, EventArgs e)
        {
            WritePLCData("FINISH", txtDevTypeFinalStep.Text, txtAddressFinalStep.Text, txtWriteFinalStep.Text);
        }

        private void btnWriteResult_Click(object sender, EventArgs e)
        {
            WritePLCData("RESULT", txtDevTypeResult.Text, txtAddressResult.Text, txtWriteResult.Text);
        }

        private bool ConnectPLC()
        {
            string ip = txtPLCIP.Text.Trim();
            if (!Util.PingTest(ip))
            {
                lblPLCStatus.BackColor = Color.Red;
                return false;
            }
            string port = txtPLCPort.Text;
            int retry = 0;
            bool success = false;
            while (retry < 5)
            {
                if (XGComm.Connect(ip, Convert.ToInt32(port)) != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lblPLCStatus.Text = "DISCONNECTED";
                    lblPLCStatus.BackColor = Color.Red;
                    success = false;
                    retry++;
                    continue;
                }
                else
                {
                    lblPLCStatus.Text = "CONNECTED";
                    lblPLCStatus.BackColor = Color.Lime;
                    success = true;
                    break;
                }
            }
            return success;
        }

        private void ReadPLCData(string data, string devType, string address, Label lbl)
        {
            bool success = ConnectPLC();
            if (!success)
            {
                MessageBox.Show("PLC Connection FAILED");
                return;
            }
            if (devType.Length < 1)
                return;

            char cDeviceType = devType[0];
            UInt16[] bufRead = new UInt16[1];
            if (Int64.TryParse(address, out long addr))
            {
                uint uReturn = XGComm.ReadDataWord(cDeviceType, addr, 1, false, bufRead);
                if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    lbl.Text = bufRead[0].ToString();
                }
                else
                {
                    MessageBox.Show("ReadPLCData Failed. " + devType + address.ToString());
                }
            }
            else
                MessageBox.Show("Input valid value");
        }

        private void WritePLCData(string data, string devType, string address, string value)
        {
            bool success = ConnectPLC();
            if (!success)
            {
                MessageBox.Show("PLC Connection FAILED");
                return;
            }
            UInt16[] uWrite = new UInt16[1];
            if (UInt16.TryParse(value, out uWrite[0]) && devType.Length > 0)
            {
                char cDeviceType = devType[0];
                if (Int32.TryParse(address, out int addr))
                {
                    uint uReturn = XGComm.WriteDataWord(cDeviceType, addr, 1, false, uWrite);
                    if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                    }
                    else
                    {
                        MessageBox.Show("WritePLCData Failed. " + devType + address.ToString() + " : " + value.ToString());
                    }
                }
            }
            else
                MessageBox.Show("Input valid value");
        }

    }
}
