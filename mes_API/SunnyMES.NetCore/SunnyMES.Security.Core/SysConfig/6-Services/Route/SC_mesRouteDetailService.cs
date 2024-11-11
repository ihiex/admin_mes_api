using Quartz.Impl.Triggers;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.Repositories;
using static NPOI.HSSF.Util.HSSFColor;

namespace SunnyMES.Security.Services
{
    public class SC_mesRouteDetailService : BaseServiceReport<string>, ISC_mesRouteDetailService
    {
        private readonly ISC_mesRouteDetailRepository _repository;
        private readonly ILogService _logService;

        public SC_mesRouteDetailService(ISC_mesRouteDetailRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            return await _repository.GetConfInfo(I_Language, I_LineID, I_StationID, I_EmployeeID, S_CurrentLoginIP);
        }

        public async Task<string> Insert(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            return await _repository.Insert(v_SC_mesRouteDetail, trans);
        }

        public async Task<string> Delete(string Id, IDbTransaction trans = null)
        {
            return await _repository.Delete(Id, trans);
        }

        public async Task<string> Update(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            return await _repository.Update(v_SC_mesRouteDetail, trans);
        }

        public async Task<string> Clone(SC_mesRouteDetail v_SC_mesRouteDetail, IDbTransaction trans = null)
        {
            return await _repository.Clone(v_SC_mesRouteDetail, trans);
        }

        public async Task<SC_mesRouteDetailList> List_mesRouteDetail(string RouteID) 
        {
            return await _repository.List_mesRouteDetail(RouteID);
        }


    }
}

