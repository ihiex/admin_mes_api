using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Mapping;
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
    public class MaxdataService : BaseService<Maxdata, MaxdataOutputDto, string>, IMaxdataService
    {


        private readonly IMaxdataRepository _appRepository;
        private readonly ILogService _logService;
        public MaxdataService(IMaxdataRepository repository, ILogService logService) : base(repository)
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
        public override long Insert(Maxdata entity, IDbTransaction trans = null)
        {
            long result = repository.Insert(entity, trans);
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
        public override async Task<bool> UpdateAsync(Maxdata entity, string id, IDbTransaction trans = null)
        {
            bool result = await repository.UpdateAsync(entity, id, trans);
            this.UpdateCacheAllowApp();
            return result;
        }
        /// <summary>
        /// 异步步新增实体。
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="trans">事务对象</param>
        /// <returns></returns>
        public override async Task<long> InsertAsync(Maxdata entity, IDbTransaction trans = null)
        {
            long result = await repository.InsertAsync(entity, trans);
            this.UpdateCacheAllowApp();
            return result;
        }
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥AppSecret</param>
        /// <returns></returns>
        public Maxdata GetData(string appid, string secret)
        {
            return _appRepository.GetMaxdata(appid, secret);
        }
        /// <summary>
        /// 获取app对象
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <returns></returns>
        public Maxdata GetData(string appid)
        {
            return _appRepository.GetMaxdata(appid);
        }
        public IList<MaxdataOutputDto> SelectMaxdata()
        {
            return _appRepository.SelectMaxdata();
        }

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public override async Task<PageResult<MaxdataOutputDto>> FindWithPagerAsync(SearchInputDto<Maxdata> search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = GetDataPrivilege(false);
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (Id like '%{0}%' or StationID like '%{0}%')", search.Keywords);
            };
            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<Maxdata> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);
            PageResult<MaxdataOutputDto> pageResult = new PageResult<MaxdataOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<MaxdataOutputDto>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
        public void UpdateCacheAllowApp()
        {
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            IEnumerable<Maxdata> appList = repository.GetAllByIsNotDeleteAndEnabledMark();
            yuebonCacheHelper.Add("AllowAppId", appList);
        }



    }
}