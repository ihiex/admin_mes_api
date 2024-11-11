using Dapper;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Linq;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.SysConfig._1_Models.SN;
using SunnyMES.Security.SysConfig._2_Dtos.SN;
using SunnyMES.Security.SysConfig._3_IRepositories.SN;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.Models.Part;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._4_Repositories.SN
{
    public class SC_SNLockRepository : BaseRepositoryReport<string>, ISC_SNLockRepository
    {
        public SC_SNLockRepository(IDbContextCore dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<PageResult<SC_SN>> FindWithPagerSearchAsync(SearchSignalInputDto search)
        {
            throw new NotImplementedException();
        }

        public async Task<SC_OperateState> UploadSNLock(SC_Lock_SN_Dto scLockSnDto)
        {
            string result = string.Empty;
            string sql = string.Empty;


            sql = $@"
                    IF OBJECT_ID('mesUnitLock') IS NULL
                    BEGIN
                        PRINT 'null'
	                    CREATE TABLE [dbo].[mesUnitLock](
		                    [ID] [int] NOT NULL,
		                    [UnitStateID] [int] NOT NULL,
		                    [StatusID] [tinyint] NOT NULL,
		                    [StationID] [int] NOT NULL,
		                    [EmployeeID] [int] NOT NULL,
		                    [CreationTime] [datetime] NOT NULL,
		                    [LastUpdate] [datetime] NULL,
		                    [PanelID] [int] NULL,
		                    [LineID] [int] NULL,
		                    [ProductionOrderID] [int] NULL,
		                    [RMAID] [int] NULL,
		                    [PartID] [int] NULL,
		                    [LooperCount] [int] NULL,
	                        [LockMess] [nvarchar](max) NULL,
                            [InsertTime] [datetime] NULL,
	                     CONSTRAINT [UnitLock_PK] PRIMARY KEY CLUSTERED 
	                    (
		                    [ID] ASC
	                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	                    ) ON [PRIMARY]
                        ALTER TABLE [dbo].[mesUnitLock] ADD  CONSTRAINT [DF_mesUnitLock_InsertTime]  DEFAULT (getdate()) FOR [InsertTime]
                    END

                    IF OBJECT_ID('mesUnitLockHistory') IS NULL
                    BEGIN
	                    CREATE TABLE [dbo].[mesUnitLockHistory](
		                    [FID] [int] IDENTITY(1,1) NOT NULL,
		                    [ID] [int] NOT NULL,
		                    [UnitStateID] [int] NOT NULL,
		                    [StatusID] [tinyint] NOT NULL,
		                    [StationID] [int] NOT NULL,
		                    [EmployeeID] [int] NOT NULL,
		                    [CreationTime] [datetime] NOT NULL,
		                    [LastUpdate] [datetime] NULL,
		                    [PanelID] [int] NULL,
		                    [LineID] [int] NULL,
		                    [ProductionOrderID] [int] NULL,
		                    [RMAID] [int] NULL,
		                    [PartID] [int] NULL,
		                    [LooperCount] [int] NULL,
		                    [LockTime] [datetime] NULL,
		                    [IsLock] [int] NULL,
		                    [IsUpdate] [int] NULL,
	                        [LockMess] [nvarchar](max) NULL,
	                     CONSTRAINT [PK_mesUnitLockHistory] PRIMARY KEY CLUSTERED 
	                    (
		                    [FID] ASC
	                    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	                    ) ON [PRIMARY]
                    END";
            await DapperConn.ExecuteAsync(sql);
            
            string SNS = string.Join(',', scLockSnDto.SNs);
            if(scLockSnDto.isLock)
            {
                sql = $@"
                DECLARE @tmpSNs VARCHAR(max) = '{SNS}',@sourceSNCount INT = {scLockSnDto.SNs.Length},@targetSNCount INT, @lockMess NVARCHAR(MAX) = '{scLockSnDto.lockMess}'
                BEGIN TRY
                    SELECT id,REPLACE(REPLACE(REPLACE(value,CHAR(13),''),CHAR(10),''),CHAR(32),'') value
                    INTO #tmpSNTabs
                    FROM dbo.F_Split(@tmpSNs,',')

                    SELECT @targetSNCount = COUNT(*) FROM #tmpSNTabs
                    IF @targetSNCount <> @sourceSNCount
                    BEGIN
                        SELECT 'SN count too large' strOutput
                        RETURN
                    END

                    SELECT a.*
                    INTO #tmpUnits
                    FROM dbo.mesUnit a
                    JOIN dbo.mesSerialNumber b ON b.UnitID = a.ID
                    JOIN #tmpSNTabs c ON c.value = b.Value
                    WHERE a.UnitStateID <> -9999 AND a.StatusID <> 4

	                SELECT a.*
	                INTO #tmpNoUpdateSN
	                FROM #tmpSNTabs a
	                LEFT JOIN dbo.mesSerialNumber b ON b.Value = a.value
	                LEFT JOIN dbo.mesUnit c ON c.ID = b.UnitID
	                WHERE c.UnitStateID IS NULL OR c.UnitStateID = -9999

                    SELECT ID
                    INTO #tmpInsertUnitIDs
                    FROM #tmpUnits a
                    EXCEPT
                    SELECT ID
                    FROM dbo.mesUnitLock

                    SELECT ID
                    INTO #tmpUpdateUnitIDs
                    FROM #tmpUnits a
                    INTERSECT
                    SELECT ID
                    FROM dbo.mesUnitLock


                    INSERT INTO dbo.mesUnitLock(ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount,LockMess)
                    SELECT a.ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount,@lockMess FROM #tmpUnits a
                    JOIN #tmpInsertUnitIDs b ON b.ID = a.ID

                    INSERT INTO dbo.mesUnitLockHistory(ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount, LockTime, IsLock, IsUpdate,LockMess)
                    SELECT a.ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount,GETDATE(),1,0,@lockMess FROM #tmpUnits a
                    JOIN #tmpInsertUnitIDs b ON b.ID = a.ID


                    UPDATE b SET b.UnitStateID = a.UnitStateID, b.StatusID = a.StatusID,b.StationID = a.StationID, b.LineID = a.LineID, b.LastUpdate = a.LastUpdate, b.LockMess = @lockMess
                    FROM #tmpUnits a
                    JOIN #tmpUpdateUnitIDs c ON c.ID = a.ID
                    JOIN dbo.mesUnitLock b ON b.ID = c.ID

                    INSERT INTO dbo.mesUnitLockHistory(ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount, LockTime, IsLock, IsUpdate,LockMess)
                    SELECT a.ID,a.UnitStateID,a.StatusID,a.StationID,a.EmployeeID,a.CreationTime,a.LastUpdate,a.PanelID,a.LineID,a.ProductionOrderID,a.RMAID,a.PartID,a.LooperCount,GETDATE(),1,1,@lockMess
                    FROM #tmpUnits a
                    JOIN #tmpUpdateUnitIDs c ON c.ID = a.ID
                    JOIN dbo.mesUnitLock b ON b.ID = c.ID


                    UPDATE a SET a.UnitStateID = -9999,a.StatusID = 4, a.LastUpdate = GETDATE()
                    FROM dbo.mesUnit a
                    JOIN #tmpUnits b ON b.ID = a.ID

                    IF EXISTS(SELECT * FROM #tmpNoUpdateSN)
	                BEGIN
		                SELECT '2' strOutput
		                SELECT value SN FROM #tmpNoUpdateSN
	                END
	                ELSE
	                BEGIN
	                    SELECT '1' strOutput
	                END                   
	            END TRY
	            BEGIN CATCH
		            SELECT ERROR_MESSAGE() strOutput
	            END CATCH
                ";
            }
            else
            {
                sql = $@"DECLARE @tmpSNs VARCHAR(max) = '{SNS}',@sourceSNCount INT = {scLockSnDto.SNs.Length},@targetSNCount INT, @lockMess NVARCHAR(MAX) = '{scLockSnDto.user ?? ""}'
                         BEGIN TRY
                            SELECT id,REPLACE(REPLACE(REPLACE(value,CHAR(13),''),CHAR(10),''),CHAR(32),'') value
                            INTO #tmpSNTabs
                            FROM dbo.F_Split(@tmpSNs,',')

                            SELECT @targetSNCount = COUNT(*) FROM #tmpSNTabs
                            IF @targetSNCount <> @sourceSNCount
                            BEGIN
                                SELECT 'SN count too large' strOutput
                                RETURN
                            END

                            SELECT d.*
                            INTO #tmpUnlocks
                            FROM #tmpSNTabs a
                            JOIN dbo.mesSerialNumber b ON b.Value = a.value
                            JOIN dbo.mesUnit c ON c.ID = b.UnitID
                            JOIN dbo.mesUnitLock d ON d.ID = c.ID
                            WHERE c.UnitStateID = -9999 AND c.StatusID = 4

							SELECT a.*
							INTO #tmpNoUpdateSN
							FROM #tmpSNTabs a
							LEFT JOIN dbo.mesSerialNumber b ON b.Value = a.value
							LEFT JOIN dbo.mesUnit c ON c.ID = b.UnitID
							WHERE c.UnitStateID IS NULL OR c.UnitStateID <> -9999

                            INSERT INTO dbo.mesUnitLockHistory(ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount, LockTime, IsLock, IsUpdate,LockMess)
                            SELECT ID, UnitStateID, StatusID, StationID, EmployeeID, CreationTime, LastUpdate, PanelID, LineID, ProductionOrderID, RMAID, PartID, LooperCount,GETDATE(),0,2,@lockMess FROM #tmpUnlocks

                            UPDATE b SET b.UnitStateID = a.UnitStateID,b.StatusID = a.StatusID, b.LastUpdate = GETDATE()
                            FROM #tmpUnlocks a
                            JOIN dbo.mesUnit b ON b.ID = a.ID                            
                            
                            DELETE mesUnitLock WHERE ID IN (SELECT ID FROM #tmpUnlocks)

                            IF EXISTS(SELECT * FROM #tmpNoUpdateSN)
	                        BEGIN
		                        SELECT '2' strOutput
		                        SELECT value SN FROM #tmpNoUpdateSN
	                        END
	                        ELSE
	                        BEGIN
	                            SELECT '1' strOutput
	                        END
                        END TRY
	                    BEGIN CATCH
		                    SELECT ERROR_MESSAGE() strOutput
	                    END CATCH
                        ";
            }

            using (var outputSet = await DapperConn.QueryMultipleAsync(sql, null, null, I_DBTimeout, null))
            {
                SC_OperateState osp = new SC_OperateState();
                var outputStr = await outputSet.ReadAsync<SqlOutputStr>();
                string outputCode = outputStr.ToList()[0].strOutput;
                osp.output = outputCode;
                if (outputCode == "1")
                {
                    osp.result = true;
                }
                else if(outputCode == "2")
                {
                    osp.result = false;
                    var noUpdateSN = await outputSet.ReadAsync<SC_SN>();
                    osp.SNs = noUpdateSN.ToList();
                }
                else
                {
                    osp.result = false;
                    osp.error = outputCode;
                }
                return osp;
            }
        }
        /// <summary>
        /// 分页查询，自行封装sql语句(仅支持sql server)
        /// 非常复杂的查询，可在具体业务模块重写该方法
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="info">分页信息</param>
        /// <param name="fieldToSort">排序字段</param>
        /// <param name="desc">排序方式 true为desc，false为asc</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public  async Task<List<T>> FindWithPagerCustomSqlAsync<T>(string sql, PagerInfo info, string fieldToSort, bool desc, IDbTransaction trans = null)
        {
            PagerCustomHelper pagerHelper = new PagerCustomHelper(sql, this.selectedFields, fieldToSort, info.PageSize, info.CurrentPageIndex <= 0 ? 1 : info.CurrentPageIndex, desc);

            string pageSql = pagerHelper.GetSqlServerCustomComplexSql();
            var reader = await DapperConn.QueryMultipleAsync(pageSql);
            info.RecordCount = reader.ReadFirst<int>();
            List<T> list = new List<T>();
            list = reader.Read<T>().ToList();
            return list;
        }
    }
}
