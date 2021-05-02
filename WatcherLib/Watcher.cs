﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsTools;

namespace WatcherLib
{
    /// <summary>
    /// A derived class of <see cref="FileSystemWatcher"/>.
    /// </summary>
    public class Watcher : FileSystemWatcher
    {
        /// <summary>
        /// Gets or sets the path to which files will be moved by the watcher
        /// </summary>
        private string MovePath { get; set; }
        
        private RenameTable RenameTable { get; }
        private Task MoveTask { get; set; }

        private readonly CancellationTokenSource _tokenSource = new();
        private volatile LinkedList<FileInfo> _fileInfos = new();

        /// <summary>
        /// Instantiates a <see cref="Watcher"/> with a given directory, watching for changes in all files.
        /// </summary>
        /// <param name="watchDirectory">Directory to watch</param>
        /// <param name="moveToDirectory">Directory to move files to</param>
        public Watcher(string watchDirectory, string moveToDirectory) : this(watchDirectory, moveToDirectory, string.Empty)
        { }

        /// <summary>
        /// Instantiates a <see cref="Watcher"/> with a given directory, watching for changes only in files that match the specified filter.
        /// </summary>
        /// <param name="watchDirectory">Directory to watch</param>
        /// <param name="moveToDirectory">Directory to move files to</param>
        /// <param name="filter">Filter</param>
        private Watcher(string watchDirectory, string moveToDirectory, string filter) : base(watchDirectory, filter)
        {
            MovePath = moveToDirectory;
            Logger _ = new(watchDirectory);

            NotifyFilter = NotifyFilters.Attributes
                           | NotifyFilters.CreationTime
                           | NotifyFilters.DirectoryName
                           | NotifyFilters.FileName
                           | NotifyFilters.LastAccess
                           | NotifyFilters.LastWrite
                           | NotifyFilters.Size;

            Changed += OnChanged;
            Error += OnError;

            RenameTable = new RenameTable(System.IO.Path.Combine(
                System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), 
                "names.rmap"));
        }

        /// <summary>
        /// Checks if a file is available
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>Boolean representing if the file is available for read/write operations</returns>
        private static bool IsFileAccessible(FileInfo file)
        {
            try
            {
                // Tries to open a read-only stream on the file.
                using var stream = file.OpenRead();
                stream.Close();
            } catch (IOException)
            {
                // If the file is already open, it throws an IOException
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if a file is a video file
        /// </summary>
        /// <param name="file">File to check</param>
        /// <returns>Boolean representing if the file is a video or not</returns>
        private static bool IsFileVideo(FileInfo file)
        {
            var videoExtensions = new[]
            {
                ".mp4",
                ".flv",
                ".mov",
                ".mkv",
                ".ts",
                ".m3u8",
                ".avi"
            }; // common video extensions
            return videoExtensions.Contains(file.Extension);
        }

        public void Start()
        {
            EnableRaisingEvents = true;
            
            MoveTask = Task.Run(() =>
            {
                    while (!_tokenSource.Token.IsCancellationRequested)
                    {
                        MoveFromQueue();
                        Thread.Sleep(1000); // allows time for the files to be moved
                    }
            }, _tokenSource.Token);
        }

        public void Stop()
        {
            try
            {
                EnableRaisingEvents = false;
                _tokenSource.Cancel();
                MoveTask.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                MoveTask.Dispose();
                Dispose();
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType != WatcherChangeTypes.Changed)
            {
                return;
            }

            var file = new FileInfo(e.FullPath);
            if (IsFileVideo(file)) _fileInfos.AddLast(file);
        }

        private void MoveFromQueue()
        {
            if (_fileInfos.Count == 0) return; // Do nothing if the LinkedList is empty

            var moved = new LinkedList<FileInfo>();
            foreach (var file in _fileInfos.Where(IsFileAccessible))
            {
                Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}] File being moved...");
                var windowTitle = Regex.Replace(User32Dll.GetActiveWindowTitle(80),
                    $@"[{new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars())}]",
                    "");
                Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}]\tWindow title: {windowTitle}");

                windowTitle = RenameTable.WriteNewEntry(windowTitle, windowTitle) ? windowTitle : RenameTable[windowTitle];

                var uniqueMovePath = System.IO.Path.Combine(MovePath, windowTitle);
                Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}]\tOld path: {file.FullName}");

                Directory.CreateDirectory(uniqueMovePath);
                uniqueMovePath = System.IO.Path.Combine(uniqueMovePath, windowTitle + " " + file.Name);
                Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}]\tNew path: {uniqueMovePath}");
                var moveSuccessMessage = string.Empty;
                try
                {
                    File.Move(file.FullName, uniqueMovePath);
                    moved.AddFirst(file); // prepending is faster if the next prop of the node is readonly
                    moveSuccessMessage = $"{file.Name} moved successfully!";
                }
                catch (IOException ex)
                {
                    moveSuccessMessage = $"{file.Name} move failed.";
                    Console.WriteLine(ex.Message);
                    Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}]\tERROR: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {moveSuccessMessage}");
                    Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}] {moveSuccessMessage}");
                    Trace.WriteLine("");
                }
            }

            foreach (var fileInfo in moved)
            {
                _fileInfos.Remove(fileInfo); // remove the files that have been moved from the queue
            }
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception? ex)
        {
            while (true)
            {
                if (ex == null) return;
                Console.WriteLine("A major error occurred with the file system watcher. Check the log in the watch directory for more details");
                Trace.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Message: {ex.Message}");
                Trace.WriteLine("Stacktrace:");
                Trace.WriteLine(ex.StackTrace);
                Trace.WriteLine("");
                ex = ex.InnerException;
            }
        }
    }
}
