using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryToolkit.Extensions;

namespace BinaryToolkit
{
    public class Module
    {
        public IntPtr BaseAddress
        {
            get
            {
                if (!Parent.IsFile)
                    return ProcessModule.BaseAddress;
                else
                    return IntPtr.Zero; // fixme
            }
        }

        public string Name
        {
            get
            {
                return ProcessModule.ModuleName;
            }
        }

        public string FileName
        {
            get
            {
                return ProcessModule.FileName;
            }
        }

        public int ModuleMemorySize
        {
            get
            {
                return ProcessModule.ModuleMemorySize;
            }
        }

        public IntPtr EntryPoint
        {
            get
            {
                return ProcessModule.EntryPointAddress;
            }
        }
        
        public IntPtr MainModuleAddress
        {
            get
            {
                return Process.MainModule.BaseAddress;
            }
        }

        public System.Diagnostics.ProcessModule ProcessModule = null;
        public System.Diagnostics.Process Process = null;
        public BinaryAccess Parent = null;

        public Module(System.Diagnostics.ProcessModule o, BinaryAccess parent)
        {
            Parent = parent;
            ProcessModule = o;
            Process = parent.Process;
        }

        // not implemented
        public object Invoke(IntPtr Address, object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            return null;
        }
        

        public T Read<T>(IntPtr Address, bool adrRelativeToTheMainModule = true, int stringLen = -1)
        { 
            Type type = typeof(T);
            
            if (!adrRelativeToTheMainModule)
                Address = BaseAddress.Increment(Address); // add BaseAddress to the Address

            if (type == typeof(string))
                return (T)(object)ReadString(Address, stringLen);

            int dwSize = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[dwSize];
            IntPtr bytesread;


            Interop.kernel32.ReadProcessMemory(Process.Handle, Address, buffer, dwSize, out bytesread);

            if (type == typeof(Int32))
            {
                return (T)(object)BitConverter.ToInt32(buffer, 0);
            }
            else if (type == typeof(Char))
            {
                return (T)(object)Convert.ToChar(buffer[0]);
                //return (T)(object)BitConverter.ToChar(buffer, 0);
            }
            else if (type == typeof(byte))
            {
                return (T)(object)buffer[0];
            }

            throw new NotImplementedException($"Type '{type.Name}' is not implemented in MemoryToolkit");
        }

        public string ReadString(IntPtr Address, int stringLen = -1)
        {
            StringBuilder result = new StringBuilder();

            int i = 0;
            while (i != stringLen)
            {
                byte buffer = Read<byte>(Address, true);

                if (buffer == 0)
                    break;

                result.Append(Convert.ToChar(buffer));

                Address = Address.Increment(1);
            }

            return result.ToString();
        }

        public override string ToString()
        {
            StringBuilder blr = new StringBuilder();

            blr.Append("[");
            blr.Append($"(BaseAddress:0x{BaseAddress.ToString("X4")})");
            blr.Append($",(EntryPoint:0x{EntryPoint.ToString("X4")})");
            blr.Append($",(Size:{ModuleMemorySize})");
            blr.Append($",(Name:{Name})");
            blr.Append("]");

            return blr.ToString();
        }
        
        //public int ReadInt32(IntPtr Address, uint dwAddress)
        //{
        //    byte[] buffer = new byte[4];
        //    int bytesread;

        //    Interop.kernel32.ReadProcessMemory(Address, dwAddress, buffer, 4, out bytesread);
        //    return BitConverter.ToInt32(buffer, 0);
        //}
    }
}
