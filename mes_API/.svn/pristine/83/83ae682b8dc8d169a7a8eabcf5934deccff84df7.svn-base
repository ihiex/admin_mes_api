using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_luPartDetailDefRepositories : BaseCustomRepository<SC_luPartDetailDef, string>, ISC_luPartDetailDefRepositories
    {
        public SC_luPartDetailDefRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
