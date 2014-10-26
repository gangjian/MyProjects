using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIPlusTest
{
    class FoundPosition
    {
        public int X = -1;
        public int Y = -1;
        public SubImgInfo subImgInfo = new SubImgInfo();

        public FoundPosition(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class SubImgInfo
    {
        public int subIdx = -1;
        public int subWidth = -1;
        public int subHeight = -1;
    }
}
