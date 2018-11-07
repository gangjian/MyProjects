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

		public SO_Project(string prj_dir)
		{
			Init(prj_dir);
			DoParse();
		}
		public SO_File GetFileInfo(string path)
		{
			foreach (var item in this.SourceInfoList)
			{
				if (item.FullName.Equals(path))
				{
					return item;
				}
			}
			foreach (var item in this.HeaderInfoList)
			{
				if (item.FullName.Equals(path))
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
			SO_Common.GetCodeFileList(this.ProjectPath, this.SourcePathList, this.HeaderPathList);
		}
		void DoParse()
		{
			foreach (var src_path in this.SourcePathList)
			{
				SO_File src_info = new SO_File(src_path);
				this.SourceInfoList.Add(src_info);
			}
			foreach (var hd_path in this.HeaderPathList)
			{
				if (!HeaderInfoExists(hd_path))
				{
					SO_File hd_info = new SO_File(hd_path);
					this.HeaderInfoList.Add(hd_info);
				}
			}
		}
		bool HeaderInfoExists(string path)
		{
			foreach (var item in this.HeaderInfoList)
			{
				if (item.FullName.Equals(path))
				{
					return true;
				}
			}
			return false;
		}
	}
}
