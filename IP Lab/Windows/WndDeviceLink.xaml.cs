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
using IP_Lab.Device;

namespace IP_Lab.Windows
{
    public partial class WndDeviceLink : ChildWindow
    {
        private DeviceBase deviceSrc;
        private DeviceBase deviceDst;

        public WndDeviceLink()
        {
            InitializeComponent();
        }

        public bool setDevice(DeviceBase src, DeviceBase dst)
        {
            Reset_Window();

            deviceSrc = src;
            deviceDst = dst;

            tbSrc.Text = src.DP.Name;
            tbDst.Text = dst.DP.Name;

            return Init_SlotCmd();
        }

        private void Reset_Window()
        {
            cmbSrcCard.Items.Clear();
            cmbSrcSocket.Items.Clear();
            cmbDstCard.Items.Clear();
            cmbDstSocket.Items.Clear();
        }

        private bool Init_SlotCmd()
        {
            foreach (DeviceCard card in deviceSrc.DP.Card_List)
            {
                // 该板卡还没使用完
                if (card.Socket_Used_Number < card.Socket_Total_Number)
                {
                    cmbSrcCard.Items.Add(card.toString());
                }
            }
            foreach (DeviceCard card in deviceDst.DP.Card_List)
            {
                // 该板卡还没使用完
                if (card.Socket_Used_Number < card.Socket_Total_Number)
                {
                    cmbDstCard.Items.Add(card.toString());
                }
            }

            if (cmbSrcCard.Items.Count == 0)
            {
                MessageBox.Show(deviceSrc.DP.Name + " 剩余可用端口不足 ！ ");
                DialogResult = false;
                Close();
                return false;
            }
            if (cmbDstCard.Items.Count == 0)
            {
                MessageBox.Show(deviceDst.DP.Name + " 剩余可用端口不足 ！ ");
                DialogResult = false;
                Close();
                return false;
            }

            cmbSrcCard.SelectionChanged += new SelectionChangedEventHandler(cmbSrcCard_SelectionChanged);
            cmbDstCard.SelectionChanged += new SelectionChangedEventHandler(cmbDstCard_SelectionChanged);

            cmbSrcCard.SelectedIndex = 0;
            cmbDstCard.SelectedIndex = 0;

            return true;
        }

        void cmbSrcCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbSrcSocket.Items.Clear();
            cmbSrcSocket.IsEnabled = true;

            string cardStr = (string)(((ComboBox)sender).SelectedValue);
            DeviceCard card = deviceSrc.DP.getCardByStr(cardStr);
            if (card != null)
            {
                SetSrcControl(card.ShowSocket);
                if (card.ShowSocket == false)
                {
                    cmbSrcSocket.IsEnabled = false;
                    return;

                }
                if (card.Socket_Used_Number < card.Socket_Total_Number)
                {
                    foreach (DeviceSocket socket in card.Socket_List)
                    {
                        if (socket.Used == false)
                            cmbSrcSocket.Items.Add(socket.toString());
                    }
                    cmbSrcSocket.SelectedIndex = 0;
                }
            }
        }

        void cmbDstCard_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbDstSocket.Items.Clear();
            cmbDstSocket.IsEnabled = true;

            string cardStr = (string)(((ComboBox)sender).SelectedValue);
            DeviceCard card = deviceDst.DP.getCardByStr(cardStr);
            if (card != null)
            {
                SetDstControl(card.ShowSocket);
                if (card.ShowSocket == false)
                {
                    cmbDstSocket.IsEnabled = false;
                    return;
                }

                if (card.Socket_Used_Number < card.Socket_Total_Number)
                {
                    foreach (DeviceSocket socket in card.Socket_List)
                    {
                        if (socket.Used == false)
                            cmbDstSocket.Items.Add(socket.toString());
                    }
                    cmbDstSocket.SelectedIndex = 0;
                }
            }
        }

        private void SetSrcControl(bool showSocket)
        {
            if (showSocket)
            {
                tbSrcCardText.Text = "板卡";
                tbSrcPortText.Text = "端口";
                tbSrcPortText.Visibility = Visibility.Visible;
                cmbSrcSocket.Visibility = Visibility.Visible;
            }
            else
            {
                tbSrcCardText.Text = "端口";
                tbSrcPortText.Visibility = Visibility.Collapsed;
                cmbSrcSocket.Visibility = Visibility.Collapsed;
            }
        }

        private void SetDstControl(bool showSocket)
        {
            if (showSocket)
            {
                tbDstCardText.Text = "板卡";
                tbDstPortText.Text = "端口";
                tbDstPortText.Visibility = Visibility.Visible;
                cmbDstSocket.Visibility = Visibility.Visible;
            }
            else
            {
                tbDstCardText.Text = "端口";
                tbDstPortText.Visibility = Visibility.Collapsed;
                cmbDstSocket.Visibility = Visibility.Collapsed;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

