using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	[Serializable]
	public class MyException : ApplicationException
	{
		public MyException()
		{
		}

		public MyException(string message)
			: base(message)
		{
		}

		public MyException(string message, Exception inner_exception)
			: base(message, inner_exception)
		{
		}
	}
}
