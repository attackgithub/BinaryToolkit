using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using BinaryToolkit.Extensions;
using BinaryToolkit.Exceptions;

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
                    return Parent.FileBaseAddress;
            }
        }

        public string Name
        {
            get
            {
                if (!Parent.IsFile)
                    return ProcessModule.ModuleName;
                else
                    return Parent.File.Name;
            }
        }

        public string FileName
        {
            get
            {
                if (!Parent.IsFile)
                    return ProcessModule.FileName;
                else
                    return Parent.File.FullName;
            }
        }

        public long ModuleMemorySize
        {
            get
            {
                if (!Parent.IsFile)
                    return ProcessModule.ModuleMemorySize;
                else
                    return Parent.File.Length;
            }
        }

        public IntPtr EntryPoint
        {
            get
            {
                if (!Parent.IsFile)
                    return ProcessModule.EntryPointAddress;
                else
                    return IntPtr.Zero; // fix me
            }
        }

        public IntPtr MainModuleAddress
        {
            get
            {
                return Parent.MainModule.BaseAddress;
            }
        }

        /// <summary>
        /// Return IntPtr.Zero if it's a file
        /// </summary>
        public IntPtr ProcessHandleSafe
        {
            get
            {
                if (Parent.IsFile)
                    return IntPtr.Zero;
                else
                    return Parent.AccessHandle;// Process.Handle;
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
        public T Invoke<T>(IntPtr Address, object[] Arguments = null)
        {
            if (Parent.IsFile)
                throw new UncompatibleException("You can't invoke methods from File. Start this program, create instance of BinaryAccess for process and invoke there.");

            if (Arguments == null)
                Arguments = new object[] { };

            return default(T);
        }
        
        public T Read<T>(IntPtr Address, bool adrRelativeToTheMainModule = true, int stringLen = -1, bool AutoAddress = true) 
        { 
            // Type of T
            Type type = typeof(T);
            
            if(AutoAddress)
                if ((!adrRelativeToTheMainModule && !Parent.IsFile) || (Parent.IsFile && adrRelativeToTheMainModule))
                    Address = Address.Decrement(BaseAddress);

            // If it's string, return ReadString method, but with new Address
            if (type == typeof(string))
                return (T)(object)ReadString(Address, stringLen);
            
            return NativeWrapper.Read<T>(ProcessHandleSafe, Address, Parent.FileStream);
        }

        public bool Write<T>(IntPtr Address, T Value, bool adrRelativeToTheMainModule = true, bool AutoAddress = true)
        {
            // Type of T
            Type type = typeof(T);

            if (AutoAddress)
                if ((!adrRelativeToTheMainModule && !Parent.IsFile) || (Parent.IsFile && adrRelativeToTheMainModule))
                    Address = Address.Decrement(BaseAddress);

            // If it's string, return ReadString method, but with new Address
            if (type == typeof(string))
                return WriteString(Address, (string)(object)Value);

            byte[] Buffer = NativeWrapper.TConvert(Value);
            int Size = Marshal.SizeOf(typeof(T));

            return NativeWrapper.Write(ProcessHandleSafe, Address, Buffer, Size, Parent.FileStream);
        }

        private bool WriteString(IntPtr Address, string Value)
        {
            for(int i = 0; i < Value.Length; i++)
            {
                Write(Address, Value[i], true, false);

                Address = Address.Increment(1);
            }

            Write(Address, '\0', true, false);

            return true;
        }

        private string ReadString(IntPtr Address, int stringLen = -1)
        {
            StringBuilder result = new StringBuilder();

            int i = 0;
            while (i != stringLen)
            {
                byte buffer = Read<byte>(Address, true, -1, false);

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
