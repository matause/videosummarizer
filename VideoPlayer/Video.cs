
//
// Abstraction for the visual data in a
// video.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoPlayer
{
    class Video
    {
        private const int TOTAL_FRAMES_IN_RAM = 72;

        public int currentFrame, numberOfFrames;
        public int startingListFrame;
        public List<Frame> frames;

        _576vReader videoReader;

        public Video(int frameWidth, int frameHeight)
        {
            currentFrame = 0;
            numberOfFrames = 0;
            startingListFrame = 0;

            videoReader = new _576vReader();

            frames = new List<Frame>();
            for (int i = 0; i < TOTAL_FRAMES_IN_RAM; ++i)
            {
                frames.Add(new Frame(i, frameWidth, frameHeight));
            }
        }

        public bool OnInitialize(string videoFilePath)
        {
            bool result = true;

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

            return true;
        }
    }
}
