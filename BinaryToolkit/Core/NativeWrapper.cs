using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryToolkit.Interop;
using BinaryToolkit.Extensions;

namespace BinaryToolkit
{
    public class NativeWrapper
    {
        /// <summary>
        /// Wrapper to Read from process or file.
        /// If it's a file, BinaryToolkit can't get BaseAdress, so it will never be relative to the main module.
        /// </summary>
        /// <param name="hProcess">Process Handle. Leave IntPtr.Zero if it's a file</param>
        /// <param name="lpBaseAddress">Where to start reading</param>
        public static bool Read(
                                IntPtr hProcess,
                                IntPtr lpBaseAddress,
                                out byte[] lpBuffer,
                                int dwSize,
                                out IntPtr lpNumberOfBytesRead,
                                FileStream fileStream = null)
        {
            if (fileStream == null)
                return kernel32.ReadProcessMemory(hProcess, lpBaseAddress, out lpBuffer, dwSize, out lpNumberOfBytesRead);
            else
            {
                List<byte> buffer = new List<byte>();

                long seek = fileStream.Seek(lpBaseAddress.ToInt64(), SeekOrigin.Begin);
                
                for (int i = 0; i < dwSize; i++)
                {
                    int b = fileStream.ReadByte();

                    if (b == -1) // EOF ?
                        break;

                    buffer.Add((byte)b);
                }

                lpNumberOfBytesRead = (IntPtr)fileStream.Position;
                lpBuffer = buffer.ToArray();

                return true;
            }
        }
    }
}
