using BinaryToolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BinaryToolkit.Extensions;

namespace MemoryToolkitTester
{
    class Program
    {
        static int Main(string[] args)
        {
            WriteLine("MemoryToolkitTester start");

            BinaryAccess mem = new BinaryAccess("speed");
            WriteLine(mem.Process.ToString());

            WriteLine("[Main Module] " + mem.MainModule.ToString());
            foreach (var module in mem.Modules)
            {
                WriteLine("[Module] " + module.ToString());
            }

            IntPtr adr = (IntPtr)0x8B26D4;
            WriteLine("[ReadChar Test] Started at: 0x" + adr.ToString("X4"));

            for (int i = 0; i < 20; i++)
            {
                var result = mem.Read<Char>(adr);
                Write(result.ToString() + ",", ConsoleColor.Green);
                adr = adr.Increment(1);
            }

            WriteLine("");
            WriteLine("[ReadChar Test] Finished at: 0x" + adr.ToString("X4"));
            WriteLine("");

            adr = (IntPtr)0x8B26D4;
            WriteLine("[Read String Test] Started at: 0x" + adr.ToString("X4"));

           
            var resultString = mem.Read<string>(adr);
            Write(resultString.ToString(), ConsoleColor.Green);

            WriteLine("");
            WriteLine("[Read String Test] Finished at: 0x" + adr.ToString("X4"));
            WriteLine("");

            mem.Dispose();
            WriteLine("[ProcessMemory] Disposed, ready to exit");
            WriteLine("");

            if (!Debugger.IsAttached)
            {
                WriteLine("Press any key to exit");
                Console.ReadKey();
            }
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
