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
    public class VpcsStartFile : FileBase
    {
        public VpcsStartFile(string floder)
        {
            device = null;
            deviceType = (int)Enum.DeviceType.DEVICE_TYPE_VPCS;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_STARTALL ;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "start_vpcs";   // 文件名称
            filePath = "lab_script/" + floder + "/vpcs";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "#!/bin/bash" + "\\n";
            fileContent += "ssh -tt 127.0.0.1 << EOF" + "\\n";
            fileContent += "/home/lab_admin/lab_script/" + floder + "/vpcs/local_start_vpcs && exit" + "\\n";
            fileContent += "EOF" + "\\n";

            fileStop = "ps auxww | grep \"\\-p 1999\" | grep -v \"grep\" | awk '{print $2}' | xargs kill -9";

            fileReturn = "Now VPCS is startup";
        }

        public override string toString()
        {
            return fileContent + "echo \"Now VPCS is startup\"";
        }
    }
}
