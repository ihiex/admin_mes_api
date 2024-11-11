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
    public interface ISC_IdDescService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_IdDesc v_IdDesc, string S_TabName, IDbTransaction trans = null);

        Task<string> Delete(string Id, string S_TabName, IDbTransaction trans = null);

        Task<string> Update(SC_IdDesc v_IdDesc, string S_TabName, IDbTransaction trans = null);

        Task<string> Clone(SC_IdDesc v_IdDesc, string S_TabName, IDbTransaction trans = null);

        Task<PageResult<SC_IdDesc>> FindWithPagerSearchAsync(SC_IdDescSearch search, string S_TabName);
    }
}
