using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Resources;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace IP_Lab
{
    /// <summary>
    /// 读取app.config文件
    /// </summary>
    /// <remarks>app.config需位于程序的根目录</remarks>

    public static class ConfigurationManager
    {
        static ConfigurationManager()
        {
            AppSettings = new Dictionary<string, string>();
            ReadSettings();
        }

        public static Dictionary<string, string> AppSettings { get; set; }
        private static void ReadSettings()
        {
            string assemblyName = Assembly.GetExecutingAssembly().FullName;
            assemblyName = assemblyName.Substring(0, assemblyName.IndexOf(','));
            string url = String.Format("{0};component/app.config", assemblyName);
            StreamResourceInfo configFile = Application.GetResourceStream(new Uri(url, UriKind.Relative));
            if (configFile != null && configFile.Stream != null)
            {
                Stream stream = configFile.Stream;
                XDocument document = XDocument.Load(stream);
                foreach (XElement element in document.Descendants("appSettings").DescendantNodes())
                {
                    AppSettings.Add(element.Attribute("key").Value, element.Attribute("value").Value);
                }
            }
        }
    }
}
