using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Models.MES.SAP;
using SunnyMES.Security.IRepositories.MES.SAP;

namespace SunnyMES.Security.Repositories.MES.SAP
{
    public class TmpExcelShipmentNewRepository : BaseCustomRepository<tmpExcelShipmentNew, string>, ITmpExcelShipmentNewRepository
    {

        //protected string primaryKey = "HAWB";
        public TmpExcelShipmentNewRepository(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            base.primaryKey = "[NO.]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HAWB"></param>
        /// <returns></returns>
        public async  Task<IEnumerable<tmpExcelShipmentNew>> GetListSapByHAWBAsync(string HAWB)
        {
            string sql = $"SELECT * FROM dbo.tmpExcelShipmentNew WHERE HAWB#= @HAWB";
            return await DapperConn.QueryAsync<tmpExcelShipmentNew>(sql, new { @HAWB  = HAWB });
        }

        public override async Task<bool> UpdateAsync(tmpExcelShipmentNew entity, string primaryKey, IDbTransaction trans = null)
        {

            return _dbContext.Edit<tmpExcelShipmentNew>(entity) > 0;
            //string sql = $"UPDATE dbo.tmpExcelShipmentNew SET CartonQTY = CartonQTY,import = import\r\nWHERE [NO.] = {primaryKey}";
            //return (await DapperConn.ExecuteAsync(sql, null, null, I_DBTimeout, null)) > 0;
        }

    }
}
