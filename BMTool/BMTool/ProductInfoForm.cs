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
    public partial class ProductInfoForm : Form
    {
        public string 名称
        {
            get { return m_名称; }
            set { m_名称 = value; }
        }
        public string m_名称 = "";

        public DateTime 日期
        {
            get { return m_日期; }
            set { m_日期 = value; }
        }
        public DateTime m_日期;

        public string 店铺
        {
            get { return m_店铺; }
            set { m_店铺 = value; }
        }
        public string m_店铺;

        public string 型号
        {
            get { return m_型号; }
            set { m_型号 = value; }
        }
        public string m_型号;

        public string 货号
        {
            get { return m_货号; }
            set { m_货号 = value; }
        }
        public string m_货号;

        public decimal 单价
        {
            get { return m_单价; }
            set { m_单价 = value; }
        }
        public decimal m_单价;

        public decimal 税率
        {
            get { return m_税率; }
            set { m_税率 = value; }
        }
        public decimal m_税率;

        public decimal 税后单价
        {
            get { return m_税后单价; }
            set { m_税后单价 = value; }
        }
        public decimal m_税后单价;

        public decimal 汇率
        {
            get { return m_汇率; }
            set { m_汇率 = value; }
        }
        public decimal m_汇率;

        public decimal 人民币单价
        {
            get { return m_人民币单价; }
            set { m_人民币单价 = value; }
        }
        public decimal m_人民币单价;

        public decimal 积分率
        {
            get { return m_积分率; }
            set { m_积分率 = value; }
        }
        public decimal m_积分率;

        public decimal 重量
        {
            get { return m_重量; }
            set { m_重量 = value; }
        }
        public decimal m_重量;

        public ProductInfoForm()
        {
            InitializeComponent();
            tbx日期.Text = DateTime.Now.ToShortDateString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ("" != tbx名称.Text)
            {
                this.m_名称 = tbx名称.Text;
                if (!DateTime.TryParse(tbx日期.Text, out this.m_日期))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                    return;
                }
                this.m_店铺 = tbx店铺.Text;
                this.m_型号 = tbx型号.Text;
                this.m_货号 = tbx货号.Text;
                if (!decimal.TryParse(tbx单价.Text, out this.m_单价))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx税率.Text, out this.m_税率))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx税后单价.Text, out this.m_税后单价))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx汇率.Text, out this.m_汇率))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx人民币单价.Text, out this.m_人民币单价))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx积分率.Text, out this.m_积分率))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }
                else if (!decimal.TryParse(tbx重量.Text, out this.m_重量))
                {
                    this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                }

                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        private void ProductInfoForm_Load(object sender, EventArgs e)
        {
            tbx名称.Text = 名称;
            tbx店铺.Text = 店铺;
            tbx型号.Text = 型号;
            tbx货号.Text = 货号;
            tbx单价.Text = 单价.ToString();
            tbx税率.Text = 税率.ToString();
            tbx税后单价.Text = 税后单价.ToString();
            tbx汇率.Text = 汇率.ToString();
            tbx人民币单价.Text = 人民币单价.ToString();
            tbx积分率.Text = 积分率.ToString();
            tbx重量.Text = 重量.ToString();
        }

        public List<string> GetDataStringList()
        {
            List<string> dataList = new List<string>();
            dataList.Add(this.名称);
            string str = this.日期.ToShortDateString();
            dataList.Add(str);
            str = this.店铺;
            dataList.Add(str);
            str = this.型号;
            dataList.Add(str);
            str = this.货号;
            dataList.Add(str);
            str = this.单价.ToString();
            dataList.Add(str);
            str = this.税率.ToString();
            dataList.Add(str);
            str = this.税后单价.ToString();
            dataList.Add(str);
            str = this.汇率.ToString();
            dataList.Add(str);
            str = this.人民币单价.ToString();
            dataList.Add(str);
            str = this.积分率.ToString();
            dataList.Add(str);
            str = this.重量.ToString();
            dataList.Add(str);

            return dataList;
        }

        public void SetDataStringList(List<string> dataList)
        {
            System.Diagnostics.Trace.Assert(dataList.Count >= 13);

            this.名称 = dataList[1];
            DateTime outDateTime;
            DateTime.TryParse(dataList[2], out outDateTime);
            this.日期 = outDateTime;
            this.店铺 = dataList[3];
            this.型号 = dataList[4];
            this.货号 = dataList[5];
            decimal outVal;
            decimal.TryParse(dataList[6], out outVal);
            this.单价 = outVal;
            decimal.TryParse(dataList[7], out outVal);
            this.税率 = outVal;
            decimal.TryParse(dataList[8], out outVal);
            this.税后单价 = outVal;
            decimal.TryParse(dataList[9], out outVal);
            this.汇率 = outVal;
            decimal.TryParse(dataList[10], out outVal);
            this.人民币单价 = outVal;
            decimal.TryParse(dataList[11], out outVal);
            this.积分率 = outVal;
            decimal.TryParse(dataList[12], out outVal);
            this.重量 = outVal;
        }
    }
}
