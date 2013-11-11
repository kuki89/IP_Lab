using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    启动VPCS脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class VpcsLocalStartFile : FileBase
    {
        public VpcsLocalStartFile(string floder)
        {
            device = null;
            deviceType = (int)Enum.DeviceType.DEVICE_TYPE_VPCS;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_CONFIG;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "local_start_vpcs";   // 文件名称
            filePath = "lab_script/" + floder + "/vpcs";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "#!/bin/bash" + "\\n";
            fileContent += "ps auxww | grep \"\\-p 1999\" | grep -v \"grep\" | awk '{print $2}' | xargs kill -9" + "\\n";
            fileContent += "/home/lab_admin/public/vpcs/vpcs -p 1999 -r /home/lab_admin/lab_script/" + floder + "/vpcs/lab_vpcs.cfg" + "\\n";
        }

        public override string toString()
        {
            return fileContent;
        }
    }
}
