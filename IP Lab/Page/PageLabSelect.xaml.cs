using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using IP_Lab.Data;
using IP_Lab.Data.Server;
using IP_Lab.Data.Message;
using IP_Lab.MyService;

using IP_Lab.Util;
using IP_Lab.Util.Navigation;
using IP_Lab.Util.Navigation.Transitions;

using System.ServiceModel;

namespace IP_Lab.Page
{
    public partial class PageLabSelect : UserControl
    {
        private DateTime _lastClick = DateTime.Now;     // 鼠标双击事件
        private bool _firstClickDone;

        public PageLabSelect()
        {
            InitializeComponent();
            GetUsableServers();
        }

        /// <summary>
        /// 返回登录界面
        /// </summary>
        private void btnReturn_Click(object sender, RoutedEventArgs e)
        {
            TransitionBase transition = new FadeTransition(TimeSpan.FromSeconds(1));
            NavigationHelper.Navigate(transition, new Page.PageLogin());
        }

        /// <summary>
        /// 自适应窗口大小
        /// </summary>
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = 0;
            double height = 0;

            if (!App.Current.Host.Content.IsFullScreen)
            {
                width = App.Current.Host.Content.ActualWidth;
                height = App.Current.Host.Content.ActualHeight;
            }

            lstServerList.Width = width - 100;
            lstServerList.Height = height - 120;
        }

        /// <summary>
        /// 刷新服务器列表
        /// </summary>
        private void btnFlush_Click(object sender, RoutedEventArgs e)
        {
            GetUsableServers();
        }

        /// <summary>
        /// 获取可使用的服务器
        /// </summary>
        private void GetUsableServers()
        {
            try
            {
                MessageSend cmd = new MessageSend();
                cmd.Command = "WSGetUsableServer";

                PLabWebServiceSoapClient getServer = new PLabWebServiceSoapClient();
                getServer.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                getServer.ExecuteCommandCompleted +=
                    new EventHandler<ExecuteCommandCompletedEventArgs>(getServer_ExecuteCommandCompleted);
                getServer.ExecuteCommandAsync(cmd.MsgSendToXMLStr(), "");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }   
        }

        void getServer_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Contains("false - "))
                {
                    MessageBox.Show("目前尚无可用实验室");
                    return;
                }
                string servers = e.Result.ToString();
                char[] split = { ';'};

                string[] ser = servers.Split(split, StringSplitOptions.RemoveEmptyEntries);
                Server[] server = new Server[ser.Length];

                for (int i = 0; i < ser.Length; i++)
                {
                    Server s = new Server();
                    s.ServerImgPath = "../Resource/Image/server.png";
                    s.ServerIP = ser[i];
                    s.ServerName = ser[i];

                    server[i] = s;
                }

                lstServerList.ItemsSource = server;
                txtServerCnt.Text = "当前空闲实验室：  " + ser.Length;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }   
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TimeSpan span = DateTime.Now - _lastClick;
            if (span.TotalMilliseconds > 300 || _firstClickDone == false)
            {
                _firstClickDone = true;
                _lastClick = DateTime.Now;
            }
            else
            {
                Image img = (Image)sender;

                // 保存所选择的信息
                string lab_ip = img.Tag.ToString();
                SingletonGeneric<Data.UserData>.GetInstance.LabIP = lab_ip;
                SingletonGeneric<Data.UserData>.GetInstance.LabIndex = Int32.Parse(lab_ip.Substring(lab_ip.LastIndexOf('.') + 1));

                TransitionBase transition = new RotateTransition();
                NavigationHelper.Navigate(transition, new PageMain());
                
                _firstClickDone = false;
            }
        }
    }
}
