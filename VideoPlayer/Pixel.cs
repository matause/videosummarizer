
//
// Abstraction for a pixel.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoPlayer
{
    class Pixel
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public Pixel()
        {
            r = g = b = 0;
            a = 255;
        }
    }
}
