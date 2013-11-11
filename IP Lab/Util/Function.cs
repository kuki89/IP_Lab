using System;
using System.Windows;
using System.Windows.Shapes;
using System.Text;
using System.Security.Cryptography;
using System.IO;

//*******************************************************************
//                                                                  *
//                  常调用的静态方法                              *
//                                                                  *
//*******************************************************************

namespace IP_Lab.Util
{
    public class Function
    {
        public static string GetProjectName()
        {
            return Application.Current.GetType().Assembly.FullName.Split(',')[0];
        }

        public static string GetWebAddress()
        {
            char split = '/';
            string[] address = Application.Current.Host.Source.ToString().Split(split);

            string str = "";
            for (int i = 2; i < address.Length - 2; i++)
                str += address[i] + "/";
            str = "http://" + str;

            return str;
        }

        public static string GetWebServiceAddress()
        {
            //return "http://202.101.107.126:9999/" + ConfigurationManager.AppSettings["WebService"];
            return GetWebAddress() + ConfigurationManager.AppSettings["WebService"];
        }

        #region Liang-Barsky算法
        private static double LB_U1, LB_U2;
        /// <summary>
        /// 用于计算直线与矩形交点
        /// </summary>
        /// <param name="data">直线与矩形的数据</param>
        /// <returns>交点坐标</returns>
        public static Line CalCrossPoint(Point ptA, Point ptB, Rect rectA, Rect rectB)
        {
            Line line = getLineFromLBData(ptA, ptB, rectA, rectB);
            return line;
        }
        /// <summary>
        /// 用于从Liang-Barsky算法得到的四个点中取得距离最近的两个点
        /// </summary>
        /// <param name="data">计算数据</param>
        /// <returns></returns>
        private static Line getLineFromLBData(Point ptA, Point ptB, Rect rectA, Rect rectB)
        {
            Line line = new Line();
            Point pointA = Liang_Barsky(ptA, ptB, rectA);
            Point pointB = Liang_Barsky(ptB, ptA, rectB);

            line.X1 = pointA.X;
            line.Y1 = pointA.Y;
            line.X2 = pointB.X;
            line.Y2 = pointB.Y;

            return line;
        }
        // Liang_Barsky算法
        // 其中,pt1为矩形内点，pt2为矩形外点
        private static Point Liang_Barsky(Point pt1, Point pt2, Rect rect)
        {
            double xwmin = rect.Left;
            double ywmin = rect.Top;
            double xwmax = rect.Right;
            double ywmax = rect.Bottom;
            double x1 = (int)(pt1.X);
            double y1 = (int)(pt1.Y);
            double x2 = (int)(pt2.X);
            double y2 = (int)(pt2.Y);

            double dx, dy;
            LB_U1 = 0.0f;//初始化两个始边斜率
            LB_U2 = 1.0f;
            dx = x2 - x1;

            if (clipTest(-dx, x1 - xwmin))
            {
                if (clipTest(dx, xwmax - x1))
                {
                    dy = y2 - y1;
                    if (clipTest(-dy, y1 - ywmin))
                    {
                        if (clipTest(dy, ywmax - y1))
                        {
                            if (LB_U2 < 1.0)
                            {
                                // 加上0.5主要用于四舍五入
                                x2 = (int)(x1 + LB_U2 * dx + 0.5);
                                y2 = (int)(y1 + LB_U2 * dy + 0.5);
                            }
                            if (LB_U1 > 0.0)
                            {
                                x1 = (int)(x1 + LB_U1 * dx + 0.5);
                                y1 = (int)(y1 + LB_U1 * dy + 0.5);
                            }
                        }
                    }
                }
            }

            return new Point(x2, y2);
        }
        /// <summary>
        /// 裁剪测试
        /// </summary>
        /// <param name="p"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        private static bool clipTest(double p, double q)
        {
            bool flag = true;
            double r;
            if (p < 0.0)
            {
                r = q / p;
                if (r > LB_U2)
                {
                    flag = false;
                }
                else if (r > LB_U1)
                {
                    LB_U1 = r;
                    flag = true;
                }
            }
            else if (p > 0.0)
            {
                r = q / p;
                if (r < LB_U1)
                {
                    flag = false;
                }
                else if (r < LB_U2)
                {
                    LB_U2 = r;
                    flag = true;
                }
            }
            else if (q < 0.0)
            {
                flag = false;
            }
            return flag;
        }
        #endregion

        #region 判断点在直线上

        private static double Pt2Pt(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt((double)((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
        }

        private static double DistPt2Line(double lx1, double ly1, double lx2, double ly2, double px, double py)
        {
            double abx = lx2 - lx1;
            double aby = ly2 - ly1;
            double acx = px - lx1;
            double acy = py - ly1;
            double f = abx * acx + aby * acy;
            if (f < 0) return Pt2Pt(lx1, ly1, px, py);

            double d = abx * abx + aby * aby;
            if (f > d) return Pt2Pt(lx2, ly2, px, py);

            //   D   =   a   +   [(ab*ac)/(ab*ab)]*ab   
            f /= d;
            double dx = lx1 + f * abx;
            double dy = ly1 + f * aby;
            return Pt2Pt(dx, dy, px, py);
        }

        public static bool HitLine(Point pt, Point ptStart, Point ptEnd, int nNear)
        {
            if (DistPt2Line(ptStart.X, ptStart.Y, ptEnd.X, ptEnd.Y, pt.X, pt.Y) < (double)nNear)
                return true;
            else
                return false;
        }

        #endregion

        #region 加密、解密

        /// <summary> 
        /// Encrypt the data 
        /// </summary> 
        /// <param name="input">String to encrypt</param> 
        /// <returns>Encrypted string</returns> 
        public static string Encrypt(string input, string password)
        {

            byte[] utfData = UTF8Encoding.UTF8.GetBytes(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string encryptedString = string.Empty;
            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);

                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform encryptTransform = aes.CreateEncryptor())
                {
                    using (MemoryStream encryptedStream = new MemoryStream())
                    {
                        using (CryptoStream encryptor =
                            new CryptoStream(encryptedStream, encryptTransform, CryptoStreamMode.Write))
                        {
                            encryptor.Write(utfData, 0, utfData.Length);
                            encryptor.Flush();
                            encryptor.Close();

                            byte[] encryptBytes = encryptedStream.ToArray();
                            encryptedString = Convert.ToBase64String(encryptBytes);
                        }
                    }
                }
            }
            return encryptedString;
        }

        /// <summary> 
        /// Decrypt a string 
        /// </summary> 
        /// <param name="input">Input string in base 64 format</param> 
        /// <returns>Decrypted string</returns> 
        public static string Decrypt(string input, string password)
        {

            byte[] encryptedBytes = Convert.FromBase64String(input);
            byte[] saltBytes = Encoding.UTF8.GetBytes(password);
            string decryptedString = string.Empty;
            using (var aes = new AesManaged())
            {
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, saltBytes);
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;
                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                using (ICryptoTransform decryptTransform = aes.CreateDecryptor())
                {
                    using (MemoryStream decryptedStream = new MemoryStream())
                    {
                        CryptoStream decryptor =
                            new CryptoStream(decryptedStream, decryptTransform, CryptoStreamMode.Write);
                        decryptor.Write(encryptedBytes, 0, encryptedBytes.Length);
                        decryptor.Flush();
                        decryptor.Close();

                        byte[] decryptBytes = decryptedStream.ToArray();
                        decryptedString =
                            UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }

            return decryptedString;
        }

        #endregion
    }
}
