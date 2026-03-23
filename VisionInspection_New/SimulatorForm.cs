using Common;
using KEYENCE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YONGSAN_CPAD_VISION;

namespace VisionInspection
{
    public partial class SimulatorForm : Form
    {
        // P/Invoke declarations
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HTCAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void label6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
            }
        }

        public MeasurementForm_IMG owner_img = null;
        public MeasurementForm_CA owner_ca = null;
        public SimulatorForm(MeasurementForm_CA p1, MeasurementForm_IMG p2)
        {
            InitializeComponent();
            owner_img = p2;
            owner_ca = p1;
            Owner = p2;
        }

        public List<Model> ModelList = new List<Model>();
        public List<CameraObject> CameraList = null;
        private void SimulatorForm_Load(object sender, EventArgs e)
        {
            LoadModels();
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            LoadModels();
        }

        private void LoadModels()
        {
            if (owner_img != null)
            {
                if (owner_img.VisionCore == null)
                    return;

                CameraList = CameraObject.GetCameraList(owner_img.VisionCore.iniCamera);
                ModelList = Model.GetModelList(owner_img.VisionCore.iniModel, CameraList);

                listModel.Items.Clear();
                for (int i = 0; i < ModelList.Count; i++)
                {
                    Model mo = ModelList[i];
                    ListViewItem item = listModel.Items.Add(mo.ModelServerName);
                    item.SubItems.Add(mo.ModelPLCNumber.ToString());
                }
            }
            if (owner_ca != null)
            {
                if (owner_ca.VisionCore == null)
                    return;

                CameraList = CameraObject.GetCameraList(owner_ca.VisionCore.iniCamera);
                ModelList = Model.GetModelList(owner_ca.VisionCore.iniModel, CameraList);

                listModel.Items.Clear();
                for (int i = 0; i < ModelList.Count; i++)
                {
                    Model mo = ModelList[i];
                    ListViewItem item = listModel.Items.Add(mo.ModelServerName);
                    item.SubItems.Add(mo.ModelPLCNumber.ToString());
                }
            }
        }

        private void btnSendModel_Click(object sender, EventArgs e)
        {
            if (listModel.SelectedItems.Count < 1)
                return;
            if (owner_img != null)
                owner_img.ManualModelChanged(listModel.SelectedItems[0].SubItems[1].Text);
            if (owner_ca != null)
                owner_ca.ManualModelChanged(listModel.SelectedItems[0].SubItems[1].Text);
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
