using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryToolkit.Interop;
using BinaryToolkit.Extensions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace BinaryToolkit
{
    public class NativeWrapper
    {
        public static T TConvert<T>(byte[] buffer)
        {
            Type type = typeof(T);

            if (type == typeof(byte))
                return (T)(object)buffer[0];
            else if (type == typeof(Int32))
                return (T)(object)BitConverter.ToInt32(buffer, 0);
            else if (type == typeof(char))
                return (T)(object)Convert.ToChar(buffer[0]);
            else
                throw new NotImplementedException($"Type {type.Name} is not implemented in BinaryToolkit");
        }

        public static byte[] TConvert(object Value)
        {
            if (Value == null)
                return new byte[] { 0 };

            Type type = Value.GetType();

            List<byte> Buffer = new List<byte>();

            if (type == typeof(byte))
                Buffer.Add((byte)Value);
            else
                Buffer.Add(Convert.ToByte(Value));

            return Buffer.ToArray();
        }

        public static bool Write(IntPtr ProcessHandle,
                                    IntPtr BaseAddress,
                                    byte[] Buffer,
                                    int Size,
                                    FileStream fileStream = null)
        {
            if (fileStream == null)
            {
                if (OSVersion.IsROS())
                {
                    return kernel32.WriteProcessMemory(ProcessHandle, BaseAddress, Buffer, Size, out var a);
                }
                else
                {

                }
            }
            else
            {
                long seek = fileStream.Seek(BaseAddress.ToInt64(), SeekOrigin.Begin);

                for (int i = 0; i < Size; i++)
                {
                    fileStream.WriteByte(Buffer[i]);
                }

                fileStream.Flush();

                return true;
            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Wrapper to Read from process or file.
        /// If it's a file, BinaryToolkit can't get BaseAdress, so it will never be relative to the main module.
        /// </summary>
        /// <param name="ProcessHandle">Process Handle. Leave IntPtr.Zero if it's a file</param>
        /// <param name="Address">Where to start reading</param>
        public static T Read<T>(IntPtr ProcessHandle, IntPtr Address, FileStream fileStream = null)
        { 
            if (fileStream == null)
            {
                if (OSVersion.IsROS())
                    return kernel32.Rpm<T>(ProcessHandle, Address);
                else
                    throw new NotImplementedException();
            }
            else
            {
                int Size = Marshal.SizeOf(typeof(T));
                List<byte> buffer = new List<byte>();

                long seek = fileStream.Seek(Address.ToInt64(), SeekOrigin.Begin);

                for (int i = 0; i < Size; i++)
                {
                    var b = fileStream.ReadByte();

                    if (b == -1)
                        break; // EOF?

                    buffer.Add((byte)b);
                }

                return TConvert<T>(buffer.ToArray());
            }
        }
    }
}
