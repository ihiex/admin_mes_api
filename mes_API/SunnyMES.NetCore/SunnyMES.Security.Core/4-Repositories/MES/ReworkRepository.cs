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
    public class ReworkRepository : BaseRepositoryReport<string>, IReworkRepository
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


        public ReworkRepository()
        {
        }

        public ReworkRepository(IDbContextCore dbContext) : base(dbContext)
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
                if (item.ValStr2.Trim() == "TTBoxSN_Pattern")
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
                if (item.ValStr2.Trim() == "IsTTBoxUnpack")
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
                  //  S_TTScanType = List_IsCheckPOPN[0].TTScanType;
                  //  S_IsTTRegistSN = List_IsCheckPOPN[0].IsTTRegistSN;
                  //  S_TTBoxType = List_IsCheckPOPN[0].TTBoxType;

                    S_IsChangePN = List_IsCheckPOPN[0].IsChangePN;
                    S_IsChangePO = List_IsCheckPOPN[0].IsChangePO;
                    S_ChangedUnitStateID = List_IsCheckPOPN[0].ChangedUnitStateID;

                  //  S_JumpFromUnitStateID = List_IsCheckPOPN[0].JumpFromUnitStateID;
                  //  S_JumpToUnitStateID = List_IsCheckPOPN[0].JumpToUnitStateID;
                  //  S_JumpStatusID = List_IsCheckPOPN[0].JumpStatusID;
                  //  S_JumpUnitStateID = List_IsCheckPOPN[0].JumpUnitStateID;
                    S_OperationType=List_IsCheckPOPN[0].OperationType;

                    string S_IsPrint = "0";
                    if (List_Login.PrintIPPort != "") { S_IsPrint = "1"; }

                    string
                    S_Sql = "select '" + S_IsCheckPO + "' as IsCheckPO,'" + S_IsCheckPN +
                            "' as IsCheckPN,'" + S_ApplicationType + "' as ApplicationType,'" + S_IsLegalPage +
                            "' as IsLegalPage,'" +
						//	S_TTScanType + "' as TTScanType,'" + S_IsTTRegistSN +
                        //    "' as IsTTRegistSN,'" + S_TTBoxType + "' as TTBoxType, '" +
                            S_IsChangePN + "' as IsChangePN,'" + S_IsChangePO + "' as IsChangePO,'" +
                            S_ChangedUnitStateID + "' as ChangedUnitStateID,'" +
                        //    S_JumpFromUnitStateID + "' as JumpFromUnitStateID,'" +
                        //    S_JumpToUnitStateID + "' as JumpToUnitStateID,'" +
                        //    S_JumpStatusID + "' as JumpStatusID,'" +
                        //    S_JumpUnitStateID + "' as JumpUnitStateID,'" +
                            S_OperationType + "' as OperationType,'" +
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

        //public async Task<ConfirmPOOutputDto> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
        //    string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        //{
        //    List<dynamic> List_Result = new List<dynamic>();
        //    //初始化数据
        //    IEnumerable<dynamic> List_PageInitialize = await GetPageInitialize(S_URL);

        //    ConfirmPOOutputDto List_SetConfirmPO = await Public_Repository.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
        //        S_PartID, S_POID, S_UnitStatus, List_Login);

        //    return List_SetConfirmPO;
        //}

        public async Task<GetReworkDto> SetScanSN_GetRework(string S_SN,string S_Type, string S_URL)
        {
            S_SN = S_SN ?? "";

            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            GetReworkDto F_GetReworkDto = new GetReworkDto();
            F_GetReworkDto.MSG = TabVal_MSGERROR;

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

                    F_GetReworkDto.MSG = TabVal_MSGERROR;
                    return F_GetReworkDto;
                }

                DataSet DS = GetRework(S_SN,S_Type);
                if (DS.Tables[0].Columns[0].Caption == "ERROR") 
                {
                    TabVal_MSGERROR = Public_Repository.GetERROR(DS.Tables[0].Rows[0]["ERROR"].ToString(), "0", List_Login, "");
                    F_GetReworkDto.MSG = TabVal_MSGERROR;
                    return F_GetReworkDto;
                }

                List<ReworkData> List_ReworkData = new List<ReworkData>();
                List<ReworkPrintSN> List_ReworkSN = new List<ReworkPrintSN>();
				if (DS.Tables.Count > 1)
				{
					string S_Result = DS.Tables[0].Rows[0][0].ToString();
					if (S_Result != "1")
					{
						TabVal_MSGERROR = Public_Repository.GetERROR(S_Result, "0", List_Login, "");
						F_GetReworkDto.MSG = TabVal_MSGERROR;
						return F_GetReworkDto;
					}

					if (DS.Tables[1].Rows.Count == 0)
					{
						TabVal_MSGERROR = Public_Repository.GetERROR("No Data", "0", List_Login, "");
						F_GetReworkDto.MSG = TabVal_MSGERROR;
						return F_GetReworkDto;
					}

					for (int i = 0; i < DS.Tables[1].Rows.Count; i++)
					{
						ReworkData ReworkData = new ReworkData();
						ReworkData.ID = Convert.ToInt32(DS.Tables[1].Rows[i]["ID"].ToString());
						ReworkData.SerialNumber = DS.Tables[1].Rows[i]["SerialNumber"].ToString();
						ReworkData.Part = DS.Tables[1].Rows[i]["Part"].ToString();
						ReworkData.InsertedTime = Convert.ToDateTime(DS.Tables[1].Rows[i]["InsertedTime"].ToString());
						//ReworkData.UnitID = DS.Tables[0].Rows[0]["UnitID"].ToString();

						List_ReworkData.Add(ReworkData);
					}
					F_GetReworkDto.ReworkData = List_ReworkData;

					for (int i = 0; i < DS.Tables[1].Rows.Count; i++)
					{
						ReworkPrintSN ReworkSN = new ReworkPrintSN();
						ReworkSN.PrintSN = DS.Tables[1].Rows[i]["SerialNumber"].ToString();
						ReworkSN.PrintSNPartID = Convert.ToInt32(DS.Tables[1].Rows[i]["PartID"].ToString());

						List_ReworkSN.Add(ReworkSN);
					}
				}
				else 
				{
                    TabVal_MSGERROR = Public_Repository.GetERROR(DS.Tables[0].Rows[0]["Valstr1"].ToString(), "0", List_Login, S_SN);
                    F_GetReworkDto.MSG = TabVal_MSGERROR;
                    return F_GetReworkDto;
                }

				F_GetReworkDto.ReworkData = List_ReworkData;
                F_GetReworkDto.PrintSN = List_ReworkSN;


                //List<dynamic> List_ProductionOrderQTY = await
                //    Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(S_POID));
                //F_GetReworkDto.ProductionOrderQTY = null;
            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, "");
                F_GetReworkDto.MSG = TabVal_MSGERROR;
                return F_GetReworkDto;
            }

            Public_Repository.SetMesLogVal("SN:  " + S_SN + "  Scan success", "OK", List_Login);
            return F_GetReworkDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_Type">解绑类型1:SN 2:UPC 3:Multipack 4:Pallet </param>
        /// <returns></returns>
        private DataSet GetRework(string S_SN,string S_Type)
        {
            DataSet DS=new DataSet ();
            try
            {
				string S_Sql =
                    @"                 
					declare	@UnitdID			VARCHAR(64),
							@idoc				int,
							@UnitStateID		VARCHAR(64),

							@AllowUnitStateID	  VARCHAR(200),
			
							@PartID				int,
							@LineID				int,
							@StatusID			int,
							@ProductionOrderID	INT,
			
							@FGUnitstateID      INT,
                            @strOutput          varchar(500)
                     
				declare @strSNFormat   nvarchar(200)='" + S_SN + @"'
				declare @StationID   nvarchar(200)='" + List_Login.StationID + @"'

				declare @strSNbuf      nvarchar(200)='" + S_Type + @"'


                SET @strOutput='1'  
				SELECT @UnitStateID=Value FROM mesStationConfigSetting WHERE Name='ReworkUnitStateID' AND StationID=@StationID
				SELECT @AllowUnitStateID=Value FROM mesStationConfigSetting WHERE Name='AllowUnitStateID' AND StationID=@StationID

	            BEGIN TRY
				---------------------------------------------------条码校验----------------------------------------------------------------
				IF @strSNbuf=1 OR @strSNbuf=2
				BEGIN
					SELECT @UnitdID=UnitID,@FGUnitstateID=B.UnitStateID
					  FROM mesSerialNumber A JOIN mesUnit B ON A.UnitID=B.ID WHERE A.Value=@strSNFormat
			 
					 --条码不存在 
					IF ISNULL(@UnitdID,'')=''
					BEGIN
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20012 + @"'
						Select  @strOutput as ValStr1
						RETURN
					END

					IF @strSNbuf=1
					BEGIN
						--如果解绑条码已经被其他条码绑定，必须先解绑上一级条码
						IF EXISTS(SELECT 1 FROM mesUnitComponent A INNER JOIN mesSerialNumber B ON A.ChildUnitID=B.UnitID 
									WHERE A.StatusID=1 AND B.Value=@strSNFormat)
						BEGIN
							SET @strOutput='" + P_MSG_Sys.MSG_Sys_20191 + @"'
							Select  @strOutput as ValStr1
							RETURN
						END

						--判断是否已经LinkUPC
						IF EXISTS (SELECT 1 FROM mesUnitDetail A INNER JOIN mesSerialNumber B ON A.UnitID=B.UnitID
									WHERE B.Value=@strSNFormat AND ISNULL(A.KitSerialNumber,'')<>'')
						BEGIN
							SET @strOutput='" + P_MSG_Sys.MSG_Sys_20191 + @"'
							Select  @strOutput as ValStr1
							RETURN
						END				
				
				
						--该条码已经完成包装,请先进行拆箱操作再解绑.
						IF EXISTS( SELECT 1 FROM mesUnitDetail A INNER JOIN mesSerialNumber B ON A.UnitID=B.UnitID 
									WHERE B.Value = @strSNFormat and isnull(A.InmostPackageID,'')<>'')
						BEGIN
							SET @strOutput='" + P_MSG_Sys.MSG_Sys_20192 + @"'
							Select  @strOutput as ValStr1
							RETURN
						END

						--判断是否已经判断是否已经HW_IN/OUT/IPQC5/TT    IF @FGUnitstateID not in (200,201,202)		
					
						-- 判断只有指定状态的才可以回退 2023-03-17
						IF @AllowUnitStateID<>''
						BEGIN
							IF @FGUnitstateID not in (SELECT value FROM  dbo.F_Split(@AllowUnitStateID,',') )
							BEGIN
								SET @strOutput='" + P_MSG_Sys.MSG_Sys_20252 + @"'
								Select  @strOutput as ValStr1
								RETURN
							END
						END

						SELECT TOP 1 @StatusID=StatusID,@PartID=B.PartID ,@LineID=B.LineID, @ProductionOrderID=ProductionOrderID
						FROM mesSerialNumber A 
						INNER JOIN mesUnit B ON A.UnitID=B.ID 
						WHERE A.Value=@strSNFormat
					END
					ELSE
					BEGIN
						--未找到UPC绑定的FG条码信息.
						IF NOT EXISTS( SELECT 1 FROM mesUnitDetail WHERE KitSerialNumber = @strSNFormat )
						BEGIN
							SET @strOutput='" + P_MSG_Sys.MSG_Sys_20189 + @"'
							Select  @strOutput as ValStr1
							RETURN
						END
						--如果已经包装需要先拆箱
						IF EXISTS( SELECT 1 FROM mesUnitDetail WHERE KitSerialNumber = @strSNFormat and isnull(InmostPackageID,'')<>'')
						BEGIN
							SET @strOutput='" + P_MSG_Sys.MSG_Sys_20192 + @"'
							Select  @strOutput as ValStr1
							RETURN
						END

						--UPC只校验主条码
						SELECT  TOP 1 @StatusID=StatusID,@PartID=B.PartID ,@LineID=B.LineID FROM mesUnitDetail A 
						INNER JOIN mesUnit B ON A.UnitID=B.ID
						WHERE A.KitSerialNumber=@strSNFormat

						-- [uspGetDisassembly] 内容 				
						--SELECT TOP 1 @StatusID=StatusID,@PartID=B.PartID ,@LineID=B.LineID, @ProductionOrderID=ProductionOrderID
						--FROM mesSerialNumber A 
						--INNER JOIN mesUnit B ON A.UnitID=B.ID 
						--WHERE A.Value=@strSNFormat				
				
					END

					-- 条码为非正常状态,不能进行解绑操作.
					IF isnull(@StatusID,0)<>1
					BEGIN
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20187 + @"'
						Select  @strOutput as ValStr1
						RETURN
					END

					-- 解绑条码类型与操作类型不符.请检查是否选错解绑工站.
					--IF NOT EXISTS(SELECT 1 FROM mesRouteDetail WHERE RouteID = DBO.ufnRTEGetRouteID(@LineID,@PartID,NULL,@ProductionOrderID) 
					--       AND UnitStateID=@UnitStateID
					--    )
					--IF NOT EXISTS(SELECT 1 FROM mesUnitOutputState WHERE RouteID = DBO.ufnRTEGetRouteID(@LineID,@PartID,NULL,@ProductionOrderID) 
					--	   AND OutputStateID=@UnitStateID
					--	)
					--BEGIN
					--	SET @strOutput='" + P_MSG_Sys.MSG_Sys_20188 + @"'
					--	Select  @strOutput as ValStr1
					--	RETURN
					--END

					DECLARE @PartFamilyID INT = 0,@routeID INT
					select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
					select top 1 @routeID = RouteID
					from mesRouteMap
					where
						(LineID = @LineID or LineID is NULL OR LineID=0) AND
						(PartID = @PartID or PartID is NULL OR PartID=0) AND
						(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
						(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
					order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID DESC
                    
					IF NOT EXISTS(SELECT 1 FROM mesUnitOutputState WHERE RouteID = @routeID
						   AND OutputStateID=@UnitStateID)
					BEGIN
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20188 + @"'
						Select  @strOutput as ValStr1
						RETURN
					END

				END	
				ELSE IF @strSNbuf=3
				BEGIN
					 --该条码已经被其他条码绑定,请先解绑上一级条码.
					IF  EXISTS(SELECT 1 FROM mesPackage WHERE StatusID=1 
												AND ISNULL(ParentID,'')<>'' AND SerialNumber=@strSNFormat)
					BEGIN
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20191 + @"'
						Select  @strOutput as ValStr1	
						RETURN
					END	
			
					IF NOT EXISTS(SELECT TOP 1 * FROM mesPackage A JOIN mesUnitDetail B ON A.ID=B.InmostPackageID WHERE A.SerialNumber=@strSNFormat)
					BEGIN
						-- 	未找到包装信息,请确认条码是否存在或者已经解绑.
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20190 + @"'
						Select  @strOutput as ValStr1						
						RETURN				
					END		
				END
				ELSE IF @strSNbuf=4
				BEGIN
					IF NOT EXISTS(SELECT TOP 1 * FROM mesPackage A JOIN mesPackage B ON A.ID=B.ParentID WHERE A.SerialNumber=@strSNFormat)
					BEGIN
						-- 	未找到包装信息,请确认条码是否存在或者已经解绑.
						SET @strOutput='" + P_MSG_Sys.MSG_Sys_20190 + @"'
						Select  @strOutput as ValStr1						
						RETURN					
					END			
				END			
					
				Select  @strOutput as ValStr1
				---------------------------------------------------数据查询----------------------------------------------------------------
				IF @strSNbuf=1
				BEGIN
					SELECT A.ID,CASE WHEN ISNULL(A.ChildSerialNumber,'')='' THEN A.ChildLotNumber ELSE A.ChildSerialNumber END SerialNumber
					,B.Description Part,A.InsertedTime,B.Id  PartID
					FROM mesUnitComponent A
					INNER JOIN mesPart B ON A.ChildPartID=B.ID WHERE A.UnitID=@UnitdID AND A.StatusID=1
					UNION ALL
					SELECT A.ID,C.Value SerialNumber,B.Description Part,A.InsertedTime,B.Id  PartID  
					FROM mesUnitComponent A
					INNER JOIN mesPart B ON A.UnitID=B.ID
					INNER JOIN mesSerialNumber C ON A.UnitID=C.UnitID WHERE A.ChildUnitID=@UnitdID AND A.StatusID=1
				END
				ELSE IF @strSNbuf=2
				BEGIN
					SELECT A.ID,D.Value SerialNumber,C.Description Part,C.Id  PartID FROM mesUnitDetail A
					INNER JOIN mesUnit B ON A.UnitID=B.ID
					INNER JOIN mesPart C ON B.PartID=C.ID
					INNER JOIN mesSerialNumber D ON A.UnitID=D.UnitID
					WHERE A.KitSerialNumber =@strSNFormat
				END
				ELSE IF @strSNbuf=3
				BEGIN
					SELECT B.ID,C.Value FGSN,B.KitSerialNumber SerialNumber,D.Description Part,A.CreationTime,D.Id  PartID FROM mesPackage A 
					INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
					INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
					INNER JOIN mesPart D ON A.CurrPartID=D.ID
					WHERE A.SerialNumber=@strSNFormat AND A.StatusID=1 and A.Stage=1
				END
				ELSE IF @strSNbuf=4
				BEGIN
					SELECT A.ID,A.SerialNumber,C.Description Part,A.CreationTime,C.Id  PartID FROM mesPackage A
					INNER JOIN mesPackage B ON A.ParentID=B.ID
					INNER JOIN mesPart C ON A.CurrPartID=C.ID
					WHERE B.SerialNumber=@strSNFormat AND (A.StatusID=0 OR A.StatusID=1) AND (B.StatusID=0	OR B.StatusID=1)
				END
				
			END TRY
			BEGIN CATCH
				SELECT @strOutput=ERROR_MESSAGE()
                Select  @strOutput as ValStr1
			END CATCH			
            ";

                DS = Data_Set(S_Sql);

            }
            catch (Exception ex)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("ERROR");
                DataRow dr = dt.NewRow();
                dr["ERROR"] = ex.ToString();
                dt.Rows.Add(dr);
                DS.Tables.Add(dt);
            }
            return DS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="S_SN">SN</param>
        /// <param name="S_Type">解绑类型1:SN 2:UPC 3:Multipack 4:Pallet</param>
        /// <returns></returns>
        public async Task<string> SetRework(string S_SN,string S_Type)   
        {
            string S_Result = "1";

            try
            {
            string S_Sql =
                @"
	            declare	@UnitdID			VARCHAR(64),
			            @idoc				int,
			            @UnitStateID		VARCHAR(64),
			            @StationID			int='" + List_Login.StationID.ToString() + @"',
			            @EmployeeID			int='" + List_Login.EmployeeID.ToString() + @"',
			            @PartID				int=0,
			            @ProductionOrderID	int=0,
			            --@RemovedEmployeeID  int,
			            --@RemovedStationID	INT,
			            @PartFamilyID int=0,
			            @LineID int=0,
			            @RouteID            INT,
			            @CurrStationTypeID  INT,
			            @ReWorkStationTypeID  INT, 
			            @StartSEQ              INT,
			            @EndSEQ               INT, 
			
			            @PalletID           INT,
			            @FGUnitID           INT,
			            @UPCUnitID          INT,
			            @BoxID              INT,			
			            @BoxSN              VARCHAR(64) 

            declare   @strSNFormat	    nvarchar(200)='" + S_SN + @"'
			declare   @strSNbuf			nvarchar(200)='" + S_Type + @"'
            declare   @strOutput        nvarchar(200)


			BEGIN TRY
			BEGIN TRANSACTION Task  
				CREATE TABLE #BOMStationTypeID(StationTypeID int)
				SELECT @ReWorkStationTypeID=Value FROM mesStationConfigSetting WHERE Name='ReWorkStationTypeID' AND StationID=@StationID
				SELECT @UnitStateID=Value FROM mesStationConfigSetting WHERE Name='ReworkUnitStateID' AND StationID=@StationID

				---------------------------------------------------Rework条码状态----------------------------------------------------------------
		
				IF @strSNbuf=1
				BEGIN
					SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID,@PartFamilyID=p.PartFamilyID,@LineID=B.LineID
					FROM mesSerialNumber A 
					INNER JOIN mesUnit B ON A.UnitID=B.ID 
					inner join mesPart p on p.id=B.partID
					WHERE Value=@strSNFormat 
			
					--SELECT @RouteID=dbo.ufnRTEGetRouteID(@LineID,@PartID,@PartFamilyID,@ProductionOrderID)			
					
					select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
					select top 1 @RouteID = RouteID
					from mesRouteMap
					where
						(LineID = @LineID or LineID is NULL OR LineID=0) AND
						(PartID = @PartID or PartID is NULL OR PartID=0) AND
						(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
						(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
					order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
					IF ISNULL(@RouteID,'')=''
					BEGIN
						SET @strOutput='ERROR:can t find route ID.'
						SELECT @strOutput as ValStr1
						RETURN
					END

					SELECT @CurrStationTypeID=C.StationTypeID
						FROM mesUnit A 
							JOIN mesStation C ON A.StationID=C.ID  
						WHERE A.ID=@UnitdID			
			
					UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID,A.LastUpdate =GETDATE()  FROM mesUnit A
					WHERE A.ID=@UnitdID

					-- 处理 FG 需要回退的子料
		        
					--SELECT @ReWorkStationTypeID=Value FROM mesStationConfigSetting WHERE Name='ReWorkStationTypeID' AND StationID=@StationID		
					SELECT TOP 1 @StartSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@ReWorkStationTypeID AND RouteID=@RouteID
					SELECT TOP 1 @EndSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@CurrStationTypeID AND RouteID=@RouteID				
								
					INSERT INTO #BOMStationTypeID	
					SELECT DISTINCT(stationTypeID) StationTypeID FROM mesRouteSEQ WHERE seq>=@StartSEQ AND seq<=@EndSEQ AND RouteID=@RouteID
			             
					UPDATE A SET A.StatusID=0,RemovedEmployeeID=@EmployeeID,RemovedStationID=@StationID,
						RemovedTime=GETDATE() FROM mesUnitComponent A
					WHERE A.UnitID=@UnitdID AND A.StatusID=1 and a.ChildPartID IN
					(
						SELECT PartID FROM mesProductStructure WHERE StationTypeID IN (SELECT StationTypeID from #BOMStationTypeID) 
							AND ParentPartID=@PartID				
					)
			
					delete #BOMStationTypeID
			
					--记录History
					Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
					values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)
				END
				ELSE IF @strSNbuf=2
				BEGIN			
					SELECT TOP 1 @FGUnitID=A.ID FROM mesUnit A 
					INNER JOIN mesUnitDetail B ON A.ID=B.UnitID WHERE B.KitSerialNumber=@strSNFormat				
			
					--UPC需要修改FG状态											
					SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
					INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@FGUnitID
			
					SELECT @CurrStationTypeID=C.StationTypeID
						FROM mesUnit A 
							JOIN mesStation C ON A.StationID=C.ID  
						WHERE A.ID=@UnitdID			
			
					UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID ,A.LastUpdate =GETDATE() FROM mesUnit A
					WHERE A.ID=@UnitdID

					-- 处理 FG 需要回退的子料
					SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID,@PartFamilyID=p.PartFamilyID,@LineID=B.LineID
					FROM mesSerialNumber A 
					INNER JOIN mesUnit B ON A.UnitID=B.ID 
					inner join mesPart p on p.id=B.partID
					WHERE Value=@strSNFormat 
			
					--SELECT @RouteID=dbo.ufnRTEGetRouteID(@LineID,@PartID,@PartFamilyID,@ProductionOrderID)				
					select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
					select top 1 @RouteID = RouteID
					from mesRouteMap
					where
						(LineID = @LineID or LineID is NULL OR LineID=0) AND
						(PartID = @PartID or PartID is NULL OR PartID=0) AND
						(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
						(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
					order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
					IF ISNULL(@RouteID,'')=''
					BEGIN
						SET @strOutput='ERROR:can t find route ID.'
						SELECT @strOutput as ValStr1
						RETURN
					END
					--SELECT @ReWorkStationTypeID=Value FROM mesStationConfigSetting WHERE Name='ReWorkStationTypeID' AND StationID=@StationID		
					SELECT TOP 1 @StartSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@ReWorkStationTypeID AND RouteID=@RouteID
					SELECT TOP 1 @EndSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@CurrStationTypeID AND RouteID=@RouteID		 		
									
					INSERT INTO #BOMStationTypeID	
					SELECT DISTINCT(stationTypeID) StationTypeID FROM mesRouteSEQ WHERE seq>=@StartSEQ AND seq<=@EndSEQ AND RouteID=@RouteID
			             
					UPDATE A SET A.StatusID=0,RemovedEmployeeID=@EmployeeID,RemovedStationID=@StationID,
						RemovedTime=GETDATE() FROM mesUnitComponent A
					WHERE A.UnitID=@UnitdID AND A.StatusID=1 and a.ChildPartID IN
					(
						SELECT PartID FROM mesProductStructure WHERE StationTypeID IN (SELECT StationTypeID from #BOMStationTypeID) 
							AND ParentPartID=@PartID				
					)
			
					delete #BOMStationTypeID

					--记录 FG History
					Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
					values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)
																																	
					-- 修改 UPC 状态
					SELECT @UPCUnitID=A.ID FROM mesUnit A JOIN mesSerialNumber B ON A.ID=B.UnitID WHERE B.[Value]=@strSNFormat
			
					UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID,A.LastUpdate =GETDATE()  FROM mesUnit A
					WHERE A.ID=@UPCUnitID				
			
					UPDATE mesUnitDetail SET reserved_06=KitSerialNumber WHERE KitSerialNumber=@strSNFormat
					UPDATE mesUnitDetail SET KitSerialNumber=NULL WHERE KitSerialNumber=@strSNFormat

					--记录 UPC History
					SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
					INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE Value=@strSNFormat 

					Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
					values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)
				END
				ELSE IF @strSNbuf=3
				BEGIN
					--解绑中箱需要更新FG状态	
					SELECT @BoxID=ID FROM mesPackage WHERE SerialNumber=@strSNFormat
								
					DECLARE cursor_FG CURSOR FOR 
					SELECT A.ID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
					INNER JOIN mesPackage C ON B.InmostPackageID=C.ID AND C.StatusID=1
					WHERE C.SerialNumber=@strSNFormat
            		
					OPEN cursor_FG 
					FETCH NEXT FROM cursor_FG INTO @FGUnitID  
					WHILE @@FETCH_STATUS = 0
						BEGIN	
							SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@FGUnitID
			
							SELECT @CurrStationTypeID=C.StationTypeID
								FROM mesUnit A 
									JOIN mesStation C ON A.StationID=C.ID  
								WHERE A.ID=@UnitdID			
			
							UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID,A.LastUpdate =GETDATE()  FROM mesUnit A
							WHERE A.ID=@UnitdID

							-- 处理 FG 需要回退的子料
							SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID,@PartFamilyID=p.PartFamilyID,@LineID=B.LineID
							FROM mesSerialNumber A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID 
							inner join mesPart p on p.id=B.partID
							WHERE Value=@strSNFormat 
			
							--SELECT @RouteID=dbo.ufnRTEGetRouteID(@LineID,@PartID,@PartFamilyID,@ProductionOrderID)										
					        SELECT @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
							select top 1 @RouteID = RouteID
							from mesRouteMap
							where
								(LineID = @LineID or LineID is NULL OR LineID=0) AND
								(PartID = @PartID or PartID is NULL OR PartID=0) AND
								(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
								(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
							order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
							IF ISNULL(@RouteID,'')=''
							BEGIN
								SET @strOutput='ERROR:can t find route ID.'
								SELECT @strOutput as ValStr1
								RETURN
							END
							--SELECT @ReWorkStationTypeID=Value FROM mesStationConfigSetting WHERE Name='ReWorkStationTypeID' AND StationID=@StationID		
							SELECT TOP 1 @StartSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@ReWorkStationTypeID AND RouteID=@RouteID
							SELECT TOP 1 @EndSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@CurrStationTypeID	AND RouteID=@RouteID 			
			
							INSERT INTO #BOMStationTypeID	
							SELECT DISTINCT(stationTypeID) StationTypeID FROM mesRouteSEQ WHERE seq>=@StartSEQ AND seq<=@EndSEQ AND RouteID=@RouteID
			             
							UPDATE A SET A.StatusID=0,RemovedEmployeeID=@EmployeeID,RemovedStationID=@StationID,
								RemovedTime=GETDATE() FROM mesUnitComponent A
							WHERE A.UnitID=@UnitdID AND A.StatusID=1 and a.ChildPartID IN
							(
								SELECT PartID FROM mesProductStructure WHERE StationTypeID IN (SELECT StationTypeID from #BOMStationTypeID) 
									AND ParentPartID=@PartID				
							)
			
							delete #BOMStationTypeID

							--记录 FG History
							Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
							values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)	
																				
							FETCH NEXT FROM cursor_FG INTO @FGUnitID									
						END
					CLOSE cursor_FG 
					DEALLOCATE cursor_FG  			
											
					--解绑中箱需要更新UPC状态
						
					DECLARE cursor_UPC CURSOR FOR 
					SELECT A.ID FROM mesUnit A 
					INNER JOIN mesSerialNumber D ON A.ID=D.UnitID
					INNER JOIN mesUnitDetail B ON D.Value=B.KitSerialNumber
					INNER JOIN mesPackage C ON B.InmostPackageID=C.ID AND C.StatusID=1
					WHERE C.SerialNumber=@strSNFormat		
			
					OPEN cursor_UPC 
					FETCH NEXT FROM cursor_UPC INTO @UPCUnitID  
					WHILE @@FETCH_STATUS = 0
						BEGIN				
							-- 修改 UPC 状态
							UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID ,A.LastUpdate =GETDATE() FROM mesUnit A
							WHERE A.ID=@UPCUnitID					
					
							UPDATE mesUnitDetail SET reserved_06=KitSerialNumber WHERE UnitID=@UPCUnitID
							UPDATE mesUnitDetail SET KitSerialNumber=NULL WHERE UnitID=@UPCUnitID

							--记录 UPC History
							SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
							INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@UPCUnitID

							Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
							values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)
																				
							FETCH NEXT FROM cursor_UPC INTO @UPCUnitID									
						END
					CLOSE cursor_UPC 
					DEALLOCATE cursor_UPC 			
			
					-- 更新中箱对应的 mesUnitDetail	 		
					UPDATE A SET A.reserved_07=InmostPackageID,InmostPackageID=NULL FROM mesUnitDetail A 
					INNER JOIN mesPackage B ON A.InmostPackageID=B.ID
					WHERE B.SerialNumber=@strSNFormat AND B.StatusID=1
					-- 更新中箱
					UPDATE mesPackage SET StatusID=0,CurrentCount=NULL WHERE SerialNumber=@strSNFormat and StatusID=1

					--记录中箱 History
					Insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
					VALUES(@BoxID,1,@StationID,@EmployeeID,GETDATE())
			
				END
				ELSE IF @strSNbuf=4
				BEGIN
					SELECT @PalletID=ID FROM mesPackage WHERE SerialNumber=@strSNFormat AND ParentID IS NULL
						   
					DECLARE cursor_Box CURSOR FOR 
					SELECT A.SerialNumber FROM mesPackage A WHERE ParentID= @PalletID
            
					OPEN cursor_Box 
					FETCH NEXT FROM cursor_Box INTO @BoxSN  
					WHILE @@FETCH_STATUS = 0
						BEGIN	
							DECLARE cursor_FG CURSOR FOR 
							SELECT A.ID FROM mesUnit A INNER JOIN mesUnitDetail B ON A.ID=B.UnitID
							INNER JOIN mesPackage C ON B.InmostPackageID=C.ID AND C.StatusID=1
							WHERE C.SerialNumber=@BoxSN
            				-- 回退FG
							OPEN cursor_FG 
							FETCH NEXT FROM cursor_FG INTO @FGUnitID  
							WHILE @@FETCH_STATUS = 0
								BEGIN	
									SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
									INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@FGUnitID
			
									SELECT @CurrStationTypeID=C.StationTypeID
										FROM mesUnit A 
											JOIN mesStation C ON A.StationID=C.ID  
										WHERE A.ID=@UnitdID			
			
									UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID,A.LastUpdate =GETDATE()  FROM mesUnit A
									WHERE A.ID=@UnitdID

									-- 处理 FG 需要回退的子料
									SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID,@PartFamilyID=p.PartFamilyID,@LineID=B.LineID
									FROM mesSerialNumber A 
									INNER JOIN mesUnit B ON A.UnitID=B.ID 
									inner join mesPart p on p.id=B.partID
									WHERE Value=@strSNFormat 
			
									--SELECT @RouteID=dbo.ufnRTEGetRouteID(@LineID,@PartID,@PartFamilyID,@ProductionOrderID)												
									select @PartFamilyID = PartFamilyID from mesPart where [ID] = @PartID
									select top 1 @RouteID = RouteID
									from mesRouteMap
									where
										(LineID = @LineID or LineID is NULL OR LineID=0) AND
										(PartID = @PartID or PartID is NULL OR PartID=0) AND
										(PartFamilyID = @PartFamilyID or PartFamilyID is NULL OR PartFamilyID=0) AND
										(ProductionOrderID = @ProductionOrderID or ProductionOrderID is NULL OR ProductionOrderID=0)
									order by ProductionOrderID desc,PartID desc, LineID desc, PartFamilyID desc
									IF ISNULL(@RouteID,'')=''
									BEGIN
										SET @strOutput='ERROR:can t find route ID.'
										SELECT @strOutput as ValStr1
										RETURN
									END
									--SELECT @ReWorkStationTypeID=Value FROM mesStationConfigSetting WHERE Name='ReWorkStationTypeID' AND StationID=@StationID		
									SELECT TOP 1 @StartSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@ReWorkStationTypeID AND RouteID=@RouteID
									SELECT TOP 1 @EndSEQ=SEQ  FROM mesRouteSEQ WHERE StationTypeID=@CurrStationTypeID	AND RouteID=@RouteID 			
			
									INSERT INTO #BOMStationTypeID	
									SELECT DISTINCT(stationTypeID) StationTypeID FROM mesRouteSEQ WHERE seq>=@StartSEQ AND seq<=@EndSEQ AND RouteID=@RouteID
			             
									UPDATE A SET A.StatusID=0,RemovedEmployeeID=@EmployeeID,RemovedStationID=@StationID,
										RemovedTime=GETDATE() FROM mesUnitComponent A
									WHERE A.UnitID=@UnitdID AND A.StatusID=1 and a.ChildPartID IN
									(
										SELECT PartID FROM mesProductStructure WHERE StationTypeID IN (SELECT StationTypeID from #BOMStationTypeID) 
											AND ParentPartID=@PartID				
									)
			
									delete #BOMStationTypeID

									--记录 FG History
									Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
									values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)	
																				
									FETCH NEXT FROM cursor_FG INTO @FGUnitID									
								END
							CLOSE cursor_FG 
							DEALLOCATE cursor_FG  			
											
							-- 回退UPC					
							DECLARE cursor_UPC CURSOR FOR 
							SELECT A.ID FROM mesUnit A 
							INNER JOIN mesSerialNumber D ON A.ID=D.UnitID
							INNER JOIN mesUnitDetail B ON D.Value=B.KitSerialNumber
							INNER JOIN mesPackage C ON B.InmostPackageID=C.ID AND C.StatusID=1
							WHERE C.SerialNumber=@BoxSN		
			
							OPEN cursor_UPC 
							FETCH NEXT FROM cursor_UPC INTO @UPCUnitID  
							WHILE @@FETCH_STATUS = 0
								BEGIN				
									-- 修改 UPC 状态
									UPDATE A SET A.UnitStateID=@UnitStateID,A.StationID =@StationID ,A.LastUpdate =GETDATE() FROM mesUnit A
									WHERE A.ID=@UPCUnitID					
					
									UPDATE mesUnitDetail SET reserved_06=KitSerialNumber WHERE UnitID=@UPCUnitID
									UPDATE mesUnitDetail SET KitSerialNumber=NULL WHERE UnitID=@UPCUnitID

									--记录 UPC History
									SELECT @UnitdID=A.UnitID,@PartID=B.PartID,@ProductionOrderID=B.ProductionOrderID FROM mesSerialNumber A 
									INNER JOIN mesUnit B ON A.UnitID=B.ID WHERE B.ID=@UPCUnitID

									Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
									values(@UnitdID,@UnitStateID,@EmployeeID,@StationID,GETDATE(),GETDATE(),@ProductionOrderID,@PartID,1,1)
																				
									FETCH NEXT FROM cursor_UPC INTO @UPCUnitID									
								END
							CLOSE cursor_UPC 
							DEALLOCATE cursor_UPC 			
			
							-- 更新中箱对应的 mesUnitDetail	 		
							UPDATE A SET A.reserved_07=InmostPackageID,InmostPackageID=NULL FROM mesUnitDetail A 
							INNER JOIN mesPackage B ON A.InmostPackageID=B.ID
							WHERE B.SerialNumber=@BoxSN AND B.StatusID=1
							-- 更新中箱
							UPDATE mesPackage SET StatusID=0,CurrentCount=NULL,
										ShipmentParentID = NULL,
										ShipmentInterID = NULL,
										ShipmentDetailID = NULL,
										ShipmentTime = NULL,
										ParentID = NULL 					 
							WHERE SerialNumber=@BoxSN and StatusID=1

							--记录中箱 History
							Insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
							VALUES(@BoxID,1,@StationID,@EmployeeID,GETDATE())				    
				    				    																				
							FETCH NEXT FROM cursor_Box INTO @BoxSN									
						END
					CLOSE cursor_Box 
					DEALLOCATE cursor_Box 		   
			  
					--回退栈板									
					UPDATE mesPackage SET StatusID=0,CurrentCount=NULL,ShipmentParentID = NULL,ShipmentInterID = NULL,ShipmentDetailID = NULL			
						WHERE ID=@PalletID	AND StatusID=1		  
			   			   			   								 			   
					--记录History
					Insert into mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
					VALUES(@PalletID,1,@StationID,@EmployeeID,GETDATE())
				END
				DROP TABLE #BOMStationTypeID
				SET @strOutput='1'
				Select  @strOutput as ValStr1
			COMMIT TRANSACTION Task
			END TRY
			BEGIN CATCH
				IF @@TRANCOUNT >0 
				BEGIN
					ROLLBACK TRANSACTION
				END
				--DROP TABLE #BOMStationTypeID
				SET @strOutput='1'
				SELECT @strOutput=ERROR_MESSAGE()
				Select  @strOutput as ValStr1
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


        public async Task<ReworkPrintDto> ReworkPrint(string S_SN,string S_ListSN, string S_Type, string S_URL) 
        {           
            TabVal TabVal_MSGERROR = new TabVal();
            TabVal_MSGERROR.ValStr1 = "1";

            ReworkPrintDto F_ReworkPrintDto = new ReworkPrintDto();
            F_ReworkPrintDto.MSG = TabVal_MSGERROR;

            try
            {
                string S_Sql = "";
                if (S_Type == "1" || S_Type == "2")
                {
                    S_Sql = @"SELECT B.PartID,B.ProductionOrderID,p.PartFamilyID
			                    FROM mesSerialNumber A 
			                    INNER JOIN mesUnit B ON A.UnitID=B.ID 
			                    inner join mesPart p on p.id=B.partID
			                WHERE Value='" + S_SN + "'";

                }

                if (S_Type == "3")
                {
                    S_Sql = @"SELECT TOP 1 C.PartID,C.ProductionOrderID,D.PartFamilyID
	                            FROM mesPackage A JOIN mesUnitDetail B ON A.ID = B.InmostPackageID
	                            JOIN mesUnit C ON B.UnitID = C.ID
	                            JOIN mesPart D ON C.PartID=D.ID
                            WHERE A.SerialNumber  = '" + S_SN + "'";
                }

                if (S_Type == "4")
                {
                    S_Sql = @" SELECT TOP 1 D.PartID,D.ProductionOrderID,E.PartFamilyID 
                            FROM mesPackage A JOIN mesPackage B ON A.ID=B.ParentID
                                JOIN mesUnitDetail C ON B.ID  =C.InmostPackageID
	                            JOIN mesUnit D ON C.UnitID=D.ID 
	                            JOIN mesPart E ON D.PartID=E.ID
                            WHERE A.SerialNumber='" + S_SN + "'";

                }

				DataTable DT_SN = Data_Table(S_Sql);
				string S_SNPartID = null;
				string S_PartFamilyID = null;
				string S_ProductionOrderID = null;
                if (DT_SN.Rows.Count > 0)
                {
                    S_SNPartID = DT_SN.Rows[0]["PartID"].ToString();
                    S_PartFamilyID = DT_SN.Rows[0]["PartFamilyID"].ToString();
                    S_ProductionOrderID = DT_SN.Rows[0]["ProductionOrderID"].ToString();
                }


                List<TabVal> List_Result = new List<TabVal>();
                if (S_ListSN=="")
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_20107, "0", List_Login, "PrintSN");             
                    TabVal_MSGERROR = List_Result[0];

                    F_ReworkPrintDto.MSG = TabVal_MSGERROR;
                    return F_ReworkPrintDto;
                }

                string S_StationTypeID = List_Login.StationTypeID.ToString();
                string S_LoginLineID = List_Login.LineID.ToString();

                string S_LabelPath = await Public_Repository.GetLabelName(S_StationTypeID,
                    S_PartFamilyID, S_SNPartID, S_ProductionOrderID, S_LoginLineID);
                if (S_LabelPath == "")
                {
                    List_Result = Public_Repository.List_ERROR_TabVal(P_MSG_Sys.MSG_Sys_20097, "0", List_Login, "");
                    TabVal_MSGERROR = List_Result[0];

                    F_ReworkPrintDto.MSG = TabVal_MSGERROR;
                    return F_ReworkPrintDto;
                }

                F_ReworkPrintDto.LabelPath= S_LabelPath;

                List<string> List_SN=new List<string>();
                string[] Array_SN = S_ListSN.Split(';');
                foreach (var item in Array_SN) 
                {
                    List_SN.Add(item);
                }                
                F_ReworkPrintDto.ListSN = List_SN;
				F_ReworkPrintDto.PartID = S_SNPartID;

            }
            catch (Exception ex)
            {
                TabVal_MSGERROR = Public_Repository.GetERROR(ex.Message, "0", List_Login, "");
                F_ReworkPrintDto.MSG = TabVal_MSGERROR;
                return F_ReworkPrintDto;
            }

            return F_ReworkPrintDto;
        }
    }
}