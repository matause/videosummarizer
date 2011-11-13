

//
// Abstraction for a video frame.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoPlayer
{
    class Frame
    {
        public int index, height, width;
        public int bytesPerFrame;
        public int bytesPerStride;
        public const int bytesPerPixel = 4;
        public List<List<Pixel>> pixels;

        public Frame(int index, int width, int height)
        {
            this.index = index;
            this.width = width;
            this.height = height;
            bytesPerStride = width * bytesPerPixel;
            bytesPerFrame = width * height * bytesPerPixel;

            // Instantiate the variable which will store this frame's pixels.
            pixels = new List<List<Pixel>>();

            for (int i = 0; i < height; ++i)
            {
                List<Pixel> row = new List<Pixel>();
                for (int j = 0; j < width; ++j)
                {
                    Pixel pixel = new Pixel();
                    row.Add(pixel);
                }
                pixels.Add(row);
            }
        }

        public void GetBytes( ref byte[] result )
        {
            int index = 0;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    result[ index++ ] = pixels[i][j].b;
                    result[ index++ ] = pixels[i][j].g;
                    result[ index++ ] = pixels[i][j].r;
                    result[ index++ ] = pixels[i][j].a;
                }
            }
        }
    }
}
