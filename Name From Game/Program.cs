using System;
using System.IO;
using System.Diagnostics;
using WatcherLib;

namespace NameFromGame
{
    class Program
    {
        static void Main(string[] args)
        {
            string savePath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Videos"),
                movePath = Path.Combine(savePath, "Clips");
            if (args.Length > 0)
            {
                savePath = args[0];
                switch (args.Length)
                {
                    case 1:
                        movePath = args[0];
                        break;

                    case 2:
                        movePath = args[1];
                        break;

                    default:
                        // display usage
                        break;
                }
            }
            if (!Directory.Exists(savePath))
            {
                Console.WriteLine($"'{savePath}' doesn't exist or couldn't be found. Falling back on default videos folder...");
            }

            var watcher = new Watcher(savePath, movePath);
            Console.WriteLine($"Path to watch: {savePath}");
            Console.WriteLine($"Path to move files to: {Path.Combine(movePath, "<game name>")}");
            Console.Write("Press anything to start...");
            Console.ReadKey();
            Console.WriteLine("\nClick into this window and press Esc at any time to stop the program.");

            watcher.EnableRaisingEvents = true; // Starts the watcher
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                // string name = User32Dll.GetActiveWindowTitle(80);
                // if (name != null)
                // {
                //    Console.Write($"\r{name.PadRight(80)}");
                // }
            }
            watcher.EnableRaisingEvents = false; // Stops the watcher
            watcher.Dispose();
            Trace.Close();
        }
    }
}
