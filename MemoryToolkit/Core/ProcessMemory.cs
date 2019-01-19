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

                return new Module(process.MainModule);
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
                    result.Add(new Module(module));
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
    }
}
