using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.PO;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.Repositories.PO
{
    public class SC_luProductionOrderDetailDefRepositories : BaseCustomRepository<SC_luProductionOrderDetailDef, string>, ISC_luProductionOrderDetailDefRepositories
    {
        public SC_luProductionOrderDetailDefRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            
        }
    }
}
