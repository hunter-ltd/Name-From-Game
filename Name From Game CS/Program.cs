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
                        savePath = Directory.Exists(args[0]) ? args[0] : null;
                        movePath = Directory.Exists(args[1]) ? args[1] : null;

                        if (string.IsNullOrEmpty(savePath))
                        {
                            Console.WriteLine($"'{savePath}' doesn't exist or couldn't be found. Please enter a valid directory");
                            return;
                        } else if(string.IsNullOrEmpty(movePath) || !Directory.Exists(movePath))
                        {
                            Console.Write($"'{movePath}' doesn't exist or couldn't be found. Would you like the program to create it for you? (y/N)");
                            switch (Console.ReadKey().Key)
                            {
                                case ConsoleKey.Y:
                                    try
                                    {
                                        Directory.CreateDirectory(movePath);
                                    } catch (Exception ex)
                                    {
                                        Console.WriteLine($"An error occurred: {ex.Message}");
                                        Console.WriteLine("Press anything to exit...");
                                        Console.ReadKey();
                                        return;
                                    }
                                    break;

                                default:
                                    return;
                            }
                        }
                        break;

                    default:
                        // display usage
                        break;
                }
            }
            var watcher = new Watcher(savePath, movePath);
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
