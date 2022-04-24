using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Forms;
using System.Diagnostics;
using Keystroke.API;
using System.IO;
using System.Reflection;

namespace pussy
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var keyApi = new KeystrokeAPI())
            {
                SoundPlayer player;
                keyApi.CreateKeyboardHook((key) =>
                {
                    Stream fileStream = Assembly.GetEntryAssembly().GetManifestResourceStream("pussy.Sounds.roaches.wav");
                    player = new SoundPlayer(fileStream);
                    player.Play();
                    //Debug.Print(Assembly.GetExecutingAssembly().GetManifestResourceNames().First());
                });
                Application.Run();
            }
            while (true) ;
        }

    }
    
}
