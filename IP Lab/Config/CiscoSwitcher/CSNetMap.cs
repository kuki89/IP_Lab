using System;
using IP_Lab.Enum;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    思科交换机的NETMAP文件                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class CSNetMap : FileBase
    {
        public CSNetMap(string floder)
        {
            device = null;
            deviceType = (int)DeviceType.DEVICE_TYPE_CISCOSWITCHER;

            fileType = FileBaseType.FILE_BASE_TYPE_CONFIG;

            GenerateConfig(floder);
        }

        protected void GenerateConfig(string floder)
        {
            fileName = "NETMAP";
            filePath = "iou/" + floder;

            string str = "";
            foreach (Link lnk in Util.SingletonGeneric<Data.SystemData>.GetInstance.LinkList)
            {
                if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                {
                    str = ((int)lnk.SrcDevice.DP.Type * 10 + lnk.SrcDevice.DP.Index) + ":" + lnk.SrcCard.Number + "/" + lnk.SrcSocket.Number + "@IOU        ";

                    if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        str += ((int)lnk.DstDevice.DP.Type * 10 + lnk.DstDevice.DP.Index);
                    else
                        str += lnk.GUID;

                    str += ":" + lnk.DstCard.Number + "/" + lnk.DstSocket.Number + "@IOU";
                    fileContent += str + "\\n";
                }
                else if (lnk.DstDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                {
                    str = ((int)lnk.DstDevice.DP.Type * 10 + lnk.DstDevice.DP.Index) + ":" + lnk.DstCard.Number + "/" + lnk.DstSocket.Number + "@IOU        ";

                    if (lnk.SrcDevice.DP.Type == DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                        str += ((int)lnk.SrcDevice.DP.Type * 10 + lnk.SrcDevice.DP.Index);
                    else
                        str += lnk.GUID;

                    str += ":" + lnk.SrcCard.Number + "/" + lnk.SrcSocket.Number + "@IOU";
                    fileContent += str + "\\n";
                }
            }
        }
    }
}
