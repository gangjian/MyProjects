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
		List<string> m_SourcePathList = new List<string>();
		List<string> m_HeaderPathList = new List<string>();
		string m_ProjectPath = null;

		List<SO_File> m_SourceInfoList = new List<SO_File>();
		List<SO_File> m_HeaderInfoList = new List<SO_File>();

		public SO_Project(string prj_dir)
		{
			Init(prj_dir);
			DoParse();
		}

		void Init(string prj_dir)
		{
			Trace.Assert(!string.IsNullOrEmpty(prj_dir) && Directory.Exists(prj_dir));
			this.m_ProjectPath = prj_dir;
			SO_Common.GetCodeFileList(this.m_ProjectPath, this.m_SourcePathList, this.m_HeaderPathList);
		}

		void DoParse()
		{
			foreach (var src_path in this.m_SourcePathList)
			{
				SO_File src_info = new SO_File(src_path);
				this.m_SourceInfoList.Add(src_info);
			}
			foreach (var hd_path in this.m_HeaderPathList)
			{
				if (!HeaderInfoExists(hd_path))
				{
					SO_File hd_info = new SO_File(hd_path);
					this.m_HeaderInfoList.Add(hd_info);
				}
			}
		}

		bool HeaderInfoExists(string path)
		{
			foreach (var item in this.m_HeaderInfoList)
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
