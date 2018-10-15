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
	public class CodePosition
	{
		public int RowNum = 0;    // 行号
		public int ColNum = 0;    // 列号

		public CodePosition(int r, int c)
		{
			this.RowNum = r;
			this.ColNum = c;
		}

		public CodePosition(CodePosition nfp)
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

		public static int Compare(CodePosition pos_1, CodePosition pos_2)
		{
			if (pos_1.RowNum < pos_2.RowNum)
			{
				return (-1);
			}
			else if (pos_1.RowNum > pos_2.RowNum)
			{
				return 1;
			}
			else
			{
				if (pos_1.ColNum < pos_2.ColNum)
				{
					return (-1);
				}
				else if (pos_1.ColNum > pos_2.ColNum)
				{
					return 1;
				}
				else
				{
					return 0;
				}
			}
		}
	}

	public class CodeScope
	{
		public CodePosition Start = new CodePosition(-1, -1);
		public CodePosition End = new CodePosition(-1, -1);

		public CodeScope(CodePosition start_pos, CodePosition end_pos)
		{
			this.Start = new CodePosition(start_pos);
			this.End = new CodePosition(end_pos);
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
		public List<FunctionParseInfo> FuncDeclareList = new List<FunctionParseInfo>();		// 函数声明列表
		public List<FunctionParseInfo> FunDefineList = new List<FunctionParseInfo>();		// 函数定义列表
		public List<UserDefineTypeInfo> UsrDefTypeList = new List<UserDefineTypeInfo>();	// 用户定义类型列表
		public List<VAR_CTX> GlobalDeclareList = new List<VAR_CTX>();					// 全局量声明列表
		public List<VAR_CTX> GlobalDefineList = new List<VAR_CTX>();					// 全局量定义列表
		public List<MacroDefineInfo> MacroDefineList = new List<MacroDefineInfo>();			// 宏定义列表
		public List<TypeDefineInfo> TypeDefineList = new List<TypeDefineInfo>();			// typedef类型定义列表

		public List<string> CodeList = new List<string>();										// 解析后(去除注释, 宏展开等)的代码行内容列表

		// MT预编译宏开关值提取追加
		public List<string> MacroSwitchList = new List<string>();

		public FILE_PARSE_INFO(string fileName)
		{
			System.Diagnostics.Trace.Assert(File.Exists(fileName));
			this.SourceName = fileName;
		}

		public UserDefineTypeInfo FindUsrDefTypeInfo(string type_name, string category_name = null)
		{
			foreach (UserDefineTypeInfo udti in this.UsrDefTypeList)
			{
				foreach (CodeIdentifier nameIdtf in udti.NameList)
				{
					if (nameIdtf.Text.Equals(type_name))
					{
						if (null != category_name)
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

		public TypeDefineInfo FindTypeDefInfo(string type_name)
		{
			foreach (TypeDefineInfo tdi in this.TypeDefineList)
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
			TypeDefineInfo tdi = null;												// 如果经过typedef重命名的话, 找出原来的类型名
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
		public FunctionParseInfo FindFuncParseInfo(string fun_name)
		{
			FunctionParseInfo retFuncInfo = null;
			if (null != (retFuncInfo = SearchFunctionInfoList(fun_name, this.FunDefineList)))
			{
				return retFuncInfo;
			}
			if (null != (retFuncInfo = SearchFunctionInfoList(fun_name, this.FuncDeclareList)))
			{
				return retFuncInfo;
			}
			return null;																// 没找到
		}

		public static FunctionParseInfo SearchFunctionInfoList(string fun_name, List<FunctionParseInfo> funInfoList)
		{
			foreach (FunctionParseInfo fsi in funInfoList)
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
		public VAR_CTX FindGlobalVarInfoByName(string var_name)
		{
			VAR_CTX retVarCtx = null;
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

		static VAR_CTX SearchVariableList(string var_name, List<VAR_CTX> var_list)
		{
			foreach (VAR_CTX vi in var_list)
			{
				if (vi.Name.Equals(var_name))
				{
					return vi;
				}
			}
			return null;
		}

		public MacroDefineInfo FindMacroDefInfo(string macro_name)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(macro_name));
			foreach (MacroDefineInfo mdi in this.MacroDefineList)
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
	public class FunctionParseInfo
	{
		public string Name = string.Empty;												// 函数名称
		public List<CodeIdentifier> Qualifiers = new List<CodeIdentifier>();	        // 修饰符列表
		public List<string> ParaList = new List<string>();						        // 参数列表

		public CodeScope Scope = null;													// 函数起止范围
	}

	/// <summary>
	/// 用户定义类型情报
	/// </summary>
	public class UserDefineTypeInfo
	{
		public string Category = string.Empty;											// "struct, enum, union"
		public List<CodeIdentifier> NameList = new List<CodeIdentifier>();		    // 可能有多个名(逗号分割)
		public List<string> MemberList = new List<string>();

		public CodeScope Scope = null;
	}

	public class MacroDefineInfo
	{
		public string Name = string.Empty;												// 宏名
		public List<string> ParaList = new List<string>();							    // 参数列表
		public string ValStr = string.Empty;											// 宏值

		public string FileName = string.Empty;
		public int LineNum = -1;

		public MacroDefineInfo(string file_name, int line_num)
		{
			this.FileName = file_name;
			this.LineNum = line_num;
		}
	}

	public class TypeDefineInfo
	{
		public string OldName = string.Empty;
		public string NewName = string.Empty;
	}
}
