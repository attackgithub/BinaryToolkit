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
        /// BinaryAccess instance for the Process
        /// </summary>
        /// <param name="processId">Process ID</param>
        public BinaryAccess(int processId)
        {
            isFile = false;
            Process = Process.GetProcessById(processId);
        }

        /// <summary>
        /// BinaryAccess instance for the Process
        /// </summary>
        /// <param name="processInstance">Process instance/param>
        public BinaryAccess(Process processInstance)
        {
            isFile = false;
            Process = processInstance;
        }

        /// <summary>
        /// BinaryAccess instance for the File.
        /// </summary>
        /// <param name="file">FileInfo instance for file</param>
        /// <param name="BaseAddress">BaseAddress of the main module. You can leave IntPtr.Zero.</param>
        public BinaryAccess(FileInfo file, IntPtr BaseAddress)
        {
            isFile = true;
            File = file;
            FileBaseAddress = BaseAddress;
        }

        /// <summary>
        /// BinaryAccess instance for the File.
        /// </summary>
        /// <param name="file">FileInfo instance for file</param>
        public BinaryAccess(FileInfo file)
        {
            isFile = true;
            File = file;
        }

        /// <summary>
        /// BinaryAccess instance for file or process.
        /// Looks for Process firstly.
        /// </summary>
        /// <param name="FileOrProcessName">File or process name</param>
        public BinaryAccess(string FileOrProcessName)
        {
            var processes = Process.GetProcessesByName(FileOrProcessName);
            if (processes.Length >= 1)
            {
                isFile = false;
                Process = processes[0];
                return;
            }

            var file = new FileInfo(FileOrProcessName);
            if (file.Exists)
            {
                isFile = true;
                File = file;
                return;
            }

            throw new FileNotFoundException($"File or process {FileOrProcessName} is not found");
        }

        /// <summary>
        /// BinaryAccess instance for file or process.
        /// Looks for Process firstly.
        /// </summary>
        /// <param name="FileOrProcessName">File or process name</param>
        /// <param name="BaseAddress">(only for files) BaseAddress of the main module. You can leave IntPtr.Zero.</param>
        public BinaryAccess(string FileOrProcessName, IntPtr BaseAddress)
        {
            var processes = Process.GetProcessesByName(FileOrProcessName);
            if (processes.Length >= 1)
            {
                isFile = false;
                Process = processes[0];
                return;
            }

            var file = new FileInfo(FileOrProcessName);
            if (file.Exists)
            {
                isFile = true;
                File = file;
                FileBaseAddress = BaseAddress;
                return;
            }

            throw new FileNotFoundException($"File or process {FileOrProcessName} is not found");
        }


        public Module MainModule
        {
            get
            {
                CheckAlive();

                if (!IsFile)
                    return new Module(process.MainModule, this);
                else
                    return new Module(null, this);
            }
        }
        
        public Module[] Modules
        {
            get
            {
                //if (IsFile)
                //    throw new UncompatibleException("You can't get modules from File. Start this program, create instance of BinaryAccess for process and try it there. Or use MainModule instead.");
                if (IsFile)
                    return new Module[] { };

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

        public IntPtr FileBaseAddress = IntPtr.Zero;
        private FileStream fileStream = null;
        public FileStream FileStream
        {
            get
            {
                return fileStream;
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
                if (!IsFile)
                    throw new Exception("This BinaryAccess instance is for Processes only. Create another one.");

                // Close access to previous file
                Dispose();

                // Set another file
                file = value;

                // Open access to the file
                fileStream = file.Open(FileMode.Open, FileAccess.ReadWrite);

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
                    throw new Exception("This BinaryAccess instance is only for files. Please, create another one for process.");

                // Close access to previous process
                Dispose();
                
                // Open access to new process
                process = value;

                CheckAlive();
            }
        }

        /// <summary>
        /// Reads from the main module
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stringLen">(only for string) -1 to read to \0</param>
        /// <returns></returns>
        public T Read<T>(IntPtr Address, bool adrRelativeToTheMainModule = true, int stringLen = -1)
        {
            return MainModule.Read<T>(Address, adrRelativeToTheMainModule, stringLen);
        }

        /// <summary>
        /// Writes to the main module
        /// </summary>
        /// <param name="Object">Object</param>
        public void Write(IntPtr Address, object Object)
        {
            MainModule.Write(Address, Object);
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
            if (!IsFile)
            {
                if (process != null)
                    process.Dispose();
            }
            else
            {
                if (fileStream != null)
                    fileStream.Dispose();
            }
        }

        public object Invoke(IntPtr Address, object[] Arguments = null)
        {
            return MainModule.Invoke(Address, Arguments);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append("[");
            b.Append($"(IsFile:{IsFile})");
            b.Append($",(MainModule.Name:{MainModule.Name})");
            b.Append($",(MainModule.BaseAddress:0x{MainModule.BaseAddress.ToString("X4")})");
            b.Append("]");

            return b.ToString();
        }
    }
}
