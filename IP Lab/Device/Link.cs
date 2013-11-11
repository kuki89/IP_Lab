using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//*******************************************************************
//                                                                  *
//                      仿真系统的中继类                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class Link : Canvas
    {
        #region 私有变量

        private Point ptSrc, ptDst;             // 直线的坐标
        private Point ptSrcOffset, ptDstOffset; // 直线相对于设备的偏移

        #endregion

        #region 属性

        public Point PTSRC
        {
            get { return ptSrc; }
            set
            {
                ptSrc = value;
                LineSrcToDst.X1 = ptSrc.X;
                LineSrcToDst.Y1 = ptSrc.Y;
            }
        }
        public Point PTDST
        {
            get { return ptDst; }
            set
            {
                ptDst = value;
                LineSrcToDst.X2 = ptDst.X;
                LineSrcToDst.Y2 = ptDst.Y;
            }
        }
        public Point PTSrcOffset
        {
            get { return ptSrcOffset; }
            set { ptSrcOffset = value; }
        }
        public Point PTDstOffset
        {
            get { return ptDstOffset; }
            set { ptDstOffset = value; }
        }
        public Canvas PaintBoard { get; set; }

        public DeviceBase SrcDevice { get; set; }
        public DeviceCard SrcCard { get; set; }
        public DeviceSocket SrcSocket { get; set; }

        public DeviceBase DstDevice { get; set; }
        public DeviceCard DstCard { get; set; }
        public DeviceSocket DstSocket { get; set; }

        public int GUID { get; set; }

        #endregion

        #region 控件

        public TextBlock TextSrc;      // 连接设备1的端口
        public TextBlock TextDst;      // 连接设备2的端口
        public Line LineSrcToDst;      // 连接线

        #endregion

        #region 初始化

        public Link()
        {
            Init_Control();
            Init_Event();

            Util.SingletonGeneric<Data.SystemData>.GetInstance.LinkList.Add(this);
            GUID = ++Util.SingletonGeneric<Data.SystemData>.GetInstance.LINKGUID;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        protected void Init_Control()
        {
            TextSrc = new TextBlock();
            TextDst = new TextBlock();
            TextSrc.FontSize = 10;
            TextDst.FontSize = 10;

            LineSrcToDst = new Line();
            LineSrcToDst.Stroke = new SolidColorBrush(Colors.Black);
            LineSrcToDst.StrokeThickness = 2;


            this.Children.Add(TextSrc);
            this.Children.Add(TextDst);
            this.Children.Add(LineSrcToDst);
        }

        /// <summary>
        /// 初始化事件
        /// </summary>
        protected void Init_Event()
        {
            LineSrcToDst.MouseLeftButtonDown += new MouseButtonEventHandler(LineSrcToDst_MouseLeftButtonDown);
        }

        #endregion

        #region 直线样式控制

        public void SetSelectedStyle()
        {
            LineSrcToDst.StrokeThickness = 4;
            LineSrcToDst.Stroke = new SolidColorBrush(Color.FromArgb(255, 66, 132, 176));

            TextSrc.FontSize = 16;
            TextDst.FontSize = 16;
            TextSrc.Foreground = new SolidColorBrush(Colors.Red);
            TextDst.Foreground = new SolidColorBrush(Colors.Red);
            TextSrc.SetValue(Canvas.ZIndexProperty, 128);
            TextDst.SetValue(Canvas.ZIndexProperty, 128);
        }

        public void SetUnSelectedStyle()
        {
            LineSrcToDst.StrokeThickness = 2;
            LineSrcToDst.Stroke = new SolidColorBrush(Colors.Black);

            TextSrc.FontSize = 10;
            TextDst.FontSize = 10;
            TextSrc.Foreground = new SolidColorBrush(Colors.Black);
            TextDst.Foreground = new SolidColorBrush(Colors.Black);
            TextSrc.SetValue(Canvas.ZIndexProperty, 0);
            TextDst.SetValue(Canvas.ZIndexProperty, 0);
        }

        #endregion

        #region 删除该控件

        public void DeleteSelf()
        {
            SrcDevice.RemoveLink(this);
            DstDevice.RemoveLink(this);

            SrcSocket.Used = false;
            DstSocket.Used = false;

            SrcCard.Socket_Used_Number--;
            DstCard.Socket_Used_Number--;

            Util.SingletonGeneric<Data.SystemData>.GetInstance.LinkList.Remove(this);
            Util.SingletonGeneric<Data.SystemData>.GetInstance.ClearSelectedLink();

            if (Util.SingletonGeneric<Data.SystemData>.GetInstance.SelectedLink == this)
                Util.SingletonGeneric<Data.SystemData>.GetInstance.ClearSelectedLink();

            PaintBoard.Children.Remove(this);
        }

        #endregion

        #region 事件处理函数

        protected void LineSrcToDst_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Util.SingletonGeneric<Data.SystemData>.GetInstance.Command == Enum.CommandType.COMMAND_TYPE_SELECT)
            {
                Util.SingletonGeneric<Data.SystemData>.GetInstance.SetSelectedLink(this);
            }
        }

        #endregion

        #region 调整直线上文字位置
        /// <summary>
        /// 调整文字的位置，使其随着直线旋转并且不被遮挡
        /// </summary>
        public void adjustTextPosition()
        {
            adjustTextPositionByPoint(TextSrc, PTSRC, SrcDevice);
            adjustTextPositionByPoint(TextDst, PTDST, DstDevice);
        }
        private void adjustTextPositionByPoint(TextBlock txt, Point pt, DeviceBase dev)
        {
            int flag = PointInLine(pt, dev);
            switch (flag)
            {
                case 0:
                    txt.SetValue(Canvas.LeftProperty, pt.X);
                    txt.SetValue(Canvas.TopProperty, pt.Y - txt.ActualHeight - 10);
                    break;
                case 1:
                    txt.SetValue(Canvas.LeftProperty, pt.X - txt.ActualWidth - 20);
                    txt.SetValue(Canvas.TopProperty, pt.Y);
                    break;
                case 2:
                    txt.SetValue(Canvas.LeftProperty, pt.X);
                    txt.SetValue(Canvas.TopProperty, pt.Y + dev.GetTxtHeight());
                    break;
                case 3:
                    txt.SetValue(Canvas.LeftProperty, pt.X);
                    txt.SetValue(Canvas.TopProperty, pt.Y);
                    break;
            }
        }
        /// <summary>
        /// 判断一个点位于矩形的哪条直线
        /// </summary>
        /// <param name="pt">点</param>
        /// <returns>
        ///       0
        ///    --------
        ///   |        | 
        /// 1 |        | 3
        ///   |        |
        ///    --------
        ///       2
        /// </returns>
        private int PointInLine(Point pt, DeviceBase dev)
        {
            Rect rect = dev.GetPosition();
            if (Math.Abs(pt.Y - rect.Top) < 1)
                return 0;
            else if (Math.Abs(pt.X - rect.Left) < 1)
                return 1;
            else if (Math.Abs(pt.Y - rect.Bottom) < 1)
                return 2;
            else
                return 3;
        }
        #endregion
    }
}
