using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Drawing;
using System.Reflection;
using System.Data;
using System.Web;
using System.Windows.Forms;

namespace Common
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NETRESOURCE
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        public string lpLocalName;
        public string lpRemoteName;
        public string lpComment;
        public string lpProvider;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    public class Util
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);
        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hLibModule);

        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, string lpReturnString, int nSize, string lpFilename);

        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFilename);

        // osk
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool Wow64DisableWow64FsRedirection(ref IntPtr ptr);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool Wow64RevertWow64FsRedirection(IntPtr ptr);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd,
            UInt32 Msg,
            IntPtr wParam,
            IntPtr lParam);
        private const UInt32 WM_SYSCOMMAND = 0x112;
        private const UInt32 SC_RESTORE = 0xf120;

        private const string OnScreenKeyboardExe = "osk.exe";
        ////////////////////////////////////////////////////////////////

        // ini 파일 읽기
        public static string GetIniFileString(string iniFile, string category, string key, string defaultValue)
        {
            string returnString = new string(' ', 1024);
            try
            {
                GetPrivateProfileString(category, key, defaultValue, returnString, 1024, iniFile);
            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
                throw;
            }
            return returnString.Split('\0')[0];
        }

        public static int GetIniFileInt(string iniFile, string category, string key, int defaultValue = 0)
        {
            string returnString = new string(' ', 1024);
            try
            {
                GetPrivateProfileString(category, key, defaultValue.ToString(), returnString, 1024, iniFile);
            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
                throw;
            }
            int result = 0;
            if (!Int32.TryParse(returnString.Split('\0')[0], out result))
                return defaultValue;

            return result;
        }

        public static double GetIniFileDouble(string iniFile, string category, string key, double defaultValue = 0)
        {
            string returnString = new string(' ', 1024);
            try
            {
                GetPrivateProfileString(category, key, defaultValue.ToString(), returnString, 1024, iniFile);
            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
                throw;
            }
            double result = 0;
            if (!Double.TryParse(returnString.Split('\0')[0], out result))
                return defaultValue;

            return result;
        }

        public static Dictionary<string, string> GetIniFileSection(string filePath, string sectionName, out bool result, out string err)
        {
            var sectionData = new Dictionary<string, string>();
            bool inTargetSection = false;
            string line;

            result = true;
            err = string.Empty;
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        // 제거하고 trim 적용
                        line = line.Trim();

                        // 비어있는 줄 또는 주석인 경우 스킵
                        if (string.IsNullOrEmpty(line) || line.StartsWith(";"))
                            continue;

                        // 섹션인지 확인
                        if (line.StartsWith("[") && line.EndsWith("]"))
                        {
                            // 새로운 섹션 시작
                            inTargetSection = line.Equals($"[{sectionName}]", StringComparison.OrdinalIgnoreCase);
                        }
                        else if (inTargetSection)
                        {
                            // 키-값 쌍을 읽어 dictionary에 추가
                            var keyValue = line.Split(new char[] { '=' }, 2);
                            if (keyValue.Length == 2)
                            {
                                sectionData[keyValue[0].Trim()] = keyValue[1].Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                err = ex.Message;
                result = false;
                return null;
            }

            return sectionData;
        }

        // ini 파일에 쓰기
        public static bool SetIniFileString(string iniFile, string category, string key, string value)
        {
            return WritePrivateProfileString(category, key, value, iniFile);
        }

        public static bool SetIniFileNull(string iniFile, string category)
        {
            return WritePrivateProfileString(category, null, null, iniFile);
        }

        public static void RenameIniSection(string filePath, string oldSection, string newSection)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("INI file not found.");
                return;
            }

            var lines = new List<string>(File.ReadAllLines(filePath));
            var modifiedLines = new List<string>();

            bool inTargetSection = false;

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("[") && line.Trim().EndsWith("]"))
                {
                    inTargetSection = line.Trim().Equals($"[{oldSection}]");

                    if (inTargetSection)
                    {
                        modifiedLines.Add($"[{newSection}]");
                        inTargetSection = false; // Reset to avoid changing future sections with the same name
                        continue;
                    }
                }

                modifiedLines.Add(line);
            }

            File.WriteAllLines(filePath, modifiedLines);
        }

        public static void getDatabaseConnectionItems(out string ip, out string setupCatalog, out string dataCatalog, out string id, out string password)
        {
            string ini = Util.GetWorkingDirectory() + "\\" + Define.INI_DATABASE;
            ip = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_IP, Define.STR_LOOPBACK_IP);
            setupCatalog = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_CATALOG, String.Empty);
            dataCatalog = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_DATACATALOG, String.Empty);
            id = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_ID, String.Empty);
            password = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_PASSWORD, String.Empty);
        }

        internal static void SetPictureBoxImage(PictureBox pic, string file)
        {
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                byte[] imageBytes = new byte[stream.Length];
                stream.Read(imageBytes, 0, imageBytes.Length);
                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    pic.Image = Image.FromStream(memoryStream);
                }
            }
        }

        internal static Bitmap GetBitmapFromFile(string file, int width = 0, int height = 0)
        {
            if (!File.Exists(file))
                return null;

            Bitmap bmp = null;
            Bitmap resizedBitmap = null;
            try
            {
                using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    byte[] imageBytes = new byte[stream.Length];
                    stream.Read(imageBytes, 0, imageBytes.Length);
                    using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                    {
                        bmp = (Bitmap)Image.FromStream(memoryStream);
                        if (width != 0 && height != 0)
                        {
                            resizedBitmap = new Bitmap(width, height);
                            using (Graphics graphics = Graphics.FromImage(resizedBitmap))
                            {
                                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                                graphics.DrawImage(bmp, 0, 0, width, height);
                                bmp = resizedBitmap;
                            }

                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return bmp;
        }

        /*
        internal static void getDatabaseConnectionItems(out string ip, out string setupCatalog, out string dataCatalog, out string rawDataCatalog, out string id, out string password)
        {
            string ini = Util.GetWorkingDirectory() + "\\" + Define.INI_DATABASE;
            ip = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_IP, Define.STR_LOOPBACK_IP);
            setupCatalog = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_CATALOG, String.Empty);
            dataCatalog = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_DATACATALOG, String.Empty);
            rawDataCatalog = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_RAWDATACATALOG, String.Empty);
            id = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_ID, String.Empty);
            password = Util.GetIniFileString(ini, Define.STR_CONNECTION, Define.STR_PASSWORD, String.Empty);
        }
        */

        public static string getDatabaseConnectionString(string ip, string catalog, string id, string password)
        {
            return /*"Provider=SQLOLEDB.1;*/"Password=" + password +
                    ";Persist Security Info=True;User ID=" + id +
                    ";Initial Catalog=" + catalog + ";Data Source=" + ip;
        }

        public static string GetWorkingDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static Bitmap CaptureScreen(Form form, string fileSave, out string err)
        {
            err = string.Empty;
            // 현재 실행 중인 창의 크기와 위치 가져오기
            Rectangle bounds = form.Bounds;
            // 비트맵 객체 생성
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            try
            {
                // 비트맵에 현재 창을 캡쳐
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    if (fileSave != string.Empty)
                    {
                        bitmap.Save(fileSave, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
            }
            catch (Exception e)
            {
                bitmap = null;
                err = e.Message;
            }
            return bitmap;
        }

        // Event Loggin
        private static Object thisLock = new Object();
        // preDir=>로그파일 출력 디렉토리의 접미사 Log'Err' , 
        // 
        public static void WriteLog(string p, string preDir = Define.STR_LOG, string preFile = Define.STR_DAQ)
        {
            // 현재 및 로그 디렉토리
            string sCurDir = Util.GetWorkingDirectory();
            string sLogDir = sCurDir + "\\" + preDir;

            // 없으면 만든다
            if (!Directory.Exists(sLogDir))
                Directory.CreateDirectory(sLogDir);

            // 일정 기간(90일) 이상 지난 파일은 삭제
            string sLogOld = sLogDir + "\\" + DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd") + "_" + preFile + ".txt";
            if (File.Exists(sLogOld))
                File.Delete(sLogOld);

            // 로그 파일 이름(날짜)
            string sLogFile = sLogDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + "_" + preFile + ".txt";
            // 로그 내용의 Prefix(시간)
            string sDateTime = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + "] ";
            // 내용
            string sContents = sDateTime + p + "\r\n";

            lock (thisLock)
            {
                FileStream myFileStream = null;
                try
                {
                    myFileStream = new FileStream(sLogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                    myFileStream.Seek(0, SeekOrigin.End);
                    myFileStream.Write(Encoding.Default.GetBytes(sContents), 0, Encoding.Default.GetByteCount(sContents));
                }
                finally
                {
                    if (null != myFileStream)
                        myFileStream.Close();
                }
            }
        }

        internal static void GetWeekDays(out DateTime[] from, out DateTime[] to)
        {
            from = new DateTime[5];
            to = new DateTime[5];
            DateTime now = DateTime.Now;
            // 첫째주 시작은 무조건 이번 달 1일 07:00
            from[0] = new DateTime(now.Year, now.Month, 1, 7, 0, 0);
            // 토요일인 경우 이틀 더해줌
            if (from[0].DayOfWeek == DayOfWeek.Saturday)
                from[0] = from[0].AddDays(2);
            // 일요일인 경우 하루 더해줌
            if (from[0].DayOfWeek == DayOfWeek.Sunday)
                from[0] = from[0].AddDays(1);

            // sunday=0, saturday=6
            to[0] = from[0].AddDays(6 - (int)from[0].DayOfWeek);
            for (int i = 1; i < 4; i++)
            {
                from[i] = to[i - 1].AddDays(2);
                to[i] = from[i].AddDays(5);
            }
            from[4] = to[3].AddDays(2);
            to[4] = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);
        }

        internal static void GetWeekDaysWithWeekend(out DateTime[] from, out DateTime[] to)
        {
            from = new DateTime[5];
            to = new DateTime[5];
            DateTime now = DateTime.Now;
            // 첫째주 시작은 무조건 이번 달 1일 07:00
            from[0] = new DateTime(now.Year, now.Month, 1, 0, 0, 0);

            // sunday=0, monday=1, tuesday=2, saturday=6
            to[0] = from[0].AddDays(7 - (int)from[0].DayOfWeek);
            for (int i = 1; i < 4; i++)
            {
                from[i] = to[i - 1].AddDays(2);
                to[i] = from[i].AddDays(5);
            }
            from[4] = to[3].AddDays(2);
            to[4] = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);
        }
        // 가동일이 몇일인지 구하는 공식
        internal static int GetProductionDayCount(int year, int month)
        {
            int totalDays = 0;
            DateTime now = DateTime.Now;
            int dayLast = DateTime.DaysInMonth(year, month);
            // 현재 월인 경우
            if (now.Year == year && now.Month == month)
            {
                dayLast = now.Day;
            }

            for (int i = 1; i <= dayLast; i++)
            {
                DateTime temp = new DateTime(year, month, i, 0, 0, 0);
                if (temp.DayOfWeek != DayOfWeek.Sunday && temp.DayOfWeek != DayOfWeek.Saturday)
                    totalDays++;
            }

            return totalDays;
        }

        // Ping Test
        public static bool PingTest(string ip)
        {
            if ("." == ip.Trim('\0'))
                return true;

            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();

            // Use the default Ttl value which is 128, but change the fragmentation behavior.
            options.DontFragment = true;

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            int timeout = 120;

            try
            {
                PingReply reply = pingSender.Send(ip, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                    return false;
            }
            catch (Exception e)
            {
                Util.WriteLog(e.ToString());
            }
            return false;
        }

        internal static void ResizePictureBox(Bitmap bmImage, PictureBox pic, Panel pnOwner)
        {
            if (bmImage == null)
                return;

            // 이미지 크기와 패널 크기를 가져옴
            int imageWidth = bmImage.Width;
            int imageHeight = bmImage.Height;
            int panelWidth = pnOwner.ClientSize.Width;
            int panelHeight = pnOwner.ClientSize.Height;

            // 이미지 비율을 유지하면서 패널 크기에 맞추기
            float aspectRatio = (float)imageWidth / imageHeight;
            int newWidth, newHeight;

            if (panelWidth / (float)panelHeight > aspectRatio)
            {
                newHeight = panelHeight;
                newWidth = (int)(panelHeight * aspectRatio);
            }
            else
            {
                newWidth = panelWidth;
                newHeight = (int)(panelWidth / aspectRatio);
            }

            pic.Size = new Size(newWidth, newHeight);
            pic.Location = new Point((panelWidth - newWidth) / 2, (panelHeight - newHeight) / 2);
            pic.Visible = true;
        }

        public static string getDateString(string d)
        {
            // yyyyMMdd -> dd/MM/yyyy
            return d.Substring(6, 2) + "/" + d.Substring(4, 2) + "/" + d.Substring(0, 4);
        }

        public static string getTimeString(string t)
        {
            // HHmmss -> HH:mm:ss
            return t.Substring(0, 2) + ":" + t.Substring(2, 2) + ":" + t.Substring(4, 2);
        }

        public static DateTime StringToDateTime(string s)
        {
            return new DateTime(Int32.Parse(s.Substring(0, 4)),
                                Int32.Parse(s.Substring(4, 2)),
                                Int32.Parse(s.Substring(6, 2)),
                                Int32.Parse(s.Substring(8, 2)),
                                Int32.Parse(s.Substring(10, 2)),
                                Int32.Parse(s.Substring(12, 2)));
        }

        public static void ShowKeyboard()
        {
            string file = Util.GetWorkingDirectory() + "\\osk.exe";
            Process.Start(file);
            /*
                    Process[] p = Process.GetProcessesByName(
                        Path.GetFileNameWithoutExtension(OnScreenKeyboardExe));

                    if (p.Length == 0)
                    {
                        // we must start osk from an MTA thread
                        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
                        {
                            ThreadStart start = new ThreadStart(StartOsk);
                            Thread thread = new Thread(start);
                            thread.SetApartmentState(ApartmentState.MTA);
                            thread.Start();
                            thread.Join();
                        }
                        else
                        {
                            StartOsk();
                        }
                    }
                    else
                    {
                        // there might be a race condition if the process terminated 
                        // meanwhile -> proper exception handling should be added
                        //
                        SendMessage(p[0].MainWindowHandle,
                            WM_SYSCOMMAND, new IntPtr(SC_RESTORE), new IntPtr(0));
                    }
             */
        }

        public static Bitmap Extract(Bitmap src, Rectangle section)
        {
            Bitmap bmp = new Bitmap(section.Width, section.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(src, 0, 0, section, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        static void StartOsk()
        {
            IntPtr ptr = new IntPtr(); ;
            bool sucessfullyDisabledWow64Redirect = false;

            // Disable x64 directory virtualization if we're on x64,
            // otherwise keyboard launch will fail.
            if (System.Environment.Is64BitOperatingSystem)
            {
                sucessfullyDisabledWow64Redirect =
                    Wow64DisableWow64FsRedirection(ref ptr);
            }


            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = OnScreenKeyboardExe;
            // We must use ShellExecute to start osk from the current thread
            // with psi.UseShellExecute = false the CreateProcessWithLogon API 
            // would be used which handles process creation on a separate thread 
            // where the above call to Wow64DisableWow64FsRedirection would not 
            // have any effect.
            //
            psi.UseShellExecute = true;
            Process.Start(psi);

            // Re-enable directory virtualisation if it was disabled.
            if (System.Environment.Is64BitOperatingSystem)
                if (sucessfullyDisabledWow64Redirect)
                    Wow64RevertWow64FsRedirection(ptr);
        }

        #region type_casting
        public static int Bytes2Integer(byte[] p)
        {
            return int.Parse(Bytes2String(p));
        }

        public static string Bytes2String(byte[] p)
        {
            return Encoding.ASCII.GetString(p);
        }

        internal static float BytesToFloat(byte[] buffer, int index)
        {
            return BitConverter.ToSingle(buffer, index * 4);
        }
        #endregion

        // Object를 byte 배열로 돌려주는 함수
        public static byte[] object2ByteArray(object p)
        {
            /* 검증 필요
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();

            try
            {
                bf.Serialize(ms, p);
                return ms.ToArray();
            }
            catch (Exception e)
            {
                WriteLog("object2ByteArray exception... " + e.ToString());
                return null;
            }
            finally
            {
                ms.Close();
            }
             */
            return null;
        }

        // byte 배열을 Object로 돌려주는 함수
        public static object ByteArray2object(byte[] p)
        {
            /* 검증 필요
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(p);

            try
            {
                return bf.Deserialize(ms);
            }
            catch (Exception e)
            {
                WriteLog("ByteArray2object exception... " + e.ToString());
                throw;
            }
             */
            return null;
        }

        public static byte[] getByteArrayFromStructure(object any, int rawSize)
        {
            /* 검증 필요
            byte[] data = new byte[rawSize];
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(any, buffer, false);
            handle.Free();
            return data;
             */
            return null;
        }

        public static void memcopy(byte[] src, int srcIndex, byte[] dst, int dstIndex, int count)
        {
            /* 검증 필요
            Array.Copy(src, srcIndex, dst, dstIndex, count);
             */
        }

        // 감도에 따른 스케일 팩터 계산
        public static float getSensitivityFactor(float sens)
        {
            return (float)(1 / sens);
        }

        /*
        // 시뮬레이션 모드에서 스칼라 데이터 임의 생성
        public static float getRandomScalar(FunctionSetup func, float ratio)
        {
            Random rand = new Random();
            // 현재 수행하고 있는 함수에 알람 설정이 안되어 있다면...
            if (-1 == func.alarmID)
                return (float)rand.NextDouble() * 100;

            var alarms = Global.alarmSetupList.Where(al => al.ID == func.alarmID);
            double temp = rand.NextDouble();
            if (temp > ratio)
                return (float)(alarms.First().alertLow * (0.9 + rand.NextDouble() / 100));

            return (float)rand.NextDouble() * alarms.First().alertHigh;
        }

        // 측정값에 대한 알람 레벨 판정
        public static short checkAlarmScalar(List<AlarmSetup> alarmList, FunctionSetup func, float value)
        {
            if (-1 == func.alarmID)
                return 0;

            var alarms = alarmList.Where(al => al.ID == func.alarmID);
            AlarmSetup alarm = alarms.First();
            if (null == alarm)
                return 0;

            if ((alarm.dangerHigh > 0) && (value >= alarm.dangerHigh))
                return 4;
            if ((alarm.dangerLow > 0) && (value >= alarm.dangerLow))
                return 3;
            if ((alarm.alertHigh > 0) && (value >= alarm.alertHigh))
                return 2;
            if ((alarm.alertLow > 0) && (value >= alarm.alertLow))
                return 1;

            return 0;
        }

        internal static short checkAlarmVector(List<AlarmSetup> alarmList, FunctionSetup func, int data_size, float[] value)
        {
            if (-1 == func.alarmID)
                return 0;

            var alarms = alarmList.Where(al => al.ID == func.alarmID);
            AlarmSetup alarm = alarms.First();
            if (null == alarm)
                return 0;

            float[] alertLow = new float[data_size];
            float[] alertHigh = new float[data_size];
            float[] dangerLow = new float[data_size];
            float[] dangerHigh = new float[data_size];
            if (FunctionCode.tfAutospectrum == func.functionType)
            {
                AutospectrumSetupAddon sa = Global.autospectrumSetupAddonList.Find(s => s.functionNo == func.functionNo);
                if (null != sa)
                {
                    alertLow = sa.alertLow;
                    alertHigh = sa.alertHigh;
                    dangerLow = sa.dangerLow;
                    dangerHigh = sa.dangerHigh;
                }
            }
            else if(FunctionCode.tfOrderSpectrum == func.functionType)
            {
                OrderspectrumSetupAddon sa = Global.orderspectrumSetupAddonList.Find(s => s.functionNo == func.functionNo);
                if (null != sa)
                {
                    alertLow = sa.alertLow;
                    alertHigh = sa.alertHigh;
                    dangerLow = sa.dangerLow;
                    dangerHigh = sa.dangerHigh;
                }
            }
            else if(FunctionCode.tfEnvSpectrum == func.functionType)
            {
                EnvspectrumSetupAddon sa = Global.envspectrumSetupAddonList.Find(s => s.functionNo == func.functionNo);
                if (null != sa)
                {
                    alertLow = sa.alertLow;
                    alertHigh = sa.alertHigh;
                    dangerLow = sa.dangerLow;
                    dangerHigh = sa.dangerHigh;
                }
            }

            short result = 0;
            for (int i = 0; i < data_size; i++)
            {
                // 4단계 알람이면 더 탐색할 필요가 없음
                if (value[i] >= dangerHigh[i])
                    return 4;
                
                if ((value[i] >= dangerLow[i]) && (value[i] < dangerHigh[i]))
                    result = 3;
                else if ((value[i] >= alertHigh[i]) && (value[i] < dangerLow[i]))
                    result = 2;
                else if ((value[i] >= alertLow[i]) && (value[i] < alertHigh[i]))
                    result = 1;
            }

            return result;
        }

        // 소음 채널인지 체크
        internal static int IsDecibel(List<ChannelSetup> channelList, int ID)
        {
            var c = channelList.Where(ch => ch.ID == ID);
            if (null == c)
                return 0;
            return ((SensorCode.ttMicrophone == c.First().sensorCode) ? 1 : 0);
        }
        */


        public static void CopyDirectry(string src, string tar, string opt = "*.*")
        {
            string strErr = string.Empty;
            string[] files = Directory.GetFiles(src, opt);
            //int count = 0;
            foreach (string f in files)
            {
                int idx = f.LastIndexOf('\\');
                string srcFile = f.Substring(idx + 1);
                string tarFile = tar + "\\" + srcFile;
                try
                {
                    File.Move(f, tarFile);
                }
                catch (Exception e)
                {
                    strErr = e.ToString();
                }
            }
        }

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        public static extern int WNetUseConnection(IntPtr hwndOwner,
            [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
            string lpPassword, string lpUserID, uint dwFlags,
            StringBuilder lpAccessName, ref int lpBufferSize, out uint lpResult);

        // 네트워크 드라이브 연결
        public static bool NetDriveConnection(string path, string id, string pwd, string drive)
        {
            int capacity = 64;
            uint resultFlags = 0;
            uint flags = 0;
            StringBuilder sb = new StringBuilder(capacity);

            NETRESOURCE ns = new NETRESOURCE();
            ns.dwType = 1;
            ns.lpLocalName = drive;
            ns.lpRemoteName = @path;
            ns.lpProvider = null;
            int result = WNetUseConnection(IntPtr.Zero, ref ns, pwd, id, flags, sb, ref capacity, out resultFlags);

            return (result == 0);
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool SetLocalTime(ref SYSTEMTIME time);

        public static bool SetSystemDateTime(DateTime dtNew)
        {
            bool bRet = false;
            if (dtNew != DateTime.MinValue)
            {
                SYSTEMTIME st;
                st.wYear = (ushort)dtNew.Year;
                st.wMonth = (ushort)dtNew.Month;
                st.wDayOfWeek = (ushort)dtNew.DayOfWeek;
                st.wDay = (ushort)dtNew.Day;
                st.wHour = (ushort)dtNew.Hour;
                st.wMinute = (ushort)dtNew.Minute;
                st.wSecond = (ushort)dtNew.Second;
                st.wMilliseconds = (ushort)dtNew.Millisecond;

                bRet = SetLocalTime(ref st);
            }
            return bRet;
        }

        public static string RemoveNull(byte[] p)
        {
            string s = Encoding.Default.GetString(p);
            char[] trims = { '\r', '\n', '\0' };
            s = s.Trim(trims);
            return s;
        }

        public void BigEndianToLittleEndian(int input, out int output)
        {
            output = 0;
        }

        public void LittleEndianToBigEndian(int input, out int output)
        {
            output = 0;
        }

        public static string GetLocalIPAddress()
        {
            string res = string.Empty;
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    res = ip.ToString();
            }
            return res;
        }

        internal static DateTime getMexicanDateFromString(string s)
        {
            // dd/MM/yy HH:mm:ss
            int yy = Int32.Parse(s.Substring(6, 2)) + 2000;
            int MM = Int32.Parse(s.Substring(3, 2));
            int dd = Int32.Parse(s.Substring(0, 2));
            int HH = Int32.Parse(s.Substring(9, 2));
            int mm = Int32.Parse(s.Substring(12, 2));
            int ss = Int32.Parse(s.Substring(15, 2));
            return new DateTime(yy, MM, dd, HH, mm, ss);
        }

        internal static uint Color2Uint(Color c)
        {
            return (uint)((c.A << 24) | (c.R << 16) | (c.G << 8) | c.B);
        }

        internal static void RebootSystem()
        {
            Process.Start("shutdown", "/r /f /t 10");
        }

        public static string GetShortName(string machine, int len = 18)
        {
            if (machine.Length < len)
                return machine;
            return machine.Substring(0, len - 1);
        }

        internal static void SetLabelEllipse(Label lbl)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(1, 1, lbl.Width - 2, lbl.Height - 2);
            lbl.Region = new Region(path);
        }
        internal static void SetControlEllipse(Control ctrl)
        {
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(1, 1, ctrl.Width - 2, ctrl.Height - 2);
            ctrl.Region = new Region(path);
        }

        internal static void GetOriginalCoordinateFromPictureBoxStretched(PictureBox pic, Bitmap bmp, int xin, int yin, out int xout, out int yout)
        {
            xout = xin;
            yout = yin;

            if (bmp == null || bmp.Width == 0 || bmp.Height == 0)
                return;

            // PictureBox의 크기
            int pbWidth = pic.ClientSize.Width;
            int pbHeight = pic.ClientSize.Height;

            // Bitmap의 크기
            int bmpWidth = bmp.Width;
            int bmpHeight = bmp.Height;

            // PictureBox의 좌표를 비트맵 좌표로 변환
            xout = xin * pbWidth / bmpWidth;
            yout = yin * pbHeight / bmpHeight;
        }
    }


}
/*
텍스트박스 등에 로그를 추가하고 나서
자동으로 마지막 입력한 내용이 보이도록 스크롤 하고 싶을때 아래와 같은 코드를 사용하시기 바랍니다.

TextBox
1.textBox1.SelectionStart = textBox1.Text.Length; 
2.textBox1.ScrollToCaret();

ListBox
1.listBox1.SelectedIndex = listBox1.Items.Count - 1; 
2.listBox1.SelectedIndex = -1;

ListView
1.listView1.EnsureVisible(listView1.Items.Count - 1);

TreeView
1.treeView1.Nodes[treeView1.Nodes.Count - 1].EnsureVisible();

DataGridView
1.dataGridView1.FirstDisplayedCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
*/