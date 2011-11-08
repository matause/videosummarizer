


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
        public List<List<Color3>> pixels;

        public Frame(int index, int height, int width)
        {
            this.index = index;
            this.width = width;
            this.height = height;


        }
    }
}
