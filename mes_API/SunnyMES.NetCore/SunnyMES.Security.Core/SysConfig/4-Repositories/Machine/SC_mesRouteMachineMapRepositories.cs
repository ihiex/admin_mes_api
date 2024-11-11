using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.IRepositories;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;

namespace SunnyMES.Security.SysConfig.Repositories.Machine
{
    public class SC_mesRouteMachineMapRepositories : BaseCustomRepository<SC_mesRouteMachineMap, string>, ISC_mesRouteMachineMapRepositories
    {
        public SC_mesRouteMachineMapRepositories(IDbContextCoreCustom dbContext):base(dbContext)
        {
            
        }
    }
}
