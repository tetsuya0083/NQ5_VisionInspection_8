using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionInspection
{
    public partial class LoggingForm : Form
    {
        public LoggingForm()
        {
            InitializeComponent();
        }

        public void Logging(string s)
        {
            if(list.Items.Count > 1000)
                list.Items.RemoveAt(0);

            string log = "[" + DateTime.Now.ToString("HH:mm:ss") + "]" + s;
            list.Items.Add(log);
            list.SelectedIndex = list.Items.Count - 1;
            list.SelectedIndex = -1;
        }

        private void LoggingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
