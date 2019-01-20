using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BinaryToolkit.Interop;

namespace BinaryToolkit
{
    public class NativeWrapper
    {
        public static bool Read(
                                IntPtr hProcess,
                                IntPtr lpBaseAddress,
                                byte[] lpBuffer,
                                int dwSize,
                                out IntPtr lpNumberOfBytesRead,
                                bool IsFile = false)
        {
            if (!IsFile)
                return kernel32.ReadProcessMemory(hProcess, lpBaseAddress, lpBuffer, dwSize, out lpNumberOfBytesRead);
            else
                throw new NotImplementedException();
        }
    }
}
