

//
// Abstraction for a video frame.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AForge.Math;
using AForge.Imaging;

namespace VideoPlayer
{
    class Frame
    {
        public int index, height, width;
        public int bytesPerFrameInMemory;
        public int bytesPerFrameInFile;
        public int bytesPerStride;
        public const int bytesPerPixelInMemory = 4;
        public const int bytesPerPixelInFile = 3;
        public List<List<Pixel>> pixels;

        // Histogram variables
        public Int32[] values;
        public AForge.Math.Histogram histogram;

        public Frame(int index, int width, int height)
        {
            this.index = index;
            this.width = width;
            this.height = height;
            bytesPerStride = width * bytesPerPixelInMemory;
            bytesPerFrameInMemory = width * height * bytesPerPixelInMemory;
            bytesPerFrameInFile = width * height * bytesPerPixelInFile;

            // Instantiate the variable which will store this frame's pixels.
            pixels = new List<List<Pixel>>();

            for (int i = 0; i < width; ++i)
            {
                List<Pixel> row = new List<Pixel>();
                for (int j = 0; j < height; ++j)
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
                for (int j = 0; j < height; ++j)
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
