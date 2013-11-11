using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    启动Juniper脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HostJuniperStartFile : FileBase
    {
        public HostJuniperStartFile(DeviceBase dev, string floder)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_START_AND_ALL;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "start_junos";
            filePath = "lab_script/" + floder + "/juniper";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "ps auxww | grep \"JunOS\" | awk '{print $2}' | xargs kill -9" + "\\n";
            fileContent += "sleep 3" + "\\n";
            startTimeOut += 6;
            fileContent += "/home/lab_admin/public/qemu/qemu -L /home/lab_admin/public/qemu/share \\\\" + "\\n";
            fileContent += "  -m 256 \\\\" + "\\n";
            fileContent += "  -hda /home/lab_admin/public/qemu/JunOS10.3R1.9.vmdk \\\\" + "\\n";
            fileContent += "  -boot c \\\\" + "\\n";
            fileContent += "  -nographic \\\\" + "\\n";
            fileContent += "  -localtime \\\\" + "\\n";

            GenerateLinkData();

            fileContent += "  -serial mon:telnet::500" + device.DP.Index + ",server,nowait &" + "\\n";

            fileReturn = "Now Juniper is startup";
            fileStop = "ps auxww | grep \"JunOS\" | awk '{print $2}' | xargs kill -9" + "\\n";
        }

        protected void GenerateLinkData()
        {
            // 列举出所有EM口
            for (int i = 0; i < 8; i++)
            {
                int vlan = 10 + i;
                int dstport = 99960 + i;
                string dstaddr = "192.168.200.96" + i;
                fileContent += "  -net nic,vlan=" + vlan + ",macaddr=00:d0:f8:01:0" + (2 * device.DP.Index - 1) + ":0" + i.ToString() + ",model=e1000 \\\\" + "\\n";
                fileContent += "  -net udp,vlan=" + vlan + ",sport=151" + i + "0,dport=" + dstport + ",daddr=" + dstaddr + " \\\\" + "\\n";
            }

            // 列举出所有FXP口
            for (int i = 0; i < 8; i++)
            {
                int vlan = 20 + i;
                int dstport = 99980 + i;
                string dstaddr = "192.168.200.98" + i;
                fileContent += "  -net nic,vlan=" + vlan + ",macaddr=00:d0:f8:01:0" + (2 * device.DP.Index) + ":0" + i.ToString() + ",model=i82559er \\\\" + "\\n";
                fileContent += "  -net udp,vlan=" + vlan + ",sport=251" + i + "0,dport=" + dstport + ",daddr=" + dstaddr + " \\\\" + "\\n";
            }

            // 遍历设备的连接列表
            foreach (Link lnk in device.GetLinkList())
            {
                int srcport = 0;
                int dstport = 0;
                string srcip = "";
                string dstip = "127.0.0.1";

                if (lnk.SrcDevice == device)
                {
                    if (lnk.SrcCard.Card_Prefix.Contains("EM"))
                    {
                        srcport = 99960 + lnk.SrcCard.Number;
                        srcip = "192.168.200.96" + lnk.SrcCard.Number;
                    }
                    else if (lnk.SrcCard.Card_Prefix.Contains("FXP"))
                    {
                        srcport = 99980 + lnk.SrcCard.Number;
                        srcip = "192.168.200.98" + lnk.SrcCard.Number;
                    }

                    dstport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.DstDevice.DP.Type * 1000 + lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dstip = "192.168.200.2";
                }
                else
                {
                    if (lnk.DstCard.Card_Prefix.Contains("EM"))
                    {
                        srcport = 99960 + lnk.DstCard.Number;
                        srcip = "192.168.200.96" + lnk.DstCard.Number;
                    }
                    else if (lnk.DstCard.Card_Prefix.Contains("FXP"))
                    {
                        srcport = 99980 + lnk.DstCard.Number;
                        srcip = "192.168.200.98" + lnk.DstCard.Number;
                    }

                    dstport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.SrcDevice.DP.Type * 1000 + lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dstip = "192.168.200.2";
                }

                fileContent = fileContent.Replace(srcport.ToString(), dstport.ToString());
                fileContent = fileContent.Replace(srcip, dstip);
            }
        }

        public override string toString()
        {
            return fileContent + "echo \"Now Juniper is startup.\"";
        }
    }
}
