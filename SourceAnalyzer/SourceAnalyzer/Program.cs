using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int argCount = args.Length;
            if (0 != argCount)
            {
                string path = args[0];
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo fi = new FileInfo(path);
                if (di.Exists)
                {
                    Console.WriteLine(path + " is a valid path name.");
                }
                else if (fi.Exists)
                {
                    Console.WriteLine(path + " is a valid file name.");
                }
                else
                {
                    Console.WriteLine(path + " is a invalid string.");
                }
            }
            Console.ReadLine();
        }
    }
}
