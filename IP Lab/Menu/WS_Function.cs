using System;
using System.Windows;
using System.Collections.Generic;

using IP_Lab.Device;
using IP_Lab.Data;
using IP_Lab.Data.Message;
using IP_Lab.MyService;

using System.ServiceModel;

//*******************************************************************
//                                                                  *
//                      菜单具体实现类                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Menu
{
    public class WS_Function
    {
        #region 变量

        private Config.ConfigFileMgr configMgr;
        private bool genConfiged;

        #endregion

        public WS_Function()
        {
            configMgr = Util.SingletonGeneric<Config.ConfigFileMgr>.GetInstance;
            genConfiged = false;
        }

        #region 生成配置文件

        public void GenerateConfigFile()
        {
            try
            {
                if (Util.SingletonGeneric<Data.SystemData>.GetInstance.DeviceList.Count == 0)
                {
                    MessageBox.Show("拓扑中尚无设备，请创建！");
                    return;
                }
                MessageSend cmd = new MessageSend();
                cmd.Command = "WSGenerateConfigFile";
                cmd.LabIP = Util.SingletonGeneric<Data.UserData>.GetInstance.LabIP;
                cmd.UserID = Util.SingletonGeneric<Data.UserData>.GetInstance.UserName;


                string strcmd = cmd.MsgSendToXMLStr();
                string strdata = configMgr.GenConfigFile();
                
                PLabWebServiceSoapClient client = new PLabWebServiceSoapClient();
                client.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                client.ExecuteCommandCompleted += new
                    EventHandler<ExecuteCommandCompletedEventArgs>(client_ExecuteCommandCompleted);
                client.ExecuteCommandAsync(strcmd, strdata);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void client_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs e)
        {
            MessageBox.Show(e.Result.ToString());
            if (e.Result.ToString() == "配置文件生成成功！")
                genConfiged = true;
        }

        #endregion

        #region 启动设备

        public void StartDevice(List<DeviceBase> devs)
        {
            try
            {
                if (!genConfiged)
                {
                    MessageBox.Show("配置文件尚未生成，请先生成配置文件！");
                    return;
                }

                MessageSend cmd = new MessageSend();
                cmd.Command = "WSStartDevice";
                cmd.LabIP = Util.SingletonGeneric<Data.UserData>.GetInstance.LabIP;
                cmd.UserID = Util.SingletonGeneric<Data.UserData>.GetInstance.UserName;

                string strcmd = cmd.MsgSendToXMLStr();
                string strdata;
                if (devs.Count == Util.SingletonGeneric<SystemData>.GetInstance.DeviceList.Count)
                    strdata = configMgr.GenStartAllDeviceCmd();
                else
                    strdata = configMgr.GenStartDeviceCmd(devs);

                PLabWebServiceSoapClient start = new PLabWebServiceSoapClient();
                start.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                start.ExecuteCommandCompleted += new
                    EventHandler<ExecuteCommandCompletedEventArgs>(start_ExecuteCommandCompleted);
                start.ExecuteCommandAsync(strcmd, strdata);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        void start_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Result.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        #endregion

        #region 停止设备

        public void StopDevice(List<DeviceBase> devs)
        {
            try
            {
                MessageSend cmd = new MessageSend();
                cmd.Command = "WSStopDevice";
                cmd.LabIP = Util.SingletonGeneric<Data.UserData>.GetInstance.LabIP;
                cmd.UserID = Util.SingletonGeneric<Data.UserData>.GetInstance.UserName;

                string strcmd = cmd.MsgSendToXMLStr();
                string strdata;
                if (devs.Count == Util.SingletonGeneric<SystemData>.GetInstance.DeviceList.Count)
                    strdata = configMgr.GenStopAllDeviceCmd();
                else
                    strdata = configMgr.GenStopDeviceCmd(devs);

                PLabWebServiceSoapClient stop = new PLabWebServiceSoapClient();
                stop.Endpoint.Address = new EndpointAddress(Util.Function.GetWebServiceAddress());
                stop.ExecuteCommandCompleted += new
                    EventHandler<ExecuteCommandCompletedEventArgs>(stop_ExecuteCommandCompleted);
                stop.ExecuteCommandAsync(strcmd, strdata);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void stop_ExecuteCommandCompleted(object sender, ExecuteCommandCompletedEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Result.ToString());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.InnerException.ToString());
            }
        }

        #endregion
    }
}
