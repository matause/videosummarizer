
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

using System.Threading;
using System.Diagnostics;
using SlimDX;
using SlimDX.Windows;

namespace VideoPlayer
{
    public partial class VPMainForm : Form
    {
        bool isVideoLoaded;
        bool isVideoPlaying;
        bool isVideoPaused;
        bool isScrollPaused;

        bool isShowingHistogram;
        VPHistogramForm histogramForm;

        const int frameWidth = 320;
        const int frameHeight = 240;

        Color4 clearColor;

        Direct2DRenderer renderer;
        RenderForm renderTarget;

        Video video;
        Stopwatch videoTimer;

        Thread videoThread;

        public VPMainForm()
        {
            TrackBar.CheckForIllegalCrossThreadCalls = false;

            isVideoLoaded = false;
            isVideoPlaying = false;
            isVideoPaused = false;
            isScrollPaused = false;

            isShowingHistogram = false;
            histogramForm = null;

            clearColor = new Color4(0.0f, 0.0f, 0.0f, 0.0f);
            renderer = new Direct2DRenderer(clearColor);

            videoTimer = new Stopwatch();
            videoTimer.Stop();

            InitializeComponent();

            // Get the render target from the GUI.
            renderTarget = new RenderForm();
            renderTarget.FormBorderStyle = FormBorderStyle.None;
            renderTarget.TopLevel = false;
            displaySplitContainer.Panel1.Controls.Add(renderTarget);
            renderTarget.Show();

            renderTarget.Size = displaySplitContainer.Panel1.ClientSize;

            //
            // Set up callbacks.
            //
            this.Load += new EventHandler(OnLoad);
            this.FormClosed += new FormClosedEventHandler(OnClose);

            fileExitMenuItem.Click += new EventHandler(OnFileExit);
            fileOpenMenuItem.Click += new EventHandler(OnFileOpen);
            fileCloseMenuItem.Click += new EventHandler(OnFileClose);

            renderTarget.Paint += new PaintEventHandler(OnRender);
            displaySplitContainer.Panel1.Paint += new PaintEventHandler(OnRender);
            displaySplitContainer.Panel1.Resize += new EventHandler(OnResize);

            playButton.Click += new EventHandler(OnPlayButtonClick);
            stopButton.Click += new EventHandler(OnStopButtonClick);

            fileSummarizeMenuItem.Click += new EventHandler(OnSummarizeMenuClick);

            timelineBar.MouseUp += new MouseEventHandler(OnTimelineMouseUp);
            timelineBar.Scroll += new EventHandler(OnTimelineScroll);

            aboutAboutMenuItem.Click += new EventHandler(OnAboutClick);

            showHideHistogramMenuItem.Click += new EventHandler(OnShowHideHistogramClick);
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

        private void OnClose(object sender, FormClosedEventArgs e)
        {
            StopVideoThreads();
            renderer.OnShutdown();
        }

        //
        // Top splitter panel (our Direct2D render target ) callbacks.
        //

        private void OnRender(object sender, PaintEventArgs e)
        {
            if (video != null)
            {
                // Load current frame into D2D.
                lock (video)
                {
                    // We really shouldn't do this every frame.
                    // TODO: Change it later.
                    renderer.OnUpdate(video.GetCurrentFrame());
                }
            }

            // Draw the frame.
            renderer.OnRender();
        }

        private void OnResize(object sender, EventArgs e)
        {
            renderTarget.Size = displaySplitContainer.Panel1.ClientSize;
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
            StopVideoThreads();

            if (video != null)
            {
                video.videoReader.OnClose();
                video = null;
            }

            timelineBar.Value = 0;
            timeFrameLabel.Text = "00:00:00 / 0";

            isVideoPaused = false;
            isScrollPaused = false;

            renderer.OnReset();
            renderTarget.Invalidate();
        }

        private void OnFileOpen(object sender, EventArgs e)
        {
            // Clean up any existing video data.
            OnFileClose(sender, e);

            // First, read in video file.
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Title = "Open the video file.";
            dlg.Filter = "576v files (*.576v)|*.576v";

            bool result = true;
            string videoFile = "";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                video = new Video(frameWidth, frameHeight);
                videoFile = dlg.FileName;
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

            string audioFile = "";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                audioFile = dlg.FileName;
            }
            else
            {
                // Bail
                return;
            }

            result = video.OnInitialize(videoFile, audioFile);

            if (result == true)
            {
                isVideoLoaded = true;

                timelineBar.Value = 0;
                timelineBar.Maximum = (int)(video.videoDuration * 1000.0f);

                renderTarget.Invalidate();
            }
        }

        private void OnSummarizeMenuClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true)
            {
                // Stop video player
                StopVideoThreads();

                isVideoPaused = false;
                isScrollPaused = false;

                // Get analysis variables from the options GUI.
                VPOptionsForm optionsDlg = new VPOptionsForm();
                optionsDlg.ShowDialog(this);
                if (optionsDlg.wasOKPressed == false)
                {
                    // User pressed cancel. Just bail.
                    return;
                }

                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Title = "Save the video summary.";
                saveDlg.Filter = "576v files (*.576v)|*.576v";
                saveDlg.OverwritePrompt = true;
                saveDlg.AddExtension = true;
                saveDlg.DefaultExt = ".576v";

                DialogResult dlgResult = saveDlg.ShowDialog();
                if (dlgResult != DialogResult.OK)
                {
                    // User pressed cancel. Just bail.
                    return;
                }
                String videoFilePath = saveDlg.FileName;

                saveDlg.Title = "Save the audio summary.";
                saveDlg.Filter = "Wav files (*.wav)|*.wav";
                saveDlg.DefaultExt = ".wav";

                dlgResult = saveDlg.ShowDialog();
                if (dlgResult != DialogResult.OK)
                {
                    // User pressed cancel. Just bail.
                    return;
                }
                String audioFilePath = saveDlg.FileName;

                //
                // Example code to get data from the GUI.
                //

                int sceneTime = optionsDlg.GetSceneDuration();
                int summaryPercentage = optionsDlg.GetSummaryPercentage();
                ShotSelectionAlgorithm sSAlg = optionsDlg.GetShotSelectionAlgorithm();
                video.usePreProcessedData = optionsDlg.UsePreProcessedMotionData();

                // Compute the data for Shot detection analysis
                bool result = video.VideoAnalysis(videoFilePath, audioFilePath, sSAlg,
                    sceneTime, summaryPercentage);
            }
        }

        private void OnAboutClick(object sender, EventArgs e)
        {
            VPAboutForm aboutDlg = new VPAboutForm();
            DialogResult result = aboutDlg.ShowDialog(this);
        }

        private void OnShowHideHistogramClick(object sender, EventArgs e)
        {
            if (isShowingHistogram == true)
            {
                // Hide the histogram window.
                if (histogramForm != null)
                {
                    histogramForm.Hide();
                    histogramForm = null;
                }

                isShowingHistogram = false;
            }
            else
            {
                // Display the histogram window.
                histogramForm = new VPHistogramForm();
                histogramForm.Show(this);

                isShowingHistogram = true;
            }
        }

        //
        // Button Callbacks.
        //

        private void OnPlayButtonClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true)
            {
                // Normally start the video
                if (isVideoPlaying == false)
                {
                    StopVideoThreads();

                    int startingVideoTime = timelineBar.Value;
                    int startingAudioTime = timelineBar.Value;

                    videoTimer.Reset();

                    // Create new thread to play the video.
                    videoThread = new Thread(PlayVideo);

                    // This starts the audio.
                    video.OnStartPlaying(startingVideoTime, startingAudioTime);

                    // Start timer and video thread.
                    videoTimer.Start();
                    videoThread.Start(startingVideoTime);

                    isVideoPlaying = true;
                    isVideoPaused = false;
                    playButton.Text = "Pause";
                }
                // Pause the video
                else
                {
                    isVideoPaused = true;
                    StopVideoThreads();
                    playButton.Text = "Play";
                }
            }
        }

        private void OnStopButtonClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true)
            {
                StopVideoThreads();
                video.OnReset();
            }

            timelineBar.Value = 0;
            timeFrameLabel.Text = "00:00:00 / 0";

            isVideoPaused = false;
            isScrollPaused = false;

            this.Invalidate();
        }

        //
        // Scroll Bar Callbacks
        //
        private void OnTimelineScroll(object sender, EventArgs e)
        {
            if (isVideoPlaying == true && isVideoPaused == false)
            {
                // Pause
                StopVideoThreads();

                isScrollPaused = true;
                isVideoPaused = true;
                playButton.Text = "Play";
            }

            if (video != null)
            {
                video.OnSetCurrentFrame(timelineBar.Value);
            }
            renderTarget.Invalidate();

            UpdateTimelineLabel();
        }

        private void OnTimelineMouseUp(object sender, MouseEventArgs e)
        {
            if (isScrollPaused == true)
            {
                OnPlayButtonClick(sender, e);
                isScrollPaused = false;
            }
        }

        //
        // Helper Threading Functions
        //

        // Threaded function for playback.
        private void PlayVideo(object time)
        {
            int startTime = (int)time;
            float lastUpdateTime = 0.0f;
            while (true)
            {
                //
                // Update current frame.
                //

                // Get current time.
                float totalTime = 0.0f;
                lock (videoTimer)
                {
                    totalTime = (float)(videoTimer.ElapsedMilliseconds) / 1000.0f;

                    int newTime = (int)videoTimer.ElapsedMilliseconds + startTime;
                    if (newTime > timelineBar.Maximum)
                    {
                        //
                        // We've reached the end of the video
                        //

                        // Do some house keeping.
                        lock (video)
                        {
                            video.currentFrameID = video.totalFramesInVideo - 1;
                        }

                        timelineBar.Value = timelineBar.Maximum;
                        UpdateTimelineLabel();

                        // Then, bail.
                        OnStopButtonClick(this, null);
                        return;
                    }

                    timelineBar.Value = newTime;
                }

                float elapsedTime = totalTime - lastUpdateTime;
                lastUpdateTime = totalTime;

                bool result = false;
                lock (video)
                {
                    result = video.OnUpdate(elapsedTime);
                }

                // If the frame changed,
                if (result == true)
                {
                    // Tell GUI to redraw
                    renderTarget.Invalidate();
                }

                //
                // Update label.
                //
                UpdateTimelineLabel();
            }
        }

        private void UpdateTimelineLabel()
        {
            int seconds = (int)(timelineBar.Value / 1000.0f);
            int minutes = (int)(seconds / 60.0f);
            int hours = (int)(minutes / 60.0f);

            minutes -= (hours * 60);
            seconds -= (minutes * 60);

            string labelText = "";
            if (hours < 10)
            {
                labelText += "0";
            }
            labelText += hours.ToString() + ":";

            if (minutes < 10)
            {
                labelText += "0";
            }
            labelText += minutes.ToString() + ":";

            if (seconds < 10)
            {
                labelText += "0";
            }
            if (video != null)
            {
                labelText += seconds.ToString() + " / " + video.currentFrameID.ToString();
            }
            else
            {
                labelText += seconds.ToString() + " / 0";
            }
            timeFrameLabel.Text = labelText;
        }

        // Helper function to stop threads.
        private void StopVideoThreads()
        {
            videoTimer.Stop();
            videoTimer.Reset();

            if (isVideoPlaying == true)
            {
                videoThread.Abort();
                videoThread = null;
            }

            if (video != null)
            {
                video.OnStopAudio();
            }
            renderTarget.Invalidate();

            isVideoPlaying = false;
            playButton.Text = "Play";
        }

        //
        // No longer hooked up to the GUI
        //
        //private void OnPlayShotsButtonClick(object sender, EventArgs e)
        //{
        //    if (isVideoLoaded == true && isVideoSummarized == true)
        //    {
        //        StopVideoThreads();

        //        // No Threads at this point. So, we can just do stuff.
        //        video.ReadShots();
        //        videoTimer.Reset();
        //        videoTimer.Start();

        //        // Create and start thread to play
        //        shotsVideoThread = new Thread(PlayShots);
        //        shotsVideoThread.Start();

        //        isShotsPlaying = true;
        //        isVideoPaused = true;
        //        playButton.Text = "Pause";
        //    }
        //}

        //
        // No longer integrated into gui
        //
        // Threaded function for play shots.
        //private void PlayShots()
        //{
        //    float lastUpdateTime = 0.0f;
        //    while (true)
        //    {
        //        // Update current frame.
        //        float totalTime = 0.0f;
        //        lock (videoTimer)
        //        {
        //            totalTime = (float)videoTimer.ElapsedMilliseconds / 1000.0f;
        //        }

        //        float elapsedTime = totalTime - lastUpdateTime;
        //        lastUpdateTime = totalTime;

        //        Thread.Sleep(1);
        //        bool result = video.OnUpdate(elapsedTime);

        //        // Update label
        //        string labelText = "0:0:0 / ";

        //        Frame frame = video.GetCurrentFrame();
        //        labelText += frame.index.ToString();
        //        timeFrameLabel.Text = labelText;

        //        // Tell GUI to redraw if the frame changed.
        //        if (result == true)
        //        {
        //            renderTarget.Invalidate();
        //        }
        //    }
        //}
    }
}
