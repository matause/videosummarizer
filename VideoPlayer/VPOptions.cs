


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

        private const String DEFAULT_SCENE_MINUTES = "0";
        private const String DEFAULT_SCENE_SECONDS = "3";

        public VPOptions()
        {
            wasOKPressed = false;

            InitializeComponent();

            sceneMinutesBox.Text = DEFAULT_SCENE_MINUTES;
            sceneSecondsBox.Text = DEFAULT_SCENE_SECONDS;

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

        // Retrieve information from the GUI
        public bool GetSceneTimeInSeconds(ref int result)
        {
            bool isDataValid = true;

            result = 0;
            try
            {
                result = Int32.Parse(sceneSecondsBox.Text);
                result += Int32.Parse(sceneMinutesBox.Text) * 60;
            }
            catch
            {
                isDataValid = false;
            }

            return isDataValid;
        }

        public int GetSummaryPercentage()
        {
            return summaryTrackBar.Value;
        }
    }
}
