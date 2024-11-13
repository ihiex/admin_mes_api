using NPOI.SS.Formula.Functions;
using SqlSugar;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig._1_Models.SN;
using SunnyMES.Security.SysConfig._2_Dtos.SN;
using SunnyMES.Security.SysConfig._3_IRepositories.SN;
using SunnyMES.Security.SysConfig._5_IServices.SN;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._6_Services.SN
{

    public class SC_SNLockServices : BaseServiceReport<string>, ISC_SNLockServices
    {
        private readonly ISC_SNLockRepository snRepositories;

        public SC_SNLockServices(ISC_SNLockRepository snRepositories) : base(snRepositories)
        {
            this.snRepositories = snRepositories;
        }

        public async Task<PageResult<Sc_Lock_Sn_Search_Dto>> FindWithPagerSearchAsync(SearchSignalInputDto search)
        {
            //return await snRepositories.FindWithPagerSearchAsync(search);

            bool order = search.Order.ToUpper().Trim() == "DESC";

            string sql = $@"
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
            await SqlSugarHelper.Db.Ado.ExecuteCommandAsync(sql);

            string selectStr = $@"SELECT b.Value SN, a.LockMess LockMessage, a.InsertTime LockTime
            FROM dbo.mesUnitLock a
            JOIN dbo.mesSerialNumber b ON a.ID = b.UnitID ";
            if (!string.IsNullOrEmpty(search.Keywords))
            {
                selectStr += $" WHERE b.Value LIKE '%{search.Keywords}%'";
            }

            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };

            var list = await snRepositories.FindWithPagerCustomSqlAsync<Sc_Lock_Sn_Search_Dto>(selectStr, pagerInfo, "LockTime", order);

            PageResult<Sc_Lock_Sn_Search_Dto> pageResult = new PageResult<Sc_Lock_Sn_Search_Dto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = list,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount,

            };
            return pageResult;
        }
        public async Task<SC_OperateState> UploadSNLock(SC_Lock_SN_Dto scLockSnDto)
        {
            return await this.snRepositories.UploadSNLock(scLockSnDto);
        }
    }
}
