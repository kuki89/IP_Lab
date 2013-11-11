using System.Windows.Media.Imaging;

//*******************************************************************
//                                                                  *
//                              华为                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Device
{
    public class Device_HuaWei : Device
    {
        protected override void Init_Property()
        {
            base.Init_Property();

            DP.Product = "华为";
            DP.Type = Enum.DeviceType.DEVICE_TYPE_HUAWEI;
            DP.Prefix = "RH";

            DP.Name = DP.Prefix;

            DP.Card_List.Add(new DeviceCard(
                Enum.DeviceCardType.DEVICE_CARD_TYPE_ETHERNET,
                "E0/", 0, 10));
        }

        protected override void Init_Control()
        {
            ImageHeight = 43;
            ImageWidth = 58;
            IcoImage = new BitmapImage(new Uri(
                string.Format(@"/{0};component/Images/router.png", Util.Func.ProjectName()), UriKind.Relative));


            base.Init_Control();
        }
    }
}
