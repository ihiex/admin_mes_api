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
    public interface ISC_mesLabelRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null);

        Task<string> Clone(SC_mesLabel v_SC_mesLabel, IDbTransaction trans = null);

        Task<List<SC_mesLabelALLDto>> FindWithPagerMyAsync(SC_mesLabelSearch search, PagerInfo info);



        Task<string> InsertDetail(string ParentId, SC_mesLabelField v_DetailDto, IDbTransaction trans = null);
        Task<string> DeleteDetail(string Id, IDbTransaction trans = null);
        Task<string> UpdateDetail(SC_mesLabelField v_DetailDto, IDbTransaction trans = null);

    }
}


