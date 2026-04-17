using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataManager
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
            Application.ThreadException += (s, ex) =>
                MessageBox.Show($"[Unhandled Thread Exception]\n{ex.Exception.Message}\n\n{ex.Exception.StackTrace}",
                    "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            AppDomain.CurrentDomain.UnhandledException += (s, ex) =>
                MessageBox.Show($"[Unhandled Exception]\n{((Exception)ex.ExceptionObject).Message}",
                    "치명적 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (DoInstanceExist("Vision Data Manager"))
            {
                Application.Exit();
            }
            else
            {
                try
                {
                    RunProgram();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"[DataManager 시작 오류]\n\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}",
                        "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"[DataManager 시작 오류]\n\n{ex.GetType().Name}: {ex.Message}\n\n{ex.StackTrace}",
                        "실행 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private static void RunProgram()
        {
            Application.Run(new MainForm());
        }
    }
}
