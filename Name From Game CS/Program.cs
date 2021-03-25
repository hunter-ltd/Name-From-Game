using System;
using WatcherLibrary;
using WindowsTools;

namespace Name_From_Game_CS
{
    class Program
    {
        static void Main(string[] args)
        {
            var watcher = new Watcher(@"R:\");
            watcher.EnableRaisingEvents = true; // Starts the watcher
            string name;
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                name = User32Dll.GetActiveWindowTitle(80);
                if (name != null)
                {
                    Console.Write($"\r{name.PadRight(80)}");
                }
            }
            watcher.EnableRaisingEvents = false; // Stops the watcher
            watcher.Dispose();
        }
    }
}
