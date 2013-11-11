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
using System.ServiceModel;

using IP_Lab.Data;
using IP_Lab.Data.Server;
using IP_Lab.Data.Message;
using IP_Lab.MyService;

namespace IP_Lab
{
    public partial class WndServerTopo : ChildWindow
    {
        private DateTime _lastClick = DateTime.Now;     // 鼠标双击事件
        private bool _firstClickDone;

        public WndServerTopo()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 查找服务器上的标准拓扑
        /// </summary>
        public void GetServerTopo()
        {
            try
            {
                MessageSend cmd = new MessageSend();
                cmd.Command = "WSGetServerTopo";

                PLabWebServiceSoapClient getServer = new PLabWebServiceSoapClient();
                getServer.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                /*getServer.Endpoint.Address = new EndpointAddress("http://localhost:1549/IP%20Lab.Web/" + ConfigurationManager.AppSettings["WebService"]);*/
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
                    MessageBox.Show(e.Result);
                    return;
                }
                string topos = e.Result.ToString();
                char[] split = { ';' };

                string[] topo = topos.Split(split, StringSplitOptions.RemoveEmptyEntries);
                Topo[] top = new Topo[topo.Length];

                for (int i = 0; i < topo.Length; i++)
                {
                    Topo t = new Topo();
                    t.TopoImgPath = "../Resource/Image/topo.png";
                    t.TopoName = topo[i];

                    top[i] = t;
                }

                lstServerTopoList.ItemsSource = top;
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
                tbSelectTopo.Text = img.Tag.ToString();

                _firstClickDone = false;

                this.DialogResult = true;
            }
        }
    }
}

