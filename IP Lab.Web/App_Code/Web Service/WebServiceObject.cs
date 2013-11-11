using System.Configuration;
using System.Collections.Generic;

/// <summary>
/// WebServiceObject 的摘要说明
/// Web Service的基类
/// </summary>
public class WebServiceObject
{
    public virtual string ExecuteCommand(string command, string data)
    {
        return "Hello World";
    }
}