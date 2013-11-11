using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Configuration;
using Renci.SshNet;
using Tamir.SharpSsh;

/// <summary>
/// 接收客户端的数据，在服务器生成配置文件
/// </summary>
public class WSGenerateConfigFile : WebServiceObject
{
    #region 变量

    Server_Information si;

    protected List<FileBase> FileList;
    protected List<FileBase> CiscoSwitchList; // 思科交换机

    #endregion

    public WSGenerateConfigFile()
    {
        FileList = new List<FileBase>();
        CiscoSwitchList = new List<FileBase>();
    }

    public override string ExecuteCommand(string command, string data)
    {
        try
        {
            // 获取服务器信息
            XDocument xser = XDocument.Parse(command);
            string lab_ip = xser.Element("IPLab").Element("LabIP").Value;
            si = Util.GetServerInfoFromIP(lab_ip);

            // 数据信息
            XDocument xdoc = XDocument.Parse(data);

            XElement filelist = xdoc.Element("IP_Lab").Element("FileList");
            XElement floder = xdoc.Element("IP_Lab").Element("Floder");
            FileList = ParseXMLtoList(filelist);

            SshShell shell = new SshShell(si.serverip, si.usrname, si.pwd);
            shell.Connect();
            if (shell.Expect(new Regex(si.sysname), 5) == null)
                throw new Exception("连接服务器出错！");

            // 首先，删除这个文件夹
            string flodername = floder.Value;

            Util.SshWriteAndExpect(shell, "cd ~", si.sysname);
            Util.SshWriteAndExpect(shell, "cd lab_script/", si.sysname);
            Util.SshWriteAndExpect(shell, "rm -rf " + flodername, si.sysname);

            CiscoSwitchList = new List<FileBase>();
            foreach (FileBase file in FileList)
            {
                if (file.deviceType == (int)DeviceType.DEVICE_TYPE_CISCOSWITCHER)
                {
                    // 对思科交换机的设备另行处理
                    CiscoSwitchList.Add(file);
                    continue;
                }
                FileToServer fts = new FileToServer(file, shell, si.sysname);
                if (fts.WriteFileToServer())
                    continue;
                else
                    return file.fileName + " 生成失败 !";
            }

            // 生成公共文件
            if (!GenerateCommonFile(xdoc.Element("IP_Lab"), shell))
                return "公共文件生成失败";

            // 存在思科交换机设备时，需要登录到其服务器
            if (CiscoSwitchList.Count > 0)
            {
                // 登录到设备上
                Util.SshWriteAndExpect(shell, "cd ~", si.sysname);
                Util.SshWriteAndExpect(shell, "ssh " + si.innerusrname + "@" + si.innerservip, "Password:");
                Util.SshWriteAndExpect(shell, si.innerpwd, si.innersysname);
                // 删除已存在目录
                Util.SshWriteAndExpect(shell, "rm -rf ./iou/" + flodername, si.innersysname);
                // 生成配置文件
                foreach (FileBase file in CiscoSwitchList)
                {
                    FileToServer fts = new FileToServer(file, shell, si.innersysname);
                    if (fts.WriteFileToServer())
                        continue;
                    else
                        return file.fileName + " 生成失败 !";
                }

                string res = Util.SshWriteAndExpect(shell, "cd ~/iou/" + flodername, si.innersysname);
                if (!res.Contains("cd: can't cd to"))
                {
                    Util.SshWriteAndExpect(shell, "echo >> NETMAP", si.innersysname);
                    Util.SshWriteAndExpect(shell, 
                        "cp /root/iou/lab_enterprise/IFMAP /root/iou/public/iou2net.pl /root/iou/public/iourc /root/iou/public/stop.sh ./ ", 
                        si.innersysname);
                }
                else
                    return "思科交换机公共文件生成失败";
            }

            return "配置文件生成成功！";
        }
        catch (System.Exception ex)
        {
            return ex.Message;
        }
    }

    #region 解析XML数据，还原成列表

    private List<FileBase> ParseXMLtoList(XElement root)
    {
        List<FileBase> list = new List<FileBase>();
        foreach (XElement elem in root.Elements())
        {
            FileBase fb = new FileBase();

            string filename = elem.Element("FileName").Value;
            string filepath = elem.Element("FilePath").Value;
            string filecontent = elem.Element("FileContent").Value;
            string devicetype = elem.Element("DeviceType").Value;

            fb.fileName = filename;
            fb.filePath = filepath;
            fb.fileContent = Util.DealSpecialChar(filecontent);
            fb.deviceType = Int32.Parse(devicetype);
            list.Add(fb);
        }
        return list;
    }

    private bool GenerateCommonFile(XElement root, SshShell shell)
    {
        // 1. 是否需要拷贝Cisco公共文件
        XElement CiscoComon = root.Element("CiscoCommon");
        string path = CiscoComon.Value;
        if (path != "false")
        {
            // 2.1 切换到文件的目录
            Util.SshWriteAndExpect(shell, "cd ~", si.sysname);  // 进入根目录
            string res = Util.SshWriteAndExpect(shell, "cd " + path, si.sysname);   // 进入Cisco公共文件目录
            if (!res.Contains("No such file or directory"))
            {
                Util.SshWriteAndExpect(shell, "echo >> NETMAP", si.sysname);        // 防止没有连纤时NETMAP未生成
                Util.SshWriteAndExpect(shell, "sed -i \"s/debian/`hostname`/g\" NETMAP", si.sysname);   // 替换HostName
                string str = "cp /home/lab_admin/public/CiscoIOU/IFMAP  /home/lab_admin/public/CiscoIOU/iourc /home/lab_admin/public/CiscoIOU/py2net.py /home/lab_admin/public/CiscoIOU/stop.sh ./ ";
                Util.SshWriteAndExpect(shell, str, si.sysname);
            }
            else
                return false;
        }
        return true;
    }

    #endregion
}