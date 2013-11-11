using System.Xml;
using System.Xml.Linq;

//*******************************************************************
//                                                                  *
//              系统申请调用web service时所发送的信息                 *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Data.Message
{
    public class MessageSend
    {
        /// <summary>
        /// 所需要调用的Web Service的类名
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// 申请该web service的用户ID
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 所使用的实验室IP
        /// </summary>
        public string LabIP { get; set; }

        public string MsgSendToXMLStr()
        {
            XElement root = new XElement("IPLab");
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            XElement xcommand = new XElement("Command", Command);
            XElement xuserid = new XElement("UserID", UserID);
            XElement xlabip = new XElement("LabIP", LabIP);

            root.Add(xcommand);
            root.Add(xuserid);
            root.Add(xlabip);

            return xdoc.ToString();
        }

    }
}
