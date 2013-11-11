using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.IO;

/// <summary>
/// 获取标准拓扑列表， a lot need to do
/// </summary>
public class WSGetServerTopo : WebServiceObject
{
    public override string ExecuteCommand(string command, string data)
    {
        try
        {
            string topo_list = "";

            DirectoryInfo dir = new DirectoryInfo(HttpRuntime.AppDomainAppPath + @"\Topo");
            foreach (DirectoryInfo dChild in dir.GetDirectories("*"))
                topo_list += dChild.Name + ";";

            return topo_list;
        }
        catch (System.Exception ex)
        {
            return "false - " + ex.Message;
        }
    }
}