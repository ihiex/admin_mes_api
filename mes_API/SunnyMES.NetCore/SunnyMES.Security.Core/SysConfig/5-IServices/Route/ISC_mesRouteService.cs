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
    public interface ISC_mesRouteService : IServiceReport<string>
    {
        Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP);

        Task<string> Insert(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null);

        Task<string> Delete(string Id, IDbTransaction trans = null);

        Task<string> Update(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null);

        Task<string> Clone(SC_mesRoute v_SC_mesRoute, IDbTransaction trans = null);

        Task<PageResult<SC_mesRouteDto>> FindWithPagerMyAsync(SC_mesRouteSearch search);

        Task<string> ShowXMLRouteV2(string Id, IDbTransaction trans = null);
        Task<string> SaveXMLRouteV2(SC_XMLRouteV2 v_XMLRouteV2, IDbTransaction trans = null);

        Task<string> SetXMLToXMLV2(string Id, IDbTransaction trans = null);

        Task<string> Undo(string RouteId, string HistoryId, IDbTransaction trans = null);

        Task<List<SC_mesRouteHistory>> GetmesRouteHistory(string RouteId);

        Task<string> ShowRouteHistory(string Id, IDbTransaction trans = null);

    }
}


