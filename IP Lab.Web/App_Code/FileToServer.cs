using System;
using System.IO;
using Tamir.SharpSsh;
using System.Configuration;

/// <summary>
/// 文件写入配置
/// </summary>
public class FileToServer
{
    private FileBase file;
    private string sysname;
    private SshShell shell;

    public FileToServer(FileBase f, SshShell s, string n)
    {
        file = f;
        shell = s;
        sysname = n;
    }

    #region 将文件写入服务器

    public bool WriteFileToServer()
    {
        // 1. 判断该目录是否存在
        ChdirOrCreatedir(file.filePath);
        // 2. 将该文件添加至服务器
        return WriteFile();
    }

    // 切换到文件指定的目录，如果不存在，则创建该目录
    protected void ChdirOrCreatedir(string path)
    {
        // 首先，切换到根目录
        Util.SshWriteAndExpect(shell, "cd ~", sysname);
        string res = Util.SshWriteAndExpect(shell, "cd " + path, sysname);

        if (res.Contains("No such file or directory"))
        {
            // 1. 对目录进行分割
            char[] splitchar = { '/' };
            string[] paths = path.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);

            // 2. 切换到最近的已存在的目录
            int index = 0;
            for (; index < paths.Length; index++)
            {
                if (Util.SshWriteAndExpect(shell, "cd " + paths[index], sysname)
                    .Contains("No such file or directory"))
                    break;
            }

            // 3. 创建未存在的目录 
            for (; index < paths.Length; index++)
            {
                Util.SshWriteAndExpect(shell, "mkdir " + paths[index], sysname);
                Util.SshWriteAndExpect(shell, "cd " + paths[index], sysname);
            }
        }
        else if (res.Contains("cd: can't cd to"))
        {
            // 1. 对目录进行分割
            char[] splitchar = { '/' };
            string[] paths = path.Split(splitchar, StringSplitOptions.RemoveEmptyEntries);
            if (paths.Length == 2)
            {
                Util.SshWriteAndExpect(shell, "cd /root/iou/", sysname);
                Util.SshWriteAndExpect(shell, "mkdir " + paths[1], sysname);
                Util.SshWriteAndExpect(shell, "cd " + paths[1], sysname);
            }
        }
    }

    protected bool WriteFile()
    {
        // 2.1 切换到文件的目录
        Util.SshWriteAndExpect(shell, "cd ~", sysname);
        string res = Util.SshWriteAndExpect(shell, "cd " + file.filePath, sysname);

        if (res.Contains("No such file or directory")
            || res.Contains("cd: can't cd to"))
        {
            return false;
        }
        else
        {
            // 2.2 对内容进行切分
            string[] splitstr = { "\\n" };
            string[] contents = file.fileContent.Split(splitstr, 
                StringSplitOptions.RemoveEmptyEntries);

            int i = 0;
            if (contents.Length > 0)
            {
                if (contents[0] == "#!/bin/bash")
                {
                    // 对于#!/bin/bash，正确的应该是   echo '#!/bin/bash'
                    string str = "echo '#!/bin/bash' >> " + file.fileName;
                    Util.SshWriteAndExpect(shell, str, sysname);
                    i++;
                }
            }

            for (; i < contents.Length; i++)
            {
                string data = contents[i].Replace("\"", "\\\"");

                string str = "echo \"" + data.Replace("$", "\\$") + "\" >> " + file.fileName;
                Util.SshWriteAndExpect(shell, str, sysname);
            }

        }

        // 2.3 修改权限
        Util.SshWriteAndExpect(shell, "chmod 755 " + file.fileName, sysname);

        return true;
    }

    #endregion
}