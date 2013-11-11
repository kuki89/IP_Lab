using System;
using System.IO;
using Tamir.SharpSsh;
using System.Configuration;

/// <summary>
/// 到实验室环境中启动设备
/// </summary>

public class StartDevice
{
    private FileBase file;
    private string sysname;
    private SshShell shell;

    public StartDevice(FileBase f, SshShell s, string n)
    {
        file = f;
        shell = s;
        sysname = n;
    }

    public bool Start()
    {
        // 首先，切换到根目录
        Util.SshWriteAndExpect(shell, "cd ~", sysname);
        Util.SshWriteAndExpect(shell, "cd " + file.filePath, sysname);
        string res = Util.SshWriteAndExpect(shell, "./" + file.fileName, file.fileReturn, file.startTimeOut);

        if (res.Contains(file.fileReturn))
            return true;
        return false;
    }

    public string GetFileName()
    {
        return file.fileName;
    }
}