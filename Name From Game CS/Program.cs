using System;
using System.IO;
using WatcherLibrary;

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
                // use current directory
                savePath = @".\";
                movePath = @".\~Clips";
            } else
            {
                switch (args.Length)
                {
                    case 1:
                        savePath = args[0];
                        movePath = Path.Combine(savePath, "~Clips");
                        break;

                    case 2:
                        savePath = args[0];
                        movePath = args[1];
                        break;

                    default:
                        // display usage
                        break;
                }
            }
            if (!Directory.Exists(savePath))
            {
                Console.WriteLine($"'{savePath}' doesn't exist or couldn't be found. Please enter a valid directory");
                return;
            }

            var watcher = new Watcher(savePath, movePath);
            watcher.EnableRaisingEvents = true; // Starts the watcher
            string name;
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                //name = User32Dll.GetActiveWindowTitle(80);
                //if (name != null)
                //{
                //    //Console.Write($"\r{name.PadRight(80)}");
                //}
            }
            watcher.EnableRaisingEvents = false; // Stops the watcher
            watcher.Dispose();
        }
    }
}
