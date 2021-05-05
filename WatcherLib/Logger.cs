using System;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace WatcherLib
{
    public class Logger
    {
        public Logger(string parentPath)
        {
            var logFile = new StreamWriter(Path.Combine(parentPath, "nameFromGame.log"), true);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            Trace.AutoFlush = true;
            Trace.WriteLine("[START] Name From Game Log on " + 
                            DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToShortTimeString());
        }

        /// <summary>
        /// Writes a message to the log file, optionally timestamped and tabbed
        /// </summary>
        /// <param name="message">Message to write</param>
        /// <param name="tabs">How far to indent the message (default is 0)</param>
        /// <param name="timeStamp">Whether or not to timestamp the message (default is true)</param>
        public void WriteNewMessage(string message, int tabs = 0, bool timeStamp = true)
        {
            var messageBuilder = new StringBuilder();
            
            if (timeStamp) 
                messageBuilder.Append(DateTime.Now.ToLongTimeString()).Append(' '); // prepend timestamp and a space
            for (var _ = 0; _ < tabs; _++) messageBuilder.Append('\t'); // add tabs (if any)
            
            messageBuilder.Append(message); // add message
            Trace.WriteLine(messageBuilder.ToString());
        }
    }
}