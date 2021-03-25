using System;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsTools
{
    public class User32Dll
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern int GetWindowTextA(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Gets the title of the currently active foreground window (a.k.a. focused window).
        /// </summary>
        /// <param name="maxLength">Maximum number of characters saved from the title</param>
        /// <returns>Title of active window as a string that is <c>maxLength</c> characters long</returns>
        public static string GetActiveWindowTitle(int maxLength)
        {
            StringBuilder buffer = new StringBuilder(maxLength);
            IntPtr windowHandle = GetForegroundWindow();

            if (GetWindowTextA(windowHandle, buffer, maxLength) > 0)
            {
                return buffer.ToString();
            }

            return null;
        }
    }
}
