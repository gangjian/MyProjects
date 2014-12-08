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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 商品登记
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            DetailListForm df = new DetailListForm(E_FORM_MODE.MODE_1);
            df.Show();
        }

        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DetailListForm df = new DetailListForm(E_FORM_MODE.MODE_2);
            df.Show();
        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DetailListForm df = new DetailListForm(E_FORM_MODE.MODE_3);
            df.Show();
        }

        /// <summary>
        /// 结算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            DetailListForm df = new DetailListForm(E_FORM_MODE.MODE_4);
            df.Show();
        }
    }
}
