
//
// Abstraction for the visual data in a
// video.
//

// Comment this to debug video without having 
// to listen to audio.
#define AUDIO

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoPlayer
{
    class Video
    {
        private int TOTAL_FRAMES_IN_RAM = 72;
        private const int VIDEO_FPS = 24;
        private float SECONDS_PER_FRAME = 1.0f / (float)VIDEO_FPS;
        private const int VIDEO_DURATION = 10;      // We Need to find a way to compute video duration dynamically
        private int TOTAL_FRAMES_IN_VIDEO = (VIDEO_DURATION * 60) * VIDEO_FPS;

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

#if AUDIO
            // Set up the audio.
            result = audioPlayer.OnInitialize(audioFilePath);

            if (result == false)
                return false;
#endif

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
            
#if AUDIO
            audioPlayer.OnStop();
            audioPlayer.OnPlay(soundOffset);
#endif
        }

        public void OnReset()
        {
            currentFrame = 0;
            currentFrameTime = 0.0f;
            
            TOTAL_FRAMES_IN_RAM = 72;
            SECONDS_PER_FRAME = 1.0f / (float)VIDEO_FPS;

            lastReadFrame = TOTAL_FRAMES_IN_RAM - 1;
            currentFrameToReadAbsolute = TOTAL_FRAMES_IN_RAM;

            for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
            {
                Frame frame = frames[i];
                bool result = videoReader.ReadFrame(i, ref frame);
            }

#if AUDIO
            audioPlayer.OnStop();
#endif
        }

        public void ReadShots()
        {
            currentFrame = 0;
            currentFrameTime = 0.0f;

            TOTAL_FRAMES_IN_RAM = histogram.shots.Count;
            SECONDS_PER_FRAME = 1.0f;

            if (histogram.shots.Count > 0)
            {
                for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
                {
                    Frame frame = frames[i];
                    bool result = videoReader.ReadFrame(histogram.shots[i], ref frame);
                }
            }

#if AUDIO
            audioPlayer.OnStop();
#endif
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
            Frame frameA, frameB;

            histogram.OnInitialize();

            // Read the first frame
            frameA = new Frame(1, frameWidth, frameHeight);
            videoReader.ReadFrame(framesAnalyzedAbsolute, ref frameA);

            // Histogram Analysis on all frames in the video file
            while (framesAnalyzedAbsolute < TOTAL_FRAMES_IN_VIDEO-1)
            {
                frameB = new Frame(2, frameWidth, frameHeight);
                videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                sum = histogram.SumOfBinWiseDiff(ref frameA, ref frameB);

                ++framesAnalyzedAbsolute;

                // shift analysis window
                frameA = frameB;
                frameB = null;
            }

            if (histogram.framesMinWiseDifferences.Count == TOTAL_FRAMES_IN_VIDEO - 1)
            {
                histogram.GenerateCSVFile(histogram.MIN_WISE_DIFF);

                // Break video into shots
                histogram.FindShotTransitions();

                histogram.GenerateCSVFile(histogram.SHOTS);

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
