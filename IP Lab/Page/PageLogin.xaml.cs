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
using IP_Lab.Util;
using IP_Lab.Util.Navigation;
using IP_Lab.Util.Navigation.Transitions;


namespace IP_Lab.Page
{
    public partial class PageLogin : UserControl
    {
        Util.IndentifyCode icc;
        public PageLogin()
        {
            InitializeComponent();
            GenerateCode();
        }

        private void hlbResetCode_Click(object sender, RoutedEventArgs e)
        {
            GenerateCode();
        }

        protected void GenerateCode()
        {
            // 生成验证码
            icc = new Util.IndentifyCode();
            icc.CreatImage(icc.CreateIndentifyCode(4), imgCode, 80, 40);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txbName.Text == "" || psw.Password == "" || txbCode.Text == "")
            {
                MessageBox.Show("请输入必要信息");
                return;
            }
            if (txbCode.Text.ToUpper() == icc.Randomcode.ToUpper())
            {
                // 保存用户信息
                SingletonGeneric<Data.UserData>.GetInstance.UserName = txbName.Text;
                SingletonGeneric<Data.UserData>.GetInstance.Password = psw.Password;
                SingletonGeneric<Data.UserData>.GetInstance.LoginTime = DateTime.Now;

                TransitionBase transition = new CompositeTransition(
                new FadeTransition(TimeSpan.FromSeconds(1)),
                new WipeTransition(WipeTransition.WipeDirection.RightToLeft));
                NavigationHelper.Navigate(transition, new PageLabSelect());
            }
            else
            {
                MessageBox.Show("验证码输入错误，请重新输入");
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            txbName.Text = "";
            psw.Password = "";
            txbCode.Text = "";

            GenerateCode();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnLogin_Click(sender, new RoutedEventArgs());
        }
    }
}
