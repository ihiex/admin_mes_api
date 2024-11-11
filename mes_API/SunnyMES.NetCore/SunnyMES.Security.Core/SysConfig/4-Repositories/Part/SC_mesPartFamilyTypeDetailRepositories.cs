using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_mesPartFamilyTypeDetailRepositories : BaseCustomRepository<SC_mesPartFamilyTypeDetail,string>, ISC_mesPartFamilyTypeDetailRepositories
    {
        public SC_mesPartFamilyTypeDetailRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }


    }
}
