
//
// Abstraction for the visual data in a
// video.
//

// Comment this to debug video without having 
// to listen to audio.
//#define AUDIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoPlayer
{
    class Video
    {
        private const int TOTAL_FRAMES_IN_RAM = 72;
        private const int VIDEO_FPS = 24;
        private const float SECONDS_PER_FRAME = 1.0f / (float)VIDEO_FPS;

        // Metrics
        private int totalFramesInVideo;
        private float videoDuration; // in seconds

        public int currentFrame;
        public int startingCachedFrame;
        public int lastReadFrame;
        public int currentFrameToReadAbsolute;
        public List<Frame> frames;

        private _576vReader videoReader;
        public AudioPlayer audioPlayer;

        private float currentFrameTime;

        // Analysis variables
        public int framesAnalyzedAbsolute;
        public List<int> framesMinWiseDifferences;
        public Histogram histogram;
        public int frameWidth;
        public int frameHeight;

        public Video(int frameWidth, int frameHeight)
        {
            currentFrame = 0;
            startingCachedFrame = 0;
            lastReadFrame = 0;
            currentFrameToReadAbsolute = 0;
            framesAnalyzedAbsolute = 0;
            currentFrameTime = 0.0f;

            totalFramesInVideo = -1;
            videoDuration = -1.0f;

            videoReader = new _576vReader();
            audioPlayer = new AudioPlayer();
            histogram = new Histogram();
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            frames = new List<Frame>();
            for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
            {
                frames.Add(new Frame(i, frameWidth, frameHeight));
            }
        }

        public bool OnInitialize(string videoFilePath, string audioFilePath)
        {
            bool result = true;

            currentFrame = 0;
            startingCachedFrame = 0;

            // Set up the video
            result = videoReader.OnInitialize(videoFilePath);

            if( result == false )
                return false;

            for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
            {
                Frame frame = frames[i];
                result = videoReader.ReadFrame(i, ref frame );
                
                if( result == false )
                    return false;
            }

            lastReadFrame = TOTAL_FRAMES_IN_RAM - 1;
            currentFrameToReadAbsolute = TOTAL_FRAMES_IN_RAM;

            // Compute some metrics.
            Frame metricFrame = frames[0]; // Just grabbing a frame. Anyone would do.
            long fileSize = videoReader.GetFileSize();
            totalFramesInVideo = (int)(fileSize / (long)(metricFrame.width * metricFrame.height * Frame.bytesPerPixelInFile));

            videoDuration = (float)totalFramesInVideo / (float)VIDEO_FPS;

            // Set up the audio.
            result = audioPlayer.OnInitialize(audioFilePath);

            if (result == false)
                return false;

            return true;
        }

        public void StreamNewFrames()
        {
            int nextFrameToRead = (lastReadFrame + 1) % TOTAL_FRAMES_IN_RAM;
            while ( isSequenceMoreRecent(nextFrameToRead, currentFrame, TOTAL_FRAMES_IN_RAM) == true )
            {
                Frame frame = frames[nextFrameToRead];

                // I don't know how to handle if the reading fails.
                bool result = videoReader.ReadFrame(currentFrameToReadAbsolute, ref frame);

                lastReadFrame = nextFrameToRead;

                nextFrameToRead = (nextFrameToRead + 1) % TOTAL_FRAMES_IN_RAM;
                currentFrameToReadAbsolute++;
            }
        }

        public void OnStartPlaying(int startingFrame, uint soundOffset)
        {
            currentFrame = startingFrame;
            currentFrameTime = 0.0f;
            audioPlayer.OnStop();

#if AUDIO
            audioPlayer.OnPlay(soundOffset);
#endif
        }

        public void OnReset()
        {
            currentFrame = 0;
            currentFrameTime = 0.0f;

            lastReadFrame = TOTAL_FRAMES_IN_RAM - 1;
            currentFrameToReadAbsolute = TOTAL_FRAMES_IN_RAM;

            for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
            {
                Frame frame = frames[i];
                bool result = videoReader.ReadFrame(i, ref frame);
            }

            audioPlayer.OnStop();
        }

        // Returns true if the current frame updated.
        public bool OnUpdate(float elapsedTime)
        {
            bool result = false;

            currentFrameTime += elapsedTime;
            while( currentFrameTime >= SECONDS_PER_FRAME )
            {
                currentFrame = (currentFrame + 1) % TOTAL_FRAMES_IN_RAM;
                currentFrameTime -= SECONDS_PER_FRAME;
                
                result = true;
            }

            return result;
        }

        public Frame GetCurrentFrame()
        {
            Frame frame;

            if (currentFrame < TOTAL_FRAMES_IN_RAM)
            {
                frame = frames[currentFrame];
            }
            else
            {
                frame = frames[TOTAL_FRAMES_IN_RAM - 1];
            }

            return frame;
        }

        public bool VideoAnalysis()
        {
            bool result = false;
            int sum = 0;

            framesMinWiseDifferences = new List<int>();
            Frame frameA, frameB;

            // Read the first frame
            frameA = new Frame(1, frameWidth, frameHeight);
            videoReader.ReadFrame(framesAnalyzedAbsolute, ref frameA);

            // Histogram Analysis on all frames in the video file
            while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
            {
                frameB = new Frame(2, frameWidth, frameHeight);
                videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                sum = histogram.SumOfBinWiseDiff(ref frameA, ref frameB);

                framesMinWiseDifferences.Add(sum);

                ++framesAnalyzedAbsolute;

                // shift analysis window
                frameA = frameB;
                frameB = null;
            }

            if (framesMinWiseDifferences.Count == totalFramesInVideo - 1)
            {
                histogram.GenerateCSVFile(framesMinWiseDifferences);
                result = true;
            }

            return result;
        }

        //
        // Helper Function
        //

        // Deals with wrap arounds
        private static bool isSequenceMoreRecent(int s1, int s2, int squenceMax)
        {
            return (s1 > s2) && (s1 - s2 <= squenceMax / 2) ||
                    (s2 > s1) && (s2 - s1 > squenceMax / 2);
        }
    }
}
