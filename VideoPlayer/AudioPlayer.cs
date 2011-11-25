

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
                    int offset = (int) Math.Floor(source.AudioFormat.BytesPerSecond * startTime);
                    long seekPosition = headerDataLength + offset;
                    reader.BaseStream.Seek( seekPosition, SeekOrigin.Begin );

                    // Read the frame
                    int readSize = (int)Math.Floor(source.AudioFormat.BytesPerSecond * (endTime - startTime));
                    result = reader.ReadBytes(readSize);
                }
                catch {}

                // Clean up
                if (reader != null)
                {
                    //reader.Close();
                }
            }

            return result;
        }
    }
}
