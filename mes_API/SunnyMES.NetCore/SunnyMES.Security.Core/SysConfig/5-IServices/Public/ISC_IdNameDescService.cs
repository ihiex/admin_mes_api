using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.IServices
{
    public interface ISC_IdNameDescService : IServiceReport<string>    
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_IdNameDesc v_IdNameDesc, string S_TabName, IDbTransaction trans = null);

        Task<string> Delete(string Id, string S_TabName, IDbTransaction trans = null);

        Task<string> Update(SC_IdNameDesc v_IdNameDesc, string S_TabName, IDbTransaction trans = null);

        Task<string> Clone(SC_IdNameDesc v_IdNameDesc, string S_TabName, IDbTransaction trans = null);

        Task<PageResult<SC_IdNameDesc>> FindWithPagerSearchAsync(SC_IdNameDescSearch search, string S_TabName);
    }
}
