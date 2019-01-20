using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryToolkit
{
    public class OSVersion
    {
        public enum OS
        {
            Windows,
            NotWindows
        }

        public static OS GetOS()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return OS.ReactOS;
            else
                return OS.NotReactOS;
        }
    }
}
