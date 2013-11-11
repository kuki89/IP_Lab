using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

//*******************************************************************
//                                                                  *
//                  仿真系统的设备图元基类                            *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceImage : Canvas
    {
        #region 私有变量

        private double txtOffsetX;          // 文字相对于图片的坐标的偏移
        private double txtOffsetY;
        private double clickOffsetX;        // 在控件上点击时点击位置相对于左上角的便宜坐标
        private double clickOffsetY;

        private Canvas paintBoard;          // 该图元所在的画布

        private DateTime _lastClick = DateTime.Now;     // 鼠标双击事件
        private bool _firstClickDone;

        #endregion

        #region 属性变量

        public double ImageWidth { get; set; }              // 宽
        public double ImageHeight { get; set; }             // 高
        public BitmapImage IcoImage { get; set; }           // 图源

        #endregion

        #region 控件

        protected Border ImageBorder;
        protected Image ImageSource;
        protected TextBlock TextDeviceName;
        protected Ellipse ImageStatues;

        #endregion

        #region 初始化函数

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
            ImageStatues.Fill = new SolidColorBrush(Colors.Gray);
            ImageStatues.Height = 12;
            ImageStatues.Width = 12;

            // 设置ZIndex
            ImageBorder.SetValue(Canvas.ZIndexProperty, -64);
            TextDeviceName.SetValue(Canvas.ZIndexProperty, 64);

            this.Children.Add(ImageBorder);
            this.Children.Add(TextDeviceName);
            this.Children.Add(ImageStatues);
        }

        protected virtual void Init_Event()
        {
            ImageBorder.MouseLeftButtonDown += new MouseButtonEventHandler(ImageBorder_MouseLeftButtonDown);
            ImageBorder.MouseLeftButtonUp += new MouseButtonEventHandler(ImageBorder_MouseLeftButtonUp);
        }

        
        #endregion

        #region 函数方法

        public virtual void Update(float timeSinceLastFrame)
        {
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
        public virtual void SetParent(Canvas parent)
        {
            paintBoard = parent;
        }
        public virtual void SetDeviceName(string name)
        {
            TextDeviceName.Text = name;
        }

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
            }
            else
            {
                MessageBox.Show("Image double cilcked !");
            }
        }


        void ImageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DejectMoveEvent();
        }

        void paintBoard_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(paintBoard);

            if (point.X < ImageBorder.Width || point.Y < ImageBorder.Height
                || (point.X > paintBoard.Width - ImageBorder.Width)
                || (point.Y > paintBoard.Height - ImageBorder.Height))
                return;

            SetPosition(point.X - clickOffsetX,
                point.Y - clickOffsetY);
        }

        void paintBoard_MouseLeave(object sender, MouseEventArgs e)
        {
            DejectMoveEvent();
        }

        #endregion
    }
}
