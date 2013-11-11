using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    华为交换机Hardcfg脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HWHardCfg : FileBase
    {
        public HWHardCfg(DeviceBase dev, string floder, string ver)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;
            version = ver;

            fileType = FileBaseType.FILE_BASE_TYPE_CONFIG;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "hardcfg.tcl.v" + version;
            filePath = "lab_script/" + floder + "/huawei/rt" + device.DP.Index;

            switch (version)
            {
                case "55":
                    {
                        fileContent = Get55VersionHead();
                        fileContent += GenerateLinkData();
                    }
                    break;
                case "57":
                    {
                        fileContent = Get57VersionHead();
                        fileContent += GenerateLinkData();
                        fileContent += "SetLocalIp 127.0.0.1" + "\\n";
                        fileContent += "SetSupportKey " + "\\n";
                    }
                    break;
                default: break;
            }
        }

        protected string Get55VersionHead()
        {
            int index = device.DP.Index;

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            string str = "SetSelfSlot 0" + "\\n";
            str += "SetMainSlot 0" + "\\n";
            str += "SetConsoleCom " + (localPortBase + index) + "\\n";
            str += "SetMemorySize 140" + "\\n";
            str += "SetWinSockOffset " + (100 * index) + "\\n";
            str += "SetWVRPInstanceName RT" + index + "\\n";

            return str;
        }
        protected string Get57VersionHead()
        {
            int index = device.DP.Index;

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            string str = "SetWVRPInstanceName RT" + device.DP.Index + ":slot17" + "\\n";
            str += "SetHeartBeatVal 1000" + "\\n";
            str += "SetConsoleCom " + (localPortBase + index) + "\\n";
            str += "SetMemorySize 256" + "\\n";
            str += "SetWinSockOffset " + (100 * index) + "\\n";
            str += "SetSelfSlot 17 -ChassisId 1 -ChassisType forward -SysType single" + "\\n";

            return str;
        }

        protected string GenerateLinkData()
        {
            string str = "";

            // 列举出所有Ethernet口
            for (int i = 0; i < 16; i++)
            {
                int lport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                    + localPortBase + (int)device.DP.Index * 100 + i;
                int dport = 40000 + (int)device.DP.Index * 100 + i;    // 远端端口，临时启用40X0X
                str += "AddEthernet -local 127.0.0.1 -lport " + lport + " -dest 127.0.0.1 -dport " + dport + "\\n";
            }
            // 列举出所有Serial口
            for (int i = 0; i < 16; i++)
            {
                int lport = Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                    + localPortBase + (int)device.DP.Index * 100 + i;
                int dport = 50000 + (int)device.DP.Index * 100 + i;    // 远端端口，临时启用50X0X
                str += "AddSerial -local 127.0.0.1 -lport " + lport + " -dest 127.0.0.1 -dport " + dport + "\\n";
            }

            // 遍历设备的连接列表
            foreach (Link lnk in device.GetLinkList())
            {
                int originport = 0, changeport = 0;
                string originstr = "", changestr = "";
                int ethernetpostbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);
                bool changeIP = false;
                bool thisSrc = true;

                // 确定端口的对端端口
                if (lnk.SrcDevice == this.device)
                {
                    originport = (int)lnk.SrcDevice.DP.Index * 100 + lnk.SrcSocket.Number;
                    changeport = (int)lnk.DstDevice.DP.Type * 1000 + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;
                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        changeIP = true;
                }
                else
                {
                    thisSrc = false;
                    originport = (int)lnk.DstDevice.DP.Index * 100 + lnk.DstSocket.Number;
                    changeport = (int)lnk.SrcDevice.DP.Type * 1000 + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;
                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        changeIP = true;
                }

                if (lnk.SrcCard.Card_Type == DeviceCardType.DEVICE_CARD_TYPE_ETHERNET)
                {
                    originport += 40000;
                    changeport += Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);

                    if (lnk.DstCard.Card_Prefix.Contains("FXP") || lnk.SrcCard.Card_Prefix.Contains("FXP"))
                        changeport += (Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"])
                            - Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]));

                    originstr = "AddEthernet -local ";
                    changestr = "AddEthernet -local ";
                }
                else
                {
                    originport += 50000;
                    changeport += Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"]);
                    ethernetpostbase = Int32.Parse(ConfigurationManager.AppSettings["SerialPortBase"]);

                    originstr = "AddSerial -local ";
                    changestr = "AddSerial -local ";
                }

                // 对端为思科交换机时，需要更改IP
                if (changeIP)
                {
                    int localport = 0;
                    if (thisSrc)
                        localport = ethernetpostbase + (int)lnk.SrcDevice.DP.Type * 1000    // i0000 + i000 + i00 + i 
                            + (int)lnk.SrcDevice.DP.Index * 100 + lnk.SrcSocket.Number;
                    else
                        localport = ethernetpostbase + (int)lnk.DstDevice.DP.Type * 1000    // i0000 + i000 + i00 + i 
                            + (int)lnk.DstDevice.DP.Index * 100 + lnk.DstSocket.Number;

                    originstr += "127.0.0.1 -lport " + localport + " -dest 127.0.0.1 -dport " + originport + "\\n";
                    changestr += "192.168.200.1 -lport " + localport + " -dest 192.168.200.2 -dport " + changeport + "\\n";

                    str = str.Replace(originstr, changestr);
                }
                else
                {
                    str = str.Replace(originport.ToString(), changeport.ToString());
                }
            }

            return str;
        }
    }
}
