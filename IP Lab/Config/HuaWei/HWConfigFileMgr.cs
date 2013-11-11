using System.Collections.Generic;
using IP_Lab.Device;

//*******************************************************************
//                                                                  *
//                    生成华为设备脚本                          *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class HWConfigFileMgr
    {
        protected Device.DeviceBase device;
        protected string flodername;

        public HWConfigFileMgr(DeviceBase dev, string floder)
        {
            device = dev;
            flodername = floder;
        }

        public List<FileBase> GenHWConfigFile()
        {
            List<FileBase> config = new List<FileBase>();

            config.Add(new HWHardCfg(device, flodername, "55"));
            config.Add(new HWHardCfg(device, flodername, "57"));
            config.Add(new HWStartFile(device, flodername, "55"));
            config.Add(new HWStartFile(device, flodername, "57"));
//             config.Add(new HWResetFile(device, flodername, "55"));
//             config.Add(new HWResetFile(device, flodername, "57"));

            return config;
        }
    }
}
