using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KEYENCE
{
    public class CameraObject : Object
    {
        public int ID = 0;
        public string IPAddress = string.Empty;
        public int Port = 8500;
        public TcpClient Client;
        public NetworkStream Stream;
        // 명령을 보내고 다음 명령을 받을 준비가 되었는지 확인하는 플래그
        // 0이면 초기 상태, 1이면 프로그램 번호 셋팅 완료, 2이면 파일명 셋팅 완료
        public int IsReady = 0;
        public int TotalResult = -1;
        public List<string> listToolResult = new List<string>();
        public string ResultFileName = string.Empty;
        public ushort CurrentStep = 0;
        public ushort PreviousStep = 0;
        public ushort CurrentFinal = 0;
        public ushort PreviousFinal = 0;

        // PLC에서 읽은 값
        public ushort ValueStep = 0;
        public ushort ValueTrigger = 0;
        public ushort ValueFinal = 0;

        // 현재 읽은 트리거 값
        public ushort CurrentTrigger = 0;
        public ushort PreviousTrigger = 0;

        // 쓰여져야 할 트리거 값
        public ushort ShouldBeWriteTrigger = 0;

        // E104처럼 동시에 끝내야 할 경우 각각의 카메라가 측정 명령이 갔는지 설정하는 플래그
        // 0이면 아무 동작도 안함. 1이면 캡쳐해야 함
        public int NeedToCapture = 0;
        // 캡쳐가 완료된 플래그
        public bool CaptureFinished = false;

        public CameraObject()
        {
        }

        internal bool Send(string msg, out string errMsg)
        {
            bool result = false;
            errMsg = string.Empty;
            try
            {
                byte[] data = Encoding.ASCII.GetBytes(msg);
                Stream.Write(data, 0, data.Length);
                result = true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }
            return result;
        }

        internal string Receive(out string errMsg)
        {
            errMsg = string.Empty;
            try
            {
                var buffsize = Client.ReceiveBufferSize;
                byte[] instream = new byte[buffsize];
                Stream.Read(instream, 0, buffsize);
                string data = Encoding.Default.GetString(instream);
                //owner.Logging("CAM" + ID.ToString() + " Data Received: " + data);
                return data;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }
            return string.Empty;
        }

        internal static List<CameraObject> GetCameraList(string ini)
        {
            List<CameraObject> list = new List<CameraObject>();
            int count = Util.GetIniFileInt(ini, "Camera", "Count", 0);

            for (int i = 0; i < count; i++)
            {
                int idx = i + 1;
                CameraObject cam = new CameraObject();
                cam.ID = idx;
                cam.IPAddress = Util.GetIniFileString(ini, "Camera", "IP" + idx.ToString(), "");
                cam.Port = Util.GetIniFileInt(ini, "Camera", "Port" + idx.ToString(), 8500);
                list.Add(cam);
            }
            return list;
        }
    }

    public class VisionPanel : Panel
    {
        public VisionPanel(string name)
        {
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Dock = System.Windows.Forms.DockStyle.Left;
            this.Location = new System.Drawing.Point(383, 0);
            this.Name = name;
            this.Size = new System.Drawing.Size(383, 327);
            this.TabIndex = 1;
        }
    }

    public class VisionLabel : Label
    {
        public VisionLabel(string name, string text, VisionPanel panel)
        {
            this.BackColor = System.Drawing.Color.Silver;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Dock = System.Windows.Forms.DockStyle.Top;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 40F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = name;
            this.Size = new System.Drawing.Size(381, 81);
            this.TabIndex = 2;
            this.Text = text;
            this.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            panel.Controls.Add(this);
        }
    }

    public class VisionPictureBox : PictureBox
    {
        public VisionPictureBox(string name, VisionPanel panel)
        {
            this.BackColor = System.Drawing.Color.Black;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Location = new System.Drawing.Point(0, 81);
            this.Name = name;
            this.Size = new System.Drawing.Size(381, 244);
            this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.TabIndex = 3;
            this.TabStop = false;
            panel.Controls.Add(this);
        }
    }
}
