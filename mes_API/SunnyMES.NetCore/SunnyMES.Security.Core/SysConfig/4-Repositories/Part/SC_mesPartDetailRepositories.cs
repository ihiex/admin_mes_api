using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_mesPartDetailRepositories : BaseCustomRepository<SC_mesPartDetail,string>, ISC_mesPartDetailRepositories
    {
        public SC_mesPartDetailRepositories(IDbContextCoreCustom dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
