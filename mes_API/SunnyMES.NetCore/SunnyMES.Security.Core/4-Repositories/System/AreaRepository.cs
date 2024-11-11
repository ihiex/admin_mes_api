using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories
{
    public class AreaRepository : BaseRepository<Area, string>, IAreaRepository
    {
        public AreaRepository()
        {
        }

        public AreaRepository(IDbContextCore dbContext) : base(dbContext)
        {
        }
    }
}