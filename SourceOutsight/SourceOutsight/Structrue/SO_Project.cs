using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace SourceOutsight
{
	public class SO_Project
	{
		List<string> SourcePathList = new List<string>();
		List<string> HeaderPathList = new List<string>();
		string ProjectPath = null;

		List<SO_File> SourceInfoList = new List<SO_File>();
		List<SO_File> HeaderInfoList = new List<SO_File>();

		public Stack<string> ParseFileStack = new Stack<string>();

		public SO_Project(string prj_dir)
		{
			Init(prj_dir);
			ParseAll();
		}
		public List<SO_File> GetSourceInfoList()
		{
			return this.SourceInfoList;
		}
		public List<SO_File> GetHeaderInfoList()
		{
			return this.HeaderInfoList;
		}
		public void AddHeaderInfo(SO_File header_info)
		{
			this.HeaderInfoList.Add(header_info);
		}
		public SO_File GetFileInfo(string path)
		{
			List<SO_File> file_list = new List<SO_File>();
			file_list.AddRange(this.SourceInfoList);
			file_list.AddRange(this.HeaderInfoList);
			foreach (var item in file_list)
			{
				if (item.FullName.Equals(path))
				{
					return item;
				}
			}
			return null;
		}
		public string GetFileFullPath(string file_name)
		{
			List<string> path_list = new List<string>();
			path_list.AddRange(this.HeaderPathList);
			path_list.AddRange(this.SourcePathList);
			foreach (var item in path_list)
			{
				string name = item;
				int idx = name.LastIndexOf('\\');
				if (-1 != idx)
				{
					name = name.Substring(idx + 1);
				}
				if (name.Equals(file_name))
				{
					return item;
				}
			}
			return null;
		}
		void Init(string prj_dir)
		{
			Trace.Assert(!string.IsNullOrEmpty(prj_dir) && Directory.Exists(prj_dir));
			this.ProjectPath = prj_dir;
			Common.GetCodeFileList(this.ProjectPath, this.SourcePathList, this.HeaderPathList);
		}
		void ParseAll()
		{
			int total = this.SourcePathList.Count + this.HeaderPathList.Count;
			int cnt = 0;
			Stopwatch sw = new Stopwatch();
			List<string> path_list = new List<string>();
			path_list.AddRange(this.SourcePathList);
			path_list.AddRange(this.HeaderPathList);
			foreach (var path in path_list)
			{
				if (null == this.GetFileInfo(path))
				{
					sw.Restart();
					SO_File file_info = new SO_File(path, this);
					this.SourceInfoList.Add(file_info);
					sw.Stop();
					cnt++;
					LogOut(path, cnt, total, sw.Elapsed);
				}
				else
				{
					cnt++;
					Trace.WriteLine(path + "***Already Exists!***");
				}
			}
		}
		void LogOut(string path, int count, int total, TimeSpan time)
		{
			string log = string.Format("{1}/{2} : {3}ms : {0}", path, count, total, time.TotalMilliseconds);
			Trace.WriteLine(log);
		}

		public List<SearchTagResult> SearchTag(string tag_str)
		{
			Trace.Assert(!string.IsNullOrEmpty(tag_str));
			List<SearchTagResult> ret_list = new List<SearchTagResult>();
			List<SO_File> file_list = new List<SO_File>();
			file_list.AddRange(this.HeaderInfoList);
			file_list.AddRange(this.SourceInfoList);
			foreach (var f in file_list)
			{
				List<TagTreeNode> tag_node_list = f.SearchTag(tag_str);
				if (0 != tag_node_list.Count)
				{
					SearchTagResult result = new SearchTagResult(f.FullName);
					foreach (var tag_node in tag_node_list)
					{
						result.TagInfoList.Add(tag_node.Info);
					}
					ret_list.Add(result);
				}
			}
			return ret_list;
		}
	}

	public class SearchTagResult
	{
		public string Path;
		public List<TagInfo> TagInfoList = new List<TagInfo>();
		public SearchTagResult(string path)
		{
			this.Path = path;
		}
	}
}
