using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCreeper
{
	class SyntaxNode
	{
		string tagStr = null;
		public string TagStr
		{
			get { return tagStr; }
			set { tagStr = value; }
		}
		string expressionStr = null;
		public string ExpressionStr
		{
			get { return expressionStr; }
			set { expressionStr = value; }
		}
		private SyntaxNode parentRef = null;
		public SyntaxNode ParentRef
		{
			get { return parentRef; }
			set { parentRef = value; }
		}
		public object InfoRef = null;

		public SyntaxNode()
		{
		}
		public SyntaxNode(string tag_str, string expression_str)
		{
			this.TagStr = tag_str;
			this.ExpressionStr = expression_str;
		}
		public virtual List<string> ToStringList(int level)
		{
			List<string> ret_list = new List<string>();
			ret_list.Add(this.ToString(level));
			return ret_list;
		}
		protected string ToString(int level)
		{
			string ret_str = GetIndentStr(level) + this.TagStr + " / " + this.ExpressionStr;
			return ret_str;
		}
		string GetIndentStr(int level)
		{
			string ret_str = string.Empty;
			for (int i = 0; i < level; i++)
			{
				ret_str += "\t";
			}
			return ret_str;
		}
	}
}
