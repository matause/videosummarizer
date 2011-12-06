
//
// Contains information about the authors.
//


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VideoPlayer
{
    public partial class VPAboutForm : Form
    {
        public VPAboutForm()
        {
            InitializeComponent();

            // Button Callbacks
            okButton.Click += new EventHandler(OnOKButtonClick);
        }

        //
        // Button Callbacks
        //

        private void OnOKButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
