
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.Models.Shift;

namespace SunnyMES.Security.SysConfig.IRepositories.Shift
{
    public class SC_mesShiftDetailRepositories : BaseCustomRepository<SC_mesShiftDetail, int>, ISC_mesShiftDetailRepositories
    {
        public SC_mesShiftDetailRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
