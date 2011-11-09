

//
// Abstraction for a video frame.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SlimDX;

namespace VideoPlayer
{
    class Frame
    {
        public int index, height, width;
        public int bytesPerFrame;
        public List<List<Color3>> pixels;

        public Frame(int index, int width, int height)
        {
            this.index = index;
            this.width = width;
            this.height = height;
            bytesPerFrame = width * height * 3;

            // Instantiate the variable which will store this frame's pixels.
            pixels = new List<List<Color3>>();

            for (int i = 0; i < width; ++i)
            {
                List<Color3> row = new List<Color3>();
                for (int j = 0; j < height; ++j)
                {
                    Color3 pixel = new Color3();
                    row.Add(pixel);
                }
                pixels.Add(row);
            }
        }
    }
}
