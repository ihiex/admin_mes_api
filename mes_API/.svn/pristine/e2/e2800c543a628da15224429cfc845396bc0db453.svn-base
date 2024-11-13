using System;
namespace SunnyMES.Commons.Core.DataManager
{
    public static class SFCEncode
    {
        static string S_Key = "GG520520@COSMO";
        /// <summary>
        ///作用：将字符串内容转化为16进制数据编码 
        /// </summary>
        /// <param name="strEncode"></param>
        /// <returns></returns>
        public static string Encode(string strEncode)
        {
            string strReturn = "";//  存储转换后的编码
            foreach (short shortx in strEncode.ToCharArray())
            {
                strReturn += shortx.ToString("X2");
            }
            return strReturn;
        }
        /// <summary>
        /// 作用：将16进制数据编码转化为字符串
        /// </summary>
        /// <param name="strDecode"></param>
        /// <returns></returns>
        public static string Decode(string strDecode)
        {
            string sResult = "";
            for (int i = 0; i < strDecode.Length / 2; i++)
            {
                sResult += (char)short.Parse(strDecode.Substring(i * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
            }
            return sResult;
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="v_Password"></param>
        /// <param name="v_key"></param>
        /// <returns></returns>
        public static string EncryptPassword(string v_Password, string v_key)
        {
            if (v_key == "") { v_key = S_Key; }

            int i, j;
            int a = 0, b = 0, c = 0;
            string hexS = "", hexskey = "", midS = "", tmpstr = "";

            hexS = Encode(v_Password);
            hexskey = Encode(v_key);
            midS = hexS;

            for (i = 1; i <= hexskey.Length / 2; i++)
            {
                if (i != 1)
                {
                    midS = tmpstr;
                }
                tmpstr = "";
                for (j = 1; j <= midS.Length / 2; j++)
                {
                    a = (char)short.Parse(midS.Substring((j - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                    b = (char)short.Parse(hexskey.Substring((i - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);

                    //a = (char)short.Parse(Convert.ToString(midS[2 * j - 2]) + Convert.ToString(midS[2 * j-1]), global::System.Globalization.NumberStyles.HexNumber);
                    //b = (char)short.Parse(Convert.ToString(hexskey[2 * i - 2]) + Convert.ToString(hexskey[2 * i-1]), global::System.Globalization.NumberStyles.HexNumber);

                    c = a ^ b;
                    tmpstr += Encode(Convert.ToString((Convert.ToChar(c))));
                }
            }
            return tmpstr;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="v_Password"></param>
        /// <param name="v_key"></param>
        /// <returns></returns>
        public static string DecryptPassword(string v_Password, string v_key)
        {
            if (v_key == "") { v_key = S_Key; }
            int i, j;
            int a = 0, b = 0, c = 0;
            string hexS = "", hexskey = "", midS = "", tmpstr = "";

            try
            {
                hexS = v_Password;
                if (hexS.Length % 2 == 1)
                {
                    return "-1:ciphertext is wrong";
                    //Response.Write("<script>alert(\"密文错误，无法解密字符串\");</script>");
                }
                hexskey = Encode(v_key);
                tmpstr = hexS;
                midS = hexS;
                for (i = hexskey.Length / 2; i >= 1; i--)
                {
                    if (i != hexskey.Length / 2)
                    {
                        midS = tmpstr;
                    }
                    tmpstr = "";
                    for (j = 1; j <= midS.Length / 2; j++)
                    {
                        a = (char)short.Parse(midS.Substring((j - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                        b = (char)short.Parse(hexskey.Substring((i - 1) * 2, 2), global::System.Globalization.NumberStyles.HexNumber);
                        c = a ^ b;
                        tmpstr += Encode(Convert.ToString((Convert.ToChar(c))));
                    }
                }
            }
            catch
            {
                return "-1:ciphertext is wrong";
            }
            return Decode(tmpstr);
        }
    }
}