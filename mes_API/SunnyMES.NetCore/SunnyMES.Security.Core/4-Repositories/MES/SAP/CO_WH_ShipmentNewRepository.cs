using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Repositories.MES.SAP
{
    public class CO_WH_ShipmentNewRepository : BaseCustomRepository<CO_WH_ShipmentNew_T, string>, ICO_WH_ShipmentNewRepository
    {
        public CO_WH_ShipmentNewRepository()
        {
            
        }
        public CO_WH_ShipmentNewRepository(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            base.primaryKey = "FInterID";
        }

    }
}
