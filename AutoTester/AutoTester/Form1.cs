using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace AutoTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            ShowTestCaseTreeView();

            InitListView();
            LoadDefaultFolders();
        }

        private const string MINGW_WND_TITLE = "MINGW32:~";

        private TestCaseTemplate m_TestCaseTemplate = new TestCaseTemplate();
        private ConfigInfo m_configInfo = new ConfigInfo();
        private List<string> m_testCaseList = new List<string>();

        private void ShowTestCaseTreeView()
        {
            treeView1.Nodes.Clear();
            for (int i = 0; i < m_TestCaseTemplate.m_templateList.Count; i++)
            {
                TestCaseTplInfo ti = m_TestCaseTemplate.m_templateList[i];
                string chapterNoStr = ti.chapterNo.ToString().PadLeft(2, '0');
                TreeNode node = new TreeNode(chapterNoStr);
                treeView1.Nodes.Add(node);
                treeView1.SelectedNode = node;
                for (int j = 0; j < ti.sectionNum; j++)
                {
                    string sectionStr = (j + 1).ToString().PadLeft(2, '0');
                    TreeNode subNode = new TreeNode(sectionStr);
                    bool bChecked = m_configInfo.LoadTestCaseChecked(chapterNoStr + "_" + sectionStr);
                    subNode.Checked = bChecked;
                    treeView1.SelectedNode.Nodes.Add(subNode);
                    if (bChecked)
                    {
                        treeViewCheckFlag = true;
                        treeView1.SelectedNode.Checked = bChecked;
                    }
                }
            }
        }

        private void InitListView()
        {
            listView1.Columns.Add("API No.", 70, HorizontalAlignment.Center);
            listView1.Columns.Add("Total", 50, HorizontalAlignment.Center);
            listView1.Columns.Add("Passed", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Failed", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Untested", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Tested", 80, HorizontalAlignment.Center);

            listView2.Columns.Add("No.", 28, HorizontalAlignment.Center);
            listView2.Columns.Add("Name", 80, HorizontalAlignment.Left);
        }

        private void LoadDefaultFolders()
        {
            string dft_tgt_path = @"..\..\..\sim_integrated";
            string dft_mst_log_path = @"..\..\..\sim_integrated\implib_test\log\mt1";
            DirectoryInfo di = new DirectoryInfo(dft_tgt_path);
            if (di.Exists)
            {
                this.tbxTargetFolder.Text = di.FullName;
            }
            else
            {
                this.tbxTargetFolder.Text = m_configInfo.m_targetPath;
            }
            di = new DirectoryInfo(dft_mst_log_path);
            if (di.Exists)
            {
                this.tbxMasterLogFolder.Text = di.FullName;
            }
            else
            {
                this.tbxMasterLogFolder.Text = m_configInfo.m_masterLogPath;
            }
        }

        private bool LaunchMsysDotBat()
        {
            string batName = @"C:\MinGW\msys\1.0\msys.bat";
            FileInfo fi = new FileInfo(batName);
            if (fi.Exists)
            {
                Process batProcess = new Process();
                string arguments = null;
                // 调用外部程序的路径和参数
                ProcessStartInfo vbsProcessStartInfo = new ProcessStartInfo(batName, arguments);
                batProcess.StartInfo = vbsProcessStartInfo;
                batProcess.Start();
                batProcess.WaitForExit();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 检查是否可以开始Build
        /// </summary>
        /// <returns></returns>
        private bool CheckBuild()
        {
            // 检查工作路径
            if (string.Empty == tbxTargetFolder.Text) { return false; }
            DirectoryInfo di = new DirectoryInfo(tbxTargetFolder.Text);
            if (!di.Exists)
            {
                MessageBox.Show(tbxTargetFolder.Text + "doesn't exist!");
                return false;
            }

            // .\Makefile
            FileInfo fi = new FileInfo(tbxTargetFolder.Text + "\\Makefile");
            if (!fi.Exists)
            {
                MessageBox.Show("Can not find \"Makefile\" in target folder!");
                return false;
            }

            // 检查mingw路径及msys.bat
            fi = new FileInfo("C:\\MinGW\\msys\\1.0\\msys.bat");
            if (!fi.Exists)
            {
                MessageBox.Show("Can not find \"msys.bat\"!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查是否可以开始执行测试
        /// </summary>
        /// <returns></returns>
        private bool CheckStart()
        {
            // 检查工作路径
            if (string.Empty == tbxTargetFolder.Text) { return false; }

            DirectoryInfo di = new DirectoryInfo(tbxTargetFolder.Text);
            if (!di.Exists)
            {
                MessageBox.Show(tbxTargetFolder.Text + "doesn't exist!");
                return false;
            }

            // .\build\libs\
            di = new DirectoryInfo(tbxTargetFolder.Text + "\\build\\libs");
            if (!di.Exists)
            {
                MessageBox.Show("Can not find \".\\build\\libs\\\" folder!");
                return false;
            }

            // .\implib_test\
            di = new DirectoryInfo(tbxTargetFolder.Text + "\\implib_test");
            if (!di.Exists)
            {
                MessageBox.Show("Can not find \".\\implib_test\\\" folder!");
                return false;
            }

            // .\implib_test\ct.exe
            FileInfo fi = new FileInfo(tbxTargetFolder.Text + "\\implib_test\\ct.exe");
            if (!fi.Exists)
            {
                MessageBox.Show("Can not find \".\\implib_test\\ct.exe\"!");
                return false;
            }
            return true;
        }

        private void btnSelMasterLogFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select master log folder";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbxMasterLogFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnSelTestLogFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select test log folder";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbxTestLogFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnSelTargetFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select target folder";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                tbxTargetFolder.Text = dialog.SelectedPath;
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void btnBuild_Click(object sender, EventArgs e)
        {
            if (!CheckBuild())
            {
                return;
            }
            if (!LaunchMsysDotBat())
            {
                return;
            }
            Thread.Sleep(3000);

            IntPtr wnd = WinAPI.FindWindow(null, MINGW_WND_TITLE);
            if (wnd.ToString() != "0")
            {
                string cmdStr = "cd " + "\"" + tbxTargetFolder.Text + "\"";
                WinAPI.SendCommand(wnd, cmdStr);

                cmdStr = @"make all";
                WinAPI.SendCommand(wnd, cmdStr);

                Thread.Sleep(3000);
                cmdStr = "export PATH=\"${PATH}:./build/libs\"";
                WinAPI.SendCommand(wnd, cmdStr);
            }
        }

        /// <summary>
        /// 点击"Start"按钮, 开始测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            // 检测是否可是开始执行测试
            if (!CheckStart())
            {
                return;
            }
            // 更新dll
            UpdateDlls();

            // 清除log
            tbTestLog.Clear();
            listView1.Items.Clear();
            listView2.Items.Clear();

            // 取得要测项目列表
            m_testCaseList = GetTestCaseList();
            progressBar1.Maximum = m_testCaseList.Count;

            Thread testWorker = new Thread(new ThreadStart(WorkerStart));
            testWorker.Start();
            EnableUIControls(false);
            m_dlgWorkerFinish = new DlgWorkerFinish(WorkerFinish);
        }

        private List<string> m_ChangedRegList = new List<string>();   // 变更过的寄存器名称列表

        private void WorkerStart()
        {
            string testExePath = tbxTargetFolder.Text + "\\implib_test\\ct.exe";
            FileInfo fi = new FileInfo(testExePath);
            if (!fi.Exists)
            {
                MessageBox.Show("Can not find the \"ct.exe\"!");
                this.BeginInvoke(m_dlgWorkerFinish, null);
                return;
            }
            if (0 == m_testCaseList.Count)
            {
                this.BeginInvoke(m_dlgWorkerFinish, null);
                return;
            }
            m_ChangedRegList = new List<string>();
            string outputLogPath = CreateLogOutputFolder();
            string chapterNoStr = "";
            OutputLogFile logFileObj = null;
            int count = 0; // 完成的项目数(用以更新进度条)
            foreach (string testCaseStr in m_testCaseList)
            {
                if (chapterNoStr != testCaseStr.Substring(0, 2))
                {
                    if (null != logFileObj)
                    {
                        logFileObj.Dispose();
                        // 更新变更寄存器名称列表
                        UpdateChangedRegList(logFileObj, outputLogPath);
                    }
                    chapterNoStr = testCaseStr.Substring(0, 2);
                    // 创建log文件
                    logFileObj = new OutputLogFile(outputLogPath, chapterNoStr);
                }
                RunTestCase(testExePath, testCaseStr, logFileObj);
                count += 1;

                // 更新进度条
                SetProgressBarVal(count);
            }

            if (null != logFileObj)
            {
                logFileObj.Dispose();
                // 更新变更寄存器名称列表
                UpdateChangedRegList(logFileObj, outputLogPath);
            }

            // 结束测试执行
            SetProgressBarVal(0);
            MessageBox.Show("Finish!");

            // 开始进行Log比对
            if ((string.Empty == tbxMasterLogFolder.Text)
                || (!Directory.Exists(tbxMasterLogFolder.Text))
                )
            {
                object nullObj = null;
                this.BeginInvoke(m_dlgWorkerFinish, new object[] { nullObj });
            }
            else
            {
                LogChecker.LogChecker logCheckerObj = new LogChecker.LogChecker(outputLogPath, tbxMasterLogFolder.Text);
                logCheckerObj.Start();

                logCheckerObj.OutputTestIdDetailResult();
                this.BeginInvoke(m_dlgWorkerFinish, new object[] { logCheckerObj });
            }
        }

        private string CreateLogOutputFolder()
        {
            string outputLogPath = System.Windows.Forms.Application.StartupPath + "\\" + "output_logs" + "\\"
                + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0')
                + DateTime.Now.Hour.ToString().PadLeft(2, '0') + DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0');
            Directory.CreateDirectory(outputLogPath);
            return outputLogPath;
        }

        /// <summary>
        /// 执行"ct.exe"进行测试
        /// </summary>
        /// <param name="testStr"></param>
        private void RunTestCase(string exePath, string testCaseStr, OutputLogFile outputLogFileObj)
        {
            ProcessStartInfo start = new ProcessStartInfo(exePath);
            start.WorkingDirectory = tbxTargetFolder.Text;
            start.CreateNoWindow = true;            // 不显示dos命令行窗口
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;          // 是否指定操作系统外壳进程启动程序

            Process p = Process.Start(start);
            StreamReader reader = p.StandardOutput;                             // 重定向Standard IO流
            StreamWriter writer = p.StandardInput;
            StreamReader err_reader = p.StandardError;                          // 当执行出错的时候, 根据Shiotaさん20141020的指摘
                                                                                // 要将Standard Error流中的assert error情报输出到log里
            // 往输入流里写入要测的项目号
            writer.WriteLine(testCaseStr);

            do
            {                                                                   // 将IO流输出到log里
                string line = reader.ReadLine();
                if (    (null != line)
                    &&  (outputLogFileObj.OutputLogProcess(line))   )
                {
                    tbTestLogAppendText(line + "\n");
                }
            } while (!reader.EndOfStream);

            do
            {                                                                   // 如果有Error, 要把Error输出到log里
                string err = err_reader.ReadLine();
                if (    (null != err)
                    &&  (string.Empty != err))
                {
                    tbTestLogAppendText(err + "\n");
                }
            } while (!err_reader.EndOfStream);

            p.WaitForExit();    // 等待程序执行完退出进程
            p.Close();          // 关闭进程
            reader.Close();     // 关闭流
            writer.Close();
        }

        private delegate void DlgtbTestLogAppendText(string appendTxt);
        private void tbTestLogAppendText(string appendTxt)
        {
            if (tbTestLog.InvokeRequired)
            {
                DlgtbTestLogAppendText dlg = new DlgtbTestLogAppendText(tbTestLogAppendText);
                this.BeginInvoke(dlg, new object[] { appendTxt });
            }
            else
            {
                tbTestLog.AppendText(appendTxt);
            }
        }

        /// <summary>
        /// 在窗体关闭前保存配置文件情报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DirectoryInfo di = null;
            if (string.Empty != tbxTargetFolder.Text)
            {
                di = new DirectoryInfo(tbxTargetFolder.Text);
                if (di.Exists)
                {
                    m_configInfo.SaveTargetPath(tbxTargetFolder.Text);
                }
            }
            if (string.Empty != tbxMasterLogFolder.Text)
            {
                di = new DirectoryInfo(tbxMasterLogFolder.Text);
                if (di.Exists)
                {
                    m_configInfo.SaveMasterLogPath(tbxMasterLogFolder.Text);
                }
            }

            SaveTestCaseTreeCheckInfo();
        }

        /// <summary>
        /// 保存树形控件的选中状态到配置文件
        /// </summary>
        private void SaveTestCaseTreeCheckInfo()
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    string testCaseStr = node.Text + "_" + subNode.Text;
                    if (!node.Checked)
                    {
                        subNode.Checked = node.Checked;
                    }
                    m_configInfo.SaveTestCaseChecked(testCaseStr, subNode.Checked);
                }
            }
        }

        /// <summary>
        /// check all
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (TreeNode node in treeView1.Nodes)
            {
                node.Checked = cbxCheckAll.Checked;
                foreach (TreeNode subNode in node.Nodes)
                {
                    subNode.Checked = cbxCheckAll.Checked;
                }
            }
        }

        /// <summary>
        /// 根据树形控件的选中状态取得要测项目的列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetTestCaseList()
        {
            List<string> testCaseList = new List<string>();

            foreach (TreeNode node in treeView1.Nodes)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    if (subNode.Checked)
                    {
                        string testCaseStr = node.Text + subNode.Text;
                        testCaseList.Add(testCaseStr);
                    }
                }
            }

            return testCaseList;
        }

        // 执行测试的子线程结束后通知主线程用的代理
        private delegate void DlgWorkerFinish(LogChecker.LogChecker logCheckerObj);
        private DlgWorkerFinish m_dlgWorkerFinish = null;

        private void WorkerFinish(LogChecker.LogChecker logCheckerObj)
        {
            EnableUIControls(true);
            if (null != logCheckerObj)
            {
                showCompareResultsOnUI(logCheckerObj);
            }
            showChangedRegList();

            // 删除dll文件
            DeleteDlls();
        }

        private delegate void DlgListViewAddItem(ListViewItem item);

        private void ListViewAddItem(ListViewItem item)
        {
            if (listView1.InvokeRequired)
            {
                DlgListViewAddItem dlg = new DlgListViewAddItem(ListViewAddItem);
                this.BeginInvoke(dlg, new object[] { item });
            }
            else
            {
                listView1.Items.Add(item);
            }
        }

        // 更新进度条用的代理
        private delegate void DlgUpdateProgressBarVal(int value);

        /// <summary>
        /// 更新进度条
        /// </summary>
        /// <param name="value"></param>
        private void SetProgressBarVal(int value)
        {
            if (progressBar1.InvokeRequired)
            {
                DlgUpdateProgressBarVal dlg = new DlgUpdateProgressBarVal(SetProgressBarVal);
                this.BeginInvoke(dlg, new object[] { value });
            }
            else
            {
                progressBar1.Value = value;
            }
        }

        /// <summary>
        /// 在UI上表示log比对的结果
        /// </summary>
        /// <param name="displayList"></param>
        private void showCompareResultsOnUI(LogChecker.LogChecker logCheckerObj)
        {
            listView1.Items.Clear();
            //listView2.Items.Clear();

            for (int i = 0; i < m_TestCaseTemplate.m_templateList.Count; i++)
            {
                TestCaseTplInfo tpl = m_TestCaseTemplate.m_templateList[i];
                for (int j = 0; j < tpl.sectionNum; j++)
                {
                    int chapterNo = tpl.chapterNo;
                    int sectionNo = j + 1;
                    int totalCnt = 0;
                    int passedCnt = 0;
                    int apiNgCnt = 0;
                    int dataNgCnt = 0;
                    int untestedCnt = 0;
                    int testedCnt = 0;
                    int idx = 0;
                    int lastIdx = 0;

                    // 如果在显示列表中找到了, 说明在实际log中出现过
                    if (-1 != (idx = logCheckerObj.findTestCaseInDisplayList(chapterNo, sectionNo, lastIdx)))
                    {
                        lastIdx = idx;    // 用来记录上回查找到结果的位置, 因为是按顺序排列的, 避免了下一次再从头开始查找;
                        logCheckerObj.getTestIdResultCount(idx, out totalCnt, out passedCnt, out apiNgCnt, out dataNgCnt, out untestedCnt, out testedCnt);

                        // 在下面列表里表示重复出现的test id次数
                        //showTestIdReoccurs(m_displayList[idx]);

                        // 输出每个test id的详细结果
                    }
                    // 没找到说明该项目并没有实际在log中出现过
                    else
                    {
                    }
                    string testCaseNoStr = chapterNo.ToString().PadLeft(2, '0') + "_" + sectionNo.ToString().PadLeft(2, '0');
                    ListViewItem item = new ListViewItem(testCaseNoStr);
                    item.SubItems.Add(totalCnt.ToString());
                    item.SubItems.Add(passedCnt.ToString());
                    string ngCntStr = (apiNgCnt + dataNgCnt).ToString();
                    //if (0 != (apiNgCnt + dataNgCnt))
                    //{
                    //    ngCntStr += ("(" + apiNgCnt.ToString() + "/" + dataNgCnt.ToString() + ")");
                    //}
                    item.SubItems.Add(ngCntStr);
                    item.SubItems.Add(untestedCnt.ToString());
                    //string percentStr = "(" + ((float)((uint)((float)testedCnt / (float)totalCnt * 10000))/100).ToString() + "%" + ")";
                    item.SubItems.Add(testedCnt.ToString());
                    //listView1.Items.Add(item);
                    ListViewAddItem(item);
                }
            }
        }

        // 用一个标志变量防止树形的checkbox在选中时发生多次AfterCheck事件
        private bool treeViewCheckFlag = false;

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (treeViewCheckFlag)
            {
                treeViewCheckFlag = false;
                return;
            }

            // 如果是次级节点
            if (null != e.Node.Parent)
            {
                if (e.Node.Checked)
                {
                    if (!e.Node.Parent.Checked)
                    {
                        treeViewCheckFlag = true;   // 防止发生连锁反应(触发父节点的AfterCheck)
                        e.Node.Parent.Checked = true;
                    }
                }
                else
                {
                    TreeNode node = e.Node.Parent.FirstNode;
                    bool bChecked = false;
                    while (null != node)
                    {
                        if (node.Checked)
                        {
                            bChecked = true;    // 至少还有一个子节点处于checked状态
                            break;
                        }
                        node = node.NextNode;
                    }
                    if (!bChecked)
                    {
                        // 如果没有任何子节点处于checked状态
                        e.Node.Parent.Checked = false;
                    }
                }
            }
            // 如果是第一级节点
            else
            {
                foreach (TreeNode subNode in e.Node.Nodes)
                {
                    if (subNode.Checked != e.Node.Checked)
                    {
                        subNode.Checked = e.Node.Checked;
                    }
                }
            }
        }

        /// <summary>
        /// 设置界面各控件的可用状态
        /// </summary>
        /// <param name="enable"></param>
        private void EnableUIControls(bool enable)
        {
            btnBuild.Enabled = enable;
            btnStart.Enabled = enable;
            btnCompare.Enabled = enable;
            btnSelTargetFolder.Enabled = enable;
            btnSelMasterLogFolder.Enabled = enable;
            btnSelTestLogFolder.Enabled = enable;
            btnSaveCSV.Enabled = enable;
            treeView1.Enabled = enable;
            listView1.Enabled = enable;
            listView2.Enabled = enable;
        }

        /// <summary>
        /// 更新变更寄存器名称列表
        /// </summary>
        /// <param name="subList"></param>
        private void UpdateChangedRegList(OutputLogFile logfileObj, string outputLogPath)
        {
            foreach (string regName in logfileObj.m_WriteRegList)
            {
                if (!m_ChangedRegList.Contains(regName))
                {
                    m_ChangedRegList.Add(regName);
                }
            }
            //foreach (string regName in logfileObj.m_ReadRegList)
            //{
            //    if (!m_ChangedRegList.Contains(regName))
            //    {
            //        m_ChangedRegList.Add(regName);
            //    }
            //}

            // 保存寄存器变更列表到文件
            logfileObj.SaveRegChangedList(logfileObj.m_ReadRegList, logfileObj.m_WriteRegList, outputLogPath);
        }

        private void showChangedRegList()
        {
            listView2.Items.Clear();
            int idx = 0;
            foreach (string regName in m_ChangedRegList)
            {
                idx++;
                ListViewItem lvi = new ListViewItem(idx.ToString());
                lvi.SubItems.Add(regName);
                listView2.Items.Add(lvi);
            }
        }

        private void UpdateDlls()
        {
            string exePath = tbxTargetFolder.Text + @"\implib_test\";
            DirectoryInfo di = new DirectoryInfo(tbxTargetFolder.Text + @"\build\libs\");
            if (!di.Exists)
            {
                return;
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                if (    fi.Extension.ToLower().Equals(".dll")
                    ||  fi.Extension.ToLower().Equals(".a"))
                {
                    if (CompareDll(fi))
                    {
                        fi.CopyTo(exePath + fi.Name, true);
                    }
                }
            }
        }

        /// <summary>
        /// 比较dll文件, 判断是否需要更新
        /// </summary>
        /// <param name="srcInfo"></param>
        /// <returns></returns>
        private bool CompareDll(FileInfo srcInfo)
        {
            string exePath = tbxTargetFolder.Text + @"\implib_test\";
            FileInfo dstInfo = new FileInfo(exePath + srcInfo.Name);
            // 如果目标dll文件不存在, 需要更新
            if (!dstInfo.Exists)
            {
                return true;
            }
            // 如果目标dll文件的创建时间更早(更久), 需要更新
            if (dstInfo.LastWriteTime.CompareTo(srcInfo.LastWriteTime) < 0)
            {
                return true;
            }
            // 不需要更新
            return false;
        }

        private void DeleteDlls()
        {
            string exePath = tbxTargetFolder.Text + @"\implib_test\";
            DirectoryInfo di = new DirectoryInfo(exePath);
            if (!di.Exists)
            {
                return;
            }
            foreach (FileInfo fi in di.GetFiles())
            {
                if (    fi.Extension.ToLower().Equals(".dll")
                    ||  fi.Extension.ToLower().Equals(".lib")
                    ||  fi.Extension.ToLower().Equals(".a"))
                {
                    try
                    {
                        fi.Delete();
                    }
                    catch (Exception ex)
                    {
                        string errMsg = ex.Message;
                    }
                }
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            if (    string.Empty == tbxTestLogFolder.Text
                ||  string.Empty == tbxMasterLogFolder.Text)
            {
                return;
            }
            DirectoryInfo di = new DirectoryInfo(tbxTestLogFolder.Text);
            if (!di.Exists)
            {
                return;
            }
            di = new DirectoryInfo(tbxMasterLogFolder.Text);
            if (!di.Exists)
            {
                return;
            }

            m_dlgWorkerFinish = new DlgWorkerFinish(WorkerFinish);
            LogChecker.LogChecker logCheckerObj = new LogChecker.LogChecker(tbxTestLogFolder.Text, tbxMasterLogFolder.Text);
            logCheckerObj.Start();

            logCheckerObj.OutputTestIdDetailResult();
            this.BeginInvoke(m_dlgWorkerFinish, new object[] { logCheckerObj });
        }

        /// <summary>
        /// 保存log比对结果到CSV文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveCSV_Click(object sender, EventArgs e)
        {
            if (0 == listView1.Items.Count)
            {
                return;
            }
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files|*.csv|All files|*.*";

                if (DialogResult.OK == sfd.ShowDialog())
                {
                    string saveFileName = sfd.FileName;
                    StreamWriter sw = new StreamWriter(saveFileName, false, Encoding.Default);

                    // 写标题
                    for (int i = 0; i < listView1.Columns.Count; i++)
                    {
                        sw.Write(listView1.Columns[i].Text);
                        if (listView1.Columns.Count - 1 != i)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.WriteLine();

                    // 写各行的内容
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        ListViewItem item = listView1.Items[i];
                        string writeLineStr = "";
                        for (int j = 0; j < item.SubItems.Count; j++)
                        {
                            ListViewItem.ListViewSubItem subItem = item.SubItems[j];
                            writeLineStr += subItem.Text;
                            if (item.SubItems.Count - 1 != j)
                            {
                                writeLineStr += ",";
                            }
                        }
                        sw.WriteLine(writeLineStr);
                    }

                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
