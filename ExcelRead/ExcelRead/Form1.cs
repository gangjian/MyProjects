using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Excel;

namespace ExcelRead
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择Excel文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenGoods_Click(object sender, EventArgs e)
        {
            textBox1.Text = OpenExcelFile();
        }

        private void btnOpenExport_Click(object sender, EventArgs e)
        {
            textBox2.Text = OpenExcelFile();
        }

        private void btnOpenClose_Click(object sender, EventArgs e)
        {
            textBox3.Text = OpenExcelFile();
        }

        string OpenExcelFile()
        {
            string retName = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files|*.xlsx|Excel files|*.xls";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                retName = openFileDialog.FileName;
            }
            return retName;
        }

        /// <summary>
        /// 读选择Excel文件的内容
        /// </summary>
        /// <param name="fullName">读取的Excel文件名</param>
        List<List<string>> ReadExcel(string fullName)
        {
            List<List<string>> readSheetsContents = new List<List<string>>();
            if (string.Empty == fullName)
            {
                return readSheetsContents;
            }
            FileInfo fi = new FileInfo(fullName);
            if (!fi.Exists)
            {
                // 文件不存在
                MessageBox.Show(fullName + " 不存在!");
                return readSheetsContents;
            }

            Microsoft.Office.Interop.Excel.Application ExcelApp
                                        = new Microsoft.Office.Interop.Excel.Application();
            ExcelApp.Visible = false;
            try
            {
                Workbook wb = ExcelApp.Workbooks.Open(fullName);    // 打开workbook
                if (null != wb)
                {
                    // 遍历各个sheet
                    foreach (Worksheet ws in wb.Worksheets)
                    {
                        List<string> sheetContents = new List<string>();
                        int row = 1;
                        int col = 1;
                        for (row = 1; row < 65536; row++)
                        {
                            string rowContents = "";
                            for (col = 1; col < 256; col++)
                            {
                                Range range = ws.Cells[row, col];
                                string cellContents = range.Text;
                                if (string.Empty == cellContents)
                                {
                                    break;
                                }
                                else
                                {
                                    rowContents += (cellContents + ",");
                                }
                            }
                            if (1 == col)
                            {
                                break;
                            }
                            else
                            {
                                sheetContents.Add(rowContents);
                            }
                        }

                        readSheetsContents.Add(sheetContents);
                    }

                    wb.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ExcelApp.Quit();
            }

            return readSheetsContents;
        }

        void ShowReadLog(List<List<string>> readSheetsContents)
        {
            tbxLog.Clear();
            int sheetIdx = 0;
            foreach (List<string> sheetContents in readSheetsContents)
            {
                sheetIdx++;
                if (0 == sheetContents.Count)
                {
                    continue;
                }
                tbxLog.AppendText("\r\n\r\n");
                tbxLog.AppendText("Sheet: " + sheetIdx.ToString() + "\r\n");
                int rowIdx = 0;
                foreach (string rowContents in sheetContents)
                {
                    rowIdx++;
                    tbxLog.AppendText(rowContents + "\r\n");
                    if (1 == rowIdx)
                    {
                        tbxLog.AppendText("-----------------------------\r\n");
                    }
                }
            }
        }

        /// <summary>
        /// "ReadGoods"按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReadGoods_Click(object sender, EventArgs e)
        {
            // 读取Excel文件
            List<List<string>> readSheetsContents = ReadExcel(textBox1.Text);
            // 在tbxLog上显示读取的内容
            ShowReadLog(readSheetsContents);

            // 导入结构体列表
            List<GoodsStruct> goodsList = new List<GoodsStruct>();
            foreach (List<string> sheetContents in readSheetsContents)
            {
                int rowIdx = 0;
                foreach (string rowContents in sheetContents)
                {
                    rowIdx++;
                    // 第一行标题越过
                    if (1 == rowIdx)
                    {
                        continue;
                    }
                    string rowStr = rowContents;
                    // 去掉末尾的逗号
                    if (rowStr.EndsWith(","))
                    {
                        rowStr = rowStr.Remove(rowContents.Length - 1).Trim();
                    }
                    // 用逗号区分各个字段
                    string[] fieldsArray = rowStr.Split(',');
                    if (fieldsArray.Length >= 4)
                    {
                        GoodsStruct goods = new GoodsStruct();
                        int.TryParse(fieldsArray[0], out goods.goods_id);               // 商品编号
                        goods.goods_name = fieldsArray[1];                              // 商品名称
                        decimal.TryParse(fieldsArray[2], out goods.goods_price_jpy);    // 日元单价
                        float.TryParse(fieldsArray[3], out goods.goods_weight);         // 重量

                        goodsList.Add(goods);
                    }
                }
            }

            PrintGoodsList2File(goodsList, "D:\\goodsList.csv");
            return;
        }

        private void btnReadExport_Click(object sender, EventArgs e)
        {
            // 读取Excel文件
            List<List<string>> readSheetsContents = ReadExcel(textBox2.Text);
            // 在tbxLog上显示读取的内容
            ShowReadLog(readSheetsContents);

            // 导入结构体列表
            List<ExportStruct> exportList = new List<ExportStruct>();
            foreach (List<string> sheetContents in readSheetsContents)
            {
                int rowIdx = 0;
                foreach (string rowContents in sheetContents)
                {
                    rowIdx++;
                    // 第一行标题越过
                    if (1 == rowIdx)
                    {
                        continue;
                    }
                    string rowStr = rowContents;
                    // 去掉末尾的逗号
                    if (rowStr.EndsWith(","))
                    {
                        rowStr = rowStr.Remove(rowContents.Length - 1).Trim();
                    }
                    // 用逗号区分各个字段
                    string[] fieldsArray = rowStr.Split(',');
                    if (fieldsArray.Length >= 4)
                    {
                        ExportStruct export = new ExportStruct();
                        export.agent_name = fieldsArray[0];                             // 代理名称
                        int.TryParse(fieldsArray[1], out export.goods_id);              // 商品编号
                        export.goods_name = fieldsArray[2];                             // 商品名称
                        int.TryParse(fieldsArray[3], out export.goods_number);          // 数量

                        exportList.Add(export);
                    }
                }
            }

            return;
        }

        private void btnReadClose_Click(object sender, EventArgs e)
        {
            // 读取Excel文件
            List<List<string>> readSheetsContents = ReadExcel(textBox3.Text);
            // 在tbxLog上显示读取的内容
            ShowReadLog(readSheetsContents);

            // 导入结构体列表
            List<CloseStruct> closeList = new List<CloseStruct>();
            foreach (List<string> sheetContents in readSheetsContents)
            {
                int rowIdx = 0;
                foreach (string rowContents in sheetContents)
                {
                    rowIdx++;
                    // 第一行标题越过
                    if (1 == rowIdx)
                    {
                        continue;
                    }
                    string rowStr = rowContents;
                    // 去掉末尾的逗号
                    if (rowStr.EndsWith(","))
                    {
                        rowStr = rowStr.Remove(rowContents.Length - 1).Trim();
                    }
                    // 用逗号区分各个字段
                    string[] fieldsArray = rowStr.Split(',');
                    if (fieldsArray.Length >= 7)
                    {
                        CloseStruct close = new CloseStruct();
                        int.TryParse(fieldsArray[0], out close.close_id);               // close_id
                        close.agent_name = fieldsArray[1];                              // 代理名称
                        int.TryParse(fieldsArray[2], out close.goods_id);               // 商品编号
                        close.goods_name = fieldsArray[3];                              // 商品名称
                        int.TryParse(fieldsArray[4], out close.goods_number);           // 数量
                        decimal.TryParse(fieldsArray[5], out close.total_cost);         // 总价
                        decimal.TryParse(fieldsArray[6], out close.present_price_cny);  // 赠品总价

                        closeList.Add(close);
                    }
                }
            }

            return;
        }

        void PrintGoodsList2File(List<GoodsStruct> goodsList, string saveFileName)
        {
            StreamWriter sw = new StreamWriter(saveFileName, false, Encoding.Unicode);
            if (null != sw)
            {
                foreach (GoodsStruct gs in goodsList)
                {
                    string wtLine = gs.goods_id.ToString() + "\t";       // 商品编号
                    wtLine += gs.goods_name + "\t";                      // 商品名称
                    wtLine += gs.goods_price_jpy.ToString() + "\t";      // 日元单价
                    wtLine += gs.goods_weight.ToString() + "\t";         // 重量

                    sw.WriteLine(wtLine);
                }
                sw.Close();
            }
        }

    }
}
