using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    思科路由器配置脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class CRStartFile : FileBase
    {
        public CRStartFile(DeviceBase dev, string floder)
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
            filePath = "lab_script/" + floder + "/cisco";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "#!/bin/bash" + "\\n";
            fileContent += "cd /home/lab_admin/" + filePath + "\\n";
            fileContent += "ps auxww | grep [w]rapper | grep \"\\-p " + (localPortBase + device.DP.Index)
                + "\" | grep -v grep | awk '{print $2}' | xargs kill >/dev/null 2>&1" + "\\n";
            fileContent += "ps aux | grep iou2net | grep \"\\-u " + (100 + deviceType * 10 + device.DP.Index)
                + "\" | awk '{print $2}' | xargs kill >/dev/null 2>&1" + "\\n";
            fileContent += "sleep 2" + "\\n";
            startTimeOut += 4;
            fileContent += "export NETIO_NETMAP=./NETMAP" + "\\n";
            fileContent += "export IOURC=/home/lab_admin/public/CiscoIOU/iourc" + "\\n";

            int Ecard = 0, Scard = 0;
            foreach (DeviceCard card in device.DP.Card_List)
            {
                if (card.Card_Type == DeviceCardType.DEVICE_CARD_TYPE_ETHERNET)
                    Ecard++;
                else
                    Scard++;
            }

            string str = "/home/lab_admin/public/CiscoIOU/wrapper-linux -m /home/lab_admin/public/CiscoIOU/i86bi_linux-adventerprisek9-ms -p " + (localPortBase + device.DP.Index)
                + " -- -e " + Ecard + " -s " + Scard + " " + (deviceType * 10 + device.DP.Index) + " &";
            fileContent += str + "\\nsleep 5\\n";
            startTimeOut += 10;

            GenerateLinkData();

            fileReturn = "Now " + device.DP.Name + " is startup";

            fileStop = "ps auxww | grep [w]rapper | grep \"\\-p " + (localPortBase + device.DP.Index)
                + "\" | grep -v grep | awk '{print $2}' | xargs kill >/dev/null 2>&1" + "\\n";
            fileStop += "ps aux | grep iou2net | grep \"\\-u " + (100 + deviceType * 10 + device.DP.Index)
                + "\" | awk '{print $2}' | xargs kill >/dev/null 2>&1" + "\\n";
        }

        protected void GenerateLinkData()
        {
            foreach (Link lnk in device.GetLinkList())
            {
                // 两种设备都是思科路由器时，无需添加iou2net的信息
                if (lnk.SrcDevice.DP.Type == lnk.DstDevice.DP.Type)
                    continue;

                string str = "";       // 配置语句
                int srcType = (int)lnk.SrcDevice.DP.Type;    // 本端设备类型
                int srcIndex = (int)lnk.SrcDevice.DP.Index;   // 本端设备序号
                int dstType = (int)lnk.DstDevice.DP.Type;   // 远端设备类型
                int dstIndex = (int)lnk.DstDevice.DP.Type;  // 远端设备序号

                int srcPort = 0;     // 本端设备端口
                int dstPort = 0;     // 远端设备端口

                int srcportbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);
                int dstportbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);

                if (lnk.SrcCard.Card_Type == DeviceCardType.DEVICE_CARD_TYPE_SERIAL)
                {
                    srcportbase = Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"]);
                    dstportbase = Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"]);
                }

                srcPort = srcportbase + (int)lnk.SrcDevice.DP.Type * 1000 + (int)lnk.SrcDevice.DP.Index * 100
                    + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;
                dstPort = dstportbase + (int)lnk.DstDevice.DP.Type * 1000 + (int)lnk.DstDevice.DP.Index * 100
                    + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;


                string serverip = ConfigurationManager.AppSettings["LocalServerIP"];

                // 本地端口
                str = "/home/lab_admin/public/CiscoIOU/iou2net.pl -u ";
                if (lnk.SrcDevice == device)
                {
                    if (lnk.DstCard.Card_Prefix.Contains("FXP"))
                        dstPort += (Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                            - Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]));

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        serverip = ConfigurationManager.AppSettings["SwitchServerIP"];

                    str += srcPort + ":" + serverip + ":" + dstPort;
                    str += " -p " + lnk.GUID + " & " + "\\n";
                }
                else
                {
                    if (lnk.SrcCard.Card_Prefix.Contains("FXP"))
                        srcPort += (Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                            - Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]));

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        serverip = ConfigurationManager.AppSettings["SwitchServerIP"];
                    str += dstPort + ":" + serverip + ":" + srcPort;
                    str += " -p " + lnk.GUID + " & " + "\\n";
                }
                fileContent += str + "\\nsleep 2\\n";
                startTimeOut += 4;
            }
        }

        public override string toString()
        {
            return fileContent + "echo \"Now " + device.DP.Name + " is startup.\"";
        }
    }
}
