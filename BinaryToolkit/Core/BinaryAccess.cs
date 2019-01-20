using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using BinaryToolkit.Exceptions;

namespace BinaryToolkit
{
    public class BinaryAccess : IDisposable
    {
        /// <summary>
        /// BinaryToolkit instance for Process
        /// </summary>
        /// <param name="processId">Process ID</param>
        public BinaryAccess(int processId)
        {
            isFile = false;
            Process = Process.GetProcessById(processId);
        }
        
        /// <summary>
        /// BinaryToolkit instance for Process
        /// </summary>
        /// <param name="processInstance">Process instance/param>
        public BinaryAccess(Process processInstance)
        {
            isFile = false;
            Process = processInstance;
        }

        /// <summary>
        /// BinaryToolkit instance for File.
        /// </summary>
        /// <param name="file">FileInfo instance for file</param>
        public BinaryAccess(FileInfo file)
        {
            isFile = true;
            File = file;
        }

        /// <summary>
        /// BinaryToolkit instance for file or process.
        /// Looks for Process firstly.
        /// </summary>
        /// <param name="FileOrProcessName">File or process name</param>
        public BinaryAccess(string FileOrProcessName)
        {
            var processes = Process.GetProcessesByName(FileOrProcessName);
            if(processes.Length >= 1)
            {
                isFile = false;
                Process = processes[0];
                return;
            }

            var file = new FileInfo(FileOrProcessName);
            if(file.Exists)
            {
                isFile = true;
                File = file;
                return;
            }

            throw new FileNotFoundException($"File or process {FileOrProcessName} is not found");
        }
        
        public Module MainModule
        {
            get
            {
                CheckAlive();

                return new Module(process.MainModule, this);
            }
        }
        
        public Module[] Modules
        {
            get
            {
                if (IsFile)
                    throw new Exception("BinaryToolkit can't get modules from file. Use MainModule as full file instead.");

                CheckAlive();

                List<Module> result = new List<Module>();

                foreach(ProcessModule module in process.Modules)
                {
                    result.Add(new Module(module, this));
                }

                return result.ToArray();
            }
        }
        private bool isFile = false;
        public bool IsFile
        {
            get
            {
                return isFile;
            }
        }

        private FileInfo file = null;
        public FileInfo File
        {
            get
            {
                return file;
            }
            set
            {
                // Close access to previous file
                Dispose();

                // Set another file
                file = value;

                CheckAlive();
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
                if (IsFile)
                    throw new Exception("This BinaryToolkit instance is only for files. Please, create another one for process.");

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
        public T Read<T>(IntPtr Address, bool adrRelativeToTheMainModule = true, int stringLen = -1)
        {
            return MainModule.Read<T>(Address, adrRelativeToTheMainModule, stringLen);
        }

        public void CheckAlive()
        {
            if (!IsFile)
            {
                if (process == null)
                    throw new NullReferenceException("Invalid process");

                if (process.HasExited)
                    throw new ProcessHasExitedException(process);
            }
            else
            {
                if (File == null)
                    throw new NullReferenceException("Invalid file");

                if (!File.Exists)
                    throw new FileNotFoundException($"File {File.FullName} is not found");
            }
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
