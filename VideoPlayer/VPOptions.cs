


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
    public partial class VPOptions : Form
    {
        public bool wasOKPressed;

        public VPOptions()
        {
            wasOKPressed = false;

            InitializeComponent();

            okButton.Click += new EventHandler(OnOKButtonClick);
            cancelButton.Click += new EventHandler(OnCancelButtonClick);
        }

        private void OnCancelButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnOKButtonClick(object sender, EventArgs e)
        {
            wasOKPressed = true;
            this.Close();
        }

        public int GetSceneDuration()
        {
            return sceneTrackBar.Value;
        }

        public int GetSummaryPercentage()
        {
            return summaryTrackBar.Value;
        }
    }
}
