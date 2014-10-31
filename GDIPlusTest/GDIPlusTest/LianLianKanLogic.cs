using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIPlusTest
{
    class LianLianKanLogic
    {
        /// <summary>
        /// 根据子图片布局生成连连看逻辑布局矩阵
        /// </summary>
        /// <param name="layoutList">子图片位置布局列表</param>
        public LianLianKanLogic()
        {
            // 求图片的平均宽高

            // 区分行

            // 区分列
        }

        // 区块位置矩阵化阈值系数
        private const double THRESHOLD_MODULUS = 0.2;

        /// <summary>
        /// 区块位置矩阵化处理
        /// </summary>
        public static double Matrixing(ref List<FoundPosition> layoutList)
        {
            // 求区块平均宽高
            double averWidthVal, averHeightVal;
            GetAverageWidthAndHeight(layoutList, out averWidthVal, out averHeightVal);

            // 异常检测
            System.Diagnostics.Trace.Assert((0 != averWidthVal) && (0 != averHeightVal));

            // 根据平均值结果设定阈值
            double threshold_h = averWidthVal * THRESHOLD_MODULUS;
            double threshold_v = averHeightVal * THRESHOLD_MODULUS;

            // 排定各区块的行列号
            //  1.找到第一个(最上面)未确定行列号的区块
            int firstIdx = -1;
            // 直到找不到, 说明所有区块都已确定行列号
            while (-1 != (firstIdx = FindFirstUnconfirmBlock(E_ROW_OR_COL.E_ROW, layoutList)))
            {
                //  2.确定跟它处于同一行的其它各区块
            }
            while (-1 != (firstIdx = FindFirstUnconfirmBlock(E_ROW_OR_COL.E_COL, layoutList)))
            {
                //  2.确定跟它处于同一列的其它各区块
            }

            return averWidthVal;
        }

        /// <summary>
        /// 求区块平均宽高值
        /// </summary>
        static void GetAverageWidthAndHeight(List<FoundPosition> layoutList,
                                                out double averWidth, out double averHeight)
        {
            averWidth = 0;
            averHeight = 0;

            System.Diagnostics.Trace.Assert(null != layoutList);
            if (0 == layoutList.Count)
            {
                return;
            }

            double sumWidthVal = 0, sumHeightVal = 0;
            foreach (FoundPosition fp in layoutList)
            {
                sumWidthVal += fp.subImgInfo.subWidth;
                sumHeightVal += fp.subImgInfo.subHeight;
            }
            averWidth = sumWidthVal / layoutList.Count;
            averHeight = sumHeightVal / layoutList.Count;
        }

        // 行处理or列处理
        enum E_ROW_OR_COL
        {
            E_ROW,
            E_COL
        };

        /// <summary>
        /// 找出第一个(最上, 最左)未被确定行列号的区块
        /// </summary>
        static int FindFirstUnconfirmBlock(E_ROW_OR_COL mode, List<FoundPosition> layoutList)
        {
            int idx = -1;       // 返回找到区块在list中的索引值
            int firstVal = -1;  // 最值
            int i = 0;
            foreach (FoundPosition fp in layoutList)
            {
                if (E_ROW_OR_COL.E_ROW == mode)
                {
                    if (-1 == fp.Row)               // 该区块还未确定行番号
                    {
                        if (-1 == firstVal)
                        {
                            firstVal = fp.X;
                            idx = i;
                        }
                        else if (fp.X < firstVal)   // 判断如果X方向位置更靠前就更新返回索引
                        {
                            firstVal = fp.X;
                            idx = i;
                        }
                    }
                }
                else // E_ROW_OR_COL.E_COL
                {
                    if (-1 == fp.Col)               // 该区块还未确定列番号
                    {
                        if (-1 == firstVal)
                        {
                            firstVal = fp.Y;
                            idx = i;
                        }
                        else if (fp.Y < firstVal)   // 判断如果Y方向位置更靠上就更新返回索引
                        {
                            firstVal = fp.Y;
                            idx = i;
                        }
                    }
                }
                i += 1;
            }

            return idx;
        }

        /// <summary>
        /// 判断两个区块是否处于同一行/列
        /// </summary>
        /// <returns></returns>
        static bool JudgeBlocksSameLevel(E_ROW_OR_COL mode,                     // 行或列
                                        List<FoundPosition> layoutList,         // 区块列表
                                        int idx1, int idx2,                     // 两个区块的索引
                                        double threshold                        // 判定的阈值
                                        )
        {
            return false;
        }
    }
}
