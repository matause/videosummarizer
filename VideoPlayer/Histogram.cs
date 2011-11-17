using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace VideoPlayer
{
    class Histogram
    {
        public void ComputeValues(ref Frame frame)
        {
            frame.values = new Int32[256];

            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    byte y = frame.pixels[i][j].y;
                    frame.values[y]++;
                }
            }
        }

        public void CreateHistogram(ref Frame frame)
        {
            // Compute the frequency of y values in the range 0-255
            ComputeValues(ref frame);

            // Instantiate 
            frame.histogram = new AForge.Math.Histogram(frame.values);
        }
    }
}
