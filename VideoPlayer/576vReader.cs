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

        public long GetFileSize()
        {
            long result = -1;

            if (file != null)
            {
                result = file.Length;
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
        }
    }
}
