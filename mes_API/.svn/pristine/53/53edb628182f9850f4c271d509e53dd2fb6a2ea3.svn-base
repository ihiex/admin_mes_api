using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 服务接口实现
    /// </summary>
    public class BZhangService : BaseServiceGeneric<BZhang, BZhangOutputDto, string>, IBZhangService
    {

        private readonly IBZhangRepository _appRepository;
        private readonly ILogService _logService;
        public BZhangService(IBZhangRepository repository, ILogService logService) : base(repository)
        {
            _appRepository = repository;
            _logService = logService;
        }

        /// <summary>
        /// 同步新增实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public override long Insert(BZhang entity, IDbTransaction trans = null)
        {
            //long result = repository.Insert(entity, trans);
            long result = _appRepository.InsertBZhang(entity, trans);

            this.UpdateCacheAllowApp();
            return result;
        }

        /// <summary>
        /// 异步更新实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="id">主键ID</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public override async Task<long> UpdateAsync2(BZhang entity, string id, IDbTransaction trans = null)
        {
            //bool result = await repository.UpdateAsync(entity, id, trans);

            long result = _appRepository.UpdateBZhang2(entity, id, trans);
            this.UpdateCacheAllowApp();
            return result;
        }
        /// <summary>
        /// 异步步新增实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public override async Task<long> InsertAsync(BZhang entity, IDbTransaction trans = null)
        {
            //long result = await repository.InsertAsync(entity, trans);

            long result = _appRepository.InsertBZhang(entity, trans);
            this.UpdateCacheAllowApp();
            return result;
        }
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        public BZhang GetB_Zhang(string appid, string secret)
        {
            return _appRepository.GetBZhang(appid, secret);
        }
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        public BZhang GetB_Zhang(string appid)
        {
            return _appRepository.GetBZhang(appid);
        }
        public IList<BZhangOutputDto> SelectBZhang()
        {
            return _appRepository.SelectBZhang();
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<BZhangOutputDto>> FindWithPagerAsync(SearchInputDto<BZhang> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (Id like '%{0}%' or Data1 like '%{0}%')", search.Keywords);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<BZhang> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<BZhangOutputDto> pageResult = new PageResult<BZhangOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<BZhangOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
        public void UpdateCacheAllowApp()
        {
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            IEnumerable<BZhang> appList = repository.GetAllByIsNotDeleteAndEnabledMark();
            yuebonCacheHelper.Add("AllowAppId", appList);
        }


        /// <summary>
        /// 按条件批量删除
        /// </summary>
        /// <param name="idsInfo">主键Id集合</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public CommonResult DeleteBatchWhere(DeletesInputDto idsInfo, IDbTransaction trans = null)
        {
            CommonResult result = new CommonResult();
            string where = string.Empty;

            where = "id in ('" + idsInfo.Ids.Join(",").Trim(',').Replace(",", "','") + "')";
            bool bl = repository.DeleteBatchWhere(where);
            if (bl)
            {
                result.ResultCode = "0";
            }
            return result;
        }

        /// <summary>
        /// 按条件批量删除
        /// </summary>
        /// <param name="idsInfo">主键Id集合</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public async Task<CommonResult> DeleteBatchWhereAsync(DeletesInputDto idsInfo, IDbTransaction trans = null)
        {
            CommonResult result = new CommonResult();
            string where = string.Empty;

            where = "id in ('" + idsInfo.Ids.Join(",").Trim(',').Replace(",", "','") + "')";
            bool bl = await repository.DeleteBatchWhereAsync(where);
            if (bl)
            {
                result.ResultCode = "0";
            }
            return result;
        }


    }
}