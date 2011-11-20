
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
        bool isShotsPlaying;
        bool isVideoPaused;

        bool isVideoSummarized;     

        float totalVideoPlayTime;

        const int frameWidth = 320;
        const int frameHeight = 240;

        Color4 clearColor;
        
        //SplitterPanel renderTarget;
        Direct2DRenderer renderer;
        RenderForm renderTarget;

        Video video;
        Stopwatch videoTimer;

        Thread videoThread;
        Thread streamThread;
        Thread shotsVideoThread;

        public VPMainForm()
        {
            isVideoLoaded = false;
            isVideoPlaying = false;
            isShotsPlaying = false;
            isVideoPaused = false;

            isVideoSummarized = false;

            totalVideoPlayTime = 0.0f;

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
            summarizeButton.Click += new EventHandler(OnSummarizeButtonClick);
            timelineBar.Scroll += new EventHandler(OnTimelineScroll);
            playShotsButton.Click += new EventHandler(OnPlayShotsButtonClick);
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
                video.OnReset();
                video = null;
            }

            videoTimer.Stop();
            
            isVideoPlaying = false;
            isVideoLoaded = false;
            isShotsPlaying = false;

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
                renderTarget.Invalidate();
            }
        }

        //
        // Button Callbacks.
        //

        private void OnPlayButtonClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true )
            {
                // Normally start the video
                if (isVideoPlaying == false)
                {
                    StopVideoThreads();

                    // No Threads at this point. So, we can just do stuff.

                    long startingVideoTime = 0;
                    long startingAudioTime = 0;
                    if (isVideoPaused == true)
                    {
                        startingVideoTime = videoTimer.ElapsedMilliseconds;
                        startingAudioTime = videoTimer.ElapsedMilliseconds;
                    }
                    else
                    {
                        video.OnReset();
                        videoTimer.Reset();
                    }

                    // These need started together.
                    video.OnStartPlaying(startingVideoTime, startingAudioTime + 1100);
                    videoTimer.Start();

                    // Create and start new playing and streaming threads.
                    videoThread = new Thread(PlayVideo);
                    videoThread.Start();

                    streamThread = new Thread(StreamVideo);
                    streamThread.Start();

                    isVideoPlaying = true;
                    isVideoPaused = false;
                    playButton.Text = "Pause";
                }
                // Pause the video
                else
                {
                    StopVideoThreads();
                    video.OnStopAudio();
                    videoTimer.Stop();

                    isVideoPaused = true;
                    isVideoPlaying = false;
                    playButton.Text = "Play";
                }
            }
        }

        private void OnPlayShotsButtonClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true && isVideoSummarized == true)
            {
                StopVideoThreads();

                // No Threads at this point. So, we can just do stuff.
                video.ReadShots();
                videoTimer.Reset();

                videoTimer.Start();

                // Create and start thread to play
                shotsVideoThread = new Thread(PlayShots);
                shotsVideoThread.Start();

                isShotsPlaying = true;
                isVideoPaused = false;
                playButton.Text = "Pause";
            }
        }

        private void OnStopButtonClick(object sender, EventArgs e)
        {
            if (isVideoPlaying == true || isShotsPlaying == true)
            {
                StopVideoThreads();
                videoTimer.Stop();
  
                if (video != null)
                {
                    video.OnReset();
                }
                renderTarget.Invalidate();

                isVideoPlaying = false;
                isShotsPlaying = false;
                isVideoPaused = false;
                playButton.Text = "Play";
            }
        }

        private void OnSummarizeButtonClick(object sender, EventArgs e)
        {
            if (isVideoLoaded == true)
            {
                // Stop video player
                StopVideoThreads();
                video.OnReset();

                isVideoPlaying = false;
                isShotsPlaying = false;
                isVideoPaused = false;
                playButton.Text = "Play";

                // Compute the data for Shot detection analysis
                bool result = video.VideoAnalysis();
                
                if(result == true)
                    isVideoSummarized = true;
            }
        }

        //
        // Scroll Bar Callbacks
        //

        private void OnTimelineScroll(object sender, EventArgs e)
        {
            // TODO
        }

        //
        // Helper Threading Functions
        //

        
        // Threaded function for playback.
        private void PlayVideo()
        {
            totalVideoPlayTime = 0.0f;

            lock (video)
            {
                while(true)
                {
                    // Update current frame.
                    float totalTime = 0.0f;
                    lock (videoTimer)
                    {
                        totalTime = (float)videoTimer.ElapsedMilliseconds / 1000.0f;
                    }

                    float elapsedTime = totalTime - totalVideoPlayTime;
                    totalVideoPlayTime = totalTime;

                    Monitor.Wait(video);
                    bool result = video.OnUpdate(elapsedTime);
                    Monitor.Pulse(video);

                    // Tell GUI to redraw if the frame changed.
                    if (result == true)
                    {
                        renderTarget.Invalidate();
                    }
                }
            }
        }
        

        // Threaded function for play shots.
        private void PlayShots()
        {
            totalVideoPlayTime = 0.0f;

            while (true)
            {
                // Update current frame.
                float totalTime = 0.0f;
                lock (videoTimer)
                {
                    totalTime = (float)videoTimer.ElapsedMilliseconds / 1000.0f;
                }

                float elapsedTime = totalTime - totalVideoPlayTime;
                totalVideoPlayTime = totalTime;

                Thread.Sleep(1);
                bool result = video.OnUpdate(elapsedTime);

                // Tell GUI to redraw if the frame changed.
                if (result == true)
                {
                    renderTarget.Invalidate();
                }
            }
        }

        // Threaded function for stream reading.
        private void StreamVideo()
        {
            lock (video)
            {
				Monitor.Pulse(video);
				while( Monitor.Wait(video,1000) == true )
				{
                    video.StreamNewFrames();
                    Monitor.Pulse(video);
                }
            }
        }

        // Helper function to stop threads.
        private void StopVideoThreads()
        {
            if (isVideoPlaying == true)
            {
                streamThread.Abort();
                videoThread.Abort();
        
                streamThread = null;
                videoThread = null;

                isVideoPlaying = false;
            }

            if (isShotsPlaying == true)
            {
                shotsVideoThread.Abort();
                shotsVideoThread = null;

                isShotsPlaying = false;
            }
        }
    }
}
