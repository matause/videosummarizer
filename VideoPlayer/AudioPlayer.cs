

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
        SoundPlayer player;

        public AudioPlayer()
        {
            player = new SoundPlayer();
        }

        public bool OnInitialize(string filePath)
        {
            bool result = true;

            try
            {
                
            }
            catch
            {

            }

            return result;
        }
    }
}
