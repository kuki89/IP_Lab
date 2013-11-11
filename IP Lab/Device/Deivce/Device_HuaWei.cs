using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

//*******************************************************************
//                                                                  *
//                              华为                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class Device_HuaWei : DeviceBase
    {
        protected override void Init_Property(int index = 1)
        {
            base.Init_Property();

            // 数量监测
            List<DeviceBase> list = Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList;
            int curCount = 0;
            foreach (DeviceBase dev in list)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_HUAWEI)
                    curCount++;
            }

            int maxCount = Util.SingletonGeneric<Data.SystemData>.GetInstance.
                DeviceMaxCount[(int)Enum.DeviceType.DEVICE_TYPE_HUAWEI];
            if (curCount == maxCount)
            {
                AllowToCreate = false;
                return;
            }

            // 为当前将要新添加的设备查找可使用的index
            int[] device = new int[maxCount + 1];
            foreach (DeviceBase dev in list)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_HUAWEI)
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

            DP.Product = "华为";
            DP.Type = Enum.DeviceType.DEVICE_TYPE_HUAWEI;
            DP.Prefix = "RH";

            DP.GUID = ++Util.SingletonGeneric<Data.SystemData>.GetInstance.GUID;

            DP.Name = DP.Prefix + DP.Index;

            Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList.Add(this);
        }

        protected override void Init_Control()
        {
            ImageWidth = 58;
            ImageHeight = 43;            

            IcoImage = new BitmapImage(new Uri(
                string.Format(@"/{0};component/Resource/Image/hw_router.png", 
                Util.Function.GetProjectName()), UriKind.Relative));


            base.Init_Control();
        }

        protected override void Init_CardSocket()
        {
            // 为华为添加2块板卡，每块板有10个端口
            DP.Card_List.Add(new DeviceCard(
                Enum.DeviceCardType.DEVICE_CARD_TYPE_ETHERNET,
                "E0/", 0, 16, true));
            DP.Card_List.Add(new DeviceCard(
                Enum.DeviceCardType.DEVICE_CARD_TYPE_SERIAL,
                "S0/", 0, 16, true));
        }
    }
}
