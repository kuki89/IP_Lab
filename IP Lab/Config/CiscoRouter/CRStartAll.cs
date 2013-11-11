using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    思科路由器启动所有设备                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class CRStartAll : FileBase
    {
        public CRStartAll(string floder)
        {
            device = null;
            deviceType = (int)DeviceType.DEVICE_TYPE_CISCOROUTER;
            localPortBase = deviceType * 1000;
            fileType = FileBaseType.FILE_BASE_TYPE_STARTALL;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "start_all.sh";   // 文件名称
            filePath = "lab_script/" + floder + "/cisco";

            // 在这里，每行末尾添加\n
            fileContent = "cd /home/lab_admin/" + filePath + "\\n";
            fileContent += "/home/lab_admin/public/CiscoIOU/stop.sh" + "\\n";
            fileContent += "sleep 4" + "\\n";
            startTimeOut += 8;
            fileContent += "echo \"all old processes are killed, and now start all cisco routers' process, please wait for about 60s ...\" " + "\\n";

            fileContent += "echo \"\"" + "\\n";
            fileContent += "export NETIO_NETMAP=./NETMAP" + "\\n";
            fileContent += "export IOURC=/home/lab_admin/public/CiscoIOU/iourc" + "\\n";

            GenerateDeviceData();

            fileReturn = "all router and switch are active now, enjoy yourself";
            fileStop = "/home/lab_admin/public/CiscoIOU/stop.sh" + "\\n";
        }

        protected void GenerateDeviceData()
        {
            foreach (DeviceBase device in Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList)
            {
                if (device.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                {
                    int Ecard = 0, Scard = 0;
                    foreach (DeviceCard card in device.DP.Card_List)
                    {
                        if (card.Card_Type == DeviceCardType.DEVICE_CARD_TYPE_ETHERNET)
                            Ecard++;
                        else
                            Scard++;
                    }

                    string str = "/home/lab_admin/public/CiscoIOU/wrapper-linux -m /home/lab_admin/public/CiscoIOU/i86bi_linux-adventerprisek9-ms -p " 
                        + (localPortBase + device.DP.Index) + " -- -e " + Ecard + " -s " + Scard + " " + (deviceType * 10 + device.DP.Index) + " &";
                    fileContent += str + "\\nsleep 5\\n";
                    startTimeOut += 10;
                }
            }

            fileContent += "\\nsleep 3\\n";
            startTimeOut += 6;

            foreach (DeviceBase device in Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList)
            {
                if (device.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                {
                    GenerateLinkData(device);
                }
            }
        }

        protected void GenerateLinkData(DeviceBase dev)
        {
            foreach (Link lnk in dev.GetLinkList())
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
                if (lnk.SrcDevice == dev)
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
            return fileContent + "echo \"all router and switch are active now, enjoy yourself\"";
        }
    }
}
