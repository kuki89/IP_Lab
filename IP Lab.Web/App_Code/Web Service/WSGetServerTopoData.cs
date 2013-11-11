using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

/// <summary>
/// 获取给定名字的拓扑文件夹里的拓扑信息
/// </summary>
public class WSGetServerTopoData : WebServiceObject
{
    public override string ExecuteCommand(string command, string data)
    {
        try
        {
            string filename = data;
            string filepath = HttpRuntime.AppDomainAppPath + @"\Topo\" + data + @"\init.topo";

            FileInfo file = new FileInfo(filepath);
            if (file.Exists)
            {
                StreamReader fileStream = new StreamReader(filepath);
                string config = fileStream.ReadToEnd();
                fileStream.Close();
                return config;
            }
            else
                throw new Exception("无法找到拓扑配置文件！");

        }
        catch (System.Exception ex)
        {
            return "false - " + ex.Message;
        }
    }
}