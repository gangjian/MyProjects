using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GDIPlusTest.GameRobots.Robot1
{
    /// <summary>
    /// 单个区块位于矩阵内的情报信息
    /// </summary>
    public class BlockLayoutInfo
    {
        public int row = -1;                // 行号
        public int col = -1;                // 列号
        public bool isEliminated = false;   // 是否已经被消掉了?

        public BlockLayoutInfo(int r, int c, bool ie = false)
        {
            row = r;
            col = c;
            isEliminated = ie;
        }
    }

    /// <summary>
    /// 区块按序号排列后的集合
    /// </summary>
    public class BlocksLayoutSet
    {
        public List<BlockLayoutInfo> layoutList = new List<BlockLayoutInfo>();      // 同一种图案区块的位置情报列表
    }

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

        // 连连看区块图案种类最大数
        public const int MAX_BLOCK_TYPE_COUNT = 100;

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
            DeterminateAllRowsOrColsIdxs(E_OP_MODE.E_ROW_OP, layoutList, averageHeightVal, threshold_v);
            DeterminateAllRowsOrColsIdxs(E_OP_MODE.E_COL_OP, layoutList, averageWidthVal, threshold_h);
        }

        /// <summary>
        /// 矩阵区块整理, 同样图案的区块位置信息放到一起做成list
        /// </summary>
        public static BlocksLayoutSet[] MatrixArrangement(int[,] arr)
        {
            BlocksLayoutSet[] blockInfoArray = new BlocksLayoutSet[MAX_BLOCK_TYPE_COUNT];
            for (int i = 0; i < MAX_BLOCK_TYPE_COUNT; i++)
            {
                blockInfoArray[i] = new BlocksLayoutSet();
            }
            int row_num = arr.GetLength(0);
            int col_num = arr.GetLength(1);
            for (int i = 0; i < row_num; i++)
            {
                for (int j = 0; j < col_num; j++)
                {
                    if (EMPTY_BLOCK == arr[i, j])
                    {
                        continue;
                    }
                    int subImgIdx = arr[i, j];
                    System.Diagnostics.Trace.Assert((subImgIdx >= 0) && (subImgIdx < MAX_BLOCK_TYPE_COUNT));
                    BlocksLayoutSet bls = blockInfoArray[subImgIdx];
                    BlockLayoutInfo bli = new BlockLayoutInfo(i, j);
                    bls.layoutList.Add(bli);
                }
            }

            return blockInfoArray;
        }

        /// <summary>
        /// 找出所有可消去的区块对
        /// </summary>
        /// <param name="blocksSetArray"></param>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static List<int[]> GetAllEliminablePairs(BlocksLayoutSet[] blocksSetArray, int[,] arr)
        {
            List<int[]> retList = null;

            for (int i = 0; i < MAX_BLOCK_TYPE_COUNT; i++)
            {
                BlocksLayoutSet bls = blocksSetArray[i];
                if (bls.layoutList.Count >= 2)
                {
                    for (int m = 0; m < bls.layoutList.Count - 1; m++)
                    {
                        // 找到第一个未被消去的区块
                        if (bls.layoutList[m].isEliminated)
                        {
                            continue;
                        }
                        for (int n = m + 1; n < bls.layoutList.Count; n++)
                        {
                            if (bls.layoutList[n].isEliminated)
                            {
                                continue;
                            }
                            // 判断两个区块是否可以消去
                            if (IsBlocksEliminable(arr, bls.layoutList[m].row, bls.layoutList[m].col,
                                                        bls.layoutList[n].row, bls.layoutList[n].col))
                            {
                                int[] findArr = new int[4] { bls.layoutList[m].row, bls.layoutList[m].col,
                                                            bls.layoutList[n].row, bls.layoutList[n].col};
                                if (null == retList)
                                {
                                    retList = new List<int[]>();
                                }
                                retList.Add(findArr);
                            }
                        }
                    }
                }
            }

            return retList;
        }

        /// <summary>
        /// 找到第一组可以消去的区块对
        /// </summary>
        public static int[] GetFirstEliminablePairs(BlocksLayoutSet[] blocksSetArray, int[,] arr)
        {
            System.Diagnostics.Trace.Assert(blocksSetArray.Length == MAX_BLOCK_TYPE_COUNT);
            List<int[]> findList = GetAllEliminablePairs(blocksSetArray, arr);
            if (null == findList)
            {
                return null;
            }
            return findList[0];
        }

        /// <summary>
        /// 找到下一组(相对于lastPairs)可消去的区块对
        /// </summary>
        /// <param name="blocksSetArray"></param>
        /// <param name="arr"></param>
        /// <param name="lastPairs"></param>
        /// <returns></returns>
        public static List<int[]> FindNextEliminatePath(ref BlocksLayoutSet[] blocksSetArray, ref int[,] arr, int[] lastPairs)
        {
            // 列出当前所有可消去的区块对, 找到等于lastPairs的区块对, 再找它的下一对
            List<int[]> findList = GetAllEliminablePairs(blocksSetArray, arr);
            if (null == findList)
            {
                return null;
            }
            for (int i = 0; i < findList.Count; i++)
            {
                if ((       findList[i][0] == lastPairs[0]
                        &&  findList[i][1] == lastPairs[1]
                        &&  findList[i][2] == lastPairs[2]
                        &&  findList[i][3] == lastPairs[3])
                    && (i < findList.Count - 1))
                {
                    // 找到下一对可消去的区块对
                    int[] nextPairs = findList[i + 1];
                    // 将其消去
                    DeleteEliminatableBlocks(nextPairs[0], nextPairs[1], nextPairs[2], nextPairs[3], ref arr, ref blocksSetArray);
                    // 剩余的区块取第一条路径, 与前面消去的区块对一并作为路径返回
                    List<int[]> newPath = GetFirstEliminateSeq(ref arr, ref blocksSetArray);
                    newPath.Insert(0, nextPairs);

                    return newPath;
                }
            }

            return null;
        }

        /// <summary>
        /// (在矩阵里)去掉一组被消掉的区块
        /// </summary>
        public static void DeleteEliminatableBlocks(int r1, int c1, int r2, int c2, ref int[,] arr2, ref BlocksLayoutSet[] blsArray)
        {
            int idx = 0;
            foreach (BlocksLayoutSet bls in blsArray)
            {
                foreach (BlockLayoutInfo bli in bls.layoutList)
                {
                    if ((r1 == bli.row && c1 == bli.col)
                        || (r2 == bli.row && c2 == bli.col))
                    {
                        // System.Diagnostics.Trace.Assert(false == bli.isEliminated);
                        bli.isEliminated = true;
                        arr2[bli.row, bli.col] = LianLianKanLogic.EMPTY_BLOCK;
                    }
                }
                idx += 1;
            }
        }

        /// <summary>
        /// (在矩阵里)恢复一组被消掉的区块
        /// </summary>
        public static void RecoverEliminatedBlocks(int r1, int c1, int r2, int c2, ref int[,] arr2, ref BlocksLayoutSet[] blsArray)
        {
            int idx = 0;
            foreach (BlocksLayoutSet bls in blsArray)
            {
                foreach (BlockLayoutInfo bli in bls.layoutList)
                {
                    if ((r1 == bli.row && c1 == bli.col)
                        || (r2 == bli.row && c2 == bli.col))
                    {
                        // System.Diagnostics.Trace.Assert(bli.isEliminated);
                        bli.isEliminated = false;
                        arr2[bli.row, bli.col] = idx;
                    }
                }
                idx += 1;
            }
        }

        /// <summary>
        /// 取得第一条(最优先)消去操作的序列
        /// </summary>
        public static List<int[]> GetFirstEliminateSeq(ref int[,] arr2, ref BlocksLayoutSet[] blsArray)
        {
            List<int[]> retSeq = new List<int[]>();
            int[] retArr;
            while (null != (retArr = GetFirstEliminablePairs(blsArray, arr2)))
            {
                System.Diagnostics.Trace.Assert(4 == retArr.Length);
                retSeq.Add(retArr);

                // 记得要把将要消去的两块从矩阵和区块情报列表里去掉, 才能继续进行下一轮查找
                DeleteEliminatableBlocks(retArr[0], retArr[1], retArr[2], retArr[3], ref arr2, ref blsArray);
            }

            return retSeq;
        }

        public static List<int[]> GetNextEliminateSeq(ref int[,] arr2, ref BlocksLayoutSet[] blsArray, List<int[]> lastSeq)
        {
            List<int[]> retSeq = new List<int[]>();
            System.Diagnostics.Trace.Assert(0 != lastSeq.Count);
            List<int[]> nextPath = null;

            do
            {
                if (1 == lastSeq.Count)
                {
                    //MessageBox.Show("已经退到最上一层节点了, 应该不会发生这样的情况吧?");
                    break;
                }
                // 去掉最后一个节点(dead end, 死胡同)
                int[] lastNode = lastSeq[lastSeq.Count - 1];
                // 恢复该节点消掉的区块
                RecoverEliminatedBlocks(lastNode[0], lastNode[1], lastNode[2], lastNode[3], ref arr2, ref blsArray);
                lastSeq.RemoveAt(lastSeq.Count - 1);

                // 剩下的最后一个节点
                lastNode = lastSeq[lastSeq.Count - 1];
                RecoverEliminatedBlocks(lastNode[0], lastNode[1], lastNode[2], lastNode[3], ref arr2, ref blsArray);
                nextPath = FindNextEliminatePath(ref blsArray, ref arr2, lastNode);
                if (null != nextPath)
                {
                    // 去掉现在的最后一个节点
                    lastSeq.RemoveAt(lastSeq.Count - 1);
                    // 将lastSeq与nextPath相接
                    lastSeq.AddRange(nextPath);
                    return lastSeq;
                }
            } while (null == nextPath);

            return null;
        }

        /// <summary>
        /// 找出当前局面下最优的(出口最多的)消去对
        /// </summary>
        static public int[] FindOptimalEliminatePairs(int[,] arr2, BlocksLayoutSet[] blsArray)
        {
            // 得到当前所有可消去的区块对
            List<int[]> optList = LianLianKanLogic.GetAllEliminablePairs(blsArray, arr2);
            if (null == optList)
            {
                return null;
            }
            // 评估每一种选项消去后, 下一步的出口数量(可消去区块对数), 每次只选出口最多的
            // 暂时先只考虑最简单的情况, 只判断下一步的出口数(以后要实现对下n步的判断), 并保留出口最多的m条路径
            int maxOptCnt = 0;
            int maxOptIdx = 0;
            for (int i = 0; i < optList.Count; i++)
            {
                int[] opt = optList[i];
                int[,] arrTmp = (int[,])arr2.Clone();
                BlocksLayoutSet[] blsArrTmp = blsArrDeepCopy(blsArray);

                LianLianKanLogic.DeleteEliminatableBlocks(opt[0], opt[1], opt[2], opt[3], ref arrTmp, ref blsArrTmp);
                List<int[]> optNextList = LianLianKanLogic.GetAllEliminablePairs(blsArrTmp, arrTmp);
                if (null == optNextList)
                {
                    // 下一步没有可消去的区块了
                    // 可能是都消完了, 结束了
                    if (0 == GetLeftBlocksNum(arrTmp))
                    {
                        return optList[i];
                    }
                    // 也可能是走到死胡同里了
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    int nextOptCnt = optNextList.Count;
                    if (nextOptCnt > maxOptCnt)
                    {
                        maxOptCnt = nextOptCnt;
                        maxOptIdx = i;
                    }
                }
            }

            return optList[maxOptIdx];
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
        enum E_OP_MODE
        {
            E_ROW_OP,
            E_COL_OP
        };

        /// <summary>
        /// 找出第一个(最上侧/最左侧)未被确定行列号的区块
        /// </summary>
        static int FindFirstUnconfirmedBlock(E_OP_MODE op_mode, List<FoundPosition> layoutList)
        {
            int idx = -1;       // 返回找到区块在list中的索引值
            int firstVal = -1;  // 最值
            int i = 0;
            foreach (FoundPosition fp in layoutList)
            {
                if (E_OP_MODE.E_ROW_OP == op_mode)
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
                else if (E_OP_MODE.E_COL_OP == op_mode) // E_OP_MODE.E_COL_OP
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
        static int FindLastConfirmedBlock(E_OP_MODE op_mode, List<FoundPosition> layoutList)
        {
            int idx = -1;
            int lastVal = -1;  // 最值
            int i = 0;
            foreach (FoundPosition fp in layoutList)
            {
                if (E_OP_MODE.E_ROW_OP == op_mode)
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
                else if (E_OP_MODE.E_COL_OP == op_mode) // E_OP_MODE.E_COL_OP
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
        static int Get2BlocksRowsOrColsDiff(E_OP_MODE op_mode,                     // 行或列
                                            FoundPosition block1,                   // 区块1
                                            FoundPosition block2,                   // 区块2
                                            double meanVal,                         // 平均值
                                            double threshold                        // 判定的阈值
                                            )
        {
            double diffVal = 0;                     // 区块间距离差值
            int diffCount = 0;                      // 相差的行列数的整数部分
            double remainder1 = 0, remainder2 = 0;  // 余数部分(分别是以第n和第n+1行/列为基准的余数)
            if (E_OP_MODE.E_ROW_OP == op_mode)
            {
                diffVal = Math.Abs((double)block1.Y - (double)block2.Y);
            }
            else if (E_OP_MODE.E_COL_OP == op_mode)
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
                // return -1;
                if (remainder1 < remainder2)
                {
                    return diffCount;
                }
                else
                {
                    return (diffCount + 1);
                }
            }
        }

        /// <summary>
        /// 确定所有区块的行列编号
        /// </summary>
        static void DeterminateAllRowsOrColsIdxs(E_OP_MODE op_mode, List<FoundPosition> layoutList, double meanVal, double threshold)
        {
            //  1.找到第一个(最上面或最左边)未确定行列号的区块
            int firstIdx = -1;
            int lastIdx = -1;
            int diffCnt = -1;   // 最后一个确定的区块和第一个未确定的区块间的行列数差
            // 直到找不到, 说明所有区块都已确定行列号
            while (-1 != (firstIdx = FindFirstUnconfirmedBlock(op_mode, layoutList)))
            {
                // 确定它的行号
                // 找到最后一个确定行列号的区块
                lastIdx = FindLastConfirmedBlock(op_mode, layoutList);
                if (-1 == lastIdx)
                {
                    // 没找到说明这是第一行/列
                    if (E_OP_MODE.E_ROW_OP == op_mode)
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
                    diffCnt = Get2BlocksRowsOrColsDiff(op_mode, layoutList[lastIdx], layoutList[firstIdx], meanVal, threshold);
                    if (-1 != diffCnt)
                    {
                        if (E_OP_MODE.E_ROW_OP == op_mode)
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
                        // System.Diagnostics.Trace.Assert(false);
                    }
                }

                // 确定跟它处于同一行的其它各区块
                foreach (FoundPosition fp in layoutList)
                {
                    if ((E_OP_MODE.E_ROW_OP == op_mode) && (-1 == fp.Row))               // 该区块还未确定行番号
                    {
                        if (0 == Get2BlocksRowsOrColsDiff(op_mode, layoutList[firstIdx], fp, meanVal, threshold))
                        {
                            fp.Row = layoutList[firstIdx].Row;
                        }
                    }
                    else if ((E_OP_MODE.E_COL_OP == op_mode) && (-1 == fp.Col))
                    {
                        if (0 == Get2BlocksRowsOrColsDiff(op_mode, layoutList[firstIdx], fp, meanVal, threshold))
                        {
                            fp.Col = layoutList[firstIdx].Col;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 判断纵向连续若干区块(不包括端点)是否都为空
        /// </summary>
        /// <param name="row_s">起始行序号</param>
        /// <param name="row_e">结束行序号</param>
        /// <param name="col">列序号</param>
        /// <param name="arr">区块序号矩阵</param>
        static bool VerticalBlocksAllEmpty(int row_s, int row_e, int col, int[,] arr)
        {
            System.Diagnostics.Trace.Assert(null != arr);
            if (row_s > row_e)
            {
                int tmp = row_s;
                row_s = row_e;
                row_e = tmp;
            }
            for (int i = row_s + 1; i < row_e; i++)
            {
                if (EMPTY_BLOCK != arr[i, col])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断横向连续若干区块(不包括端点)是否都为空
        /// </summary>
        /// <param name="row">行序号</param>
        /// <param name="col_s">起始列序号</param>
        /// <param name="col_e">结束列序号</param>
        /// <param name="arr">区块序号矩阵</param>
        /// <returns></returns>
        static bool HorizontalBlocksAllEmpty(int row, int col_s, int col_e, int[,] arr)
        {
            System.Diagnostics.Trace.Assert(null != arr);
            if (col_s > col_e)
            {
                int tmp = col_s;
                col_s = col_e;
                col_e = tmp;
            }
            for (int i = col_s + 1; i < col_e; i++)
            {
                if (EMPTY_BLOCK != arr[row, i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断指定的两个区块是否可以被消除
        /// </summary>
        /// <param name="arr">矩阵化后的区块编号数组</param>
        /// <param name="row1">区块1的行号</param>
        /// <param name="col1">区块1的列号</param>
        /// <param name="row2">区块2的行号</param>
        /// <param name="col2">区块2的列号</param>
        public static bool IsBlocksEliminable(int[,] arr, int row1, int col1, int row2, int col2)
        {
            // 横向扫描
            for (int i = -1; i <= ROW_NUM; i++)
            {
                // 判断两个区块垂直投影方向各区块是否都为空
                if (!VerticalBlocksAllEmpty(i, row1, col1, arr)
                    || !VerticalBlocksAllEmpty(i, row2, col2, arr))
                {
                    continue;
                }
                if ((-1 == i) || (ROW_NUM == i))
                {
                    return true;
                }
                // 判断水平方向连线各区块是否都为空
                if (!HorizontalBlocksAllEmpty(i, col1, col2, arr))
                {
                    continue;
                }
                // 还要判断投影点是否是指定的区块本身, 不是的话也要为空
                if ((i != row1) && (EMPTY_BLOCK != arr[i, col1]))
                {
                    continue;
                }
                if ((i != row2) && (EMPTY_BLOCK != arr[i, col2]))
                {
                    continue;
                }
                return true;
            }
            // 纵向扫描
            for (int j = -1; j <= COL_NUM; j++)
            {
                // 判断两个区块水平投影方向各区块是否都为空
                if (!HorizontalBlocksAllEmpty(row1, j, col1, arr)
                    || !HorizontalBlocksAllEmpty(row2, j, col2, arr))
                {
                    continue;
                }
                if (-1 == j || COL_NUM == j)
                {
                    return true;
                }
                // 判断垂直方向连线各区块是否都为空
                if (!VerticalBlocksAllEmpty(row1, row2, j, arr))
                {
                    continue;
                }
                // 还有判断投影点是否是指定的区块本身, 不是的话也要为空
                if ((j != col1) && (EMPTY_BLOCK != arr[row1, j]))
                {
                    continue;
                }
                if ((j != col2) && (EMPTY_BLOCK != arr[row2, j]))
                {
                    continue;
                }
                return true;
            }

            return false;
        }

        static BlocksLayoutSet[] blsArrDeepCopy(BlocksLayoutSet[] arr_in)
        {
            BlocksLayoutSet[] arr_out = new BlocksLayoutSet[arr_in.Length];
            int idx = 0;
            foreach (BlocksLayoutSet bls in arr_in)
            {
                arr_out[idx] = new BlocksLayoutSet();
                foreach (BlockLayoutInfo bli in arr_in[idx].layoutList)
                {
                    BlockLayoutInfo bli_new = new BlockLayoutInfo(bli.row, bli.col, bli.isEliminated);
                    arr_out[idx].layoutList.Add(bli_new);
                }
                idx++;
            }

            return arr_out;
        }

        /// <summary>
        /// 检查是否所有的区块都被消除了
        /// </summary>
        /// <returns></returns>
        public static int GetLeftBlocksNum(int[,] arr)
        {
            int retNum = 0;
            int rowNum = arr.GetLength(0);
            int colNum = arr.GetLength(1);
            for (int i = 0; i < rowNum; i++)
            {
                for (int j = 0; j < colNum; j++)
                {
                    if (LianLianKanLogic.EMPTY_BLOCK != arr[i, j])
                    {
                        retNum += 1;
                    }
                }
            }
            return retNum;
        }

    }
}
