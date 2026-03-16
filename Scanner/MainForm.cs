using Common;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scanner
{
    public partial class MainForm : Form
    {
        public SerialPort _serialPort;
        public MainForm()
        {
            InitializeComponent();
        }

        string iniSetup = string.Empty;
        private void MainForm_Load(object sender, EventArgs e)
        {
            iniSetup = Path.Combine(Util.GetWorkingDirectory(), "Print.ini");
            string comm = Util.GetIniFileString(iniSetup, "Setup", "Scanner", string.Empty);
            string res = string.Empty;
            if (comm == string.Empty)
            {
                while (true)
                {
                    DialogResult dr = MessageBox.Show("Input COM Port Name", "COM Port", MessageBoxButtons.OKCancel);
                    if (dr == DialogResult.Cancel)
                    {
                        this.Close();
                        return;
                    }
                    res = Interaction.InputBox("Input COM Port Name", "COM Port");
                    Util.SetIniFileString(iniSetup, "Setup", "Scanner", res);
                    comm = res;

                    if (res != string.Empty)
                        break;
                }
            }
            

            // 1. 시리얼 포트 설정 (포트 번호는 장치 관리자에서 확인한 번호로 변경)
            _serialPort = new SerialPort(comm);

            _serialPort.BaudRate = 9600;
            _serialPort.Parity = Parity.None;
            _serialPort.StopBits = StopBits.One;
            _serialPort.DataBits = 8;
            _serialPort.Handshake = Handshake.None;

            // 2. 데이터 수신 이벤트 핸들러 등록
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                _serialPort.Open();
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }

            timerStart.Start();
        }

        // 3. 데이터가 들어왔을 때 실행되는 메서드
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting(); // 스캔된 데이터 읽기
            lblData.Text = indata;

            Util.SetIniFileString(iniSetup, "Setup", "AdditionalInforamtion", indata);

            this.Show();
            timerInterval.Start();
        }

        private void timerStart_Tick(object sender, EventArgs e)
        {
            timerStart.Stop();
            this.Hide();
        }

        private void timerInterval_Tick(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
