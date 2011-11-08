
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

using SlimDX;

namespace VideoPlayer
{
    public partial class VPMainForm : Form
    {
        Color4 clearColor;
        SplitterPanel renderTarget;
        Direct2DRenderer renderer;

        public VPMainForm()
        {
            clearColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            renderer = new Direct2DRenderer(clearColor);

            InitializeComponent();

            // Get the render target from the GUI.
            renderTarget = displaySplitContainer.Panel1;

            //
            // Set up callbacks.
            //

            this.Load += new EventHandler(OnLoad);

            fileExitMenuItem.Click += new EventHandler(OnFileExit);
            fileOpenMenuItem.Click += new EventHandler(OnFileOpen);
            fileCloseMenuItem.Click += new EventHandler(OnFileClose);

            renderTarget.Paint += new PaintEventHandler(OnRender);
            renderTarget.Resize += new EventHandler(OnResize);

            playButton.Click += new EventHandler(OnPlayButtonClick);
            stopButton.Click += new EventHandler(OnStopButtonClick);
        }

        //
        // Main form callbacks.
        //

        private void OnLoad(object sender, EventArgs e)
        {
            // Try to load up Direct2D
            bool result = renderer.OnInitialize(renderTarget.Handle, renderTarget.Width,
                renderTarget.Height);
            
            if (result == false)
            {
                MessageBox.Show("Direct2D failed to load." +
                    "  The application will now exit.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        //
        // Top splitter panel (our Direct2D render target ) callbacks.
        //

        private void OnRender(object sender, PaintEventArgs e)
        {
            renderer.OnRender();
        }

        private void OnResize(object sender, EventArgs e)
        {
            renderer.OnResize(renderTarget.Width, renderTarget.Height);
        }

        //
        // Menu Callbacks
        //

        public void OnFileExit(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OnFileClose(object sender, EventArgs e)
        {
            // TODO: Clear all the video and audio data and reset the renderer
            // for a new video.
        }

        private void OnFileOpen(object sender, EventArgs e)
        {
            // First, read in video file.
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Title = "Open the video file.";
            dlg.Filter = "576v files (*.576v)|*.576v";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // TODO: read it in.
            }
            else
            {
                // No sense in trying to read audio if the user didn't select
                // a valid video file.
                return;
            }

            // Then, read in audio file.
            dlg.Title = "Open the audio file.";
            dlg.Filter = "Wav files (*.wav)|*.wav";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // TODO: read it in.
            }
            else
            {
                // Bail
                return;
            }

            // TODO: Call a function to display first frame on the screen
            // and get the player ready to rock for playing a video.
        }

        //
        // Button Callbacks.
        //

        private void OnPlayButtonClick(object sender, EventArgs e)
        {
            // TODO
        }

        private void OnStopButtonClick(object sender, EventArgs e)
        {
            // TODO
        }
    }
}
