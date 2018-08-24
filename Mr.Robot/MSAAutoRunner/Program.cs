using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mr.Robot.MacroSwitchAnalyser;
using System.IO;
using System.Diagnostics;

namespace MSAAutoRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			TestRunner test_runner = new TestRunner();
			test_runner.StartNextTest();
		}
	}
}
