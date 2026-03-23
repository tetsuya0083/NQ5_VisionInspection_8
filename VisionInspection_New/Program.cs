using Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionInspection
{
    static class Program
    {
        /// <summary>
        /// 중복 실행 방지용 DLL로딩
        /// </summary>
        /// <param name="lpClassName"></param>
        /// <param name="lpWindowName"></param>
        /// <returns></returns>
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        // Show window
        [DllImport("user32")]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        ////////////////////////////////////////////////////////////////

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ////////////////////////////////////////////////////////////////
            // 중복 실행 방지를 위해 기존 코드는 주석처리
            /*
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MeasurementForm_CA());
             */
            if (DoInstanceExist("Vision Inspection"))
            {
                Application.Exit();
            }
            else
            {
                try
                {
                    RunProgram();
                }
                catch (Exception)
                {
                }
            }
        }

        private static bool DoInstanceExist(string formTitle)
        {
            bool New;
            Mutex mut = new Mutex(true, formTitle, out New);
            if (New)
            {
                try
                {
                    RunProgram();
                    // 뮤텍스 릴리즈
                    mut.ReleaseMutex();
                }
                catch (Exception)
                {
                }
                return true;
            }
            // 실행 중인 창을 찾아 앞으로 띄워준다
            IntPtr wnd = FindWindow(null, formTitle);
            if (wnd != IntPtr.Zero)
            {
                ShowWindow(wnd, 1);
                SetForegroundWindow(wnd);
            }
            return true;
        }

        public static string iniEquiptment = string.Empty;
        public static string EQUIPMENT = string.Empty;
        private static void RunProgram()
        {
            if (!ReloadConfiguration())
                return;
            switch (EQUIPMENT)
            {
                case "E104":
                    Application.Run(new MeasurementForm_IMG());
                    break;
                case "E204":
                    Application.Run(new MeasurementForm_CA());
                    break;
                default:
                    break;
            }
        }

        public static bool ReloadConfiguration()
        {
            iniEquiptment = Util.GetWorkingDirectory() + "\\Equipment.ini";
            if (!File.Exists(iniEquiptment))
            {
                try
                {
                    using (FileStream f = File.Create(iniEquiptment))
                    {

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(iniEquiptment + " file create error. " + ex.Message);
                    return false;
                }

                MessageBox.Show("There is no configuration. Run VisionSetup program first.");
                Process.Start(Util.GetWorkingDirectory() + "\\VisionSetup.exe");
                return false;
            }

            EQUIPMENT = Util.GetIniFileString(iniEquiptment, "Equipment", "Code", "").ToUpper();
            if (EQUIPMENT.Trim() == string.Empty)
            {
                string res = Interaction.InputBox("Input equipment code.\r\n\r\nSuch as CA, IMG, MV1, etc.", "Equipment Code");
                if (res.Trim() == string.Empty)
                {
                    MessageBox.Show("You can't execute program without Equipment Code. Please lauch program again.");
                    return false;
                }
                EQUIPMENT = res.ToUpper();
                Util.SetIniFileString(iniEquiptment, "Equipment", "Code", EQUIPMENT);

                AddOrUpdateEquipmentInfo();
            }

            return true;
        }

        public static bool AddOrUpdateEquipmentInfo()
        {
            // 리스트에 없으면 추가
            Dictionary<string, string> equips = Util.GetIniFileSection(iniEquiptment, "List", out bool result, out string err);
            if (err != string.Empty)
            {
                MessageBox.Show("Read Ini File Section Error. " + iniEquiptment + ", " + err);
                return false;
            }

            if (equips.Count == 0)
            {
                Util.SetIniFileString(iniEquiptment, "List", "Equipment1", EQUIPMENT);
            }
            else
            {
                int maxID = 0;
                bool found = false;
                foreach (var val in equips)
                {
                    if (val.Value.ToUpper() == EQUIPMENT)
                    {
                        found = true;
                    }
                    if (Int32.TryParse(val.Key[val.Key.Length - 1].ToString(), out int id))
                    {
                        maxID = Math.Max(maxID, id);
                    }
                }

                // 기존에 없던 항목인 경우 
                if (found == false)
                {
                    string key = "Equipment" + (maxID + 1).ToString();
                    Util.SetIniFileString(iniEquiptment, "List", key, EQUIPMENT);
                }
            }
            return true;
        }
    }
}
