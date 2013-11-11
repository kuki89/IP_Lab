using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    启动XP主机脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HostXPStartFile : FileBase
    {
        public HostXPStartFile(DeviceBase dev, string floder)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_START_AND_ALL;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "start_winxp" + device.DP.Index;
            filePath = "lab_script/" + floder + "/host";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "ps auxww | grep \"winxp" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "sleep 3" + "\\n";
            startTimeOut += 6;
            fileContent += "/home/lab_admin/public/qemu/qemu \\\\" + "\\n";
            fileContent += "  -L /home/lab_admin/public/qemu/share \\\\" + "\\n";
            fileContent += "  -m 256 \\\\" + "\\n";
            fileContent += "  -hda /home/lab_admin/public/qemu/winxp" + device.DP.Index + ".img \\\\" + "\\n";
            fileContent += "  -boot c \\\\" + "\\n";
            fileContent += "  -localtime \\\\" + "\\n";
            fileContent += "  -usbdevice tablet \\\\" + "\\n";
            fileContent += "  -vnc 0.0.0.0:" + device.DP.Index + " \\\\" + "\\n";

            GenerateLinkData();

            fileReturn = "Now XP" + device.DP.Index + " is startup";
            fileStop = "ps auxww | grep \"winxp" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
        }

        protected void GenerateLinkData()
        {
            // 查找与主机相连的设备的端口
            int dstport = 0;
            int srcport = 0;

            int vlan = 10;
            int portbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);
            string dip_addr = "127.0.0.1";

            foreach (Link lnk in device.GetLinkList())
            {
                vlan++;

                if (lnk.SrcDevice == device)
                {
                    srcport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.SrcDevice.DP.Type * 1000 + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    if (lnk.DstCard.Card_Prefix == "FXP")
                        portbase = 20000;
                    dstport = portbase + (int)lnk.DstDevice.DP.Type * 1000 
                        + (int)lnk.DstDevice.DP.Index * 100 
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dip_addr = "192.168.200.2";
                }
                else
                {
                    srcport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.DstDevice.DP.Type * 1000 + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    if (lnk.SrcCard.Card_Prefix == "FXP")
                        portbase = 20000;
                    dstport = portbase + (int)lnk.SrcDevice.DP.Type * 1000 
                        + (int)lnk.SrcDevice.DP.Index * 100 
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dip_addr = "192.168.200.2";

                }

                fileContent += "  -net nic,vlan=" + vlan + ",macaddr=00:d0:f8:26:0" + device.DP.Index + ":0" + (vlan - 10) + ",model=e1000 \\\\" + "\\n";
                fileContent += "  -net udp,vlan=" + vlan + ",sport=" + srcport + ",dport=" + dstport + ",daddr=" + dip_addr + " \\\\" + "\\n";
            }

            fileContent += "  -net nic,vlan=20,macaddr=00:d0:f8:26:0" + device.DP.Index + ":20,model=e1000 \\\\" + "\\n";
            fileContent += "  -net tap,vlan=20,ifname=tap" + device.DP.Index + ",script=no,downscript=no &" + "\\n";
        }

        public override string toString()
        {
            return fileContent + "echo \"Now XP" + device.DP.Index + " is startup.\"";
        }
    }
}
