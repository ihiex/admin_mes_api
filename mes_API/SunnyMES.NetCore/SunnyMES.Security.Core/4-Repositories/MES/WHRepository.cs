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
using System.Net.NetworkInformation;
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
    public class WHRepository : BaseRepositoryReport<string>, IWHRepository
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


        public WHRepository()
        {
        }

        public WHRepository(IDbContextCore dbContext) : base(dbContext)
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
                    S_OperationType = List_IsCheckPOPN[0].OperationType;

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


        public async Task<List<IdDescription>> GetWHType() 
        {
            string S_Sql =
                @"
                SELECT 0 ID,'暂未确认NoConfirm' Description,'' Name
                UNION
                SELECT 1 ID,'已确认栈板纸ConfirmedOK' Description,'' Name
                UNION
                SELECT 2 ID,'已扫描出库ShipScanOK' Description,'' Name
                UNION
                SELECT 3 ID,'已确认出货Shipped' Description,'' Name
                UNION
                SELECT 4 ID,'已生成EDI' Description,'' Name
                UNION
                SELECT 5 ID,'已发送SAP_Recieved' Description,'' Name
                UNION
                SELECT 6 ID,'已关闭Closed' Description,'' Name
                UNION
                SELECT 999 ID,'ALL' Description,'' Name
                ";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<IdDescription>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();

        }

        public async Task<List<CO_WH_ShipmentNew>> GetShipmentNew(string S_Start, string S_End, string FStatus)
        {
            string S_Sql = @"select *,Web# WebJ,DN# DNJ,Delivery# DeliveryJ,
　　　　                    (case FStatus
　　　　	                    when 0     then    '暂未确认NoConfirm'
　　　　	                    when 1     then    '已确认栈板纸ConfirmedOK'
　　　　	                    when 2     then    '已扫描出库ShipScanOK'
　　　		                    when 3     then    '已确认出货Shipped'
			                    when 4     then    '已生成EDI'
			                    when 5     then    '已发送SAP_Recieved'
			                    when 6     then    '已关闭Closed'
						                    else '未定义' end
		                    ) MyStatus
                    FROM CO_WH_ShipmentNew
                  where FDate>='" + S_Start + "' and FDate<='" + S_End + "'";
            if (FStatus != "999")
            {
                S_Sql += " and FStatus=" + FStatus;
            }
           
            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<CO_WH_ShipmentNew>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<CO_WH_ShipmentEntryNew>> GetShipmentEntryNew(string S_FInterID)
        {
            string S_Sql = "select FDetailID,FMPNNO,FKPONO,FCTN from CO_WH_ShipmentEntryNew where FInterID='" + S_FInterID + "'";
            DataTable DT = Data_Table(S_Sql);
            int I_Count = 12 - DT.Rows.Count;

            S_Sql = "select FInterID,FDetailID,FMPNNO,FKPONO,FCTN,FProjectNO,FEntryID,FLineItem " + "\r\n" +
                     " ,FStatus, FOutSN, FPartNumberDesc, FQTY, FCrossWeight, FNetWeight," + "\r\n" +
                     " FCarrierNo, FCommercialInvoice, FProjectNO" + "\r\n" +
                     "from CO_WH_ShipmentEntryNew " + "\r\n" +
                     "  where FInterID='" + S_FInterID + "'";
            //+ "\r\n" +
            //"union select top " + I_Count.ToString() +
            //" '' FInterID, 100000000 + column_id FDetailID,'' FMPNNO,'' FKPONO,0 FCTN,'' FProjectNO,'' FEntryID,'' FLineItem " + "\r\n" +
            //",'0' FStatus,'0' FOutSN,'0' FPartNumberDesc,'0' FQTY,'0' FCrossWeight,'0' FNetWeight," + "\r\n" +
            //" '0' FCarrierNo,'0' FCommercialInvoice,'' FProjectNO" + "\r\n" +
            //"from sys.columns";

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<CO_WH_ShipmentEntryNew>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }

        public async Task<List<CO_WH_ShipmentReport>> GetShipmentReport(string S_Start, string S_End, string FStatus)
        {
            string S_Sql = @"select A.*, B.FEntryID, B.FDetailID, B.FKPONO, B.FMPNNO, B.FCTN FCTN2,B.FProjectNO FProjectNO2,
                                   B.FStatus FStatus2, B.FOutSN, B.FLineItem, B.FPartNumberDesc, B.FQTY, B.FCrossWeight,
                                   B.FNetWeight, B.FCarrierNo, B.FCommercialInvoice,
                                    (SELECT value FROM  [dbo].[F_Split](A.FShipNO,'#') WHERE id=1 )  FShipNO_L1  
                              from CO_WH_ShipmentNew A join CO_WH_ShipmentEntryNew B
                              on A.FInterID = B.FInterID 
                where A.FDate>='" + S_Start + "' and A.FDate<='" + S_End + "'";

            if (FStatus != "999")
            {
                S_Sql += " and A.FStatus=" + FStatus;
            }

            if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
            {

            }

            var v_Query = await DapperConnRead2.QueryAsync<CO_WH_ShipmentReport>(S_Sql, null, null, I_DBTimeout, null);
            return v_Query.AsList();
        }


        public async Task<string> SetShipmentNew
            (
                string S_FInterID,

                string S_HAWB, string S_PalletCount, string S_GrossWeight, string S_Project,
                string S_ShipDate, string S_PalletSeq, string S_EmptyCarton,
                string S_PalletSN, string S_ShipNO, string S_ShipID,

                string S_Regin, string S_ReferenceNO, string S_Country, string S_Carrier, string S_HubCode,
                string S_TruckNo, string S_ReturnAddress, string S_DeliveryStreetAddress, string S_DeliveryRegion,
                string S_DeliveryToName, string S_DeliveryCityName, string S_DeliveryCountry, string S_AdditionalDeliveryToName,
                string S_DeliveryPostalCode, string S_TelNo,

                string S_OceanContainerNumber, string S_Origin, string S_TotalVolume, string S_POE_COC, string S_TransportMethod, string S_POE,

                string S_CTRY, string S_SHA, string S_Sales, string S_WebJ, string S_UUI, string S_DNJ, string S_DeliveryJ,
                string S_Special, string S_SCAC, string S_OEMSpecificPO1, string S_OEMSpecificPO2,

                string S_Type
            )
        {
            string S_Result = "1";

            S_HAWB = S_HAWB ?? "";
            S_PalletCount = S_PalletCount ?? "";
            S_GrossWeight = S_GrossWeight ?? "";
            S_Project = S_Project ?? "";
            S_ShipDate = S_ShipDate ?? "";
            S_PalletSeq = S_PalletSeq ?? "";
            S_EmptyCarton = S_EmptyCarton ?? "";
            S_PalletSN = S_PalletSN ?? "";
            S_ShipNO = S_ShipNO ?? "";
            S_ShipID = S_ShipID ?? "";
            S_Regin = S_Regin ?? "";
            S_ReferenceNO = S_ReferenceNO ?? "";
            S_Country = S_Country ?? "";
            S_Carrier = S_Carrier ?? "";
            S_HubCode = S_HubCode ?? "";
            S_TruckNo = S_TruckNo ?? "";
            S_ReturnAddress = S_ReturnAddress ?? "";
            S_DeliveryStreetAddress = S_DeliveryStreetAddress ?? "";
            S_DeliveryRegion = S_DeliveryRegion ?? "";
            S_DeliveryToName = S_DeliveryToName ?? "";
            S_DeliveryCityName = S_DeliveryCityName ?? "";
            S_DeliveryCountry = S_DeliveryCountry ?? "";
            S_AdditionalDeliveryToName = S_AdditionalDeliveryToName ?? "";
            S_DeliveryPostalCode = S_DeliveryPostalCode ?? "";
            S_TelNo = S_TelNo ?? "";
            S_OceanContainerNumber = S_OceanContainerNumber ?? "";
            S_Origin = S_Origin ?? "";
            S_TotalVolume = S_TotalVolume ?? "";
            S_POE_COC = S_POE_COC ?? "";
            S_TransportMethod = S_TransportMethod ?? "";
            S_POE = S_POE ?? "";
            S_CTRY = S_CTRY ?? "";
            S_SHA = S_SHA ?? "";
            S_Sales = S_Sales ?? "";
            S_WebJ = S_WebJ ?? "";
            S_UUI = S_UUI ?? "";
            S_DNJ = S_DNJ ?? "";
            S_DeliveryJ = S_DeliveryJ ?? "";
            S_Special = S_Special ?? "";
            S_SCAC = S_SCAC ?? "";
            S_OEMSpecificPO1 = S_OEMSpecificPO1 ?? "";
            S_OEMSpecificPO2 = S_OEMSpecificPO2 ?? "";

            if (S_HAWB == "")
            {
                S_Result = "HAWB Can't be empty";
                return S_Result;
            }
            if (S_PalletCount == "")
            {
                S_Result = "Pallet Count Can't be empty";
                return S_Result;
            }
            if (S_GrossWeight == "")
            {
                S_Result = "GrossWeight  Can't be empty";
                return S_Result;
            }
            if (S_Project == "")
            {
                S_Result = "Project  Can't be empty";
                return S_Result;
            }
            if (S_ShipDate == "")
            {
                S_Result = "ShipDate  Can't be empty";
                return S_Result;
            }
            if (S_PalletSeq == "")
            {
                S_Result = "PalletSeq  Can't be empty";
                return S_Result;
            }
            if (S_EmptyCarton == "")
            {
                S_Result = "EmptyCarton  Can't be empty";
                return S_Result;
            }

            if (S_ShipNO == "")
            {
                S_Result = "ShipNO  Can't be empty";
                return S_Result;
            }
            if (S_ShipID == "")
            {
                S_Result = "ShipID   Can't be empty";
                return S_Result;
            }

            if (S_Regin == "")
            {
                S_Result = "Regin    Can't be empty";
                return S_Result;
            }
            if (S_ReferenceNO == "")
            {
                S_Result = "ReferenceNO Can't be empty";
                return S_Result;
            }
            if (S_Country == "")
            {
                S_Result = "Country Can't be empty";
                return S_Result;
            }
            if (S_Carrier == "")
            {
                S_Result = "Carrier Can't be empty";
                return S_Result;
            }
            if (S_HubCode == "")
            {
                S_Result = "HubCode Can't be empty";
                return S_Result;
            }
            if (S_TruckNo == "")
            {
                S_Result = "TruckNo Can't be empty";
                return S_Result;
            }

            if (S_OceanContainerNumber == "")
            {
                S_Result = "OceanContainerNumber Can't be empty";
                return S_Result;
            }
            if (S_Origin == "")
            {
                S_Result = "Origin  Can't be empty";
                return S_Result;
            }
            if (S_TotalVolume == "")
            {
                S_Result = "TotalVolume  Can't be empty";
                return S_Result;
            }
            if (S_POE_COC == "")
            {
                S_Result = "POE_COC  Can't be empty";
                return S_Result;
            }
            if (S_TransportMethod == "")
            {
                S_Result = "TransportMethod  Can't be empty";
                return S_Result;
            }
            if (S_POE == "")
            {
                S_Result = "POE  Can't be empty";
                return S_Result;
            }

            string S_Sql =
                @"
                DECLARE 
	            @FInterID                     int='" + S_FInterID + @"',
	            @S_HAWB						  nvarchar(200)='"+ S_HAWB + @"',
	            @S_PalletCount				  nvarchar(200)='"+ S_PalletCount + @"',
	            @S_GrossWeight				  nvarchar(200)='"+ S_GrossWeight + @"',
	            @S_Project					  nvarchar(200)='"+ S_Project + @"',

	            @S_ShipDate					  nvarchar(200)='"+ S_ShipDate + @"',
	            @S_PalletSeq				  nvarchar(200)='"+ S_PalletSeq + @"',
	            @S_EmptyCarton				  nvarchar(200)='"+ S_EmptyCarton + @"',

	            @S_PalletSN					  nvarchar(200)='"+ S_PalletSN + @"',
	            @S_ShipNO					  nvarchar(200)='"+ S_ShipNO + @"',
	            @S_ShipID					  nvarchar(200)='"+ S_ShipID + @"',

	            @S_Regin					  nvarchar(200)='"+ S_Regin + @"',
	            @S_ReferenceNO				  nvarchar(200)='"+ S_ReferenceNO + @"',
	            @S_Country					  nvarchar(200)='"+ S_Country + @"',
	            @S_Carrier					  nvarchar(200)='"+ S_Carrier + @"',
	            @S_HubCode					  nvarchar(200)='"+ S_HubCode + @"',
	            @S_TruckNo					  nvarchar(200)='"+ S_TruckNo + @"',
	            @S_ReturnAddress              nvarchar(200)='"+ S_ReturnAddress + @"',
	            @S_DeliveryStreetAddress      nvarchar(200)='"+ S_DeliveryStreetAddress + @"',
	            @S_DeliveryRegion             nvarchar(200)='"+ S_DeliveryRegion + @"',
	            @S_DeliveryToName             nvarchar(200)='"+ S_DeliveryToName + @"',
	            @S_DeliveryCityName           nvarchar(200)='"+ S_DeliveryCityName + @"',
	            @S_DeliveryCountry            nvarchar(200)='"+ S_DeliveryCountry + @"',
	            @S_AdditionalDeliveryToName   nvarchar(200)='"+ S_AdditionalDeliveryToName + @"',
	            @S_DeliveryPostalCode         nvarchar(200)='"+ S_DeliveryPostalCode + @"',
	            @S_TelNo                      nvarchar(200)='"+ S_TelNo + @"',

	            @S_OceanContainerNumber       nvarchar(200)='"+ S_OceanContainerNumber + @"',
	            @S_Origin                     nvarchar(200)='"+ S_Origin + @"',
	            @S_TotalVolume                nvarchar(200)='"+ S_TotalVolume + @"',
	            @S_POE_COC                    nvarchar(200)='"+ S_POE_COC + @"',
	            @S_TransportMethod            nvarchar(200)='"+ S_TransportMethod + @"',
	            @S_POE                        nvarchar(200)='"+ S_POE + @"',
	                
                @S_CTRY                       nvarchar(200)='" + S_CTRY + @"',
                @S_SHA                        nvarchar(200)='" + S_SHA + @"',
                @S_Sales                      nvarchar(200)='" + S_Sales + @"',
                @S_WebJ                       nvarchar(200)='" + S_WebJ + @"',
                @S_UUI                        nvarchar(200)='" + S_UUI + @"',
                @S_DNJ                        nvarchar(200)='" + S_DNJ + @"',
                @S_DeliveryJ                  nvarchar(200)='" + S_DeliveryJ + @"',
                @S_Special                    nvarchar(200)='" + S_Special + @"',
                @S_SCAC                       nvarchar(200)='" + S_SCAC + @"',
                @S_OEMSpecificPO1             nvarchar(200)='" + S_OEMSpecificPO1 + @"',
                @S_OEMSpecificPO2             nvarchar(200)='" + S_OEMSpecificPO2 + @"',

	            @Type                         nvarchar(10)='" + S_Type + @"' 	

	            declare @strOutput  NVARCHAR(500)
	            set @strOutput='1'
	            BEGIN TRY		
		            if @Type='New' 
		            BEGIN
			            if exists(select * from CO_WH_ShipmentNew where FBillNO=@S_HAWB+'-'+@S_PalletSeq)
			            BEGIN
				            set @strOutput=@S_HAWB+'-'+@S_PalletSeq+' is exists!'
				            Select  @strOutput as ValStr1
				            return
			            END 
			
			            insert into CO_WH_ShipmentNew
			            (
				            -- FInterID -- this column value is auto-generated
				                FBillNO,
					            HAWB        ,
					            FPalletCount ,
					            FGrossWeight ,
					            FProjectNO     ,

					            FDate    ,
					            FPalletSeq   ,
					            FEmptyCarton ,

					            PalletSN    ,
					            FShipNO      ,
					            ShipID      ,

					            Region       ,
					            ReferenceNO ,
					            Country     ,
					            Carrier     ,
					            HubCode     ,
					            TruckNo     ,
					            ReturnAddress           ,
					            DeliveryStreetAddress   ,
					            DeliveryRegion          ,
					            DeliveryToName          ,
					            DeliveryCityName        ,
					            DeliveryCountry         ,
					            AdditionalDeliveryToName,
					            DeliveryPostalCode      ,
					            TelNo                   ,

					            MAWB_OceanContainerNumber,
					            Origin                  ,
					            TotalVolume             ,
					            POE_COC                 ,
					            TransportMethod         ,
					            POE                     ,

                                CTRY                    ,
                                SHA                     ,
                                Sales                   ,
                                Web#                    ,
                                UUI                     ,
                                DN#                     ,
                                Delivery#               , 
                                Special                 ,
                                SCAC                    ,
                                OEMSpecificPO1          ,
                                OEMSpecificPO2          ,
					                                    
					            FStatus

			            )
			            VALUES
			            (
				            @S_HAWB+'-'+@S_PalletSeq,
				            @S_HAWB        ,
				            @S_PalletCount ,
				            @S_GrossWeight ,
				            @S_Project     ,
			
				            @S_ShipDate    ,
				            @S_PalletSeq   ,
				            @S_EmptyCarton ,
			
				            @S_PalletSN    ,
				            @S_ShipNO      ,
				            @S_ShipID      ,
			
				            @S_Regin       ,
				            @S_ReferenceNO ,
				            @S_Country     ,
				            @S_Carrier     ,
				            @S_HubCode     ,
				            @S_TruckNo     ,
				            @S_ReturnAddress           ,
				            @S_DeliveryStreetAddress   ,
				            @S_DeliveryRegion          ,
				            @S_DeliveryToName          ,
				            @S_DeliveryCityName        ,
				            @S_DeliveryCountry         ,
				            @S_AdditionalDeliveryToName,
				            @S_DeliveryPostalCode      ,
				            @S_TelNo                   ,
			
				            @S_OceanContainerNumber    ,
				            @S_Origin                  ,
				            @S_TotalVolume             ,
				            @S_POE_COC                 ,
				            @S_TransportMethod         ,
				            @S_POE                     ,
								           
                            @S_CTRY                     ,
                            @S_SHA                      ,
                            @S_Sales                    ,
                            @S_WebJ                     ,
                            @S_UUI                      ,
                            @S_DNJ                      ,
                            @S_DeliveryJ                ,
                            @S_Special                  ,
                            @S_SCAC                     ,
                            @S_OEMSpecificPO1           ,
                            @S_OEMSpecificPO2           ,
                            0
			            )                               
		            END	
		            else if @Type='Mod'
		            BEGIN
			            if exists(select * from CO_WH_ShipmentNew where FBillNO=@S_HAWB+'-'+@S_PalletSeq and FInterID<>@FInterID)
			            BEGIN
				            set @strOutput=@S_HAWB+'-'+@S_PalletSeq+' is exists!'
				            Select  @strOutput as ValStr1
				            return
			            END 			
			
			            update CO_WH_ShipmentNew  set
				            HAWB							=@S_HAWB        ,
				            FPalletCount					=@S_PalletCount ,
				            FGrossWeight					=@S_GrossWeight ,
				            FProjectNO						=@S_Project     ,
			
				            FDate							=@S_ShipDate    ,
				            FPalletSeq						=@S_PalletSeq   ,
				            FEmptyCarton					=@S_EmptyCarton ,
			
				            PalletSN						=@S_PalletSN    ,
				            FShipNO							=@S_ShipNO      ,
				            ShipID							=@S_ShipID      ,
			
				            Region							=@S_Regin       ,
				            ReferenceNO						=@S_ReferenceNO ,
				            Country							=@S_Country     ,
				            Carrier							=@S_Carrier     ,
				            HubCode							=@S_HubCode     ,
				            TruckNo							=@S_TruckNo     ,
				            ReturnAddress					=@S_ReturnAddress           ,
				            DeliveryStreetAddress			=@S_DeliveryStreetAddress   ,
				            DeliveryRegion					=@S_DeliveryRegion          ,
				            DeliveryToName					=@S_DeliveryToName          ,
				            DeliveryCityName				=@S_DeliveryCityName        ,
				            DeliveryCountry					=@S_DeliveryCountry         ,
				            AdditionalDeliveryToName		=@S_AdditionalDeliveryToName,
				            DeliveryPostalCode				=@S_DeliveryPostalCode      ,
				            TelNo							=@S_TelNo                   ,
			
				            MAWB_OceanContainerNumber		=@S_OceanContainerNumber    ,
				            Origin							=@S_Origin                  ,
				            TotalVolume						=@S_TotalVolume             ,
				            POE_COC							=@S_POE_COC                 ,
				            TransportMethod					=@S_TransportMethod         ,
				            POE								=@S_POE                     ,

                            CTRY                            =@S_CTRY                   ,             
                            SHA                             =@S_SHA                    ,
                            Sales                           =@S_Sales                  ,
                            Web#                            =@S_WebJ                   ,
                            UUI                             =@S_UUI                    ,
                            DN#                             =@S_DNJ                    ,
                            Delivery#                       =@S_DeliveryJ              ,
                            Special                         =@S_Special                ,
                            SCAC                            =@S_SCAC                   ,
                            OEMSpecificPO1                  =@S_OEMSpecificPO1         ,
                            OEMSpecificPO2                  =@S_OEMSpecificPO2          


			            where FInterID=@FInterID				
		            END 
	
	            END TRY
	            BEGIN CATCH

		            set @strOutput=ERROR_MESSAGE()
	            END CATCH	
	            Select  @strOutput as ValStr1
                ";

            var v_Query =await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
            S_Result= v_Query.ToList().FirstOrDefault().ValStr1;
            return S_Result;
        }

        public async Task<string> UpdateFStatus(string FInterID_List, string FStatus)
        {
            string S_Result = "1";
            try
            {
                string S_Sql = "Update  CO_WH_ShipmentNew set FStatus='" + FStatus + "' where FInterID in(" + FInterID_List + ")";
                int I_Result = await DapperConnRead2.ExecuteAsync(S_Sql);
            }
            catch (Exception ex)
            {
                S_Result=ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> DelShipmentNew(string FInterID)
        {
            string S_Result = "1";
            try
            {
                string S_Sql =
                    @"
                    BEGIN TRY
                        declare @strOutput  NVARCHAR(500)='1'
                        declare @FStatus  int
                        select @FStatus=FStatus from CO_WH_ShipmentNew where FInterID
                        if  @FStatus<>1
                        begin
                            set  @strOutput='It has been confirmed that deletion is not allowed'
                            Select  @strOutput as ValStr1
                            return 
                        end

                        delete CO_WH_ShipmentNew  where FInterID='" + FInterID + @"'
                        delete CO_WH_ShipmentEntryNew  where FInterID='" + FInterID + @"'
	                END TRY
	                BEGIN CATCH

		                set @strOutput=ERROR_MESSAGE()
	                END CATCH	
	                Select  @strOutput as ValStr1
                    ";

                var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                S_Result = v_Query.ToList().FirstOrDefault().ValStr1;               
            }
            catch (Exception ex)
            {
                S_Result=ex.ToString();
            }
            return S_Result;
        }


        public async Task<string> SetShipmentEntryNew
            (
                string S_FDetailID, string S_FInterID, string S_FEntryID, string S_FCarrierNo, string S_FCommercialInvoice,
                string S_FCrossWeight, string S_FCTN, string S_FKPONO, string S_FLineItem, string S_FMPNNO, string S_FNetWeight,
                string S_FOutSN, string S_FPartNumberDesc, string S_FQTY, string S_FStatus, string S_FProjectNO,

                string S_Type
            )
        {
            string S_Result = "1";

            S_FDetailID = S_FDetailID ?? "";
            S_FEntryID = S_FEntryID ?? "";
            S_FInterID = S_FInterID ?? "";
            S_FCarrierNo = S_FCarrierNo ?? "";
            S_FCommercialInvoice = S_FCommercialInvoice ?? "";
            S_FCrossWeight = S_FCrossWeight ?? "";
            S_FCTN = S_FCTN ?? "";
            S_FKPONO = S_FKPONO ?? "";
            S_FLineItem = S_FLineItem ?? "";
            S_FMPNNO = S_FMPNNO ?? "";
            S_FNetWeight = S_FNetWeight ?? "";
            S_FOutSN = S_FOutSN ?? "";
            S_FPartNumberDesc = S_FPartNumberDesc ?? "";
            S_FQTY = S_FQTY ?? "";
            S_FStatus = S_FStatus ?? "";
            S_FProjectNO = S_FProjectNO ?? "";

            if (S_FEntryID == "")
            {
                S_Result="NO. Can't be empty";
                return S_Result;
            }

            if (S_FCarrierNo == "")
            {
                S_Result = "CarrierNo Can't be empty";
                return S_Result;
            }
            if (S_FCommercialInvoice == "")
            {
                S_Result = "CommercialInvoice Can't be empty";
                return S_Result;
            }

            if (S_FCrossWeight == "")
            {
                S_Result = "FCrossWeight Can't be empty";
                return S_Result;
            }
            if (S_FCTN == "")
            {
                S_Result = "FCTN Can't be empty";
                return S_Result;
            }
            if (S_FKPONO == "")
            {
                S_Result = "PO Can't be empty";
                return S_Result;
            }

            if (S_FLineItem == "")
            {
                S_Result = "LineItem Can't be empty";
                return S_Result;
            }
            if (S_FMPNNO == "")
            {
                S_Result = "MPN Can't be empty";
                return S_Result;
            }
            if (S_FNetWeight == "")
            {
                S_Result = "NetWeight Can't be empty";
                return S_Result;
            }
            if (S_FPartNumberDesc == "")
            {
                S_Result = "PartNumberDesc. Can't be empty";
                return S_Result;
            }
            if (S_FQTY == "")
            {
                S_Result = "QTY Can't be empty";
                return S_Result;
            }
            if (S_FProjectNO == "")
            {
                S_Result = "ProjectNO Can't be empty";
                return S_Result;
            }

            try
            {
                string S_Sql =
                    @"
                DECLARE
	            @FDetailID				int='" + S_FDetailID + @"',
	            @FEntryID				int='" + S_FEntryID + @"',
	            @FInterID				int='" + S_FInterID + @"',
	            @FCarrierNo				nvarchar(100)='" + S_FCarrierNo + @"',
	            @FCommercialInvoice		nvarchar(100)='" + S_FCommercialInvoice + @"',
	            @FCrossWeight			numeric(18,2)='" + S_FCrossWeight + @"',
	            @FCTN					int='" + S_FCTN + @"',
	            @FKPONO					nvarchar(100)='" + S_FKPONO + @"',
	            @FLineItem				nvarchar(100)='" + S_FLineItem + @"',
	            @FMPNNO					nvarchar(100)='" + S_FMPNNO + @"',
	            @FNetWeight				numeric(18,2)='" + S_FNetWeight + @"',
	            @FOutSN					int='" + S_FOutSN + @"',
	            @FPartNumberDesc		nvarchar(100)='" + S_FPartNumberDesc + @"',
	            @FQTY					int='" + S_FQTY + @"',
	            @FStatus				int='" + S_FStatus + @"',
	            @FProjectNO             nvarchar(100)='" + S_FProjectNO + @"',
	
	            @Type                   nvarchar(10)='" + S_Type + @"'

	            declare @strOutput  NVARCHAR(500)
	            set @strOutput='1'
	            BEGIN TRY		
		            if @Type='New' 
		            BEGIN	
			            if exists(select * from CO_WH_ShipmentEntryNew where FInterID=@FInterID and FEntryID=@FEntryID)
			            BEGIN
				            set @strOutput='Data is exists!'
				            Select  @strOutput as ValStr1
				            return
			            END 			
			
			            if not exists(select *  from CO_WH_ShipmentEntryNew WHERE FInterID=@FInterID)
			            BEGIN
				            set @FEntryID='1'
			            END		
			            else BEGIN
			                 select @FEntryID=max(FEntryID)+1  from CO_WH_ShipmentEntryNew WHERE FInterID=@FInterID
			            END
			
			            insert into CO_WH_ShipmentEntryNew
			            (
				            --FDetailID
				            FInterID,
				            FEntryID,				
				            FCarrierNo,
				            FCommercialInvoice,
				            FCrossWeight,
				            FCTN,

				            FKPONO,
				            FLineItem,
				            FMPNNO,
				            FNetWeight,
				            --FOutSN,
				            FPartNumberDesc,
				            FQTY,
				            FStatus,
				            FProjectNO				
			            )
			            VALUES
			            (
				            @FInterID,
				            @FEntryID,				
				            @FCarrierNo,
				            @FCommercialInvoice,
				            @FCrossWeight,
				            @FCTN,

				            @FKPONO,
				            @FLineItem,
				            @FMPNNO,
				            @FNetWeight,
				            --@FOutSN,
				            @FPartNumberDesc,
				            @FQTY,
				            @FStatus,
				            @FProjectNO			
			            )
		            END	
		            else if @Type='Mod'
		            BEGIN
			            if exists(select * from CO_WH_ShipmentEntryNew where FInterID=@FDetailID and FEntryID=@FEntryID and FDetailID<>@FDetailID)
			            BEGIN
				            set @strOutput='Data is exists!'
				            select @strOutput
				            return
			            END 			
			
			            update CO_WH_ShipmentEntryNew  set
			
				            --FDetailID=@FDetailID
				            FEntryID=@FEntryID,
				            FInterID=@FInterID,
				            FCarrierNo=@FCarrierNo,
				            FCommercialInvoice=@FCommercialInvoice,
				            FCrossWeight=@FCrossWeight,
				            FCTN=@FCTN,

				            FKPONO=@FKPONO,
				            FLineItem=@FLineItem,
				            FMPNNO=@FMPNNO,
				            FNetWeight=@FNetWeight,
				            --FOutSN=@FOutSN,
				            FPartNumberDesc=@FPartNumberDesc,
				            FQTY=@FQTY,
				            --FStatus=@FStatus
				            FProjectNO=@FProjectNO
                   
			            where FInterID=@FInterID and FDetailID=@FDetailID			
		            END 
	                else if @Type='Del'
	                BEGIN
	    	            delete CO_WH_ShipmentEntryNew where FInterID=@FInterID and FDetailID=@FDetailID	
	                END
		
	                declare @FCTNCount int
	                select @FCTNCount=sum(FCTN) from CO_WH_ShipmentEntryNew WHERE FInterID=@FInterID
	                update CO_WH_ShipmentNew set FCTN = @FCTNCount where FInterID=@FInterID
	
	            END TRY
	            BEGIN CATCH
		            set @strOutput=ERROR_MESSAGE()
	            END CATCH	
	            Select  @strOutput as ValStr1
                ";
                var v_Query = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                S_Result = v_Query.ToList().FirstOrDefault().ValStr1;
            }
            catch ( Exception ex ) 
            {
                S_Result=ex.ToString();
            }
            return S_Result;
        }

        public async Task<string> DelShipmentEntryNew(string S_FDetailID,string S_FInterID)
        {
            string S_Result = "1";
            S_Result = await SetShipmentEntryNew(S_FDetailID, "0", S_FInterID, "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "Del");
            return S_Result;
        }


    }
}