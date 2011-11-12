
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
            int bytesPerFrameInFile = frame.bytesPerFrame * 3 / 4;

            byte[] data = new byte[bytesPerFrameInFile];
            file.Read(data, 0, bytesPerFrameInFile);

            for (int i = 0; i < frame.height; ++i)
            {
                for (int j = 0; j < frame.width; ++j)
                {
                    Pixel color = new Pixel();
                    color.r = data[i + j * frame.width];
                    color.g = data[(i + j * frame.width) + frame.width * frame.height];
                    color.b = data[(i + j * frame.width) + frame.width * frame.height * 2];

                    if ((color.r != 0) || (color.g != 0) || (color.b != 0))
                        return;
                    
                    frame.pixels[i][j] = color;
                }
            }
        }
    }
}
