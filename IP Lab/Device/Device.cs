
//*******************************************************************
//                                                                  *
//                  仿真系统的设备类                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class Device : DeviceImage
    {
        #region 私有变量


        #endregion

        #region 属性

        public DeviceProperty DP;

        #endregion

        #region 初始化函数

        public Device()
        {
            Init_Property();
            Init_Control();
        }

        /// <summary>
        /// 属性，变量的初始化
        /// </summary>
        protected virtual void Init_Property()
        {
            DP = new DeviceProperty();
        }

        #endregion

    }
}
