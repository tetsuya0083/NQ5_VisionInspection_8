using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataManager
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        public string iniManager = string.Empty;
        private void MainForm_Load(object sender, EventArgs e)
        {
            iniManager = Util.GetWorkingDirectory() + "\\DataManager.ini";
            LoadValues();

            timerWork.Start();
        }

        public string FolderOriginal = string.Empty;
        public string FolderResult = string.Empty;
        public int DayDeleteOriginal = 100;
        public int DayDeleteResult = 200;
        public int TimerInterval = 1;
        private void LoadValues()
        {
            txtFolderOriginal.Text = FolderOriginal = Util.GetIniFileString(iniManager, "Setup", "DataFolder", string.Empty);
            txtDayDeleteOriginal.Text = (DayDeleteOriginal = Util.GetIniFileInt(iniManager, "Setup", "DayDeleteOriginal", 10)).ToString();
            txtFolderResult.Text = FolderResult = Util.GetIniFileString(iniManager, "Setup", "SaveFolder", "C:\\VISION_DATA");
            txtDayDeleteResult.Text = (DayDeleteResult = Util.GetIniFileInt(iniManager, "Setup", "DayDeleteResult", 10)).ToString();
            txtInterval.Text = (TimerInterval = Math.Max(10, Util.GetIniFileInt(iniManager, "Setup", "Interval", 10))).ToString();

            listDrives.Items.Clear();
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady)
                    continue;

                double percent = (drive.TotalSize - drive.AvailableFreeSpace) * 1000.0 / drive.TotalSize;
                ListViewItem item = listDrives.Items.Add(drive.Name);
                item.SubItems.Add((percent / 10.0).ToString("F1"));
            }
        }

        public bool IsRunning = true;
        public int TimeRemainning = 0;
        private void timerWork_Tick(object sender, EventArgs e)
        {
            timerWork.Stop();

            LoadValues();
            DoProcess();

            timerWork.Interval = Math.Max(10, Util.GetIniFileInt(iniManager, "Setup", "Interval", 10)) * 1000;
            timerWork.Start();
        }

        private void timerTimer_Tick(object sender, EventArgs e)
        {
            TimerInterval--;
            btnStart.Text = IsRunning ? $"RUNNING({TimerInterval})" : "STOPPED";
            btnStart.BackColor = IsRunning ? Color.Lime : Color.Red;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IsRunning = !IsRunning;

            timerWork.Interval = 100;
            timerWork.Start();
        }

        private void DoProcess()
        {
            DoDeleteFile(FolderOriginal, DayDeleteOriginal);
            DoDeleteFile(FolderResult, DayDeleteResult);
        }

        private void DoDeleteFile(string folder, int day)
        {
            if (!Directory.Exists(folder))
            {
                Util.WriteLog($"Folder not found: {folder}", "Log", "DataManager");
                return;
            }

            DateTime currentDate = DateTime.Now;

            try
            {
                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    DateTime creationDate = File.GetCreationTime(file);

                    if ((currentDate - creationDate).TotalDays > day)
                    {
                        File.Delete(file);
                        Util.WriteLog($"Deleted: {file}", "Log", "DataManager");
                    }
                }

                Util.WriteLog("Process completed.", "Log", "DataManager");
            }
            catch (Exception ex)
            {
                Util.WriteLog($"File deletion error: {ex.Message}", "Log", "DataManager");
            }
        }

        private void btnBrowseDataFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = txtFolderOriginal.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string newPath = dlg.SelectedPath;
                    if (MessageBox.Show($"Set the following path?\n\n{newPath}",
                        "Confirm DataFolder", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        FolderOriginal = newPath;
                        txtFolderOriginal.Text = newPath;
                        Util.SetIniFileString(iniManager, "Setup", "DataFolder", newPath);
                    }
                }
            }
        }

        private void btnBrowseSaveFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.SelectedPath = txtFolderResult.Text;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    string newPath = dlg.SelectedPath;
                    if (MessageBox.Show($"Set the following path?\n\n{newPath}",
                        "Confirm SaveFolder", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                    {
                        FolderResult = newPath;
                        txtFolderResult.Text = newPath;
                        Util.SetIniFileString(iniManager, "Setup", "SaveFolder", newPath);
                    }
                }
            }
        }

        private int? ShowNumberInputDialog(string title, int currentValue, int minValue)
        {
            Form dlg = new Form();
            dlg.Text = title;
            dlg.Size = new Size(300, 165);
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.MaximizeBox = false;
            dlg.MinimizeBox = false;

            TextBox txt = new TextBox();
            txt.Text = currentValue.ToString();
            txt.Location = new Point(10, 10);
            txt.Size = new Size(265, 30);
            txt.TextAlign = HorizontalAlignment.Center;
            txt.KeyPress += (s, ev) => {
                if (!char.IsDigit(ev.KeyChar) && ev.KeyChar != (char)Keys.Back)
                    ev.Handled = true;
            };

            Label lblMin = new Label();
            lblMin.Text = $"Minimum: {minValue}";
            lblMin.Location = new Point(10, 45);
            lblMin.Size = new Size(265, 20);
            lblMin.TextAlign = ContentAlignment.MiddleCenter;
            lblMin.ForeColor = Color.Gray;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(10, 75);
            btnOk.Size = new Size(130, 40);

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(145, 75);
            btnCancel.Size = new Size(130, 40);

            int result = 0;
            bool confirmed = false;
            btnOk.Click += (s, ev) => {
                if (!int.TryParse(txt.Text, out int val) || val < minValue)
                {
                    MessageBox.Show($"Please enter a value of {minValue} or greater.", "Input Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt.Focus();
                    txt.SelectAll();
                    return;
                }
                result = val;
                confirmed = true;
                dlg.Close();
            };

            dlg.Controls.AddRange(new Control[] { txt, lblMin, btnOk, btnCancel });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;
            dlg.ShowDialog(this);

            return confirmed ? result : (int?)null;
        }

        private void btnSetDayDeleteOriginal_Click(object sender, EventArgs e)
        {
            int? val = ShowNumberInputDialog("Set Original File Deletion Period", DayDeleteOriginal, 1);
            if (val.HasValue)
            {
                DayDeleteOriginal = val.Value;
                txtDayDeleteOriginal.Text = val.Value.ToString();
                Util.SetIniFileString(iniManager, "Setup", "DayDeleteOriginal", val.Value.ToString());
            }
        }

        private void btnSetDayDeleteResult_Click(object sender, EventArgs e)
        {
            int? val = ShowNumberInputDialog("Set Result File Deletion Period", DayDeleteResult, 1);
            if (val.HasValue)
            {
                DayDeleteResult = val.Value;
                txtDayDeleteResult.Text = val.Value.ToString();
                Util.SetIniFileString(iniManager, "Setup", "DayDeleteResult", val.Value.ToString());
            }
        }

        private void btnSetInterval_Click(object sender, EventArgs e)
        {
            int? val = ShowNumberInputDialog("Set Execution Interval (sec)", TimerInterval, 10);
            if (val.HasValue)
            {
                TimerInterval = val.Value;
                txtInterval.Text = val.Value.ToString();
                Util.SetIniFileString(iniManager, "Setup", "Interval", val.Value.ToString());
            }
        }
    }
}
