using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    启动防火墙脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class FWStartFile : FileBase
    {
        public FWStartFile(DeviceBase dev, string floder)
        {
            device = dev;
            deviceType = (int)dev.DP.Type;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_START_AND_ALL;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "start_asa.sh";
            filePath = "lab_script/" + floder + "/firewall";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            fileContent = "#!/bin/bash" + "\\n";
            fileContent += "ps auxww | grep \"asa842-vmlinuz\" | awk '{print $2}' | xargs kill" + "\\n";
            fileContent += "sleep 2" + "\\n";
            startTimeOut += 4;
            fileContent += "/home/lab_admin/public/qemu/qemu \\\\" + "\\n";
            fileContent += "  -L /home/lab_admin/public/qemu/share \\\\" + "\\n";
            fileContent += "  -m 512 \\\\" + "\\n";
            fileContent += "  -cpu coreduo \\\\" + "\\n";
            fileContent += "  -icount auto \\\\" + "\\n";
            fileContent += "  -hda /home/lab_admin/public/qemu/asa842.img \\\\" + "\\n";
            fileContent += "  -nographic \\\\" + "\\n";

            fileContent += "  -kernel /home/lab_admin/public/qemu/asa842-vmlinuz \\\\" + "\\n";
            fileContent += "  -initrd /home/lab_admin/public/qemu/asa842-initrd.gz \\\\" + "\\n";
            fileContent += "  -hdachs 980,16,32 \\\\" + "\\n";
            fileContent += "  -append \"ide_generic.probe_mask=0x01 ide_core.chs=0.0:980,16,32 auto nousb console=ttyS0,9600 bigphysarea=65536\"\\\\" + "\\n";

            fileContent += "  -net nic,vlan=10,macaddr=00:d0:f8:01:01:00,model=i82559er \\\\" + "\\n";
            fileContent += "  -net udp,vlan=10,sport=16100,dport=99990,daddr=192.168.200.990 \\\\" + "\\n";
            fileContent += "  -net nic,vlan=11,macaddr=00:d0:f8:01:01:01,model=i82559er \\\\" + "\\n";
            fileContent += "  -net udp,vlan=11,sport=16110,dport=99991,daddr=192.168.200.991 \\\\" + "\\n";
            fileContent += "  -net nic,vlan=12,macaddr=00:d0:f8:01:01:02,model=i82559er \\\\" + "\\n";
            fileContent += "  -net udp,vlan=12,sport=16120,dport=99992,daddr=192.168.200.992 \\\\" + "\\n";
            fileContent += "  -serial mon:telnet::6001,server,nowait &" + "\\n";

            GenerateLinkData();

            fileReturn = "Now Firewall is startup";
            fileStop = "ps auxww | grep \"asa842-vmlinuz\" | awk '{print $2}' | xargs kill" + "\\n";
        }

        protected void GenerateLinkData()
        {
            int srcport = 0;
            int dstport = 0;
            string srcip = "";
            string dstip = "127.0.0.1";
            int portbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);

            // 对防火墙的对端端口,IP地址进行替换        
            foreach (Link lnk in device.GetLinkList())
            {
                if (lnk.SrcDevice == device)
                {
                    srcport = 99990 + lnk.SrcCard.Number;
                    srcip = "192.168.200.99" + lnk.SrcCard.Number;

                    if (lnk.DstCard.Card_Prefix == "FXP")
                        portbase = 20000;
                    dstport = portbase + (int)lnk.DstDevice.DP.Type * 1000
                        + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dstip = "192.168.200.2";
                }
                else
                {
                    srcport = 99990 + lnk.DstCard.Number;
                    srcip = "192.168.200.99" + lnk.DstCard.Number;

                    if (lnk.SrcCard.Card_Prefix == "FXP")
                        portbase = 20000;
                    dstport = portbase + (int)lnk.SrcDevice.DP.Type * 1000
                        + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        dstip = "192.168.200.2";
                }

                fileContent = fileContent.Replace(srcport.ToString(), dstport.ToString());
                fileContent = fileContent.Replace(srcip, dstip);
            }
        }

        public override string toString()
        {
            return fileContent + "echo \"Now Firewall is startup.\"";
        }
    }
}
