using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    启动Linux主机脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HostLinuxStartFile : FileBase
    {
        public HostLinuxStartFile(DeviceBase dev, string floder)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_START_AND_ALL;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "start_centos" + device.DP.Index;
            filePath = "lab_script/" + floder + "/host";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "ps auxww | grep \"centos" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "ps auxww | grep \"VMLinux" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "sleep 3" + "\\n";
            startTimeOut += 6;
            fileContent += "/home/lab_admin/public/qemu/qemu \\\\" + "\\n";
            fileContent += "  -L /home/lab_admin/public/qemu/share \\\\" + "\\n";
            fileContent += "  -m 512 \\\\" + "\\n";
            fileContent += "  -hda /home/lab_admin/public/qemu/centos" + device.DP.Index + ".img \\\\" + "\\n";
            fileContent += "  -boot c \\\\" + "\\n";
            fileContent += "  -localtime \\\\" + "\\n";
            fileContent += "  -nographic \\\\" + "\\n";

            GenerateLinkData();

            fileReturn = "Now Linux " + device.DP.Index + " is startup";
            fileStop = "ps auxww | grep \"centos" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
            fileStop += "ps auxww | grep \"VMLinux" + device.DP.Index + ".img\" | awk '{print $2}' | xargs kill" + "\\n";
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

                fileContent += "  -net nic,vlan=" + vlan + ",macaddr=00:d0:f8:27:0" + device.DP.Index + ":0" + (vlan - 10) + ",model=e1000 \\\\" + "\\n";
                fileContent += "  -net udp,vlan=" + vlan + ",sport=" + srcport + ",dport=" + dstport + ",daddr=" + dip_addr + " \\\\" + "\\n";
            }

            fileContent += "  -net nic,vlan=20,macaddr=00:d0:f8:27:0" + device.DP.Index + ":20,model=e1000 \\\\" + "\\n";
            int offset = Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceMaxCount[(int)DeviceType.DEVICE_TYPE_WINDOWS];
            fileContent += "  -net tap,vlan=20,ifname=tap" + (offset + device.DP.Index) + ",script=no,downscript=no \\\\" + "\\n";
            fileContent += "  -serial mon:telnet::" + (localPortBase + device.DP.Index) + ",server,nowait &" + "\\n";
        }

        public override string toString()
        {
            return fileContent + "echo \"Now Linux " + device.DP.Index + " is startup.\"";
        }
    }
}
