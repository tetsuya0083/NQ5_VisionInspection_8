using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabelPrint
{
    public partial class MainForm : Form
    {
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
        static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool StartDocPrinter(IntPtr hPrinter, int Level, [In] ref DOCINFOA pDocInfo);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.drv", SetLastError = true)]
        static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        [StructLayout(LayoutKind.Sequential)]
        public struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            timerStart.Start();
        }

        private void timerStart_Tick(object sender, EventArgs e)
        {
            timerStart.Stop();
            string ini = Util.GetWorkingDirectory() + "\\Print.ini";
            string comm = Util.GetIniFileString(ini, "Setup", "COM", "COM1");
            string productShortName = Util.GetIniFileString(ini, "Setup", "ProductShortName", ""); //"C/PADASS'Y(+)";
            string company = Util.GetIniFileString(ini, "Setup", "Company", ""); //"BWMW";
            string EONum = Util.GetIniFileString(ini, "Setup", "EONumber", ""); //"HPIR0436";
            string part4M = Util.GetIniFileString(ini, "Setup", "Part4M", ""); //"J1D1";
            string AorAt = Util.GetIniFileString(ini, "Setup", "AorAt", ""); //"A";
            string time = DateTime.Now.ToString("yyMMdd");
            string lastPrinted = Util.GetIniFileString(ini, "Setup", "LastPrinted", string.Empty);
            string lastSequence = Util.GetIniFileString(ini, "Setup", "Sequence", string.Empty);
            int seq = 1;
            /*
            if (lastPrinted != time || !Int32.TryParse(lastSequence, out seq))
            {
                seq = 1;
            }
            else
            {
                seq++;
            }
            */
            string sequence = $"R{time}{seq.ToString("00000")}";
            // 마지막 출력 날짜와 시퀀스 값 갱신
            Util.SetIniFileString(ini, "Setup", "LastPrinted", time);
            Util.SetIniFileString(ini, "Setup", "Sequence", seq.ToString());

            string iniOffset = Util.GetWorkingDirectory() + "\\PrintOffset.ini";
            int x = Util.GetIniFileInt(iniOffset, "Setup", "OriginalX", 10);
            int y = Util.GetIniFileInt(iniOffset, "Setup", "OriginalY", 35);

            string key = "MODEL" + Util.GetIniFileString(ini, "Setup", "ModelNo", "");
            string modelName = Util.GetIniFileString(ini, key, "ModelName", ""); //"NE1a"
            string color = Util.GetIniFileString(ini, key, "Color", ""); //"NNB";
            string partNo = Util.GetIniFileString(ini, key, "PartNumber", ""); //"84701PI000NNB";
            string rpcs = Util.GetIniFileString(ini, key, "RPCS", ""); //"Q047N00B";
            string productType = Util.GetIniFileString(ini, key, "Type", ""); //(+), (-);
            string addInfo = Util.GetIniFileString(ini, key, "AdditionalInformation", string.Empty);
            string zplCommand = GenerateZPL(comm, x, y, company, color, modelName, productShortName, productType, partNo, rpcs, EONum, part4M, AorAt, time + " " + DateTime.Now.ToString("HH:mm"), sequence, addInfo);

            string printerName = Util.GetIniFileString(ini, "Setup", "PrinterName", "ZDesigner ZD421-203dpi ZPL");

            PrintBarcodeUSB(printerName, zplCommand);

            this.Close();
        }

        public void PrintBarcode(string comm, int x, int y, string company, string color, string modelName, string productShortName, string productType, string partNo, string rpcs, string eonum, string part4M, string aorat, string time, string seq)
        {
            // 시리얼 포트 설정
            SerialPort serialPort = new SerialPort
            {
                PortName = comm, // 사용할 COM 포트 이름 설정
                BaudRate = 9600,   // Baud Rate 설정 (프린터의 설정과 일치하도록 설정)
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                ReadTimeout = 500,
                WriteTimeout = 500
            };

            try
            {
                // 시리얼 포트 열기
                serialPort.Open();

                // ZPL 명령어 작성
                string zplCommand = "^XA^LH0,0^PW340^CVY" +
                    //"^FO160,25^A0N,40,40^FD" + color + "^FS" +
                    //"^FT^A0N,20,20^FD^FS" +
                    "^FO" + (x + 135).ToString() + "," + y.ToString() + "^A0N,23,23^FD" + modelName + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 22).ToString() + "^A0N,23,23^FD" + productShortName + productType + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 44).ToString() + "^A0N,23,23^FD" + partNo + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 66).ToString() + "^A0N,23,23^FD" + rpcs + " " + color + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 88).ToString() + "^A0N,23,23^FD" + time + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 110).ToString() + "^A0N,23,23^FD" + seq + "^FS" +
                    "^FO" + x.ToString() + "," + y.ToString() + "^BXN,4,200^FH_^FD[)>_1E06_1DV" + company + "_1DP" + partNo.Replace(" ", string.Empty) +
                    "_1DS" + rpcs + "_1DE" + eonum + "_1DT" + DateTime.Now.ToString("yyMMdd") +
                    "" + part4M + "" + aorat +
                    "" + seq + "_1D_1E_04^FS" +
                    "^XZ";

                // ZPL 명령어 전송
                serialPort.WriteLine(zplCommand);
                Util.WriteLog("------------------------------------------------------", "Log", "Print");
                Util.WriteLog("Label Printed", "Log", "Print");
                Util.WriteLog(zplCommand, "Log", "Print");
            }
            catch (Exception ex)
            {
                Util.WriteLog($"에러: {ex.Message}", "LogErr", "Print");
            }
            finally
            {
                // 시리얼 포트 닫기
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
            }
        }

        public string GenerateZPL(string comm, int x, int y, string company, string color, string modelName, string productShortName, string productType, string partNo, string rpcs, string eonum, string part4M, string aorat, string time, string seq, string addInfo)
        {
            string zplCommand = string.Empty;
            try
            {
                // ZPL 명령어 작성
                zplCommand = "^XA^LH0,0^PW340^CVY" +
                    //"^FO160,25^A0N,40,40^FD" + color + "^FS" +
                    //"^FT^A0N,20,20^FD^FS" +
                    "^FO" + (x + 135).ToString() + "," + y.ToString() + "^A0N,23,23^FD" + modelName + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 22).ToString() + "^A0N,23,23^FD" + productShortName + productType + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 44).ToString() + "^A0N,23,23^FD" + partNo + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 66).ToString() + "^A0N,23,23^FD" + time + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 88).ToString() + "^A0N,23,23^FD" + rpcs + "^FS" +
                    "^FO" + (x + 135).ToString() + "," + (y + 110).ToString() + "^A0N,23,23^FD" + seq + "^FS" +
                    "^FO" + x.ToString() + "," + y.ToString() + "^BXN,4,200^FH_^FD[)>_1E06_1DV" + company + "_1DP" + partNo.Replace("-", "") +
                    "_1DS" + rpcs + "_1DE" + eonum + "_1DT" + DateTime.Now.ToString("yyMMdd") +
                    "" + part4M + "" + aorat +
                    "" + seq + addInfo + "_1D_1E_04^FS" +
                    "^XZ";

                // ZPL 명령어 전송
                Util.WriteLog(zplCommand, "Log", "Print");
            }
            catch (Exception ex)
            {
                Util.WriteLog($"에러: {ex.Message}", "LogErr", "Print");
            }
            finally
            {
            }
            return zplCommand;
        }

        public string PrintBarcodeUSB(string printerName, string zplCommand)
        {
            string result = string.Empty;

            try
            {
                IntPtr hPrinter;
                DOCINFOA docInfo = new DOCINFOA
                {
                    pDocName = "ZPL Command",
                    pDataType = "RAW"
                };

                if (OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
                {
                    if (StartDocPrinter(hPrinter, 1, ref docInfo))
                    {
                        if (StartPagePrinter(hPrinter))
                        {
                            IntPtr unmanagedBytes = Marshal.StringToCoTaskMemAnsi(zplCommand);
                            WritePrinter(hPrinter, unmanagedBytes, zplCommand.Length, out _);
                            Marshal.FreeCoTaskMem(unmanagedBytes);
                            EndPagePrinter(hPrinter);
                        }
                        //Util.WriteLog(zplCommand, "Log", "Print");
                        EndDocPrinter(hPrinter);
                    }
                    ClosePrinter(hPrinter);
                }
            }
            catch (Exception ex)
            {
                Util.WriteLog($"에러: {ex.Message}", "LogErr", "Print");
                //Util.WriteLog(zplCommand, "LogErr", "Print");
                result = ex.Message;
            }

            return result;
        }
    }
}
