using API_MSG;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Page;
using SunnyMES.Commons.Services;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.SysConfig.Dtos.Shift;
using SunnyMES.Security.SysConfig.IRepositories.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.Shift
{
    public class SC_mesShiftUPHServices : BaseCustomService<SC_mesShiftUPH, SC_mesShiftUPH,int>, ISC_mesShiftUPHServices
    {
        MSG_Public msgPublic;
        public SC_mesShiftUPHServices(ISC_mesShiftUPHRepositories shiftUPHRepositories): base(shiftUPHRepositories) 
        {
            msgPublic = new MSG_Public(commonHeader.Language);
        }

        public async Task<string> CheckLineShift(List<int> lineIds, List<int> shiftIds)
        {
            string result = "0";
            if (!lineIds.Any() || !shiftIds.Any())
                return result = msgPublic.MSG_Public_6060;

            try
            {
                string sql = $@"DECLARE @tmpLines TABLE(
                                id INT,
                                value VARCHAR(max)
                                )
                                INSERT @tmpLines(id, value)
                                SELECT *  FROM dbo.F_Split('{string.Join(",", lineIds)}',',')

                                IF EXISTS(SELECT *
                                FROM @tmpLines a
                                WHERE NOT EXISTS(SELECT * FROM dbo.mesLine b WHERE b.ID = CAST(a.value AS INT)))
                                BEGIN
                                    SELECT 'line id no exists.' strOutput
	                                RETURN
                                END

                                DELETE @tmpLines
                                INSERT @tmpLines(id, value)
                                SELECT *  FROM dbo.F_Split('{string.Join(",", shiftIds)}',',')
                                IF EXISTS(SELECT *
                                FROM @tmpLines a
                                WHERE NOT EXISTS(SELECT * FROM dbo.mesShift b WHERE b.ID = CAST(a.value AS INT)))
                                BEGIN
                                    SELECT 'shift id no exists.' strOutput
	                                RETURN
                                END
                                SELECT '1' strOutput
                                ";

                var r = await SqlSugarHelper.Db.Ado.SqlQueryAsync<SqlOutputStr>(sql);

                result = (r != null && r.Count > 0) ? r[0].strOutput : "check failed.";
            }
            catch (Exception e)
            {
                result = e.Message;
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            }
            return result;
        }

        public async Task<string> InsertBulkAsync(MesShiftUPHInputDto tinfo)
        {
            string result = "0";
            try
            {
                string tmpR = await CheckLineShift(tinfo.LineIds, tinfo.ShiftIds);

                if (tmpR != "1")
                    return result = tmpR;

                //拆分日期
                DateTime tmpDateTime = tinfo.StartTime.Date;
                List<DateTime> tmpDataList = new List<DateTime>();
                do
                {
                    if (!tmpDataList.Contains(tmpDateTime))
                        tmpDataList.Add(tmpDateTime);
                    tmpDateTime = tmpDateTime.AddDays(1);
                } while (tmpDateTime <= tinfo.EndTime.Date);

                //查询是否存在相同日期，相同班次，相同线别
                string insertSql = string.Empty;
                for (int i = 0; i < tmpDataList.Count; i++)
                {
                    DateTime currentDate = tmpDataList[i];
                    for (int j = 0; j < tinfo.LineIds.Count; j++)
                    {
                        int currentLineId = tinfo.LineIds[j];
                        for (int k = 0; k < tinfo.ShiftIds.Count; k++)
                        {
                            int currentShiftId = tinfo.ShiftIds[k];
                            //是否存在已存在
                            string sql = $@"SELECT *
                                    FROM dbo.mesShiftUPH 
                                    WHERE ShiftDate = '{currentDate}' AND LineID = {currentLineId} AND ShiftID = {currentShiftId}";
                            var tmpShiftUph = await SqlSugarHelper.Db.Ado.SqlQueryAsync<SC_mesShiftUPH>(sql);
                            if (tmpShiftUph.Any(x => x.ID > 0))
                            {
                                result = $"{msgPublic.MSG_Public_6059}, {currentDate}, lineID: {currentLineId}, ShiftID: {currentShiftId} ";
                                return result;
                            }
                            insertSql += $@"INSERT INTO dbo.mesShiftUPH( ShiftID, LineID, UPH, YieldTarget, CreateTime, UpdateTime, State, ShiftDate)
                                        VALUES(
                                        {currentShiftId}   , -- ShiftID - int
                                        {currentLineId}   , -- LineID - int
                                        {tinfo.UPH}   , -- UPH - int
                                        {tinfo.YieldTarget} , -- YieldTarget - float
                                        GETDATE(), -- CreateTime - datetime
                                        GETDATE(), -- UpdateTime - datetime
                                        1, -- State - bit
                                        '{currentDate}'
                                            )
                                        ";
                        }
                    }
                }
                var count = await SqlSugarHelper.Db.Ado.ExecuteCommandAsync(insertSql);
                result = count > 0 ? "1" : "insert failed.";
            }
            catch (Exception e)
            {
                result = e.Message;
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            }
            return result;
        }

        public override async Task<bool> UpdateAsync(SC_mesShiftUPH entity, int id, IDbTransaction trans = null)
        {
            string sql = $"UPDATE dbo.mesShiftUPH SET	ShiftDate = '{entity.ShiftDate}', ShiftID = {entity.ShiftID},LineID = {entity.LineID}, UPH = {entity.UPH}, YieldTarget = {entity.YieldTarget}, UpdateTime = GETDATE(),[State] = {entity.State} WHERE ID = {entity.ID}";
            return await SqlSugarHelper.Db.Ado.ExecuteCommandAsync(sql) > 0;
        }

        public override async Task<IEnumerable<SC_mesShiftUPH>> FindWithAllPagerAsync(PageCustomInfo pageCustomInfo)
        {
            string sql = $@"SELECT a.*,b.ShiftType, b.ShiftCode,b.ShiftDesc,c.Description LineName
                            FROM dbo.mesShiftUPH a
                            JOIN dbo.mesShift b ON b.ID = a.ShiftID
                            JOIN dbo.mesLine c ON c.ID = a.LineID
                            ";
            if (pageCustomInfo.IsEnabled == 1 || pageCustomInfo.IsEnabled == -1)
            {
                sql += $"AND a.State =  {(pageCustomInfo.IsEnabled == 1 ? "1" : "0")}  ";
            }

            if (!string.IsNullOrEmpty(pageCustomInfo.Sortfield))
            {
                sql += $"ORDER BY a.{pageCustomInfo.Sortfield} {(pageCustomInfo.IsAsc ? "ASC" : "DESC")}  ";
            }
            return await SqlSugarHelper.Db.Ado.SqlQueryAsync<SC_mesShiftUPH>(sql);
        }

        public async Task<string> DeleteBulkAsync(MesShiftUPHBulkDeleteDto tinfo)
        {
            string sql = $@"delete from  dbo.mesShiftUPH WHERE ShiftDate >= '{tinfo.StartTime.ToString("yyyy-MM-dd")}' AND ShiftDate <= '{tinfo.StartTime.ToString("yyyy-MM-dd")}'
                            ";
            var res = await SqlSugarHelper.Db.Ado.ExecuteCommandAsync(sql);
             return  res <= 0 ? "delete failed.":"1";
        }
    }
}
