using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using IP_Lab.Windows;

//*******************************************************************
//                                                                  *
//                  仿真系统的设备类                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceBase : Canvas
    {
        #region 私有变量

        private double txtOffsetX;          // 文字相对于图片的坐标的偏移
        private double txtOffsetY;
        private double clickOffsetX;        // 在控件上点击时点击位置相对于左上角的便宜坐标
        private double clickOffsetY;

        private Canvas paintBoard;          // 该图元所在的画布

        private DateTime _lastClick = DateTime.Now;     // 鼠标双击事件
        private bool _firstClickDone;

        private bool _selected;

        private Data.SystemData sysData;        // 系统数据

        private DeviceBase destination;         // 连线时的终点
        private List<Link> linkList;

        private WndDeviceLink linkWindow;       // 连线子窗口

        #endregion

        #region 属性变量

        public double ImageWidth { get; set; }              // 宽
        public double ImageHeight { get; set; }             // 高
        public BitmapImage IcoImage { get; set; }           // 图源
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (_selected)
                    ImageBorder.BorderThickness = new Thickness(1);
                else
                    ImageBorder.BorderThickness = new Thickness(0);
            }
        }
        #endregion

        #region 控件

        protected Border ImageBorder;    // 图片边框
        protected Image ImageSource;    // 图片
        protected TextBlock TextDeviceName; // 设备名称
        protected Ellipse ImageStatues;   // 设备状态
        protected Line LineVirtual;    // 连接时显示的虚线

        #endregion

        #region 初始化函数
        public DeviceBase()
        {
            
        }

        public void Init(int index = 1)
        {
            Init_Property(index);
            Init_CardSocket();
            Init_Control();
            Init_Event();
        }

        /// <summary>
        /// 属性，变量的初始化
        /// </summary>
        protected virtual void Init_Property(int index = 1)
        {
            DP = new DeviceProperty();
            DP.State = Enum.DeviceState.DEVICE_STATE_INITIALIZE;

            linkList = new List<Link>();
            linkWindow = new WndDeviceLink();
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        protected virtual void Init_Control()
        {
            // 图片
            ImageSource = new Image();
            ImageSource.Source = IcoImage;
            ImageSource.Width = ImageWidth;
            ImageSource.Height = ImageHeight;

            // 图片的边框
            ImageBorder = new Border();
            ImageBorder.BorderBrush = new SolidColorBrush(Colors.Black);
            ImageBorder.BorderThickness = new Thickness(0);
            ImageBorder.CornerRadius = new CornerRadius(3);
            ImageBorder.Width = ImageSource.Width;
            ImageBorder.Height = ImageSource.Height;
            ImageBorder.Child = ImageSource;

            // 设备名
            TextDeviceName = new TextBlock();
            TextDeviceName.FontSize = 14;
            TextDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            TextDeviceName.VerticalAlignment = VerticalAlignment.Center;
            TextDeviceName.TextAlignment = TextAlignment.Center;

            // 设备当前状态
            ImageStatues = new Ellipse();
            ImageStatues.Fill = new SolidColorBrush(Colors.Green);
            ImageStatues.Height = 12;
            ImageStatues.Width = 12;

            // 连接时显示的虚线
            LineVirtual = new Line();
            LineVirtual.Stroke = new SolidColorBrush(Colors.Black);
            LineVirtual.StrokeThickness = 2;
            DoubleCollection dc = new DoubleCollection();
            dc.Add(1);
            dc.Add(1);
            LineVirtual.StrokeDashArray = dc;

            // 设置ZIndex
            ImageBorder.SetValue(Canvas.ZIndexProperty, -64);
            TextDeviceName.SetValue(Canvas.ZIndexProperty, 64);

            this.Children.Add(ImageBorder);
            this.Children.Add(TextDeviceName);
            this.Children.Add(ImageStatues);
            this.Children.Add(LineVirtual);
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        protected virtual void Init_Event()
        {
            ImageBorder.MouseLeftButtonDown += new MouseButtonEventHandler(ImageBorder_MouseLeftButtonDown);
            ImageBorder.MouseLeftButtonUp += new MouseButtonEventHandler(ImageBorder_MouseLeftButtonUp);

            linkWindow.Closed += new EventHandler(linkWindow_Closed);
        }

        /// <summary>
        /// 初始化板卡列表
        /// </summary>
        protected virtual void Init_CardSocket()
        {
        }


        #endregion

        #region 函数方法

        public virtual void Update(float timeSinceLastFrame)
        {
            //             if (DP.State == Enum.DeviceState.DEVICE_STATE_INITIALIZE)
            //                 return;

            if (ImageStatues.Visibility == Visibility.Visible)
                ImageStatues.Visibility = Visibility.Collapsed;
            else
                ImageStatues.Visibility = Visibility.Visible;
        }
        public virtual void SetPosition(double x, double y)
        {
            ImageBorder.SetValue(Canvas.LeftProperty, x);
            ImageBorder.SetValue(Canvas.TopProperty, y);

            ImageStatues.SetValue(Canvas.LeftProperty, x + ImageWidth);
            ImageStatues.SetValue(Canvas.TopProperty, y - 12);

            TextDeviceName.SetValue(Canvas.LeftProperty, x + ImageWidth / 4);
            TextDeviceName.SetValue(Canvas.TopProperty, y + ImageHeight);
        }
        public virtual Rect GetPosition()
        {
            Rect rect = new Rect();
            rect.X = (double)ImageBorder.GetValue(Canvas.LeftProperty);
            rect.Y = (double)ImageBorder.GetValue(Canvas.TopProperty);

            rect.Width = ImageWidth;
            rect.Height = ImageHeight;

            return rect;
        }
        public void SetParent(Canvas parent)
        {
            paintBoard = parent;
        }
        public void SetDeviceName(string name)
        {
            TextDeviceName.Text = name;
        }
        public void SetSysData()
        {
            sysData = Util.SingletonGeneric<Data.SystemData>.GetInstance;
        }
        public double GetTxtHeight()
        {
            return TextDeviceName.ActualHeight;
        }

        public void RemoveLink(Link link)
        {
            if (linkList.Contains(link))
                linkList.Remove(link);
        }

        public List<Link> GetLinkList()
        {
            return linkList;
        }
        private bool SetDeviceSocketUsed(DeviceCard card, DeviceSocket socket, DeviceBase device)
        {
            if (card != null && socket != null)
            {
                socket.Used = true;
                card.Socket_Used_Number++;
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// 添加连接，之所以使用偏移而不使用起始点，是因为在保存文件时，double会导致精度问题，从而点跑出框外
        /// </summary>
        /// <param name="pt1">起点偏移</param>
        /// <param name="pt2">终点偏移</param>
        public void AddLink(Point pt1, Point pt2,
            DeviceCard srcCard, DeviceCard dstCard, DeviceSocket srcSocket, DeviceSocket dstSocket, DeviceBase dest)
        {
            Link lnk = new Link();


            lnk.PTSrcOffset = pt1;
            lnk.PTDstOffset = pt2;

            lnk.PTSRC = new Point(
                pt1.X + (double)ImageBorder.GetValue(Canvas.LeftProperty),
                pt1.Y + (double)ImageBorder.GetValue(Canvas.TopProperty));

            lnk.PTDST = new Point(
                pt2.X + (double)dest.ImageBorder.GetValue(Canvas.LeftProperty),
                pt2.Y + (double)dest.ImageBorder.GetValue(Canvas.TopProperty));

            lnk.PaintBoard = paintBoard;

            lnk.SrcDevice = this;
            lnk.SrcCard = srcCard;
            lnk.SrcSocket = srcSocket;

            lnk.DstDevice = dest;
            lnk.DstCard = dstCard;
            lnk.DstSocket = dstSocket;

            lnk.TextSrc.Text = srcCard.toString();
            if (srcCard.ShowSocket)
                lnk.TextSrc.Text += "/" + srcSocket.toString();

            lnk.TextDst.Text = dstCard.toString();
            if (dstCard.ShowSocket)
                lnk.TextDst.Text += "/" + dstSocket.toString();

            try
            {
                if (SetDeviceSocketUsed(srcCard, srcSocket, this)
                && SetDeviceSocketUsed(dstCard, dstSocket, dest))
                {
                    linkList.Add(lnk);
                    dest.GetLinkList().Add(lnk);

                    paintBoard.Children.Add(lnk);
                    AdjustLinkPosition(lnk);

                    destination = null;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #region 删除控件

        public void DeleteSelf()
        {
            try
            {
                int count = linkList.Count;
                // 首先删除该图元的所有连线
                for (int i = 0; i < count; i++)
                {
                    // 因为每次删除之后，List都会自适应调整容器里的内容
                    // 故每次删除第一个元素
                    linkList[0].DeleteSelf();
                }
                paintBoard.Children.Remove(this);
                Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList.Remove(this);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        #region 移动时更新位置
        /// <summary>
        /// 根据ptA, ptB的坐标调整line坐标，主要用于裁剪掉位于线框内部的线段
        /// </summary>
        /// <param name="ptA">D1的坐标</param>
        /// <param name="ptB">D2的坐标</param>
        /// <param name="line">连接D1,D2的连线</param>
        protected void AdjustLinkPosition(Link link)
        {
            Rect rtA = link.SrcDevice.GetPosition();
            Rect rtB = link.DstDevice.GetPosition();

            Point ptA = new Point(rtA.Left + link.PTSrcOffset.X, rtA.Top + link.PTSrcOffset.Y);
            Point ptB = new Point(rtB.Left + link.PTDstOffset.X, rtB.Top + link.PTDstOffset.Y);

            Line l = Util.Function.CalCrossPoint(ptA, ptB, rtA, rtB);
            link.PTSRC = new Point(l.X1, l.Y1);
            link.PTDST = new Point(l.X2, l.Y2);

            link.adjustTextPosition();
        }
        /// <summary>
        /// 移动时更新连接的坐标
        /// </summary>
        public void AdjustLinkPosition()
        {
            // 调整与该设备相连的连接坐标
            if (linkList.Count != 0)
            {
                foreach (Link lnk in linkList)
                {
                    AdjustLinkPosition(lnk);
                }
            }
        }
        #endregion

        #endregion

        #region 事件函数

        private void InjectMoveEvent()
        {
            paintBoard.MouseMove += paintBoard_MouseMove;
            paintBoard.MouseLeave += paintBoard_MouseLeave;
            ImageBorder.CaptureMouse();

            ImageBorder.Cursor = Cursors.Hand;
        }

        private void DejectMoveEvent()
        {
            paintBoard.MouseMove -= paintBoard_MouseMove;
            paintBoard.MouseLeave -= paintBoard_MouseLeave;
            ImageBorder.ReleaseMouseCapture();

            ImageBorder.Cursor = Cursors.Arrow;
        }

        void ImageBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TimeSpan span = DateTime.Now - _lastClick;
            if (span.TotalMilliseconds > 300 || _firstClickDone == false)
            {
                _firstClickDone = true;
                _lastClick = DateTime.Now;

                // 记录下鼠标点击时的偏移位置
                Point point = e.GetPosition(ImageBorder);
                clickOffsetX = point.X;
                clickOffsetY = point.Y;

                InjectMoveEvent();

                if (sysData.Command == Enum.CommandType.COMMAND_TYPE_SELECT)
                {
                    sysData.SetSelectedDevice(this);
                }
                else if (sysData.Command == Enum.CommandType.COMMAND_TYPE_CONNECT)
                {
                    point = e.GetPosition(paintBoard);

                    ImageBorder.Cursor = Cursors.Stylus;

                    LineVirtual.X1 = point.X;
                    LineVirtual.Y1 = point.Y;
                    LineVirtual.X2 = point.X;
                    LineVirtual.Y2 = point.Y;
                    LineVirtual.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("Image double cilcked !");
            }
        }

        void ImageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(paintBoard);

            if (sysData.Command == Enum.CommandType.COMMAND_TYPE_CONNECT)
            {
                LineVirtual.Visibility = Visibility.Collapsed;

                destination = sysData.IsDeviceInPosition(point);
                if (destination != null && destination != this)
                {
                    linkWindow.Title = "您将连接设备 " + DP.Name + " 和 " + destination.DP.Name;
                    if(linkWindow.setDevice(this, destination))
                        linkWindow.Show();
                }
            }

            DejectMoveEvent();
        }

        void paintBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(paintBoard);

            if (point.X < ImageBorder.Width || point.Y < ImageBorder.Height
                || (point.X > paintBoard.Width - ImageBorder.Width)
                || (point.Y > paintBoard.Height - ImageBorder.Height))
                return;

            if (sysData.Command == Enum.CommandType.COMMAND_TYPE_SELECT)
            {
                SetPosition(point.X - clickOffsetX,
                    point.Y - clickOffsetY);
                AdjustLinkPosition();
            }
            else if (sysData.Command == Enum.CommandType.COMMAND_TYPE_CONNECT)
            {
                LineVirtual.X2 = point.X;
                LineVirtual.Y2 = point.Y;
            }
        }

        void paintBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            DejectMoveEvent();
        }

        void linkWindow_Closed(object sender, EventArgs e)
        {
            if ((bool)linkWindow.DialogResult)
            {
                DeviceCard srcCard = DP.getCardByStr((string)linkWindow.cmbSrcCard.SelectedValue);
                DeviceCard dstCard = destination.DP.getCardByStr((string)linkWindow.cmbDstCard.SelectedValue);

                DeviceSocket srcSocket = srcCard.getSocketByStr((string)linkWindow.cmbSrcSocket.SelectedValue);
                DeviceSocket dstSocket = dstCard.getSocketByStr((string)linkWindow.cmbDstSocket.SelectedValue);

                // 如果不显示端口信息的板卡，则此处的Socket为null
                if (srcCard.ShowSocket == false)
                    srcSocket = srcCard.getSocketByStr("0");
                if (dstCard.ShowSocket == false)
                    dstSocket = dstCard.getSocketByStr("0");

                if (srcCard.Card_Type == dstCard.Card_Type)
                {
                    AddLink(
                        new Point(
                            LineVirtual.X1 - (double)ImageBorder.GetValue(Canvas.LeftProperty),
                            LineVirtual.Y1 - (double)ImageBorder.GetValue(Canvas.TopProperty)),
                        new Point(
                            LineVirtual.X2 - (double)destination.ImageBorder.GetValue(Canvas.LeftProperty),
                            LineVirtual.Y2 - (double)destination.ImageBorder.GetValue(Canvas.TopProperty)),
                        srcCard,
                        dstCard,
                        srcSocket,
                        dstSocket,
                        destination
                        );
                }
                else
                {
                    MessageBox.Show("您所连接网口类型不一致！请重新选择");
                }
            }
        }

        #endregion

        #region 属性

        public DeviceProperty DP;
        public bool AllowToCreate = true;       //当前设备是否允许创建（数量限制）

        #endregion

    }
}
