using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BMTool
{
    public partial class InputOutputInfoForm : Form
    {
        public DateTime 日期
        {
            get { return _日期; }
            set { _日期 = value; }
        }
        public DateTime _日期;

        public string 名称
        {
            get { return _名称; }
            set { _名称 = value; }
        }
        public string _名称 = "";

        public decimal 单价
        {
            get { return _单价; }
            set { _单价 = value; }
        }
        public decimal _单价;

        public decimal 数量
        {
            get { return _数量; }
            set { _数量 = value; }
        }
        public decimal _数量;

        public decimal 优惠率
        {
            get { return _优惠率; }
            set { _优惠率 = value; }
        }
        public decimal _优惠率;

        public decimal 均摊运费
        {
            get { return _均摊运费; }
            set { _均摊运费 = value; }
        }
        public decimal _均摊运费;

        public InputOutputInfoForm()
        {
            InitializeComponent();
            tbx日期.Text = DateTime.Now.ToShortDateString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ("" != tbx名称.Text)
            {
                this.名称 = tbx名称.Text;
                if (!DateTime.TryParse(tbx日期.Text, out this._日期))
                {
                    MessageBox.Show("日期格式不对!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return;
                }
                else if (!decimal.TryParse(tbx单价.Text, out this._单价))
                {
                    MessageBox.Show("单价 格式不对!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx数量.Text, out this._数量))
                {
                    MessageBox.Show("数量 格式不对!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx优惠率.Text, out this._优惠率))
                {
                    MessageBox.Show("优惠率 格式不对!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx均摊运费.Text, out this._均摊运费))
                {
                    MessageBox.Show("均摊运费 格式不对!");
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            }
        }

        private void InputOutputInfoForm_Load(object sender, EventArgs e)
        {
            tbx名称.Text = 名称;
            tbx单价.Text = 单价.ToString();
            tbx数量.Text = 数量.ToString();
            tbx优惠率.Text = 优惠率.ToString();
            tbx均摊运费.Text = 均摊运费.ToString();
        }

        public List<string> GetDataStringList()
        {
            List<string> dataList = new List<string>();
            string str = this.日期.ToShortDateString();
            dataList.Add(str);
            dataList.Add(this.名称);
            str = this.单价.ToString();
            dataList.Add(str);
            str = this.数量.ToString();
            dataList.Add(str);
            str = this.优惠率.ToString();
            dataList.Add(str);
            str = this.均摊运费.ToString();
            dataList.Add(str);

            return dataList;
        }

        public void SetDataStringList(List<string> dataList)
        {
            System.Diagnostics.Trace.Assert(dataList.Count >= 7);

            DateTime outDateTime;
            DateTime.TryParse(dataList[1], out outDateTime);
            this.日期 = outDateTime;
            this.名称 = dataList[2];
            decimal outVal;
            decimal.TryParse(dataList[3], out outVal);
            this.单价 = outVal;
            decimal.TryParse(dataList[4], out outVal);
            this.数量 = outVal;
            decimal.TryParse(dataList[5], out outVal);
            this.优惠率 = outVal;
            decimal.TryParse(dataList[6], out outVal);
            this.均摊运费 = outVal;
        }
    }
}
