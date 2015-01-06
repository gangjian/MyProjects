using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GDIPlusTest.Tools.AutoKeys
{
    public partial class FormAutoKeys : Form
    {
        public FormAutoKeys()
        {
            InitializeComponent();
        }

        WinIOApi _winio_api;

        private void FormAutoKeys_Load(object sender, EventArgs e)
        {
            _winio_api = new WinIOApi();
        }

        private void FormAutoKeys_FormClosing(object sender, FormClosingEventArgs e)
        {
            _winio_api.Dispose();
        }

        private List<AutoKeyInfo> _autoKeyInfoList;

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "TXT files|*.txt|All files|*.*";
            ofd.RestoreDirectory = true;
            ofd.FilterIndex = 1;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.FileName + "\r\n\r\n";
                List<AutoKeyInfo> keyInfoList;
                textBox1.AppendText(loadScript(ofd.FileName, out keyInfoList));
                _autoKeyInfoList = keyInfoList;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (    (null == _autoKeyInfoList)
                ||  (0 == _autoKeyInfoList.Count))
            {
                return;
            }
            _startWaitCounter = 0;
            foreach (AutoKeyInfo aki in _autoKeyInfoList)
            {
                aki.couter = 0;
            }
            timer1.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _startWaitCounter = 0;
            timer1.Stop();
        }

        private int _startWaitCounter = 0;
        private const int START_WAIT_COUNT = 5;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (_startWaitCounter < START_WAIT_COUNT)
            {
                _startWaitCounter += 1;
                return;
            }
            foreach (AutoKeyInfo aki in _autoKeyInfoList)
            {
                aki.couter += 1;
                if (aki.couter == aki.period)
                {
                    this._winio_api.KeyPress(aki.key, 100);
                    System.Threading.Thread.Sleep(100);
                    this._winio_api.KeyPress(aki.key, 100);
                    System.Threading.Thread.Sleep(100);
                    aki.couter = 0;
                }
            }
        }

        private string loadScript(string fileName, out List<AutoKeyInfo> keyInfoList)
        {
            string fileContents = "";
            keyInfoList = new List<AutoKeyInfo>();

            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                StreamReader sr = new StreamReader(fileName);
                string rdline = "";
                int idx = -1;
                while (null != (rdline = sr.ReadLine()))
                {
                    if (-1 != (idx = rdline.IndexOf(@"//")))
                    {
                        rdline = rdline.Remove(idx).Trim();
                    }
                    if ("" == rdline)
                    {
                        continue;
                    }
                    fileContents += (rdline + "\r\n");
                    string[] rdArr = rdline.Split(',');
                    if (rdArr.Length >= 2)
                    {
                        int key, period;
                        if (int.TryParse(rdArr[0], out key)
                            && int.TryParse(rdArr[1], out period))
                        {
                            AutoKeyInfo aki = new AutoKeyInfo(key, period);
                            keyInfoList.Add(aki);
                        }
                    }
                }
                sr.Close();
            }

            return fileContents;
        }

    }

    public class AutoKeyInfo
    {
        public SuperKeys.WinIoSys.Key key;      // 按键名
        public int period;                      // 周期(单位秒)
        public int couter;                      // 计数器

        public AutoKeyInfo(int keyVal, int periodVal)
        {
            key = (SuperKeys.WinIoSys.Key)keyVal;
            period = periodVal;
            couter = 0;
        }
    }

}
