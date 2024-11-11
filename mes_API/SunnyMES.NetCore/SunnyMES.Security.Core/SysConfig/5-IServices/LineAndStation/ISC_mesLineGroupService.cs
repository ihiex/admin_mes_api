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
    public interface ISC_mesLineGroupService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesLineGroup v_SC_mesLineGroupDto, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesLineGroupDto v_SC_mesLineGroupDto, IDbTransaction trans = null);

        Task<string> Clone(SC_mesLineGroupDto v_SC_mesLineGroupDto, IDbTransaction trans = null);
        Task<PageResult<SC_mesLineGroupSearch>> FindWithPagerMyAsync(SC_mesLineGroupSearch search );
    }
}

