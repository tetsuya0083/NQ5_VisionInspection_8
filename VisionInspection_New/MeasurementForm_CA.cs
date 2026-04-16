using Common;
using KEYENCE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
#if NET8_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Shared;
using XGCommLibDemo;
using YONGSAN_CPAD_VISION;

namespace VisionInspection
{
    public partial class MeasurementForm_CA : Form
    {
        public MeasurementForm_CA()
        {
            InitializeComponent();
        }

        private void label6_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
            if (e.Button == MouseButtons.Right)
            {
                this.WindowState = FormWindowState.Maximized;
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (VisionCore != null)
            {
                VisionCore.CloseObject();
                Thread.Sleep(100);
            }

            Application.Exit();
            try
            {
                Environment.Exit(0);
            }
            catch (Exception)
            {
            }
        }

        private void ShowSimulatorForm()
        {
            if (formSimulator == null)
                formSimulator = new SimulatorForm(this, null);

            formSimulator.Show();
            formSimulator.WindowState = FormWindowState.Normal;
        }

        private void lblPLC_MouseDown(object sender, MouseEventArgs e)
        {
            ShowSimulatorForm();
        }

        private void lblModel_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void lblResult_Click(object sender, EventArgs e)
        {
            // Repair Mode이므로 라벨 출력
            string iniPrint = Util.GetWorkingDirectory() + "\\Print.ini";
            Util.SetIniFileString(iniPrint, "Setup", "ModelNo", VisionCore.CurrentModel.ModelPLCNumber.ToString());
            Util.SetIniFileString(iniPrint, "Setup", "Sequence", DateTime.Now.ToString("HHmmss"));
            System.Diagnostics.Process.Start(Util.GetWorkingDirectory() + "\\LabelPrint.exe");
        }

        public void Logging(string s, bool err = false, string suffix = "Log")
        {
            Util.WriteLog(s, err ? "LogErr" : "Log", suffix);
            formLogging.Logging(s);
        }

        public LoggingForm formLogging = new LoggingForm();
        private void lblServer_Click(object sender, EventArgs e)
        {
            if (formLogging == null)
            {
                formLogging = new LoggingForm();
                formLogging.Owner = this;
            }

            formLogging.Show();
        }

        public YONGSAN_VISION_CORE VisionCore = null;
        public SimulatorForm formSimulator = null;

        public PictureBox[] picCams = null;
        public List<VisionPanel> listImagePanelLeft = new List<VisionPanel>();
        public List<VisionPanel> listImagePanelRight = new List<VisionPanel>();
        public Label[] lblSteps = new Label[2];
        public Label[] lblTriggers = new Label[2];
        public Label[] lblFinals = new Label[2];
        private void MainForm_Load(object sender, EventArgs e)
        {
            Util.WriteLog("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ PROGRAM STARTED $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$", "Log", "MEASUREMENT1");
            Util.WriteLog("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ PROGRAM STARTED $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$", "Log", "MEASUREMENT2");

            this.Left = this.Top = 0;
            this.Width = 1920;
            this.Height = 1080;
            this.Left = -1920;
            this.WindowState = FormWindowState.Maximized;

            picCams = new PictureBox[2];
            picCams[0] = pic1;
            picCams[1] = pic2;

            lblTriggers[0] = lblTrigger1;
            lblFinals[0] = lblFinal1;

            Util.SetLabelEllipse(lblPLC);
            Util.SetLabelEllipse(lblServer);

            InitValues();

            formSimulator = new SimulatorForm(this, null);

            VisionCore = new YONGSAN_VISION_CORE(this, Program.EQUIPMENT, this.picModel);
            if (VisionCore == null || VisionCore.Initialized == false)
            {
                MessageBox.Show("Load Configuration Failed. Check the configuration from Vision Setup Program");
                this.Close();
                Process.Start(Util.GetWorkingDirectory() + "\\VisionSetup.exe");
            }
            ShowSimulatorForm();

            CreateFileWatchers();

            timerPLCMonitor.Start();
        }

        public FileSystemWatcher watcher1;
        public FileSystemWatcher watcher2;
        private void CreateFileWatchers()
        {
            watcher1 = new FileSystemWatcher();
            {
                string path = $"C:\\TP\\DATA\\CAM1";
                if (Directory.Exists(path))
                {
                    // 모니터링할 폴더 설정
                    watcher1.Path = path;

                    // 모니터링할 이벤트 설정 (파일 생성)
                    watcher1.NotifyFilter = NotifyFilters.FileName;

                    // 필터 설정 (모든 파일을 모니터링)
                    watcher1.Filter = "*.*";
                    watcher1.IncludeSubdirectories = true;

                    // 이벤트 핸들러 등록
                    watcher1.Created += OnCreated1;

                    // 모니터링 시작
                    watcher1.EnableRaisingEvents = true;
                }
            }
            watcher2 = new FileSystemWatcher();
            {
                string path = $"C:\\TP\\DATA\\CAM2";
                if (Directory.Exists(path))
                {
                    // 모니터링할 폴더 설정
                    watcher2.Path = path;

                    // 모니터링할 이벤트 설정 (파일 생성)
                    watcher2.NotifyFilter = NotifyFilters.FileName;

                    // 필터 설정 (모든 파일을 모니터링)
                    watcher2.Filter = "*.*";
                    watcher2.IncludeSubdirectories = true;

                    // 이벤트 핸들러 등록
                    watcher2.Created += OnCreated2;

                    // 모니터링 시작
                    watcher2.EnableRaisingEvents = true;
                }
            }
        }

        public void OnCreated1(object sender, FileSystemEventArgs e)
        {
            int retry = 0;
            while (retry < 50)
            {
                string file = e.FullPath;
                try
                {
                    // 파일에 읽기 접근 시도
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        // 파일이 열렸다면, 생성이 완료된 것으로 간주
                        Util.WriteLog($"MAIN 1 - {e.FullPath}", "Log", "FILECREATED");
                    }
                    pic1.Image = Util.GetBitmapFromFile(e.FullPath);
                    pic1.Invalidate();
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                    retry++;
                    continue;
                }
            }
        }

        public void OnCreated2(object sender, FileSystemEventArgs e)
        {
            int retry = 0;
            while (retry < 50)
            {
                string file = e.FullPath;
                try
                {
                    // 파일에 읽기 접근 시도
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        Util.WriteLog($"MAIN 2 - {e.FullPath}", "Log", "FILECREATED");
                    }
                    // 파일이 열렸다면, 생성이 완료된 것으로 간주
                    pic2.Image = Util.GetBitmapFromFile(e.FullPath);
                    pic2.Invalidate();
                    break;
                }
                catch (Exception)
                {
                    Thread.Sleep(100);
                    retry++;
                    continue;
                }
            }
        }

        private void InitValues()
        {
            lblModel.Text = string.Empty;
            lblResult.Text = "";
            lblResult.BackColor = Color.Gray;
            lblDateTime.Text = string.Empty;

            picModel.Image = pic1.Image = pic2.Image = null;

            lblNGList.BackColor = Color.Gray;
            lblNGList.Text = string.Empty;
        }

        private bool IsValid()
        {
            if (VisionCore != null && VisionCore.Initialized == true)
                return true;
            return false;
        }

        public List<Label> labelControls = new List<Label>();
        public void ClearLabels()
        {
            foreach (var lbl in labelControls)
            {
                if (lbl.Parent != null)
                    lbl.Parent.Controls.Remove(lbl);   // 화면에서 제거
                lbl.Dispose();
            }

            labelControls.Clear();    // 리스트 비우기
        }

        private void CreateLabelList(LabelModel model)
        {
            // 1) UI Label 생성
            Label lbl = new Label()
            {
                Text = model.Text,
                AutoSize = false,
                BackColor = model.BackColor,
                ForeColor = model.ForeColor,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = ContentAlignment.MiddleCenter,
                Left = (int)Math.Ceiling(model.RelativeX * picModel.Width),
                Top = (int)Math.Ceiling(model.RelativeY * picModel.Height),
                Width = model.Width * 2,
                Height = model.Height,
            };
            // Font는 이렇게 따로 설정해야 함
            lbl.Font = new Font(model.FontName, model.FontSize);
            //lbl.Click += Lbl_Click;
            //lbl.MouseDown += Lbl_MouseDown;

            picModel.Controls.Add(lbl);
            labelControls.Add(lbl);
            lbl.Tag = model.Tag;
        }

        internal void ManualModelChanged(string model)
        {
            if (IsValid())
            {
                VisionCore.ModelChanged(model, true);
            }
        }

        private void timerPLCMonitor_Tick(object sender, EventArgs e)
        {
            if (VisionCore != null)
            {
                if (VisionCore.CameraList.Count > 1)
                {
                    try
                    {
                        lblTrigger1.Text = string.Join(",", VisionCore.Data_Trigger);
                        lblFinal1.Text = string.Join(",", VisionCore.Data_Finish);
                        int mon = Util.GetIniFileInt(VisionCore.iniSetup, "Setup", "PLCMonitor", 0);
                        if (mon == 1)
                        {
                            string s = $"VisionCore.Data_Trigger:{string.Join(",", VisionCore.Data_Trigger)}, \tVisionCore.Data_Finish:{string.Join(",", VisionCore.Data_Finish)}";
                            Logging(s, false, "SIGNAL");
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.WriteLog($"timerPLCMonitor_Tick Error. {ex.Message}", "LogErr");
                    }
                }
            }
        }

        public ResultForm formResult = null;
        protected override void WndProc(ref Message m)
        {
            // 0x0400 ~ 0x7FFF WM_USER 개발자가 임의로 정의해서 쓰는 메시지.
            if (m.Msg >= 0x0400 && m.Msg < 0x7FFF)
            {
                try
                {
                    if (m.Msg == YONGSAN_VISION_CORE.WM_USER_FLICKER)
                    {
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_TEST_INIT)
                    {
                        InitValues();
                        if (formResult != null)
                        {
                            formResult.Close();
                            formResult = null;
                        }
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_TEST_MODELNAME)
                    {
                        InitValues();
                        string s = Marshal.PtrToStringUni(m.LParam);
                        lblModel.Text = s;

                        if (VisionCore != null && (VisionCore.CurrentModel == null || !File.Exists(VisionCore.CurrentModel.ModelImage)))
                        {
                            // DockStyle.Fill 컨트롤은 크기를 수동으로 지정하지 않음
                            // (이전 코드의 picModel.Height = picModel.Parent.Height는 panel2 높이까지 포함되어
                            //  이후 label 위치 계산 오류를 유발함)
                            picModel.Visible = true;
                            return;
                        }
                        Bitmap bmImage = Util.GetBitmapFromFile(VisionCore.CurrentModel.ModelImage);
                        //Util.ResizePictureBox(bmImage, picModel, (Panel)picModel.Parent);
                        picModel.Image = bmImage;
                        picModel.Visible = true;

                        // label 생성 전 DockStyle.Fill 크기가 올바른지 보장
                        picModel.Parent?.PerformLayout();

                        ClearLabels();
                        for (int i = 0; i < VisionCore.CurrentModel.LabelList.Count; i++)
                        {
                            CreateLabelList(VisionCore.CurrentModel.LabelList[i]);
                        }

                        Util.WriteLog("  ", "Log", "MEASUREMENT1");
                        Util.WriteLog("#################### " + s + " #################### READY", "Log", "MEASUREMENT2");
                        Util.WriteLog("  ", "Log", "MEASUREMENT1");
                        Util.WriteLog("#################### " + s + " #################### READY", "Log", "MEASUREMENT2");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_STATUS_PLC)
                    {
                        // 1: Enabled, 0: Disabled
                        lblPLC.BackColor = (m.WParam.ToInt32() == 1) ? Color.Lime : Color.Red;
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_STATUS_SERVER)
                    {
                        lblServer.BackColor = (m.WParam.ToInt32() == 1) ? Color.Lime : Color.Red;
                        string log = Marshal.PtrToStringUni(m.LParam);
                        if (log.Trim().Length > 0)
                            Util.WriteLog(log, "LogErr", "SERVER");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_LOG_CAMERA)
                    {
                        // id가 1000이 넘으면 에러 로그
                        int id = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog("ID:" + id.ToString() + " " + log, "Log", "MEASUREMENT1");
                        Util.WriteLog("ID:" + id.ToString() + " " + log, "Log", "MEASUREMENT2");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_CAMERA)
                    {
                        int id = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog(log, "Log", "MEASUREMENT" + id.ToString());
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_WRONGSTEP)
                    {
                        int id = m.WParam.ToInt32();
                        string stepNo = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog("CAM ID:" + id.ToString() + " WRONG STEP NO: " + stepNo, "Log", "MEASUREMENT" + id.ToString());
                        /*
                        ListViewItem item = null;
                        if (id == 1)
                        {
                            item = listResultCam1.Items.Add(stepNo);
                            item.SubItems.Add(" ");
                            item.SubItems.Add("MISMATCH");
                            item.BackColor = Color.Red;
                            listResultCam1.EnsureVisible(item.Index);
                        }
                        else if (id == 2)
                        {
                            item = listResultCam1.Items.Add(stepNo);
                            item.SubItems.Add(" ");
                            item.SubItems.Add("MISMATCH");
                            item.BackColor = Color.Red;
                            listResultCam2.EnsureVisible(item.Index);
                        }
                        */
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_STEPRESULT)
                    {
                        int camNo = m.WParam.ToInt32();
                        string progNo = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog($"CAM{camNo} PROGRAM:{progNo}", "Log", "MEASUREMENT" + camNo.ToString());

                        foreach (var lbl in VisionCore.CurrentModel.LabelList)
                        {
                            int result = 0;
                            if (lbl.ToolList.Count > 0)
                            {
                                if (lbl.ToolList.Any(x => x.Result == 2))
                                    result = 2;
                                if (lbl.ToolList.All(x => x.Result == 1))
                                    result = 1;
                            }

                            Label label = labelControls.FirstOrDefault(x => x.Tag.ToString() == lbl.Tag) ?? null;
                            if (label != null)
                            {
                                switch (result)
                                {
                                    case 0: label.BackColor = Color.Silver; break;
                                    case 1: label.BackColor = Color.Lime; break;
                                    case 2: label.BackColor = Color.Red; label.ForeColor = Color.Cyan; break;
                                    default: break;
                                }
                            }
                        }
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_CAMERA_IMAGE)
                    {
                        int camNo = m.WParam.ToInt32();
                        string file = Marshal.PtrToStringUni(m.LParam);
                        picCams[camNo - 1].Image = Util.GetBitmapFromFile(file);
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_LOG_PLC)
                    {
                        // 1:OK, 0:NG
                        int status = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Logging(log, (status == 0), "PLC");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_GENERAL)
                    {
                        // 1:OK, 0:NG
                        int status = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Logging(log, (status == 0), "MEASUREMENT1");
                        Logging(log, (status == 0), "MEASUREMENT2");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_PAINT)
                    {
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_DATETIME)
                    {
                        string s = Marshal.PtrToStringUni(m.LParam);
                        lblDateTime.Text = s;
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_FINAL_IMAGE)
                    {
                        int index = m.WParam.ToInt32();
                        string contents = Marshal.PtrToStringUni(m.LParam);
                        string[] split = contents.Split(',');
                        if (split.Length > 1)
                        {
                            string file = split[1];
                            Bitmap bmp = Util.GetBitmapFromFile(file);
                            if (bmp != null && Int32.TryParse(split[0], out int rotate))
                            {
                                bmp.RotateFlip((RotateFlipType)rotate);
                                //picCams[index].Image = bmp;
                            }
                        }
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_RESULT)
                    {
                        int res = m.WParam.ToInt32();
                        if (res == 0)
                        {
                            lblResult.BackColor = Color.Red;
                            lblResult.Text = "NG";

                            lblNGList.BackColor = Color.Red;
                            List<string> ngList = new List<string>();

                            int count = VisionCore.CurrentModel.LabelList.Count;
                            for (int i = 0; i < count; i++)
                            {
                                LabelModel mo = VisionCore.CurrentModel.LabelList[i];
                                if (mo.ToolList.Any(x => x.Result == 2))
                                    ngList.Add(mo.Text);
                            }
                            string s = string.Join(", ", ngList.ToArray());
                            lblNGList.Text = s;

                            if (VisionCore != null)
                            {
                                if (formResult == null)
                                    formResult = new ResultForm();

                                try
                                {
                                    formResult.Visible = false;
                                    formResult.SetResultImages(VisionCore);
                                    formResult.Show();
                                }
                                catch (Exception exRe)
                                {
                                    Util.WriteLog($"Set Result Form Exception. {exRe.Message}", "LogErr", "DAQ");
                                }
                            }

                        }
                        else
                        {
                            lblResult.BackColor = Color.Lime;
                            lblResult.Text = "OK";
                            lblNGList.BackColor = Color.Green;
                            lblNGList.Text = string.Empty;
                        }
                        /*
                        try
                        {
                            // 여기서 NG가 발생한 이미지를 보여준다
                            for (int i = 0; i < VisionCore.CurrentModel.CameraList.Count; i++)
                            {
                                List<string> imageList = VisionCore.FinalImageFiles[i];
                                foreach (string f in imageList)
                                {
                                    if (f.ToUpper().Contains("NG"))
                                    {
                                        LoadNGImage(f, $"pic{i + 1}");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Util.WriteLog($"Show NG Image Exeption. {ex.Message}", "LogErr", "ImageFile");
                        }
                        */
                        Util.WriteLog("######################################## FINISH", "Log", "MEASUREMENT1");
                        Util.WriteLog("  ", "Log", "MEASUREMENT1");
                        Util.WriteLog("######################################## FINISH", "Log", "MEASUREMENT2");
                        Util.WriteLog("  ", "Log", "MEASUREMENT2");
                    }
                    else
                    {
                        int wParamValue = m.WParam.ToInt32();
                        string lParamValue = Marshal.PtrToStringUni(m.LParam);
                        {
                            string s = m.Msg.ToString() + ", " + wParamValue.ToString() + ", " + lParamValue.ToString();
                            Logging(s);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging("WndProc error. " + ex.Message + "\tMessage: WP-" + m.WParam.ToString() + ", LP-" + m.LParam.ToString());
                }
            }
            base.WndProc(ref m);
        }

        private void LoadNGImage(string file, string picName)
        {
            var pb = Owner.Controls.Find(picName, true).FirstOrDefault() as PictureBox;
            if (pb != null)
            {
                pb.Image = Util.GetBitmapFromFile(file);
            }
        }

        private void timerFlicker_Tick(object sender, EventArgs e)
        {
            if (VisionCore == null || VisionCore.CurrentModel == null)
                return;
        }
    }
}
