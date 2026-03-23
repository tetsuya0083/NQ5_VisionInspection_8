using Common;
using KEYENCE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
using YONGSAN_CPAD_VISION;

namespace VisionInspection
{
    public partial class MeasurementForm_IMG : Form
    {
        public MeasurementForm_IMG()
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
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
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

        public PictureBox[] picCams = null;
        public YONGSAN_VISION_CORE VisionCore = null;
        public SimulatorForm formSimulator = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            string iniSetup = Util.GetWorkingDirectory() + "\\Setup.ini";
            Util.SetWindowScreen(this, Util.GetIniFileInt(iniSetup, "Setup", "ScreenNo", 1));
            //this.WindowState = FormWindowState.Maximized;

            Util.SetLabelEllipse(lblPLC);
            Util.SetLabelEllipse(lblServer);

            picCams = new PictureBox[4];
            picCams[0] = pic1;
            picCams[1] = pic2;
            picCams[2] = pic3;
            picCams[3] = pic4;

            RegisterEventHandler();
            InitValues();

            formSimulator = new SimulatorForm(null, this);

            VisionCore = new YONGSAN_VISION_CORE(this, Program.EQUIPMENT, picModel);
            if (VisionCore == null || VisionCore.Initialized == false)
            {
                MessageBox.Show("Load Configuration Failed. Check the configuration from Vision Setup Program");
                this.Close();
                Process.Start(Util.GetWorkingDirectory() + "\\VisionSetup.exe");
            }
            ShowSimulatorForm();
        }

        private void RegisterEventHandler()
        {
            this.picModel.Paint += new PaintEventHandler(this.picModel_Paint);
        }

        private void InitValues()
        {
            lblModel.Text = string.Empty;
            lblResult.Text = "";
            lblResult.BackColor = Color.Gray;
            lblDateTime.Text = string.Empty;

            picModel.Image = pic1.Image = pic2.Image = pic3.Image = pic4.Image = null;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg >= 0x0400 && m.Msg < 0x7FFF)
            {
                try
                {
                    if (m.Msg == YONGSAN_VISION_CORE.WM_USER_FLICKER)
                    {
                        if (m.WParam.ToInt32() == 0)
                            timerFlicker.Stop();
                        else
                            timerFlicker.Start();
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_TEST_INIT)
                    {
                        InitValues();
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_TEST_MODELNAME)
                    {
                        string s = Marshal.PtrToStringUni(m.LParam);
                        lblModel.Text = s;

                        if (VisionCore != null && (VisionCore.CurrentModel == null || !File.Exists(VisionCore.CurrentModel.ModelImage)))
                        {
                            picModel.Left = picModel.Top = 0;
                            picModel.Width = pnModelParent.Width;
                            picModel.Height = pnModelParent.Height;
                            picModel.Visible = true;
                            return;
                        }
                        Bitmap bmImage = Util.GetBitmapFromFile(VisionCore.CurrentModel.ModelImage);
                        Util.ResizePictureBox(bmImage, picModel, pnModelParent);

                        Util.WriteLog("  ", "Log", "MEASUREMENT");
                        Util.WriteLog("#################### " + s + " #################### READY", "Log", "MEASUREMENT");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_STATUS_PLC)
                    {
                        // 1: Enabled, 0: Disabled
                        lblPLC.BackColor = (m.WParam.ToInt32() == 1) ? Color.Lime : Color.Red;
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_STATUS_SERVER)
                    {
                        lblServer.BackColor = (m.WParam.ToInt32() == 1) ? Color.Lime : Color.Red;
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_LOG_CAMERA)
                    {
                        int id = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog("ID:" + id.ToString() + " " + log, "Log", "MEASUREMENT");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_MESSAGE_CAMERA)
                    {
                        int id = m.WParam.ToInt32();
                        string log = Marshal.PtrToStringUni(m.LParam);
                        Util.WriteLog("ID:" + id.ToString() + " " + log, "Log", "MEASUREMENT");
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
                        Logging(log, (status == 0), "DAQ");
                    }
                    else if (m.Msg == YONGSAN_VISION_CORE.WM_USER_PAINT)
                    {
                        picModel.Invalidate();
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
                                picCams[index].Image = bmp;
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
                        }
                        else
                        {
                            lblResult.BackColor = Color.Lime;
                            lblResult.Text = "OK";
                        }
                        Util.WriteLog("######################################## FINISH", "Log", "MEASUREMENT");
                        Util.WriteLog("  ", "Log", "MEASUREMENT");
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

        internal void ManualModelChanged(string model)
        {
            if (IsValid())
            {
                VisionCore.ModelChanged(model, true);
            }
        }

        private bool IsValid()
        {
            if (VisionCore != null && VisionCore.Initialized == true)
                return true;
            return false;
        }

        private void picModel_Paint(object sender, PaintEventArgs e)
        {
        }

        private void timerFlicker_Tick(object sender, EventArgs e)
        {
            //if (IsValid())
            //    VisionCore.TimerFlicker();
        }

        private void picModel_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void lblModel_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void lblResult_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void lblPLC_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void pnModelParent_Click(object sender, EventArgs e)
        {
            ShowSimulatorForm();
        }

        private void ShowSimulatorForm()
        {
            if (formSimulator == null)
                formSimulator = new SimulatorForm(null, this);

            formSimulator.Show();
            formSimulator.WindowState = FormWindowState.Normal;
        }
    }
}
