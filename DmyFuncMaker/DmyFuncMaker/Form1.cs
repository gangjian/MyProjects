using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DmyFuncMaker
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			this.textBox1.Clear();
			this.textBox2.Clear();
		}

		private void btnGenerate_Click(object sender, EventArgs e)
		{
			this.textBox2.Clear();
			foreach (string line in this.textBox1.Lines)
			{
				DmyFuncMaker.DmyFuncPrototypeProc(line);
			}
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.RestoreDirectory = true;
			dlg.FileName = "ut_dummy.c";
			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(dlg.FileName, false);
				sw.Write(this.textBox2.Text);
				sw.Close();
			}
		}

	}
}
