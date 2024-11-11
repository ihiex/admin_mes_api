using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.Linq;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Linq;

namespace SunnyMES.AspNetCore.Common
{
    public static class ExtendErrCode
    {
        public static readonly Dictionary<string, (string CN, string EN)> keyValuePairs;
        static ExtendErrCode()
        {
            var fields = typeof(ErrCode).GetFields();
            var descFields = fields.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where(y => y.AttributeType.Name == "DescriptionAttribute").Count() > 0);
            keyValuePairs = new Dictionary<string, (string, string)>();
            descFields.ForEach(x =>
            {
                var v = x.GetValue(null);
                keyValuePairs.Add(x.Name, (v.ToString(), x.GetDescription()));
            });

        }
        public static (string CN, string EN) LanguageMsg(this string msg)
        {
            var v = ExtendErrCode.keyValuePairs.Where(x => x.Value.Item1 == msg);
            return v?.Count() == 1 ? v.ToList()[0].Value : (msg, msg);
        }


    }
}
