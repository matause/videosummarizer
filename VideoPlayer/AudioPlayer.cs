

//
// An abstraction for the audio playing component
// of the video.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IrrKlang;
using System.IO;
using System.Windows.Forms;

namespace VideoPlayer
{
    class AudioPlayer
    {
        public bool isSoundLoaded;

        private String soundFilePath;
        private FileStream fileStream;

        private ISoundEngine engine;
        private ISound sound;
        private ISoundSource source;

        float volume;

        public AudioPlayer()
        {
            volume = 1.0f;
            engine = new ISoundEngine();
            sound = null;
            source = null;

            soundFilePath = "";
            isSoundLoaded = false;
        }

        public bool OnInitialize(string filePath)
        {
            bool result = true;

            soundFilePath = filePath;

            // Try to open a file stream for the audio file.
            FileInfo info = new FileInfo(filePath);
            if (info.Exists == true)
            {
                fileStream = null;
                try
                {
                    fileStream = new FileStream(info.FullName, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            // Clean up on failure.
            if (result == false && fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }

            if (result == true)
            {
                source = engine.AddSoundSourceFromIOStream(fileStream, filePath);
                source.StreamMode = StreamMode.Streaming;
            }

            return result;
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="startingPosition">The time to start playback. (in milliseconds)</param>
        public void OnPlay(uint startingPosition)
        {
            try
            {
                sound = engine.Play2D(source, false, false, false);
                sound.PlayPosition = startingPosition;
                sound.Volume = volume;
                isSoundLoaded = true;
            }
            catch {}
        }

        public void OnSetVolume(float volume)
        {
            this.volume = volume;

            if (isSoundLoaded == true)
            {
                sound.Volume = volume;
            }
        }

        public void OnStop()
        {
            isSoundLoaded = false;
            try
            {
                engine.StopAllSounds();
            }
            catch {}
        }

        /// <summary>
        /// Returns the bytes of audio in a time window.
        /// </summary>
        /// <param name="startTime">Starting point in seconds.</param>
        /// <param name="duration">Length in seconds.</param>
        /// <returns></returns>
        public byte[] GetRawSoundData(float startTime, float endTime)
        {
            // Sanity
            if (source == null)
            {
                return null;
            }

            byte[] result = null;

            FileInfo info = new FileInfo(soundFilePath);
            if (fileStream != null)
            {
                // Create a binary reader and get the data.
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(fileStream);

                    int frameDataLength = source.AudioFormat.FrameCount * source.AudioFormat.FrameSize;
                    long headerDataLength = info.Length - (long)frameDataLength;

                    // Seek past the header and up to the current frame.
                    int offset = (int) Math.Floor((float)source.AudioFormat.BytesPerSecond * startTime);
                    if (offset % 2 != 0)
                    {
                        // We need to be always reading 2 bytes at a time.
                        offset++;
                    }

                    long seekPosition = headerDataLength + offset;
                    reader.BaseStream.Seek( seekPosition, SeekOrigin.Begin );

                    // Read the frame
                    int readSize = (int)Math.Ceiling((float)source.AudioFormat.BytesPerSecond * (endTime - startTime));
                    if (readSize % 2 != 0)
                    {
                        // Again, 2 bytes at a time;
                        readSize++;
                    }

                    result = reader.ReadBytes(readSize);
                }
                catch {}
            }

            return result;
        }

        public bool WriteSummary(String audioFilePath, List<int> frames)
        {
            // Sanity
            if (source == null)
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
            BinaryWriter writer = null;
            try
            {
                file = new FileStream(filePath, FileMode.Create);
                writer = new BinaryWriter(file);
            }
            catch
            {
                result = false;
            }

            if (result == true)
            {
                result = WriteHeader(writer, frames.Count);
            }

            if( result == true )
            {
                // Write all the frames into the audio file.
                int startFrame = 0;
                int endFrame = 0;
                for (int i = 0; i < frames.Count; ++i)
                {
                    // Conglomerate a group of consecutive frames.
                    startFrame = endFrame = frames[i];
                    endFrame++;
                    while (i + 1 < frames.Count && frames[i + 1] == endFrame)
                    {
                        ++endFrame;
                        ++i;
                    }

                    result = WriteBlockOfFrames(startFrame, endFrame, writer);
                    if (result == false)
                    {
                        break;
                    }
                }
            }

            // Clean up
            if (writer != null)
            {
                writer.Close();
            }

            if (file != null)
            {
                file.Close();
            }

            return result;
        }

        private bool WriteHeader(BinaryWriter writer, int numberOfFrames)
        {
            bool result = true;

            // Read and store the header
            FileInfo info = new FileInfo(soundFilePath);
            if (fileStream != null)
            {
                // Create a binary reader and get the data.
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(fileStream);
                    reader.BaseStream.Seek( 0, SeekOrigin.Begin );

                    // Determine length of the header
                    int frameDataLength = source.AudioFormat.FrameCount * source.AudioFormat.FrameSize;
                    long headerDataLength = info.Length - (long)frameDataLength;

                    // Read the header
                    byte[] data = reader.ReadBytes((int)headerDataLength);

                    // Write the header to the summary file.
                    writer.Write(data, 0, data.Length);

                    //
                    // We need to make some adjustments to the header to account for the new file.
                    //

                    //
                    // Rewrite chunk size.
                    //

                    //  chunkSize = (remainerOfHeader) + 
                    //              bytesPerFrame * 
                    //              numberOfFrames
                    int chunkSize = (int)(headerDataLength - 8) +
                        (int)Math.Floor((float)source.AudioFormat.BytesPerSecond * Video.secondsPerFrame) *
                        numberOfFrames;

                    // Seek to chunk and write it out.
                    writer.Seek(4, SeekOrigin.Begin);
                    writer.Write(chunkSize);

                    //
                    // Rewrite second chunk size.
                    //
                    
                    // chunkSize = bytesPerFrame * numberOfFrames
                    chunkSize = (int)Math.Floor((float)source.AudioFormat.BytesPerSecond * Video.secondsPerFrame) *
                        numberOfFrames;

                    // Seek to chunk and write it out.
                    writer.Seek((int)(headerDataLength - 4), SeekOrigin.Begin);
                    writer.Write(chunkSize);
                }
                catch 
                {
                    result = false;
                }
            }

            return result;
        }

        private bool WriteBlockOfFrames(int startFrame, int endFrame, BinaryWriter writer)
        {
            bool result = true;

            float startTime = startFrame * Video.secondsPerFrame;
            float endTime = endFrame * Video.secondsPerFrame;

            // Read the current frame.
            FileInfo info = new FileInfo(soundFilePath);
            if (fileStream != null)
            {
                // Create a binary reader and get the data.
                BinaryReader reader = null;
                try
                {
                    reader = new BinaryReader(fileStream);

                    int frameDataLength = source.AudioFormat.FrameCount * source.AudioFormat.FrameSize;
                    long headerDataLength = info.Length - (long)frameDataLength;

                    // Seek past the header and up to the current frame.
                    int offset = (int)Math.Floor((float)source.AudioFormat.BytesPerSecond * startTime);
                    if (offset % 2 != 0)
                    {
                        // We need to be always reading 2 bytes at a time.
                        offset++;
                    }

                    long seekPosition = headerDataLength + offset;
                    reader.BaseStream.Seek(seekPosition, SeekOrigin.Begin);

                    // Read the frame
                    int readSize = (int)Math.Ceiling((float)source.AudioFormat.BytesPerSecond * (endTime - startTime));
                    if (readSize % 2 != 0)
                    {
                        // Again, always 2 bytes at a time.
                        readSize++;
                    }

                    byte[] data = reader.ReadBytes(readSize);

                    // Write the frame to the summary file.
                    writer.Write(data, 0, data.Length);
                }
                catch 
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
