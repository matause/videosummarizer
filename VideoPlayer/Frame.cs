

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

        public byte[] GetBytes()
        {
            byte[] result = new byte[bytesPerFrame];
            int resultIndex = 0;

            for (int i = 0; i < height; ++i)
            {
                for (int j = 0; j < width; ++j)
                {
                    result[ resultIndex] = pixels[i][j].b;
                    result[ resultIndex + 1] = pixels[i][j].g;
                    result[ resultIndex + 2] = pixels[i][j].r;
                    result[ resultIndex + 3] = pixels[i][j].a;

                    resultIndex += 4;

                    //if ((pixels[i][j].b != 0) || (pixels[i][j].g != 0) || (pixels[i][j].r != 0))
                    //    return result;
                }
            }

            return result;
        }
    }
}
