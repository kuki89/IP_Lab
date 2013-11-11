using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

//*******************************************************************
//                                                                  *
//                  仿真系统的设备列表                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceList
    {
        #region 属性
        public string IcoTag { get; set; }
        public string IcoText { get; set; }
        public BitmapImage IcoImage { get; set; }
        #endregion

        #region 公共方法
        public static List<DeviceList> getDeviceIcoList()
        {
            List<DeviceList> deviceIcoList = new List<DeviceList>()
            {
                new DeviceList(){ IcoTag="IP_Lab.Device.Device_CiscoRouter", IcoText="思科路由器",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/cisco-router.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_CiscoSwitch", IcoText="思科交换机",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/cisco-switch.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_HuaWei", IcoText="华为路由器",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/hw_router.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_Juniper", IcoText="Juniper",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/juniper.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_ASAFirewall", IcoText="防火墙",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/firewall.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_Windows", IcoText="Windows",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/server.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_Linux", IcoText="Linux",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/linux.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},

                new DeviceList(){ IcoTag="IP_Lab.Device.Device_Vpcs", IcoText="VPCS",
                    IcoImage=new BitmapImage(
                        new Uri(string.Format(@"/{0};component/Resource/Image/vpcs.png", 
                            Util.Function.GetProjectName()), UriKind.Relative))},
            };
            return deviceIcoList;
        }
        #endregion
    }
}
