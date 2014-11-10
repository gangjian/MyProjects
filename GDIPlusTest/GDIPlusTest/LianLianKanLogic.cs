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

        // 连连看矩阵的最大行列数定义
        public const int ROW_NUM = 12;
        public const int COL_NUM = 15;

        // 空的区块定义序号为-1
        public const int EMPTY_BLOCK = -1;

        // 区块位置矩阵化阈值系数
        private const double THRESHOLD_MODULUS = 0.3;

        /// <summary>
        /// 区块位置矩阵化处理
        /// </summary>
        public static void Matrixing(ref List<FoundPosition> layoutList)
        {
            // 求所有区块的平均宽高
            double averageWidthVal = 0, averageHeightVal = 0;
            GetAverageWidthAndHeight(layoutList, out averageWidthVal, out averageHeightVal);

            // 异常检测
            System.Diagnostics.Trace.Assert((0 != averageWidthVal) && (0 != averageHeightVal));

            // 根据平均值结果设定阈值
            double threshold_h = averageWidthVal * THRESHOLD_MODULUS;
            double threshold_v = averageHeightVal * THRESHOLD_MODULUS;

            // 排定各区块的行列号
            DeterminateAllRowsOrColsIdxs(E_ROW_OR_COL.E_ROW, layoutList, averageHeightVal, threshold_v);
            DeterminateAllRowsOrColsIdxs(E_ROW_OR_COL.E_COL, layoutList, averageWidthVal, threshold_h);
        }

        /// <summary>
        /// 判断指定的两个区块是否可以被消除
        /// </summary>
        /// <param name="arr">矩阵化后的区块编号数组</param>
        /// <param name="row1">区块1的行号</param>
        /// <param name="col1">区块1的列号</param>
        /// <param name="row2">区块2的行号</param>
        /// <param name="col2">区块2的列号</param>
        public static void BlocksCanBeEliminated(int[,] arr, int row1, int col1, int row2, int col2)
        {
            int row_s;  // 扫描开始行序号
            int row_e;  // 扫描结束行序号
            int col_s;  // 扫描开始列序号
            int col_e;  // 扫描结束列序号

            // 横向扫描(, 第ROW_NUM同理也是位于框外)
            for (int i = -1; i <= ROW_NUM; i++)
            {
                row_s = i + 1;
                if (-1 == i)            // 从第-1行开始, -1表示第0行之上, 连连看图像区域框外的位置
                {
                }
                else if (ROW_NUM == i)  // 表示最后一行(ROW_NUM - 1)之下, 框外的位置
                {
                    ;
                }
                else
                {
                    if (row1 == i)
                    {
                    }
                    else if (row1 > i)
                    {
                    }
                    else
                    {
                    }
                    if (row2 == i)
                    {
                    }
                    else if (row2 > i)
                    {
                    }
                    else
                    {
                    }
                }
                // 判断两个区块垂直投影方向各区块是否都为空
                // 判断水平方向连线各区块是否都为空
            }
            // 纵向扫描
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
                            firstVal = fp.Y;
                            idx = i;
                        }
                        else if (fp.Y < firstVal)   // 判断如果X方向位置更靠左就更新返回索引
                        {
                            firstVal = fp.Y;
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
                            firstVal = fp.X;
                            idx = i;
                        }
                        else if (fp.X < firstVal)   // 判断如果Y方向位置更靠上就更新返回索引
                        {
                            firstVal = fp.X;
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
                            lastVal = fp.Y;
                            idx = i;
                        }
                        else if (fp.Y > lastVal)   // 判断如果X方向位置更靠右就更新返回索引
                        {
                            lastVal = fp.Y;
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
                            lastVal = fp.X;
                            idx = i;
                        }
                        else if (fp.X > lastVal)   // 判断如果Y方向位置更靠下就更新返回索引
                        {
                            lastVal = fp.X;
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
                // System.Diagnostics.Trace.Assert(false);
                return -1;
            }
        }

        /// <summary>
        /// 确定所有区块的行列编号
        /// </summary>
        static void DeterminateAllRowsOrColsIdxs(E_ROW_OR_COL mode, List<FoundPosition> layoutList, double meanVal, double threshold)
        {
            //  1.找到第一个(最上面或最左边)未确定行列号的区块
            int firstIdx = -1;
            int lastIdx = -1;
            int diffCnt = -1;   // 最后一个确定的区块和第一个未确定的区块间的行列数差
            // 直到找不到, 说明所有区块都已确定行列号
            while (-1 != (firstIdx = FindFirstUnconfirmedBlock(mode, layoutList)))
            {
                // 确定它的行号
                // 找到最后一个确定行列号的区块
                lastIdx = FindLastConfirmedBlock(mode, layoutList);
                if (-1 == lastIdx)
                {
                    // 没找到说明这是第一行/列
                    if (E_ROW_OR_COL.E_ROW == mode)
                    {
                        layoutList[firstIdx].Row = 0;
                    }
                    else
                    {
                        layoutList[firstIdx].Col = 0;
                    }
                }
                else
                {
                    // 算出它们之间的行列数之差
                    diffCnt = Get2BlocksRowsOrColsDiff(mode, layoutList[lastIdx], layoutList[firstIdx], meanVal, threshold);
                    if (-1 != diffCnt)
                    {
                        if (E_ROW_OR_COL.E_ROW == mode)
                        {
                            layoutList[firstIdx].Row = layoutList[lastIdx].Row + diffCnt;
                        }
                        else
                        {
                            layoutList[firstIdx].Col = layoutList[lastIdx].Col + diffCnt;
                        }
                    }
                    else
                    {
                        // 没能正确算出行列数差, 距离太远导致误差比较大, 增加阈值再试试
                        System.Diagnostics.Trace.Assert(false);
                    }
                }

                // 确定跟它处于同一行的其它各区块
                foreach (FoundPosition fp in layoutList)
                {
                    if ((E_ROW_OR_COL.E_ROW == mode) && (-1 == fp.Row))               // 该区块还未确定行番号
                    {
                        if (0 == Get2BlocksRowsOrColsDiff(mode, layoutList[firstIdx], fp, meanVal, threshold))
                        {
                            fp.Row = layoutList[firstIdx].Row;
                        }
                    }
                    else if ((E_ROW_OR_COL.E_COL == mode) && (-1 == fp.Col))
                    {
                        if (0 == Get2BlocksRowsOrColsDiff(mode, layoutList[firstIdx], fp, meanVal, threshold))
                        {
                            fp.Col = layoutList[firstIdx].Col;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 判断纵向连续若干区块是否都为空
        /// </summary>
        /// <param name="row_s">起始行序号</param>
        /// <param name="row_e">结束行序号</param>
        /// <param name="col">列序号</param>
        /// <param name="arr">区块序号矩阵</param>
        static bool VerticalBlocksAllEmpty(int row_s, int row_e, int col, int[,] arr)
        {
            System.Diagnostics.Trace.Assert(null != arr);
            int max_rows = arr.GetLength(0);
            int max_cols = arr.GetLength(1);
            System.Diagnostics.Trace.Assert((row_s >= 0) && (row_s <= row_e) && (row_e <= max_rows) && (col >= 0) && (col <= max_cols));

            for (int i = row_s; i <= row_e; i++)
            {
                if (EMPTY_BLOCK != arr[i, col])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row">行序号</param>
        /// <param name="col_s">起始列序号</param>
        /// <param name="col_e">结束列序号</param>
        /// <param name="arr">区块序号矩阵</param>
        /// <returns></returns>
        static bool HorizontalBlocksAllEmpty(int row, int col_s, int col_e, int[,] arr)
        {
            System.Diagnostics.Trace.Assert(null != arr);
            int max_rows = arr.GetLength(0);
            int max_cols = arr.GetLength(1);
            System.Diagnostics.Trace.Assert((col_s >= 0) && (col_s <= col_e) && (col_e <= max_cols) && (row >= 0) && (row <= max_cols));

            for (int i = col_s; i <= col_e; i++)
            {
                if (EMPTY_BLOCK != arr[row, i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
