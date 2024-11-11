using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class FilterIPRepository : BaseRepository<FilterIP, string>, IFilterIPRepository
    {
        public FilterIPRepository()
        {
        }

        public FilterIPRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

        /// <summary>
        /// 验证IP地址是否被拒绝
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool ValidateIP(string ip)
        {
            long ipv = ip.Replace(".", "").ToLong();
            string where = " replace(StartIP,'.','')>=" + ipv + " and replace(EndIP,'.','')<=" + ipv + " and FilterType=0 and EnabledMark=1";
            int count = GetCountByWhere(where);
            return count > 0 ? true : false;
        }
    }
}