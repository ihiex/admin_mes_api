﻿using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_luDefectTypeRepositories :BaseCustomRepository<SC_luDefectType, string>, ISC_luDefectTypeRepositories
    {
        public SC_luDefectTypeRepositories(IDbContextCoreCustom dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
