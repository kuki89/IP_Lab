using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Data;
using System.Data.Common;
using System.Configuration;

/// <summary>
/// 获取目前可用实验室列表
/// </summary>
public class WSGetUsableServer : WebServiceObject
{
    private DbHelper sql = new DbHelper(ConfigurationManager.ConnectionStrings["CONN"].ConnectionString);

    public override string ExecuteCommand(string command, string data)
    {
        try
        {
            string ser_list = "";

            string strsql = "select lab_ip from dbo.lab where using = 'false'";
            DbCommand cmd = sql.GetSqlStringCommond(strsql);
            DataTable dt = sql.ExecuteDataTable(cmd);

            foreach (DataRow dr in dt.Rows)
            {
                ser_list += dr["lab_ip"].ToString() + ";";
            }

            return ser_list;
        }
        catch (System.Exception ex)
        {
            return "false - " + ex.Message;
        }
    }
}