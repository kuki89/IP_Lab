using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

//*******************************************************************
//                                                                  *
//                         Juniper                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class Device_Juniper : DeviceBase
    {
        protected override void Init_Property(int index = 1)
        {
            base.Init_Property();

            // 数量监测
            List<DeviceBase> list = Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList;
            int curCount = 0;
            foreach (DeviceBase dev in list)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_JUNIPER)
                    curCount++;
            }

            int maxCount = Util.SingletonGeneric<Data.SystemData>.GetInstance.
                DeviceMaxCount[(int)Enum.DeviceType.DEVICE_TYPE_JUNIPER];
            if (curCount == maxCount)
            {
                AllowToCreate = false;
                return;
            }

            // 为当前将要新添加的设备查找可使用的index
            int[] device = new int[maxCount + 1];
            foreach (DeviceBase dev in list)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_JUNIPER)
                    device[dev.DP.Index] = 1;
            }
            for (int j = index; j <= maxCount; j++)
            {
                if (device[j] == 0)
                {
                    DP.Index = j;
                    break;
                }
            }

            DP.Product = "Juniper";
            DP.Type = Enum.DeviceType.DEVICE_TYPE_JUNIPER;
            DP.Prefix = "RJ";

            DP.GUID = ++Util.SingletonGeneric<Data.SystemData>.GetInstance.GUID;

            DP.Name = DP.Prefix + DP.Index;

            Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList.Add(this);
        }

        protected override void Init_Control()
        {
            ImageWidth = 51;
            ImageHeight = 55;

            IcoImage = new BitmapImage(new Uri(
                string.Format(@"/{0};component/Resource/Image/juniper.png",
                Util.Function.GetProjectName()), UriKind.Relative));


            base.Init_Control();
        }

        protected override void Init_CardSocket()
        {
            for (int i = 0; i < 8; i++)
            {
                DP.Card_List.Add(new DeviceCard(
                    Enum.DeviceCardType.DEVICE_CARD_TYPE_ETHERNET,
                    "EM", i, 1, false));
            }

            for (int i = 0; i < 8; i++)
            {
                DP.Card_List.Add(new DeviceCard(
                    Enum.DeviceCardType.DEVICE_CARD_TYPE_ETHERNET,
                    "FXP", i, 1, false));
            }
        }
    }
}
