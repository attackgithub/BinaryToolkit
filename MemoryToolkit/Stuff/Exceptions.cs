using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BinaryToolkit.Exceptions
{
    public class ProcessHasExitedException : Exception
    {
        public System.Diagnostics.Process process = null;
        public ProcessHasExitedException(System.Diagnostics.Process Process)
        {
            process = Process;
        }
        public ProcessHasExitedException() { }

        public override string ToString()
        {
            if (process == null)
                return "Process has exited";
            else
                return $"Process {process.ProcessName} (pid: {process.Id}) has exited";
        }
    }
}
