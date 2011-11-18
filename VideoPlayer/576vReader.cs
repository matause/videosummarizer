//
// A reader to read 576v files
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using SlimDX;

namespace VideoPlayer
{
    class _576vReader
    {
        private FileStream file;

        public bool OnInitialize(string filePath)
        {
            bool result = true;

            try
            {
                file = new FileStream(filePath, FileMode.Open);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public bool ReadFrame(int frameNumber, ref Frame frame)
        {
            bool result = true;

            try
            {
                long filePosition = (long)frameNumber * (long)frame.bytesPerFrameInFile;

                // Go to the start of the frame.
                file.Seek( filePosition, SeekOrigin.Begin );

                // Read in the frame.
                ReadFrame(ref frame);
            }
            catch
            {
                result = false;
            }

            return result;
        }

        public void OnClose()
        {
            if (file != null)
            {
                file.Close();
            }
        }

        private void ReadFrame(ref Frame frame)
        {
            byte[] data = new byte[frame.bytesPerFrameInFile];
            file.Read(data, 0, frame.bytesPerFrameInFile);

            int index = 0;

            // Read red channel.
            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    Pixel color = frame.pixels[i][j];
                    color.r = data[index++];
                }
            }

            // Read green channel.
            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    Pixel color = frame.pixels[i][j];
                    color.g = data[index++];
                }
            }

            // Read blue channel.
            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    Pixel color = frame.pixels[i][j];
                    color.b = data[index++];
                }
            }

            // Compute Y channel
            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j)
                {
                    Pixel color = frame.pixels[i][j];
                    double y = (0.299 * color.r) + (0.587 * color.g) + (0.114 * color.b);

                    color.y = (byte)(((y - Math.Floor(y / 1.0) * 1.0) > 0.5) ? Math.Ceiling(y) : Math.Floor(y));

                    frame.pixels[i][j] = color;
                }
            }
        }
    }
}
