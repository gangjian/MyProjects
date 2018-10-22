using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SourceOutsight
{
	class SO_File
	{
		public string FullName = null;
		List<SO_CodeLineInfo> m_LineInfoList = new List<SO_CodeLineInfo>();
		public SO_File(string path)
		{
			Trace.Assert(!string.IsNullOrEmpty(path) && File.Exists(path));
			this.FullName = path;
			List<string> code_list = File.ReadAllLines(path).ToList();
			foreach (var item in code_list)
			{
				this.m_LineInfoList.Add(new SO_CodeLineInfo(item));
			}
			DoParse();
		}

		SO_IdentifierTable IdTable = new SO_IdentifierTable();
		void DoParse()
		{

		}
	}

	class SO_IdentifierTable
	{
		List<IdentifierTreeNode> IdTreeList = new List<IdentifierTreeNode>();
	}

	class IdentifierTreeNode
	{
		public string TextStr = null;
		public CodePosition Position = null;
		public CodeScope AffectScope = null;											// 作用域
		public IdentifierType Type = IdentifierType.Unknown;

		IdentifierTreeNode ParentRef = null;
		List<IdentifierTreeNode> ChildList = new List<IdentifierTreeNode>();
	}

	class CodePosition
	{
		public int Row = -1;
		public int Col = -1;
	}

	class CodeScope
	{
		public CodePosition Start = null;
		public CodePosition End = null;
	}

	enum IdentifierType
	{
		Unknown,
		PrecompileSwitch,
		IncludeHeader,
		MacroDef,
		MacroFunc,
		StructType,
		UnionType,
		MemberVar,
		Typedef,
		GlobalExtern,
		GlobalDef,
		FuncExtern,
		FuncDef,
	}

	class SO_CodeLineInfo
	{
		string TextStr = null;
		List<ContentsMark> MarkList = new List<ContentsMark>();

		public SO_CodeLineInfo(string code_line_str)
		{
			this.TextStr = code_line_str;
		}
	}

	class ContentsMark
	{
		ContentsType Type;
		int Offset = -1;
	}

	enum ContentsType
	{
		ReservedWord,
		Identifier,
		Symbol,
		Punctuation,
		Number,
		String,
		Char,
		Comments,
	}
}
