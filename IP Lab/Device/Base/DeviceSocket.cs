//*******************************************************************
//                                                                  *
//                  仿真系统的设备的端口信息                            *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class DeviceSocket
    {
        #region 属性

        public int Number { get; set; }    // 端口编号:从0开始
        public bool Used { get; set; }    // 端口是否使用
        public int GUID { get; set; }

        #endregion

        #region 函数

        public string toString()
        {
            return Number.ToString();
        }

        #endregion
    }
}
