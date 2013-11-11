//*******************************************************************
//                                                                  *
//                      仿真系统的Enum定义                       *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Enum
{
    /// <summary>
    /// 仿真系统的设备列表以及其Index
    /// </summary>
    public enum DeviceType
    {
        DEVICE_TYPE_VPCS = 1,
        DEVICE_TYPE_HUAWEI = 2,
        DEVICE_TYPE_CISCOROUTER = 3,
        DEVICE_TYPE_CISCOSWITCHER = 7,
        DEVICE_TYPE_JUNIPER = 5,
        DEVICE_TYPE_ASAFIREWALL = 6,       
        DEVICE_TYPE_WINDOWS = 8,
        DEVICE_TYPE_LINUX = 9,
        DEVICE_TYPE_NONE
    }

    /// <summary>
    /// 设备状态
    /// </summary>
    //*******************************************************************
    //                         状态转换图
    //                                      Stop                        *
    //                                      ︿ |                        *
    //                                      | ﹀                        *
    //      Initial  --- >  Config  --- >   Run  --- >  Login      *
    //                                           < ---                 *
    //                                                                  *
    //*******************************************************************
    public enum DeviceState
    {
        DEVICE_STATE_INITIALIZE,    // 设备刚刚从列表拖出，创建时
        DEVICE_STATE_CONFIG,        // 在服务器上生成了配置文件
        DEVICE_STATE_RUN,           // 设备已启动
        DEVICE_STATE_STOP,          // 设备已关闭
        DEVICE_STATE_LOGIN          // 用户已登录设备
    }

    /// <summary>
    /// 板卡类型
    /// </summary>
    public enum DeviceCardType
    {
        DEVICE_CARD_TYPE_ETHERNET,
        DEVICE_CARD_TYPE_SERIAL
    }

    public enum CommandType
    {
        COMMAND_TYPE_SELECT,
        COMMAND_TYPE_CONNECT,
        COMMAND_TYPE_NONE
    }

    /// <summary>
    /// 配置文件类型
    /// </summary>
    public enum FileBaseType
    {
        FILE_BASE_TYPE_CONFIG,             // 配置文件
        FILE_BASE_TYPE_START,              // 启动设备文件
        FILE_BASE_TYPE_STARTALL,           // 启动同一类型的所有设备
        FILE_BASE_TYPE_START_AND_ALL       // 该设备没有StartAll文件时，需要一个一个启动
    }
}
