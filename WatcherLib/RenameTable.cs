using System;
using System.IO;
using System.Collections.Generic;

namespace WatcherLib
{
    public class RenameTable : Dictionary<string, string>
    {
        private string _tablePath;

        public string TablePath
        {
            get => _tablePath;
        }

        /// <summary>
        /// Creates a rename table from a given file
        /// </summary>
        /// <param name="tablePath">File path to the rename data</param>
        public RenameTable(string tablePath)
        {
            this._tablePath = tablePath;
            if (!File.Exists(TablePath))
            {
                File.CreateText(TablePath).Close();
            }

            string[] lines = File.ReadAllLines(TablePath);
            for (int i = 0; i < lines.Length; i+=2)
            {
                this.Add(lines[i], lines[i + 1]);
            }
        }

        /// <summary>
        /// Synchronously writes a single new entry into the rename table file
        /// </summary>
        /// <param name="name">Entry name parameter</param>
        public void WriteNewEntry(string name)
        {
            StreamWriter writer = new StreamWriter(TablePath, append: true);
            foreach (string line in new string[] {name, "", ""})
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
            StreamWriter writer = new StreamWriter(TablePath, append: true);
            foreach (string line in new string[] {name, "", ""})
            {
                await writer.WriteLineAsync(line);
            }
            writer.Close();
        }
    }
}