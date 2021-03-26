using System;
using System.IO;
using WatcherLibrary;
using WindowsTools;

namespace NameFromGame
{
    class Program
    {
        /// <summary>
        /// Creates a folder and returns the path of the folder it created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string CreateFolder(string path)
        {
        }

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

            if (!Directory.Exists(movePath))
            {
                Console.Write($"The specified path to move to, '{movePath}', doesn't exist or couldn't be found. Would you like the program to create it for you? (y/N): ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.Y:
                        try
                        {
                            Directory.CreateDirectory(movePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                            Console.WriteLine("Press anything to exit...");
                            Console.ReadKey();
                            return;
                        }
                        break;

                    default:
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
