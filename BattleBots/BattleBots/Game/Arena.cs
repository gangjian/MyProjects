using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleBots.Game
{
	class Arena
	{
		Size MapSize = new Size(100, 100);

		List<Robot> RobotList = new List<Robot>();
		List<Bullet> BulletList = new List<Bullet>();
		UInt32 RoundCounter = 0;

		void GameStart()
		{
			while (!GameOver())
			{
				RobotsLoop();
				BulletsLoop();
				RoundCounter += 1;
			}
		}

		const UInt32 MAX_GAME_ROUND = 100;
		bool GameOver()
		{
			if (MAX_GAME_ROUND == this.RoundCounter)
			{
				return true;
			}
			return false;
		}

		void RobotsLoop()
		{
			foreach (var bot in this.RobotList)
			{
				string rsp_str = bot.RoundReq();
				UpdateRobot(bot, rsp_str);
			}
		}

		void BulletsLoop()
		{
			foreach (var blt in this.BulletList)
			{
				
			}
		}

		void UpdateRobot(Robot bot_ref, string rsp_str)
		{
			RobotAction action = ResponseProc.GetRobotAction(rsp_str);
		}
	}

	class Size
	{
		UInt32 Height = 0;
		UInt32 Width = 0;

		public Size(UInt32 h, UInt32 w)
		{
			this.Height = h;
			this.Width = w;
		}
	}

	class Robot
	{
		// 各种属性
		RobotProperty Property = new RobotProperty();
		// 状态
		ObjectStatus Status = new ObjectStatus();

		// AI引用
		RobotAI AiRef = null;

		public Robot()
		{
			this.AiRef = new RobotAI(this.Status);
		}

		public string RoundReq()
		{
			return this.AiRef.GameRequest("Round Request");
		}
	}

	class RobotProperty
	{
		readonly UInt32 ID = 0;
		readonly string Name = string.Empty;
	}

	class Bullet
	{
		UInt32 ID = 0;
		ObjectStatus Status = new ObjectStatus();
	}

	class ObjectStatus
	{
		Size Size = new Size(1, 1);
		Point Position = new Point();
		UInt16 Direction = 0;													// 方向: 与Y周顺时针方向夹角(0~360)
		UInt16 Speed = 0;
	}
}
