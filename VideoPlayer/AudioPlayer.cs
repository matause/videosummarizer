

//
// An abstraction for the audio playing component
// of the video.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Media;

namespace VideoPlayer
{
    class AudioPlayer
    {
        public bool isSoundLoaded;
        private SoundPlayer player;

        public AudioPlayer()
        {
            player = new SoundPlayer();
            isSoundLoaded = false;
        }

        public bool OnInitialize(string filePath)
        {
            bool result = true;

            StreamReader file = null; 

            try
            {
                file = new StreamReader(filePath);
            }
            catch
            {
                result = false;
            }

            if (result == true)
            {
                try
                {
                    player.Stream = file.BaseStream;
                    player.Load();
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public void OnPlay()
        {
            try
            {
                player.Stop();
                player.Play();
            }
            catch { }
        }

        public void OnStop()
        {
            try
            {
                player.Stop();
            }
            catch {}
        }
    }
}
