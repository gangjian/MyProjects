using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DmyFuncMaker
{
	class DmyFuncMaker
	{
		public static List<string> DmyFuncPrototypeProc(string prot_str)
		{
			prot_str = prot_str.Trim();
			if (string.IsNullOrEmpty(prot_str))
			{
				return null;
			}
			if (prot_str.StartsWith("FUNC"))
			{
				
			}
			else
			{

			}
			return null;
		}

		static BracketPair GetNextBracketPair(string statement_str, int offset)
		{
			int left = -1;
			int counter = 0;
			for (int i = offset; i < statement_str.Length; i++)
			{
				if ('(' == statement_str[i])
				{
					counter += 1;
					if (-1 == left)
					{
						left = i;
					}
				}
				else if (')' == statement_str[i])
				{
					counter -= 1;
					if (0 == counter)
					{
						return new BracketPair(left, i);
					}
				}
			}
			return null;
		}

		static List<BracketPair> GetBracketPairs(string statement_str, int offset)
		{
			List<BracketPair> retList = new List<BracketPair>();
			while (true)
			{
				BracketPair bp = GetNextBracketPair(statement_str, offset);
				if (null == bp)
				{
					break;
				}
				else
				{
					retList.Add(bp);
					offset = bp.rightOffset + 1;
				}
			}
			return retList;
		}

		class BracketPair
		{
			public int leftOffset = -1;
			public int rightOffset = -1;

			public BracketPair(int left, int right)
			{
				this.leftOffset = left;
				this.rightOffset = right;
			}
		}
	}
}
