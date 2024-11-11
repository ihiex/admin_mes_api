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
    public class SC_luMachineFamilyTypeRepositories : BaseCustomRepository<SC_luMachineFamilyType, string>, ISC_luMachineFamilyTypeRepositories
    {
        public SC_luMachineFamilyTypeRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            
        }
    }
}
