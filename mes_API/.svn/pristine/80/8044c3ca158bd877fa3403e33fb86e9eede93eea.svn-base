using Dapper;
using System.Data;
using System.Threading.Tasks;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.MES.ProjectBase;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models.MES;

namespace SunnyMES.Security.Repositories.MES.ProjectBase
{
    public class CO_WH_ProjectBaseRepository : BaseCustomRepository<CO_WH_ProjectBase, string>, ICO_WH_ProjectBaseRepository
    {

        public CO_WH_ProjectBaseRepository(Commons.IDbContext.IDbContextCoreCustom context) : base(context)
        {
            _dbContext = context;
            base.primaryKey = "FID";
        }


        public override async Task<long> InsertAsync(CO_WH_ProjectBase entity, IDbTransaction trans = null)
        {
            if (entity is null or { FProjectNO: "" })
                return -1;

            var tmpEntity = await GetProjectBaseEntityByProjectNO(entity.FProjectNO);
            if (tmpEntity is not null)
                return -2;
            if (entity.FCountByCase <= 0)
                return -3;
            if (entity.FTotalWeight <= 0)
                return -4;
            if (entity.FWeightByPallet <= 0)
                return -5;
            return await base.InsertAsync(entity, trans);
        }

        public override Task<bool> UpdateAsync(CO_WH_ProjectBase entity, string primaryKey, IDbTransaction trans = null)
        {
            return base.UpdateAsync(entity, primaryKey, trans);
        }

        public async Task<CO_WH_ProjectBase> GetProjectBaseEntityByProjectNO(string projectNo)
        {
            return await DapperConn.QueryFirstOrDefaultAsync<CO_WH_ProjectBase>($"SELECT * FROM dbo.CO_WH_ProjectBase WHERE FProjectNO = '{projectNo}'", null, null, I_DBTimeout, null);
        }
    }
}
