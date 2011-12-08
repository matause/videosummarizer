
//
// Abstraction for the visual data in a
// video.
//

// Comment this to debug video without having 
// to listen to audio.
#define AUDIO

// Comment this to output CVS files
#define CVS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace VideoPlayer
{
    class Video
    {
        private const int VIDEO_FPS = 24;
        public const float secondsPerFrame = 1.0f / (float)VIDEO_FPS;

        // Metrics
        public int totalFramesInVideo;
        public float videoDuration; // in seconds

        public int currentFrameID;
        public Frame currentFrame;

        public int startingCachedFrame;

        public _576vReader videoReader;
        public AudioPlayer audioPlayer;

        private float currentFrameTime;

        // Analysis variables
        public int framesAnalyzedAbsolute;
        public Summarizer summarizer;
        public int frameWidth;
        public int frameHeight;

        public bool usePreProcessedData = true;

        public Video(int frameWidth, int frameHeight)
        {
            currentFrameID = 0;
            startingCachedFrame = 0;
            framesAnalyzedAbsolute = 0;
            currentFrameTime = 0.0f;

            totalFramesInVideo = -1;
            videoDuration = -1.0f;

            videoReader = new _576vReader();
            audioPlayer = new AudioPlayer();
            summarizer = new Summarizer();
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            currentFrame = new Frame(0, frameWidth, frameHeight);
        }

        public bool OnInitialize(string videoFilePath, string audioFilePath)
        {
            bool result = true;

            currentFrameID = 0;
            startingCachedFrame = 0;

            // Set up the video
            result = videoReader.OnInitialize(videoFilePath);

            if (result == false)
                return false;

            result = BufferVideo(currentFrameID);

            if (result == false)
                return false;

            // Compute some metrics.
            long fileSize = videoReader.GetFileSize();
            totalFramesInVideo = (int)(fileSize / (long)(currentFrame.width * currentFrame.height
                * Frame.bytesPerPixelInFile));

            videoDuration = (float)totalFramesInVideo / (float)VIDEO_FPS;

#if AUDIO
            // Set up the audio.
            result = audioPlayer.OnInitialize(audioFilePath);

            if (result == false)
                return false;

#endif

            return true;
        }

        private bool BufferVideo(int startingFrame)
        {

            bool result = videoReader.ReadFrame(startingFrame,
                ref currentFrame);

            if (result == false)
                return false;

            return true;
        }

        public void OnSetCurrentFrame(long startingVideoTime)
        {
            currentFrameTime = 0.0f;
            currentFrameID = 0;

            float elapsedTime = (float)startingVideoTime / 1000.0f;
            currentFrameTime += elapsedTime;

            while (currentFrameTime >= secondsPerFrame)
            {
                currentFrameTime -= secondsPerFrame;
                currentFrameID++;
            }

            BufferVideo(currentFrameID);
        }

        public void OnStartPlaying(long startingVideoTime, long startingAudioTime)
        {
            OnSetCurrentFrame(startingVideoTime);

#if AUDIO
            audioPlayer.OnStop();
            audioPlayer.OnPlay((uint)startingAudioTime);
#endif
        }

        public void OnStopAudio()
        {
#if AUDIO
            audioPlayer.OnStop();
#endif
        }

        public void OnReset()
        {
            currentFrameID = 0;
            currentFrameTime = 0.0f;

            videoReader.ReadFrame(currentFrameID,
                ref currentFrame);

#if AUDIO
            audioPlayer.OnStop();
#endif
        }

        public bool OnUpdate(float elapsedTime)
        {
            bool result = false;

            currentFrameTime += elapsedTime;
            while (currentFrameTime >= secondsPerFrame)
            {
                currentFrameID++;
                currentFrameTime -= secondsPerFrame;

                result = true;
            }

            // If the result is true, we have updated the currentFrameID.
            // So, we need to read in a new frame.
            if (result == true)
            {
                result = videoReader.ReadFrame(currentFrameID,
                    ref currentFrame);
            }

            return result;
        }

        public Frame GetCurrentFrame()
        {
            return currentFrame;
        }

        public bool VideoAnalysis(String videoFilePath, String audioFilePath, ShotSelectionAlgorithm sSAlg,
            int sceneTime, int summaryPercentage)
        {
            bool result = false;

            Frame frameA, frameB;
            summarizer.OnInitialize(this);

            // Read the first frame
            frameA = new Frame(framesAnalyzedAbsolute, frameWidth, frameHeight);
            videoReader.ReadFrame(framesAnalyzedAbsolute, ref frameA);

            if (sSAlg == ShotSelectionAlgorithm.HISTOGRAM)
            {
                // Histogram Analysis on all frames in the video file
                while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
                {
                    frameB = new Frame(framesAnalyzedAbsolute + 1, frameWidth, frameHeight);
                    videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                    summarizer.FillAnalysisDataHistogram(ref frameB, ref frameA, true);

                    ++framesAnalyzedAbsolute;

                    // shift analysis window
                    frameA = frameB;
                    frameB = null;
                }
            }
            else if (sSAlg == ShotSelectionAlgorithm.MOTION)
            {
                if (usePreProcessedData)
                {
                    summarizer.ReadMotionDataFromFile();

                    while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
                    {
                        frameB = new Frame(framesAnalyzedAbsolute + 1, frameWidth, frameHeight);
                        videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                        summarizer.FillAudiodataForMotion(ref frameB, ref frameA);

                        ++framesAnalyzedAbsolute;

                        // shift analysis pair
                        frameA = frameB;
                        frameB = null;
                    }
                }
                else
                {
                    // Motion vector Analysis between every frameStep frames in the video file
                    while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
                    {
                        frameB = new Frame(framesAnalyzedAbsolute + summarizer.frameStep, frameWidth, frameHeight);
                        videoReader.ReadFrame(framesAnalyzedAbsolute + summarizer.frameStep, ref frameB);

                        summarizer.ComputeMotionDataOnFly(ref frameB, ref frameA, true);

                        framesAnalyzedAbsolute += summarizer.frameStep;

                        // shift analysis pair
                        frameA = frameB;
                        frameB = null;
                    }

                    framesAnalyzedAbsolute = 0;

                    // Read the first frame again to begin audio analysis
                    frameA = new Frame(framesAnalyzedAbsolute, frameWidth, frameHeight);
                    videoReader.ReadFrame(framesAnalyzedAbsolute, ref frameA);

                    // Compute audio avgs for every consecutive pair
                    while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
                    {
                        frameB = new Frame(framesAnalyzedAbsolute + 1, frameWidth, frameHeight);
                        videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                        summarizer.FillAudiodataForMotion(ref frameB, ref frameA);

                        ++framesAnalyzedAbsolute;

                        // shift analysis pair
                        frameA = frameB;
                        frameB = null;
                    }
                }
            }

            if (framesAnalyzedAbsolute >= totalFramesInVideo - 1)
            {
#if CVS
                FolderBrowserDialog dlg = new FolderBrowserDialog();
                dlg.Description = "Select a directory to store video metric files.";
                dlg.ShowNewFolderButton = false;

                DialogResult dlgResult = dlg.ShowDialog();
                if (dlgResult == DialogResult.Cancel)
                {
                    return false;
                }

                String directory = dlg.SelectedPath;

                if (sSAlg == ShotSelectionAlgorithm.HISTOGRAM)
                {
                    summarizer.GenerateCSVFile(summarizer.MIN_WISE_DIFF, directory);
                }
                else if (sSAlg == ShotSelectionAlgorithm.MOTION && !usePreProcessedData)
                {
                    summarizer.GenerateCSVFile(summarizer.MOTION_VECTOR_BEST_MATCH, directory);
                }

                summarizer.GenerateCSVFile(summarizer.AVG_AUDIO_AMPS, directory);
#endif
                // Break video into shots
                summarizer.FindShotTransitions(sSAlg);
#if CVS
                summarizer.GenerateCSVFile(summarizer.SHOTS, directory);
#endif

                // Find Key-frames
                summarizer.FindKeyFrames();
#if CVS
                summarizer.GenerateCSVFile(summarizer.KEY_FRAMES, directory);
#endif

                // Summarize the video
                summarizer.GenerateSummaryVideo(sceneTime, summaryPercentage);
#if CVS
                summarizer.GenerateCSVFile(summarizer.VIDEO_SUMMARY, directory);
#endif

                // Write the summary to disk.
                _576vWriter writer = new _576vWriter();
                writer.OnInitialize(videoReader);
                result = writer.WriteSummary(videoFilePath, summarizer.summaryFrames);
                if (result == true)
                {
                    result = audioPlayer.WriteSummary(audioFilePath, summarizer.summaryFrames);
                }
            }

            return result;
        }

        //
        // No Longer integrated into the logic of the GUI.
        //

        //        public bool ReadShots()
        //        {
        //            bool result = false;

        //            if (summarizer.shots.Count > 0)
        //            {
        //                currentFrame = 0;
        //                currentFrameTime = 0.0f;

        //                if (summarizer.shots.Count > frames.Count)
        //                {
        //                    for (int i = frames.Count; i < summarizer.shots.Count; ++i)
        //                        frames.Add(new Frame(i, frameWidth, frameHeight));
        //                }

        //                totalFramesInRam = summarizer.shots.Count;
        //                secondsPerFrame = 1.0f;

        //                for (int i = 0; i < totalFramesInRam; ++i)
        //                {
        //                    Frame frame = frames[i];
        //                    videoReader.ReadFrame(summarizer.shots[i].startFrame, ref frame);
        //                    Shot s = summarizer.shots[i];
        //                    frame.index = s.startFrame;
        //                }

        //#if AUDIO
        //                audioPlayer.OnStop();
        //#endif

        //                result = true;
        //            }

        //            return result;
        //        }
    }
}
