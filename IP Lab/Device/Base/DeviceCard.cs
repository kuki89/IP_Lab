using System.Collections.Generic;
//*******************************************************************
//                                                                  *
//                  仿真系统的设备的板卡信息                            *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceCard
    {
        #region 属性

        public int Number { get; set; }                     // 板卡编号:从0开始
        public int Socket_Used_Number { get; set; }         // 端口使用数量
        public int Socket_Total_Number { get; set; }        // 端口总数量
        public Enum.DeviceCardType Card_Type { get; set; }  // 板卡的类型，分为：以太网接口以及广域网接口
        public string Card_Prefix { get; set; }             // 板卡的前缀: E、S、FXP、EM
        public List<DeviceSocket> Socket_List;

        public bool ShowSocket;                             // 是否显示端口，主要是用于那些只有一个端口的板卡，例如FXP0 ~ 8
                                                            // 其中，FXP为前缀， 0 ~ 8 为 Number

        #endregion

        #region 函数

        public DeviceCard(Enum.DeviceCardType type, string prefix, int number, int socketnumber, bool showsocket)
        {
            Number = number;
            Socket_Used_Number = 0;
            Socket_Total_Number = socketnumber;
            Card_Type = type;
            Card_Prefix = prefix;
            ShowSocket = showsocket;

            // 往插槽上添加端口
            Socket_List = new List<DeviceSocket>();
            for (int i = 0; i < socketnumber; i++)
            {
                Socket_List.Add(new DeviceSocket() 
                { Number = i, Used = false, GUID = ++Util.SingletonGeneric<Data.SystemData>.GetInstance.SOCKETGUID });
            }
        }


        public string toString()
        {
            return Card_Prefix + Number;
        }

        public DeviceSocket getSocketByStr(string str)
        {
            foreach (DeviceSocket socket in Socket_List)
            {
                if (socket.toString() == str)
                    return socket;
            }
            return null;
        }

        #endregion

    }
}
