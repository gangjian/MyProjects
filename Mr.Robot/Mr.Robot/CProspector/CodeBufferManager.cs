using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Diagnostics;

namespace Mr.Robot
{
	public class CodeBufferManager
	{
		List<CodeBuffer> BufferList = new List<CodeBuffer>();
		const int MAX_LEN = 500;
		System.Timers.Timer _timer = new System.Timers.Timer(10 * 1000);

		public CodeBufferManager()
		{
			this._timer.AutoReset = true;
			this._timer.Elapsed += _timer_Elapsed;
			this._timer.Start();
		}

		void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (this.BufferList)
			{
				this.BufferList.Sort(CodeBufferCompare);
				//if (0 != this.BufferList.Count)
				//{
				//	string str = string.Format("CodeBufferSort count = {0} TopTime = {1}, BottomTime = {2}",
				//								this.BufferList.Count,
				//								this.BufferList.First().AccessTime.ToString(),
				//								this.BufferList.Last().AccessTime.ToString());
				//	Console.WriteLine(str);
				//}
			}
		}

		public List<string> GetCodeList(string file_name)
		{
			for (int i = 0; i < this.BufferList.Count; i++)
			{
				if (this.BufferList[i].FileName.Equals(file_name))
				{
					//Console.WriteLine("GetCodeList at buffer index = " + i.ToString());
					return this.BufferList[i].GetCodeList();
				}
			}
			return AddNewBuffer(file_name);
		}

		List<string> AddNewBuffer(string file_name)
		{
			lock (this.BufferList)
			{
				while (this.BufferList.Count >= MAX_LEN)
				{
					this.BufferList.RemoveAt(this.BufferList.Count - 1);
				}
				CodeBuffer new_buf = new CodeBuffer(file_name);
				this.BufferList.Insert(0, new_buf);
				return new_buf.GetCodeList();
			}
		}

		int CodeBufferCompare(CodeBuffer buf_1, CodeBuffer buf_2)
		{
			if (buf_1.AccessTime > buf_2.AccessTime)
			{
				return -1;
			}
			else if (buf_1.AccessTime < buf_2.AccessTime)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

		public void Clear()
		{
			this._timer.Stop();
			this.BufferList.Clear();
		}
	}

	class CodeBuffer
	{
		public string FileName = null;
		List<string> CodeList = null;
		public DateTime AccessTime = new DateTime();

		public CodeBuffer(string file_name)
		{
			Trace.Assert(File.Exists(file_name));
			this.FileName = file_name;
			this.CodeList = COMN_PROC.RemoveComments(file_name);
		}

		public List<string> GetCodeList()
		{
			this.AccessTime = DateTime.Now;
			return this.CodeList;
		}
	}
}
