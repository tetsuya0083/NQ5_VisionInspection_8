using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionSetup
{
    public partial class CameraForm : Form
    {
        public MainForm owner = null;
        public int CameraNo = 0;
        public string IPAddress { get; set; } = "192.168.10.10";
        public int CommPort { get; set; } = 8500;
        public CameraForm(MainForm p, int no, string ip = "192.168.10.10", int port = 8500)
        {
            owner = p;
            CameraNo = no;
            IPAddress = ip;
            CommPort = port;
            InitializeComponent();
        }

        private void CameraForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void CameraForm_Load(object sender, EventArgs e)
        {
            lblNo.Text = CameraNo.ToString();
            txtIP.Text = IPAddress;
            txtPort.Text = CommPort.ToString();
        }

        private void btnCheckConnection_Click(object sender, EventArgs e)
        {
            if (!Util.PingTest(txtIP.Text))
            {
                btnCheckConnection.BackColor = Color.Red;
                btnCheckConnection.Text = "Connection Failed";
                return;
            }

            btnCheckConnection.Text = "Checking Connection...";
            try
            {
                TcpClient tcpClient = new TcpClient(txtIP.Text, Int32.Parse(txtPort.Text));
                NetworkStream stream = tcpClient.GetStream();
                btnCheckConnection.BackColor = Color.Lime;
                btnCheckConnection.Text = "Connected";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnCheckConnection.BackColor = Color.Red;
                btnCheckConnection.Text = "Connection Failed";
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //if(owner.CheckCameraConfiguration(txtIP.Text, Int32.Parse(txtPort.Text), out string err))
            IPAddress = txtIP.Text;
            CommPort = Int32.Parse(txtPort.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
