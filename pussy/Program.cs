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
                    if (key.KeyCode.ToString().Length == 1)
                    {
                        string fileName = $"pussy.Sounds.{key.KeyCode.ToString().ToLower()}.wav";
                        try
                        {
                            Stream fileStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName);
                            player = new SoundPlayer(fileStream);
                            player.Play();
                        }
                        catch { }
                    }
                });
                Application.Run();
            }
            while (true) ;
        }

    }
    
}
