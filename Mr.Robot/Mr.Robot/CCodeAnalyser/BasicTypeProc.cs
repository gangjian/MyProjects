using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot
{
	public class BasicTypeProc
	{
		/// <summary>
		/// 判断一组标识符是否是一个基本类型名
		/// </summary>
		/// <param name="idStrList"></param>
		public static bool IsBasicTypeName(List<string> idStrList, ref int count)
		{
			List<string> initialParts = new List<string>();
			List<string> lastParts = new List<string>();
			count = 0;
			foreach (string str in idStrList)
			{
				if (("signed" == str || "unsigned" == str)
						 && (0 == lastParts.Count))
				{
					// 类型名开头部分
					initialParts.Add(str);
				}
				else if (("char" == str)
						 || ("short" == str)
						 || ("int" == str)
						 || ("long" == str)
						 || ("float" == str)
						 || ("double" == str)
						 || ("void" == str)
						 )
				{
					lastParts.Add(str);
				}
				else
				{
					break;
				}
				count += 1;
			}
			if (0 != lastParts.Count)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// 判断一个类型名是否是基本类型名
		/// </summary>
		public static bool IsBasicTypeName(string type_name)
		{
			if (string.IsNullOrEmpty(type_name.Trim()))
			{
				return false;
			}
			string[] strArr = type_name.Trim().Split(' ');
			string typeName = string.Empty;
			if (2 == strArr.Length)
			{
				if ("unsigned" == strArr[0] || "signed" == strArr[0])
				{
					typeName = strArr[1];
				}
				else
				{
					return false;
				}
			}
			else if (1 == strArr.Length)
			{
				typeName = strArr[0];
			}

			if (string.Empty != typeName)
			{
				if ("char" == typeName
					|| "short" == typeName
					|| "int" == typeName
					|| "long" == typeName)
				{
					return true;
				}
				else if (("float" == typeName || "double" == typeName)
						 && 1 == strArr.Length)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		public static object CalcTypeCastingValue(string type_name, object val)
		{
			switch (type_name)
			{
				case "char":
					return Convert.ToChar(val);
				case "unsigned char":
					return Convert.ToByte(val);
				case "int":
					return Convert.ToInt32(val);
				case "unsigned int":
					return Convert.ToUInt32(val);
				case "short":
					return Convert.ToInt16(val);
				case "unsigned short":
					return Convert.ToUInt16(val);
				case "long":
					return Convert.ToInt64(val);
				case "unsigned long":
					return Convert.ToUInt64(val);
				case "float":
					return Convert.ToSingle(val);
				case "double":
					return Convert.ToDouble(val);
				default:
					return null;
			}
		}
	}
}
