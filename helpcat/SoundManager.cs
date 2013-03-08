using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;

namespace helpcat
{
    static class SoundManager
    {
        static SoundPlayer soundPlayer = new SoundPlayer();

        public static async void Play(string path)
        {
            if (File.Exists(path))
            {
                soundPlayer.Stop();
                soundPlayer.SoundLocation = path;
                await Task.Run((Action)soundPlayer.Load);
                soundPlayer.Play();
            }
        }
    }
}
