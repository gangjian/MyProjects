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
    public partial class MainForm : Form
    {
        public List<DataInfo> _dataList;

        public MainForm()
        {
            InitializeComponent();
            // load数据
            string appPath = Application.ExecutablePath;
            appPath = appPath.Remove(appPath.LastIndexOf('\\') + 1);
            ClassDataFileManager dfMng = new ClassDataFileManager(appPath + "data.log");
            _dataList = dfMng.ReadDataList();
        }

        private void buttonTomatoTimer_Click(object sender, EventArgs e)
        {
            TomatoTimerForm tomatoTimerForm = new TomatoTimerForm();
            tomatoTimerForm.Show();
        }

        private void buttonDiagram_Click(object sender, EventArgs e)
        {
            DiagramForm diagramForm = new DiagramForm(_dataList);
            diagramForm.Show();
        }

        private void buttonDailyRecord_Click(object sender, EventArgs e)
        {
            DataInfo lastDataInfo = null;
            if (0 != _dataList.Count)
            {
                lastDataInfo = _dataList[_dataList.Count - 1];
            }
            DailyRecordForm dailyRecordForm = new DailyRecordForm(lastDataInfo);
            if (DialogResult.OK == dailyRecordForm.ShowDialog())
            {
            }
        }
    }
}
