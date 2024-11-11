using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories.Public;
using SunnyMES.Security.IServices.Public;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.Dtos.Public;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.SysConfig.Models.Public;

namespace SunnyMES.Security.Services.Public
{
    public class PublicPropertiesService : BaseCommonService<string>, IPublicPropertiesService
    {
        private readonly IPublicPropertiesRepository iRepository;

        public PublicPropertiesService(IPublicPropertiesRepository iRepository) : base(iRepository)
        {
            this.iRepository = iRepository;
        }

        public async Task<bool> CheckIsExists(string sql)
        {
            return await iRepository.CheckIsExists(sql);
        }

        public async Task<long> CloneAsync(SC_IdDescTab idDescTab)
        {
            return await iRepository.CloneAsync(idDescTab);
        }

        public async Task<long> DeleteAsync(SC_IdDescTab idDescTab)
        {
            return await iRepository.DeleteAsync(idDescTab);
        }

        public async Task<List<SC_IdDesc>> GetCommonTabList()
        {
            return await iRepository.GetCommonTabList();
        }

        public async Task<long> InsertAsync(SC_IdDescTab idDescTab)
        {
            return await iRepository.InsertAsync(idDescTab);
        }

        public async Task<long> UpdateAsync(SC_IdDescTab idDescTab)
        {
            return await iRepository.UpdateAsync(idDescTab);
        }


        public async Task<PageResult<SC_IdDesc>> FindWithPagerAsync(SearchPropertiesInputDto search)
        {
            bool order = search.Order == "asc" ? false : true;
            string where = string.Empty;

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where = $"  Description LIKE '%{search.Keywords}%'";
            }
            List<SC_IdDesc> list = await iRepository.FindWithPagerAsync(where, pagerInfo, search.Sort, order,search.TableName);
            PageResult<SC_IdDesc> pageResult = new PageResult<SC_IdDesc>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list.MapTo<SC_IdDesc>(),
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }
    }
}
