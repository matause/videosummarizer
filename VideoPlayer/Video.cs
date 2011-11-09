
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
        public int currentFrame, numberOfFrames;
        public int startingListFrame;
        public List<Frame> frames;

        public Video()
        {
            currentFrame = 0;
            numberOfFrames = 0;
            startingListFrame = 0;
            frames = new List<Frame>();
        }
    }
}
