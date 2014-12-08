using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace BMTool
{
    public enum E_FORM_MODE
    {
        MODE_1,     // 登记
        MODE_2,     // 入库
        MODE_3,     // 出库
        MODE_4,     // 结算
    }

    public partial class DetailListForm : Form
    {
        private E_FORM_MODE m_mode;

        public DetailListForm()
        {
            InitializeComponent();
        }

        public DetailListForm(E_FORM_MODE mode)
        {
            InitializeComponent();
            m_mode = mode;
        }

        private void DetailListForm_Load(object sender, EventArgs e)
        {
            ListViewInitial();
        }

        private void ListViewInitial()
        {
            if (E_FORM_MODE.MODE_1 == m_mode)
            {
                listView1.Columns.Add("编号", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("名称", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("日期", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("店铺", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("型号", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("货号", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("单价", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("税率", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("税后单价", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("汇率", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("人民币单价", 120, HorizontalAlignment.Left);
                listView1.Columns.Add("积分率", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("重量", 80, HorizontalAlignment.Left);
            }
            else if (E_FORM_MODE.MODE_2 == m_mode)
            {
            }
            else if (E_FORM_MODE.MODE_3 == m_mode)
            {
            }
            else if (E_FORM_MODE.MODE_4 == m_mode)
            {
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (0 == listView1.SelectedItems.Count)
            {
                return;
            }
            ListViewItem selectItem = listView1.SelectedItems[0];
            ProductInfoForm pif = new ProductInfoForm();

            pif.名称 = selectItem.SubItems[1].Text;
            pif.店铺 = selectItem.SubItems[3].Text;
            pif.型号 = selectItem.SubItems[4].Text;
            pif.货号 = selectItem.SubItems[5].Text;
            decimal outVal;
            decimal.TryParse(selectItem.SubItems[6].Text, out outVal);
            pif.单价 = outVal;
            decimal.TryParse(selectItem.SubItems[7].Text, out outVal);
            pif.税率 = outVal;
            decimal.TryParse(selectItem.SubItems[8].Text, out outVal);
            pif.税后单价 = outVal;
            decimal.TryParse(selectItem.SubItems[9].Text, out outVal);
            pif.汇率 = outVal;
            decimal.TryParse(selectItem.SubItems[10].Text, out outVal);
            pif.人民币单价 = outVal;
            decimal.TryParse(selectItem.SubItems[11].Text, out outVal);
            pif.积分率 = outVal;
            decimal.TryParse(selectItem.SubItems[12].Text, out outVal);
            pif.重量 = outVal;
            if (DialogResult.OK == pif.ShowDialog())
            {
                string str = pif.名称;
                selectItem.SubItems[1].Text = str;
                str = pif.日期.ToString();
                selectItem.SubItems[2].Text = str;
                str = pif.店铺;
                selectItem.SubItems[3].Text = str;
                str = pif.型号;
                selectItem.SubItems[4].Text = str;
                str = pif.货号;
                selectItem.SubItems[5].Text = str;
                str = pif.单价.ToString();
                selectItem.SubItems[6].Text = str;
                str = pif.税率.ToString();
                selectItem.SubItems[7].Text = str;
                str = pif.税后单价.ToString();
                selectItem.SubItems[8].Text = str;
                str = pif.汇率.ToString();
                selectItem.SubItems[9].Text = str;
                str = pif.人民币单价.ToString();
                selectItem.SubItems[10].Text = str;
                str = pif.积分率.ToString();
                selectItem.SubItems[11].Text = str;
                str = pif.重量.ToString();
                selectItem.SubItems[12].Text = str;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            ProductInfoForm pif = new ProductInfoForm();
            if (DialogResult.OK == pif.ShowDialog())
            {
                ListViewItem lvi = new ListViewItem((listView1.Items.Count + 1).ToString());
                string str = pif.名称;
                lvi.SubItems.Add(str);
                str = pif.日期.ToString();
                lvi.SubItems.Add(str);
                str = pif.店铺;
                lvi.SubItems.Add(str);
                str = pif.型号;
                lvi.SubItems.Add(str);
                str = pif.货号;
                lvi.SubItems.Add(str);
                str = pif.单价.ToString();
                lvi.SubItems.Add(str);
                str = pif.税率.ToString();
                lvi.SubItems.Add(str);
                str = pif.税后单价.ToString();
                lvi.SubItems.Add(str);
                str = pif.汇率.ToString();
                lvi.SubItems.Add(str);
                str = pif.人民币单价.ToString();
                lvi.SubItems.Add(str);
                str = pif.积分率.ToString();
                lvi.SubItems.Add(str);
                str = pif.重量.ToString();
                lvi.SubItems.Add(str);

                listView1.Items.Add(lvi);
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (0 == listView1.SelectedItems.Count)
            {
                return;
            }
            ListViewItem selectItem = listView1.SelectedItems[0];
            if (DialogResult.OK == MessageBox.Show("确定删除?", "删除", MessageBoxButtons.OKCancel))
            {
                listView1.Items.Remove(selectItem);
            }
        }

        private void btbSave_Click(object sender, EventArgs e)
        {
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (0 == listView1.Items.Count)
            {
                return;
            }

            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "csv files(*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                if (null != (myStream = saveFileDialog1.OpenFile()))
                {
                    using (StreamWriter sw = new StreamWriter(myStream))
                    {
                        foreach (ListViewItem lvi in listView1.Items)
                        {
                            foreach (ListViewItem.ListViewSubItem subItm in lvi.SubItems)
                            {
                                sw.Write(subItm.Text + ",");
                            }
                        }
                        sw.Write("\r\n");
                    }

                    myStream.Close();
                }
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "csv files(*.csv)|*.csv";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Stream myStream = openFileDialog.OpenFile();
                if (null == myStream)
                {
                    return;
                }
                listView1.Items.Clear();
                using (StreamReader sr = new StreamReader(myStream))
                {
                    string rdLine = "";
                    while (null != (rdLine = sr.ReadLine()))
                    {
                        if ("" == rdLine.Trim())
                        {
                            continue;
                        }
                        string[] rdArr = rdLine.Split(',');
                        if (rdArr.Length < 13)
                        {
                            continue;
                        }
                        ListViewItem lvi = new ListViewItem(rdArr[0]);
                        for (int i = 1; i <= 12; i++)
                        {
                            lvi.SubItems.Add(rdArr[i]);
                        }
                        listView1.Items.Add(lvi);
                    }
                }
                myStream.Close();
            }
        }
    }
}
