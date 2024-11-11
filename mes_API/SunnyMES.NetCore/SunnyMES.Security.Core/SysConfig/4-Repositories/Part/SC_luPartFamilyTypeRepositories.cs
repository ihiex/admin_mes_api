using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Repositories.Part
{
    public class SC_luPartFamilyTypeRepositories : BaseCustomRepository<SC_luPartFamilyType,string>, ISC_luPartFamilyTypeRepositories
    {
        public SC_luPartFamilyTypeRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            base.primaryKey = "ID";
        }

        public async Task<bool> CheckExistAsync(SC_luPartFamilyType mainDto)
        {
            string sql = $@"SELECT * FROM dbo.luPartFamilyType WHERE Name = '{mainDto.Name}' AND Description = '{mainDto.Description}'";
            var r = await DapperConn.QueryFirstOrDefaultAsync<SC_luPartFamilyType>(sql, null, null, I_DBTimeout, null);
            return r is null;
        }

        public async Task<bool> CloneDataAsync(SC_luPartFamilyType mainDto, IEnumerable<SC_mesPartFamilyTypeDetail> childDtos)
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
                        d.PartFamilyTypeID = mainDto.ID;
                        _dbContext.Add<SC_mesPartFamilyTypeDetail>(d);
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

        public async Task<bool> DeleteDataAsync(SC_luPartFamilyType inputDto, IEnumerable<SC_mesPartFamilyTypeDetail> childDtos)
        {
            bool result = false;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    _dbContext.Delete<SC_luPartFamilyType, int>(inputDto.ID);
                    childDtos.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesPartFamilyTypeDetail, int>(d.ID);
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
