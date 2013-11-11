using System.Collections.Generic;

//*******************************************************************
//                                                                  *
//                  仿真系统的设备属性信息                            *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceProperty
    {
        #region 属性

        public List<DeviceCard> Card_List;

        public string Product { get; set; }       // 设备厂家
        public Enum.DeviceType Type { get; set; } // 设备类型
        public string Name { get; set; }          // 设备名称
        public string Prefix { get; set; }        // 设备前缀
        public int Index { get; set; }            // 设备内部编号
        public int GUID { get; set; }             // 设备全局编号

        public Enum.DeviceState State { get; set; }// 设备当前状态

        #endregion

        #region 方法

        public DeviceProperty()
        {
            Card_List = new List<DeviceCard>();
        }

        public DeviceCard getCardByStr(string str)
        {
            foreach (DeviceCard card in Card_List)
            {
                if (card.toString() == str)
                    return card;
            }
            return null;
        }

        #endregion
    }
}
