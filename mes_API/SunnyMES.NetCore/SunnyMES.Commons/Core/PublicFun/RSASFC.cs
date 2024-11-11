using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.PublicFun
{
    /// <summary>
    /// RSASFC
    /// </summary>
    public class RSASFC
    {
        static string publicKey = "<RSAKeyValue><Modulus>uhso+CK5u0r0S2Bb9By5I+rH60xPmMwHuLhuRQaInndpgvOWZbxDc8sZK4GoPIvLnyxm9INr3DL8tZWbMbcYPvQqLKc9SFcI7dQ+o2X8h4BJH43otLn3SdP1d4AcF8ltXsEXQdS0xPdvRzGkZgi1xTJ46rMEEgK/9WQxBHERcNU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        static string privateKey = "<RSAKeyValue><Modulus>uhso+CK5u0r0S2Bb9By5I+rH60xPmMwHuLhuRQaInndpgvOWZbxDc8sZK4GoPIvLnyxm9INr3DL8tZWbMbcYPvQqLKc9SFcI7dQ+o2X8h4BJH43otLn3SdP1d4AcF8ltXsEXQdS0xPdvRzGkZgi1xTJ46rMEEgK/9WQxBHERcNU=</Modulus><Exponent>AQAB</Exponent><D>BUPLbumck33VV2SMdWVyn19+9FseTVZISaN+CxnaN5FtPLUjZhFjXx05ww9R8RSLWB9rcjNdk8clewWWdFuXpfPqjj/4AVpLxOm9ZTP5WPqPo19b2/reUR2mn3HRLz8Sbhw4ZFu6OeXF9UrwF3kKD5hrsUO2PWS040sYfSXGSWk=</D><P>4p+1bBw+nInt0N8/nXjvwW/zoLj2xnPfpt9UnMmM5PsXrcCoEEwahXjN03XHf/fS3fDVla7sBZaLVSOooCwW8w==</P><Q>0jroqQFfkpL+JdmZK02cLToc4FAk27j97Q2zQjwG2kXtyUIVvxTD2hhLZtP7cZvkLfexx4PxY01Qp4W7DgFbFw==</Q><DP>VwiA80kRnqq2A36Jft+gLEjjZrlCRMrhfMPOSfx5uMLZwCf6I3Amy1WurmRQPswdVpEUZczs5eSAFC2CqCjmkw==</DP><DQ>HJdYZwvBxLxrBhjG5QXEFL6PiM49hQhuuFuhooNpZywVf8aWEIuxayrcrlpsGvJZoQrLydee76NnMbFVVD1I9w==</DQ><InverseQ>3LdMeERBUg/Rh+ujZMPa/8KqvgVgBg1x+Jz3w+Gmng795JWH4v6tqgfO9WC6ud5x1Lmp5I8E3PEY4II1kZ3wxA==</InverseQ></RSAKeyValue>";


        /// <summary>
        /// RSA公钥加密
        /// </summary>
        /// <param name="value">待加密的明文</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaEncrypt(string value, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            string S_Result = "";
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(publicKey);//将公钥导入到RSA对象中，准备加密；
                var buffer = Encoding.UTF8.GetBytes(value);
                buffer = rsa.Encrypt(buffer, false);
                S_Result = Convert.ToBase64String(buffer);
            }
            catch
            {
                S_Result = "";
            }
            return S_Result;
        }



        /// <summary>
        /// RSA私钥解密
        /// </summary>
        /// <param name="value">密文</param>
        /// <param name="usePkcs8">是否使用pkcs8填充</param>
        /// <returns></returns>
        public static string RsaDecrypt(string value, bool usePkcs8 = false)
        {
            if (string.IsNullOrEmpty(value)) return value;

            string S_Result = "";
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(1024);
                rsa.FromXmlString(privateKey);//将私钥导入RSA中，准备解密；          
                var buffer = rsa.Decrypt(Convert.FromBase64String(value), false);

                S_Result = Encoding.UTF8.GetString(buffer);
            }
            catch
            {
                S_Result = "";
            }
            return S_Result;
        }
    }
}
