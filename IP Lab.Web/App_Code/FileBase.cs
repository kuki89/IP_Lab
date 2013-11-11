//*******************************************************************
//                                                                  *
//                        配置文件的基类                          *
//                                                                  *
//*******************************************************************

public class FileBase
{
    public string fileName = "";    // 文件名
    public string filePath = "";    // 文件位置
    public string fileContent = ""; // 文件内容
    public string fileStop = "";    // 关闭设备时的语句
    public string fileReturn = "";  // 配置启动成功时，应返回的内容（用在判断是否启动成功）
    public string version = "";     // 版本号

    public int deviceType = -1;
    public int localPortBase = -1;   // 本地服务端口基数（如3000, 5000)
    public int mapTerminalBase = -1; // 映射到操作终端端口基数

    public int startTimeOut = 5;     // 通过该文件启动设备时的默认超时时间

    public virtual string toString()
    {
        return fileContent;
    }
}
