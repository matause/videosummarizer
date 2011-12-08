
//
// Contains summarization options.
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
    public enum ShotSelectionAlgorithm
    {
        HISTOGRAM,
        MOTION
    };

    public partial class VPOptionsForm : Form
    {
        public bool wasOKPressed;

        private const String DEFAULT_SCENE_LABEL = "Scene Duration: ";
        private const String DEFAULT_SUMMARY_LABEL = "Summary Duration: ";

        public VPOptionsForm()
        {
            wasOKPressed = false;

            InitializeComponent();

            // Button Callbacks
            okButton.Click += new EventHandler(OnOKButtonClick);
            cancelButton.Click += new EventHandler(OnCancelButtonClick);

            // Trackbar Callbacks
            sceneTrackBar.Scroll += new EventHandler(OnSceneTrackbarScroll);
            summaryTrackBar.Scroll += new EventHandler(OnSummaryTrackbarScroll);
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

        private void OnSceneTrackbarScroll(object sender, EventArgs e)
        {
            sceneDurationLabel.Text = DEFAULT_SCENE_LABEL + sceneTrackBar.Value + " (secs)";
        }

        private void OnSummaryTrackbarScroll(object sender, EventArgs e)
        {
            summaryDuration.Text = DEFAULT_SUMMARY_LABEL + summaryTrackBar.Value + "%";
        }

        public int GetSceneDuration()
        {
            return sceneTrackBar.Value;
        }

        public int GetSummaryPercentage()
        {
            return summaryTrackBar.Value;
        }

        public ShotSelectionAlgorithm GetShotSelectionAlgorithm()
        {
            ShotSelectionAlgorithm result = ShotSelectionAlgorithm.HISTOGRAM;

            if (motionAnalysisButton.Checked == true)
            {
                result = ShotSelectionAlgorithm.MOTION;
            }

            return result;
        }
    }
}
