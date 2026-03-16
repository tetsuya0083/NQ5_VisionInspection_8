using Common;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.IO;
using KEYENCE;

namespace VisionSetup
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
            Application.Run(new MainForm());
             */

            bool New;
            string formTitle = "Vision Setup";
            Mutex mut = new Mutex(true, formTitle, out New);

            if (!New)
            {
                // 실행 중인 창을 찾아 앞으로 띄워준다
                IntPtr wnd = FindWindow(null, formTitle);
                if (wnd != IntPtr.Zero)
                {
                    ShowWindow(wnd, 1);
                    SetForegroundWindow(wnd);
                }
                mut.ReleaseMutex();
                Application.Exit();
                return;
            }

            try
            {
                RunProgram();
                // 뮤텍스 릴리즈
                mut.ReleaseMutex();
            }
            catch (Exception)
            {
            }
        }

        private static bool DoInstanceExist(string formTitle)
        {
            return true;
        }

        public static string iniEquiptment = string.Empty;
        public static string EQUIPMENT = string.Empty;
        public static string iniSetup = string.Empty;
        public static string iniPLC = string.Empty;
        public static string iniCamera = string.Empty;
        public static string iniModel = string.Empty;
        public static string iniServer = string.Empty;
        private static void RunProgram()
        {
            if (!ReloadConfiguration())
                return;

            Application.Run(new ModelSetupForm());
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
            }

            EQUIPMENT = Util.GetIniFileString(iniEquiptment, "Equipment", "Code", "");
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

                AddOrUpdateEquipmentInfo(EQUIPMENT);
            }

            try
            {
                iniSetup = Util.GetWorkingDirectory() + "\\Setup" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniSetup, FileMode.OpenOrCreate)) { }
                iniPLC = Util.GetWorkingDirectory() + "\\PLC" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniPLC, FileMode.OpenOrCreate)) { }
                iniCamera = Util.GetWorkingDirectory() + "\\Camera" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniCamera, FileMode.OpenOrCreate)) { }
                iniModel = Util.GetWorkingDirectory() + "\\Models.ini";
                using (FileStream f = File.Open(iniModel, FileMode.OpenOrCreate)) { }
                iniServer = Util.GetWorkingDirectory() + "\\Server" + EQUIPMENT + ".ini";
                using (FileStream f = File.Open(iniServer, FileMode.OpenOrCreate)) { }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Create INI File failed. " + ex.Message);
                return false;
            }

            return true;
        }

        internal static bool ChangeEquipmentCode(string oldCode, string newCode, out string err)
        {
            err = string.Empty;
            EQUIPMENT = newCode;

            try
            {
                string oldSetup = Util.GetWorkingDirectory() + "\\Setup" + oldCode + ".ini";
                iniSetup = Util.GetWorkingDirectory() + "\\Setup" + EQUIPMENT + ".ini";
                File.Move(oldSetup, iniSetup);

                string oldPLC = Util.GetWorkingDirectory() + "\\PLC" + oldCode + ".ini";
                iniPLC = Util.GetWorkingDirectory() + "\\PLC" + EQUIPMENT + ".ini";
                File.Move(oldPLC, iniPLC);

                string oldCamera = Util.GetWorkingDirectory() + "\\Camera" + oldCode + ".ini";
                iniCamera = Util.GetWorkingDirectory() + "\\Camera" + EQUIPMENT + ".ini";
                File.Move(oldCamera, iniCamera);

                string oldModel = Util.GetWorkingDirectory() + "\\Model" + oldCode + ".ini";
                iniModel = Util.GetWorkingDirectory() + "\\Model" + EQUIPMENT + ".ini";
                File.Move(oldModel, iniModel);

                string oldServer = Util.GetWorkingDirectory() + "\\Server" + oldCode + ".ini";
                iniServer = Util.GetWorkingDirectory() + "\\Server" + EQUIPMENT + ".ini";
                File.Move(oldServer, iniServer);

                AddOrUpdateEquipmentInfo(EQUIPMENT);

                Util.SetIniFileString(iniEquiptment, "Equipment", "Code", EQUIPMENT);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ChangeEquipmentCode failed. " + ex.Message);
                return false;
            }

            return ReloadConfiguration();
        }

        public static bool AddOrUpdateEquipmentInfo(string equip)
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
                Util.SetIniFileString(iniEquiptment, "List", "Equipment1", equip);
            }
            else
            {
                int maxID = 0;
                bool found = false;
                foreach (var val in equips)
                {
                    if (val.Value.ToUpper() == equip)
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
                    Util.SetIniFileString(iniEquiptment, "List", key, equip);
                }
            }
            return true;
        }
    }
}
