using System;
using System.IO;

namespace WatcherLibrary
{
    /// <summary>
    /// A derived class of <see cref="FileSystemWatcher"/>.
    /// </summary>
    public class Watcher : FileSystemWatcher
    {
        /// <summary>
        /// Instantiates a <see cref="Watcher"/> with a given directory, watching for changes in all files.
        /// </summary>
        /// <param name="watchDirectory">Directory to watch</param>
        public Watcher(string watchDirectory, string moveToDirectory) : this(watchDirectory, moveToDirectory, string.Empty)
        { }

        /// <summary>
        /// Instantiates a <see cref="Watcher"/> with a given directory, watching for changes only in files that match the specified filter.
        /// </summary>
        /// <param name="watchDirectory">Directory to watch</param>
        /// <param name="filter">Filter</param>
        public Watcher(string watchDirectory, string moveToDirectory, string filter) : base(watchDirectory, filter)
        {
            NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Size;

            Changed += OnChanged;
            Created += OnCreated;
            Deleted += OnDeleted;
            Renamed += OnRenamed;
            Error += OnError;
        }

        /// <summary>
        /// Checks if a file is available
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>Boolean representing if the file is available for read/write operations</returns>
        public static bool IsFileAccessible(FileInfo file)
        {
            try
            {
                // Tries to open a read-only stream on the file.
                using (FileStream stream = file.OpenRead())
                {
                    stream.Close();
                }
            } catch (IOException)
            {
                // If the file is already open, it throws an IOException
                return false;
            }
            return true;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }
            var file = new FileInfo(e.FullPath);
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Changed: {file.FullName}");
            Console.WriteLine($"\tIs Available: {IsFileAccessible(file)}");
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            string value = $"[{DateTime.Now.ToLongTimeString()}] Created: {e.FullPath}";
            Console.WriteLine(value);
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e) =>
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Deleted: {e.FullPath}");

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Renamed:");
            Console.WriteLine($"\tOld: {e.OldFullPath}");
            Console.WriteLine($"\tNew: {e.FullPath}");
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
