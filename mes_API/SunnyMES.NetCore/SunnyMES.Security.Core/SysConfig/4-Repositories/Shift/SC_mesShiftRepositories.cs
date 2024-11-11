
using Dapper;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security.SysConfig.Models.Shift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig.IRepositories.Shift
{
    public class SC_mesShiftRepositories : BaseCustomRepository<SC_mesShift, int>, ISC_mesShiftRepositories
    {
        public SC_mesShiftRepositories(IDbContextCoreCustom dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckExistAsync(SC_mesShift mainDto)
        {
            string sql = $@"SELECT * FROM dbo.mesShift WHERE ShiftCode = '{mainDto.ShiftCode}' AND ShiftType = '{mainDto.ShiftType}'";
            var r = await DapperConn.QueryFirstOrDefaultAsync<SC_mesShift>(sql, null, null, I_DBTimeout, null);
            return r is null;
        }

        public async Task<bool> CloneDataAsync(SC_mesShift mainDto, IEnumerable<SC_mesShiftDetail> childDtos)
        {
            bool result = false;
            int MainID = mainDto.ID;
            //var beforeT = await GetSingleOrDefaultAsync(x => x.ID == MainID);
            var beforeT = await GetAsync(mainDto.ID);
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    mainDto.ID = 0;
                    var r1 = _dbContext.Add(mainDto);

                    childDtos.ForEach(d =>
                    {
                        d.ID = 0;
                        d.ShiftCodeID = mainDto.ID;
                        _dbContext.Add<SC_mesShiftDetail>(d);
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

        public async Task<bool> DeleteDataAsync(SC_mesShift inputDto, IEnumerable<SC_mesShiftDetail> childDtos)
        {
            bool result = false;
            using (var transaction = DbContext.GetDatabase().BeginTransaction())
            {
                try
                {
                    _dbContext.Delete<SC_mesShift, int>(inputDto.ID);
                    childDtos.ForEach(d =>
                    {
                        _dbContext.Delete<SC_mesShiftDetail, int>(d.ID);
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
