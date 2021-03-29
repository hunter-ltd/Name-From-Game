using System;
using System.IO;
using System.Diagnostics;

namespace WatcherLib
{
    public class Logger
    {
        public Logger(string parentPath)
        {
            StreamWriter logFile = new StreamWriter(Path.Combine(parentPath, "nameFromGame.log"), true);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile));
            Trace.AutoFlush = true;
            Trace.WriteLine("[START] Name From Game Log on " + DateTime.Now.ToLongDateString() + " at " + DateTime.Now.ToShortTimeString());
        }
    }
}