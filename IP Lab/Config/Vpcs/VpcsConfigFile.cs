using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    VPCS 主机配置文件                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class VpcsConfigFile : FileBase
    {
        public VpcsConfigFile(string floder)
        {
            device = null;
            deviceType = (int)Enum.DeviceType.DEVICE_TYPE_VPCS;
            localPortBase = deviceType * 1000;

            fileType = FileBaseType.FILE_BASE_TYPE_CONFIG;

            GenerateConfig(floder);
        }


        protected void GenerateConfig(string floder)
        {
            fileName = "lab_vpcs.cfg";   // 文件名称
            filePath = "lab_script/" + floder + "/vpcs";

            // 在这里，每行末尾添加\n
            // 发送到web service后，以此为分隔符切割
            // 首先，添加1,2,3,4,6....
            for (int i = 1; i <= 9; i++)
            {
                fileContent += "【" + i + "】\\n";
            }

            int portbase = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"]);

            foreach (Link lnk in Util.SingletonGeneric<Data.SystemData>.GetInstance.LinkList)
            {
                int index = 0;

                int lport = 0;
                int rport = 0;
                if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_VPCS)
                {
                    lport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.SrcDevice.DP.Type * 1000 + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    if (lnk.DstCard.Card_Prefix == "FXP")
                        portbase = 20000;

                    rport = portbase + (int)lnk.DstDevice.DP.Type * 1000
                        + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    index = lnk.SrcDevice.DP.Index;
                }
                else if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_VPCS)
                {
                    lport = Int32.Parse(ConfigurationManager.AppSettings["EthernetPortBase"])
                        + (int)lnk.DstDevice.DP.Type * 1000 + (int)lnk.DstDevice.DP.Index * 100
                        + lnk.DstCard.Number * 10 + lnk.DstSocket.Number;

                    if (lnk.DstCard.Card_Prefix == "FXP")
                        portbase = 20000;

                    rport = portbase + (int)lnk.SrcDevice.DP.Type * 1000
                        + (int)lnk.SrcDevice.DP.Index * 100
                        + lnk.SrcCard.Number * 10 + lnk.SrcSocket.Number;

                    index = lnk.DstDevice.DP.Index;
                }

                if (index != 0)
                {
                    string str = "set lport " + lport + "\\n";

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER
                        || lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        str += "set rhost 192.168.200.2 " + "\\n";

                    str += "set rport " + rport + "\\n";

                    int filecontentIndex = fileContent.IndexOf("【" + index.ToString() + "】\\n") + 5;
                    fileContent = fileContent.Insert(filecontentIndex, str);
                }
            }
            fileContent = fileContent.Replace("【", "").Replace("】", "");
        }

        public override string toString()
        {
            return fileContent;
        }
    }
}
