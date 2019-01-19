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
            Console.WriteLine("MemoryToolkitTester start");
            Console.WriteLine("Process: ");
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
