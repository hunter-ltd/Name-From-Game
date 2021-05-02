using System;
using System.IO;
using System.Diagnostics;
using WatcherLib;

namespace NameFromGame
{
    class Program
    {
        public static void Main(string[] args)
        {
            // default video folder
            string savePath = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE")!, "Videos"),
                movePath = Path.Combine(savePath, "Clips");
            if (args.Length > 0)
            {
                savePath = args[0];
                movePath = args[^1]; // args.Length - 1, not seen as bitwise XOR but instead syntactic sugar
            }
            
            if (args.Length > 2)
            {
                DisplayHelp();
                return;
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

            watcher.Start(); // Starts the watcher
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                // string name = User32Dll.GetActiveWindowTitle(80);
                // if (name != null)
                // {
                //    Console.Write($"\r{name.PadRight(80)}");
                // }
            }
            watcher.Stop();
            Trace.Close();
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("usage: nfg.exe [<save-path>] [<move-path>]");
            Console.WriteLine("  save path (optional)\t\tWhere you save your videos and where this program will watch");
            Console.WriteLine("  move path (optional)\t\tWhere the renamed files will be moved");
        }
    }
}
