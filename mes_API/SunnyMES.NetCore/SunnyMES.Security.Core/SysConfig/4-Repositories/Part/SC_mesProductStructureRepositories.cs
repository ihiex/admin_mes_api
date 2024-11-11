using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_mesProductStructureRepositories : BaseCustomRepository<SC_mesProductStructure,string>, ISC_mesProductStructureRepositories
    {
        public SC_mesProductStructureRepositories(IDbContextCoreCustom dbContext) :base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
