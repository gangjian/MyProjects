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
            while (-1 != (firstIdx = FindFirstUnconfirmedBlock(E_ROW_OR_COL.E_ROW, layoutList)))
            {
                // 确定它的行号
                    // 找到最后一个确定行列号的区块, 算出它们之间的行列数之差

                // 确定跟它处于同一行的其它各区块
                for (int i = 0; i < layoutList.Count; i++)
                {
                }
            }
            while (-1 != (firstIdx = FindFirstUnconfirmedBlock(E_ROW_OR_COL.E_COL, layoutList)))
            {
                // 确定它的列号
                // 确定跟它处于同一列的其它各区块
            }

            return averWidthVal;
        }

//////////////////////////////////////////////////////以下都是子函数////////////////////////////////////////////////////

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
        /// 找出第一个(最上侧/最左侧)未被确定行列号的区块
        /// </summary>
        static int FindFirstUnconfirmedBlock(E_ROW_OR_COL mode, List<FoundPosition> layoutList)
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
                        else if (fp.X < firstVal)   // 判断如果X方向位置更靠左就更新返回索引
                        {
                            firstVal = fp.X;
                            idx = i;
                        }
                    }
                }
                else if (E_ROW_OR_COL.E_COL == mode) // E_ROW_OR_COL.E_COL
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
                else
                {
                }
                i += 1;
            }

            return idx;
        }

        /// <summary>
        /// 找出最后一个(最下侧/最右侧)已被确定行列号的区块
        /// </summary>
        static int FindLastConfirmedBlock(E_ROW_OR_COL mode, List<FoundPosition> layoutList)
        {
            int idx = -1;
            int lastVal = -1;  // 最值
            int i = 0;
            foreach (FoundPosition fp in layoutList)
            {
                if (E_ROW_OR_COL.E_ROW == mode)
                {
                    if (-1 != fp.Row)               // 该区块已经确定行番号
                    {
                        if (-1 == lastVal)
                        {
                            lastVal = fp.X;
                            idx = i;
                        }
                        else if (fp.X > lastVal)   // 判断如果X方向位置更靠右就更新返回索引
                        {
                            lastVal = fp.X;
                            idx = i;
                        }
                    }
                }
                else if (E_ROW_OR_COL.E_COL == mode) // E_ROW_OR_COL.E_COL
                {
                    if (-1 != fp.Col)               // 该区块已经确定列番号
                    {
                        if (-1 == lastVal)
                        {
                            lastVal = fp.Y;
                            idx = i;
                        }
                        else if (fp.Y > lastVal)   // 判断如果Y方向位置更靠下就更新返回索引
                        {
                            lastVal = fp.Y;
                            idx = i;
                        }
                    }
                }
                else
                {
                    System.Diagnostics.Trace.Assert(false);
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
                                        FoundPosition block1,                   // 区块1
                                        FoundPosition block2,                   // 区块2
                                        double threshold                        // 判定的阈值
                                        )
        {
            if (E_ROW_OR_COL.E_ROW == mode)
            {
                // 如果两个区块的始点(左上角顶点)的Y坐标之差小于阈值, 就认为它们处于同一行
                if (Math.Abs((float)block1.Y - (float)block2.Y) < threshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (E_ROW_OR_COL.E_COL == mode)
            {
                // 如果两个区块的始点(左上角顶点)的X坐标之差小于阈值, 就认为它们处于同一列
                if (Math.Abs((float)block1.X - (float)block2.X) < threshold)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                System.Diagnostics.Trace.Assert(false);
            }
            return false;
        }

        /// <summary>
        /// 取得两个区块间的行/列数的差值
        /// </summary>
        /// <returns></returns>
        static int Get2BlocksRowsOrColsDiff(E_ROW_OR_COL mode,                     // 行或列
                                            FoundPosition block1,                   // 区块1
                                            FoundPosition block2,                   // 区块2
                                            double meanVal,                         // 平均值
                                            double threshold                        // 判定的阈值
                                            )
        {
            double diffVal = 0;                     // 区块间距离差值
            int diffCount = 0;                      // 相差的行列数的整数部分
            double remainder1 = 0, remainder2 = 0;  // 余数部分(分别是以第n和第n+1行/列为基准的余数)
            if (E_ROW_OR_COL.E_ROW == mode)
            {
                diffVal = Math.Abs((double)block1.Y - (double)block2.Y);
            }
            else if (E_ROW_OR_COL.E_COL == mode)
            {
                diffVal = Math.Abs((double)block1.X - (double)block2.X);
            }
            else
            {
                // 不可能出现的情况
                System.Diagnostics.Trace.Assert(false);
                return -1;
            }
            diffCount = (int)(diffVal / meanVal);
            remainder1 = diffVal - meanVal * diffCount;
            remainder2 = meanVal * (diffCount + 1) - diffVal;
            if (remainder1 < threshold)
            {
                return diffCount;
            }
            else if (remainder2 < threshold)
            {
                return (diffCount + 1);
            }
            else
            {
                // 遇到麻烦了, 可能是阈值选得不合适
                System.Diagnostics.Trace.Assert(false);
                return -1;
            }
        }
    }
}
