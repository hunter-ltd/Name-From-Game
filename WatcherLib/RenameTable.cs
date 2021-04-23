using System.Collections.Generic;
using System.IO;

namespace WatcherLib
{
    public class RenameTable : Dictionary<string, string>
    {
        public string TablePath { get; }

        /// <summary>
        /// Creates a rename table from a given file
        /// </summary>
        /// <param name="tablePath">File path to the rename data</param>
        public RenameTable(string tablePath)
        {
            TablePath = tablePath;
            if (!File.Exists(TablePath))
            {
                File.CreateText(TablePath).Close();
            }

            string[] lines = File.ReadAllLines(TablePath);
            for (int i = 0; i < lines.Length; i+=2)
            {
                Add(lines[i], lines[i + 1]);
            }
        }

        /// <summary>
        /// Synchronously writes a single new entry into the rename table file
        /// </summary>
        /// <param name="name">Entry name parameter</param>
        public void WriteNewEntry(string name)
        {
            StreamWriter writer = new StreamWriter(TablePath, true); // Opens a new writer in append mode
            foreach (string line in new[] {name, "", ""})
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        /// <summary>
        /// Asynchronously writes a single new entry into the rename table file
        /// </summary>
        /// <param name="name">Entry name parameter</param>
        /// <returns></returns>
        public async void WriteNewEntryAsync(string name)
        {
            StreamWriter writer = new StreamWriter(TablePath, true); // Opens a new writer in append mode
            foreach (string line in new[] {name, "", ""})
            {
                await writer.WriteLineAsync(line);
            }
            writer.Close();
        }
    }
}