﻿
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
        private const int VIDEO_FPS = 24;

        private int totalFramesInRam = 72;
        private float secondsPerFrame = 1.0f / (float)VIDEO_FPS;

        // Metrics
        public int totalFramesInVideo;
        public float videoDuration; // in seconds

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

            totalFramesInVideo = -1;
            videoDuration = -1.0f;

            videoReader = new _576vReader();
            audioPlayer = new AudioPlayer();
            histogram = new Histogram();
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;

            frames = new List<Frame>();
            for (int i = 0; i < totalFramesInRam; ++i)
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

            result = BufferVideo(0);

            if (result == false)
                return false;

            // Compute some metrics.
            Frame metricFrame = frames[0]; // Just grabbing a frame. Anyone would do.
            long fileSize = videoReader.GetFileSize();
            totalFramesInVideo = (int)(fileSize / (long)(metricFrame.width * metricFrame.height * Frame.bytesPerPixelInFile));

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
            for (int i = 0; i < totalFramesInRam; ++i)
            {
                Frame frame = frames[i];
                bool result = videoReader.ReadFrame(i + startingFrame, ref frame);

                if (result == false)
                    return false;
            }

            lastReadFrame = totalFramesInRam - 1;
            currentFrameToReadAbsolute = totalFramesInRam + startingFrame;

            return true;
        }

        public void StreamNewFrames()
        {
            int nextFrameToRead = (lastReadFrame + 1) % totalFramesInRam;
            while (isSequenceMoreRecent(nextFrameToRead, currentFrame, totalFramesInRam) == true)
            {
                Frame frame = frames[nextFrameToRead];

                // I don't know how to handle if the reading fails.
                bool result = videoReader.ReadFrame(currentFrameToReadAbsolute, ref frame);

                lastReadFrame = nextFrameToRead;

                nextFrameToRead = (nextFrameToRead + 1) % totalFramesInRam;
                currentFrameToReadAbsolute++;
            }
        }

        public void OnStartPlaying(long startingVideoTime, long startingAudioTime)
        {
            currentFrameTime = 0.0f;
            currentFrame = 0;

            float elapsedTime = (float)startingVideoTime / 1000.0f;
            currentFrameTime += elapsedTime;

            int frameCount = 0;
            while (currentFrameTime >= secondsPerFrame)
            {
                currentFrameTime -= secondsPerFrame;
                frameCount++;
            }

            BufferVideo(frameCount);

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
            currentFrame = 0;
            currentFrameTime = 0.0f;

            // To play video after playing shots
            totalFramesInRam = 72;                              
            secondsPerFrame = 1.0f / (float)VIDEO_FPS;

            lastReadFrame = totalFramesInRam - 1;
            currentFrameToReadAbsolute = totalFramesInRam;

            for (int i = 0; i < totalFramesInRam; ++i)
            {
                Frame frame = frames[i];
                bool result = videoReader.ReadFrame(i, ref frame);
            }

#if AUDIO
            audioPlayer.OnStop();
#endif
        }

        public bool ReadShots()
        {
            bool result = false;

            if (histogram.shots.Count > 0)
            {
                currentFrame = 0;
                currentFrameTime = 0.0f;

                totalFramesInRam = histogram.shots.Count;
                secondsPerFrame = 1.0f;

                if (histogram.shots.Count > 0)
                {
                    for (int i = 0; i < totalFramesInRam; ++i)
                    {
                        Frame frame = frames[i];
                        videoReader.ReadFrame(histogram.shots[i], ref frame);
                    }
                }

#if AUDIO
                audioPlayer.OnStop();
#endif

                result = true;
            }

            return result;
        }

        // Returns true if the current frame updated.
        public bool OnUpdate(float elapsedTime)
        {
            bool result = false;

            currentFrameTime += elapsedTime;
            while( currentFrameTime >= secondsPerFrame )
            {
                currentFrame = (currentFrame + 1) % totalFramesInRam;
                currentFrameTime -= secondsPerFrame;
                
                result = true;
            }

            return result;
        }

        public Frame GetCurrentFrame()
        {
            Frame frame;

            if (currentFrame < totalFramesInRam)
            {
                frame = frames[currentFrame];
            }
            else
            {
                frame = frames[totalFramesInRam - 1];
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
            while (framesAnalyzedAbsolute < totalFramesInVideo - 1)
            {
                frameB = new Frame(2, frameWidth, frameHeight);
                videoReader.ReadFrame(framesAnalyzedAbsolute + 1, ref frameB);

                sum = histogram.SumOfBinWiseDiff(ref frameA, ref frameB);

                ++framesAnalyzedAbsolute;

                // shift analysis window
                frameA = frameB;
                frameB = null;
            }

            if (histogram.framesMinWiseDifferences.Count == totalFramesInVideo - 1)
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
