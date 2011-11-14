
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
        private const int TOTAL_FRAMES_IN_RAM = 72;
        private const int VIDEO_FPS = 24;
        private const float SECONDS_PER_FRAME = 1.0f / (float)VIDEO_FPS;

        public int currentFrame;
        public int startingCachedFrame;
        public int lastReadFrame;
        public int currentFrameToReadAbsolute;
        public List<Frame> frames;

        private _576vReader videoReader;
        public AudioPlayer audioPlayer;

        private float currentFrameTime;

        public Video(int frameWidth, int frameHeight)
        {
            currentFrame = 0;
            startingCachedFrame = 0;
            lastReadFrame = 0;
            currentFrameToReadAbsolute = 0;
            currentFrameTime = 0.0f;

            videoReader = new _576vReader();
            audioPlayer = new AudioPlayer();

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

        public void OnStartPlaying()
        {
            currentFrame = 0;
            currentFrameTime = 0.0f;
            audioPlayer.OnStop();

#if AUDIO
            audioPlayer.OnPlay();
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
