using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
                    Form1 fm = new Form1();
                    fm.Show();
                }
                else
                {
                    Console.WriteLine("Repeat: " + cmd);
                }
            }
        }
    }
}
