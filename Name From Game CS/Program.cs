using System;
using System.IO;
using WatcherLibrary;
using WindowsTools;

namespace NameFromGame
{
    class Program
    {
        static void Main(string[] args)
        {
            string savePath = string.Empty,
                movePath = string.Empty;
            if (args.Length == 0)
            {
                savePath = @".\";
                movePath = @".\~Clips";
                // use current directory
            } else
            {
                switch (args.Length)
                {
                    case 1:
                        if (Directory.Exists(args[0]))
                        {
                            savePath = args[0];
                            movePath = Path.Combine(savePath, "~Clips");
                        }
                        break;

                    case 2:
                    // both directories given

                    default:
                        // display usage
                        break;
                }
            }
            var watcher = new Watcher(savePath);
            watcher.EnableRaisingEvents = true; // Starts the watcher
            string name;
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                name = User32Dll.GetActiveWindowTitle(80);
                if (name != null)
                {
                    //Console.Write($"\r{name.PadRight(80)}");
                }
            }
            watcher.EnableRaisingEvents = false; // Stops the watcher
            watcher.Dispose();
        }
    }
}
