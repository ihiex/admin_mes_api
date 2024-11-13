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
    public class SC_luPartFamilyTypeDetailDefRepositories : BaseCustomRepository<SC_luPartFamilyTypeDetailDef, string>, ISC_luPartFamilyTypeDetailDefRepositories
    {
        public SC_luPartFamilyTypeDetailDefRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
