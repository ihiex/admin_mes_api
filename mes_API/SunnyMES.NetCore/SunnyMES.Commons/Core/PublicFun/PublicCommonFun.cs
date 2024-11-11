using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.PublicFun
{
    public class PublicCommonFun
    {
        /// <summary>
        /// 取数据表第一行中对应键的值
        /// </summary>
        /// <param name="ld"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ConvertDynamic(dynamic ld, string key)
        {
            string result = string.Empty;
            foreach (KeyValuePair<string, object> keyvalue in ld[0])
            {
                if (keyvalue.Key == key.Trim() && !string.IsNullOrEmpty(keyvalue.Value?.ToString()))
                {
                    result = keyvalue.Value.ToString();
                    break;
                }
            }

            return result;
        }
    }
}
