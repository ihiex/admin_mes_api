using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISNFormatService :IServiceGeneric<TabVal,TabVal,string> //IServiceReport<string> 
    {
        Task<string> GetSNRGetNext(string S_SNFormat, string S_ReuseSNByStation,
            string S_ProdOrder, string S_Part, string S_Station, string S_ExtraData);
    }
}