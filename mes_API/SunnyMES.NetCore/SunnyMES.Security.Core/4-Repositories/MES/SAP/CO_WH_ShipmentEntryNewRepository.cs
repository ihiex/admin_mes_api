using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.IRepositories.MES.SAP;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.Repositories.MES.SAP
{
    public class CO_WH_ShipmentEntryNewRepository : BaseCustomRepository<CO_WH_ShipmentEntryNew_T, string>, ICO_WH_ShipmentEntryNewRepository
    {

        public CO_WH_ShipmentEntryNewRepository(IDbContextCoreCustom dbContext) : base(dbContext)
        {

            base.primaryKey = "FDetailID";
        }

        public async Task<int> GetMaxFInterID()
        {
            return (await DapperConn.ExecuteScalarAsync("SELECT MAX(FInterID) FROM dbo.CO_WH_ShipmentEntryNew", null, null, I_DBTimeout, null)).ToInt();
        }
        public async Task<int> GetMaxFEntryID(string FInterID)
        {
            return (await DapperConn.ExecuteScalarAsync($"SELECT MAX(FEntryID) FROM dbo.CO_WH_ShipmentEntryNew WHERE FInterID = {FInterID}", null, null, I_DBTimeout, null)).ToInt();
        }

        public override async Task<long> InsertAsync(CO_WH_ShipmentEntryNew_T entity, IDbTransaction trans = null)
        {
            var existId = (await DapperConn.ExecuteScalarAsync($"SELECT 1 FROM dbo.CO_WH_ShipmentEntryNew WHERE FInterID = {entity.FInterID} AND FEntryID = {entity.FEntryID}",null, null, I_DBTimeout, null))?.ToString();

            if (existId == "1") 
            {
                return 0;
            }

            return await base.InsertAsync(entity, trans);
        }

    }
}
