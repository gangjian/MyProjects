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
        private static string m_打开文件名 = "";
        public const string S_MODE_1_TITLE = @"商品登记";
        public const string S_MODE_2_TITLE = @"入库";
        public const string S_MODE_3_TITLE = @"出库";
        public const string S_MODE_4_TITLE = @"结算";

        public string DataFileName
        {
            get
            {
                return m_打开文件名;
            }
            set
            {
                m_打开文件名 = value;
            }
        }

        public DetailListForm()
        {
            InitializeComponent();
        }

        public DetailListForm(E_FORM_MODE mode)
        {
            InitializeComponent();
            m_mode = mode;
            if ("" != m_打开文件名)
            {
                this.Text = m_打开文件名;
            }
        }

        public string 打开文件名
        {
            get { return m_打开文件名; }
            set { m_打开文件名 = value; }
        }

        private void DetailListForm_Load(object sender, EventArgs e)
        {
            ListViewInitial();
            if ("" != 打开文件名)
            {
                FileInfo fi = new FileInfo(打开文件名);
                if (fi.Exists)
                {
                    ReadDataFile(打开文件名);
                }
            }
        }

        private void ListViewInitial()
        {
            if (E_FORM_MODE.MODE_1 == m_mode)
            {
                listView1.Columns.Add("编号", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("名称", 80, HorizontalAlignment.Left);
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
            else if (   E_FORM_MODE.MODE_2 == m_mode
                     || E_FORM_MODE.MODE_3 == m_mode)
            {
                listView1.Columns.Add("编号", 50, HorizontalAlignment.Left);
                listView1.Columns.Add("日期", 150, HorizontalAlignment.Left);
                listView1.Columns.Add("名称", 120, HorizontalAlignment.Left);
                listView1.Columns.Add("单价", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("数量", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("优惠率", 80, HorizontalAlignment.Left);
                listView1.Columns.Add("均摊运费", 100, HorizontalAlignment.Left);
            }
            else if (E_FORM_MODE.MODE_4 == m_mode)
            {
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            InfoEdit();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (E_FORM_MODE.MODE_1 == m_mode)
            {
                ProductInfoForm pif = new ProductInfoForm();
                if (DialogResult.OK == pif.ShowDialog())
                {
                    ListViewItem lvi = new ListViewItem((listView1.Items.Count + 1).ToString());
                    List<string> dataList = pif.GetDataStringList();
                    foreach (string str in dataList)
                    {
                        lvi.SubItems.Add(str);
                    }
                    listView1.Items.Add(lvi);
                }
            }
            else if (   (E_FORM_MODE.MODE_2 == m_mode)
                     || (E_FORM_MODE.MODE_3 == m_mode))
            {
                InputOutputInfoForm ioif = new InputOutputInfoForm();
                if (DialogResult.OK == ioif.ShowDialog())
                {
                    ListViewItem lvi = new ListViewItem((listView1.Items.Count + 1).ToString());
                    List<string> dataList = ioif.GetDataStringList();
                    foreach (string str in dataList)
                    {
                        lvi.SubItems.Add(str);
                    }
                    listView1.Items.Add(lvi);
                }
            }
            else if (E_FORM_MODE.MODE_4 == m_mode)
            {
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (0 == listView1.Items.Count)
            {
                return;
            }
            if ("" != m_打开文件名)
            {
                SaveToFile(m_打开文件名);
            }
            else
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = "csv files(*.csv)|*.csv";
                saveFileDialog1.FilterIndex = 1;
                saveFileDialog1.RestoreDirectory = true;

                if (DialogResult.OK == saveFileDialog1.ShowDialog())
                {
                    string fileName = saveFileDialog1.FileName;
                    SaveToFile(fileName);
                }
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            if (0 == listView1.Items.Count)
            {
                return;
            }

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "csv files(*.csv)|*.csv";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (DialogResult.OK == saveFileDialog1.ShowDialog())
            {
                string fileName = saveFileDialog1.FileName;
                SaveToFile(fileName);
            }
        }

        private void SaveToFile(string fileName)
        {
            try
            {
                StreamReader sr = new StreamReader(fileName);
                List<string> wtList = new List<string>();

                string rdline = "";
                bool wtFlag = true;
                bool savedFlag = false;
                while (null != (rdline = sr.ReadLine()))
                {
                    rdline = rdline.Trim();
                    if (S_MODE_1_TITLE == rdline)
                    {
                        if (E_FORM_MODE.MODE_1 == m_mode)
                        {
                            wtList.Add(rdline);
                            wtFlag = false;
                            wtList.AddRange(GetListViewData());
                            savedFlag = true;
                        }
                        else
                        {
                            wtFlag = true;
                        }
                    }
                    else if (S_MODE_2_TITLE == rdline)
                    {
                        if (E_FORM_MODE.MODE_2 == m_mode)
                        {
                            wtList.Add(rdline);
                            wtFlag = false;
                            wtList.AddRange(GetListViewData());
                            savedFlag = true;
                        }
                        else
                        {
                            wtFlag = true;
                        }
                    }
                    else if (S_MODE_3_TITLE == rdline)
                    {
                        if (E_FORM_MODE.MODE_3 == m_mode)
                        {
                            wtList.Add(rdline);
                            wtFlag = false;
                            wtList.AddRange(GetListViewData());
                            savedFlag = true;
                        }
                        else
                        {
                            wtFlag = true;
                        }
                    }
                    else if (S_MODE_4_TITLE == rdline)
                    {
                        if (E_FORM_MODE.MODE_4 == m_mode)
                        {
                            wtList.Add(rdline);
                            wtFlag = false;
                            wtList.AddRange(GetListViewData());
                            savedFlag = true;
                        }
                        else
                        {
                            wtFlag = true;
                        }
                    }

                    if (wtFlag)
                    {
                        wtList.Add(rdline);
                    }
                }
                sr.Close();
                if (false == savedFlag)
                {
                    string wtTitle = "";
                    switch (m_mode)
                    {
                        case E_FORM_MODE.MODE_1:
                            wtTitle = S_MODE_1_TITLE;
                            break;
                        case E_FORM_MODE.MODE_2:
                            wtTitle = S_MODE_2_TITLE;
                            break;
                        case E_FORM_MODE.MODE_3:
                            wtTitle = S_MODE_3_TITLE;
                            break;
                        case E_FORM_MODE.MODE_4:
                            wtTitle = S_MODE_4_TITLE;
                            break;
                    }
                    wtList.Add(wtTitle);
                    wtList.AddRange(GetListViewData());
                    savedFlag = true;
                }
                StreamWriter sw = new StreamWriter(fileName);

                foreach (string str in wtList)
                {
                    sw.WriteLine(str);
                }
                sw.Close();
                m_打开文件名 = fileName;
                MessageBox.Show("保存成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
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
                ReadDataFile(openFileDialog.FileName);
                打开文件名 = openFileDialog.FileName;
                this.Text = 打开文件名;
                openFileDialog.Dispose();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            InfoEdit();
        }


        void InfoEdit()
        {
            if (0 == listView1.SelectedItems.Count)
            {
                return;
            }
            ListViewItem selectItem = listView1.SelectedItems[0];

            try
            {
                List<string> dataList = new List<string>();
                for (int i = 0; i < selectItem.SubItems.Count; i++)
                {
                    dataList.Add(selectItem.SubItems[i].Text);
                }
                if (E_FORM_MODE.MODE_1 == m_mode)
                {
                    ProductInfoForm pif = new ProductInfoForm();
                    pif.SetDataStringList(dataList);
                    if (DialogResult.OK == pif.ShowDialog())
                    {
                        dataList = pif.GetDataStringList();
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            selectItem.SubItems[i + 1].Text = dataList[i];
                        }
                    }
                }
                else if (   (E_FORM_MODE.MODE_2 == m_mode)
                         || (E_FORM_MODE.MODE_3 == m_mode))
                {
                    InputOutputInfoForm ioif = new InputOutputInfoForm();
                    ioif.SetDataStringList(dataList);
                    if (DialogResult.OK == ioif.ShowDialog())
                    {
                        dataList = ioif.GetDataStringList();
                        for (int i = 0; i < dataList.Count; i++)
                        {
                            selectItem.SubItems[i + 1].Text = dataList[i];
                        }
                    }
                }
                else if (E_FORM_MODE.MODE_4 == m_mode)
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void ReadDataFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            if (null == sr)
            {
                return;
            }
            listView1.Items.Clear();

            string rdline = "";
            bool rdflag = false;
            while (null != (rdline = sr.ReadLine()))
            {
                if ("" == rdline.Trim())
                {
                    continue;
                }
                if (S_MODE_1_TITLE == rdline)
                {
                    rdflag = ((E_FORM_MODE.MODE_1 == m_mode) ?  true : false);
                }
                else if (S_MODE_2_TITLE == rdline)
                {
                    rdflag = ((E_FORM_MODE.MODE_2 == m_mode) ? true : false);
                }
                else if (S_MODE_3_TITLE == rdline)
                {
                    rdflag = ((E_FORM_MODE.MODE_3 == m_mode) ? true : false);
                }
                else if (S_MODE_4_TITLE == rdline)
                {
                    rdflag = ((E_FORM_MODE.MODE_4 == m_mode) ? true : false);
                }
                if (!rdflag)
                {
                    continue;
                }
                string[] rdArr = rdline.Split(',');
                if (E_FORM_MODE.MODE_1 == m_mode)
                {
                    if (rdArr.Length < 13)
                    {
                        continue;
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(rdArr[0]);
                        for (int i = 1; i <= 12; i++)
                        {
                            lvi.SubItems.Add(rdArr[i]);
                        }
                        listView1.Items.Add(lvi);
                    }
                }
                if ((E_FORM_MODE.MODE_2 == m_mode || E_FORM_MODE.MODE_3 == m_mode))
                {
                    if (rdArr.Length < 8)
                    {
                        continue;
                    }
                    else
                    {
                        ListViewItem lvi = new ListViewItem(rdArr[0]);
                        for (int i = 1; i <= 7; i++)
                        {
                            lvi.SubItems.Add(rdArr[i]);
                        }
                        listView1.Items.Add(lvi);
                    }
                }
                else
                {
                }
            }
            sr.Close();
        }

        List<string> GetListViewData()
        {
            List<string> dataList = new List<string>();
            string dataStr = "";
            foreach (ListViewItem lvi in listView1.Items)
            {
                dataStr = "";
                foreach (ListViewItem.ListViewSubItem subItm in lvi.SubItems)
                {
                    dataStr += (subItm.Text + ",");
                }
                dataList.Add(dataStr);
            }
            return dataList;
        }
    }
}
