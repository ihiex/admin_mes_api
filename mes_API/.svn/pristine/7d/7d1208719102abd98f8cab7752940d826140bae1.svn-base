#define IsSql
using API_MSG;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using Dapper;
using System;
using System.Data;
using System.Linq;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Log;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES;

namespace SunnyMES.Security.Repositories;

public class PackageOverStationRepository : BaseCommonRepository<string>,IPackageOverStationRepository
{
    MSG_Public P_MSG_Public;
    private MSG_Sys msgSys;

    private LoginList List_Login = new LoginList();
    PublicRepository Public_Repository;

    DataCommitRepository DataCommit_Repository;

    private InitPageInfo mInitPageInfo = new InitPageInfo();

    #region 存储过程参数
    private string xmlProdOrder = string.Empty;
    private string xmlPart = string.Empty;
    private string xmlExtraData = string.Empty;
    private string xmlStation = string.Empty;
    #endregion


    public PackageOverStationRepository(IDbContextCore dbContext):base(dbContext)
    {
        _dbContext = dbContext;
        mInitPageInfo.stationAttribute = new StationAttributes();
        mInitPageInfo.poAttributes = new PoAttributes();

    }
    public  void GetConfInfo(CommonHeader commonHeader)
    {
        baseCommonHeader= commonHeader;
        List_Login.LanguageID = commonHeader.Language;
        List_Login.LineID = commonHeader.LineId;
        List_Login.StationID = commonHeader.StationId;
        List_Login.EmployeeID = commonHeader.EmployeeId;
        List_Login.CurrentLoginIP = commonHeader.CurrentLoginIp;

        P_MSG_Public ??= new MSG_Public(List_Login.LanguageID);
        msgSys ??= new MSG_Sys(List_Login.LanguageID);
        Public_Repository ??= new PublicRepository(_dbContext, List_Login.LanguageID);
        DataCommit_Repository ??= new DataCommitRepository(_dbContext, List_Login.LanguageID);
    }

    public async Task<GetPageInitializeOutput> GetPageInitializeAsync(string S_URL)
    {
        GetPageInitializeOutput pageInitializeOutput = new GetPageInitializeOutput()
        {
            CurrentSettingInfo =
            {
                LineId = baseCommonHeader.LineId,
                LineName = baseCommonHeader.LineName,
                StationId = baseCommonHeader.StationId,
                StationName = baseCommonHeader.StationName,
                CurrentLoginIp = baseCommonHeader.CurrentLoginIp,
                Url = S_URL,
            }
        };


        try
        {
            //获取站点名称
            if (List_Login.StationID != 0 && List_Login.StationID != -1)
            {
                var mmesStations = await Public_Repository.MesGetStationAsync(List_Login.StationID.ToString());
                if (mmesStations?.Count < 1)
                {
                    pageInitializeOutput.ErrorMsg = P_MSG_Public.MSG_Public_6017;
                }
                else
                {
                    List_Login.Station = mmesStations[0].Description;
                    List_Login.StationTypeID = mmesStations[0].StationTypeID;
                }
            }
            else
            {
                pageInitializeOutput.ErrorMsg = P_MSG_Public.MSG_Public_6017;
            }
            var tmpStationInfo = await Public_Repository.GetPageAllStationInfoAsync(List_Login, S_URL);
            pageInitializeOutput.ErrorMsg = tmpStationInfo.Item1;
            pageInitializeOutput.CurrentInitPageInfo = mInitPageInfo = tmpStationInfo.Item2;
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            pageInitializeOutput.ErrorMsg = e.Message;
        }

        return pageInitializeOutput;
    }

    public async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
        string S_UnitStatus, string S_URL)
    {
        SetConfirmPoOutput mConfirmPoOutput = new SetConfirmPoOutput();
        try
        {
            var pageInit = await GetPageInitializeAsync(S_URL);

            UniversalConfirmPoOutput universalConfirmPoOutput =  await Public_Repository.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID,
                S_PartID, S_POID, S_UnitStatus, List_Login);
             mConfirmPoOutput = new SetConfirmPoOutput()
            {
                CurrentInitPageInfo = pageInit.CurrentInitPageInfo,
                CurrentSettingInfo = pageInit.CurrentSettingInfo,
                UniversalConfirmPoOutput =  universalConfirmPoOutput.ErrorMsg == null? universalConfirmPoOutput:null,
                ErrorMsg = universalConfirmPoOutput.ErrorMsg
            };

            mConfirmPoOutput.CurrentSettingInfo.PartFamilyTypeID = S_PartFamilyTypeID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.PartFamilyID = S_PartFamilyID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.PartID = S_PartID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.POID = S_POID.ToInt();
            mConfirmPoOutput.CurrentSettingInfo.UnitStatus = S_UnitStatus.ToInt();

            //存储过程参数
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + S_PartID + "\"> </Part>";
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId + "\"> </ExtraData>";
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";

            ////初始化数据
            mConfirmPoOutput.CurrentInitPageInfo = await Public_Repository.GetAllPagePoInfoAsync(S_POID, mConfirmPoOutput.CurrentInitPageInfo);
        }
        catch (Exception e)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", e);
            mConfirmPoOutput.ErrorMsg = e.Message;
        }
        return mConfirmPoOutput;
    }

    public async Task<PalletSnVerifyOutput> PalletSnVerifyAsync(PackageOverStation_PalletSn_Input palletSnInput)
    {
        var pageInit = await GetPageInitializeAsync(palletSnInput.S_URL);
        PalletSnVerifyOutput mPalletSnVerifyOutput = new PalletSnVerifyOutput();
        string needScanPalletSn = pageInit.CurrentInitPageInfo.stationAttribute.IsScanPalletSNOrCartonBoxSN;
        if (needScanPalletSn != "1" && needScanPalletSn != "2")
        {
            mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6016;
            return mPalletSnVerifyOutput;
        }

        
        string mUnitStateId = await Public_Repository.GetMesUnitState(palletSnInput.S_PartID,
            palletSnInput.S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0, palletSnInput.S_POID,
            palletSnInput.S_UnitStatus);
        //获取整个栈板的数据
        var tmpPalletDatas = await Public_Repository.GetPalletAllData(palletSnInput.S_PalletSN);

        if (pageInit.CurrentInitPageInfo.stationAttribute.IsScanPalletSNOrCartonBoxSN == "2")
        {
            #region 只需要扫描栈板条码就可以将所有产品进行过站
            //只做正常入库
            if (!palletSnInput.IsEntry)
            {
                mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6021;
                return mPalletSnVerifyOutput;
            }
            
            int actualType = await Public_Repository.GetBarcodeType(palletSnInput.S_PalletSN);
            if (actualType.ToString() != pageInit.CurrentInitPageInfo.stationAttribute.IsScanPalletSNOrCartonBoxSN)
            {
                //当输入的不是栈板条码时
                mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6022;
                return mPalletSnVerifyOutput;
            }

            int tmpPartId = 0;
            int tmpPoId = 0;
            string isCheckPn = pageInit.CurrentInitPageInfo.stationAttribute.IsCheckPN;
            string isCheckPo = pageInit.CurrentInitPageInfo.stationAttribute.IsCheckPO;
            
            //检查料号或者工单的前提是在装箱或者装栈板的时候，不能设置为混工单或者混料号
            if (isCheckPn == "1")
            {
                var tmpUnitPartIds = tmpPalletDatas.AsEnumerable().GroupBy(x => x.Field<int>("PartID")).Select(x => x.Key);
                if (tmpUnitPartIds.Count() != 1)
                {
                    mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6023;
                    return mPalletSnVerifyOutput;
                }
                tmpPartId = tmpUnitPartIds.ToList()[0];
                if (tmpPartId.ToString() != palletSnInput.S_PartID)
                {
                    mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6020;
                    return mPalletSnVerifyOutput;
                }

                if (isCheckPo == "1")
                {
                    var tmpUnitPoId = tmpPalletDatas.AsEnumerable().GroupBy(x => x.Field<int>("ProductionOrderID")).Select(x => x.Key);
                    if (tmpUnitPoId.Count() != 1)
                    {
                        mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6024;
                        return mPalletSnVerifyOutput;
                    }
                    tmpPoId = tmpUnitPoId.ToList()[0];
                    if (tmpPoId.ToString() != palletSnInput.S_POID)
                    {
                        mPalletSnVerifyOutput.ErrorMsg = msgSys.MSG_Sys_20050;
                        return mPalletSnVerifyOutput;
                    }
                }
            }

            if (string.IsNullOrEmpty(xmlStation))
                xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";

            if (string.IsNullOrEmpty(xmlExtraData))
                xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + tmpPoId.ToString() + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + tmpPartId.ToString() + "\"> </Part>";
            string strOutputCheck = string.Empty;
#if IsSql
            strOutputCheck = await Public_Repository.uspPackageRouteCheck(palletSnInput.S_PalletSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");
#else
            strOutputCheck = await Public_Repository.uspCallProcedureAsync("uspPackageRouteCheck", palletSnInput.S_PalletSN, xmlProdOrder, xmlPart,
                xmlStation, xmlExtraData, "");
#endif
            if (strOutputCheck != "1")
            {
                mPalletSnVerifyOutput.ErrorMsg = msgSys.GetLanguage(strOutputCheck);
                return mPalletSnVerifyOutput;
            }

            string strOutputData = string.Empty;
#if IsSql
            strOutputData = await DataCommit_Repository.uspSetPackageData(palletSnInput.S_PalletSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");
#else
            strOutputData = await Public_Repository.uspCallProcedureAsync("uspSetPackageData",
                palletSnInput.S_PalletSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");
#endif
            if (strOutputData != "1")
            {
                mPalletSnVerifyOutput.ErrorMsg = msgSys.GetLanguage(strOutputData);
                return mPalletSnVerifyOutput;
            }
            #endregion
        }
        else
        {
            #region 还需要扫描箱码进行过站
            var sortUnitState = tmpPalletDatas.DefaultView.ToTable(true, new[] { "UnitStateID" });
            if (sortUnitState?.Rows.Count > 2)
            {
                //整个栈板存在多于2两种状态
                mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6025;
                return mPalletSnVerifyOutput;
            }

            var sortSnState = tmpPalletDatas.DefaultView.ToTable(true, new[] { "KITSN", "UnitStateID" });
            var errorBoxSn = sortSnState.AsEnumerable().GroupBy(x => new { KITSN = x.Field<string>("KITSN") })
                .Select(y => new { KITSN = y.Key, tCount = y.Count() })
                .Where(y => y.tCount > 1).ToList();

            if (errorBoxSn.Count > 0)
            {
                //箱子中存在多个状态条码
                string errorCode = errorBoxSn.Select(x => new { x.KITSN.KITSN }).Join(",");
                mPalletSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6026 + $",{errorCode}";
                return mPalletSnVerifyOutput;

            }
            tmpPalletDatas.Columns.Add("IsScan", typeof(string), "0");

            var groupSn = sortSnState.AsEnumerable().Select(x => new IsScanedBox { KITSN = x.Field<string>("KITSN"), UnitStateID = x.Field<int>("UnitStateID"), IsScan = x.Field<int>("UnitStateID").ToString() == mUnitStateId ? "1" : "0" }).ToList();

            //cacheHelper.Add(
            //    $"{palletSnInput.S_PalletSN}_{baseCommonHeader.CurrentLoginIp}_{baseCommonHeader.StationId}_States",
            //    groupSn, new TimeSpan(10), new TimeSpan(1, 0, 0, 0));

            mPalletSnVerifyOutput.CurrentInitPageInfo.DataList = groupSn;
            #endregion

        }

        return mPalletSnVerifyOutput;
    }

    public async Task<BoxSnVerifyOutput> BoxSnVerifyAsync(PackageOverStation_BoxSn_Input boxSnInput)
    {
        var pageInit = await GetPageInitializeAsync(boxSnInput.S_URL);
        BoxSnVerifyOutput mBoxSnVerifyOutput = new BoxSnVerifyOutput();

        string mUnitStateId = await Public_Repository.GetMesUnitState(boxSnInput.S_PartID,
            boxSnInput.S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0, boxSnInput.S_POID,
            boxSnInput.S_UnitStatus);

        
        if (pageInit.CurrentInitPageInfo.stationAttribute.IsCheckPN == "1")
        {
            var tmpPalletDatas = await Public_Repository.GetProductDataInfo(boxSnInput.S_BoxSN, 1);
            var tmpUnitPartIds = tmpPalletDatas.AsEnumerable().GroupBy(x => x.Field<int>("PartID")).Select(x => x.Key);
            if (tmpUnitPartIds.Count() != 1)
            {
                mBoxSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6018;
                return mBoxSnVerifyOutput;
            }
            int tmpPartId = tmpUnitPartIds.ToList()[0];

            if (tmpPartId.ToString() != boxSnInput.S_PartID)
            {
                mBoxSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6020;
                return mBoxSnVerifyOutput;
            }

            if (pageInit.CurrentInitPageInfo.stationAttribute.IsCheckPO == "1")
            {
                var tmpUnitPoId = tmpPalletDatas.AsEnumerable().GroupBy(x => x.Field<int>("ProductionOrderID")).Select(x => x.Key);
                if (tmpUnitPoId.Count() != 1)
                {
                    mBoxSnVerifyOutput.ErrorMsg = P_MSG_Public.MSG_Public_6019;
                    return mBoxSnVerifyOutput;
                }
                int tmpPoId = tmpUnitPoId.ToList()[0];
                if (tmpPoId.ToString() != boxSnInput.S_POID)
                {
                    mBoxSnVerifyOutput.ErrorMsg = msgSys.MSG_Sys_20050;
                    return mBoxSnVerifyOutput;
                }
            }
        }

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + boxSnInput.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + boxSnInput.S_PartID + "\"> </Part>";

        string strOutput = string.Empty;
        if (boxSnInput.IsEntry)
        {
            strOutput = await Public_Repository.uspCallProcedureAsync("uspPackageRouteCheck", boxSnInput.S_BoxSN, xmlProdOrder, xmlPart,
                xmlStation, xmlExtraData, "");

            if (strOutput !="1")
            {
                mBoxSnVerifyOutput.ErrorMsg = msgSys.GetLanguage(strOutput);
                return mBoxSnVerifyOutput;
            }

            strOutput = "";
#if IsSql
            strOutput = await DataCommit_Repository.uspSetPackageData(boxSnInput.S_BoxSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");
#else
            strOutput = await Public_Repository.uspCallProcedureAsync("uspSetPackageData", boxSnInput.S_BoxSN, xmlProdOrder, xmlPart,
                            xmlStation, xmlExtraData, "");
#endif
            if (strOutput != "1")
                return (BoxSnVerifyOutput)Public_Repository.SetMsgSys(mBoxSnVerifyOutput, strOutput);
        }

        if (!boxSnInput.IsEntry)
        {
            strOutput = "";
            strOutput = await SetCancelInWHEntry(boxSnInput.S_BoxSN, boxSnInput.S_POID, boxSnInput.S_PartID,
                baseCommonHeader.StationId.ToString(), baseCommonHeader.EmployeeId.ToString(), "", "");

            if (strOutput != "OK")
                return (BoxSnVerifyOutput)Public_Repository.SetMsgSys(mBoxSnVerifyOutput, strOutput);
        }

        return mBoxSnVerifyOutput;
    }
    private async Task<string> SetCancelInWHEntry(string S_BoxSN, string S_ProdID, string S_PartID, string S_StationID, string S_EmployeeID, string S_ReturnToStationTypeID, string S_ReturnStatus)
    {
        string S_Result = "";
        try
        {
            string S_Sql =
            @"
                DECLARE	 @IBoxSN VARCHAR(100) = '" + S_BoxSN + @"',
		        @IProdID			INT='" + S_ProdID + @"',
		        @partID				INT = '" + S_PartID + @"',		
		        @IStationID			INT = '" + S_StationID + @"',
		        @IEmployeeId         INT = '" + S_EmployeeID + @"',
                @IReturnToStationTypeID VARCHAR(10) = '" + S_ReturnToStationTypeID + @"',
                @IStatusId VARCHAR(10) = '" + S_ReturnStatus + @"',

		        @stationTypeID		int,
		        @mStationId INT,						
		        @PackageID			INT,
		        @CancelToStationID  INT,

		        @Result    NVARCHAR(100)


                
                declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre INT,@LineID INT,@routeID INT,@UnitStateID INT

                --SELECT TOP 10 * FROM dbo.mesHistory WHERE StationID = 319
                SELECT @Result='OK'													
                SELECT @PackageID=ID,@mStationId=StationID FROM mesPackage WHERE SerialNumber=@IBoxSN AND Stage='1'	
                SELECT @stationTypeID=StationTypeID FROM mesStation WHERE ID=@mStationId

                if ISNULL(@IReturnToStationTypeID,'') <> ''
                begin
	                SET @Result='ERROR:no setup current station to return.'
					SELECT @Result as SqlResult
	                RETURN
                end                

                CREATE TABLE #TempRoute
                (	
	                ID				int,
	                UnitStateID		int
                )
                --20220524 howard
                select u.UnitStateID,u.StationID, ROW_NUMBER() OVER(ORDER BY u.UnitStateID,u.StationID) AS rowIndex into #tmpStatesTable
                from mesUnitDetail ud
                join mesUnit u on ud.UnitID=u.ID
                join mesPackage p on ud.InmostPackageID=p.id 
				WHERE  p.SerialNumber = @IBoxSN AND p.Stage=1
				GROUP BY u.UnitStateID,u.StationID

				IF (SELECT COUNT(1) FROM #tmpStatesTable) > 1
				BEGIN
				    SET @Result='ERROR: FG status more than 1.'
					SELECT @Result as SqlResult
	                RETURN
				END


                select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
                from mesUnitDetail ud
                join mesUnit u on ud.UnitID=u.ID
                join mesPackage p on ud.InmostPackageID=p.id where  p.SerialNumber = @IBoxSN AND p.Stage=1

                IF ISNULL(@unitId, 0) = 0 OR ISNULL(@LineID, 0) = 0 OR ISNULL(@ProductionOrderID,0) = 0 OR @StationID_Pre <> @IStationID
                BEGIN
                    SET @Result='ERROR:current station no match.'
					SELECT @Result as SqlResult
	                RETURN
                END
		
                --SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
				DECLARE @PartFamilyID INT = 0
				select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
				select top 1 @routeID = RouteID
				from mesRouteMap
				where
					(LineID = @LineID or LineID is NULL OR LineID=0) AND
					(PartID = @PartID or PartID is NULL OR PartID=0) AND
					(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
					(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
				order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
                IF ISNULL(@routeID,'')=''
                BEGIN
	                SET @Result='ERROR:can t find route ID.'
					SELECT @Result as SqlResult
	                RETURN
                END

                --根据route类型获取上一站UnitstateID
                IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
                BEGIN
	                IF(SELECT COUNT(1) FROM mesUnitInputState A 
	                INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
	                WHERE RouteID=@routeID AND NOT EXISTS(SELECT 1 FROM dbo.V_StationTypeInfo 
		                WHERE StationTypeID = b.StationTypeID AND DetailDef = 'StationTypeType' AND ISNULL(Content,'') = 'TT')
		                )>1
	                BEGIN
		                SET @Result='ERROR:The current station type is not configured in the configured process flow. Please check the process flow.'
						SELECT @Result as SqlResult
		                RETURN			    
	                END

	                SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
		                INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
	                WHERE RouteID=@routeID
                END
                ELSE
                BEGIN
	                INSERT INTO #TempRoute(ID,UnitStateID)
	                SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
	                FROM mesRouteDetail where RouteID=@routeID

	                SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
		                (SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
                END

                IF ISNULL(@UnitStateID,'')=''
                BEGIN
	                SET @Result='ERROR:The part number is not configured with the unit state of the process flow.'
					SELECT @Result as SqlResult
	                RETURN 
                END

                IF not EXISTS(SELECT 1 FROM dbo.mesHistory WHERE UnitID = @unitId AND UnitStateID = @UnitStateID)
                BEGIN
                    SET @Result = 'ERROR:no exist to return station id.'
					SELECT @Result as SqlResult
	                RETURN
                END
                SELECT TOP 1 @CancelToStationID = StationID FROM dbo.mesHistory 
                WHERE UnitID = @unitId AND UnitStateID = @UnitStateID
                ORDER BY ID DESC

                BEGIN TRY
	                BEGIN TRAN
	                UPDATE mesPackage SET StationID=@CancelToStationID,LastUpdate=GETDATE(),EmployeeID=@IEmployeeId WHERE ID=@PackageID

	                INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
	                VALUES (@PackageID,9,@IStationID,@IEmployeeId,GETDATE())	
		
	                --修改mesUnit
	                UPDATE D SET D.StationID=@CancelToStationID,UnitStateID=@UnitStateID,EmployeeID=@IEmployeeId,
	                D.StatusID = CASE WHEN(ISNULL(@IStatusId,'') = '') THEN 1 ELSE CAST(@IStatusId AS INT) END FROM mesUnit D
	                INNER JOIN mesUnitDetail C ON C.UnitID=D.ID  
	                INNER JOIN mesPackage A ON ISNULL(C.InmostPackageID,'')=A.ID
	                WHERE A.ID=@PackageID

	                --mesHistory记录
	                INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)				
	                SELECT D.ID,@UnitStateID,@IEmployeeId,@IStationID,GETDATE(),GETDATE(),@IProdID,@partID,1,CASE WHEN(ISNULL(@IStatusId,'') = '') THEN 1 ELSE CAST(@IStatusId AS INT) END FROM mesPackage A  
	                INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=A.ID
	                INNER JOIN mesUnit D ON C.UnitID=D.ID WHERE A.ID=@PackageID	
	                COMMIT TRAN
					set @Result = 'OK'
                END TRY
                BEGIN CATCH
	                IF @@ROWCOUNT > 0
	                BEGIN
	                    ROLLBACK TRAN
	                END
	                SET @Result = ERROR_MESSAGE()
                END CATCH	

                SELECT @Result as SqlResult
                ";
            var queryRes = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null,I_DBTimeout,null);
            S_Result = queryRes?.ToString();
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", ex);
            S_Result = "ERROR:" + ex.ToString();
        }

        return S_Result;

    }
    private async Task<string> SetCancelInWHEntry_P(string S_BoxSN, string S_ProdID, string S_PartID, string S_StationID, string S_EmployeeID, string S_ReturnToStationTypeID, string S_ReturnStatus)
    {
        string S_Result = "";
        try
        {
            string S_Sql =
            @"
                DECLARE	 @IBoxSN VARCHAR(100) = '" + S_BoxSN + @"',
		        @IProdID			INT='" + S_ProdID + @"',
		        @partID				INT = '" + S_PartID + @"',		
		        @IStationID			INT = '" + S_StationID + @"',
		        @IEmployeeId         INT = '" + S_EmployeeID + @"',
                @IReturnToStationTypeID VARCHAR(10) = '" + S_ReturnToStationTypeID + @"',
                @IStatusId VARCHAR(10) = '" + S_ReturnStatus + @"',

		        @stationTypeID		int,
		        @mStationId INT,						
		        @PackageID			INT,
		        @CancelToStationID  INT,

		        @Result    NVARCHAR(100)


                
                declare @CurrStateID int,@ProductionOrderID int,@unitId int,@StationID_Pre INT,@LineID INT,@routeID INT,@UnitStateID INT

                --SELECT TOP 10 * FROM dbo.mesHistory WHERE StationID = 319
                SELECT @Result='OK'													
                SELECT @PackageID=ID,@mStationId=StationID FROM mesPackage WHERE SerialNumber=@IBoxSN AND Stage='1'	
                SELECT @stationTypeID=StationTypeID FROM mesStation WHERE ID=@mStationId

                if ISNULL(@IReturnToStationTypeID,'') <> ''
                begin
	                SET @Result='ERROR:no setup current station to return.'
					SELECT @Result as SqlResult
	                RETURN
                end                

                CREATE TABLE #TempRoute
                (	
	                ID				int,
	                UnitStateID		int
                )
                --20220524 howard
                select u.UnitStateID,u.StationID, ROW_NUMBER() OVER(ORDER BY u.UnitStateID,u.StationID) AS rowIndex into #tmpStatesTable
                from mesUnitDetail ud
                join mesUnit u on ud.UnitID=u.ID
                join mesPackage p on ud.InmostPackageID=p.id 
				WHERE  p.SerialNumber = @IBoxSN AND p.Stage=1
				GROUP BY u.UnitStateID,u.StationID

				IF (SELECT COUNT(1) FROM #tmpStatesTable) > 1
				BEGIN
				    SET @Result='ERROR: FG status more than 1.'
					SELECT @Result as SqlResult
	                RETURN
				END


                select  top 1 @unitId=ud.UnitID,@CurrStateID=UnitStateID,@PartID=u.PartID,@LineID=u.LineID,@ProductionOrderID=u.ProductionOrderID,@StationID_Pre=u.StationID 
                from mesUnitDetail ud
                join mesUnit u on ud.UnitID=u.ID
                join mesPackage p on ud.InmostPackageID=p.id where  p.SerialNumber = @IBoxSN AND p.Stage=1

                IF ISNULL(@unitId, 0) = 0 OR ISNULL(@LineID, 0) = 0 OR ISNULL(@ProductionOrderID,0) = 0 OR @StationID_Pre <> @IStationID
                BEGIN
                    SET @Result='ERROR:current station no match.'
					SELECT @Result as SqlResult
	                RETURN
                END
		
                SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@ProductionOrderID)
                IF ISNULL(@routeID,'')=''
                BEGIN
	                SET @Result='ERROR:can t find route ID.'
					SELECT @Result as SqlResult
	                RETURN
                END

                --根据route类型获取上一站UnitstateID
                IF EXISTS(SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
                BEGIN
	                IF(SELECT COUNT(1) FROM mesUnitInputState A 
	                INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
	                WHERE RouteID=@routeID AND NOT EXISTS(SELECT 1 FROM dbo.V_StationTypeInfo 
		                WHERE StationTypeID = b.StationTypeID AND DetailDef = 'StationTypeType' AND ISNULL(Content,'') = 'TT')
		                )>1
	                BEGIN
		                SET @Result='ERROR:The current station type is not configured in the configured process flow. Please check the process flow.'
						SELECT @Result as SqlResult
		                RETURN			    
	                END

	                SELECT @UnitStateID = CurrStateID FROM mesUnitInputState A 
		                INNER JOIN mesStation B ON A.StationTypeID=B.StationTypeID AND B.ID=@StationID_Pre
	                WHERE RouteID=@routeID
                END
                ELSE
                BEGIN
	                INSERT INTO #TempRoute(ID,UnitStateID)
	                SELECT ROW_NUMBER() OVER(ORDER BY Sequence) AS ID,UnitStateID
	                FROM mesRouteDetail where RouteID=@routeID

	                SELECT @UnitStateID=A.UnitStateID FROM #TempRoute A WHERE EXISTS 
		                (SELECT 1 FROM #TempRoute B WHERE B.UnitStateID=@CurrStateID AND A.ID=B.ID-1)
                END

                IF ISNULL(@UnitStateID,'')=''
                BEGIN
	                SET @Result='ERROR:The part number is not configured with the unit state of the process flow.'
					SELECT @Result as SqlResult
	                RETURN 
                END

                IF not EXISTS(SELECT 1 FROM dbo.mesHistory WHERE UnitID = @unitId AND UnitStateID = @UnitStateID)
                BEGIN
                    SET @Result = 'ERROR:no exist to return station id.'
					SELECT @Result as SqlResult
	                RETURN
                END
                SELECT TOP 1 @CancelToStationID = StationID FROM dbo.mesHistory 
                WHERE UnitID = @unitId AND UnitStateID = @UnitStateID
                ORDER BY ID DESC

                BEGIN TRY
	                BEGIN TRAN
	                UPDATE mesPackage SET StationID=@CancelToStationID,LastUpdate=GETDATE(),EmployeeID=@IEmployeeId WHERE ID=@PackageID

	                INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
	                VALUES (@PackageID,9,@IStationID,@IEmployeeId,GETDATE())	
		
	                --修改mesUnit
	                UPDATE D SET D.StationID=@CancelToStationID,UnitStateID=@UnitStateID,EmployeeID=@IEmployeeId,
	                D.StatusID = CASE WHEN(ISNULL(@IStatusId,'') = '') THEN 1 ELSE CAST(@IStatusId AS INT) END FROM mesUnit D
	                INNER JOIN mesUnitDetail C ON C.UnitID=D.ID  
	                INNER JOIN mesPackage A ON ISNULL(C.InmostPackageID,'')=A.ID
	                WHERE A.ID=@PackageID

	                --mesHistory记录
	                INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)				
	                SELECT D.ID,@UnitStateID,@IEmployeeId,@IStationID,GETDATE(),GETDATE(),@IProdID,@partID,1,CASE WHEN(ISNULL(@IStatusId,'') = '') THEN 1 ELSE CAST(@IStatusId AS INT) END FROM mesPackage A  
	                INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=A.ID
	                INNER JOIN mesUnit D ON C.UnitID=D.ID WHERE A.ID=@PackageID	
	                COMMIT TRAN
					set @Result = 'OK'
                END TRY
                BEGIN CATCH
	                IF @@ROWCOUNT > 0
	                BEGIN
	                    ROLLBACK TRAN
	                END
	                SET @Result = ERROR_MESSAGE()
                END CATCH	

                SELECT @Result as SqlResult
                ";
            var queryRes = await DapperConnRead2.ExecuteScalarAsync(S_Sql, null, null, I_DBTimeout, null);
            S_Result = queryRes?.ToString();
        }
        catch (Exception ex)
        {
            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", ex);
            S_Result = "ERROR:" + ex.ToString();
        }

        return S_Result;

    }
}