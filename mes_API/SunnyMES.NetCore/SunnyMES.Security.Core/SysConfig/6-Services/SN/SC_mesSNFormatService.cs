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
    public class SC_mesSNFormatService : BaseServiceReport<string>, ISC_mesSNFormatService
    {
        private readonly ISC_mesSNFormatRepository _repository;
        private readonly ILogService _logService;

        public SC_mesSNFormatService(ISC_mesSNFormatRepository repository, ILogService logService) : base(repository)
        {
            _repository = repository;
            _logService = logService;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            return await _repository.GetConfInfo(I_Language, I_LineID, I_StationID, I_EmployeeID, S_CurrentLoginIP);
        }

        public async Task<string> Insert(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            return await _repository.Insert(v_SC_mesSNFormat, trans);
        }

        public async Task<string> Delete(string Id, IDbTransaction trans = null)
        {
            return await _repository.Delete(Id, trans);
        }

        public async Task<string> Update(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            return await _repository.Update(v_SC_mesSNFormat, trans);
        }

        public async Task<string> Clone(SC_mesSNFormat v_SC_mesSNFormat, IDbTransaction trans = null)
        {
            return await _repository.Clone(v_SC_mesSNFormat, trans);
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public async Task<PageResult<SC_mesSNFormatALLDto>> FindWithPagerSearchAsync(SC_mesSNFormatSearch search)
        {

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            List<SC_mesSNFormatALLDto> list = await _repository.FindWithPagerMyAsync(search, pagerInfo);

            decimal v_PageTotal = 0;
            try
            {
                int I_Mod = pagerInfo.RecordCount % pagerInfo.PageSize;
                decimal I_De = pagerInfo.RecordCount / pagerInfo.PageSize;
                v_PageTotal = I_Mod == 0 ? I_De : I_De + 1;
            }
            catch (Exception ex)
            { }

            PageResult<SC_mesSNFormatALLDto> pageResult = new PageResult<SC_mesSNFormatALLDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,
                TotalPages = Convert.ToInt32(v_PageTotal)
            };
            return pageResult;
        }


        public async Task<string> InsertDetail(string ParentId, SC_mesSNSectionEdit v_DetailDto,IDbTransaction trans = null)
        {
            return await _repository.InsertDetail(ParentId, v_DetailDto,trans);
        }

        public async Task<string> DeleteDetail(string Id, IDbTransaction trans = null)
        {
            return await _repository.DeleteDetail(Id, trans);
        }

        public async Task<string> UpdateDetail(SC_mesSNSectionEdit v_DetailDto, IDbTransaction trans = null)
        {
            return await _repository.UpdateDetail(v_DetailDto, trans);
        }
        public async Task<SC_mesSNFormatDetailList> List_Detail(string ParentId)
        {
            return await _repository.List_Detail(ParentId);
        }


        public async Task<string> InsertSNRange(string ParentId, SC_mesSNRange v_DetailDto, IDbTransaction trans = null)
        {
            return await _repository.InsertSNRange(ParentId, v_DetailDto, trans);
        }
        public async Task<string> DeleteSNRange(string Id, IDbTransaction trans = null)
        {
            return await _repository.DeleteSNRange(Id, trans);
        }
        public async Task<string> UpdateSNRange(SC_mesSNRange v_DetailDto, IDbTransaction trans = null)
        {
            return await _repository.UpdateSNRange(v_DetailDto, trans);
        }
        public async Task<SC_mesSNRangeList> List_SNRange(string ParentId)
        {
            return await _repository.List_SNRange(ParentId);
        }
    }
}
