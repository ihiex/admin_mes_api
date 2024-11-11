using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IRepositories
{
    public interface ISNFormatRepository :IRepositoryGeneric<TabVal,string>   //IRepositoryReport<string>
    {
        Task<string> GetSNRGetNext(string S_SNFormat,string S_ReuseSNByStation,
            string S_ProdOrder,string S_Part,string S_Station,string S_ExtraData);

    }
}

