using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string cmd;
            while(true)
            {
                cmd = Console.ReadLine();
                if ("quit" == cmd)
                {
                    break;
                }
                else if ("show form" == cmd)
                {
                }
                else
                {
                    Console.WriteLine("Repeat: " + cmd);
                }
            }
        }
    }
}
