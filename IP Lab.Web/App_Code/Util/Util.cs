using System;
using System.Text.RegularExpressions;
using Renci.SshNet;
using Tamir.SharpSsh;

using System.Data;
using System.Data.Common;
using System.Configuration;

using System.IO;
using Microsoft.Win32;
using System.Diagnostics;

//*******************************************************************
//                                                                  *
//                  常调用的静态方法                              *
//                                                                  *
//*******************************************************************

public struct Server_Information
{
    public string serverip;
    public string usrname;
    public string pwd;
    public string sysname;
    public string innerservip;
    public string innerusrname;
    public string innerpwd;
    public string innersysname;
}

public enum DeviceType
{
    DEVICE_TYPE_VPCS = 1,
    DEVICE_TYPE_HUAWEI = 2,
    DEVICE_TYPE_CISCOROUTER = 3,
    DEVICE_TYPE_CISCOSWITCHER = 7,
    DEVICE_TYPE_JUNIPER = 5,
    DEVICE_TYPE_ASAFIREWALL = 6,
    
    DEVICE_TYPE_WINDOWS = 8,
    DEVICE_TYPE_LINUX = 9,
    DEVICE_TYPE_NONE
}

public class Util
{
    private static DbHelper sql = new DbHelper(ConfigurationManager.ConnectionStrings["CONN"].ConnectionString);

    public static string DealSpecialChar(string oristr)
    {
        // 处理XML中的特殊字符，包括"<", ">", "&", """, "'"
        string dststr = oristr;

        dststr = dststr.Replace("&gt;", ">");
        dststr = dststr.Replace("&lt;", "<");
        dststr = dststr.Replace("&quot;", "\"");
        dststr = dststr.Replace("&amp;", "&");
        dststr = dststr.Replace("&apos;", "\'");

        return dststr;
    }

    /// <summary>
    /// 由于SSH使用WriteLine发送命令后，会保留命令，以及最后一行，要把它们裁掉
    /// SSH.NET 使用
    /// </summary>
    public static string GetSSHResult(string str)
    {
        if (string.IsNullOrEmpty(str))
            return null;

        if (str.IndexOf("\r\n") != -1)
            str = str.Substring(str.IndexOf("\r\n"));
        if (str.LastIndexOf("\r\n") != -1)
            str = str.Substring(0, str.LastIndexOf("\r\n"));

        return str;
    }

    /// <summary>
    /// SSH发送命令，并Expect返回值
    /// 若返回所需结果，则返回true
    /// 否则，超时则返回false
    /// </summary>
    /// <param name="stream">ShellStream</param>
    /// <param name="command">发送的命令</param>
    /// <param name="regex">Expect值</param>
    /// <param name="seconds">超时时间(s)</param>
    public static string SshWriteAndExpect(
        SshShell shell,
        string command,
        string regex,
        int seconds = 5     
        )
    {
        shell.WriteLine(command);

        string result = shell.Expect(new Regex(regex), seconds);

        if (result == null)
            throw new Exception("Execute: " + command + System.Environment.NewLine + "Except: " + regex + " 时发生超时错误!" + System.Environment.NewLine 
                + new System.Diagnostics.StackTrace().ToString());
        else
            return result;

    }

    /// <summary>
    /// 从IP信息查询到服务器的相应信息
    /// </summary>
    /// <param name="lab_ip">ip地址</param>
    /// <returns></returns>
    public static Server_Information GetServerInfoFromIP(string lab_ip)
    {
        Server_Information server = new Server_Information();

        string strsql = "select * from dbo.lab where lab_ip = '" + lab_ip + "'";
        DbCommand cmd = sql.GetSqlStringCommond(strsql);

        using (DbDataReader reader = sql.ExecuteReader(cmd))
        {
            if (reader.Read())
            {
                server.serverip = lab_ip;
                server.usrname = reader["lab_username"].ToString();
                server.pwd = reader["lab_password"].ToString();

                server.innerservip = reader["lab_switch_serverip"].ToString();
                server.innerusrname = reader["lab_switch_username"].ToString();
                server.innerpwd = reader["lab_switch_password"].ToString();

                server.sysname = reader["lab_username"].ToString() + reader["lab_sysname"].ToString();
                server.innersysname = reader["lab_switch_sysname"].ToString();
            }
        }

        return server;
    }

    /// <summary>
    /// 对服务器上的文件，执行Winrar操作
    /// </summary>
    /// <param name="command">操作： r - 修复 ； x - 解压缩</param>
    /// <param name="unRarPath">解压路径</param>
    /// <param name="RarPath">原路径</param>
    /// <param name="RarName">文件名</param>
    /// <returns>操作结果</returns>
    public static bool WinRARExecute(string command, string unRarPath, string RarPath, string RarName)
    {
        string rar;
        RegistryKey reg;
        object obj;
        string info;

        try
        {
            reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            obj = reg.GetValue("");
            rar = obj.ToString();
            reg.Close();

            if (Directory.Exists(unRarPath) == false)
            {
                Directory.CreateDirectory(unRarPath);
            }

            info = command + " " + RarName + " " + unRarPath + " -y";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = rar;
            startInfo.Arguments = info;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = RarPath;

            Process prc = new Process();
            prc.StartInfo = startInfo;
            prc.Start();
            prc.WaitForExit();
            prc.Close();
        }
        catch (System.Exception)
        {
            return false;
        }

        return true;
;
    }
}