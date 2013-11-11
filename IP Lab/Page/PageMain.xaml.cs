using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using SilverlightMenu.Library;

using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.ServiceModel;
using System.Runtime.InteropServices.Automation;

using IP_Lab.Data;
using IP_Lab.Device;
using IP_Lab.Data.Server;
using IP_Lab.Data.Message;
using IP_Lab.MyService;

namespace IP_Lab.Page
{
    public partial class PageMain : UserControl
    {
        #region 私有变量

        private bool mouseDragDevice = false;
        private DispatcherTimer dispatcherTimer;    // 定时器，用于每秒的更新
        private Data.SystemData sysData;            // 系统数据
        private Data.UserData usrData;

        private Menu.WS_Function menu;

        private WndServerTopo topoWindow;           // 标准拓扑子窗口

        #endregion

        #region 属性

        Canvas _gridLinesContainer;
        Canvas GridLinesContainer
        {
            get
            {
                if (_gridLinesContainer == null)
                {

                    Canvas temCan = new Canvas();
                    temCan.Name = "canGridLinesContainer";
                    PaintBoard.Children.Add(temCan);
                    _gridLinesContainer = temCan;

                }
                return _gridLinesContainer;
            }
        }

        #endregion

        public PageMain()
        {
            InitializeComponent();
            Init();
        }

        #region 初始化
        protected void Init()
        {
            Init_Params();
            Init_DeviceList();
            Init_Control();
            Init_DispatchTimer();
            Init_ContextMenu();
        }

        /// <summary>
        /// 初始化部分参数
        /// </summary>
        protected void Init_Params()
        {
            // 一些需要使用到的系统数值
            sysData = Util.SingletonGeneric<Data.SystemData>.GetInstance;
            usrData = Util.SingletonGeneric<Data.UserData>.GetInstance;
            menu = new Menu.WS_Function();

            // 子窗口
            topoWindow = new WndServerTopo();
            topoWindow.Closed += new EventHandler(topoWindow_Closed);

            // 数值初始化
            sysData.Command = Enum.CommandType.COMMAND_TYPE_SELECT;
            sysData.HWversion = "57";

            tbLabIndex.Text = "尊敬的 " + usrData.UserName + " 同学，欢迎进入 " + usrData.LabIndex + " 实验室";
        }

        /// <summary>
        /// 初始化左侧设备列表
        /// </summary>
        protected void Init_DeviceList()
        {
            lstDevice.ItemsSource = Device.DeviceList.getDeviceIcoList();
            btnSelect.IsEnabled = false;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        protected void Init_Control()
        {
            // 画布默认为 1280 * 1024
            PaintBoard.Width = 1280;
            PaintBoard.Height = 1024;
        }

        protected void ResetPaint()
        {
            int count = sysData.DeviceList.Count;
            // 首先删除该图元的所有连线
            for (int i = 0; i < count; i++)
            {
                sysData.DeviceList[0].DeleteSelf();
            }
            sysData.ResetSystemData();
        }

        #endregion

        #region 右键菜单
        private void Init_ContextMenu()
        {
            ContextMenu cm = new ContextMenu();

            System.Windows.Controls.MenuItem mi_powerup = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem mi_stop = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem mi_login = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem mi_import = new System.Windows.Controls.MenuItem();
            System.Windows.Controls.MenuItem mi_export = new System.Windows.Controls.MenuItem();

            mi_powerup.Header = "启动/重启";
            mi_stop.Header = "停止";
            mi_login.Header = "登录";
            mi_import.Header = "导入配置";
            mi_export.Header = "导出配置";

            mi_powerup.Click += new RoutedEventHandler(mi_powerup_Click);
            mi_stop.Click += new RoutedEventHandler(mi_stop_Click);
            mi_login.Click += new RoutedEventHandler(mi_login_Click);
            mi_import.Click += new RoutedEventHandler(mi_import_Click);
            mi_export.Click += new RoutedEventHandler(mi_export_Click);

            cm.Items.Add(mi_powerup);
            cm.Items.Add(mi_stop);
            cm.Items.Add(mi_login);
            cm.Items.Add(mi_import);
            cm.Items.Add(mi_export);

            ContextMenuService.SetContextMenu(PaintBoard, cm);  //为控件绑定右键菜单
        }

        private void mi_powerup_Click(object sender, RoutedEventArgs e)
        {
            if (sysData.SelectedDeviceList.Count == 0)
            {
                MessageBox.Show("请选择需要启动的设备！");
                return;
            }
            menu.StartDevice(sysData.SelectedDeviceList);
        }

        private void mi_stop_Click(object sender, RoutedEventArgs e)
        {
            if (sysData.SelectedDeviceList.Count == 0)
            {
                MessageBox.Show("请选择需要停止的设备！");
                return;
            }
            menu.StopDevice(sysData.SelectedDeviceList);
        }

        private void mi_login_Click(object sender, RoutedEventArgs e)
        {
            if (sysData.SelectedDeviceList.Count == 0)
            {
                MessageBox.Show("请选择需要登陆的设备！");
                return;
            }
            DeviceBase dev = sysData.SelectedDeviceList[0];
            string index = "" + dev.DP.Index;
            if (dev.DP.Index < 10)
                index = "0" + index;
            var u = new Uri(dev.DP.Prefix.ToLower() + index + "://" + dev.DP.Prefix.ToLower() + index, UriKind.RelativeOrAbsolute);
            System.Windows.Browser.HtmlPage.Window.Navigate(u, "_blank");
        }

        private void mi_import_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mi_export_Click(object sender, RoutedEventArgs e)
        {
        }

        private void LayoutRoot_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
        #endregion

        #region 拖拽图标
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDragDevice = true;
            Image image = sender as Image;
            Img.Source = image.Source;
            Img.Tag = image.Tag;

            Point point = e.GetPosition(lstDevice);
            Img.SetValue(Canvas.LeftProperty, point.X);
            Img.SetValue(Canvas.TopProperty, point.Y);

            Img.Visibility = Visibility.Visible;

            // 清除当前选中图标
            sysData.ClearSelectedControl();

        }

        private void PaintBoard_MouseMove(object sender, MouseEventArgs e)
        {
            // 让图片跟随鼠标的移动而移动
            if (mouseDragDevice)
            {
                Point point = e.GetPosition(PaintBoard);

                Img.SetValue(Canvas.LeftProperty, point.X);
                Img.SetValue(Canvas.TopProperty, point.Y);
            }
        }

        private void PaintBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(PaintBoard);

            // 在点击画布空白处时，清除选中
            if (sysData.IsDeviceInPosition(point) == null)
                sysData.ClearSelectedControl();

            // 在点击画布空白处时，清除选中
            if (sysData.IsLineInPosition(point) == null)
                sysData.ClearSelectedLink();
        }

        private void PaintBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mouseDragDevice)
            {
                Point point = e.GetPosition(PaintBoard);

                Type type = Type.GetType(Img.Tag.ToString(), true);
                Device.DeviceBase device = (Device.DeviceBase)Activator.CreateInstance(type);
                device.Init();

                device.SetPosition(point.X, point.Y);
                device.SetParent(PaintBoard);
                device.SetDeviceName(device.DP.Name);
                device.SetSysData();

                if (device.AllowToCreate)
                    PaintBoard.Children.Add(device);
                else
                    MessageBox.Show("可用数量已达最大！");

                PaintBoard.MouseMove -= PaintBoard_MouseMove;
                mouseDragDevice = false;
                Img.Visibility = Visibility.Collapsed;
                Img.Source = null;
            }
        }

        private void PaintBoard_MouseEnter(object sender, MouseEventArgs e)
        {
            // 拖拽图标进入画布时，添加移动事件
            if (mouseDragDevice)
            {
                PaintBoard.MouseMove += PaintBoard_MouseMove;
            }
        }

        private void PaintBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            if (mouseDragDevice)
            {
                PaintBoard.MouseMove -= PaintBoard_MouseMove;
                mouseDragDevice = false;
                Img.Visibility = Visibility.Collapsed;
                Img.Source = null;
            }
        }

        #endregion

        #region 心跳线程
        public void Init_DispatchTimer()
        {
            dispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (DeviceBase device in Util.SingletonGeneric<SystemData>.GetInstance.DeviceList)
            {
                device.Update(1);
            }
        }
        #endregion

        #region 自适应窗口大小
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width = 0;
            double height = 0;

            // 获取浏览器大小
            if (!App.Current.Host.Content.IsFullScreen)
            {
                width = App.Current.Host.Content.ActualWidth;
                height = App.Current.Host.Content.ActualHeight;
            }

            CanvaGrid.Width = width;

            lstDevice.Width = 120;
            lstDevice.Height = height - 40 - 75;

            spDetail.Height = height - 40 - 75;

            svPaintBoard.Width = width - 120 - 5 - 200; // 120为左侧宽度，150为右侧宽度，5为调节
            svPaintBoard.Height = height - 40 - 75;

            PaintBoard.Width = (width - 120 - 5 - 200) > 1024 ? (width - 120 - 5 - 200) : 1024;
            PaintBoard.Height = (height - 40 - 75) > 768 ? (height - 40 - 75) : 768;

            SliWidth.Minimum = PaintBoard.Width;
            SliHeight.Minimum = PaintBoard.Height;
        }
        #endregion

        #region PaintBoard的参数调整

        // 绘制网格线
        private void SetGridLines()
        {
            if (!CbShowGridLines.IsChecked.HasValue || !CbShowGridLines.IsChecked.Value)
                return;
            GridLinesContainer.Children.Clear();
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = Color.FromArgb(255, 160, 160, 160);

            double thickness = 0.3;
            double top = 0;
            double left = 0;

            double width = PaintBoard.Width;
            double height = PaintBoard.Height;

            double stepLength = 40;

            double x, y;
            x = left + stepLength;
            y = top;

            while (x < width + left)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x;
                line.Y2 = y + height;



                line.Stroke = brush;
                line.StrokeThickness = thickness;
                line.Stretch = Stretch.Fill;
                GridLinesContainer.Children.Add(line);
                x += stepLength;
            }


            x = left;
            y = top + stepLength;

            while (y < height + top)
            {
                Line line = new Line();
                line.X1 = x;
                line.Y1 = y;
                line.X2 = x + width;
                line.Y2 = y;


                line.Stroke = brush;
                line.Stretch = Stretch.Fill;
                line.StrokeThickness = thickness;
                GridLinesContainer.Children.Add(line);
                y += stepLength;
            }
        }

        private void CbShowGridLines_Click(object sender, RoutedEventArgs e)
        {
            if (CbShowGridLines.IsChecked.HasValue && CbShowGridLines.IsChecked.Value)
            {
                SetGridLines();
            }
            else
            {
                if (GridLinesContainer != null)
                    GridLinesContainer.Children.Clear();
            }
        }

        private void SliWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PaintBoard != null)
            {
                PaintBoard.Width = SliWidth.Value;
                SetGridLines();
            }
        }

        private void SliHeight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PaintBoard != null)
            {
                PaintBoard.Height = SliHeight.Value;
                SetGridLines();
            }
        }

        private void CbHWversion55_Click(object sender, RoutedEventArgs e)
        {
            if (CbHWversion55.IsChecked.HasValue && CbHWversion55.IsChecked.Value)
            {
                sysData.HWversion = "55";
                CbHWversion57.IsChecked = false;
            }
            else
            {
                sysData.HWversion = "57";
                CbHWversion57.IsChecked = true;
            }
        }

        private void CbHWversion57_Click(object sender, RoutedEventArgs e)
        {
            if (CbHWversion57.IsChecked.HasValue && CbHWversion57.IsChecked.Value)
            {
                sysData.HWversion = "57";
                CbHWversion55.IsChecked = false;
            }
            else
            {
                sysData.HWversion = "55";
                CbHWversion55.IsChecked = true;
            }
        }

        #endregion

        #region 命令模式选择

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            sysData.ClearSelectedControl();
            sysData.Command = Enum.CommandType.COMMAND_TYPE_SELECT;

            btnSelect.IsEnabled = false;
            btnConnect.IsEnabled = true;
        }
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            sysData.ClearSelectedControl();
            sysData.Command = Enum.CommandType.COMMAND_TYPE_CONNECT;

            btnSelect.IsEnabled = true;
            btnConnect.IsEnabled = false;
        }

        #endregion

        #region 命令菜单

        private void mnuIPLab_MenuItemClicked(object sender, EventArgs e)
        {
            try
            {
                var clickedItem = (SilverlightMenu.Library.MenuItem)sender;
                switch (clickedItem.Name)
                {
                    case "mnu_newtopo":
                        {
                            ResetPaint();
                        }
                        break;
                    case "mnu_opentopo":
                        {
                            var dialog = new OpenFileDialog { Multiselect = false };
                            if (dialog.ShowDialog() == true)
                            {
                                using (var stream = dialog.File.OpenRead())
                                {
                                    var buffer = new byte[stream.Length];
                                    stream.Read(buffer, 0, buffer.Length);
                                    string encryptdata = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                                    string data = Util.Function.Decrypt(
                                        encryptdata, ConfigurationManager.AppSettings["EncryptPwd"]);
                                    OpenTopoFromXml(XDocument.Parse(data));
                                }
                            }
                        }
                        break;
                    case "mnu_savetopo":
                        {
                            var dialog = new SaveFileDialog { DefaultExt = "topo" };
                            if (dialog.ShowDialog() == true)
                            {
                                using (var stream = dialog.OpenFile())
                                {
                                    Byte[] fileContent = Encoding.UTF8.GetBytes(
                                        Util.Function.Encrypt(GenerateTopoXml().ToString(),
                                        ConfigurationManager.AppSettings["EncryptPwd"]));
                                    stream.Write(fileContent, 0, fileContent.Length);
                                    stream.Close();
                                }
                            }
                            MessageBox.Show("保存成功!");
                        }
                        break;
                    case "mnu_openservertopo":
                        {
                            topoWindow.GetServerTopo();
                            topoWindow.Show();
                        }
                        break;
                    case "mnu_topogenconfig":
                        menu.GenerateConfigFile();
                        break;
                    case "mnu_topostartall":
                        menu.StartDevice(sysData.DeviceList);
                        break;
                    case "mnu_topostopall":
                        menu.StopDevice(sysData.DeviceList);
                        break;
                    case "mnu_filecompare":
                        {
                            WndFileCompare wnd = new WndFileCompare();
                            wnd.Show();
                        }
                        break;
                    case "mnu_vpn":
                        {
                            var u = new Uri(Util.Function.GetWebAddress() + 
                                ConfigurationManager.AppSettings["VPN"] + "VPN_" + usrData.LabIndex + ".exe", UriKind.RelativeOrAbsolute);
                            System.Windows.Browser.HtmlPage.Window.Navigate(u, "_blank");
                        }
                        break;
                    case "mnu_crt":
                        {
                            var u = new Uri(Util.Function.GetWebAddress() + ConfigurationManager.AppSettings["SecureCRT"], UriKind.RelativeOrAbsolute);
                            System.Windows.Browser.HtmlPage.Window.Navigate(u, "_blank");
                        }
                        break;
                    case "mnu_ViewHelp":
                        {
                            Windows.WndAbout about = new Windows.WndAbout();
                            about.Show();
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region 按键命令

        private void LayoutRoot_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        if (sysData.SelectedDeviceList.Count > 0)
                        {
                            int count = sysData.SelectedDeviceList.Count;
                            for (int i = 0; i < count; i++)
                            {
                                DeviceBase device = sysData.SelectedDeviceList[0];
                                sysData.RemoveSelectedControl(device);
                                device.DeleteSelf();
                            }
                        }
                        if (sysData.SelectedLink != null)
                        {
                            Link link = sysData.SelectedLink;
                            sysData.ClearSelectedLink();
                            link.DeleteSelf();
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region 拓扑打开、保存时XML文件生成、解析

        #region 保存

        /// <summary>
        /// 保存拓扑配置
        /// </summary>
        /// <returns>生成的XDocument文档</returns>
        protected XDocument GenerateTopoXml()
        {
            XElement root = new XElement("IP_Lab");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            root.Add(GenerateSettingXml());
            root.Add(GenerateDevicesXml());
            root.Add(GenerateLinksXml());

            return xdoc;
        }

        /// <summary>
        /// 画布设置： 宽、高、是否显示网格
        /// </summary>
        protected XElement GenerateSettingXml()
        {
            XElement setting = new XElement("Setting");

            XElement canvasHeight = new XElement("CanvasHeight", (int)SliHeight.Value);
            XElement canvasWidth = new XElement("CanvasWidth", (int)SliWidth.Value);
            XElement showGrid = new XElement("Grid", CbShowGridLines.IsChecked);

            setting.Add(canvasHeight);
            setting.Add(canvasWidth);
            setting.Add(showGrid);

            return setting;
        }

        /// <summary>
        /// 生成设备信息
        /// </summary>
        protected XElement GenerateDevicesXml()
        {
            XElement devs = new XElement("Devices");

            foreach (DeviceBase db in sysData.DeviceList)
            {
                XElement dev = new XElement("Device");

                XElement type = new XElement("Type", db.ToString());
                XElement location = new XElement("Location", (int)db.GetPosition().Left + "," + (int)db.GetPosition().Top);
                XElement index = new XElement("Index", db.DP.Index);

                dev.Add(type);
                dev.Add(location);
                dev.Add(index);

                devs.Add(dev);
            }

            return devs;
        }

        /// <summary>
        /// 生成连线信息
        /// </summary>
        protected XElement GenerateLinksXml()
        {
            XElement lnks = new XElement("Links");

            foreach (Link l in sysData.LinkList)
            {
                XElement lnk = new XElement("Link");

                XElement srcname = new XElement("SrcDevice", l.SrcDevice.DP.Name);
                XElement srccard = new XElement("SrcDeviceCard", l.SrcCard.toString());
                XElement srcsock = new XElement("SrcDeviceSocket", l.SrcSocket.toString());
                XElement srcoffx = new XElement("SrcDeviceOffsetX", (int)l.PTSrcOffset.X);
                XElement srcoffy = new XElement("SrcDeviceOffsetY", (int)l.PTSrcOffset.Y);

                XElement dstname = new XElement("DstDevice", l.DstDevice.DP.Name);
                XElement dstcard = new XElement("DstDeviceCard", l.DstCard.toString());
                XElement dstsock = new XElement("DstDeviceSocket", l.DstSocket.toString());
                XElement dstoffx = new XElement("DstDeviceOffsetX", (int)l.PTDstOffset.X);
                XElement dstoffy = new XElement("DstDeviceOffsetY", (int)l.PTDstOffset.Y);

                lnk.Add(srcname);
                lnk.Add(srccard);
                lnk.Add(srcsock);
                lnk.Add(srcoffx);
                lnk.Add(srcoffy);

                lnk.Add(dstname);
                lnk.Add(dstcard);
                lnk.Add(dstsock);
                lnk.Add(dstoffx);
                lnk.Add(dstoffy);

                lnks.Add(lnk);
            }

            return lnks;
        }

        #endregion

        #region 打开

        protected void OpenTopoFromXml(XDocument xdoc)
        {
            try
            {
                ResetPaint();

                XElement root = xdoc.Element("IP_Lab");

                ResetGlobalSettingFromXml(root.Element("Setting"));
                LoadDevicesFromXml(root.Element("Devices"));
                LoadLinksXml(root.Element("Links"));

                InvalidateArrange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw new Exception("配置打开失败，请勿修改配置文件！");
            }
        }

        protected void ResetGlobalSettingFromXml(XElement xele)
        {
            int canvasHeight = Int32.Parse(xele.Element("CanvasHeight").Value);
            int canvasWidth = Int32.Parse(xele.Element("CanvasWidth").Value);
            bool showGrid = Boolean.Parse(xele.Element("Grid").Value);

            if (canvasWidth > SliWidth.Maximum)
            {
                SliWidth.Maximum = canvasWidth;
            }

            if (canvasHeight > SliHeight.Maximum)
            {
                SliHeight.Maximum = canvasHeight;
            }

            SliWidth.Value = canvasWidth;
            SliHeight.Value = canvasHeight;

            if (showGrid)
            {
                CbShowGridLines.IsChecked = true;
                SetGridLines();
            }
        }

        protected void LoadDevicesFromXml(XElement xele)
        {
            IEnumerable<XElement> elements =
                from el in xele.Elements("Device")
                select el;


            foreach (XElement ele in elements)
            {
                int index = Int32.Parse(ele.Element("Index").Value);
                string location = ele.Element("Location").Value;
                string[] position = location.Split(',');

                Type type = Type.GetType(ele.Element("Type").Value, true);
                Device.DeviceBase device = (Device.DeviceBase)Activator.CreateInstance(type);
                device.Init(index);

                device.SetPosition(Double.Parse(position[0]), Double.Parse(position[1]));
                device.SetParent(PaintBoard);
                device.SetDeviceName(device.DP.Name);
                device.SetSysData();

                if (device.AllowToCreate)
                    PaintBoard.Children.Add(device);
                else
                    MessageBox.Show("可用数量已达最大！");
            }
        }

        protected void LoadLinksXml(XElement xele)
        {
            IEnumerable<XElement> elements =
                from el in xele.Elements("Link")
                select el;


            foreach (XElement ele in elements)
            {
                string strsrcname = ele.Element("SrcDevice").Value;
                string strsrccard = ele.Element("SrcDeviceCard").Value;
                string strsrcsock = ele.Element("SrcDeviceSocket").Value;
                string srcoffx = ele.Element("SrcDeviceOffsetX").Value;
                string srcoffy = ele.Element("SrcDeviceOffsetY").Value;

                string strdstname = ele.Element("DstDevice").Value;
                string strdstcard = ele.Element("DstDeviceCard").Value;
                string strdstsock = ele.Element("DstDeviceSocket").Value;
                string dstoffx = ele.Element("DstDeviceOffsetX").Value;
                string dstoffy = ele.Element("DstDeviceOffsetY").Value;

                // 解析设备数据
                DeviceBase src = sysData.GetDeviceByName(strsrcname);
                DeviceCard srccard = src.DP.getCardByStr(strsrccard);
                DeviceSocket srcsock = srccard.getSocketByStr(strsrcsock);
                Point pt1 = new Point(Double.Parse(srcoffx), Double.Parse(srcoffy));

                DeviceBase dst = sysData.GetDeviceByName(strdstname);
                DeviceCard dstcard = dst.DP.getCardByStr(strdstcard);
                DeviceSocket dstsock = dstcard.getSocketByStr(strdstsock);
                Point pt2 = new Point(Double.Parse(dstoffx), Double.Parse(dstoffy));

                src.AddLink(pt1, pt2, srccard, dstcard, srcsock, dstsock, dst);
            }
        }

        #endregion

        #region 打开服务器标准拓扑

        void topoWindow_Closed(object sender, EventArgs e)
        {
            if ((bool)topoWindow.DialogResult)
            {
                try
                {
                    MessageSend cmd = new MessageSend();
                    cmd.Command = "WSGetServerTopoData";

                    PLabWebServiceSoapClient getServer = new PLabWebServiceSoapClient();
                    getServer.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                    /*getServer.Endpoint.Address = new EndpointAddress("http://localhost:1549/IP%20Lab.Web/" + ConfigurationManager.AppSettings["WebService"]);*/
                    getServer.ExecuteCommandCompleted += new EventHandler<ExecuteCommandCompletedEventArgs>(getServer_ExecuteCommandCompleted);
                    getServer.ExecuteCommandAsync(cmd.MsgSendToXMLStr(), topoWindow.tbSelectTopo.Text);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.InnerException.Message);
                }
            }
        }

        void getServer_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs e)
        {
            string encryptdata = e.Result.ToString();
            string data = Util.Function.Decrypt(
                encryptdata, ConfigurationManager.AppSettings["EncryptPwd"]);
            OpenTopoFromXml(XDocument.Parse(data));
            MessageBox.Show("打开成功！");
        }

        #endregion

        #endregion

    }
}
