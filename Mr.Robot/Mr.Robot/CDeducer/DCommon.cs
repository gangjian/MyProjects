using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mr.Robot.CDeducer
{
	class D_COMMON
	{
		public static VAR_CTX2 CreateVarCtx2(	MEANING_GROUP type_group, List<MEANING_GROUP> var_group_list,
												FILE_PARSE_INFO parse_info, string step_mark, VAR_CATEGORY var_category)
		{
			System.Diagnostics.Trace.Assert(var_group_list.Count > 0 && type_group.Type == MeaningGroupType.VariableType);
			VAR_TYPE2 varType = GetVarTypeFromMeaningGroup(type_group, parse_info);
			string typeName = varType.TypeName;
			string varName = GetVarNameFromMeaningGroup(var_group_list[0], parse_info);
			VAR_CTX2 retCtx = new VAR_CTX2(varType, varName, var_category);
			MEANING_GROUP initGroup = null;
			if (var_group_list.Count > 2 && var_group_list[1].Type == MeaningGroupType.AssignmentMark)
			{
				initGroup = var_group_list[2];
			}
			if (BasicTypeProc.IsBasicTypeName(typeName))
			{
				retCtx.ValueEvolveList.Add(new VAR_RECORD(VAR_BEHAVE.DECLARE, step_mark));			// 声明
				if (null != initGroup)
				{
					retCtx.ValueEvolveList.Add(new VAR_RECORD(VAR_BEHAVE.ASSIGNMENT, step_mark));	// 初始化赋值
				}
			}
			else
			{
				USER_DEFINE_TYPE_INFO udti = parse_info.FindUsrDefTypeInfo(typeName);
				if (null != udti)
				{
					foreach (var item in udti.MemberList)
					{

					}
				}
				else
				{
					System.Diagnostics.Trace.Assert(false);
				}
			}
			return retCtx;
		}

		static VAR_TYPE2 GetVarTypeFromMeaningGroup(MEANING_GROUP type_group, FILE_PARSE_INFO parse_info)
		{
			System.Diagnostics.Trace.Assert(null != type_group && type_group.Type == MeaningGroupType.VariableType);
			string typeName = string.Empty;
			List<STATEMENT_COMPONENT> cpntList = type_group.ComponentList.GetRange(type_group.PrefixCount, type_group.ComponentList.Count - type_group.PrefixCount - type_group.SuffixCount);
			foreach (var item in cpntList)
			{
				typeName = item.Text + " ";
			}
			typeName = parse_info.GetOriginalTypeName(typeName.Trim());
			List<string> prefix_list = new List<string>();
			cpntList = type_group.ComponentList.GetRange(0, type_group.PrefixCount);
			foreach (var cpnt in cpntList)
			{
				prefix_list.Add(cpnt.Text);
			}
			List<string> suffix_list = new List<string>();
			cpntList = type_group.ComponentList.GetRange(type_group.ComponentList.Count - type_group.SuffixCount, type_group.SuffixCount);
			foreach (var cpnt in cpntList)
			{
				suffix_list.Add(cpnt.Text);
			}
			return new VAR_TYPE2(typeName, prefix_list, suffix_list);
		}

		static string GetVarNameFromMeaningGroup(MEANING_GROUP name_group, FILE_PARSE_INFO parse_info)
		{
			string retName = string.Empty;
			if (COMN_PROC.IsStandardIdentifier(name_group.Text))
			{
				retName = name_group.Text;
			}
			return retName;
		}
	}
}
