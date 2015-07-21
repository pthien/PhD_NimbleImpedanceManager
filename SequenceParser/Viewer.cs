using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nimble.Sequences
{
    public partial class Viewer : Form
    {
        public string SequenceDirectory { get; set; }
        public string ImpedanceDirectory { get; set; }

        public Viewer()
        {
            InitializeComponent();
        }

        private void Viewer_Load(object sender, EventArgs e)
        {

        }
    }
}
