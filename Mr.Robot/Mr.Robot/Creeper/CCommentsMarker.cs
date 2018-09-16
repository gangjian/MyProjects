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
		public static List<CommentsInfo> MarkCSourceComments(string c_src_path)
		{
			Trace.Assert(!string.IsNullOrEmpty(c_src_path) && File.Exists(c_src_path));
			List<CommentsInfo> ret_list = new List<CommentsInfo>();
			List<string> code_list = File.ReadAllLines(c_src_path).ToList();
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
									new CODE_POSITION(line_idx, line_cmts_idx),
									null));
				}
				else if ((-1 != line_cmts_idx)
						 && (-1 == block_cmts_idx))
				{
					// 只包含行注释
					ret_list.Add(	new CommentsInfo(CommentsCategory.LINE,
									new CODE_POSITION(line_idx, line_cmts_idx),
									null));
				}
				else if (-1 != block_cmts_idx)
				{
					// 块注释在前
					int idx_s = block_cmts_idx;
					CODE_POSITION start_pos = new CODE_POSITION(line_idx, idx_s);
					int idx_e = code_line.IndexOf("*/", idx_s + 2);
					while (-1 == idx_e)
					{
						line_idx += 1;
						if (line_idx >= code_list.Count)
						{
							break;
						}
						code_line = code_list[line_idx];
						idx_e = code_list.IndexOf("*/");
					}
					Trace.Assert(-1 != idx_e);
					CODE_POSITION end_pos = new CODE_POSITION(line_idx, idx_e);
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

		// 注释类别
		public enum CommentsCategory
		{
			LINE,			// 行注释
			BLOCK,			// 块注释
		}

		public class CommentsInfo
		{
			public CommentsCategory CateGory;
			public CODE_POSITION StartPosition;
			public CODE_POSITION EndPosition;

			public CommentsInfo(CommentsCategory category,
								CODE_POSITION start_position,
								CODE_POSITION end_position)
			{
				this.CateGory = category;
				this.StartPosition = start_position;
				this.EndPosition = end_position;
			}
		}
	}
}
