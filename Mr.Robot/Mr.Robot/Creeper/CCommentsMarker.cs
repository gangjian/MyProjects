using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Mr.Robot.Creeper
{
	public class CCommentsMarker
	{
		public static List<CommentsInfo> MarkCSourceComments(List<string> code_list)
		{
			List<CommentsInfo> ret_list = new List<CommentsInfo>();
			int line_idx = 0;
			int column_idx = 0;
			while (true)
			{
				string code_line = code_list[line_idx];
				int line_cmts_idx = code_line.IndexOf("//", column_idx);
				int block_cmts_idx = code_line.IndexOf("/*", column_idx);
				if ((-1 != line_cmts_idx)
					&& (-1 != block_cmts_idx)
					&& (line_cmts_idx < block_cmts_idx)
					)
				{
					// 行注释在前
					ret_list.Add(	new CommentsInfo(CommentsCategory.LINE,
									new CodePosition(line_idx, line_cmts_idx),
									null));
				}
				else if ((-1 != line_cmts_idx)
						 && (-1 == block_cmts_idx))
				{
					// 只包含行注释
					ret_list.Add(	new CommentsInfo(CommentsCategory.LINE,
									new CodePosition(line_idx, line_cmts_idx),
									null));
				}
				else if (-1 != block_cmts_idx)
				{
					// 块注释在前
					int idx_s = block_cmts_idx;
					CodePosition start_pos = new CodePosition(line_idx, idx_s);
					int idx_e = code_line.IndexOf("*/", idx_s + 2);
					while (-1 == idx_e)
					{
						line_idx += 1;
						if (line_idx >= code_list.Count)
						{
							break;
						}
						code_line = code_list[line_idx];
						idx_e = code_line.IndexOf("*/");
					}
					Trace.Assert(-1 != idx_e);
					CodePosition end_pos = new CodePosition(line_idx, idx_e);
					ret_list.Add(	new CommentsInfo(CommentsCategory.BLOCK,
									start_pos,
									end_pos));
					column_idx = idx_e + 2;
					// 用continue跳到循环开始处继续处理下一条注释
					continue;
				}
				else
				{
					// 不包含注释
				}

				line_idx += 1;
				column_idx = 0;
				if (line_idx >= code_list.Count)
				{
					break;
				}
			}
			return ret_list;
		}

		public static List<string> RemoveComments2(List<string> code_list)
		{
			List<string> ret_list = new List<string>(code_list);
			// 取得注释位置标记列表
			List<CommentsInfo> comments_info_list = MarkCSourceComments(code_list);
			// 根据标记删除注释
			for (int i = comments_info_list.Count - 1; i >= 0; i--)
			{
				RemoveComment(ret_list, comments_info_list[i]);
			}
			for (int i = 0; i < ret_list.Count; i++)
			{
				ret_list[i] = ret_list[i].TrimEnd();
			}
			return ret_list;
		}

		static void RemoveComment(List<string> code_list, CommentsInfo cmt_info)
		{
			if (cmt_info.CateGory == CommentsCategory.LINE)
			{
				int r = cmt_info.StartPosition.RowNum;
				int c = cmt_info.StartPosition.ColNum;
				code_list[r] = code_list[r].Remove(c).TrimEnd();
			}
			else if (cmt_info.CateGory == CommentsCategory.BLOCK)
			{
				CodePosition s_pos = cmt_info.StartPosition;
				CodePosition e_pos = cmt_info.EndPosition;
				for (int r = s_pos.RowNum; r <= e_pos.RowNum; r++)
				{
					int s_col = 0;
					if (r == s_pos.RowNum)
					{
						s_col = s_pos.ColNum;
					}
					int e_col = code_list[r].Length - 1;
					if (r == e_pos.RowNum)
					{
						e_col = e_pos.ColNum + 1;
					}
					code_list[r] = code_list[r].Remove(s_col, e_col - s_col + 1).TrimEnd();
				}
			}
		}

		// 注释类别
		public enum CommentsCategory
		{
			LINE,			// 行注释
			BLOCK,			// 块注释
		}

		public class CommentsInfo
		{
			public CommentsCategory CateGory;
			public CodePosition StartPosition;
			public CodePosition EndPosition;

			public CommentsInfo(CommentsCategory category,
								CodePosition start_position,
								CodePosition end_position)
			{
				this.CateGory = category;
				this.StartPosition = start_position;
				this.EndPosition = end_position;
			}
		}
	}
}
