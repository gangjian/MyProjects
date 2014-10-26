using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VisualizeMyLife
{
    public partial class DailyRecordForm : Form
    {
        public DailyRecordForm(DataInfo lastDataInfo)
        {
            InitializeComponent();

            if (null != lastDataInfo)
            {
                loadLastRecord(lastDataInfo);
            }
        }

        private double _income1, _income2;                  // 收益
        private double _historyIncome1, _historyIncome2;    // 历史累计收益
        private double _totalAssets1, _totalAssets2;        // 总资产
        private double _rate1, _rate2;                      // 收益率
        private double _bodyWeight;                         // 体重

        private void buttonOutputText_Click(object sender, EventArgs e)
        {
            string divider = "----------------+------------------------------";

            string dispStr = "\t\t昨日收益\t累计收益\t总资产\t收益率\r\n";
            dispStr += (divider + "\r\n");
            dispStr += ("合计\t\t|" + (_income1 + _income2).ToString() + "\t"
                                    + (_historyIncome1 + _historyIncome2).ToString() + "\t"
                                    + (_totalAssets1 + _totalAssets2).ToString()   );
            dispStr += "\r\n" + divider + "\r\n";
            dispStr += (CommonDefine.D_NAME_1 + "\t\t|" + _income1.ToString() + "\t"
                                      + _historyIncome1.ToString() + "\t"
                                      + _totalAssets1.ToString() + "\t"
                                      + _rate1.ToString() + "%");
            dispStr += "\r\n" + divider + "\r\n";

            dispStr += (CommonDefine.D_NAME_2 + "\t\t|" + _income2.ToString() + "\t"
                                        + _historyIncome2.ToString() + "\t"
                                        + _totalAssets2.ToString() + "\t"
                                        + _rate2.ToString() + "%");
            dispStr += "\r\n" + divider + "\r\n";

            if (double.TryParse(tbBodyWeight.Text, out _bodyWeight))
            {
                dispStr += "体重\t\t|" + _bodyWeight.ToString() + "kg";
                dispStr += "\r\n" + divider + "\r\n";
            }

            textBoxWiz.Text = dispStr;
            this.Invalidate();
        }

        private void buttonSaveData2File_Click(object sender, EventArgs e)
        {
            DataInfo dataInfo = new DataInfo();
            dataInfo._dateTime = DateTime.Now;
            dataInfo._bodyWeight = _bodyWeight;
            dataInfo._historyIncome1 = _historyIncome1;
            dataInfo._historyIncome2 = _historyIncome2;
            dataInfo._income1 = _income1;
            dataInfo._income2 = _income2;
            dataInfo._rate1 = _rate1;
            dataInfo._rate2 = _rate2;
            dataInfo._totalAssets1 = _totalAssets1;
            dataInfo._totalAssets2 = _totalAssets2;

            string appPath = Application.ExecutablePath;
            appPath = appPath.Remove(appPath.LastIndexOf('\\') + 1);
            ClassDataFileManager dfMng = new ClassDataFileManager(appPath + "data.log");
            List<DataInfo> list = dfMng.ReadDataList();
            for (int i = 0; i < list.Count; i++)
            {
                DataInfo di = list[i];
                if ((di._dateTime.Year == DateTime.Now.Year)
                    && (di._dateTime.Month == DateTime.Now.Month)
                    && (di._dateTime.Day == DateTime.Now.Day))
                {
                    // already exist
                    string str = "记录已经存在, 要更新吗?";
                    if (DialogResult.Yes == MessageBox.Show(str, "", MessageBoxButtons.YesNo))
                    {
                        // overwrite
                    }
                    else
                    {
                        // cancel
                    }
                }
            }
            dfMng.WriteDataToFile(dataInfo);
            MessageBox.Show("保存完成!");
        }

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            buttonOutputText.Enabled = false;
            buttonSaveData2File.Enabled = false;

            // 收益
            if (!double.TryParse(tbIncome1.Text, out _income1))
            {
                tbIncome1.Focus();
                return;
            }
            if (!double.TryParse(tbIncome2.Text, out _income2))
            {
                tbIncome2.Focus();
                return;
            }
            labelTotalIncome.Text = "总收益:" + (_income1 + _income2).ToString();

            // 累计收益
            if (!double.TryParse(tbHistoryIncome1.Text, out _historyIncome1))
            {
                tbHistoryIncome1.Focus();
                return;
            }
            if (!double.TryParse(tbHistoryIncome2.Text, out _historyIncome2))
            {
                tbHistoryIncome2.Focus();
                return;
            }
            labelHistoryIncome.Text = "累计收益:" + (_historyIncome1 + _historyIncome2).ToString();

            // 总资产
            if (!double.TryParse(tbTotalAssets1.Text, out _totalAssets1))
            {
                tbTotalAssets1.Focus();
                return;
            }
            if (!double.TryParse(tbTotalAssets2.Text, out _totalAssets2))
            {
                tbTotalAssets2.Focus();
                return;
            }
            labelTotalAssets.Text = "总资产:" + (_totalAssets1 + _totalAssets2).ToString();

            // 收益率
            if (!double.TryParse(tbRate1.Text, out _rate1))
            {
                tbRate1.Focus();
                return;
            }
            if (!double.TryParse(tbRate2.Text, out _rate2))
            {
                tbRate2.Focus();
                return;
            }

            buttonOutputText.Enabled = true;
            buttonSaveData2File.Enabled = true;
        }

        private void loadLastRecord(DataInfo lastDataInfo)
        {
            this.tbIncome1.Text = lastDataInfo._income1.ToString();
            this.tbIncome2.Text = lastDataInfo._income2.ToString();
            this.tbBodyWeight.Text = lastDataInfo._bodyWeight.ToString();
            this.tbHistoryIncome1.Text = lastDataInfo._historyIncome1.ToString();
            this.tbHistoryIncome2.Text = lastDataInfo._historyIncome2.ToString();
            this.tbRate1.Text = lastDataInfo._rate1.ToString();
            this.tbRate2.Text = lastDataInfo._rate2.ToString();
            this.tbTotalAssets1.Text = lastDataInfo._totalAssets1.ToString();
            this.tbTotalAssets2.Text = lastDataInfo._totalAssets2.ToString();
        }
    }
}
