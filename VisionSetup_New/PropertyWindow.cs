using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vision.Shared;

namespace VisionSetup
{
    public partial class PropertyWindow : Form
    {
        public ModelSetupForm owner = null;
        public PropertyWindow(ModelSetupForm owner)
        {
            InitializeComponent();
            this.owner = owner;
        }

        private void PropertyWindow_Load(object sender, EventArgs e)
        {
        }

        internal void RefreshGrid()
        {
            Grid.Refresh();
        }

        private void Grid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            owner.Grid_PropertyValueChanged(Grid.SelectedObject, e);
        }

        internal void SelectedObject(LabelModel model)
        {
            Grid.SelectedObject = model;
        }

        private void btnAddLabel_Click(object sender, EventArgs e)
        {
            Grid.SelectedObject = owner.AddLabelModel();
        }

        private void btnDeleteLabel_Click(object sender, EventArgs e)
        {
            if (Grid.SelectedObject != null)
            {
                DialogResult dr = MessageBox.Show($"Do you want to delete label [{((LabelModel)Grid.SelectedObject).Text}]?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr != DialogResult.Yes)
                    return;

                owner.DeleteLabel(Grid.SelectedObject);
                Grid.SelectedObject = null;
            }
        }

        private void btnDuplicate_Click(object sender, EventArgs e)
        {
            if (Grid.SelectedObject != null)
            {
                Grid.SelectedObject = owner.DuplicateLabel(Grid.SelectedObject);
            }
        }

        private void PropertyWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
