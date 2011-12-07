

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Windows.Forms;
using SlimDX;

namespace VideoPlayer
{
    class _576vWriter
    {
        private _576vReader reader;
        private bool isInitialized;

        public _576vWriter()
        {
            reader = null;
            isInitialized = false;
        }

        public void OnInitialize(_576vReader reader)
        {
            this.reader = reader;
            isInitialized = true;
        }

        public bool WriteSummary(String audioFilePath, List<int> frames)
        {
            // Sanity
            if (isInitialized == false)
            {
                return false;
            }

            return WriteSummary(frames, audioFilePath);
        }

        private bool WriteSummary(List<int> frames, String filePath)
        {
            bool result = true;

            // Open the file to save.
            FileStream file = null;
            try
            {
                file = new FileStream(filePath, FileMode.OpenOrCreate);
            }
            catch
            {
                result = false;
            }

            if (result == true)
            {
                // Write all the frames into the video file.
                foreach (int frame in frames)
                {
                    result = WriteFrame(frame, file);

                    if (result == false)
                    {
                        break;
                    }
                }
            }

            // Clean up
            if (file != null)
            {
                file.Close();
            }

            return result;
        }

        private bool WriteFrame(int frameNumber, FileStream file)
        {
            bool result = true;

            // Read the current frame.
            Frame frame = new Frame(0, 320, 240); // Hard coded nastiness but it should be alright.
            result = reader.ReadFrame(frameNumber, ref frame);

            if (result == true)
            {
                // Allocate some data to store the frame.
                byte[] data = new byte[frame.bytesPerFrameInFile];

                int index = 0;

                // Read red channel.
                for (int i = 0; i < frame.width; ++i)
                {
                    for (int j = 0; j < frame.height; ++j)
                    {
                        Pixel color = frame.pixels[i][j];
                        data[index++] = color.r;
                    }
                }

                // Read green channel.
                for (int i = 0; i < frame.width; ++i)
                {
                    for (int j = 0; j < frame.height; ++j)
                    {
                        Pixel color = frame.pixels[i][j];
                        data[index++] = color.g;
                    }
                }

                // Read blue channel.
                for (int i = 0; i < frame.width; ++i)
                {
                    for (int j = 0; j < frame.height; ++j)
                    {
                        Pixel color = frame.pixels[i][j];
                        data[index++] = color.b;
                    }
                }

                file.Write(data, 0, frame.bytesPerFrameInFile);
            }

            return result;
        }
    }
}
