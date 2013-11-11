using System;
using System.IO;
using Tamir.SharpSsh;
using System.Configuration;

/// <summary>
/// StopDevice 到实验室环境中停止设备
/// </summary>
public class StopDevice
{
    private FileBase file;
    private string sysname;
    private SshShell shell;

    public StopDevice(FileBase f, SshShell s, string n)
    {
        file = f;
        shell = s;
        sysname = n;
    }

    public bool Stop()
    {
        try
        {
            // 2.2 对内容进行切分
            string[] splitstr = { "\\n" };
            string[] stops = file.fileStop.Split(splitstr,
                StringSplitOptions.RemoveEmptyEntries);
            foreach (string stop in stops)
                Util.SshWriteAndExpect(shell, stop, sysname);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public string GetFileName()
    {
        return file.fileName;
    }
}