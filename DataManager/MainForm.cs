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

        public string iniSetup = string.Empty;
        public string iniManager = string.Empty;
        private void MainForm_Load(object sender, EventArgs e)
        {
            iniSetup = Util.GetWorkingDirectory() + "\\SetupE204.ini";
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
            txtFolderOriginal.Text = FolderOriginal = Util.GetIniFileString(iniSetup, "Setup", "DataFolder", string.Empty);
            txtDayDeleteOriginal.Text = (DayDeleteOriginal = Util.GetIniFileInt(iniManager, "Setup", "DayDeleteOriginal", 10)).ToString();
            txtFolderResult.Text = FolderResult = Util.GetIniFileString(iniSetup, "Setup", "SaveFolder", "C:\\VISION_DATA");
            txtDayDeleteResult.Text = (DayDeleteResult = Util.GetIniFileInt(iniManager, "Setup", "DayDeleteResult", 10)).ToString();
            txtInterval.Text = (TimerInterval = Util.GetIniFileInt(iniManager, "Setup", "Interval", 5)).ToString();

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

            timerWork.Interval = Util.GetIniFileInt(iniManager, "Setup", "Interval") * 1000;
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
            // 폴더가 존재하는지 확인
            if (!Directory.Exists(folder))
            {
                Util.WriteLog($"폴더가 존재하지 않습니다: {folder}", "Log", "DataManager");
                return;
            }

            // 오늘 날짜 가져오기
            DateTime currentDate = DateTime.Now;

            try
            {
                // 폴더 내의 모든 파일 검색
                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    // 파일의 생성 날짜 가져오기
                    DateTime creationDate = File.GetCreationTime(file);

                    // 생성 날짜가 90일을 초과했는지 확인
                    if ((currentDate - creationDate).TotalDays > day)
                    {
                        // 파일 삭제
                        File.Delete(file);
                        Util.WriteLog($"삭제된 파일: {file}", "Log", "DataManager");
                    }
                }

                Util.WriteLog("작업이 완료되었습니다.", "Log", "DataManager");
            }
            catch (Exception ex)
            {
                Util.WriteLog($"파일 삭제 오류 발생: {ex.Message}", "Log", "DataManager");
            }
        }
    }
}
