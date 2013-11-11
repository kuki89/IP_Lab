using System.Windows;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using IP_Lab.Device;
using IP_Lab.Enum;
using System.Text;

//*******************************************************************
//                                                                  *
//                       根据拓扑图生成配置文件                    *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Config
{
    public class ConfigFileMgr
    {
        #region 变量

        private bool genCRFile = false;
        private bool genCSFile = false;
        private bool genHWFile = false;
        private bool genVpcs = false;
        private string AlertInformation;

        #endregion

        #region 属性变量

        public string FloderName { get; set; }
        public List<FileBase> FileList;         // 文件列表
        public WndInformation wndInfo;

        #endregion

        public ConfigFileMgr()
        {
            Init();
        }

        protected void Init()
        {
            FloderName = Util.SingletonGeneric<Data.UserData>.GetInstance.UserName;
            if (string.IsNullOrEmpty(FloderName))
                FloderName = "lab_admin_test";

            FileList = new List<FileBase>();
            wndInfo = new WndInformation();
        }

        #region 根据拓扑图生成配置文件

        protected void Reset()
        {
            try
            {
                genCRFile = false;
                genCSFile = false;
                genHWFile = false;
                genVpcs = false;

                FileList.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public string GenConfigFile()
        {
            Reset();

            foreach (DeviceBase device in Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList)
            {
                switch (device.DP.Type)
                {
                    case Enum.DeviceType.DEVICE_TYPE_CISCOROUTER:
                        {
                            CRStartFile config = new CRStartFile(device, FloderName);

                            AddFileToFileList(config);

                            genCRFile = true;
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_CISCOSWITCHER:
                        {
                            CSStartFile config = new CSStartFile(device, FloderName);

                            AddFileToFileList(config);

                            genCSFile = true;
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_HUAWEI:
                        {
                            HWConfigFileMgr hf = new HWConfigFileMgr(device, FloderName);

                            List<FileBase> list = hf.GenHWConfigFile();
                            foreach (FileBase file in list)
                                AddFileToFileList(file);

                            genHWFile = true;
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_WINDOWS:
                        {
                            HostXPStartFile xp = new HostXPStartFile(device, FloderName);

                            AddFileToFileList(xp);
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_LINUX:
                        {
                            HostLinuxStartFile linux = new HostLinuxStartFile(device, FloderName);

                            AddFileToFileList(linux);
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_ASAFIREWALL:
                        {
                            FWStartFile fw = new FWStartFile(device, FloderName);

                            AddFileToFileList(fw);
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_VPCS:
                        {
                            genVpcs = true;
                        }
                        break;
                    case Enum.DeviceType.DEVICE_TYPE_JUNIPER:
                        {
                            HostJuniperStartFile juniper = new HostJuniperStartFile(device, FloderName);

                            AddFileToFileList(juniper);
                        }
                        break;
                    default: break;
                }
            }

            #region 思科路由器、交换机生成NETMAP、StartAll文件，华为StartAll文件

            if (genCRFile)
            {
                AddFileToFileList(new CRNetMap(FloderName));
                AddFileToFileList(new CRStartAll(FloderName));
            }
            if (genCSFile)
            {
                AddFileToFileList(new CSNetMap(FloderName));
                AddFileToFileList(new CSStartAll(FloderName));
            }
            if (genHWFile)
            {
                AddFileToFileList(new HWStartAllFile(FloderName, "55"));
                AddFileToFileList(new HWStartAllFile(FloderName, "57"));
            }

            if (genVpcs)
            {
                AddFileToFileList(new VpcsStartFile(FloderName));
                AddFileToFileList(new VpcsLocalStartFile(FloderName));
                AddFileToFileList(new VpcsConfigFile(FloderName));
            }

            #endregion

            return FileListToXML();
        }

        protected void AddFileToFileList(FileBase file)
        {
            FileList.Add(file);
        }

        protected string FileListToXML()
        {
            XElement root = new XElement("IP_Lab");
            XElement fi = new XElement("FileList");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            XElement floder = new XElement("Floder", FloderName);

            root.Add(floder);
            root.Add(fi);

            AddFileToXML(fi);
            AddCommonFileToXML(root);

            return xdoc.ToString();
        }

        protected void AddFileToXML(XElement fi)
        {
            foreach (FileBase file in FileList)
            {
                // 文件属性信息
                XElement elem = new XElement("File");
                XElement name = new XElement("FileName", file.fileName);
                XElement path = new XElement("FilePath", file.filePath);
                XElement content = new XElement("FileContent", file.toString());
                XElement type = new XElement("DeviceType", file.deviceType);

                elem.Add(name);
                elem.Add(path);
                elem.Add(content);
                elem.Add(type);

                fi.Add(elem);
            }
        }

        /// <summary>
        /// 是否添加公共文件
        /// </summary>
        protected void AddCommonFileToXML(XElement root)
        {
            // <CiscoCommon> path </CiscoCommon>
            // 目录不存在时path为false
            XElement ciscoCommon;
            if (genCRFile)
                ciscoCommon = new XElement("CiscoCommon", "lab_script/" + FloderName + "/cisco");
            else
                ciscoCommon = new XElement("CiscoCommon", "false");
            root.Add(ciscoCommon);

        }

        #endregion

        #region 根据设备列表生成启动命令

        public string GenStartDeviceCmd(List<DeviceBase> devs)
        {
            AlertInformation = "";

            XElement root = new XElement("IP_Lab");
            XElement start = new XElement("StartList");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            XElement floder = new XElement("Floder", FloderName);

            bool containVPCS = false;

            foreach (DeviceBase dev in devs)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_VPCS)
                    containVPCS = true;
                foreach (FileBase file in FileList)
                {
                    if (file.device == dev &&
                        !(file.fileType == FileBaseType.FILE_BASE_TYPE_CONFIG))
                    {
                        AddStartCmdToList(file, start);
                    }
                }
            }

            if (containVPCS)
            {
                foreach (FileBase file in FileList)
                {
                    if(file.deviceType == (int)Enum.DeviceType.DEVICE_TYPE_VPCS
                        && file.fileType == FileBaseType.FILE_BASE_TYPE_STARTALL)
                        AddStartCmdToList(file, start);
                }
            }

            root.Add(floder);
            root.Add(start);

            if (!string.IsNullOrEmpty(AlertInformation))
                showInformation(AlertInformation + "   请耐心等待！");

            return xdoc.ToString();
        }

        public string GenStartAllDeviceCmd()
        {
            AlertInformation = "";

            XElement root = new XElement("IP_Lab");
            XElement start = new XElement("StartList");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            XElement floder = new XElement("Floder", FloderName);

            foreach (FileBase file in FileList)
            {
                if (file.fileType == FileBaseType.FILE_BASE_TYPE_START_AND_ALL
                    || file.fileType == FileBaseType.FILE_BASE_TYPE_STARTALL)
                {
                    AddStartCmdToList(file, start);
                }
            }

            root.Add(floder);
            root.Add(start);

            if (!string.IsNullOrEmpty(AlertInformation))
                showInformation(AlertInformation + "   请耐心等待！");

            return xdoc.ToString();
        }

        protected void AddStartCmdToList(FileBase file, XElement stlist)
        {
            XElement elem;
            if (file.deviceType == (int)Enum.DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                elem = new XElement("Switch");
            else
                elem = new XElement("Device");

            switch (file.deviceType)
            {
                case (int)Enum.DeviceType.DEVICE_TYPE_HUAWEI:
                    {
                        string version = Util.SingletonGeneric<Data.SystemData>.GetInstance.HWversion;
                        if (!file.fileName.Contains(version))
                            return;
                    }
                    break;
                case (int)Enum.DeviceType.DEVICE_TYPE_WINDOWS:
                    {
                        AlertInformation += file.device.DP.Name + " 启动需要 2 分钟 " + System.Environment.NewLine;
                    }
                    break;
                case (int)Enum.DeviceType.DEVICE_TYPE_LINUX:
                    {
                        AlertInformation += file.device.DP.Name + " 启动需要 3 分钟 " + System.Environment.NewLine;
                    }
                    break;
                case (int)Enum.DeviceType.DEVICE_TYPE_JUNIPER:
                    {
                        AlertInformation += "Juniper 启动需要 15 分钟 " + System.Environment.NewLine;
                    }
                    break;
                default:
                    break;
            }

            XElement name = new XElement("FileName", file.fileName);
            XElement path = new XElement("FilePath", file.filePath);
            XElement ret = new XElement("FileReturn", file.fileReturn);
            XElement time = new XElement("Timeout", file.startTimeOut);

            elem.Add(name);
            elem.Add(path);
            elem.Add(ret);
            elem.Add(time);

            stlist.Add(elem);
        }

        protected void showInformation(string info)
        {
            wndInfo.tbInformation.Text = info;
            wndInfo.Show();
        }

        #endregion

        #region 根据设备列表生成停止命令

        public string GenStopDeviceCmd(List<DeviceBase> devs)
        {
            AlertInformation = "";

            XElement root = new XElement("IP_Lab");
            XElement stop = new XElement("StopList");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            bool containVPCS = false;

            foreach (DeviceBase dev in devs)
            {
                if (dev.DP.Type == Enum.DeviceType.DEVICE_TYPE_VPCS)
                    containVPCS = true;
                foreach (FileBase file in FileList)
                {
                    if (file.device == dev &&
                        !(file.fileType == FileBaseType.FILE_BASE_TYPE_CONFIG))
                    {
                        AddStopCmdToList(file, stop);
                    }
                }
            }

            if (containVPCS)
            {
                foreach (FileBase file in FileList)
                {
                    if (file.deviceType == (int)Enum.DeviceType.DEVICE_TYPE_VPCS
                        && file.fileType == FileBaseType.FILE_BASE_TYPE_STARTALL)
                        AddStopCmdToList(file, stop);
                }
            }

            root.Add(stop);
            return xdoc.ToString();
        }

        public string GenStopAllDeviceCmd()
        {
            AlertInformation = "";

            XElement root = new XElement("IP_Lab");
            XElement stop = new XElement("StopList");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            foreach (FileBase file in FileList)
            {
                if (file.fileType == FileBaseType.FILE_BASE_TYPE_START_AND_ALL
                    || file.fileType == FileBaseType.FILE_BASE_TYPE_STARTALL)
                {
                    AddStopCmdToList(file, stop);
                }
            }

            root.Add(stop);
            return xdoc.ToString();
        }

        protected void AddStopCmdToList(FileBase file, XElement stlist)
        {
            XElement elem;
            if (file.deviceType == (int)Enum.DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                elem = new XElement("Switch");
            else
                elem = new XElement("Device");

            XElement name = new XElement("FileName", file.fileName);
            XElement stop = new XElement("FileStop", file.fileStop);

            elem.Add(name);
            elem.Add(stop);
            stlist.Add(elem);
        }

        #endregion

    }
}
