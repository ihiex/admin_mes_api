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
    public interface ISC_mesLineRepository : IRepositoryReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);
        
        Task<string> Insert(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null);

        Task<string> Clone(SC_mesLineDto v_SC_mesLineDto, IDbTransaction trans = null);
        Task<List<SC_mesLineALLDto>> FindWithPagerMyAsync(SC_mesLineSearch search, PagerInfo info);


        Task<string> InsertDetail(string ParentId, SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null);
        Task<string> DeleteDetail(string Id, IDbTransaction trans = null);
        Task<string> UpdateDetail(SC_mesLineDetailDto v_DetailDto, IDbTransaction trans = null);
        Task<SC_mesLineDetailList> List_Detail(string ParentId);
    }
} 