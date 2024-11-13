using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NPOI.HSSF.Record.PivotTable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_mesPartRepositories : BaseCustomRepository<SC_mesPart, string>, ISC_mesPartRepositories
    {
        public SC_mesPartRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckExistAsync(SC_mesPart mainDto)
        {
            string sql = $@"SELECT * FROM dbo.mesPart WHERE PartNumber = '{mainDto.PartNumber}' AND Description = '{mainDto.Description}' AND PartFamilyID = {mainDto.PartFamilyID}";
            var r = await DapperConn.QueryFirstOrDefaultAsync<SC_mesPart>(sql, null, null, I_DBTimeout, null);
            return r is null;
        }

        public async Task<bool> CloneDataAsync(SC_mesPart mainDto, IEnumerable<SC_mesPartDetail> childDtos)
        {
            bool result = false;
            int MainID = mainDto.ID;
            var beforeT = await GetSingleOrDefaultAsync(x => x.ID == MainID);
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    mainDto.ID = 0;
                    var r1 = _dbContext.Add(mainDto);

                    childDtos.ForEach(d =>
                    {
                        d.ID = 0;
                        d.PartID = mainDto.ID;
                        _dbContext.Add<SC_mesPartDetail>(d);
                    });

                    transaction.Commit();
                    result = true;
                    await base.FormatCloneMsg(beforeT, mainDto);
                }
                catch (Exception e)
                {
                    Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, "", e);
                    transaction.Rollback();
                }
            }
            return result;
        }

        public async Task<bool> DeleteDataAsync(SC_mesPart inputDto, IEnumerable<SC_mesPartDetail> childDtos)
        {
            bool result = false;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    _dbContext.Delete<SC_mesPart, int>(inputDto.ID);
                    childDtos.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesPartDetail, int>(d.ID);
                    });
                    await base.FormatDeleteMsg(inputDto, childDtos.ToList());
                    transaction.Commit(); 
                    result = true;
                }
                catch (Exception e)
                {
                    Log4NetHelper.Error(MethodBase.GetCurrentMethod()?.DeclaringType, "", e);
                    transaction.Rollback();
                }
            }
            return result;
        }
    }
}
