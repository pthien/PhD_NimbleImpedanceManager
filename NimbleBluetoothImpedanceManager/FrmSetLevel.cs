using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NimbleBluetoothImpedanceManager
{
    public partial class FrmSetLevel : Form
    {
        public int Result { get; private set; }

        private int max, min;

        public FrmSetLevel(int max, int min)
        {
            InitializeComponent();
            this.max = max;
            this.min = min;
        }

        private void FrmSetLevel_Load(object sender, EventArgs e)
        {

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            int val;
            bool res = int.TryParse(txtNewLevel.Text, out val);
            if (res)
            {
                if (val <= max && val >= min)
                {
                    Result = val;
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(string.Format("Please enter a value betweeen {0} and {1}", min, max),"Error");
                }                
            }
            else
                MessageBox.Show(string.Format("Please enter a value betweeen {0} and {1}", min, max), "Error");

        }

        private void txtNewLevel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnOk_Click(null, null);

        }


        private void txtNewLevel_TextChanged(object sender, EventArgs e)
        {
           
        }
    }
}
