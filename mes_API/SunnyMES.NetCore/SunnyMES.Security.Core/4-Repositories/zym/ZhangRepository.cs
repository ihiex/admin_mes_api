using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class ZhangRepository : BaseRepository<Zhang, string>, IZhangRepository
    {
        public ZhangRepository()
        {
        }

        public ZhangRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }

    }
}