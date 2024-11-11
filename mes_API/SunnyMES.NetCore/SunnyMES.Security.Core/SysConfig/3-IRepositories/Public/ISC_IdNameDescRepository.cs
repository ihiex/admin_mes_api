using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Models;


namespace SunnyMES.Security.IRepositories
{
    public interface ISC_IdNameDescRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_IdNameDesc v_IdNameDesc,string S_TabName, IDbTransaction trans = null);

        Task<string> Delete(string Id, string S_TabName, IDbTransaction trans = null);

        Task<string> Update(SC_IdNameDesc v_IdNameDesc, string S_TabName, IDbTransaction trans = null);

        Task<string> Clone(SC_IdNameDesc v_IdNameDesc, string S_TabName, IDbTransaction trans = null);
        Task<List<SC_IdNameDesc>> FindWithPagerMyAsync(SC_IdNameDescSearch search, string S_TabName, PagerInfo info);




    }
}
