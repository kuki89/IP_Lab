using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    华为交换机重置文件脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HWResetFile : FileBase
    {
        public HWResetFile(DeviceBase dev, string floder, string ver)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;
            version = ver;

            fileType = FileBaseType.FILE_BASE_TYPE_START;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "reset_rt" + device.DP.Index + "_v" + version;
            filePath = "lab_script/" + floder + "/huawei/rt" + device.DP.Index;

            int index = device.DP.Index;
            int port = localPortBase + device.DP.Index;

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "echo \"Restarting RT" + device.DP.Index + " now, please wait for about 90 seconds...\"" + "\\n";
            fileContent += "ps aux | grep rt" + index + "_ | grep -v grep | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "FILESIZE=1" + "\\n";
            fileContent += "while [ $FILESIZE -ge 1 ];do" + "\\n";
            fileContent += "sleep 1" + "\\n";
            startTimeOut += 2;
            fileContent += "netstat -an | grep \"127.0.0.1:" + port + " \" > IFLISTEN" + port + "\\n";
            fileContent += "FILESIZE=$(ls -l IFLISTEN" + port + " | cut -d ' ' -f 5)" + "\\n";
            fileContent += "done" + "\\n";
            fileContent += "rm IFLISTEN" + port + "\\n";
            fileContent += "cd /home/lab_admin/" + filePath + "\\n";
            fileContent += "cp -f hardcfg.tcl.v" + version + " hardcfg.tcl" + "\\n";
            fileContent += "rm flash.dat" + "\\n";
            fileContent += "wine /home/lab_admin/public/wvrp/rt" + index + "_v" + version + ".exe &" + "\\n";

            fileReturn = "Now RT" + device.DP.Index + " is startup";
            fileStop = "ps aux | grep rt" + index + "_ | grep -v grep | awk '{print $2}' | xargs kill" + "\\n";
        }

        public override string toString()
        {
            return fileContent + "echo \"Now RT" + device.DP.Index + " is startup.\"";
        }
    }
}
