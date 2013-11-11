using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                 华为路由器复位重启所有设备                         *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HWResetAllFile : FileBase
    {
        public HWResetAllFile(string floder, string ver)
        {
            device = null;
            deviceType = (int)Enum.DeviceType.DEVICE_TYPE_HUAWEI;
            localPortBase = deviceType * 1000;
            version = ver;

            fileType = FileBaseType.FILE_BASE_TYPE_STARTALL;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "reset_all_v" + version;   // 文件名称
            filePath = "lab_script/" + floder + "/huawei";

            // 在这里，每行末尾添加\n
            fileContent = "#!/bin/bash" + "\\n";
            fileContent += "ps aux | grep wvrp | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "sleep 5" + "\\n";
            startTimeOut += 10;

            GenerateDeviceData(floder);

            fileReturn = "all HuaWei Router are active now, enjoy yourself.";
            fileStop = "ps aux | grep wvrp | awk '{print $2}' | xargs kill" + "\\n";
        }

        protected void GenerateDeviceData(string floder)
        {
            foreach (DeviceBase device in Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList)
            {
                if (device.DP.Type == DeviceType.DEVICE_TYPE_HUAWEI)
                {
                    fileContent += "/home/lab_admin/lab_script/" + floder + "/huawei/rt" + device.DP.Index
                        + "/reset_rt" + device.DP.Index + "_v" + version + "\\n";
                }
            }
        }

        public override string toString()
        {
            return fileContent + "echo \"all HuaWei Router are active now, enjoy yourself.\"";
        }

    }
}
