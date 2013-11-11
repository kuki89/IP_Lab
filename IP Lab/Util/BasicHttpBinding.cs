//*******************************************************************
//                                                                  *
//                           动态设置EndPoint                   *
//                                                                  *
//*******************************************************************
namespace IP_Lab.Util
{
    public class DynamicEndpointHelper
    {
        // BaseUrl是部署服务的Web服务器地址
        private const string BaseUrl = "http://localhost:2440/IP Lab.Web/";

        public static string ResolveEndpointUrl(string endpointUrl, string xapPath)
        {
            string baseUrl = xapPath.Substring(0, xapPath.IndexOf("ClientBin"));
            string relativeEndpointUrl = endpointUrl.Substring(BaseUrl.Length);
            string dynamicEndpointUrl = baseUrl + relativeEndpointUrl;
            return dynamicEndpointUrl;
        }
    }
}
