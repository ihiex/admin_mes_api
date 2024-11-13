using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_luPartFamilyRepositories : BaseCustomRepository<SC_luPartFamily, string>, ISC_luPartFamilyRepositories
    {
        public SC_luPartFamilyRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckExistAsync(SC_luPartFamily mainDto)
        {
            string sql = $@"SELECT * FROM dbo.luPartFamily WHERE Name = '{mainDto.Name}' AND Description = '{mainDto.Description}'  AND PartFamilyTypeID = {mainDto.PartFamilyTypeID}";
            var r = await DapperConn.QueryFirstOrDefaultAsync<SC_luPartFamily>(sql, null, null, I_DBTimeout, null);
            return r is null;
        }

        public async Task<bool> CloneDataAsync(SC_luPartFamily mainDto, IEnumerable<SC_mesPartFamilyDetail> childDtos)
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
                        d.PartFamilyID = mainDto.ID;
                        _dbContext.Add<SC_mesPartFamilyDetail>(d);
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

        public async Task<bool> DeleteDataAsync(SC_luPartFamily inputDto, IEnumerable<SC_mesPartFamilyDetail> childDtos)
        {
            bool result = false;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    _dbContext.Delete<SC_luPartFamily, int>(inputDto.ID);
                    childDtos.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesPartFamilyDetail, int>(d.ID);
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
