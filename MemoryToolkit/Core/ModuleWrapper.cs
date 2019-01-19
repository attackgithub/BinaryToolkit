using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MemoryToolkit
{
    public class Module
    {
        public IntPtr BaseAddress
        {
            get
            {
                return ProcessModule.BaseAddress;
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
        
        public System.Diagnostics.ProcessModule ProcessModule = null;

        public Module(System.Diagnostics.ProcessModule o)
        {
            ProcessModule = o;
        }

        // not implemented
        public object Invoke(IntPtr Address, object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            return null;
        }

        public T Read<T>(IntPtr Address)
        {
            if(typeof(T) == typeof(byte))
            {

            }
            else if (typeof(T) == typeof(int))
            {

            }

            throw new NotImplementedException($"Type '{typeof(T).Name}' is not implemented in MemoryToolkit");
        }
        
        public object Read(IntPtr Address, int size)
        {
            return null;
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
    }
}
