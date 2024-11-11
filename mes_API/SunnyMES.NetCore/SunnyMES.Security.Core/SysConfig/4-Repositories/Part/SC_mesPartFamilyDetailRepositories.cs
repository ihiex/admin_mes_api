using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_mesPartFamilyDetailRepositories : BaseCustomRepository<SC_mesPartFamilyDetail,string>, ISC_mesPartFamilyDetailRepositories
    {
        public SC_mesPartFamilyDetailRepositories(IDbContextCoreCustom dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
