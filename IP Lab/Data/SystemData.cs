using System.Collections.Generic;
using IP_Lab.Device;
using IP_Lab.Enum;
using System.Windows;

//*******************************************************************
//                                                                  *
//              用于保存一些系统需要使用到的的数据                      *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Data
{
    public class SystemData
    {
        /// <summary>
        /// web service 的 IP地址
        /// </summary>
        public string ServiceAddress { get; set; }

        /// <summary>
        /// 每台设备的全局ID
        /// </summary>
        public int GUID { get; set; }

        /// <summary>
        /// 端口的GUID
        /// </summary>
        public int SOCKETGUID { get; set; }

        /// <summary>
        /// 中继的GUID
        /// </summary>
        public int LINKGUID { get; set; }

        /// <summary>
        /// 每种设备允许的最大数量
        /// </summary>
        public int[] DeviceMaxCount;

        /// <summary>
        /// 当前的命令模式
        /// </summary>
        public Enum.CommandType Command { get; set; }

        /// <summary>
        /// 已有设备列表
        /// </summary>
        public List<DeviceBase> DeviceList;

        /// <summary>
        /// 当前选中的设备列表
        /// </summary>
        public List<DeviceBase> SelectedDeviceList;

        /// <summary>
        /// 已有中继连接列表
        /// </summary>
        public List<Link> LinkList;

        /// <summary>
        /// 当前选中的中继
        /// </summary>
        public Link SelectedLink { get; set; }

        /// <summary>
        /// 华为版本
        /// </summary>
        public string HWversion { get; set; }

        public SystemData()
        {
            DeviceList = new List<DeviceBase>();
            LinkList = new List<Link>();
            SelectedDeviceList = new List<DeviceBase>();

            GUID = 0;
            SOCKETGUID = 0;
            DeviceMaxCount = new int[(int)DeviceType.DEVICE_TYPE_NONE];

            SetConfigData();
        }

        public void ResetSystemData()
        {
            DeviceList.Clear();
            LinkList.Clear();
            SelectedDeviceList.Clear();

            GUID = 0;
            SOCKETGUID = 0;
        }

        /// <summary>
        /// 设置一些系统的参数
        /// </summary>
        public void SetConfigData()
        {
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_VPCS] = 10;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_HUAWEI] = 10;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_CISCOROUTER] = 20;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_JUNIPER] = 1;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_ASAFIREWALL] = 1;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_CISCOSWITCHER] = 10;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_WINDOWS] = 2;
            DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_LINUX] = 2;
        }

        #region 设备数据操作

        public void SetSelectedDevice(DeviceBase device)
        {
            ClearSelectedControl();
            AddSelectedControl(device);
        }
        public void AddSelectedControl(DeviceBase device)
        {
            if (device != null && !SelectedDeviceList.Contains(device))
            {
                SelectedDeviceList.Add(device);
                device.Selected = true;
            }
        }
        public void RemoveSelectedControl(DeviceBase device)
        {
            if (SelectedDeviceList.Contains(device))
            {
                SelectedDeviceList.Remove(device);
                device.Selected = false;
            }
        }
        public void ClearSelectedControl()
        {
            if (SelectedDeviceList.Count > 0)
            {
                int count = SelectedDeviceList.Count;
                for (int i = 0; i < count; i++)
                {
                    RemoveSelectedControl(SelectedDeviceList[0]);
                }
            }
        }
        public DeviceBase IsDeviceInPosition(Point point)
        {
            foreach (DeviceBase device in DeviceList)
            {
                Rect rect = device.GetPosition();
                if (rect.Contains(point))
                    return device;
            }
            return null;
        }

        public Link IsLineInPosition(Point point)
        {
            foreach (Link link in LinkList)
            {
                Point pStart = new Point(link.LineSrcToDst.X1, link.LineSrcToDst.Y1);
                Point pEnd = new Point(link.LineSrcToDst.X2, link.LineSrcToDst.Y2);

                if (Util.Function.HitLine(point, pStart, pEnd, 8))
                    return link;
            }
            return null;
        }
        public void SetSelectedLink(Link link)
        {
            SetSelectedDevice(null);
            ClearSelectedLink();

            SelectedLink = link;
            SelectedLink.SetSelectedStyle();
        }
        public void ClearSelectedLink()
        {
            if (SelectedLink != null)
            {
                SelectedLink.SetUnSelectedStyle();
                SelectedLink = null;
            }
        }

        public DeviceBase GetDeviceByName(string name)
        {
            foreach (DeviceBase dev in DeviceList)
            {
                if (dev.DP.Name == name)
                    return dev;
            }
            return null;
        }

        #endregion
    }
}
