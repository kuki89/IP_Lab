using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Xml.Linq;

/// <summary>
///IPLabWebService 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
// [System.Web.Script.Services.ScriptService]
public class IPLabWebService : System.Web.Services.WebService {

    public IPLabWebService () {

        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string ExecuteCommand(string command, string data) {
        XDocument xdoc = XDocument.Parse(command);

        string cmd = xdoc.Element("IPLab").Element("Command").Value;

        Type type = Type.GetType(cmd, true);
        WebServiceObject wso = (WebServiceObject)Activator.CreateInstance(type);

        return wso.ExecuteCommand(command, data);
    }
    
}
