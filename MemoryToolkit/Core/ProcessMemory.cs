using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using MemoryToolkit.Exceptions;

namespace MemoryToolkit
{
    public class ProcessMemory : IDisposable
    {
        public ProcessMemory(int processId)
        {
            Process = Process.GetProcessById(processId);
        }

        public ProcessMemory(Process processInstance)
        {
            Process = processInstance;
        }

        public Module MainModule
        {
            get
            {
                CheckAlive();

                return new Module(process.MainModule, process);
            }
        }
        
        public Module[] Modules
        {
            get
            {
                CheckAlive();

                List<Module> result = new List<Module>();

                foreach(ProcessModule module in process.Modules)
                {
                    result.Add(new Module(module, process));
                }

                return result.ToArray();
            }
        }
        
        private Process process = null;
        
        public Process Process
        {
            get
            {
                return process;
            }
            set
            {
                // Close access to previous process
                Dispose();

                // Open access to new process
                process = value;

                CheckAlive();
            }
        }

        /// <summary>
        /// Reads from main module
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="Address">Address</param>
        /// <param name="stringLen">(only for string) -1 to read to \0</param>
        /// <returns></returns>
        public T Read<T>(IntPtr Address, int stringLen = -1)
        {
            return MainModule.Read<T>(Address, stringLen);
        }

        public void CheckAlive()
        {
            if (process == null)
                throw new NullReferenceException("Invalid process");

            if (process.HasExited)
                throw new ProcessHasExitedException(process);
        }

        public void Dispose()
        {
            if(process != null)
                process.Dispose();
        }

        public object Invoke(IntPtr Address, object[] Arguments = null)
        {
            return MainModule.Invoke(Address, Arguments);
        }
    }
}
