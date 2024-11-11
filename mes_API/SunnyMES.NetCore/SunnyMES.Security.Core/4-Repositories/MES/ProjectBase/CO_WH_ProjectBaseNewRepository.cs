using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.MES.ProjectBase;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.Repositories.MES.ProjectBase
{
    public class CO_WH_ProjectBaseNewRepository : BaseCustomRepository<CO_WH_ProjectBaseNew, string>, ICO_WH_ProjectBaseNewRepository
    {


        public CO_WH_ProjectBaseNewRepository(IDbContextCoreCustom context) : base(context)
        {
            base.primaryKey = "FID";
            _dbContext = context;
        }


        public override async Task<long> InsertAsync(CO_WH_ProjectBaseNew entity, IDbTransaction trans = null)
        {
            if (entity is null or { FProjectNO: "" } )
                return -1;

            var tmpEntity = await GetProjectBaseEntityByProjectNO(entity.FProjectNO);
            if (tmpEntity is not null)
                return -2;

            if (entity.FCountByCase<= 0)
                return -3;
            if (entity.FTotalWeight <= 0)
                return -4;
            if (entity.FWeightByPallet <= 0)
                return -5;

            return await base.InsertAsync(entity, trans);
        }

        public override Task<bool> UpdateAsync(CO_WH_ProjectBaseNew entity, string primaryKey, IDbTransaction trans = null)
        {
            return base.UpdateAsync(entity, primaryKey, trans);
        }

        public async Task<CO_WH_ProjectBaseNew> GetProjectBaseEntityByProjectNO(string  projectNo)
        {
            return await DapperConn.QueryFirstOrDefaultAsync<CO_WH_ProjectBaseNew>($"SELECT * FROM dbo.CO_WH_ProjectBaseNew WHERE FProjectNO = '{projectNo}'", null, null, I_DBTimeout, null);
        }
    }
}
