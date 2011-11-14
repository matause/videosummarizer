
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

        public int currentFrame, numberOfFrames;
        public int startingListFrame;
        public List<Frame> frames;

        private _576vReader videoReader;
        public AudioPlayer audioPlayer;

        private float currentFrameTime;

        public Video(int frameWidth, int frameHeight)
        {
            currentFrame = 0;
            numberOfFrames = 0;
            startingListFrame = 0;
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

            // Set up the audio.
            result = audioPlayer.OnInitialize(audioFilePath);

            if (result == false)
                return false;

            return true;
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
            audioPlayer.OnStop();
        }

        public void OnUpdate(float elapsedTime)
        {
            currentFrameTime += elapsedTime;
            while( currentFrameTime >= SECONDS_PER_FRAME )
            {
                currentFrame++;
                currentFrameTime -= SECONDS_PER_FRAME;
            }
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
    }
}
