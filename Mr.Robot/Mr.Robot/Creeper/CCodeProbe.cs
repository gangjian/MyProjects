using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace Mr.Robot.Creeper
{
	public class CCodeProbe
	{
		// 源文件列表(入力)
		List<string> HeaderList = new List<string>();
		List<string> SourceList = new List<string>();

		//public EventHandler UpdateProgressHandler = null;

		// 源文件解析结果(处理)
		List<CodeFileProbeInfo> HeaderProbeResultList = new List<CodeFileProbeInfo>();
		List<CodeFileProbeInfo> SourceProbeResultList = new List<CodeFileProbeInfo>();

		public CCodeProbe(List<string> source_list, List<string> header_list)
		{
			Trace.Assert(null != source_list && null != header_list);
			this.SourceList = source_list;
			this.HeaderList = header_list;
		}

		public CCodeProbe(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && Directory.Exists(path));
			Common.GetCodeFileList(path, this.SourceList, this.HeaderList);
		}

		public void ProbeStart()
		{
			ClearResults();
			foreach (string src_name in this.SourceList)
			{
				CodeFileProbeInfo probe_info = new CodeFileProbeInfo(src_name);
				probe_info.GoProbe();
			}
		}

		void ClearResults()
		{
			this.HeaderProbeResultList.Clear();
			this.SourceProbeResultList.Clear();
		}
	}

	class CodeFileProbeInfo
	{
		string FileName = null;
		List<string> CodeLineList = null;
		List<CodeLineProbeInfo> LineProbeInfoList = null;
		public CodeFileProbeInfo(string file_name)
		{
			this.FileName = file_name;
			this.CodeLineList = File.ReadAllLines(file_name).ToList();
			this.LineProbeInfoList = new List<CodeLineProbeInfo>();
			for (int i = 0; i < this.CodeLineList.Count; i++)
			{
				this.LineProbeInfoList.Add(new CodeLineProbeInfo());
			}
		}

		CodePosition m_CurrentPosition = null;
		public void GoProbe()
		{
			this.m_CurrentPosition = new CodePosition(0, 0);
			while (true)
			{
				if (null == this.m_CurrentPosition)
				{
					break;
				}
				CodeElement element = GetNextElement();
				if (null == element)
				{
					break;
				}
				else if (Common.IsDefineStart(element.TextStr))
				{
					this.m_CurrentPosition = DefineProc(element);										// 宏定义
				}
			}
		}

		CodeElement GetNextElement()
		{
			CodePosition cur_pos = new CodePosition(this.m_CurrentPosition);
			while (true)
			{
				char ch = this.CodeLineList[cur_pos.RowNum][cur_pos.ColNum];
				if (char.IsWhiteSpace(ch))
				{
					// 白空格
				}
				else if (ch.Equals('/'))
				{
					// 注释
					cur_pos = CommentProc(cur_pos);
				}
				else if (ch.Equals('#'))
				{
					string line_str = this.CodeLineList[cur_pos.RowNum].Substring(cur_pos.ColNum);
					// 预编译命令
					if (line_str.StartsWith("#include"))
					{
						// 头文件包含
					}
					else if (line_str.StartsWith("#define"))
					{
						// 宏定义
						CodePosition end_pos = Common.GetNextPosN(this.CodeLineList, cur_pos, "#define".Length - 1);
						CodeElement ret_element
							= new CodeElement(CodeElementType.ReservedWord, cur_pos, end_pos, "#define");
						this.m_CurrentPosition = end_pos;
						return ret_element;
					}
					else if (Common.IsConditionalComilationStart(line_str))
					{
						// 条件编译
					}
				}
				// 标识符
				else if (Char.IsLetter(ch) || ch.Equals('_'))
				{
					string line_str = this.CodeLineList[cur_pos.RowNum].Substring(cur_pos.ColNum);
					int len = Common.GetIdentifierLength(line_str, 0);
					CodePosition end_pos = Common.GetNextPosN(this.CodeLineList, cur_pos, len - 1);
					CodeElement ret_element
						= new CodeElement(CodeElementType.Identifier, cur_pos, end_pos, line_str.Substring(0, len));
					this.m_CurrentPosition = end_pos;
					return ret_element;
				}
				// 字符串,字符
				else if (ch.Equals('"')
					|| ch.Equals('\''))
				{

				}
				// 数字
				// 运算符

				cur_pos = Common.GetNextPos(this.CodeLineList, cur_pos);
				if (null == cur_pos)
				{
					break;
				}
			}
			return null;
		}

		CodePosition CommentProc(CodePosition cur_pos)
		{
			string line_str = this.CodeLineList[cur_pos.RowNum].Substring(cur_pos.ColNum);
			if (line_str.StartsWith("//"))
			{
				// 行注释
				int row = cur_pos.RowNum;
				int col = this.CodeLineList[row].Length - 1;
				CodePosition end_pos = new CodePosition(row, col);
				CodeElement comment_element
								= new CodeElement(CodeElementType.LineComments, cur_pos, end_pos, null);
				this.LineProbeInfoList[row].AddElement(comment_element);
				cur_pos = end_pos;
			}
			else if (line_str.StartsWith("/*"))
			{
				// 块注释
				CodePosition end_pos = Common.FindStrPosition(this.CodeLineList, cur_pos, "*/");
				Trace.Assert(null != end_pos);
				for (int row = cur_pos.RowNum; row <= end_pos.RowNum; row++)
				{
					int start_col, end_col;
					if (row == cur_pos.RowNum)
					{
						start_col = cur_pos.ColNum;
					}
					else
					{
						start_col = 0;
					}
					if (row == end_pos.RowNum)
					{
						end_col = end_pos.ColNum;
					}
					else
					{
						end_col = this.CodeLineList[row].Length - 1;
					}
					CodePosition s_pos = new CodePosition(row, start_col);
					CodePosition e_pos = new CodePosition(row, end_col);
					CodeElement comment_element
									= new CodeElement(CodeElementType.BlockComments, s_pos, e_pos, null);
					this.LineProbeInfoList[row].AddElement(comment_element);
				}
				cur_pos = Common.GetNextPosN(this.CodeLineList, end_pos, 1);
			}
			return cur_pos;
		}

		CodePosition DefineProc(CodeElement def_element)
		{
			Trace.Assert(Common.IsDefineStart(def_element.TextStr));
			// 定位到"#define"后
			CodePosition search_pos = Common.GetNextPosN(this.CodeLineList,
											def_element.Scope.Start, "#define".Length);
			int row = def_element.Scope.Start.RowNum;
			List<CodeElement> symbol_list = GetDefineElementList(ref search_pos);
			return search_pos;
		}

		List<CodeElement> GetDefineElementList(ref CodePosition start_position)
		{
			List<CodeElement> ret_list = new List<CodeElement>();
			return ret_list;
		}
	}

	class CodeLineProbeInfo
	{
		public List<CodeElement> ElementsList = new List<CodeElement>();				// 元素列表
		public void AddElement(CodeElement element)
		{
			this.ElementsList.Add(element);
		}
	}
}
