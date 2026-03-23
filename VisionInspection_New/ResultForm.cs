using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YONGSAN_CPAD_VISION;

namespace VisionInspection
{
    public partial class ResultForm : Form
    {
        public ResultForm()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        internal void InitValues()
        {
            lblTitle.Text = $"NG IMAGES - {CurrentIndex + 1} / {FinalImages.Count}";
            ShowImage();
        }

        public int CurrentIndex = 0;
        public List<string> FinalImages = new List<string>();
        private void ShowImage()
        {
            lblTitle.Text = $"NG IMAGES - {CurrentIndex + 1} / {FinalImages.Count}";
            picResult.Image = Util.GetBitmapFromFile(FinalImages[CurrentIndex]);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            CurrentIndex++;
            if (CurrentIndex >= FinalImages.Count)
                CurrentIndex = 0;

            ShowImage();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CurrentIndex--;
            if (CurrentIndex < 0)
                CurrentIndex = FinalImages.Count - 1;

            ShowImage();
        }

        private void ResultForm_Load(object sender, EventArgs e)
        {
            
        }

        internal void SetResultImages(YONGSAN_VISION_CORE core)
        {
            for (int i = 0; i < core.CameraList.Count; i++)
            {
                foreach (string f in core.FinalImageFiles[i])
                {
                    if (f.ToUpper().Contains("NG"))
                        FinalImages.Add(f);
                }
            }
            CurrentIndex = 0;
            InitValues();
        }
    }
}
