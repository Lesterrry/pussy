using System;
using System.Media;
using System.Windows.Forms;
using Keystroke.API;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Permissions;
using Microsoft.Win32;

namespace pussy
{
    internal static class Program 
    {
        const string SAFEWORD = "pussy";
        static string givenSafeword = "";
        static bool isStarted = false;
        static string wallpaperPath;
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
                        if (!isStarted)
                        {
                            wallpaperPath = DesktopManagement.GetCurrentDesktopWallpaper();
                            isStarted = true;
                        }
                        char keyChar = char.Parse(key.KeyCode.ToString().ToLower());

                        if (keyChar == SAFEWORD[givenSafeword.Length])
                        {
                            givenSafeword += keyChar;
                            if (givenSafeword == SAFEWORD)
                            {
                                Wallpaper.Set(wallpaperPath);
                                Environment.Exit(0);
                            }
                        } else
                        {
                            givenSafeword = "";
                        }
                        try
                        {
                            string soundName = $"pussy.Sounds.{keyChar}.wav";
                            Stream soundStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(soundName);
                            player = new SoundPlayer(soundStream);
                            player.Play();
                            string wallpaperName = $"pussy.Wallpapers.{keyChar}.jpg";
                            Stream wallpaperStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(wallpaperName);
                            Wallpaper.Set(System.Drawing.Image.FromStream(wallpaperStream));
                        }
                        catch { }
                    }
                });
                Application.Run();
            }
            while (true) ;
        }

    }
    public sealed class Wallpaper
    {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uiAction, int uiParam, string pvParam, int fWinIni);

        public static void Set(System.Drawing.Image img)
        {
            InternalSet(img);
        }
        public static void Set(string filePath)
        {
            System.Drawing.Image img = System.Drawing.Image.FromFile(Path.GetFullPath(filePath));
            InternalSet(img);
        }
        private static void InternalSet(System.Drawing.Image img)
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            key.SetValue("WallpaperStyle", "10");
            key.SetValue("TileWallpaper", "0");
            SystemParametersInfo(
                SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
    public sealed class DesktopManagement
    {
        private const UInt32 SPI_GETDESKWALLPAPER = 0x73;
        private const int MAX_PATH = 260;
 
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(UInt32 uiAction, int uiParam, string pvParam, int fWinIni);
 
        public static string GetCurrentDesktopWallpaper()
        {
            string currentWallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, currentWallpaper.Length, currentWallpaper, 0);
            return currentWallpaper.Substring(0, currentWallpaper.IndexOf('\0'));
        }
    }

}
