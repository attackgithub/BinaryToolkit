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

            if (args.Length == 0)
                args = new string[] { "speed.exe" };

            BinaryAccess mem = new BinaryAccess(args[0], (IntPtr)0x400000);
            WriteLine(mem.ToString(), ConsoleColor.DarkYellow);

            WriteLine("[Main Module] " + mem.MainModule.ToString());

            IntPtr adr = (IntPtr)0x8B26D4;
            WriteLine("[Read Char Test] 0x" + adr.ToString("X4"));

            for (int i = 0; i < 100; i++)
            {
                var result = mem.Read<Char>(adr);
                Write(result.ToString() + ",", ConsoleColor.Green);
                adr = adr.Increment(1);
            }

            WriteLine("");
            adr = (IntPtr)0x8B26D4;
            WriteLine("[Read String Test] 0x" + adr.ToString("X4"));
            
            var resultString = mem.Read<string>(adr);
            Write(resultString.ToString(), ConsoleColor.Green);
            WriteLine("");

            mem.Dispose();

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
