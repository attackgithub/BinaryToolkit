using MemoryToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace MemoryToolkitTester
{
    class Program
    {
        static int Main(string[] args)
        {
            WriteLine("MemoryToolkitTester start");

            var processes = Process.GetProcessesByName("speed");
            if(processes.Length == 0)
            {
                WriteLine("Process not found");
                return 1;
            }

            ProcessMemory mem = new ProcessMemory(processes[0]);
            WriteLine(mem.Process.ToString());

            WriteLine("[Main Module] " + mem.MainModule.ToString());
            foreach (var module in mem.Modules)
            {
                WriteLine("[Module] " + module.ToString());
            }

            mem.Dispose();
            return 0;
        }
        static void WriteLine(object text, ConsoleColor color = ConsoleColor.Black)
        {
            ConsoleColor old = Console.ForegroundColor;

            if (color != ConsoleColor.Black)
                Console.ForegroundColor = color;

            Console.WriteLine(text);

            Console.ForegroundColor = old;

            if (Debugger.IsAttached)
                Debugger.Log(0, Debugger.DefaultCategory, text + Environment.NewLine);
        }

        static void Write(object text, ConsoleColor color = ConsoleColor.Black)
        {
            ConsoleColor old = Console.ForegroundColor;

            if (color != ConsoleColor.Black)
                Console.ForegroundColor = color;

            Console.Write(text);

            Console.ForegroundColor = old;

            if (Debugger.IsAttached)
                Debugger.Log(0, Debugger.DefaultCategory, text.ToString());
        }
    }
}
