using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    思科交换机配置脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class CSStartFile : FileBase
    {
        public CSStartFile(DeviceBase dev, string floder)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_START;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "start_" + device.DP.Name.ToLower() + ".sh";
            filePath = "iou/" + floder;

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "#!/bin/sh" + "\\n";
            fileContent += "ulimit -d unlimited" + "\\n";
            fileContent += "cd /root/" + filePath + "\\n";
            fileContent += "../public/stop.sh" + "\\n";
            fileContent += "sleep 4" + "\\n";
            startTimeOut += 8;
            fileContent += "export NETIO_NETMAP=./NETMAP" + "\\n";
            fileContent += "export IOURC=../public/iourc" + "\\n";

            int Ecard = 0, Scard = 0;
            foreach (DeviceCard card in device.DP.Card_List)
            {
                if (card.Card_Type == DeviceCardType.DEVICE_CARD_TYPE_ETHERNET)
                    Ecard++;
                else
                    Scard++;
            }

            string str = "../public/wrapper -m ../public/unixl2-upk9-ms.port-security -p " + (localPortBase + device.DP.Index)
                + " -- -e " + Ecard + " -s " + Scard + " " + (deviceType * 10 + device.DP.Index) + " &";
            fileContent += str + "\\nsleep 5\\n";
            startTimeOut += 10;

            GenerateLinkData();

            fileReturn = "Now " + device.DP.Name + " is startup";
            fileStop = "../public/stop.sh" + "\\n";
        }

        protected void GenerateLinkData()
        {
            foreach (Link lnk in device.GetLinkList())
            {
                // 两种设备都是思科交换机时，无需添加iou2net的信息
                if (lnk.SrcDevice.DP.Type == lnk.DstDevice.DP.Type)
                    continue;

                string str = "";       // 配置语句
                int srcType = (int)lnk.SrcDevice.DP.Type;    // 本端设备类型
                int srcIndex = (int)lnk.SrcDevice.DP.Index;  // 本端设备序号
                int dstType = (int)lnk.DstDevice.DP.Type;    // 远端设备类型
                int dstIndex = (int)lnk.DstDevice.DP.Type;   // 远端设备序号
                int srcPort = 0;     // 本端设备端口
                int dstPort = 0;     // 远端设备端口

                srcPort = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.SrcDevice.DP.Type * 1000 + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;
                dstPort = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.DstDevice.DP.Type * 1000 + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                string serverip = ConfigurationManager.AppSettings["NormalServerIP"];

                // 本地端口
                str = "../public/iou2net.pl -u ";
                if (lnk.SrcDevice == this.device)
                {
                    if (lnk.DstCard.Card_Prefix.Contains("FXP"))
                        dstPort += (Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                            - Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]));

                    str += srcPort + ":" + serverip + ":" + dstPort;
                    str += " -p " + lnk.GUID + " & " + "\\n";
                }
                else
                {
                    if (lnk.SrcCard.Card_Prefix.Contains("FXP"))
                        srcPort += (Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                            - Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]));

                    str += dstPort + ":" + serverip + ":" + srcPort;
                    str += " -p " + lnk.GUID + " & " + "\\n";
                }
                fileContent += str;
                fileContent += "sleep 2" + "\\n";
                startTimeOut += 4;
            }
        }

        public override string toString()
        {
            return fileContent + "echo \"Now " + device.DP.Name + " is startup.\"";
        }
    }
}
