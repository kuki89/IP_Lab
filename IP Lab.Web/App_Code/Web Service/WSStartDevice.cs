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
/// Web Service 启动设备
/// </summary>
public class WSStartDevice : WebServiceObject
{
        #region 变量

    Server_Information si;

    protected List<FileBase> FileList;
    protected List<FileBase> CiscoSwitchList; // 思科交换机

    #endregion

    public WSStartDevice()
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

            XDocument xdoc = XDocument.Parse(data);

            XElement filelist = xdoc.Element("IP_Lab").Element("StartList");
            XElement floder = xdoc.Element("IP_Lab").Element("Floder");
            ParseXMLtoList(filelist);

            SshShell shell = new SshShell(si.serverip, si.usrname, si.pwd);
            shell.Connect();
            if (shell.Expect(new Regex(si.sysname), 5) == null)
                throw new Exception("连接服务器出错！");

            if (CiscoSwitchList.Count > 0)
            {
                // 登录到设备上
                Util.SshWriteAndExpect(shell, "cd ~", si.sysname);
                Util.SshWriteAndExpect(shell, "ssh " + si.innerusrname + "@" + si.innerservip, "Password:");
                Util.SshWriteAndExpect(shell, si.innerpwd, si.innersysname);

                // 先启动思科交换机
                foreach (FileBase file in CiscoSwitchList)
                {
                    StartDevice sd = new StartDevice(file, shell, si.innersysname);
                    if (sd.Start())
                        continue;
                    else
                        return sd.GetFileName() + " 启动失败！";
                }

                Util.SshWriteAndExpect(shell, "cd ~", si.innersysname);
                Util.SshWriteAndExpect(shell, "exit", si.sysname);
            }

            Util.SshWriteAndExpect(shell, "cd ~", si.sysname);

            foreach (FileBase file in FileList)
            {
                StartDevice sd = new StartDevice(file, shell, si.sysname);
                if (sd.Start())
                    continue;
                else
                    return sd.GetFileName() + " 启动失败！";
            }
  
            return "启动设备成功！";
        }
        catch (System.Exception ex)
        {
            return ex.Message;
        }
    }

    #region 解析XML数据，还原成列表

    private void ParseXMLtoList(XElement root)
    {
        foreach (XElement elem in root.Elements())
        {
            FileBase fb = new FileBase();

            string filename = elem.Element("FileName").Value;
            string filepath = elem.Element("FilePath").Value;
            string filereturn = elem.Element("FileReturn").Value;
            string timeout = elem.Element("Timeout").Value;

            fb.fileName = filename;
            fb.filePath = filepath;
            fb.fileReturn = filereturn;
            fb.startTimeOut = Int32.Parse(timeout);

            if (elem.Name == "Device")
                FileList.Add(fb);
            else if (elem.Name == "Switch")
                CiscoSwitchList.Add(fb);

        }
    }

    #endregion
}