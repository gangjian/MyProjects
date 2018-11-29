using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CodeCreeper
{
	class CodeFileInfo
	{
		string fullName = null;
		public string FullName
		{
			get { return fullName; }
		}
		List<string> codeList = null;
		public List<string> CodeList
		{
			get { return codeList; }
		}
		CodePosition currentPosition = null;
		public CodeFileInfo(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.fullName = path;
			this.codeList = File.ReadAllLines(path).ToList();
			this.currentPosition = GetFileStartPosition();
		}
		CodePosition GetFileStartPosition()
		{
			if (null == this.CodeList)
			{
				return null;
			}
			for (int i = 0; i < this.CodeList.Count; i++)
			{
				if (!string.IsNullOrEmpty(this.CodeList[i]))
				{
					return new CodePosition(i, 0);
				}
			}
			return null;
		}

		public CodeElement GetNextElement()
		{
			if (null == this.currentPosition)
			{
				return null;
			}
			while (true)
			{
				char ch = GetCurrentChar();
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/')
						 && CommProc.IsCommentStart(this.currentPosition, this.CodeList))
				{
					// 注释
					return CommentProc();
				}
				this.currentPosition = this.currentPosition.GetNextPosition(this.CodeList);
				if (null == this.currentPosition)
				{
					break;
				}
			}
			return null;
		}
		char GetCurrentChar()
		{
			return this.CodeList[this.currentPosition.Row][this.currentPosition.Col];
		}
		string GetCurrentLineString()
		{
			return this.CodeList[this.currentPosition.Row].Substring(this.currentPosition.Col);
		}
		CodeElement CommentProc()
		{
			string line_str = GetCurrentLineString();
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = this.currentPosition.Row;
				int end_col = this.CodeList[row].Length - 1;
				int len = end_col - this.currentPosition.Col + 1;
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, len);
				this.currentPosition = this.currentPosition.GetNextPositionN(this.CodeList, len - 1);
				return comment_element;
			}
			else if (line_str.StartsWith("/*"))
			{
				// 块注释
				CodePosition end_pos = CommProc.FindStrPosition(this.currentPosition, "*/", this.CodeList);
				Trace.Assert(null != end_pos);
				CodeElement comment_element = new CodeElement(ElementType.Comments, this.currentPosition, end_pos);
				this.currentPosition = end_pos.GetNextPosition(this.CodeList);
				return comment_element;
			}
			else
			{
				return null;
			}
		}
	}
}
