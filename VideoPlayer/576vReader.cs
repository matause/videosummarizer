
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
                // Go to the start of the frame.
                file.Seek(frameNumber * frame.bytesPerFrame, SeekOrigin.Begin);

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
            file.Close();
        }

        private void ReadFrame(ref Frame frame)
        {
            byte[] data = new byte[frame.bytesPerFrame];
            file.Read(data, 0, frame.bytesPerFrame);

            for (int i = 0; i < frame.width; ++i)
            {
                for (int j = 0; j < frame.height; ++j )
                {
                    Color3 color = new Color3(
                        ((float)data[  i + j * frame.width ]) / 255.0f,
                        ((float)data[ (i + j * frame.width) + frame.width * frame.height ]) / 255.0f,
                        ((float)data[ (i + j * frame.width) + frame.width * frame.height * 2 ]) / 255.0f);

                    frame.pixels[i][j] = color;
                }
            }
        }
    }
}
