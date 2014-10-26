using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace WebBrowserTool
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetActiveWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public Form1()
        {
            InitializeComponent();
            textBoxURL.Text = "https://auth.alipay.com/login/index.htm?needTransfer=true&goto=http://financeprod.alipay.com/fund/index.htm";
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string strUrl = textBoxURL.Text.Trim();
            if (strUrl.Length > 0)
            {
                webBrowser1.Navigate(strUrl);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            HtmlDocument doc = webBrowser1.Document;
            HtmlElement ClickBtn = null;
            for (int i = 0; i < doc.All.Count; i++)
            {
                if (doc.All[i].TagName.ToUpper().Equals("INPUT"))
                {
                    switch (doc.All[i].Name)
                    {
                        case "logonId":
                            doc.All[i].InnerText = "gangjian.mail@qq.com";　　// 用户名
                            break;
                    }
                }
                if ("J-login-btn" == doc.All[i].Id)
                {
                    ClickBtn = doc.All[i];
                }
            }
            if (null == ClickBtn)
            {
                return;
            }
            SendKeys.Send("gangjian4715354");                   // 自动录入密码
            SendKeys.SendWait("{tab}");
            Thread.Sleep(1000);
            SendKeys.Send("yr8j");
            Thread.Sleep(1000);
            ClickBtn.InvokeMember("Click");　　　　　　　　　　　　 // 点击“登录”按钮
        }
    }
}
