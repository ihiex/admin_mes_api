using API_MSG;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.Record.Chart;
using NPOI.Util;
using NPOI.XWPF.UserModel;
using Quartz.Impl.Triggers;
using SQLitePCL;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.DataManager;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.DbContextCore;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using static Dapper.SqlMapper;
using static QRCoder.PayloadGenerator.SwissQrCode.Iban;

namespace SunnyMES.Security.Repositories
{
    public class JumpStationRepository : BaseRepositoryReport<string>, IJumpStationRepository
    {
        MSG_Public P_MSG_Public;
        PublicRepository Public_Repository;
        DataCommitRepository DataCommit_Repository;
        SNFormatRepository SNFormat_Repository;

        MSG_Sys P_MSG_Sys;

        string S_Batch_Pattern = "";
        string S_SN_Pattern = "";

        LoginList List_Login = new LoginList();
        IDbContextCore DB_Context;

        public JumpStationRepository()
        {
        }

        public JumpStationRepository(IDbContextCore dbContext) : base(dbContext)
        {
            DB_Context = dbContext;
        }

        public async Task<List<TabVal>> GetConfInfo(int I_Language, int I_LineID, int I_StationID, int I_EmployeeID, string S_CurrentLoginIP)
        {
            List_Login.LanguageID = I_Language;
            List_Login.LineID = I_LineID;
            List_Login.StationID = I_StationID;
            List_Login.EmployeeID = I_EmployeeID;
            List_Login.CurrentLoginIP = S_CurrentLoginIP;

            PublicRepository v_Public_Repository = new PublicRepository();
            List<TabVal> List_Station = await v_Public_Repository.MesGetStationNoTask(I_StationID.ToString(), "", "");
            List_Login.StationTypeID = List_Station[0].Valint1;
            List_Login.Station = List_Station[0].ValStr1;

            List<TabVal> List_StationType = await v_Public_Repository.MesGetStationTypeNoTask(List_Login.StationTypeID.ToString());
            List_Login.StationType = List_StationType[0].ValStr1;

            List<TabVal> List_StationConfig = await v_Public_Repository.GetStationConfigSetting("", List_Login.StationID.ToString());
            List<TabVal> List_StationTypeDetail = await v_Public_Repository.GetStationTypeDetail(List_Login.StationTypeID.ToString());

            string S_TTBoxSN_Pattern = "[\\s\\S]*";
            foreach (var item in List_StationTypeDetail)
            {
                if (item.ValStr2 != null && item.ValStr2.Trim() == "TTBoxSN_Pattern")
                {
                    S_TTBoxSN_Pattern = item.ValStr1.Trim();

                    foreach (var item_StationConfig in List_StationConfig)
                    {
                        if (item_StationConfig.ValStr1 == "TTBoxSN_Pattern")
                        {
                            S_TTBoxSN_Pattern = item_StationConfig.ValStr2.Trim();
                            continue;
                        }
                    }
                }
            }
            List_Login.TTBoxSN_Pattern = S_TTBoxSN_Pattern;

            string S_IsTTBoxUnpack = "";
            foreach (var item in List_StationTypeDetail)
            {
                if (item.ValStr2 != null && item.ValStr2.Trim() == "IsTTBoxUnpack")
                {
                    S_IsTTBoxUnpack = item.ValStr1.Trim();

                    foreach (var item_StationConfig in List_StationConfig)
                    {
                        if (item_StationConfig.ValStr1 == "IsTTBoxUnpack")
                        {
                            S_IsTTBoxUnpack = item_StationConfig.ValStr2.Trim();
                            continue;
                        }
                    }
                }
            }
            List_Login.IsTTBoxUnpack = S_IsTTBoxUnpack;

            foreach (var item_StationConfig in List_StationConfig)
            {
                if (item_StationConfig.ValStr1 == "PrintIPPort")
                {
                    List_Login.PrintIPPort = item_StationConfig.ValStr2.Trim();
                    continue;
                }
            }


            if (P_MSG_Public == null)
            {
                P_MSG_Public = new MSG_Public(I_Language);
            }
            if (P_MSG_Sys == null)
            {
                P_MSG_Sys = new MSG_Sys(I_Language);
            }


            if (Public_Repository == null)
            {
                Public_Repository = new PublicRepository(DB_Context, I_Language);
            }

            if (DataCommit_Repository == null)
            {
                DataCommit_Repository = new DataCommitRepository(DB_Context, I_Language);
            }

            if (SNFormat_Repository == null)
            {
                SNFormat_Repository = new SNFormatRepository();
            }

            string S_Sql = "select 'ok' as ValStr1,'' ValStr2,'1' ValStr3";
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<IEnumerable<dynamic>> GetPageInitialize(string S_URL)
        {
            IEnumerable<dynamic> List_Result = null;
            List<IsCheckPOPN> List_IsCheckPOPN = await Public_Repository.GetPageInitialize(List_Login, S_URL);
            if (List_IsCheckPOPN.Count > 0)
            {
                if (List_IsCheckPOPN[0].TabVal == null)
                {
                    S_IsCheckPO = List_IsCheckPOPN[0].IsCheckPO;
                    S_IsCheckPN = List_IsCheckPOPN[0].IsCheckPN;
                    S_ApplicationType = List_IsCheckPOPN[0].ApplicationType;
                    S_IsLegalPage = List_IsCheckPOPN[0].IsLegalPage;
                    S_TTScanType = List_IsCheckPOPN[0].TTScanType;
                    S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                    S_TTBoxType = List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO = List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                    S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                    S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                    S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                    S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;


                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" + S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                            "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                            S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                            S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                            S_JumpStatusID + "' as JumpStatusID,'" +
                            S_JumpUnitStateID + "' as JumpUnitStateID,'" +
                            List_Login.PrintIPPort + "' as PrintIPPort, '" + S_IsPrint + "' as IsPrint";
                    if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                    {

                    }

                    var v_Query = await DapperConnRead2.QueryAsync<dynamic>(S_Sql, null, null, I_DBTimeout, null);
                    List_Result = v_Query;
                }
            }
            return List_Result;
        }

        public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {
            List<dynamic> List_Result = new List<dynamic>();
            //初始化数据
            IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

            ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);

            List_SetConfirmPO.JumpFromUnitStateID = S_JumpFromUnitStateID;
            List_SetConfirmPO.JumpToUnitStateID = S_JumpToUnitStateID;
            List_SetConfirmPO.JumpStatusID = S_JumpStatusID;
            List_SetConfirmPO.JumpUnitStateID = S_JumpUnitStateID;

            return List_SetConfirmPO;
        }
    
        public async Task<SetScanSNOutputDto> SetScanSN_JumpStation(string S_SN, string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_DefectID, string S_URL)
        {
            S_SN = S_SN ?? "";
            S_PartFamilyTypeID = S_PartFamilyTypeID ?? "";
            S_PartFamilyID = S_PartFamilyID ?? "";
            S_PartID = S_PartID ?? "";
            S_POID = S_POID ?? "";
            S_UnitStatus = S_UnitStatus ?? "";
            S_DefectID = S_DefectID ?? "";

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();
            List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            SetScanSNOutputDto F_SetScanSNOutputDto = new SetScanSNOutputDto();
            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;

            try
            {
                //初始化
                IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);
                //获取验证信息
                List<TabVal> List_Result = new List<TabVal>();
                if (S_SN == "")
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Public.MSG_Public_016, "0", List_Login, S_SN);//条码不能为空.                
                    TabVal_MSGERROR = List_Result[0];

                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                if (S_JumpUnitStateID != S_UnitStatus)
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_60600, "0", List_Login, S_SN);
                    TabVal_MSGERROR = List_Result[0];

                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                List<mesUnitSerialNumber> List_Unit = await Public_Repository.GetmesUnitSerialNumber(S_SN);
                if (List_Unit.Count < 1)
                {
                    List<TabVal> List_SN = await Public_Repository.MesGetBatchIDByBarcodeSN(S_SN);
                    if (List_SN.Count < 1)
                    {
                        List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Public.MSG_Public_027, "0", List_Login, S_SN);
                        TabVal_MSGERROR = List_Result[0];

                        F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                        return F_SetScanSNOutputDto;
                    }
                    string S_UnitID = List_SN[0].ValStr2;
                    List<mesSerialNumber> List_mesSerialNumber = await Public_Repository.GetmesSerialNumber(S_UnitID, "");
                    S_SN = List_mesSerialNumber[0].Value;
                    List_Unit = await Public_Repository.GetmesUnitSerialNumber(S_SN);
                }

                int I_UnitStateID = List_Unit[0].UnitStateID;
                int I_StatusID = List_Unit[0].StatusID;
                int I_ProductionOrderID = List_Unit[0].ProductionOrderID;

                if (I_ProductionOrderID.ToString() != S_POID)
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_20050, "0", List_Login, S_SN);
                    TabVal_MSGERROR = List_Result[0];

                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                if (!S_JumpFromUnitStateID.Contains(I_UnitStateID.ToString()))
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_20201, "0", List_Login, S_SN);
                    TabVal_MSGERROR = List_Result[0];

                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }

                if (I_StatusID.ToString() != S_JumpStatusID)
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_20045, "0", List_Login, S_SN);
                    TabVal_MSGERROR = List_Result[0];

                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }


                string xmlExtraData = "UnitID=" + List_Unit[0].ID + "&UnitStateID=" + S_JumpToUnitStateID
                                     + "&EmployeeID=" + List_Login.EmployeeID + "&StatusID=" + S_JumpUnitStateID;

                string xmlUnitDefect = "";
                string[] Array_Defect = S_DefectID.Split(',');
                if (S_UnitStatus != "1")
                {
                    int i = 1;
                    foreach (var item in Array_Defect)
                    {
                        try
                        {
                            if (item.Trim() != "")
                            {
                                int I_DefectID = Convert.ToInt32(item);

                                xmlUnitDefect = xmlUnitDefect + "&DefectID" + i + "=" + I_DefectID + "";
                                i++;
                            }
                        }
                        catch (Exception ex)
                        {
                            TabVal_MSGERROR = Public_Repository.GetERROR(ex.ToString(), "0", List_Login, "");
                            F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                            return F_SetScanSNOutputDto;
                        }
                    }
                }

                xmlExtraData = xmlExtraData + xmlUnitDefect;

                List<TabVal> List_JumpSave = await Public_Repository.List_ExecProc("uspJumpStation_Save",
                        S_SN, I_ProductionOrderID.ToString(), S_PartID, xmlExtraData, null, List_Login);
                string S_JumpSave = List_JumpSave[0].ValStr1;

                if (S_JumpSave != "1")
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(List_JumpSave[0]?.ValStr2, "0", List_Login, "");
                    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                    return F_SetScanSNOutputDto;
                }
                //string S_JumpSave = await SetJumpStation(S_PartFamilyID, S_PartID, S_POID, xmlExtraData);                
                //if (S_JumpSave != "1")
                //{
                //    TabVal_MSGERROR = Public_Repository.GetERROR(S_JumpSave, "0", List_Login, "");
                //    F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                //    return F_SetScanSNOutputDto;
                //}

                List<dynamic> List_ProductionOrderQTY = await
                    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                F_SetScanSNOutputDto.ProductionOrderQTY = List_ProductionOrderQTY;
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, "");
                F_SetScanSNOutputDto.MSG = TabVal_MSGERROR;
                return F_SetScanSNOutputDto;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);
            return F_SetScanSNOutputDto;
        }

        private async Task<string> SetJumpStation(string S_PartFamilyID,
            string S_PartID, string S_ProductionOrderID, string S_xmlExtraData)
        {
            string S_Result = "1";

            try
            {
                string S_Sql =
                    @"
                declare @idoc     int,  
		                @partId			  INT = '" + S_PartID + @"',  
		                @unitId           int,  
		                @stationId    INT = '" + List_Login.StationID + @"',    
		                @StationTypeID    int='" + List_Login.StationTypeID + @"',    
		                @ProdOrderID      int='" + S_ProductionOrderID + @"',    
		                @UnitStateID      int,    
		                @EmployeeID       varchar(40)='" + List_Login.EmployeeID + @"',  
		                @StatusID         int,
		                @xmlExtraData     varchar(1000)='" + S_xmlExtraData + @"' ,
                   @strOutput        nvarchar(200) 
                SET @strOutput='1'  
  
 
  
  
                --EXEC sp_xml_preparedocument @idoc output, @xmlStation      
                --SELECT @StationID=StationID      
                --FROM   OPENXML (@idoc,   '/Station',2)      
                --WITH (StationID  int '@StationID')         
                --EXEC sp_xml_removedocument @idoc      
  
                --EXEC sp_xml_preparedocument @idoc output, @xmlProdOrder      
                --SELECT @ProdOrderID=ProdOrderID      
                --FROM   OPENXML (@idoc,   '/ProdOrder',2)      
                --WITH (ProdOrderID  int '@ProdOrderID')         
                --EXEC sp_xml_removedocument @idoc      
  
                --EXEC sp_xml_preparedocument @idoc output, @xmlPart      
                --SELECT @partId=PartID      
                --FROM   OPENXML (@idoc,   '/Part',2)      
                --WITH (PartID  int '@PartID')         
                --EXEC sp_xml_removedocument @idoc      
  
                --declare @xmlExtraData varchar(400)  
                --set @xmlExtraData='UnitID=3674615&UnitStateID=105&StatusID=1&EmployeeID=1&DefectID1=6283&DefectID2=6285'  
  
                select @unitId=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='UnitID'  
                select @UnitStateID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='UnitStateID'  
                select @EmployeeID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='EmployeeID'  
                select @StatusID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='StatusID'  

                -- 2022-05-06 工站在要回退的站，在历史记录中找到  StationID 
                declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
                DECLARE @UnitState NVARCHAR(200)
                SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID
                IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID) 
                BEGIN
	                SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
	                SELECT  @strOutput as ValStr1
	                RETURN
                END 

                SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
                FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID
                select @LineId_History=LineID from mesStation where id=@StationId_History

                IF (@ProdOrderID<>@PODID_History or @partId<>@PartID_History)
                BEGIN
	                SET @strOutput='ERROR:ProductionOrder or Product ParNumber mismatch.'
	                SELECT  @strOutput as ValStr1
	                RETURN
                END

                 DECLARE @currentUnitStateId INT

                 SELECT @currentUnitStateId = a.UnitStateID
                 FROM dbo.mesUnit a 
                 WHERE a.ID = @unitId

                ---20231018 TT工站不允许进入跳站程序
                DECLARE @routeID INT,@tmpPartId INT,@tmpPOId INT,@tmpLineId INT, @tmpLastStationTypeID INT, @OldStationTypeType VARCHAR(50)
                SELECT @tmpPartId = PartID,@tmpLineId = LineID,@tmpPOId = ProductionOrderID, @currentUnitStateId = UnitStateID FROM dbo.mesUnit WHERE ID = @unitId


                SELECT @routeID = dbo.ufnRTEGetRouteID(@tmpLineId,@tmpPartId,NULL,@tmpPOId)

                SELECT TOP 1 @tmpLastStationTypeID = StationTypeID FROM dbo.mesUnitOutputState WHERE RouteID = @routeID AND OutputStateID = @currentUnitStateId

                SELECT @OldStationTypeType = A.Content FROM mesStationTypeDetail A
		                INNER JOIN luStationTypeDetailDef B ON A.StationTypeDetailDefID=B.ID AND B.Description='StationTypeType'
		                WHERE A.StationTypeID=@tmpLastStationTypeID

                IF @OldStationTypeType = 'TT'
                BEGIN
	                SET @strOutput='ERROR:Not allow to jump from TT station.'
	                SELECT  @strOutput as ValStr1
                    RETURN
                END

                -------------------------------------------------------------------------------
  
                BEGIN TRAN  --开启事务    
                BEGIN TRY  

                SELECT ROW_NUMBER() OVER (ORDER BY a.EnterTime) AS RowID, a.ID, a.UnitID, a.UnitStateID, a.EmployeeID, a.StationID, a.EnterTime, a.ExitTime, a.ProductionOrderID, a.PartID, a.LooperCount, a.StatusID, b.Description
                INTO #temp1
                FROM dbo.mesHistory a
                     INNER JOIN dbo.mesUnitState b ON a.UnitStateID=b.ID
                WHERE a.UnitID=@unitId
                ORDER BY a.EnterTime

                DECLARE @RowFrom INT, @RowTo INT, @LastRow INT
                SELECT TOP 1 @RowFrom=RowID
                FROM #temp1
                WHERE UnitStateID=@currentUnitStateId
                ORDER BY EnterTime

                SELECT TOP 1 @RowTo=RowID
                FROM #temp1
                WHERE UnitStateID=@UnitStateID
                ORDER BY EnterTime

                IF @RowTo < @RowFrom
                BEGIN
		                SELECT TOP 1 @LastRow = RowID FROM #temp1 WHERE UnitStateID = @UnitStateID  ORDER BY EnterTime DESC

    	                SELECT RowID, ID, UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID, Description
		                INTO #temp3
		                FROM #temp1
		                WHERE RowID > @LastRow

		                UPDATE a
		                SET a.StatusID=0, a.RemovedStationID=@stationId, a.RemovedEmployeeID=@EmployeeID, a.RemovedTime=GETDATE()
		                FROM dbo.mesUnitComponent a
			                 INNER JOIN #temp3 b ON a.UnitID=b.UnitID AND a.InsertedStationID=b.StationID AND b.StatusID=1
		                WHERE a.UnitID=@unitId --650555 
                END
 
                UPDATE mesUnit SET UnitStateID=@UnitStateID,StatusID=@StatusID,StationID=@StationId_History,LineID=@LineId_History,EmployeeID=@EmployeeID,  
                LastUpdate=GETDATE()  WHERE ID=@unitId  
 
 
                 INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)  
                VALUES (@unitId,-400,@EmployeeID,@StationId,GETDATE(),GETDATE(),@ProdOrderID,@partId,1,@StatusID)  

                declare @sqlValue varchar(800)  
                declare @MaxID int  
                declare @DefectID int  
                select identity(int,1,1) as RowID,* into #temp4 from dbo.split(@xmlExtraData,'&') where itemsName like 'DefectID%'  
                declare @j int  
                declare @k int  
                set @j=1  
                select @k=count(1) from #temp4  
                while @j<=@k  
                begin  
                 select @DefectID=itemsValue from #temp4 where RowID=@j  
                 select @MaxID=ISNULL(Max(ID),0)+1  from mesUnitDefect  
                 INSERT INTO mesUnitDefect(ID,UnitID,DefectID,StationID,EmployeeID)  
                 VALUES (@MaxID,@UnitID,@DefectID,@StationId,@EmployeeID)  
                 set @j=@j+1  
                end  
  
                COMMIT   
                END TRY   
                BEGIN CATCH   
	                 ROLLBACK          
	                 SET @strOutput='错误信息:'+ERROR_MESSAGE() 
	                SELECT  @strOutput as ValStr1 
                END CATCH    
                ";

                IEnumerable<TabVal> List_Reult = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                if (List_Reult.Count() > 0)
                {
                    S_Result = List_Reult.First().ValStr1;
                }
            }
            catch (Exception ex)
            {
                S_Result = ex.ToString();
            }
            return S_Result;
        }


        //private async Task<string> SetJumpStation(string S_PartFamilyID,
        //    string S_PartID, string S_ProductionOrderID,string S_xmlExtraData)
        //{
        //    string S_Result = "1";

        //    try
        //    {
        //        string S_Sql =
        //            @"
        //        declare 
        //        @strOutput        varchar(500),
        //        @partId			  int='" + S_PartID + @"',  
        //        @unitId           int,    
        //        @stationId        int='" + List_Login.StationID + @"',      
        //        @StationTypeID    int='" + List_Login.StationTypeID + @"',    
        //        @ProdOrderID      int='" + S_ProductionOrderID + @"',    
        //        @UnitStateID      int,    
        //        @EmployeeID       varchar(40)='" + List_Login.EmployeeID + @"',  
        //        @StatusID         int,
        //        @xmlExtraData     varchar(1000)='" + S_xmlExtraData + @"' 
  
        //        SET @strOutput='1'  
  
        //        BEGIN TRAN  --开启事务    
        //        BEGIN TRY  

        //        select @unitId=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='UnitID'  
        //        select @UnitStateID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='UnitStateID'  
        //        select @EmployeeID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='EmployeeID'  
        //        select @StatusID=itemsValue from dbo.split(@xmlExtraData,'&') where itemsName='StatusID' 

        //        -- 2022-05-06 工站在要回退的站，在历史记录中找到  StationID 
        //        declare @StationId_History    int, @LineId_History int,@PODID_History int, @PartID_History int
        //        DECLARE @UnitState NVARCHAR(200)
        //        SELECT @UnitState=[Description] FROM mesUnitState WHERE ID=@UnitStateID
        //        IF NOT EXISTS (SELECT TOP 1 * FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID) 
        //        BEGIN
	       //         SET @strOutput='ERROR:没有过站记录('+@UnitState+')  不允许回退没有扫描过的站。'
        //            Select  @strOutput as ValStr1
	       //         RETURN
        //        END 

        //        SELECT TOP 1  @StationId_History=StationID,@PODID_History=ProductionOrderID,@PartID_History=PartID 
        //        FROM mesHistory WHERE UnitID= @unitId AND UnitStateID= @UnitStateID ORDER BY ID
        //        select @LineId_History=LineID from mesStation where id=@StationId_History

        //        IF (@ProdOrderID<>@PODID_History or @partId<>@PartID_History)
        //        BEGIN
	       //         SET @strOutput='ERROR:ProductionOrder or Product ParNumber mismatch.'
        //            Select  @strOutput as ValStr1
	       //         RETURN
        //        END

        //        UPDATE mesUnit SET UnitStateID=@UnitStateID,StatusID=@StatusID,StationID=@StationId_History,
        //            LineID=@LineId_History,EmployeeID=@EmployeeID,  
        //            LastUpdate=GETDATE()  WHERE ID=@unitId  
  

        //        select Row_Number() over (order by  EnterTime)as RowID,a.*,b.Description into #temp1 from mesHistory a
        //         inner join mesUnitState b on a.UnitStateID=b.ID where UnitID=@unitId order by EnterTime  
    
        //        declare @RowStart int  
        //        declare @RowEnd int  
        //        SELECT TOP 1 @RowStart = RowID FROM #temp1  where unitstateid=@UnitStateID order by EnterTime desc  
        //        INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)  
        //        VALUES (@unitId,@UnitStateID,@EmployeeID,@StationId,GETDATE(),GETDATE(),@ProdOrderID,@partId,1,@StatusID)  

  
        //        select * into #temp3 from #temp1 where RowID>@RowStart
  

        //        update a set a.StatusID=0,RemovedStationID=@stationId, RemovedEmployeeID=@EmployeeID,RemovedTime=getdate() 
        //        from mesUnitComponent a inner join #temp3 b on a.UnitID=b.UnitID and a.InsertedStationID=b.StationID  and b.StatusID=1  
        //        where a.UnitID=@unitId  --650555  
   
        //        declare @sqlValue varchar(800)  
        //        declare @MaxID int  
        //        declare @DefectID int  
        //        select identity(int,1,1) as RowID,* into #temp4 from dbo.split(@xmlExtraData,'&') where itemsName like 'DefectID%'  
        //        declare @j int  
        //        declare @k int  
        //        set @j=1  
        //        select @k=count(1) from #temp4  
        //        while @j<=@k  
        //        begin  
        //         select @DefectID=itemsValue from #temp4 where RowID=@j  
        //         select @MaxID=ISNULL(Max(ID),0)+1  from mesUnitDefect  
        //         INSERT INTO mesUnitDefect(ID,UnitID,DefectID,StationID,EmployeeID)  
        //         VALUES (@MaxID,@UnitID,@DefectID,@StationId,@EmployeeID)  
        //         set @j=@j+1  
        //        end  
  
        //        COMMIT   
        //        END TRY   
        //        BEGIN CATCH   
        //         ROLLBACK          
        //            SET @strOutput='错误信息:'+ERROR_MESSAGE()  
        //            Select  @strOutput as ValStr1
        //        END CATCH  
        //        ";

        //        IEnumerable<TabVal> List_Reult = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
        //        if (List_Reult.Count() > 0)
        //        {
        //            S_Result = List_Reult.First().ValStr1;
        //        }
        //    }                   
        //    catch (Exception ex)
        //    {
        //        S_Result = ex.ToString();
        //    }
        //    return S_Result;
        //}


    }
}
