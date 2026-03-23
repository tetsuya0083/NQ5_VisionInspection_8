using Common;
using KEYENCE;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
#if NET8_0_OR_GREATER
using Microsoft.Data.SqlClient;
#else
using System.Data.SqlClient;
#endif
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Vision.Shared;
using XGCommLibDemo;

namespace YONGSAN_CPAD_VISION
{
    public class CONFIG
    {
        public static string GetConnectionString(string ini, out string dataSource)
        {
            dataSource = Util.GetIniFileString(ini, "Server", "Data Source", "");
            string user = Util.GetIniFileString(ini, "Server", "User ID", "");
            string pass = Util.GetIniFileString(ini, "Server", "Password", "");
            string catalog = Util.GetIniFileString(ini, "Server", "Initial Catalog", "");

            return "Password=" + pass + ";Persist Security Info=True;" +
                "User ID=" + user + ";Initial Catalog=" + catalog + ";Data Source=" + dataSource + ";Connection Timeout=5";
        }
    }

    public class Model
    {
        public int ModelID;
        public string ModelServerName;
        public int ModelPLCNumber = 0;
        public string ModelImage;
        public List<LabelModel> LabelList = new List<LabelModel>();

        public List<CameraObject> CameraList = new List<CameraObject>();
        public DateTime dtStarted;
        public bool IsFinished = false;

        public Model()
        {
        }

        public static List<Model> GetModelList(string iniModel, List<CameraObject> camList)
        {
            List<Model> list = new List<Model>();

            int MaxID = Util.GetIniFileInt(iniModel, "Model", "MaxID", 0);
            for (int i = 1; i <= MaxID; i++)
            {
                string key = "MODEL" + i.ToString();
                string name = Util.GetIniFileString(iniModel, key, "ServerName", string.Empty);
                if (name == string.Empty)
                    continue;

                Model mo = new Model();
                mo.ModelID = Util.GetIniFileInt(iniModel, key, "ID", 0);
                mo.ModelServerName = Util.GetIniFileString(iniModel, key, "ServerName", string.Empty);
                mo.ModelPLCNumber = Util.GetIniFileInt(iniModel, key, "PLCNumber");
                mo.ModelImage = Util.GetIniFileString(iniModel, key, "Image", string.Empty);
                if (!File.Exists(mo.ModelImage) && mo.ModelImage.LastIndexOf("\\") > 0)
                    mo.ModelImage = Util.GetWorkingDirectory() + "\\Images\\" + Path.GetFileName(mo.ModelImage);

                mo.CameraList = camList;

                string file = GetJsonFile(mo);
                if (File.Exists(file))
                {
                    string json = File.ReadAllText(file);
                    mo.LabelList = JsonConvert.DeserializeObject<List<LabelModel>>(json);
                }
                //mo.StepList = Step.GetStepList(iniModel, mo);
                list.Add(mo);
            }

            // ModelPLCNumber순으로 정렬
            list = list.OrderBy(model => model.ModelPLCNumber).ToList();

            return list;
        }

        private static string GetJsonFile(Model mo)
        {
            string folder = Path.Combine(Util.GetWorkingDirectory(), "Setup");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string file = Path.Combine(folder, mo.ModelServerName + ".json");
            return file;
        }

        internal static Model DuplicateModel(string iniModel, Model moSrc, int NewID)
        {
            string newName = moSrc.ModelServerName + "-COPY";
            string fileSrc = Path.Combine(Util.GetWorkingDirectory(), "Setup", $"{moSrc.ModelServerName}.json");
            string fileDst = Path.Combine(Util.GetWorkingDirectory(), "Setup", $"{newName}.json");
            try
            {
                File.Copy(fileSrc, fileDst, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Duplicate Model Failed. " + ex.Message);
                return null;
            }

            Model mo = new Model();
            mo.ModelID = NewID;
            mo.ModelServerName = newName;
            mo.ModelPLCNumber = NewID;
            mo.ModelImage = moSrc.ModelImage;
            foreach (CameraObject cam in moSrc.CameraList)
                mo.CameraList.Add(cam);

            string category = "MODEL" + mo.ModelID.ToString();
            Util.SetIniFileString(iniModel, category, "ID", NewID.ToString());
            Util.SetIniFileString(iniModel, category, "ServerName", mo.ModelServerName);
            Util.SetIniFileString(iniModel, category, "PLCNumber", mo.ModelPLCNumber.ToString());
            Util.SetIniFileString(iniModel, category, "Image", mo.ModelImage);

            // MaxID 증가
            Util.SetIniFileString(iniModel, "Model", "MaxID", NewID.ToString());

            return mo;
        }

        internal static void Remove(string ini, Model mo)
        {
            string key = "MODEL" + mo.ModelID.ToString();
            Util.SetIniFileNull(ini, key);
            try
            {
                string file = Model.GetJsonFile(mo);
                File.Delete(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Remove Model Failed. " + ex.Message);
            }
        }

        internal void SaveInformation()
        {
            try
            {
                string file = Model.GetJsonFile(this);
                string json = JsonConvert.SerializeObject(LabelList, Formatting.Indented);
                File.WriteAllText(file, json);
            }
            catch (Exception)
            {
            }
        }
    }

    public enum HIT_TEST
    {
        _NONE
        , _MOVE, _LEFTTOP, _TOP, _RIGHTTOP
        , _LEFT, _RIGHT
        , _LEFTBOTTOM, _BOTTOM, _RIGHTBOTTOM
    };

    public enum USER_MESSAGE { WM_USER = 0x0400 }
    public class YONGSAN_VISION_CORE
    {
        // PInvoke declaration for SendMessage
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam = 0, string lParam = "");
        // Windows Messages Constants
        public static uint WM_USER_PAINT = ((int)USER_MESSAGE.WM_USER) + 100;

        public static uint WM_USER_STATUS_PLC = ((int)USER_MESSAGE.WM_USER) + 201;
        public static uint WM_USER_STATUS_SERVER = ((int)USER_MESSAGE.WM_USER) + 202;

        public static uint WM_USER_LOG_CAMERA = ((int)USER_MESSAGE.WM_USER) + 1001;
        public static uint WM_USER_LOG_PLC = ((int)USER_MESSAGE.WM_USER) + 1002;

        public static uint WM_USER_TEST_INIT = ((int)USER_MESSAGE.WM_USER) + 2000;
        public static uint WM_USER_TEST_MODELNAME = ((int)USER_MESSAGE.WM_USER) + 2100;
        public static uint WM_USER_FINAL_IMAGE = ((int)USER_MESSAGE.WM_USER) + 3000;

        public static uint WM_USER_RESULT = ((int)USER_MESSAGE.WM_USER) + 4000;
        public static uint WM_USER_FINAL_RESULT = ((int)USER_MESSAGE.WM_USER) + 4444;

        public static uint WM_USER_MESSAGE_CAMERA = ((int)USER_MESSAGE.WM_USER) + 5001;
        public static uint WM_USER_MESSAGE_GENERAL = ((int)USER_MESSAGE.WM_USER) + 8001;

        public static uint WM_USER_MESSAGE_WRONGSTEP = ((int)USER_MESSAGE.WM_USER) + 8010;
        public static uint WM_USER_MESSAGE_STEPRESULT = ((int)USER_MESSAGE.WM_USER) + 8020;
        public static uint WM_USER_MESSAGE_CAMERA_IMAGE = ((int)USER_MESSAGE.WM_USER) + 8030;

        public static uint WM_USER_DATETIME = ((int)USER_MESSAGE.WM_USER) + 8500;
        public static uint WM_USER_FLICKER = ((int)USER_MESSAGE.WM_USER) + 9999;

        public string DataFolder { get; set; } = string.Empty;
        public string BackupFolder { get; set; } = string.Empty;
        public string SaveFolder { get; set; } = string.Empty;
        public bool Initialized { get; set; } = false;
        public List<Model> ModelList = null;

        public List<CameraObject> CameraList = null;
        public int IsNewData = 0;
        public Form Owner = null;
        public System.Threading.Timer timerWorker = null;

        public string EQUIPMENT = string.Empty;
        public string iniSetup = string.Empty;
        public string iniPLC = string.Empty;
        public string iniCamera = string.Empty;
        public string iniModel = string.Empty;
        public string iniServer = string.Empty;

        delegate void FormSendMessageCallback(uint message, int wparam, string lparam);
        private void FormSendMessage(uint message, int wparam = 0, string lparam = "")
        {
            try
            {
                if (Owner == null)
                    return;

                if (Owner.InvokeRequired)
                {
                    FormSendMessageCallback cb = new FormSendMessageCallback(FormSendMessage);
                    Owner.Invoke(cb, new object[] { message, wparam, lparam });
                }
                else
                {
                    SendMessage(Owner.Handle, message, wparam, lparam);
                }
            }
            catch (Exception)
            {

            }
        }

        public YONGSAN_VISION_CORE(Form form, string equip, PictureBox pic)
        {
            Owner = form;
            EQUIPMENT = equip;

            if (Load(pic))
                this.Initialized = true;
        }

        public static bool _StopPLCReaderThread = false;
        internal void CloseObject()
        {
            _StopPLCReaderThread = true;
            if (PLCReaderThread != null)
                PLCReaderThread.Join();
        }

        public void timerWorker_Callback(object state)
        {
            timerWorker.Change(Timeout.Infinite, Timeout.Infinite);
            DoProcess();

            int interval = 50;// Int32.Parse(Util.GetIniFileString(iniSetup, "Setup", "TimerInterval", "1000"));
            timerWorker.Change(interval, interval);
        }

        public Model CurrentModel = null;
        public PictureBox picTarget = null;
        internal bool Load(PictureBox picOwner)
        {
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

            DataFolder = Util.GetIniFileString(iniSetup, "Setup", "DataFolder", "C:\\TP\\DATA");
            BackupFolder = Util.GetIniFileString(iniSetup, "Setup", "BackupFolder", "C:\\TP\\DATA_BACKUP");
            SaveFolder = Util.GetIniFileString(iniSetup, "Setup", "SaveFolder", "C:\\VISION_DATA");

            CameraList = CameraObject.GetCameraList(iniCamera);
            ModelList = Model.GetModelList(iniModel, CameraList);

            picTarget = picOwner;
            StartPLCReader();

            //InitServerDatabase();

            // 작업 타이머
            TimerCallback callback = new TimerCallback(timerWorker_Callback);
            timerWorker = new System.Threading.Timer(callback, null, 0, 1000);

            return true;
        }

        public Thread PLCReaderThread = null;
        public ushort[] Data_Trigger;
        public ushort[] Data_Finish;
        public ushort[][][] Data_Result_Cam; // [카메라 인덱스][데이터 블럭 인덱스][상세 데이터]
        private void StartPLCReader()
        {
            PLCReaderThread = new Thread(new ParameterizedThreadStart(PLCReaderThreadTask));
            PLCReaderThread.IsBackground = true;
            PLCReaderThread.Start(this);
        }

        public static void PLCReaderThreadTask(object obj)
        {
            string iniPLC = Util.GetWorkingDirectory() + "\\PLCE204.ini";
            YONGSAN_VISION_CORE core = (YONGSAN_VISION_CORE)obj;

            int cameraCount = core.CameraList.Count;
            core.Data_Trigger = new ushort[cameraCount];
            core.Data_Finish = new ushort[cameraCount];
            core.Data_Result_Cam = new ushort[cameraCount][][];
            for (int j = 0; j < cameraCount; j++)
            {
                int dataCount_Cam = Util.GetIniFileInt(iniPLC, "ResultDataCam" + (j + 1).ToString(), "DataCount");
                core.Data_Result_Cam[j] = new ushort[dataCount_Cam][];
                int dataSize_Cam = Util.GetIniFileInt(iniPLC, "ResultDataCam" + (j + 1).ToString(), "DataSize");
                for (int i = 0; i < dataCount_Cam; i++)
                {
                    core.Data_Result_Cam[j][i] = new ushort[dataSize_Cam];
                }
            }

            string ip = Util.GetIniFileString(iniPLC, "PLC", "IP", "192.168.0.1");
            string port = Util.GetIniFileString(iniPLC, "PLC", "Port", "2004");

            XGCommSocket XGComm = new XGCommSocket();
            int retry = 10;
            while (Util.PingTest(ip) && retry > 0)
            {
                // PLC 연결이 되지 않으면 종료...
                // DoProcess()에서 실행 중인지 체크 후 실행 중이 아닐 경우 다시 실행함
                if (XGComm.Connect(ip, Convert.ToInt32(port)) != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                {
                    core.FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                    Thread.Sleep(1000);
                    retry--;
                    continue;
                }
                break;
            }

            char cDeviceType;
            UInt16[] bufRead;
            while (!_StopPLCReaderThread)
            {
                if (!Util.PingTest(ip))
                    continue;

                Thread.Sleep(100);
                if (!Util.PingTest(ip))
                {
                    core.FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                    Thread.Sleep(1000);
                    continue;
                }

                core.FormSendMessage(WM_USER_STATUS_PLC, 1, "");
                if (core.IsNewData == 0)
                {
                    Thread.Sleep(0);
                    continue;
                }

                for (int j = 0; j < cameraCount; j++)
                {
                    string key = "CAMERA" + (j + 1).ToString();
                    cDeviceType = Util.GetIniFileString(iniPLC, key + "Finish", "DeviceType", "D")[0];
                    if (Int64.TryParse(Util.GetIniFileString(iniPLC, key + "Finish", "Address", "0"), out long addrFinish))
                    {
                        try
                        {
                            bufRead = new UInt16[1];
                            uint uReturn = XGComm.ReadDataWord(cDeviceType, addrFinish, 1, false, bufRead);
                            if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                            {
                                core.Data_Finish[j] = bufRead[0];
                            }
                        }
                        catch (Exception)
                        {
                            core.FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                        }
                    }

                    cDeviceType = Util.GetIniFileString(iniPLC, key + "Trigger", "DeviceType", "D")[0];
                    if (Int64.TryParse(Util.GetIniFileString(iniPLC, key + "Trigger", "Address", "0"), out long addrTrigger))
                    {
                        try
                        {
                            bufRead = new UInt16[1];
                            uint uReturn = XGComm.ReadDataWord(cDeviceType, addrTrigger, 1, false, bufRead);
                            if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                            {
                                core.Data_Trigger[j] = bufRead[0];
                            }
                        }
                        catch (Exception)
                        {
                            core.FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                        }
                    }

                }

                for (int n = 0; n < core.Data_Result_Cam.Length; n++)
                {
                    // Data Read Cam1
                    cDeviceType = Util.GetIniFileString(iniPLC, "ResultDataCam" + (n + 1).ToString(), "DeviceType", "D")[0];
                    int dataCount_Cam = Util.GetIniFileInt(iniPLC, "ResultDataCam" + (n + 1).ToString(), "DataCount");
                    int offsetCam = Util.GetIniFileInt(iniPLC, "ResultDataCam" + (n + 1).ToString(), "Offset", 10);
                    if (Int64.TryParse(Util.GetIniFileString(iniPLC, "ResultDataCam" + (n + 1).ToString(), "AddressStart", "0"), out long addrStart))
                    {
                        //Util.WriteLog("", "Log", $"DATA_CAM{(n + 1)}");
                        Util.WriteLog("##################", "Log", $"DATA_CAM{(n + 1)}");
                        try
                        {
                            for (int i = 0; i < dataCount_Cam; i++)
                            {
                                int addr = (int)addrStart + (i * offsetCam);
                                uint uReturn = XGComm.ReadDataWord(cDeviceType, addr, core.Data_Result_Cam[n][i].Length, false, core.Data_Result_Cam[n][i]);
                                if (uReturn == (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                                {
                                    //string val = string.Join(",", core.Data_Result_Cam[n][i]);
                                    //string s = $"CAM{(n + 1)} Address:{addr}\tValue:" + val;
                                    //Util.WriteLog(s, "Log", $"DATA_CAM{(n + 1)}");
                                }
                            }
                        }
                        catch (Exception)
                        {
                            core.FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                        }
                    }
                }
            }
        }

        private void InitServerDatabase()
        {
            return;
            /*
            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            string[] split = ip.Split(',');
            if (split.Length > 1)
                ip = split[0];

            if (!Util.PingTest(ip))
                return;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                MessageBox.Show(e.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            */
        }

        private void CheckServerResetStatus()
        {
            if (IsNewData == 1 || IsNewData == 2)
            {
                // 정상 동작 모드에서라면
                if (RunMode == MODE_NORMAL)
                {
                    string status = ReadServerStatus();
                    if (status == "0")
                    {
                        ResetAllValues();
                        IsNewData = 0;
                    }
                }
            }
        }

        public Thread PLCWriterThread;
        private void ResetAllValues()
        {
            // PLC Trigger, Final값 초기화 
            string category = "Trigger";
            char dev = Util.GetIniFileString(iniPLC, category, "DeviceType", "D")[0];
            if (Int32.TryParse(Util.GetIniFileString(iniPLC, category, "Address", "100"), out int addrTrigger))
            {
                WritePLCData(dev, addrTrigger, 0);
            }
            category = "Finish";
            if (Int32.TryParse(Util.GetIniFileString(iniPLC, category, "Address", "0"), out int addrFinish))
            {
                WritePLCData(dev, addrFinish, 0);
            }
        }

        private void WriteResult(DateTime dt, string contents)
        {
            string dir = SaveFolder + "\\" + dt.ToString("yyyy") + "\\" +
                dt.ToString("MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // 로그 파일 이름(날짜)
            string sLogFile = dir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            // 내용

            FileStream myFileStream = null;
            try
            {
                myFileStream = new FileStream(sLogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                myFileStream.Seek(0, SeekOrigin.End);
                myFileStream.Write(Encoding.Default.GetBytes(contents), 0, Encoding.Default.GetByteCount(contents));
            }
            finally
            {
                if (null != myFileStream)
                    myFileStream.Close();
            }
        }

        private bool ReadServerDatabase(out string model, out bool isReset)
        {
            model = string.Empty;
            isReset = false;
            /*
            isReset = false;
            model = string.Empty;
            bool inspectionReady = false;

            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return false;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return false;
            }

            string cmd = "SELECT * FROM EQIP_IF WHERE EQIP='" + EQUIPMENT + "'";
            SqlCommand qry = new SqlCommand(cmd, conn);

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null && reader.Read())
                    {
                        string status = Database.getStringValue(reader, "STATUS");
                        // 0이면 설비 가동 불가능
                        if (status == "0")
                        {
                            IsNewData = 0;
                        }
                        // 1이면 설비 가동 준비
                        else if (status == "1")
                        {
                            string modelName = Database.getStringValue(reader, "SPEC");
                            if (ModelList.Find(x => x.ModelPLCNumber.ToString() == modelName) != null)
                            {
                                model = modelName;
                                inspectionReady = true;
                            }
                        }

                        // 제품이 로딩된 상태에서 0으로 변경되면 리셋
                        if (IsNewData != 0 && status == "0")
                        {
                            isReset = true;
                        }
                    }
                }
                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                Util.WriteLog("Database Error. " + e.Message, "LogErr", "DB");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return inspectionReady;
            */
            return true;
        }

        private string ReadServerStatus()
        {
            /*
            string status = string.Empty;
            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return string.Empty;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return string.Empty;
            }

            string cmd = "SELECT * FROM EQIP_IF WHERE EQIP='" + EQUIPMENT + "'";
            SqlCommand qry = new SqlCommand(cmd, conn);

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlDataReader reader = qry.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader != null && reader.Read())
                    {
                        status = Database.getStringValue(reader, "STATUS");
                    }
                }
                FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
            }
            catch (Exception e)
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                Util.WriteLog("Database Error. " + e.Message, "LogErr", "DB");
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            return status;
            */
            return string.Empty;
        }

        public const uint PLC_NOT_CONNECTED = 100;
        private uint WritePLCData(char devType, int address, int value)
        {
            int retry = 10;
            bool success = false;
            // PLC 연결 시도
            while (retry > 0)
            {
                retry--;
                string ip = Util.GetIniFileString(iniPLC, "PLC", "IP", "192.168.0.1");
                if (!Util.PingTest(ip))
                {
                    FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                    Thread.Sleep(1000);
                    retry--;
                    continue;
                }

                try
                {
                    XGCommSocket XGComm = new XGCommSocket();
                    string port = Util.GetIniFileString(iniPLC, "PLC", "Port", "2004");
                    if (XGComm.Connect(ip, Convert.ToInt32(port)) != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                        Thread.Sleep(1000);
                        retry--;
                        continue;
                    }

                    UInt16[] uWrite = new UInt16[1];
                    uWrite[0] = (UInt16)value;
                    // PLC 쓰기
                    uint uReturnWrite = XGComm.WriteDataWord(devType, address, 1, false, uWrite);
                    if (uReturnWrite != (uint)XGCOMM_FUNC_RESULT.RT_XGCOMM_SUCCESS)
                    {
                        string s = "WritePLCData Failed. " + devType + address.ToString() + " : " + value.ToString() + " RETRY:" + retry.ToString();
                        FormSendMessage(WM_USER_LOG_PLC, 0, s);
                        Thread.Sleep(1000);
                        retry++;
                        continue;
                    }
                    else
                    {
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    string s = "WritePLCData Failed. " + devType + address.ToString() + " : " + value.ToString() + " RETRY:" + retry.ToString();
                    s += " EXEPTION: " + ex.Message;
                    FormSendMessage(WM_USER_LOG_PLC, 0, s);
                    Thread.Sleep(1000);
                    retry--;
                    continue;
                }

                if (success)
                {
                    break;
                }
            }

            if (!success)
                return 999;

            return 0;
        }

        public class FinalServerImage : IComparable<FinalServerImage>
        {
            public int CameraID = -1;
            public int ImageIndex = -1;
            public string FileName = string.Empty;
            public int CompareTo(FinalServerImage other)
            {
                if (this.CameraID < other.CameraID)
                    return -1;

                return this.ImageIndex.CompareTo(other.ImageIndex);
            }
        }

        private void UpdateServerData(int res, string file)
        {
            /*
            SqlConnection conn = new SqlConnection(CONFIG.GetConnectionString(iniServer, out string ip));
            if (ip == string.Empty)
                return;
            string[] split = ip.Split(',');
            if (!Util.PingTest(split[0]))
            {
                FormSendMessage(WM_USER_STATUS_SERVER, 0, "");
                return;
            }

            int status = 2;
            if (res == 0)
            {
                status = 3;
                file = string.Empty;
            }
            if (res == 4)
                status = 4;

            string cmd = "UPDATE EQIP_IF SET STATUS='" + status.ToString() + "', IMG_FILE_NAME1='" + file + "' WHERE EQIP='" + EQUIPMENT + "'";
            SqlCommand qry = new SqlCommand(cmd, conn);

            int retry = 0;
            while (true)
            {
                bool succeed = false;
                try
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    qry.ExecuteNonQuery();
                    FormSendMessage(WM_USER_STATUS_SERVER, 1, "");
                    Util.WriteLog("UpdateServerDatabase: " + cmd);
                    succeed = true;
                }
                catch (Exception e)
                {
                    FormSendMessage(WM_USER_STATUS_SERVER, 0, "UpdateServerData failed. " + e.Message);
                    Util.WriteLog("UpdateServerDatabase FAILED: " + cmd);
                    Util.WriteLog(e.Message);
                    retry++;
                    Thread.Sleep(100);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
                if (succeed || retry > 200)
                    break;
            }
            */
        }

        public Dictionary<int, FileSystemWatcher> _watcher = new Dictionary<int, FileSystemWatcher>();
        internal void ModelChanged(string model, bool isSimul = false)
        {
            if (timerWorker != null)
                timerWorker.Change(Timeout.Infinite, Timeout.Infinite);

            FormSendMessage(WM_USER_TEST_INIT);
            FormSendMessage(WM_USER_FLICKER, 0);

            ModelList = Model.GetModelList(iniModel, CameraList);
            CurrentModel = ModelList.Find(x => x.ModelPLCNumber.ToString() == model.ToUpper());
            if (CurrentModel == null)
            {
                if (CurrentModel == null)
                {
                    timerWorker.Change(1000, 1000);
                    IsNewData = 0;
                    return;
                }
                ModelList.Add(CurrentModel);
            }

            string file = Path.Combine(Util.GetWorkingDirectory(), "Images", Path.GetFileName(CurrentModel.ModelImage));
            if (File.Exists(file))
                CurrentModel.ModelImage = file;

            FormSendMessage(WM_USER_TEST_MODELNAME, 0, CurrentModel.ModelServerName);
            RunMode = isSimul ? MODE_SIMULATED : MODE_NORMAL;
            // 시뮬레이션 모드이면 서버는 리셋
            if (isSimul)
                UpdateServerData(4, string.Empty);

            FinalImageFiles = new List<string>[CurrentModel.CameraList.Count];
            // 포함된 모든 프로그램에 대한 저장소를 할당하고 빈 문자열이 있으면 완료되지 않은 것으로 판정
            for (int i = 0; i < CurrentModel.CameraList.Count; i++)
            {
                FinalImageFiles[i] = new List<string>();
            }

            foreach (var cam in CurrentModel.CameraList)
            {
                string path = Path.Combine(DataFolder, $"CAM{cam.ID}");
                Directory.CreateDirectory(path);

                if (Directory.Exists(path))
                {
                    _watcher[cam.ID] = new FileSystemWatcher()
                    {
                        Path = Path.Combine(DataFolder, $"CAM{cam.ID}"),
                        NotifyFilter = NotifyFilters.LastWrite,
                        Filter = "*.*",
                        IncludeSubdirectories = true
                    };

                    _watcher[cam.ID].Changed += (sender, e) => OnImageFileCreated(sender, e, cam.ID);
                    _watcher[cam.ID].EnableRaisingEvents = true;
                }
            }

            CurrentModel.IsFinished = false;
            IsNewData = 1;
            FormSendMessage(WM_USER_PAINT);

            if (timerWorker != null)
                timerWorker.Change(0, 1000);

            CurrentModel.dtStarted = DateTime.Now;
        }

        public List<string>[] FinalImageFiles;
        private void OnImageFileCreated(object sender, FileSystemEventArgs e, int cameraId)
        {
            if (File.Exists(e.FullPath))
            {
                if (!FinalImageFiles[cameraId - 1].Contains(e.FullPath))
                {
                    FinalImageFiles[cameraId - 1].Add(e.FullPath);
                }
            }
        }

        public const int MODE_SIMULATED = 1;
        public const int MODE_NORMAL = 2;
        public int RunMode = MODE_NORMAL;
        public enum JUDGE_RESULT { _NONE = 0, _OK = 1, _NG = 2 }
        private void DoProcess()
        {
            // PLC 데이터 읽는 스레드가 실행 중인지 확인
            if (PLCReaderThread == null || PLCReaderThread.ThreadState == ThreadState.Stopped)
            {
                StartPLCReader();
                Thread.Sleep(1000);
                return;
            }

            // 초기 상태임
            ////////////////////////////////////////////////////////////////////////////////
            if (IsNewData == 0)
            {
                /*
                if (RunMode == MODE_NORMAL)
                {
                    if (true == ReadServerDatabase(out string model, out bool isReset))
                    {
                        ModelChanged(model);
                    }
                    else if (isReset == true)
                    {
                        ResetAllValues();
                        FormSendMessage(WM_USER_MESSAGE_GENERAL, 0, "Server is RESET");
                        IsNewData = 0;
                        return;
                    }
                }
                */
            }

            // 모델 데이터 수신됨
            ////////////////////////////////////////////////////////////////////////////////
            if (IsNewData == 1)
            {
                // 모델 설정
                int addrModel = Int32.Parse(Util.GetIniFileString(iniPLC, "Model", "Address", "150"));
                char cDeviceType = Util.GetIniFileString(iniPLC, "Model", "DeviceType", "D")[0];
                int modelValue = CurrentModel.ModelPLCNumber;
                uint ret = WritePLCData(cDeviceType, addrModel, modelValue);
                /*
                // 모델 설정 데이터 쓰기가 실패하면 에러로 처리
                if (ret != 0)
                {
                    FormSendMessage(WM_USER_STATUS_PLC, 0, "");
                    IsNewData = 0;
                    UpdateServerData(4, string.Empty);
                    return;
                }
                */
                // 트리거 설정
                for (int i = 0; i < CameraList.Count; i++)
                {
                    int addrTrigger = Util.GetIniFileInt(iniPLC, $"CAMERA{(i + 1)}Trigger", "Address", 0);
                    WritePLCData(cDeviceType, addrTrigger, 1);
                }

                Thread.Sleep(1000);
                IsNewData = 2;
                return;
            }
            ////////////////////////////////////////////////////////////////////////////////

            // 측정 중
            ////////////////////////////////////////////////////////////////////////////////
            if (IsNewData == 2)
            {
                if (CurrentModel.IsFinished == false && (Data_Finish[0] + Data_Finish[1] == 2))
                {
                    IsNewData = 0;
                    Thread.Sleep(500);

                    for (int cam = 0; cam < 2; cam++)
                    {
                        int camNo = cam + 1;
                        int blockCount = Data_Result_Cam[cam].Length;
                        for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
                        {
                            int dataIdx = 0;
                            int progNo = Data_Result_Cam[cam][blockIndex][dataIdx];
                            dataIdx = 1;
                            int progResult = Data_Result_Cam[cam][blockIndex][dataIdx];
                            // 스텝 번호와 결과가 둘다 0이 아니면 판정이 완료된 것임
                            if (progNo != 0 && progResult != 0)
                            {
                                dataIdx = 2;
                                int blockLength = Data_Result_Cam[cam][blockIndex].Length - 2;
                                int toolNum = 1;
                                for (int idx = dataIdx; idx < blockLength; idx += 2)
                                {
                                    // ---------------------------------------
                                    // ★ Tool 갱신 처리
                                    // ---------------------------------------
                                    foreach (var label in CurrentModel.LabelList)
                                    {
                                        foreach (var tool in label.ToolList)
                                        {
                                            // posAdjust가 true면 툴이 하나씩 뒤로 밀림
                                            int gap = 0;
                                            //if (tool.PosAdjust == true)
                                            //    gap = 2;

                                            int toolResult = Data_Result_Cam[cam][blockIndex][idx + gap] == 0 ? 2 : 1;
                                            int toolScore = Data_Result_Cam[cam][blockIndex][idx + gap + 1];
                                            if (tool.CameraNo == camNo && tool.ProgramNo == progNo && tool.ToolNo == toolNum)
                                            {
                                                tool.Result = toolResult;
                                            }
                                        }
                                        label.Result = progResult;
                                    }
                                    toolNum++;
                                }
                                FormSendMessage(WM_USER_MESSAGE_STEPRESULT, camNo, progNo.ToString());
                            }
                        }
                    }

                    int totalResult = 1;
                    for (int i = 0; i < CurrentModel.LabelList.Count; i++)
                    {
                        if (CurrentModel.LabelList[i].ToolList.Any(x => x.Result == 2))
                        {
                            totalResult = 0;
                            break;
                        }
                    }
                    foreach (var label in CurrentModel.LabelList)
                    {
                        foreach (var tool in label.ToolList)
                        {
                            Util.WriteLog($"CAM{tool.CameraNo}, Program:{tool.ProgramNo}, Tool:{tool.ToolNo}, Result:{tool.Result}");
                        }
                    }

                    Form loading = new Form
                    {
                        Width = 500,
                        Height = 120,
                        FormBorderStyle = FormBorderStyle.None,
                        StartPosition = FormStartPosition.CenterScreen,
                        WindowState = FormWindowState.Maximized,
                        ControlBox = false,
                        Text = "Processing...",
                        TopMost = true
                    };

                    Label lbl = new Label
                    {
                        Text = "Processing Final Result...",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                    };
                    lbl.Font = new Font("Microsoft San Serif", 54, FontStyle.Bold);

                    loading.Controls.Add(lbl);

                    //loading.Show();
                    loading.Refresh();

                    CurrentModel.IsFinished = true;
                    DateTime dt = DateTime.Now;
                    FormSendMessage(WM_USER_DATETIME, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + $" ({(dt - CurrentModel.dtStarted).TotalSeconds.ToString("F1")})");
                    FormSendMessage(WM_USER_RESULT, totalResult, "");
                    /*
                    try
                    {
                        // 여기서 NG가 발생한 이미지를 보여준다
                        for (int i = 0; i < CurrentModel.CameraList.Count; i++)
                        {
                            List<string> imageList = FinalImageFiles[i];
                            foreach (string f in imageList)
                            {
                                if (f.ToUpper().Contains("NG"))
                                {
                                    if (Owner != null && !Owner.IsDisposed)
                                    {
                                        if (Owner.InvokeRequired)
                                            Owner.Invoke(new Action(() => LoadNGImage(f, $"pic{i + 1}")));
                                        else
                                            LoadNGImage(f, $"pic{i + 1}");
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.WriteLog($"Show NG Image Exeption. {ex.Message}", "LogErr", "ImageFile");
                    }
                    */
                    string finalImageFile = MakeFinalResultImage(dt, (totalResult == 0) ? "NG" : string.Empty);

                    string fileName = Path.GetFileName(finalImageFile);
                    if (RunMode == MODE_NORMAL)
                    {
                        // 정상 동작 모드일 때만 서버 업데이트를 하고, 시뮬레이션 모드이면 하지 않음
                        UpdateServerData(totalResult, fileName);
                    }
                    else
                    {
                        // When OK Only
                        if (totalResult != 0)
                        {
                            // Repair Mode이므로 라벨 출력
                            string iniPrint = Util.GetWorkingDirectory() + "\\Print.ini";
                            Util.SetIniFileString(iniPrint, "Setup", "ModelNo", CurrentModel.ModelPLCNumber.ToString());
                            Util.SetIniFileString(iniPrint, "Setup", "Sequence", dt.ToString("HHmmss"));
                            System.Diagnostics.Process.Start(Util.GetWorkingDirectory() + "\\LabelPrint.exe");
                        }
                    }

                    if (fileName != string.Empty)
                    {
                        // 최종 결과 텍스트 파일로 저장
                        string contents = dt.ToString("yyyy-MM-dd HH:mm:ss") + "\t" +
                            CurrentModel.ModelServerName + "\t" +
                            (totalResult == 1 ? "OK" : "NG") + "\t" + fileName + "\r\n";
                        WriteResult(dt, contents);
                    }

                    RunMode = MODE_NORMAL;
                    IsNewData = 0;

                    TerminateFileWather();

                    loading.Close();
                    loading.Dispose();
                }
            }
            ////////////////////////////////////////////////////////////////////////////////

            // 측정 준비 또는 진행 중 서버 STATUS가 0으로 바뀌면 모든 데이터 리셋
            // 시뮬레이션이 아니라 정상 동작 모드에서 서버 상태 체크
            // NQ5는 서버 없이 구동
            //CheckServerResetStatus();
        }

        private void LoadNGImage(string file, string picName)
        {
            var pb = Owner.Controls.Find("picName", true).FirstOrDefault() as PictureBox;
            if (pb != null)
            {
                pb.Image = Util.GetBitmapFromFile(file);
            }
        }

        private void TerminateFileWather()
        {
            foreach (var cam in CurrentModel.CameraList)
            {
                if (_watcher.ContainsKey(cam.ID))
                {
                    var w = _watcher[cam.ID];

                    w.EnableRaisingEvents = false;
                    w.Changed -= (sender, e) => OnImageFileCreated(sender, e, cam.ID);  // 이벤트 해제
                    w.Dispose();  // 리소스 해제
                }
            }
        }

        private string MakeFinalResultImage(DateTime dt, string result)
        {
            string fullpath = string.Empty;

            try
            {
                string dir = SaveFolder + "\\" + dt.ToString("yyyy") + "\\" +
                    dt.ToString("MM-dd");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                fullpath = dir + "\\" + dt.ToString("yyyyMMdd_HHmmss") + "_" + CurrentModel.ModelServerName +
                    ((result != string.Empty) ? "_" + result : string.Empty) + ".png";

                List<string> orderedFiles = FinalImageFiles
                    .Where(x => x != null)     // null 요소가 있을 경우 대비
                    .SelectMany(x => x)        // 내부 리스트를 펼침
                    .ToList();

                int countFileList = orderedFiles.Count;
                if (countFileList < 1)
                    return string.Empty;

                // 전체 화면 캡쳐를 위해서 이미지를 하나 더 추가
                countFileList++;

                // 가로, 세로 사각형 개수를 구하기 위해 근사값을 사용합니다.
                int squaresPerColumn = (int)Math.Ceiling(Math.Sqrt(countFileList));
                int squaresPerRow = (int)Math.Ceiling((double)countFileList / squaresPerColumn);

                Bitmap bmpEach = Util.GetBitmapFromFile(orderedFiles[0]);
                if (bmpEach == null)
                    return string.Empty;

                int size = Util.GetIniFileInt(iniSetup, "Setup", "ImageCompressionRatio", 100);
                int widthOne = (int)(bmpEach.Width * size * 1.0 / 100.0);
                int heightOne = (int)(bmpEach.Height * size * 1.0 / 100.0);

                // 이미지들 사이의 빈 공간
                int gapWidth = 10;
                int gapHeight = 10;

                // 최종 이미지 크기
                int TotalWidth = squaresPerColumn * (widthOne + gapWidth);
                int TotalHeight = squaresPerRow * (heightOne + gapHeight);

                // 새로 생성된 이미지
                Bitmap bmpFinal = new Bitmap(TotalWidth, TotalHeight);
                using (Graphics graphics = Graphics.FromImage(bmpFinal))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    int imageIndex = 0;
                    int resultImageIndex = 0;
                    for (int row = 0; row < squaresPerRow; row++)
                    {
                        for (int col = 0; col < squaresPerColumn; col++)
                        {
                            if (imageIndex < countFileList)
                            {
                                Bitmap bmp = null;
                                // 인덱스가 0인건 전체 화면 캡쳐 이미지
                                if (imageIndex == 0)
                                {
                                    //bmp = Util.FormCaptureScreen(Owner, string.Empty);
                                    Owner.Invoke(new Action(() =>
                                    {
                                        bmp = new Bitmap(Owner.Width, Owner.Height);
                                        Owner.DrawToBitmap(bmp, new Rectangle(0, 0, Owner.Width, Owner.Height));
                                    }));
                                }
                                else
                                {
                                    if (resultImageIndex < orderedFiles.Count)
                                    {
                                        string file = orderedFiles[resultImageIndex];
                                        bmp = Util.GetBitmapFromFile(file);
                                        resultImageIndex++;
                                    }
                                }

                                if (bmp != null)
                                {
                                    int x = col * (widthOne + gapWidth);
                                    int y = row * (heightOne + gapHeight);
                                    graphics.DrawImage(bmp, x, y, widthOne, heightOne);
                                    //graphics.DrawString(imageIndex.ToString(), new Font("Verdana", 200), new SolidBrush(Color.Red), new Point(x, y));
                                }
                            }
                            imageIndex++;
                        }
                    }
                    bmpFinal.Save(fullpath);
                    string userMessage = "Final Image Saved. " + fullpath;
                    FormSendMessage(WM_USER_MESSAGE_GENERAL, 1, userMessage);

                    //System.Diagnostics.Process.Start(fullpath);
                }
            }
            catch (Exception e)
            {
                string userMessage = "MakeFinalResultImage failed. " + e.Message;
                FormSendMessage(WM_USER_MESSAGE_GENERAL, 0, userMessage);
                return string.Empty;
            }

            return fullpath;
        }

        public void DoFindFinalImage(object step)
        {
            /*
            Step s = (Step)step;
            CameraObject cam = CameraList.Find(x => x.ID == s.CameraID);
            if (cam == null)
                return;

            string file = FindFinalImageFile(cam);
            Util.WriteLog("FindFinalImageFile. CAM:" + cam.ID.ToString() + " " + file);
            s.StepImageFile = file;
            string contents = s.Rotate.ToString() + "," + file;
            FormSendMessage(WM_USER_FINAL_IMAGE, cam.ID - 1, contents);
            FormSendMessage(WM_USER_PAINT);
            */
        }

        private string FindFinalImageFile(CameraObject cam)
        {
            if (cam.ResultFileName == string.Empty)
                return string.Empty;

            string result = string.Empty;
            int retry = 0;
            string folder = DataFolder + "\\CAM" + cam.ID.ToString();
            while (retry < 100)
            {
                if (Directory.Exists(folder))
                {
                    string[] files = Directory.GetFiles(folder, cam.ResultFileName + "*", SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        // 파일이 있다면 생성된 것으로 간주
                        //result = files[0];
                        //break;
                        try
                        {
                            // 파일에 읽기 접근 시도
                            using (FileStream fs = new FileStream(files[0], FileMode.Open, FileAccess.Read, FileShare.None))
                            {
                                // 파일이 열렸다면, 생성이 완료된 것으로 간주
                                result = files[0];
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            // 파일 접근이 불가능한 경우, 대기 후 재시도
                            //Util.WriteLog("File IOException. " + filePath, "Log", "File");
                            Thread.Sleep(100);
                            retry++;
                            continue;
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                        retry++;
                        continue;
                    }
                }
                else
                {
                    Thread.Sleep(100);
                    retry++;
                }
            }
            return result;
        }
    }
}