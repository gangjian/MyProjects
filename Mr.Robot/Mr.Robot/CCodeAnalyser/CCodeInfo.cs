using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Mr.Robot
{
	// 条件编译处理情报
	class CONDITIONAL_COMPILATION_INFO
	{
		public string Exp = string.Empty;
		public bool UnidentifiedFlag = false;
		public bool WriteFlag = true;
		public bool WriteNextFlag = false;
		public bool PopUpStack = false;
	}

	// 文件当中某个内容的位置(行列号)
	public class CODE_POSITION
	{
		public int RowNum = 0;    // 行号
		public int ColNum = 0;    // 列号

		public CODE_POSITION(int r, int c)
		{
			this.RowNum = r;
			this.ColNum = c;
		}

		public CODE_POSITION(CODE_POSITION nfp)
		{
			if (null != nfp)
			{
				this.RowNum = nfp.RowNum;
				this.ColNum = nfp.ColNum;
			}
		}

		public void Move2HeadOfNextRow()
		{
			this.RowNum += 1;
			this.ColNum = 0;
		}
	}

	public class CODE_SCOPE
	{
		public CODE_POSITION Start = new CODE_POSITION(-1, -1);
		public CODE_POSITION End = new CODE_POSITION(-1, -1);

		public CODE_SCOPE(CODE_POSITION start_pos, CODE_POSITION end_pos)
		{
			this.Start = new CODE_POSITION(start_pos);
			this.End = new CODE_POSITION(end_pos);
		}
	}

	public enum E_CHAR_TYPE
	{
		E_CTYPE_WHITE_SPACE,
		E_CTYPE_LETTER,
		E_CTYPE_DIGIT,
		E_CTYPE_UNDERLINE,
		E_CTYPE_PUNCTUATION,
		E_CTYPE_SYMBOL,
		E_CTYPE_UNKNOWN
	}

	/// <summary>
	/// C源代码文件解析结果情报类
	/// </summary>
	public class FILE_PARSE_INFO
	{
		public string SourceName = string.Empty;
		public List<string> IncFileList = new List<string>();									// "include"头文件名列表
		public List<FUNCTION_PARSE_INFO> FuncDeclareList = new List<FUNCTION_PARSE_INFO>();		// 函数声明列表
		public List<FUNCTION_PARSE_INFO> FunDefineList = new List<FUNCTION_PARSE_INFO>();		// 函数定义列表
		public List<USER_DEFINE_TYPE_INFO> UsrDefTypeList = new List<USER_DEFINE_TYPE_INFO>();	// 用户定义类型列表
		public List<VAR_CONTEXT> GlobalDeclareList = new List<VAR_CONTEXT>();					// 全局量声明列表
		public List<VAR_CONTEXT> GlobalDefineList = new List<VAR_CONTEXT>();					// 全局量定义列表
		public List<MACRO_DEFINE_INFO> MacroDefineList = new List<MACRO_DEFINE_INFO>();			// 宏定义列表
		public List<TYPE_DEFINE_INFO> TypeDefineList = new List<TYPE_DEFINE_INFO>();			// typedef类型定义列表

		public List<string> CodeList = new List<string>();										// 解析后(去除注释, 宏展开等)的代码行内容列表

		// MT预编译宏开关值提取追加
		public List<string> MacroSwitchList = new List<string>();

		public FILE_PARSE_INFO(string fileName)
		{
			System.Diagnostics.Trace.Assert(File.Exists(fileName));
			this.SourceName = fileName;
		}

		public USER_DEFINE_TYPE_INFO FindUsrDefTypeInfo(string type_name, string category_name)
		{
			foreach (USER_DEFINE_TYPE_INFO udti in this.UsrDefTypeList)
			{
				foreach (CODE_IDENTIFIER nameIdtf in udti.NameList)
				{
					if (nameIdtf.Text.Equals(type_name))
					{
						if (string.Empty != category_name)
						{
							if (udti.Category.Equals(category_name))
							{
								return udti;
							}
						}
						else
						{
							return udti;
						}
					}
				}
			}
			return null;
		}

		public TYPE_DEFINE_INFO FindTypeDefInfo(string type_name)
		{
			foreach (TYPE_DEFINE_INFO tdi in this.TypeDefineList)
			{
				if (tdi.NewName.Equals(type_name))
				{
					return tdi;
				}
			}
			return null;
		}

		/// <summary>
		/// 如果类型名经过typedef重命名的话,取得原来的类型名
		/// </summary>
		public string GetOriginalTypeName(string type_name)
		{
			TYPE_DEFINE_INFO tdi = null;													// 如果经过typedef重命名的话, 找出原来的类型名
			while (null != (tdi = this.FindTypeDefInfo(type_name)))
			{
				type_name = tdi.OldName;
			}
			return type_name;
		}

		#region 以下方法,是针对代码解析结果的各种操作(查找,判断...)
		/// <summary>
		/// 根据函数名查找函数的解析结果
		/// </summary>
		public FUNCTION_PARSE_INFO FindFuncParseInfo(string fun_name)
		{
			FUNCTION_PARSE_INFO retFuncInfo = null;
			if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, this.FuncDeclareList)))
			{
				return retFuncInfo;
			}
			if (null != (retFuncInfo = SearchFuncStructInfoList(fun_name, this.FunDefineList)))
			{
				return retFuncInfo;
			}
			return null;																// 没找到
		}

		static FUNCTION_PARSE_INFO SearchFuncStructInfoList(string fun_name, List<FUNCTION_PARSE_INFO> funInfoList)
		{
			foreach (FUNCTION_PARSE_INFO fsi in funInfoList)
			{
				if (fsi.Name.Equals(fun_name))
				{
					return fsi;
				}
			}
			return null;
		}


		/// <summary>
		/// 根据变量名查找全局变量
		/// </summary>
		public VAR_CONTEXT FindGlobalVarInfoByName(string var_name)
		{
			VAR_CONTEXT retVarCtx = null;
			if (null != (retVarCtx = SearchVariableList(var_name, this.GlobalDeclareList)))
			{
				return retVarCtx;
			}
			else if (null != (retVarCtx = SearchVariableList(var_name, this.GlobalDefineList)))
			{
				return retVarCtx;
			}
			else
			{
				return null;
			}
		}

		static VAR_CONTEXT SearchVariableList(string var_name, List<VAR_CONTEXT> var_list)
		{
			foreach (VAR_CONTEXT vi in var_list)
			{
				if (vi.Name.Equals(var_name))
				{
					return vi;
				}
			}
			return null;
		}

		public MACRO_DEFINE_INFO FindMacroDefInfo(string macro_name)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(macro_name));
			foreach (MACRO_DEFINE_INFO mdi in this.MacroDefineList)
			{
				if (mdi.Name == macro_name)
				{
					return mdi;
				}
			}
			return null;
		}
		#endregion

	}

	/// <summary>
	/// C函数的结构情报
	/// </summary>
	public class FUNCTION_PARSE_INFO
	{
		public string Name = string.Empty;												// 函数名称
		public List<CODE_IDENTIFIER> Qualifiers = new List<CODE_IDENTIFIER>();	        // 修饰符列表
		public List<string> ParaList = new List<string>();						        // 参数列表

		public CODE_SCOPE Scope = null;													// 函数起止范围
	}

	/// <summary>
	/// 用户定义类型情报
	/// </summary>
	public class USER_DEFINE_TYPE_INFO
	{
		public string Category = string.Empty;											// "struct, enum, union"
		public List<CODE_IDENTIFIER> NameList = new List<CODE_IDENTIFIER>();		    // 可能有多个名(逗号分割)
		public List<string> MemberList = new List<string>();

		public CODE_SCOPE Scope = null;
	}

	public class MACRO_DEFINE_INFO
	{
		public string Name = string.Empty;												// 宏名
		public List<string> ParaList = new List<string>();							    // 参数列表
		public string Value = string.Empty;												// 宏值

		public string FileName = string.Empty;
		public int LineNum = -1;

		public MACRO_DEFINE_INFO(string file_name, int line_num)
		{
			this.FileName = file_name;
			this.LineNum = line_num;
		}
	}

	public class TYPE_DEFINE_INFO
	{
		public string OldName = string.Empty;
		public string NewName = string.Empty;
	}
}
