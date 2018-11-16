using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BattleBots.Game
{
	class RobotAI
	{
		ObjectStatus StatusRef = null;
		public RobotAI(ObjectStatus status_ref)
		{
			Trace.Assert(null != status_ref);
			this.StatusRef = status_ref;
		}

		public string GameRequest(string req_str)
		{
			string rsp_str = string.Empty;
			return rsp_str;
		}
	}
}
