using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DirSize
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btnBrowseFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbxRootPath.Text = dlg.SelectedPath;

				DirSizeNode root = new DirSizeNode();
				root.Type = DirNodeType.Directory;
				root.Path = dlg.SelectedPath;
				DirectoryInfo di = new DirectoryInfo(dlg.SelectedPath);
				root.Name = di.Name;

				CaculateDirSize(ref root);

				ShowDirTreeView(root);
			}
		}

		void ShowDirTreeView(DirSizeNode root)
		{
			this.treeView1.Nodes.Clear();
			TreeNode treeNode = new TreeNode(root.Name + " (" + GetSizeString(root.Size) + ")");
			GetTreeViewNode(ref treeNode, root);
			treeNode.ExpandAll();
			this.treeView1.Nodes.Add(treeNode);
		}

		void GetTreeViewNode(ref TreeNode tree_node, DirSizeNode dir_node)
		{
			foreach (var item in dir_node.ChildList)
			{
				TreeNode childNode = new TreeNode(item.Name + " (" + GetSizeString(item.Size) + " : " +item.Percent.ToString() + "%)");
				childNode.BackColor = Color.Green;
				if (DirNodeType.Directory == item.Type)
				{
					GetTreeViewNode(ref childNode, item);
				}
				tree_node.Nodes.Add(childNode);
			}
		}

		static void CaculateDirSize(ref DirSizeNode node_ref)
		{
			DirectoryInfo di = new DirectoryInfo(node_ref.Path);
			FileInfo[] fiArr = di.GetFiles();
			DirectoryInfo[] diArr = di.GetDirectories();
			UInt64 totalSize = 0;
			foreach (var item in fiArr)
			{
				DirSizeNode node = new DirSizeNode();
				node.Path = item.FullName;
				node.Name = item.Name;
				node.Type = DirNodeType.File;
				node.Size = (ulong)item.Length;
				node_ref.ChildList.Add(node);

				totalSize += node.Size;
			}

			foreach (var item in diArr)
			{
				DirSizeNode node = new DirSizeNode();
				node.Path = item.FullName;
				node.Name = item.Name;
				node.Type = DirNodeType.Directory;
				CaculateDirSize(ref node);
				node_ref.ChildList.Add(node);

				totalSize += node.Size;
			}
			node_ref.Size = totalSize;
			if (0 != totalSize)
			{
				foreach (var item in node_ref.ChildList)
				{
					item.Percent = Math.Round((decimal)item.Size * 100 / totalSize, 2);
				}
			}
		}

		const UInt64 FILE_SIZE_KB = 1024;
		const UInt64 FILE_SIZE_MB = 1024 * 1024;
		const UInt64 FILE_SIZE_GB = 1024 * 1024 * 1024;

		static string GetSizeString(UInt64 size_value)
		{
			string gbStr = string.Empty;
			if (size_value > FILE_SIZE_GB)
			{
				gbStr = (size_value / FILE_SIZE_GB).ToString() + "g";
				size_value = (size_value % FILE_SIZE_GB);
			}
			string mbStr = string.Empty;
			if (size_value > FILE_SIZE_MB)
			{
				mbStr = (size_value / FILE_SIZE_MB).ToString() + "m";
				size_value = (size_value % FILE_SIZE_MB);
			}
			string kbStr = string.Empty;
			if (size_value > FILE_SIZE_KB)
			{
				kbStr = (size_value / FILE_SIZE_KB).ToString() + "k";
				size_value = (size_value % FILE_SIZE_KB);
			}
			string bStr = string.Empty;
			if (0 != size_value)
			{
				bStr = size_value.ToString() + "b";
			}
			return gbStr + mbStr + kbStr + bStr;
		}

		public class DirSizeNode
		{
			public string Name = string.Empty;
			public string Path = string.Empty;
			public DirNodeType Type = DirNodeType.File;
			public UInt64 Size = 0;
			public decimal Percent = 0;

			public List<DirSizeNode> ChildList = new List<DirSizeNode>();
		}

		public enum DirNodeType
		{
			File,
			Directory,
		}
	}
}
