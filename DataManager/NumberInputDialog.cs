using System;
using System.Windows.Forms;

namespace DataManager
{
    public partial class NumberInputDialog : Form
    {
        public int Value { get; private set; }
        private int _minValue;

        public NumberInputDialog(string title, int currentValue, int minValue)
        {
            InitializeComponent();
            this.Text = title;
            _minValue = minValue;
            txtValue.Text = currentValue.ToString();
            lblMinimum.Text = $"Minimum: {minValue}";
        }

        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtValue.Text, out int val) || val < _minValue)
            {
                MessageBox.Show($"Please enter a value of {_minValue} or greater.", "Input Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtValue.Focus();
                txtValue.SelectAll();
                return;
            }
            Value = val;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
