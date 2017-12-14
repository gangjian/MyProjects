using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	public class DEDUCER_CONTEXT
	{
		public List<VAR_CTX2> VarCtxList = new List<VAR_CTX2>();
		public STATEMENT_NODE LastStepNode = null;
		public List<STATEMENT_NODE> ForkPointList = new List<STATEMENT_NODE>();			// 歧路点List
		//public DEDUCER_INPUT_TBL InputTable = new DEDUCER_INPUT_TBL();

		public VAR_CTX2 FindVarCtxByName(string name)
		{
			foreach (var item in this.VarCtxList)
			{
				if (item.VarName.Equals(name))
				{
					return item;
				}
			}
			return null;
		}

		/// <summary>
		/// 恢复上下文状态到指定节点位置
		/// </summary>
		public void RestoreStatus(string step_mark_str)
		{
			for (int i = 0; i < this.VarCtxList.Count; i++)
			{
				VAR_CTX2 varCtx = this.VarCtxList[i];
				int count = varCtx.ValueEvolveList.Count;
				for (int j = count - 1; j >= 0 ; j--)
				{
					VAR_RECORD varRecord = varCtx.ValueEvolveList[j];
					if (!string.IsNullOrEmpty(varRecord.StepMarkStr)
						&& StepMarkStringCompare(varRecord.StepMarkStr, step_mark_str) > 0)
					{
						varCtx.ValueEvolveList.RemoveAt(j);
					}
				}
			}
		}

		int StepMarkStringCompare(string step_mark_1, string step_mark_2)
		{
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(step_mark_1));
			System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(step_mark_2));
			if (step_mark_1.Equals(step_mark_2))
			{
				return 0;
			}
			else
			{
				string[] arr1 = step_mark_1.Split(',');
				string[] arr2 = step_mark_2.Split(',');
				System.Diagnostics.Trace.Assert(0 != arr1.Count());
				System.Diagnostics.Trace.Assert(0 != arr2.Count());
				int count = arr1.Count();
				int retVal = -1;
				if (arr2.Count() < count)
				{
					count = arr2.Count();
					retVal = 1;
				}
				for (int i = 0; i < count; i++)
				{
					if (!arr1[i].Equals(arr2[i]))
					{
						if (arr1[i].StartsWith("-") || arr2[i].StartsWith("-"))
						{
							// 包含分支(Branch)且不同, 说明不在同一条路径上
							return CompareValStr(arr1[i].Substring(1), arr2[i].Substring(1));
						}
						else
						{
							return CompareValStr(arr1[i], arr2[i]);
						}
					}
				}
				// 比到这还没有分出前后, 那长的就靠后,短的靠前
				return retVal;
			}
		}

		int CompareValStr(string val_str1, string val_str2)
		{
			int val1, val2;
			if (!int.TryParse(val_str1, out val1)
				|| !int.TryParse(val_str2, out val2))
			{
				System.Diagnostics.Trace.Assert(false);
				return 0;
			}
			if (val1 > val2)
			{
				return 1;
			}
			else if (val1 < val2)
			{
				return -1;
			}
			else
			{
				return 0;
			}
		}
	}

	public class VAR_CTX2
	{
		public string VarName = string.Empty;											// 变量名
		public VAR_TYPE2 VarType = null;												// 变量类型
		public VAR_CATEGORY VarCategory = VAR_CATEGORY.INVALID;							// 变量分类(函数参数, 全局变量, 局部变量)
		public List<VAR_CTX2> MemberList = new List<VAR_CTX2>();						// 成员列表(非基本型时)
		public List<VAR_RECORD> ValueEvolveList = new List<VAR_RECORD>();				// 变量值的Step演化列表

		public VAR_CTX2(VAR_TYPE2 var_type, string var_name, VAR_CATEGORY category)
		{
			this.VarName = var_name;
			this.VarType = var_type;
			this.VarCategory = category;
		}

		public VAR_RECORD GetLastAssignmentRecord()
		{
			if (this.ValueEvolveList.Last().VarBehave != VAR_BEHAVE.DECLARE)
			{
				return this.ValueEvolveList.Last();
			}
			return null;
		}

		/// <summary>
		/// 取值限定是否可接受判断
		/// </summary>
		public bool CheckValueLimitPossible(VAL_LIMIT_EXPR val_expr)
		{
			// 1.先判断是否符合变量类型的限制条件
			if (!D_COMMON.CheckValueLimitTypeCompatible(this.VarType, val_expr))
			{
				return false;
			}
			// 1.首先判断跟既存的取值限定是否冲突
			foreach (var item in this.ValueEvolveList)
			{
				if (item.VarBehave == VAR_BEHAVE.VALUE_LIMIT
					&& !D_COMMON.Check2ValLimitExprCompatible(item.ValExpr, val_expr))
				{
					return false;
				}
			}
			return true;
		}
	}

	public class VAR_TYPE2
	{
		public string TypeName = string.Empty;											// 类型名
		public List<string> PrefixList = new List<string>();							// 前缀列表
		public List<string> SuffixList = new List<string>();							// 后缀列表

		public VAR_TYPE2(string type_name, List<string> prefix_list, List<string> suffix_list)
		{
			this.TypeName = type_name;
			this.PrefixList = prefix_list;
			this.SuffixList = suffix_list;
		}

		public void GetLimitsVal(out object max_val, out object min_val)
		{
			max_val = null;
			min_val = null;
			switch (this.TypeName)
			{
				case "int":
				case "signed int":
					max_val = int.MaxValue;
					min_val = int.MinValue;
					break;
				case "unsigned int":
					max_val = uint.MaxValue;
					min_val = uint.MinValue;
					break;
				case "char":
					max_val = char.MaxValue;
					min_val = char.MinValue;
					break;
				case "unsigned char":
					max_val = byte.MaxValue;
					min_val = byte.MinValue;
					break;
				case "short":
					max_val = short.MaxValue;
					min_val = short.MinValue;
					break;
				case "unsigned short":
					max_val = ushort.MaxValue;
					min_val = ushort.MinValue;
					break;
				case "long":
					max_val = long.MaxValue;
					min_val = long.MinValue;
					break;
				case "unsigned long":
					max_val = ulong.MaxValue;
					min_val = ulong.MinValue;
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
		}

		public bool TryParse(string val_str, out object val)
		{
			val = null;
			bool ret = false;
			switch (this.TypeName)
			{
				case "int":
				case "signed int":
					int iVal;
					ret = int.TryParse(val_str, out iVal);
					val = iVal;
					break;
				case "unsigned int":
					uint uiVal;
					ret = uint.TryParse(val_str, out uiVal);
					val = uiVal;
					break;
				case "char":
					char cVal;
					ret = char.TryParse(val_str, out cVal);
					val = cVal;
					break;
				case "unsigned char":
					byte bVal;
					ret = byte.TryParse(val_str, out bVal);
					val = bVal;
					break;
				case "short":
					short sVal;
					ret = short.TryParse(val_str, out sVal);
					val = sVal;
					break;
				case "unsigned short":
					ushort usVal;
					ret = ushort.TryParse(val_str, out usVal);
					val = usVal;
					break;
				case "long":
					long lVal;
					ret = long.TryParse(val_str, out lVal);
					val = lVal;
					break;
				case "unsigned long":
					ulong ulVal;
					ret = ulong.TryParse(val_str, out ulVal);
					val = ulVal;
					break;
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}

			return ret;
		}

		public bool LogicCompare(string oprt_str, object val1, object val2)
		{
			switch (this.TypeName)
			{
				case "int":
				case "signed int":
					return IntCompare(oprt_str, val1, val2);
				case "unsigned int":
					return UIntCompare(oprt_str, val1, val2);
				case "char":
					return CharCompare(oprt_str, val1, val2);
				case "unsigned char":
					return ByteCompare(oprt_str, val1, val2);
				case "short":
					return ShortCompare(oprt_str, val1, val2);
				case "unsigned short":
					return UShortCompare(oprt_str, val1, val2);
				case "long":
					return LongCompare(oprt_str, val1, val2);
				case "unsigned long":
					return ULongCompare(oprt_str, val1, val2);
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}

		// TODO: 以后要用泛型方法替换
		static bool IntCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToInt32(val1) > Convert.ToInt32(val2));
				case ">=":
					return (Convert.ToInt32(val1) >= Convert.ToInt32(val2));
				case "<":
					return (Convert.ToInt32(val1) < Convert.ToInt32(val2));
				case "<=":
					return (Convert.ToInt32(val1) <= Convert.ToInt32(val2));
				case "==":
					return (Convert.ToInt32(val1) == Convert.ToInt32(val2));
				case "!=":
					return (Convert.ToInt32(val1) != Convert.ToInt32(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool UIntCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToUInt32(val1) > Convert.ToUInt32(val2));
				case ">=":
					return (Convert.ToUInt32(val1) >= Convert.ToUInt32(val2));
				case "<":
					return (Convert.ToUInt32(val1) < Convert.ToUInt32(val2));
				case "<=":
					return (Convert.ToUInt32(val1) <= Convert.ToUInt32(val2));
				case "==":
					return (Convert.ToUInt32(val1) == Convert.ToUInt32(val2));
				case "!=":
					return (Convert.ToUInt32(val1) != Convert.ToUInt32(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool CharCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToChar(val1) > Convert.ToChar(val2));
				case ">=":
					return (Convert.ToChar(val1) >= Convert.ToChar(val2));
				case "<":
					return (Convert.ToChar(val1) < Convert.ToChar(val2));
				case "<=":
					return (Convert.ToChar(val1) <= Convert.ToChar(val2));
				case "==":
					return (Convert.ToChar(val1) == Convert.ToChar(val2));
				case "!=":
					return (Convert.ToChar(val1) != Convert.ToChar(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool ByteCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToByte(val1) > Convert.ToByte(val2));
				case ">=":
					return (Convert.ToByte(val1) >= Convert.ToByte(val2));
				case "<":
					return (Convert.ToByte(val1) < Convert.ToByte(val2));
				case "<=":
					return (Convert.ToByte(val1) <= Convert.ToByte(val2));
				case "==":
					return (Convert.ToByte(val1) == Convert.ToByte(val2));
				case "!=":
					return (Convert.ToByte(val1) != Convert.ToByte(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool ShortCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToInt16(val1) > Convert.ToInt16(val2));
				case ">=":
					return (Convert.ToInt16(val1) >= Convert.ToInt16(val2));
				case "<":
					return (Convert.ToInt16(val1) < Convert.ToInt16(val2));
				case "<=":
					return (Convert.ToInt16(val1) <= Convert.ToInt16(val2));
				case "==":
					return (Convert.ToInt16(val1) == Convert.ToInt16(val2));
				case "!=":
					return (Convert.ToInt16(val1) != Convert.ToInt16(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool UShortCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToUInt16(val1) > Convert.ToUInt16(val2));
				case ">=":
					return (Convert.ToUInt16(val1) >= Convert.ToUInt16(val2));
				case "<":
					return (Convert.ToUInt16(val1) < Convert.ToUInt16(val2));
				case "<=":
					return (Convert.ToUInt16(val1) <= Convert.ToUInt16(val2));
				case "==":
					return (Convert.ToUInt16(val1) == Convert.ToUInt16(val2));
				case "!=":
					return (Convert.ToUInt16(val1) != Convert.ToUInt16(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool LongCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToInt64(val1) > Convert.ToInt64(val2));
				case ">=":
					return (Convert.ToInt64(val1) >= Convert.ToInt64(val2));
				case "<":
					return (Convert.ToInt64(val1) < Convert.ToInt64(val2));
				case "<=":
					return (Convert.ToInt64(val1) <= Convert.ToInt64(val2));
				case "==":
					return (Convert.ToInt64(val1) == Convert.ToInt64(val2));
				case "!=":
					return (Convert.ToInt64(val1) != Convert.ToInt64(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
		static bool ULongCompare(string oprt_str, object val1, object val2)
		{
			switch (oprt_str)
			{
				case ">":
					return (Convert.ToUInt64(val1) > Convert.ToUInt64(val2));
				case ">=":
					return (Convert.ToUInt64(val1) >= Convert.ToUInt64(val2));
				case "<":
					return (Convert.ToUInt64(val1) < Convert.ToUInt64(val2));
				case "<=":
					return (Convert.ToUInt64(val1) <= Convert.ToUInt64(val2));
				case "==":
					return (Convert.ToUInt64(val1) == Convert.ToUInt64(val2));
				case "!=":
					return (Convert.ToUInt64(val1) != Convert.ToUInt64(val2));
				default:
					System.Diagnostics.Trace.Assert(false);
					break;
			}
			return false;
		}
	}

	public class VAR_RECORD
	{
		public VAR_BEHAVE VarBehave;
		public string StepMarkStr;
		public VAL_LIMIT_EXPR ValExpr = null;											// 表达式(比如取值限制or赋值表达式)

		public VAR_RECORD(VAR_BEHAVE var_behave, string step_mark)
		{
			this.VarBehave = var_behave;
			this.StepMarkStr = step_mark;
		}
	}

	public class VAL_LIMIT_EXPR
	{
		public string OprtStr = string.Empty;											// 运算符
		public string ExprStr = string.Empty;											// 运算符右侧表达式语句
		public VAL_LIMIT_EXPR(string oprt_str, string expr_str)
		{
			this.OprtStr = oprt_str;
			this.ExprStr = expr_str;
		}
	}

	/// <summary>
	/// 变量分类
	/// </summary>
	public enum VAR_CATEGORY
	{
		INVALID,
		FUNC_PARA,
		LOCAL,
		GLOBAL,
	}

	/// <summary>
	/// 变量行为
	/// </summary>
	public enum VAR_BEHAVE
	{
		DECLARE,					// 声明
		ASSIGNMENT,					// 赋值
		READ_OUT,					// (读)出参
		VALUE_LIMIT,				// 取值限制
	}
}
