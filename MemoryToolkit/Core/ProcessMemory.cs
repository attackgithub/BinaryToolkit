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
        
        private Process process = null;
        //
        // <summary>
        // Selected process
        // </summary>
        //
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

                if (process.HasExited)
                    throw new ProcessHasExitedException(process);
            }
        }

        public void Dispose()
        {
            
        }
    }
}
