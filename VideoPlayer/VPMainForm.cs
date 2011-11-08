
//
// Main GUI form for the player.
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
    public partial class VPMainForm : Form
    {
        public VPMainForm()
        {
            InitializeComponent();

            playButton.Click += new EventHandler(OnPlayButtonClick);
        }

        private void OnPlayButtonClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
