using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIPlusTest.GameRobots.Robot1
{
    class FoundPosition
    {
        // 起始点位置坐标
        public int X = -1;
        public int Y = -1;

        // 矩阵化后的行列编号
        public int Row = -1;
        public int Col = -1;

        public SubImgInfo subImgInfo = new SubImgInfo();

        public FoundPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // 区块情报
    class SubImgInfo
    {
        // 编号
        public int subIdx = -1;
        // 宽
        public int subWidth = -1;
        // 高
        public int subHeight = -1;
    }
}
