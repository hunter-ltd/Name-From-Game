using System;
using System.IO;
using System.Collections.Generic;

namespace WatcherLib
{
    public class RenameTable : Dictionary<string, string>
    {
        /// <summary>
        /// Creates a rename table from a given file
        /// </summary>
        /// <param name="tablePath">File path to the rename data</param>
        public RenameTable(string tablePath)
        {
            if (!File.Exists(tablePath))
            {
                File.CreateText(tablePath).Close();
            }

            string[] lines = File.ReadAllLines(tablePath);
            for (int i = 0; i < lines.Length; i+=2)
            {
                this.Add(lines[i], lines[i + 1]);
            }
        }

        public void WriteNewName(string name)
        {
            // TODO write the name then 2 newlines (e.g. $"{name}\n\n") to have the name then a blank line beneath
        }
    }
}