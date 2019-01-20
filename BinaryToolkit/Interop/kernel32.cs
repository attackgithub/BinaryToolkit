using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryToolkit.Interop
{
    public class kernel32
    {
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }
        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01, x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }
        [DllImport("kernel32.dll")] //OpenProcess function
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheiritHandle, int dwProcessId);
        [DllImport("kernel32.dll")] //CloseHandle function
        public static extern Int32 CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    [Out] byte[] lpBuffer,
    int dwSize,
    out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]

        public static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
  IntPtr hProcess,
  IntPtr lpBaseAddress,
  byte[] lpBuffer,
  Int32 nSize,
  out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          [MarshalAs(UnmanagedType.AsAny)] object lpBuffer,
          int dwSize,
          out IntPtr lpNumberOfBytesWritten);

        public static T Rpm<T>(IntPtr pHandle, IntPtr lpBaseAddress) 
        {
            T[] buffer = new T[Marshal.SizeOf(typeof(T))];
            ReadProcessMemory(pHandle, lpBaseAddress, buffer, Marshal.SizeOf(typeof(T)), out var bytesread);
            return buffer.First(); // [0] would be faster, but First() is safer. E.g. of buffer[0] ?? default(T)
        }

        public static T Rpm<T>(IntPtr pHandle, IntPtr lpBaseAddress, List<int> offsets) 
        {
            IntPtr address = lpBaseAddress;

            var lastOffset = offsets.Last();
            offsets.RemoveAt(offsets.Count - 1);

            foreach (var offset in offsets)
            {
                address = Rpm<IntPtr>(pHandle, IntPtr.Add(address, offset));
            }

            return Rpm<T>(pHandle, IntPtr.Add(address, lastOffset));
        }

        public static bool Wpm<T>(IntPtr pHandle, IntPtr lpBaseAddress, T value) 
        {
            var buffer = new T[Marshal.SizeOf(typeof(T))];
            buffer[0] = value;
            return WriteProcessMemory(pHandle, lpBaseAddress, buffer, Marshal.SizeOf(typeof(T)), out var bytesread);
        }

        public static bool Wpm<T>(IntPtr pHandle, IntPtr lpBaseAddress, T value, List<int> offsets)
        {
            IntPtr address = lpBaseAddress;

            var lastOffset = offsets.Last();
            offsets.RemoveAt(offsets.Count - 1);

            foreach (var offset in offsets)
            {
                address = Rpm<IntPtr>(pHandle, IntPtr.Add(address, offset));
            }

            return Wpm<T>(pHandle, IntPtr.Add(address, lastOffset), value);
        }
    }
}
