using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    思科路由器的NETMAP文件                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class CRNetMap : FileBase
    {
        public CRNetMap(string floder)
        {
            device = null;
            deviceType = (int)DeviceType.DEVICE_TYPE_CISCOROUTER;
            fileType = FileBaseType.FILE_BASE_TYPE_CONFIG;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "NETMAP";
            filePath = "lab_script/" + floder + "/cisco";

            string str = "";
            foreach (Link lnk in Util.SingletonGeneric<Data.SystemData>.GetInstance.LinkList)
            {
                if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                {
                    str = ((int)lnk.SrcDevice.DP.Type * 10 + lnk.SrcDevice.DP.Index) + ":" + lnk.SrcCard.Number + "/" + lnk.SrcSocket.Number + "@debian        ";

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                        str += ((int)lnk.DstDevice.DP.Type * 10 + lnk.DstDevice.DP.Index);
                    else
                        str += lnk.GUID;

                    str += ":" + lnk.DstCard.Number + "/" + lnk.DstSocket.Number + "@debian";
                    fileContent += str + "\\n";
                }
                else if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                {
                    str = ((int)lnk.DstDevice.DP.Type * 10 + lnk.DstDevice.DP.Index) + ":" + lnk.DstCard.Number + "/" + lnk.DstSocket.Number + "@debian        ";

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOROUTER)
                        str += ((int)lnk.SrcDevice.DP.Type * 10 + lnk.SrcDevice.DP.Index);
                    else
                        str += lnk.GUID;

                    str += ":" + lnk.SrcCard.Number + "/" + lnk.SrcSocket.Number + "@debian";
                    fileContent += str + "\\n";
                }
            }
        }
    }
}
