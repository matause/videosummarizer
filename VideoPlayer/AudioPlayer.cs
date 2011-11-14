

//
// An abstraction for the audio playing component
// of the video.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IrrKlang;

namespace VideoPlayer
{
    class AudioPlayer
    {
        public bool isSoundLoaded;
        private ISoundEngine engine;
        private string soundFileName;
        private ISound sound;

        float volume;

        public AudioPlayer()
        {
            volume = 1.0f;
            engine = new ISoundEngine();
            isSoundLoaded = false;
        }

        public bool OnInitialize(string filePath)
        {
            bool result = true;

            soundFileName = filePath;

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
                sound = engine.Play2D(soundFileName, false, false, StreamMode.AutoDetect, false);
                sound.PlayPosition = startingPosition;
                sound.Volume = volume;
                isSoundLoaded = true;
            }
            catch { }
        }

        public void OnSetVolume(float volume)
        {
            this.volume = volume;
        }

        public void OnStop()
        {
            try
            {
                engine.StopAllSounds();
            }
            catch {}
        }
    }
}
