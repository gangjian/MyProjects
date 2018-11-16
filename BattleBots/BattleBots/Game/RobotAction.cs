using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBots.Game
{
	class RobotAction
	{
		MoveAction Move = MoveAction.NONE;
	}

	enum MoveAction
	{
		NONE,
		FORWARD,
		TURNLEFT,
		TURNRIGHT,
		BACKWARD,
	}

	class ResponseProc
	{
		public static RobotAction GetRobotAction(string rsp_str)
		{
			return null;
		}
	}
}
