using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;

namespace IP_Lab.Page
{
    public partial class PageSwitcher : UserControl
    {
        public PageSwitcher()
        {
            InitializeComponent();
            DetectedBrowserInfo();
            SwitchPage(new PageLogin());
            //SwitchPage(new PageMain());
        }

        /// <summary> 
        /// 切换页面 
        /// </summary> 
        /// <param name="newPage">需要被切换到的页面</param> 
        /* 
         * 在需要切换页面的事件响应方法中
           PageSwitcher switcher = this.Parent as PageSwitcher;
           switcher.SwitchPage(new AnotherPage());
         * */
        public void SwitchPage(UserControl newPage)
        {
            this.Content = newPage;
        }

        protected void DetectedBrowserInfo()
        {
            BrowserInformation browserInfo = HtmlPage.BrowserInformation;
            if(browserInfo.ProductName == "MSIE")
                MessageBox.Show("推荐您使用非IE的浏览器以获得更好的体验~~");
        }
    }
}
