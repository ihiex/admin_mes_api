using API_MSG;
using Dapper;
using Microsoft.EntityFrameworkCore;
using NPinyin;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Org.BouncyCastle.Utilities.Collections;
using Quartz.Impl.Triggers;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dapper;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.MES.Query.ShipMent;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.Models;
using SunnyMES.Security.Models.Public;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NPOI.HSSF.Util.HSSFColor;

namespace SunnyMES.Security.Repositories
{
    public class DataCommitRepository : BaseRepositoryReport<string>, IDataCommitRepository
    {
        public DataCommitRepository()
        {

        }

        MSG_Public P_MSG_Public;
        PublicMiniRepository v_Public_Repository;

        public DataCommitRepository(IDbContextCore dbContext, int I_Language) : base(dbContext)
        {
            P_MSG_Public = new MSG_Public(I_Language);
            v_Public_Repository = new PublicMiniRepository();
        }


        public async Task<string> SubmitData_UnitMod(
              List<mesUnit> List_mesUnit
          )
        {
            string S_Result = "";
            string S_Sql = "";

            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                if (List_mesUnit != null)
                {
                    for (int i = 0; i < List_mesUnit.Count; i++)
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit = List_mesUnit[i];

                        S_Sql += " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                ",PanelID='" + v_mesUnit.PanelID + "'\r\n" +
                                ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                ",LastUpdate=getdate() \r\n" +
                                " where ID='" + v_mesUnit.ID + "' \r\n";


                    }
                }

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                if (Tuple_Result.Item1 == false)
                {
                    S_Result = "ERROR:" + Tuple_Result.Item2;
                    return S_Result;
                }

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }



        /// <summary>
        /// 一般过站
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesHistory"></param>
        /// <param name="List_mesUnitDefect"></param>
        /// <returns></returns>
        public async Task<string> SubmitData_UnitMod_HistoryAdd_DefectAdd(
                List<mesUnit> List_mesUnit,
                List<mesHistory> List_mesHistory,
                List<mesUnitDefect> List_mesUnitDefect
            )
        {
            string S_Result = "";
            string S_Sql = "";

            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                if (List_mesUnit != null)
                {
                    for (int i = 0; i < List_mesUnit.Count; i++)
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit = List_mesUnit[i];

                        S_Sql += " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                ",PanelID='" + v_mesUnit.PanelID + "'\r\n" +
                                ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                ",LastUpdate=getdate() \r\n" +
                                " where ID='" + v_mesUnit.ID + "' \r\n";

                        //NG产品解绑关联的治具
                        S_Sql +=
                        @"IF  EXISTS(SELECT 1 FROM mesSerialNumber WHERE UnitId='" + v_mesUnit.ID + @"')
		                BEGIN                    			                		               
		                    SELECT F.reserved_01 INTO #TempRelease FROM mesSerialNumber(NOLOCK) A 
		                        INNER JOIN mesUnitComponent(NOLOCK) B ON A.UnitID=B.UnitID
		                        INNER JOIN mesPart C ON B.ChildPartID=C.ID
		                        INNER JOIN mesPartDetail D ON C.ID=D.PartID
		                        INNER JOIN luPartDetailDef E ON D.PartDetailDefID=E.ID
		                        INNER JOIN mesUnitDetail(NOLOCK) F ON B.ChildUnitID=F.UnitID
		                    WHERE E.Description='ScanType' AND D.Content='4' AND A.UnitId='" + v_mesUnit.ID + @"'

		                    UPDATE M SET M.RuningCapacityQuantity=0,StatusID=1 FROM mesMachine M
		                    WHERE EXISTS(SELECT 1 FROM #TempRelease F WHERE M.SN=F.reserved_01)

		                    UPDATE S SET reserved_03 = '2' FROM mesUnitDetail S WHERE reserved_03 = '1' AND EXISTS
		                    (SELECT 1 FROM #TempRelease F WHERE S.reserved_01=F.reserved_01)

		                    drop table #TempRelease
                        END" + "\r\n";
                    }
                }


                if (List_mesHistory != null)
                {
                    for (int i = 0; i < List_mesHistory.Count; i++)
                    {
                        mesHistory v_mesHistory = new mesHistory();
                        v_mesHistory = List_mesHistory[i];

                        S_Sql += " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                            "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                            "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                            "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                            "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                            "'" + v_mesHistory.StationID + "'," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                            "'" + v_mesHistory.PartID + "'," + "\r\n" +
                            "'1'," + "\r\n" +
                            "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                            ")" + "\r\n";

                    }
                }

                if (List_mesUnitDefect != null)
                {
                    if (List_mesUnitDefect.Count > 0)
                    {
                        S_Sql += "declare @MaxDefID int" + "\r\n";
                    }
                    int I_Qyt = 0;


                    for (int i = 0; i < List_mesUnitDefect.Count; i++)
                    {
                        mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                        v_mesUnitDefect = List_mesUnitDefect[i];
                        I_Qyt = i + 1;

                        S_Sql +=
                                "select @MaxDefID=ISNULL(Max(ID),0)+" + I_Qyt + " from mesUnitDefect " + "\r\n" +
                                "INSERT INTO mesUnitDefect(ID, UnitID, DefectID, StationID, EmployeeID) Values(" + "\r\n" +
                                "@MaxDefID," + "\r\n" +
                                "'" + v_mesUnitDefect.UnitID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.DefectID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.StationID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.EmployeeID + "'" + "\r\n" +
                                ")" + "\r\n";
                    }
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                if (Tuple_Result.Item1 == false)
                {
                    S_Result = "ERROR:" + Tuple_Result.Item2;
                    return S_Result;
                }

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }

        /// <summary>
        ///  TT 过站
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesHistory"></param>
        /// <param name="S_SNType"></param>
        /// <param name="S_MachineSN"></param>
        /// <param name="S_CardID"></param>
        /// <param name="S_IsCheckCardID"></param>
        /// <param name="Login_List"></param>
        /// <returns></returns>
        public async Task<string> SubmitData_UnitMod_HistoryAdd_TT(
                List<mesUnit> List_mesUnit,
                List<mesHistory> List_mesHistory,
                string S_SNType,   //扫码类型 1:条码SN 2:BOX 3:MachineBox (未配置默认1)  
                string S_MachineSN,
                string S_CardID, string S_IsCheckCardID,
                LoginList Login_List
            )
        {
            string S_Result = "";
            string S_Sql = "";

            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                if (List_mesUnit != null)
                {
                    for (int i = 0; i < List_mesUnit.Count; i++)
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit = List_mesUnit[i];

                        S_Sql += " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                // ",PanelID='" + v_mesUnit.PanelID + "'\r\n" +
                                ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                ",LastUpdate=getdate() \r\n" +
                                " where ID='" + v_mesUnit.ID + "' \r\n";

                        S_Sql += @"Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,    
                                            ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)    
                                        Values('" + v_mesUnit.ID + @"','" +
                                        v_mesUnit.UnitStateID + @"','" +
                                        v_mesUnit.EmployeeID + @"','" +
                                        v_mesUnit.StationID + @"',GETDATE(),GETDATE(),'" + v_mesUnit.ProductionOrderID + @"','" +
                                        v_mesUnit.PartID + @"',1,1    
                                         )";


                        //更新盒子中的SN信息    
                        //if (S_SNType == "2" || S_SNType == "3")
                        {
                            S_Sql += " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                    ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                    ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                    //   ",PanelID='" + v_mesUnit.PanelID + "'\r\n" +
                                    ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                    ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                    ",LastUpdate=getdate() \r\n" +
                                    " where PanelID='" + v_mesUnit.ID + "' \r\n";

                            S_Sql += @"Insert into mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,    
                                            ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)    
                                       SELECT ID,'" + v_mesUnit.UnitStateID + @"','" +
                                            v_mesUnit.EmployeeID + @"','" +
                                            v_mesUnit.StationID + @"',GETDATE(),GETDATE(),'" + v_mesUnit.ProductionOrderID + @"','" +
                                            v_mesUnit.PartID + @"',1,1    
                                          FROM mesUnit WHERE PanelID='" + v_mesUnit.ID + "'" + "\r\n";
                        }


                        //--解除BOXID(存在就解除)
                        //string S_reserved_06 = v_mesUnit.ID.ToString() + ";";
                        S_Sql += "   DECLARE   @Panel_ID VARCHAR(100)" + "\r\n" +
                                "IF EXISTS (SELECT 1 FROM mesUnit WHERE ID='" + v_mesUnit.ID + "' AND ISNULL(PanelID,'')<>'') " + "\r\n" +
                                "Begin" + "\r\n" +

                                "   Select @Panel_ID=PanelID from mesUnit WHERE ID='" + v_mesUnit.ID + "'" + "\r\n" +
                                "   Update mesUnitDetail set reserved_06=ISNULL(reserved_06,'')+';'+@Panel_ID" +
                                "  where  UnitID= '" + v_mesUnit.ID + "'" + "\r\n" +
                                "End" + "\r\n" +

                            "UPDATE mesUnit SET PanelID=NULL  WHERE ID='" + v_mesUnit.ID + "' AND ISNULL(PanelID,'')<>'' " + "\r\n";

                        // 2022-10-28  增加
                        if (Login_List.IsTTBoxUnpack == "1")
                        {
                            //if (S_SNType == "2" || S_SNType == "3")
                            {
                                string S_reserved_06 = v_mesUnit.ID.ToString() + ";";
                                S_Sql +=
                                        " Update mesUnitDetail set reserved_06=ISNULL(reserved_06,'')+'" + S_reserved_06 +
                                        "' where  UnitID in " + "\r\n" +
                                        "(" + "\r\n" +
                                        "     select ID from mesUnit where PanelID = '" + v_mesUnit.ID + "'\r\n" +
                                        ")" + "\r\n" +

                                        " update mesUnit SET PanelID=NULL where PanelID='" + v_mesUnit.ID + "'\r\n";
                                ;
                            }
                        }

                        if (S_SNType == "1")
                        {
                            S_Sql += "DECLARE @ChildSNCount int" + "\r\n" +
                                      // "DECLARE @PanelID_C int" + "\r\n" +
                                      "DECLARE @MachineSN NVARCHAR(50)" + "\r\n" +
                                     // "SELECT @PanelID_C=PanelId FROM mesUnit Where ID='" + v_mesUnit.ID + "' " + "\r\n" +
                                     "SELECT @ChildSNCount=Count(*) FROM mesUnit A " + "\r\n" +
                                     "    WHERE A.PanelID=@Panel_ID " + "\r\n" +
                                     "SELECT @MachineSN=reserved_01 FROM mesUnitDetail WHERE UnitID=@Panel_ID" + "\r\n" +

                                     "if @ChildSNCount<=1" + "\r\n" +
                                    " Begin" + "\r\n" +
                                    "    UPDATE mesMachine SET StatusID = 1 WHERE SN= @MachineSN" + "\r\n" +
                                    " End" + "\r\n"
                                     ;
                        }

                        if (S_IsCheckCardID == "1" && S_CardID != "")
                        {
                            S_Sql += "INSERT INTO dbo.mesUnitStatusHistory(UnitID, UnitStatusID," + "\r\n" +
                                    " UnitStatusReasonID, Comment, EmployeeID, StationID, Time, LooperCount)" + "\r\n" +
                                  "VALUES('" + v_mesUnit.ID + "'," + "\r\n" +
                                  "1," + "\r\n" +
                                  "0, " + "\r\n" +
                                  "'" + S_CardID + "', " + "\r\n" +
                                  "'" + v_mesUnit.EmployeeID + "', " + "\r\n" +
                                  "'" + v_mesUnit.StationID + "', " + "\r\n" +
                                  "GETDATE(), " + "\r\n" +
                                  "0" + "\r\n" +
                                  ")" + "\r\n";


                            if (S_SNType == "2" || S_SNType == "3")
                            {
                                S_Sql += "INSERT INTO dbo.mesUnitStatusHistory(UnitID, UnitStatusID," + "\r\n" +
                                        " UnitStatusReasonID, Comment, EmployeeID, StationID, Time, LooperCount)" + "\r\n" +
                                        " SELECT ID,1,0,@tmpCardId,@EmployeeID,@StationID,GETDATE(),0 " + "\r\n" +
                                        " FROM mesUnit WHERE PanelID = '" + v_mesUnit.ID + "'" + "\r\n";
                            }
                        }

                        if (S_SNType == "3")
                        {
                            string S_ModMachine_Sql = await
                                v_Public_Repository.MesModMachineBySNStationTypeID_Sql(S_MachineSN, Convert.ToInt32(Login_List.StationTypeID));

                            if (S_ModMachine_Sql.Length >= 5)
                            {
                                if (S_ModMachine_Sql.Substring(0, 5) != "ERROR")
                                {
                                    S_Sql += S_ModMachine_Sql + "\r\n";
                                }
                                else
                                {
                                    S_Result = "ERROR:" + S_ModMachine_Sql;
                                    return S_Result;
                                }
                            }
                        }
                    }
                }

                //if (List_mesHistory != null)
                //{
                //    for (int i = 0; i < List_mesHistory.Count; i++)
                //    {
                //        mesHistory v_mesHistory = new mesHistory();
                //        v_mesHistory = List_mesHistory[i];

                //        S_Sql += " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                //            "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                //            "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                //            "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                //            "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                //            "'" + v_mesHistory.StationID + "'," + "\r\n" +
                //            "getdate()," + "\r\n" +
                //            "getdate()," + "\r\n" +
                //            "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                //            "'" + v_mesHistory.PartID + "'," + "\r\n" +
                //            "'1'," + "\r\n" +
                //            "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                //            ")" + "\r\n";

                //    }
                //}

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                if (Tuple_Result.Item1 == false)
                {
                    S_Result = "ERROR:" + Tuple_Result.Item2;
                    return S_Result;
                }

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }

        /// <summary>
        /// UnitAdd UnitDetailAdd SNAdd HistoryAdd
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesmesUnitDetail"></param>
        /// <param name="List_mesSerialNumber"></param>
        /// <param name="List_mesHistory"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd(
            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber,
            List<mesHistory> List_mesHistory
        )
        {
            string S_Result = "";
            string S_Sql = " declare @UnitID int " + " \r\n";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;
            int I_MaxID = 0;

            try
            {
                for (int i = 0; i < List_mesUnit.Count; i++)
                {
                    mesUnit v_mesUnit = List_mesUnit[i];
                    mesUnitDetail v_mesUnitDetail = List_mesmesUnitDetail[i];
                    mesSerialNumber v_mesSerialNumber = List_mesSerialNumber[i];
                    mesHistory v_mesHistory = List_mesHistory[i];

                    //string S_Sql_ID = " select max(ID)+1 as Valint1 from mesUnit";
                    //var v_MaxID = await DapperConnRead2.QueryAsync<TabVal>(S_Sql_ID, null, null, I_DBTimeout, null);
                    //I_MaxID = Convert.ToInt32(v_MaxID.AsList()[0].Valint1);

                    S_Sql += //" declare @UnitID int "  + " \r\n" +

                        " select @UnitID=isnull(max(ID),0)+1   from mesUnit" + " \r\n" +

                           " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                           "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                           "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                          "VALUES (@UnitID" + "\r\n" +
                                 ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                 ",1" + "\r\n" +
                                 ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                 ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                 ",GETDATE()" + "\r\n" +
                                 ",GETDATE()" + "\r\n" +
                                 ",0" + "\r\n" +
                                 ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                 ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                 ",0" + "\r\n" +
                                 ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                 ",1)" + "\r\n" +

                          " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                          " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                        "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                        v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                          " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + v_mesSerialNumber.SerialNumberTypeID + "','" +
                                    v_mesSerialNumber.Value + "') " + "\r\n" +

                           " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                "@UnitID," + "\r\n" +
                                "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                "'1'," + "\r\n" +
                                "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                ")";

                    // + "\r\n" + " select @UnitID as SqlResult";
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout, I_MaxID);

                //if (Tuple_Result.Item1 == false)
                //{
                //    S_Result = "ERROR:" + Tuple_Result.Item2;
                //    Tuple_Result = new Tuple<bool, string>(false, S_Result);
                //}                         
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }
            //return S_Result;

            return Tuple_Result;

        }

        /// <summary>
        /// UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesmesUnitDetail"></param>
        /// <param name="List_mesSerialNumber"></param>
        /// <param name="List_mesHistory"></param>
        /// <param name="List_mesUnitComponent"></param>
        /// <param name="List_mesUnitDefect"></param>
        /// <returns></returns>
        public async Task<Tuple<bool, string>> SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAdd_UnitComponentAdd_UnitDefectAdd
       (
            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber,
            List<mesHistory> List_mesHistory,
            List<mesUnitComponent> List_mesUnitComponent,
            List<mesUnitDefect> List_mesUnitDefect
        )
        {
            string S_Result = "";
            string S_Sql = "";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;
            int I_MaxID = 0;

            try
            {
                for (int i = 0; i < List_mesUnit.Count; i++)
                {
                    mesUnit v_mesUnit = List_mesUnit[i];
                    mesUnitDetail v_mesUnitDetail = List_mesmesUnitDetail[i];
                    mesSerialNumber v_mesSerialNumber = List_mesSerialNumber[i];
                    mesHistory v_mesHistory = List_mesHistory[i];

                    S_Sql = " select max(ID)+1 as Valint1 from mesUnit";
                    var v_MaxID = await DapperConnRead2.QueryAsync<TabVal>(S_Sql, null, null, I_DBTimeout, null);
                    I_MaxID = Convert.ToInt32(v_MaxID.AsList()[0].Valint1);

                    S_Sql = " declare @UnitID int =" + I_MaxID + " \r\n" +

                           " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                           "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                           "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                          "VALUES (@UnitID" + "\r\n" +
                                 ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                ",'" + v_mesUnit.StatusID + "'" + "\r\n" +
                                 ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                 ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                 ",GETDATE()" + "\r\n" +
                                 ",GETDATE()" + "\r\n" +
                                 ",0" + "\r\n" +
                                 ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                 ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                 ",0" + "\r\n" +
                                 ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                 ",1)" + "\r\n" +

                          " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                          " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                        "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                        v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                          " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + v_mesSerialNumber.SerialNumberTypeID + "','" +
                                    v_mesSerialNumber.Value + "') " + "\r\n" +

                           " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                "@UnitID," + "\r\n" +
                                "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                "'1'," + "\r\n" +
                                "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                ")" + "\r\n";

                    if (List_mesUnitComponent != null)
                    {
                        for (int k = 0; k < List_mesUnitComponent.Count; k++)
                        {
                            mesUnitComponent v_mesUnitComponent = new mesUnitComponent();
                            v_mesUnitComponent = List_mesUnitComponent[k];

                            S_Sql +=
                            " insert into mesUnitComponent(UnitID, UnitComponentTypeID, ChildUnitID, ChildSerialNumber, ChildLotNumber, " + "\r\n" +
                            "ChildPartID, ChildPartFamilyID,Position, InsertedEmployeeID, InsertedStationID, InsertedTime, StatusID, LastUpdate) " + "\r\n" +
                            "values(@UnitID,'1','0'" + "\r\n" +
                            ",'" + v_mesUnitComponent.ChildSerialNumber + "'" + "\r\n" +
                            ",'',0,0,''" + "\r\n" +
                            ",'" + v_mesUnitComponent.InsertedEmployeeID + "'" + "\r\n" +
                            ",'" + v_mesUnitComponent.InsertedStationID + "'" + "\r\n" +
                            ",GETDATE()" + "\r\n" +
                            ",'" + v_mesUnit.StatusID + "'" + "\r\n" +
                            ",GETDATE()) " + "\r\n"; ;

                        }
                    }

                    if (List_mesUnitDefect != null)
                    {
                        if (List_mesUnitDefect.Count > 0)
                        {
                            S_Sql += "declare @MaxDefID int" + "\r\n";
                        }
                        int I_Qyt = 1;

                        for (int j = 0; j < List_mesUnitDefect.Count; j++)
                        {
                            mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                            v_mesUnitDefect = List_mesUnitDefect[j];
                            I_Qyt = j + 1;

                            S_Sql +=
                                    "select @MaxDefID=ISNULL(Max(ID),0)+" + I_Qyt + " from mesUnitDefect " + "\r\n" +
                                    "INSERT INTO mesUnitDefect(ID, UnitID, DefectID, StationID, EmployeeID) Values(" + "\r\n" +
                                    "@MaxDefID," + "\r\n" +
                                    "@UnitID," + "\r\n" +
                                    "'" + v_mesUnitDefect.DefectID + "'," + "\r\n" +
                                    "'" + v_mesUnitDefect.StationID + "'," + "\r\n" +
                                    "'" + v_mesUnitDefect.EmployeeID + "'" + "\r\n" +
                                    ")" + "\r\n";
                        }
                    }
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout, I_MaxID);

            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }
            return Tuple_Result;

        }



        public async Task<Tuple<bool, string>> SubmitData_FgPrintUPC(string PrintSN, string S_UnitStateID,
            string S_POID, string S_PartID, string S_UnitID, LoginList List_Login)
        {
            string S_Result = "";
            string S_Sql = "";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;
            int I_MaxID = 0;

            try
            {
                S_Sql =
                " declare @UnitID int select @UnitID=max(ID)+1 from mesUnit" +
                " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID,CreationTime,LastUpdate,PanelID,LineID," +
                "ProductionOrderID,RMAID,PartID,LooperCount) " +
                "VALUES (@UnitID,'" + Convert.ToInt32(S_UnitStateID) + "',1,'" + List_Login.StationID + "','" + List_Login.EmployeeID + "',GETDATE(),GETDATE(),0,'" + List_Login.LineID + "'" +
                ",'" + Convert.ToInt32(S_POID) + "',0,'" + Convert.ToInt32(S_PartID) + "',1)" +

                " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,6,'" + PrintSN + "')" +

                    "INSERT INTO mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) VALUES " +
                "(@UnitID,'','','','','')" +

                " INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)" +
                " VALUES (@UnitID,'" + Convert.ToInt32(S_UnitStateID) + "','" + List_Login.EmployeeID + "','" + List_Login.StationID + "',GETDATE(),GETDATE(),'" + Convert.ToInt32(S_POID) + "','" + Convert.ToInt32(S_PartID) + "',1,1)" +

                " UPDATE mesUnit SET UnitStateID='" + Convert.ToInt32(S_UnitStateID) + "',StatusID=1,StationID='" + List_Login.StationID + "'," +
                "LastUpdate=GETDATE(),ProductionOrderID='" + Convert.ToInt32(S_POID) + "' WHERE ID=" + Convert.ToInt32(S_UnitID) +

                " UPDATE mesUnitDetail SET KitSerialNumber='" + PrintSN + "' WHERE UnitID=" + Convert.ToInt32(S_UnitID) +

                " INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)" +
                " VALUES (" + Convert.ToInt32(S_UnitID) + ",'" + Convert.ToInt32(S_UnitStateID) + "','" + List_Login.EmployeeID + "','" + List_Login.StationID + "',GETDATE(),GETDATE(),'" + Convert.ToInt32(S_POID) + "','" + Convert.ToInt32(S_PartID) + "',1,1)";

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout, I_MaxID);
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }
            return Tuple_Result;
        }

        public async Task<string> SubmitData_DataOne(
            mesHistory v_mesHistory
            )
        {
            string S_Result = "OK";
            string S_Sql = "";

            S_Sql = " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                                "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "getdate()," + "\r\n" +
                                "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                "'1'," + "\r\n" +
                                "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                ")" + "\r\n";


            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;

        }

        public async Task<string> SubmitData_ToolingPrint(
            mesUnit v_mesUnit,
            mesHistory v_mesHistory
            )
        {
            string S_Result = "OK";
            string S_Sql = "";

            S_Sql = " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                ",StatusID=1" + ",StationID='" + v_mesUnit.StationID + "',EmployeeID='" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "',LastUpdate=getdate() where ID='" + v_mesUnit.ID + "'" + "\r\n" +

                " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                    "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                    "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                    "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                    "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                    "'" + v_mesHistory.StationID + "'," + "\r\n" +
                    "getdate()," + "\r\n" +
                    "getdate()," + "\r\n" +
                    "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                    "'" + v_mesHistory.PartID + "'," + "\r\n" +
                    "'1'," + "\r\n" +
                    "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                    ")" + "\r\n";


            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;

        }






        /// <summary>
        /// SubmitData_MesModMachineBySNStationTypeID
        /// </summary>
        /// <param name="S_MachineSN"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public async Task<string> SubmitData_MesModMachineBySNStationTypeID(
            string S_MachineSN, LoginList List_Login
        )
        {
            string S_Result = "";
            string S_Sql = "";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                string S_ModMachine_Sql = await
                    v_Public_Repository.MesModMachineBySNStationTypeID_Sql(S_MachineSN, Convert.ToInt32(List_Login.StationTypeID));

                S_Sql = S_ModMachine_Sql;
                if (S_ModMachine_Sql.Substring(0, 5) != "ERROR")
                {
                    Tuple_Val = new Tuple<string, object>(S_Sql, null);
                    List_Tuple.Add(Tuple_Val);
                    Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                    if (Tuple_Result.Item1 == false)
                    {
                        S_Result = "ERROR:" + Tuple_Result.Item2;
                        return S_Result;
                    }
                }
                else
                {
                    S_Result = "ERROR:" + S_Sql;
                    return S_Result;
                }

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;

        }



        public async Task<Tuple<bool, string>> SubmitData_MaterialInitial(
            List<mesMaterialUnit> List_mesMaterialUnit,
            List<mesMaterialUnitDetail> List_mesMaterialUnitDetail,

            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber,
            List<mesHistory> List_mesHistory
        )
        {
            string S_Result = "";
            string S_Sql = "";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;
            int I_MaxID = 0;

            try
            {
                for (int i = 0; i < List_mesMaterialUnit.Count; i++)
                {
                    mesMaterialUnit v_mesMaterialUnit = List_mesMaterialUnit[i];


                    mesMaterialUnitDetail v_mesMaterialUnitDetail = null;
                    mesUnit v_mesUnit = null;
                    mesUnitDetail v_mesUnitDetail = null;
                    mesSerialNumber v_mesSerialNumber = null;
                    mesHistory v_mesHistory = null;

                    if (List_mesMaterialUnitDetail.Count > 0)
                    {
                        v_mesMaterialUnitDetail = List_mesMaterialUnitDetail[i];
                    }

                    if (List_mesUnit.Count > 0)
                    {
                        v_mesUnit = List_mesUnit[i];
                    }

                    if (List_mesmesUnitDetail.Count > 0)
                    {
                        v_mesUnitDetail = List_mesmesUnitDetail[i];
                    }

                    if (List_mesSerialNumber.Count > 0)
                    {
                        v_mesSerialNumber = List_mesSerialNumber[i];
                    }

                    if (List_mesHistory.Count > 0)
                    {
                        v_mesHistory = List_mesHistory[i];
                    }

                    if (v_mesMaterialUnit.ExpiringTime == null)
                    {
                        S_Sql = " declare @MaterialUnitID int" + " \r\n" +
                               "INSERT INTO dbo.mesMaterialUnit(SerialNumber , PartID , StatusID , MaterialTypeID ," + " \r\n" +
                               " StationID , EmployeeID , LotCode , MaterialDateCode , DateCode , TraceCode , RollCode , " + " \r\n" +
                               "MPN , VendorID , Quantity , RemainQTY , CreationTime , LineID , LastUpdate )" + " \r\n" +

                               "VALUES('" + v_mesMaterialUnit.SerialNumber + "'" + " \r\n" +
                                        ",'" + v_mesMaterialUnit.PartID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.StatusID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MaterialTypeID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.StationID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.EmployeeID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.LotCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MaterialDateCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.DateCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.TraceCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.RollCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MPN + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.VendorID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.Quantity + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.RemainQTY + "'" + "\r\n" +
                                        ",getdate()" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.LineID + "'" + "\r\n" +
                                        ",getdate()" + "\r\n" +
                                      //   ",'" + v_mesMaterialUnit.ExpiringTime + "'" + "\r\n" +
                                      ")" + "\r\n";
                    }
                    else
                    {

                        S_Sql = " declare @MaterialUnitID int" + " \r\n" +
                               "INSERT INTO dbo.mesMaterialUnit(SerialNumber , PartID , StatusID , MaterialTypeID ," + " \r\n" +
                               " StationID , EmployeeID , LotCode , MaterialDateCode , DateCode , TraceCode , RollCode , " + " \r\n" +
                               "MPN , VendorID , Quantity , RemainQTY , CreationTime , LineID , LastUpdate , ExpiringTime)" + " \r\n" +

                               "VALUES('" + v_mesMaterialUnit.SerialNumber + "'" + " \r\n" +
                                        ",'" + v_mesMaterialUnit.PartID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.StatusID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MaterialTypeID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.StationID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.EmployeeID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.LotCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MaterialDateCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.DateCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.TraceCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.RollCode + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.MPN + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.VendorID + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.Quantity + "'" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.RemainQTY + "'" + "\r\n" +
                                        ",getdate()" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.LineID + "'" + "\r\n" +
                                        ",getdate()" + "\r\n" +
                                        ",'" + v_mesMaterialUnit.ExpiringTime + "'" + "\r\n" +
                                      ")" + "\r\n";

                    }
                    S_Sql += "select @MaterialUnitID=@@IDENTITY " + "\r\n";

                    if (v_mesMaterialUnitDetail != null)
                    {
                        S_Sql +=
                           "insert into mesMaterialUnitDetail(MaterialUnitID,LooperCount,Reserved_01," + "\r\n" +
                            " Reserved_02,Reserved_03,Reserved_04,Reserved_05)" + "\r\n" +
                            "VALUES(@MaterialUnitID" + "\r\n" +
                            ",'1'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_01 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_02 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_03 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_04 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_05 + "'" + "\r\n" +
                            ")" + "\r\n"
                            ;
                    }
                    //string S_SqlMaxID = " select max(ID)+1 as Valint1 from mesUnit";
                    //var v_MaxID = await DapperConnRead2.QueryAsync<TabVal>(S_SqlMaxID, null, null, I_DBTimeout, null);
                    //I_MaxID = Convert.ToInt32(v_MaxID.AsList()[0].Valint1);

                    //S_Sql = " declare @UnitID int =" + I_MaxID + " \r\n" +

                    if (v_mesUnit != null)
                    {
                        S_Sql += " declare @UnitID int " + "\r\n" +
                               " select @UnitID=max(ID)+1 from mesUnit" + " \r\n" +
                               " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                               "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                               "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                              "VALUES (@UnitID" + "\r\n" +
                                     ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                     ",1" + "\r\n" +
                                     ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                     ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                     ",GETDATE()" + "\r\n" +
                                     ",GETDATE()" + "\r\n" +
                                     ",0" + "\r\n" +
                                     ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                     ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                     ",0" + "\r\n" +
                                     ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                     ",1)" + "\r\n" +

                              " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                              " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                            "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                            v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                              " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + "2" + "','" +
                                        v_mesMaterialUnit.SerialNumber + "') " + "\r\n" +

                               " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                    "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                    "@UnitID," + "\r\n" +
                                    "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                    "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                    "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                    "getdate()," + "\r\n" +
                                    "getdate()," + "\r\n" +
                                    "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                    "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                    "'1'," + "\r\n" +
                                    "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                    ")";
                    }
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout, I_MaxID);
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }

            return Tuple_Result;

        }


        public async Task<Tuple<bool, string>> SubmitData_MaterialSplit(
            List<mesMaterialUnit> List_mesMaterialUnit,
            List<mesMaterialUnitDetail> List_mesMaterialUnitDetail,

            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber,
            List<mesHistory> List_mesHistory
        )
        {
            string S_Result = "";
            string S_Sql = "";
            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;
            int I_MaxID = 0;

            try
            {
                for (int i = 0; i < List_mesMaterialUnit.Count; i++)
                {
                    mesMaterialUnit v_mesMaterialUnit = List_mesMaterialUnit[i];


                    mesMaterialUnitDetail v_mesMaterialUnitDetail = null;
                    mesUnit v_mesUnit = null;
                    mesUnitDetail v_mesUnitDetail = null;
                    mesSerialNumber v_mesSerialNumber = null;
                    mesHistory v_mesHistory = null;

                    if (List_mesMaterialUnitDetail.Count > 0)
                    {
                        v_mesMaterialUnitDetail = List_mesMaterialUnitDetail[i];
                    }

                    if (List_mesUnit.Count > 0)
                    {
                        v_mesUnit = List_mesUnit[i];
                    }

                    if (List_mesmesUnitDetail.Count > 0)
                    {
                        v_mesUnitDetail = List_mesmesUnitDetail[i];
                    }

                    if (List_mesSerialNumber.Count > 0)
                    {
                        v_mesSerialNumber = List_mesSerialNumber[i];
                    }

                    if (List_mesHistory.Count > 0)
                    {
                        v_mesHistory = List_mesHistory[i];
                    }


                    S_Sql = @" declare @MaterialUnitID int
                                INSERT INTO dbo.mesMaterialUnit
                                    (SerialNumber
                                    , PartID
                                    , StatusID
                                    , MaterialTypeID
                                    , StationID
                                    , EmployeeID
                                    , LotCode
                                    , DateCode
                                    , TraceCode
                                    , MPN
                                    , VendorID
                                    , Quantity
                                    , RemainQTY
                                    , CreationTime
                                    , LineID
                                    , LastUpdate
                                    , ExpiringTime
				                    , ParentID
				                    , RollCode)
                                SELECT  '" + v_mesMaterialUnit.SerialNumber + @"' 
				                    , '" + v_mesMaterialUnit.PartID + @"'
                                    , StatusID
                                    , '" + v_mesMaterialUnit.MaterialTypeID + @"'
                                    , '" + v_mesMaterialUnit.StationID + @"'
                                    , '" + v_mesMaterialUnit.EmployeeID + @"'
                                    , LotCode
                                    , DateCode
                                    , TraceCode
                                    , MPN
                                    , VendorID
                                    , '" + v_mesMaterialUnit.Quantity + @"'
                                    , '" + v_mesMaterialUnit.Quantity + @"'
                                    , GETDATE()
                                    , '" + v_mesMaterialUnit.LineID + @"'
                                    , GETDATE()
                                    , ExpiringTime
				                    , '" + v_mesMaterialUnit.ParentID + @"'
				                    , '" + v_mesMaterialUnit.RollCode + @"'
                                FROM mesMaterialUnit WHERE ID=" + v_mesMaterialUnit.ParentID;


                    S_Sql += "select @MaterialUnitID=@@IDENTITY " + "\r\n";

                    if (v_mesMaterialUnitDetail != null)
                    {
                        S_Sql +=
                           "insert into mesMaterialUnitDetail(MaterialUnitID,LooperCount,Reserved_01," + "\r\n" +
                            " Reserved_02,Reserved_03,Reserved_04,Reserved_05)" + "\r\n" +
                            "VALUES(@MaterialUnitID" + "\r\n" +
                            ",'1'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_01 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_02 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_03 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_04 + "'" + "\r\n" +
                            ",'" + v_mesMaterialUnitDetail.Reserved_05 + "'" + "\r\n" +
                            ")" + "\r\n"
                            ;
                    }

                    if (v_mesUnit != null)
                    {
                        S_Sql += " declare @UnitID int " + "\r\n" +
                               " select @UnitID=max(ID)+1 from mesUnit" + " \r\n" +
                               " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                               "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                               "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                              "VALUES (@UnitID" + "\r\n" +
                                     ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                     ",1" + "\r\n" +
                                     ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                     ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                     ",GETDATE()" + "\r\n" +
                                     ",GETDATE()" + "\r\n" +
                                     ",0" + "\r\n" +
                                     ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                     ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                     ",0" + "\r\n" +
                                     ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                     ",1)" + "\r\n" +

                              " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                              " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                            "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                            v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                              " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + "2" + "','" +
                                        v_mesMaterialUnit.SerialNumber + "') " + "\r\n" +

                               " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                    "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                    "@UnitID," + "\r\n" +
                                    "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                    "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                    "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                    "getdate()," + "\r\n" +
                                    "getdate()," + "\r\n" +
                                    "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                    "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                    "'1'," + "\r\n" +
                                    "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                    ")";
                    }
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout, I_MaxID);
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
                Tuple_Result = new Tuple<bool, string>(false, S_Result);
            }

            return Tuple_Result;

        }






        #region howard add
        /// <summary>
        /// 产品包装
        /// </summary>
        /// <param name="PartID"></param>
        /// <param name="ProductionOrderID"></param>
        /// <param name="S_UPCSN"></param>
        /// <param name="S_CartonSN"></param>
        /// <param name="loginList"></param>
        /// <param name="Allnumber"></param>
        /// <param name="CurrentQty"></param>
        /// <param name="BoxQty">-1: 只扫描FG条码包装，0：扫描UPC进行包装, 大于0，则为尾箱包装</param>
        /// <returns></returns>
        public async Task<(string,string)> uspKitBoxPackagingNew(string PartID, string ProductionOrderID, string S_UPCSN, string S_CartonSN, LoginList loginList, int Allnumber, int CurrentQty, int BoxQty)
        {
            try
            {
                string strSql = @"SELECT ID FROM mesPackage WHERE SerialNumber=@S_CartonSN AND StatusID=0 AND Stage=1";
                var parames = new DynamicParameters();
                parames.Add("@S_CartonSN", S_CartonSN, DbType.String, ParameterDirection.Input, 1000);
                var dtMp = await DapperConnRead2.ExecuteScalarAsync(strSql, parames, null, I_DBTimeout, null);

                if (dtMp is null)
                    return ("20119","");

                string boxID = dtMp.ToString();

                string sqlValue = "";

                if (BoxQty > 0)
                {
                    sqlValue += @"INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                            VALUES(" + boxID + ", 1, " + loginList.StationID + ", " + loginList.EmployeeID + ", GETDATE()) \r\n";

                    sqlValue += @"UPDATE mesPackage SET CurrentCount=" + CurrentQty + ",StationID=" + loginList.StationID +
                                ",EmployeeID=" + loginList.EmployeeID + ",StatusID=1,LastUpdate=GETDATE() where ID=" + boxID + " \r\n";

                    sqlValue +=
                        @$"DECLARE @PackageDetailDefDesc VARCHAR(200) = 'LastBoxCount',@PackageID int = {boxID}, @LastBoxCount VARCHAR(50) = '{BoxQty}'
                            IF NOT EXISTS(SELECT 1 FROM dbo.luPackageDetailDef WHERE Description = @PackageDetailDefDesc)
                            BEGIN
	                            INSERT INTO dbo.luPackageDetailDef(ID, Description)
	                            VALUES( (SELECT MAX(ID)+1 FROM dbo.luPackageDetailDef), -- ID - int
	                            @PackageDetailDefDesc -- Description - nvarchar(200)
	                                )
                            END

                            INSERT INTO
                            dbo.mesPackageDetail(PackageID, PackageDetailDefID, Content)
                            SELECT @PackageID, ID,@LastBoxCount
                            from dbo.luPackageDetailDef
                            WHERE Description = @PackageDetailDefDesc
";
                }
                else
                {
                    string FGSN = string.Empty;
                    //关联包装信息
                    if (BoxQty == 0)
                    {
                        sqlValue = @"UPDATE A SET A.InmostPackageID = " + boxID + " FROM mesUnitDetail A WHERE A.KitSerialNumber = '" + S_UPCSN + "' \r\n";
                        FGSN = await v_Public_Repository.MesGetFGSNByUPCSNAsync(S_UPCSN);
                    }
                    else if (BoxQty == -1)
                    {
                        sqlValue = @"UPDATE A SET A.InmostPackageID = " + boxID + " FROM mesUnitDetail A INNER JOIN mesSerialNumber B ON A.UnitID=B.UnitID WHERE B.Value = '" + S_UPCSN + "' \r\n";
                        FGSN = S_UPCSN;
                    }
                    if (CurrentQty == Allnumber)
                    {
                        sqlValue += @"INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                            VALUES(" + boxID + ", 1, " + loginList.StationID + ", " + loginList.EmployeeID + ", GETDATE()) \r\n";


                        sqlValue += @"UPDATE mesPackage SET CurrentCount=" + CurrentQty + ",StationID=" + loginList.StationID +
                            ",EmployeeID=" + loginList.EmployeeID + ",StatusID=1,LastUpdate=GETDATE() where ID=" + boxID + " \r\n";
                    }

                    var strResult = await SetmesHistoryNoUpdatePOAsync(FGSN, loginList, ProductionOrderID);
                    if (strResult.Item1 != "1")
                        return strResult;

                    sqlValue += strResult.Item2;
                    //扫描UPC包装需要FG与UPC都过站
                    if (!string.IsNullOrEmpty(S_UPCSN) && S_UPCSN != FGSN)
                    {
                        var UpcResult = await SetmesHistoryNoUpdatePOAsync(S_UPCSN, loginList, ProductionOrderID);
                        if (UpcResult.Item1 != "1")
                            return UpcResult;
                        sqlValue += UpcResult.Item2;
                    }
                    
                }

                return ("1", sqlValue);
            }
            catch (Exception ex)
            {
                return (ex.Message.ToString(),"");
            }
        }

        /// <summary>
        /// 包装过站记录
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="loginList"></param>
        /// <returns></returns>
        public async Task<(string, string)> SetmesHistoryUpdatePOAsync(string S_SN, LoginList loginList, string ProductionOrderID)
        {
            string strSql = string.Empty;
            var SNInfo = await v_Public_Repository.GetSerialNumber2Async(S_SN);

            if (SNInfo is null || SNInfo.Count <= 0)
                return ("20127", "");

            mesUnit vMesUnit = SNInfo[0];
            var unitStateId = await v_Public_Repository.GetMesUnitState(vMesUnit.PartID.ToString(), vMesUnit.PartFamilyID.ToString(), loginList.LineID.ToString(),
                loginList.StationTypeID.ToInt(), ProductionOrderID, "1");

            //修改Unit属性
            strSql = $"UPDATE mesUnit SET UnitStateID={unitStateId},ProductionOrderID={ProductionOrderID},StatusID=1,EmployeeID={loginList.EmployeeID},LineID={loginList.LineID},StationID={loginList.StationID},LastUpdate=GETDATE() WHERE ID={vMesUnit.ID}\r\n";
            //插入历史记录
            strSql +=
                @$"INSERT INTO	dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                    VALUES({vMesUnit.ID}, -- UnitID - int
                    {unitStateId}   , -- UnitStateID - int
                    {loginList.EmployeeID}   , -- EmployeeID - int
                    {loginList.StationID}   , -- StationID - int
                    GETDATE(), -- EnterTime - datetime
                    GETDATE(), -- ExitTime - datetime
                    {ProductionOrderID}   , -- ProductionOrderID - int
                    {vMesUnit.PartID}   , -- PartID - int
                    1   , -- LooperCount - int
                    1   -- StatusID - int
                        )
                ";
            return ("1", strSql);
        }

        public async Task<(string, string)> SetmesHistoryNoUpdatePOAsync(string S_SN, LoginList loginList, string ProductionOrderID)
        {
            string strSql = string.Empty;
            var SNInfo = await v_Public_Repository.GetSerialNumber2Async(S_SN);

            if (SNInfo is null || SNInfo.Count <= 0)
                return ("20127", "");

            mesUnit vMesUnit = SNInfo[0];
            var unitStateId = await v_Public_Repository.GetMesUnitState(vMesUnit.PartID.ToString(), vMesUnit.PartFamilyID.ToString(), loginList.LineID.ToString(),
                loginList.StationTypeID.ToInt(), vMesUnit.ProductionOrderID.ToString(), "1");

            //修改Unit属性
            strSql = $"UPDATE mesUnit SET UnitStateID={unitStateId},StatusID=1,EmployeeID={loginList.EmployeeID},LineID={loginList.LineID},StationID={loginList.StationID},LastUpdate=GETDATE(),ProductionOrderID={ProductionOrderID} WHERE ID={vMesUnit.ID}\r\n";
            //插入历史记录
            strSql +=
                @$"INSERT INTO	dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                    VALUES({vMesUnit.ID}, -- UnitID - int
                    {unitStateId}   , -- UnitStateID - int
                    {loginList.EmployeeID}   , -- EmployeeID - int
                    {loginList.StationID}   , -- StationID - int
                    GETDATE(), -- EnterTime - datetime
                    GETDATE(), -- ExitTime - datetime
                    {ProductionOrderID}   , -- ProductionOrderID - int
                    {vMesUnit.PartID}   , -- PartID - int
                    1   , -- LooperCount - int
                    1   -- StatusID - int
                        )
                ";
            return ("1", strSql);

        }

        /// <summary>
        /// 修改unit和unitDetail表，并插入历史记录
        /// </summary>
        /// <param name="vMesUnit"></param>
        /// <param name="vMesUnitDetail"></param>
        /// <returns></returns>
        public  string SubmitData_Unit_Detail_History_Mod(
               mesUnit vMesUnit,
               mesUnitDetail vMesUnitDetail
           )
        {
            string S_Sql = "";
            string S_Result = string.Empty;
            try
            {
                if (vMesUnit is not null && vMesUnit.ID > 0)
                {
                    S_Sql += " update mesUnit set UnitStateID='" + vMesUnit.UnitStateID + "'\r\n" +
                             ",StationID='" + vMesUnit.StationID + "'\r\n" +
                             ",StatusID='" + vMesUnit.StatusID + "'\r\n" +
                             ",PanelID='" + vMesUnit.PanelID + "'\r\n" +
                             ",EmployeeID='" + vMesUnit.EmployeeID + "'\r\n" +
                             ",ProductionOrderID='" + vMesUnit.ProductionOrderID + "'\r\n" +
                             ",LastUpdate=getdate() \r\n" +
                             " where ID='" + vMesUnit.ID + "' \r\n";

                    S_Sql += " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                             "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                             $"{vMesUnit.ID},\r\n" +
                             "'" + vMesUnit.UnitStateID + "'," + "\r\n" +
                             "'" + vMesUnit.EmployeeID + "'," + "\r\n" +
                             "'" + vMesUnit.StationID + "'," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "'" + vMesUnit.ProductionOrderID + "'," + "\r\n" +
                             "'" + vMesUnit.PartID + "'," + "\r\n" +
                             "'1'," + "\r\n" +
                             "'" + vMesUnit.StatusID + "'" + "\r\n" +
                             ")\r\n";

                }

                if (vMesUnitDetail is not  null && vMesUnitDetail.UnitID > 0)
                {
                    S_Sql += $"UPDATE dbo.mesUnitDetail SET ProductionOrderID = {vMesUnitDetail.ProductionOrderID}, RMAID = {vMesUnitDetail.RMAID}, LooperCount = {vMesUnitDetail.LooperCount}, KitSerialNumber = '{vMesUnitDetail.KitSerialNumber}', InmostPackageID = {vMesUnitDetail.InmostPackageID}, OutmostPackageID = {vMesUnitDetail.OutmostPackageID}, reserved_01 = '{vMesUnitDetail.reserved_01}', reserved_02 = '{vMesUnitDetail.reserved_02}', reserved_03 = '{vMesUnitDetail.reserved_03}', reserved_04 = '{vMesUnitDetail.reserved_04}', reserved_05 = '{vMesUnitDetail.reserved_05}', reserved_06  = '{vMesUnitDetail.reserved_06}', reserved_07 = '{vMesUnitDetail.reserved_07}', reserved_08 = '{vMesUnitDetail.reserved_08}', reserved_09 = '{vMesUnitDetail.reserved_09}', reserved_10 = '{vMesUnitDetail.reserved_10}', reserved_11 = '{vMesUnitDetail.reserved_11}', reserved_12 = '{vMesUnitDetail.reserved_12}', reserved_13 = '{vMesUnitDetail.reserved_13}', reserved_14 = '{vMesUnitDetail.reserved_14}', reserved_15 = '{vMesUnitDetail.reserved_15}', reserved_16 = '{vMesUnitDetail.reserved_16}', reserved_17 = '{vMesUnitDetail.reserved_17}', reserved_18 = '{vMesUnitDetail.reserved_18}', reserved_19 = '{vMesUnitDetail.reserved_19}', reserved_20  = '{vMesUnitDetail.reserved_20}' WHERE UnitID = {vMesUnitDetail.UnitID}";
                }

                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                return S_Result = S_Sql;
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }




        /// <summary>
        /// 仅用于拼装sql使用，无法单独使用
        /// </summary>
        /// <param name="FromTooling">新治具条码</param>
        /// <param name="ToTooling">旧治具条码</param>
        /// <param name="loginList">登录信息</param>
        /// <returns></returns>
        private string SetToolingLinkTooling_Sql(string FromTooling, string ToTooling, LoginList loginList)
        {
            string S_Result = "";
            ///查询出旧治具的unitID
            string Str_Sql = string.Format("SELECT MAX(UnitID) UnitID FROM mesUnitDetail WHERE reserved_01='{0}'", ToTooling);

            string ToUnitID = string.Empty;

            ToUnitID = DapperConnRead2.ExecuteScalar(Str_Sql)?.ToString();

            if (string.IsNullOrEmpty(ToUnitID))
            {
                S_Result = "ERROR:" + ToTooling + " not exists";
                return S_Result;
            }

            //Str_Sql = $"SELECT 1 FROM dbo.mesUnitDetail WHERE UnitID = {ToUnitID}";

            //var existsUnit = DapperConnRead2.ExecuteScalar(Str_Sql);
            //if (existsUnit == null)
            {
                Str_Sql = string.Format(@"insert into mesUnitDetail(UnitID,ProductionOrderID,RMAID,LooperCount,KitSerialNumber,InmostPackageID,OutmostPackageID
                                          , reserved_01, reserved_02, reserved_03, reserved_04, reserved_05, reserved_06, reserved_07, reserved_08, reserved_09, reserved_10
                                          , reserved_11, reserved_12, reserved_13, reserved_14, reserved_15, reserved_16, reserved_17, reserved_18, reserved_19, reserved_20)
                                    select CAST(@UnitID as varchar(200)) as UnitID,ProductionOrderID,RMAID,LooperCount,KitSerialNumber,InmostPackageID,OutmostPackageID
                                          ,'{1}',reserved_02,reserved_03,reserved_04,reserved_05,reserved_06,'{2}',reserved_08,reserved_09,reserved_10
                                          ,reserved_11,reserved_12,reserved_13,reserved_14,reserved_15,reserved_16,reserved_17,reserved_18,reserved_19,reserved_20
                                    from mesUnitDetail where UnitID ={0} ", ToUnitID, FromTooling, ToTooling);
            }
            //else
            //{
            //    Str_Sql = $"UPDATE a SET a.reserved_07 = '{FromTooling}' FROM dbo.mesUnitDetail a WHERE UnitID = {ToUnitID}";
            //}

            Str_Sql += "\r\n";

            //只能应用于组装sql字符串，语句中包含unitID
            Str_Sql += string.Format(@"INSERT INTO mesUnitComponent(UnitID,UnitComponentTypeID,ChildUnitID,ChildSerialNumber,ChildLotNumber
	                                      ,ChildPartID,ChildPartFamilyID,Position,InsertedEmployeeID,InsertedStationID,InsertedTime
	                                      ,RemovedEmployeeID,RemovedStationID,RemovedTime,StatusID,LastUpdate,PreviousLink)
                                      SELECT CAST(@UnitID as varchar(200)) as UnitID,UnitComponentTypeID,ChildUnitID,ChildSerialNumber,ChildLotNumber
	                                      ,ChildPartID,ChildPartFamilyID,Position,'{0}',{1},GETDATE()
	                                      ,RemovedEmployeeID,RemovedStationID,RemovedTime,StatusID,GETDATE(),PreviousLink
	                                      FROM mesUnitComponent WHERE UnitID={2} AND StatusID=1", loginList.EmployeeID, loginList.StationID, ToUnitID);

            Str_Sql += "\r\n";
            return S_Result = Str_Sql;
        }

        /// <summary>
        /// UnitAdd UnitDetailAdd SNAdd HistoryAdd
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesmesUnitDetail"></param>
        /// <param name="List_mesSerialNumber"></param>
        /// <param name="List_mesHistory"></param>
        /// <returns></returns>
        public string SubmitData_UnitAdd_UnitDetailAdd_SNAdd_HistoryAddStr(
            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber,
            List<mesHistory> List_mesHistory
        )
        {
            string S_Result = "Error:";
            string S_Sql = "";

            string sMaxId = string.Empty;
            try
            {
                using (IDbConnection connection = DapperConn)
                {
                    bool isClosed = connection.State == ConnectionState.Closed;
                    if (isClosed) connection.Open();
                    //开启事务
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            for (int i = 0; i < List_mesUnit.Count; i++)
                            {
                                mesUnit v_mesUnit = List_mesUnit[i];
                                mesUnitDetail v_mesUnitDetail = List_mesmesUnitDetail[i];
                                mesSerialNumber v_mesSerialNumber = List_mesSerialNumber[i];
                                mesHistory v_mesHistory = List_mesHistory[i];
                                S_Sql = " select max(ID)+1 from mesUnit";
                                var maxID = connection.ExecuteScalar(S_Sql, null, transaction, I_DBTimeout, CommandType.Text);
                                if (maxID == null)
                                {
                                    throw new Exception("can not get max id.");
                                }
                                sMaxId = maxID.ToString();
                                S_Sql = " declare @UnitID int =" + sMaxId + " \r\n " +

                                       " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                                       "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                                       "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                                      "VALUES (@UnitID" + "\r\n" +
                                             ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                             ",1" + "\r\n" +
                                             ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                             ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                             ",GETDATE()" + "\r\n" +
                                             ",GETDATE()" + "\r\n" +
                                             ",0" + "\r\n" +
                                             ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                             ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                             ",0" + "\r\n" +
                                             ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                             ",1)" + "\r\n" +

                                      " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                                      " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                                    "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                                    v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                                      " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + v_mesSerialNumber.SerialNumberTypeID + "','" +
                                                v_mesSerialNumber.Value + "') " + "\r\n" +
                                        
                                       " insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                                            "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                                            "@UnitID," + "\r\n" +
                                            "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                                            "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                                            "'" + v_mesHistory.StationID + "'," + "\r\n" +
                                            "getdate()," + "\r\n" +
                                            "getdate()," + "\r\n" +
                                            "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                                            "'" + v_mesHistory.PartID + "'," + "\r\n" +
                                            "'1'," + "\r\n" +
                                            "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                                            ")" + "\r\n" +

                                " select @UnitID as SqlResult";
                                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                                {

                                }
                                var affRos = connection.Execute(S_Sql, null, transaction, I_DBTimeout, CommandType.Text);
                                if (affRos != 4)
                                    throw new Exception("affected rows is not 4");
                            }
                            //提交事务
                            transaction.Commit();
                            return S_Result = sMaxId;
                        }
                        catch (Exception ex)
                        {
                            //回滚事务
                            Log4NetHelper.Error("ExecuteTransaction", ex);
                            transaction?.Rollback();
                            connection?.Close();
                            connection?.Dispose();
                            return S_Result += ex.ToString();
                        }
                        finally
                        {
                            connection?.Close();
                            connection?.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result += ex.ToString();
            }
            return S_Result;

        }
        /// <summary>
        /// UnitAdd UnitDetailAdd SNAdd HistoryAdd
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesmesUnitDetail"></param>
        /// <param name="List_mesSerialNumber"></param>
        /// <param name="List_mesHistory"></param>
        /// <returns></returns>
        public string SubmitData_UnitAdd_UnitDetailAdd_SN_AddStr(
            List<mesUnit> List_mesUnit,
            List<mesUnitDetail> List_mesmesUnitDetail,
            List<mesSerialNumber> List_mesSerialNumber
        )
        {
            string S_Result = "Error:";
            string S_Sql = "";

            string sMaxId = string.Empty;
            try
            {
                using (IDbConnection connection = DapperConn)
                {
                    bool isClosed = connection.State == ConnectionState.Closed;
                    if (isClosed) connection.Open();
                    //开启事务
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {

                            for (int i = 0; i < List_mesUnit.Count; i++)
                            {
                                mesUnit v_mesUnit = List_mesUnit[i];
                                mesUnitDetail v_mesUnitDetail = List_mesmesUnitDetail[i];
                                mesSerialNumber v_mesSerialNumber = List_mesSerialNumber[i];

                                S_Sql = " select max(ID)+1 from mesUnit";
                                var maxID = connection.ExecuteScalar(S_Sql, null, transaction, I_DBTimeout, CommandType.Text);
                                if (maxID == null)
                                {
                                    throw new Exception("can not get max id.");
                                }
                                sMaxId = maxID.ToString();
                                S_Sql = " declare @UnitID int =" + sMaxId + " \r\n " +

                                       " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                                       "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                                       "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                                      "VALUES (@UnitID" + "\r\n" +
                                             ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                             ",1" + "\r\n" +
                                             ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                             ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                             ",GETDATE()" + "\r\n" +
                                             ",GETDATE()" + "\r\n" +
                                             ",0" + "\r\n" +
                                             ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                             ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                             ",0" + "\r\n" +
                                             ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                             ",1)" + "\r\n" +

                                      " Insert into mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) Values(" + "\r\n" +
                                      " @UnitID,'" + v_mesUnitDetail.reserved_01 + "','" + v_mesUnitDetail.reserved_02 +
                                                    "','" + v_mesUnitDetail.reserved_03 + "','" + v_mesUnitDetail.reserved_04 + "','" +
                                                    v_mesUnitDetail.reserved_05 + "')" + "\r\n" +

                                      " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES (@UnitID,'" + v_mesSerialNumber.SerialNumberTypeID + "','" +
                                                v_mesSerialNumber.Value + "') " + "\r\n" +

                                " select @UnitID as SqlResult";
                                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                                {

                                }
                                var affRos = connection.Execute(S_Sql, null, transaction, I_DBTimeout, CommandType.Text);
                                if (affRos != 3)
                                    throw new Exception("affected rows is not 3");
                            }
                            //提交事务
                            transaction.Commit();
                            return S_Result = sMaxId;
                        }
                        catch (Exception ex)
                        {
                            //回滚事务
                            Log4NetHelper.Error("ExecuteTransaction", ex);
                            transaction?.Rollback();
                            connection?.Close();
                            connection?.Dispose();
                            return S_Result += ex.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result += ex.ToString();
            }
            return S_Result;

        }


        /// <summary>
        /// 拼装单个产品对应的相关表的sql 语句
        /// </summary>
        /// <param name="v_mesUnit"></param>
        /// <param name="v_mesUnitDetail"></param>
        /// <param name="v_mesSerialNumber"></param>
        /// <param name="v_mesUnitComponent"></param>
        /// <param name="v_mesHistory"></param>
        /// <param name="List_mesUnitDefect"></param>
        /// <param name="v_mesMachine"></param>
        /// <param name="L_TLinkT"></param>
        /// <param name="List_Login"></param>
        /// <returns></returns>
        public string SubmitData_Unit_UnitDetail_SN_History_Defect_Machine(
            mesUnit v_mesUnit,
            mesUnitDetail v_mesUnitDetail,
            mesSerialNumber v_mesSerialNumber,
            mesUnitComponent v_mesUnitComponent,
            mesHistory v_mesHistory,
            List<mesUnitDefect> List_mesUnitDefect,
            mesMachine v_mesMachine,
            string[] L_TLinkT,
            LoginList List_Login)
        {
            string S_Result = "Error:";
            string S_Sql = "";

            try
            {
                if (v_mesUnit != null)
                {
                    S_Sql +=
                        " declare @UnitID int select @UnitID=max(ID)+1 from mesUnit" + "\r\n" +

                        " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                        "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                        "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                        "VALUES (@UnitID" + "\r\n" +
                                ",'" + v_mesUnit.UnitStateID + "'" + "\r\n" +
                                ",'" + v_mesUnit.StatusID + "'" + "\r\n" +
                                ",'" + v_mesUnit.StationID + "'" + "\r\n" +
                                ",'" + v_mesUnit.EmployeeID + "'" + "\r\n" +
                                ",GETDATE()" + "\r\n" +
                                ",GETDATE()" + "\r\n" +
                                ",0" + "\r\n" +
                                ",'" + v_mesUnit.LineID + "'" + "\r\n" +
                                ",'" + v_mesUnit.ProductionOrderID + "'" + "\r\n" +
                                ",0" + "\r\n" +
                                ",'" + v_mesUnit.PartID + "'" + "\r\n" +
                                ",1)" + "\r\n";
                }


                if (v_mesUnitDetail != null)
                {
                    S_Sql +=
                             "INSERT INTO mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) VALUES " + "\r\n" +
                             "(@UnitID" + "\r\n" +
                             ",'" + (string)v_mesUnitDetail.reserved_01 + "'" + "\r\n" +
                             ",'" + (string)v_mesUnitDetail.reserved_02 + "'" + "\r\n" +
                             ",'" + (string)v_mesUnitDetail.reserved_03 + "'" + "\r\n" +
                             ",'" + (string)v_mesUnitDetail.reserved_04 + "'" + "\r\n" +
                             ",'" + (string)v_mesUnitDetail.reserved_05 + "'" + "\r\n" +
                             ")" + "\r\n";
                }


                if (v_mesHistory != null)
                {
                    S_Sql += "insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                             "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                             "@UnitID," + "\r\n" +
                             "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                             "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                             "'" + v_mesHistory.StationID + "'," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                             "'" + v_mesHistory.PartID + "'," + "\r\n" +
                             "'1'," + "\r\n" +
                             "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                             ")" + "\r\n";
                }


                if (v_mesSerialNumber != null)
                {
                    S_Sql +=
                        " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES " + "\r\n" +
                        "(@UnitID," + "\r\n" +
                        "'" + v_mesSerialNumber.SerialNumberTypeID + "'," + "\r\n" +
                        "'" + v_mesSerialNumber.Value + "'" + "\r\n" +
                        ") " + "\r\n";
                }


                if (v_mesUnitComponent != null)
                {
                    S_Sql +=
                        " insert into mesUnitComponent(UnitID, UnitComponentTypeID, ChildUnitID, ChildSerialNumber, ChildLotNumber, " + "\r\n" +
                        "ChildPartID, ChildPartFamilyID,Position, InsertedEmployeeID, InsertedStationID, InsertedTime, StatusID, LastUpdate) " + "\r\n" +
                        "values(@UnitID,'1','0'" + "\r\n" +
                        ",'" + v_mesUnitComponent.ChildSerialNumber + "'" + "\r\n" +
                        ",'',0,0,''" + "\r\n" +
                        ",'" + v_mesUnitComponent.InsertedEmployeeID + "'" + "\r\n" +
                        ",'" + v_mesUnitComponent.InsertedStationID + "'" + "\r\n" +
                        ",GETDATE(),1,GETDATE()) " + "\r\n";
                }


                if (List_mesUnitDefect != null)
                {
                    if (List_mesUnitDefect.Count > 0)
                    {
                        S_Sql += "declare @MaxDefID int" + "\r\n";
                    }
                    int I_Qyt = 1;

                    for (int i = 0; i < List_mesUnitDefect.Count; i++)
                    {
                        mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                        v_mesUnitDefect = List_mesUnitDefect[i];
                        I_Qyt = I_Qyt + 1;

                        S_Sql +=
                                "select @MaxDefID=ISNULL(Max(ID),0)+" + I_Qyt + " from mesUnitDefect " + "\r\n" +
                                "INSERT INTO mesUnitDefect(ID, UnitID, DefectID, StationID, EmployeeID) Values(" + "\r\n" +
                                "@MaxDefID," + "\r\n" +
                                "@UnitID," + "\r\n" +
                                "'" + v_mesUnitDefect.DefectID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.StationID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.EmployeeID + "'" + "\r\n" +
                                ")" + "\r\n";
                    }
                }

                if (v_mesMachine != null)
                {
                    string S_MachineReult = string.Empty;
                    S_MachineReult = GetModMachineBySNStationTypeID_Sql(v_mesMachine.SN, List_Login.StationTypeID.ToInt());

                    if (S_MachineReult != "")
                    {
                        if (S_MachineReult.Substring(0, 5) == "ERROR")
                        {
                            return S_Result = S_MachineReult;
                        }
                        else
                        {
                            S_Sql += S_MachineReult;
                        }
                    }
                }

                if (L_TLinkT is { Length: >= 3 } )
                {
                    string S_ToolingLinkTooling = string.Empty;
                    S_ToolingLinkTooling = SetToolingLinkTooling_Sql(L_TLinkT[0], L_TLinkT[1], List_Login);

                    if (S_ToolingLinkTooling.Substring(0, 5) == "ERROR")
                    {
                        return S_Result = S_ToolingLinkTooling;
                    }
                    else
                    {
                        S_Sql += S_ToolingLinkTooling;
                    }
                }

                S_Result = S_Sql;
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }
        public string SubmitData_Units_UnitDetails_SNs_Historys_Defects_Machines(
            List<mesUnit> v_mesUnit,
            List<mesUnitDetail> v_mesUnitDetail,
            List<mesSerialNumber> v_mesSerialNumber,
            List<mesUnitComponent> v_mesUnitComponent,
            List<mesUnitDefect> v_List_mesUnitDefect,
            List<mesMachine> v_mesMachine,
            string[] L_TLinkT,
            LoginList List_Login)
        {
            string S_Result = "Error:";
            string S_Sql = "";

            try
            {
                if (v_mesUnit?.Count <= 0)
                    return S_Result + "exception";
                if (v_mesUnit?.Count != v_mesSerialNumber?.Count)
                    return S_Result + "exception";


                S_Sql = "declare @UnitID int \r\n";
                if (v_List_mesUnitDefect is { Count: > 0 })
                {
                    S_Sql += "declare @MaxDefID int \r\n";
                }
                for (int i = 0; i < v_mesUnit.Count; i++)
                {
                    var mesUnit = v_mesUnit[i];
                    mesUnitDetail mesUnitDetail = v_mesUnitDetail is { Count: > 0 } ? v_mesUnitDetail?[i] : null;
                    var mesSerialNumber = v_mesSerialNumber[i];
                    S_Sql +=
                        " select @UnitID=max(ID)+1 from mesUnit" + "\r\n" +

                        " INSERT INTO mesUnit(ID,UnitStateID,StatusID,StationID,EmployeeID," +
                        "CreationTime,LastUpdate,PanelID,LineID," + "\r\n" +
                        "ProductionOrderID,RMAID,PartID,LooperCount) " + "\r\n" +

                        "VALUES (@UnitID" + "\r\n" +
                        ",'" + mesUnit.UnitStateID + "'" + "\r\n" +
                        ",'" + mesUnit.StatusID + "'" + "\r\n" +
                        ",'" + mesUnit.StationID + "'" + "\r\n" +
                        ",'" + mesUnit.EmployeeID + "'" + "\r\n" +
                        ",GETDATE()" + "\r\n" +
                        ",GETDATE()" + "\r\n" +
                        ",0" + "\r\n" +
                        ",'" + mesUnit.LineID + "'" + "\r\n" +
                        ",'" + mesUnit.ProductionOrderID + "'" + "\r\n" +
                        ",0" + "\r\n" +
                        ",'" + mesUnit.PartID + "'" + "\r\n" +
                        ",1)" + "\r\n";

                    if (mesUnitDetail is not null)
                    {
                        S_Sql +=
                            "INSERT INTO mesUnitDetail(UnitID,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05) VALUES " + "\r\n" +
                            "(@UnitID" + "\r\n" +
                            ",'" + (string)mesUnitDetail.reserved_01 + "'" + "\r\n" +
                            ",'" + (string)mesUnitDetail.reserved_02 + "'" + "\r\n" +
                            ",'" + (string)mesUnitDetail.reserved_03 + "'" + "\r\n" +
                            ",'" + (string)mesUnitDetail.reserved_04 + "'" + "\r\n" +
                            ",'" + (string)mesUnitDetail.reserved_05 + "'" + "\r\n" +
                            ")" + "\r\n";
                    }

                    S_Sql +=
                        " INSERT INTO mesSerialNumber(UnitID,SerialNumberTypeID,Value) VALUES " + "\r\n" +
                        "(@UnitID," + "\r\n" +
                        "'" + mesSerialNumber.SerialNumberTypeID + "'," + "\r\n" +
                        "'" + mesSerialNumber.Value + "'" + "\r\n" +
                        ") " + "\r\n";

                    S_Sql += "insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                             "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                             "@UnitID," + "\r\n" +
                             "'" + mesUnit.UnitStateID + "'," + "\r\n" +
                             "'" + mesUnit.EmployeeID + "'," + "\r\n" +
                             "'" + mesUnit.StationID + "'," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "getdate()," + "\r\n" +
                             "'" + mesUnit.ProductionOrderID + "'," + "\r\n" +
                             "'" + mesUnit.PartID + "'," + "\r\n" +
                             "'1'," + "\r\n" +
                             "'" + mesUnit.StatusID + "'" + "\r\n" +
                             ")" + "\r\n";


                    if (v_List_mesUnitDefect is { Count: > 0 })
                    {
                        for (int t = 0; t < v_List_mesUnitDefect.Count; t++)
                        {
                            mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                            v_mesUnitDefect = v_List_mesUnitDefect[i];

                            S_Sql +=
                                "select @MaxDefID=ISNULL(Max(ID),0) + 1  from mesUnitDefect " + "\r\n" +
                                "INSERT INTO mesUnitDefect(ID, UnitID, DefectID, StationID, EmployeeID) Values(" + "\r\n" +
                                "@MaxDefID," + "\r\n" +
                                "@UnitID," + "\r\n" +
                                "'" + v_mesUnitDefect.DefectID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.StationID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.EmployeeID + "'" + "\r\n" +
                                ")" + "\r\n";
                        }
                    }

                    for (int r = 0; r < v_mesUnitComponent?.Count; r++)
                    {
                        var mesUnitComponent = v_mesUnitComponent[r];
                        S_Sql +=
                            " insert into mesUnitComponent(UnitID, UnitComponentTypeID, ChildUnitID, ChildSerialNumber, ChildLotNumber, " + "\r\n" +
                            "ChildPartID, ChildPartFamilyID,Position, InsertedEmployeeID, InsertedStationID, InsertedTime, StatusID, LastUpdate) " + "\r\n" +
                            "values(@UnitID,'1','0'" + "\r\n" +
                            ",'" + mesUnitComponent.ChildSerialNumber + "'" + "\r\n" +
                            ",'',0,0,''" + "\r\n" +
                            ",'" + mesUnitComponent.InsertedEmployeeID + "'" + "\r\n" +
                            ",'" + mesUnitComponent.InsertedStationID + "'" + "\r\n" +
                            ",GETDATE(),1,GETDATE()) " + "\r\n";
                    }
                }

                if (L_TLinkT is { Length: >= 3 })
                {
                    //新旧治具只会存在一组，当mesunit存在多个时，只使用最后一个unit的ID进行更新---------------------------------------
                    string S_ToolingLinkTooling = string.Empty;
                    S_ToolingLinkTooling = SetToolingLinkTooling_Sql(L_TLinkT[0], L_TLinkT[1], List_Login);

                    if (S_ToolingLinkTooling.Substring(0, 5) == "ERROR")
                    {
                        return S_Result = S_ToolingLinkTooling;
                    }
                    else
                    {
                        S_Sql += S_ToolingLinkTooling;
                    }
                }

                if (v_mesMachine is { Count: > 0 })
                {
                    for (int i = 0; i < v_mesMachine.Count; i++)
                    {
                        string S_MachineReult = string.Empty;
                        var mesMachine = v_mesMachine[i];
                        S_MachineReult = GetModMachineBySNStationTypeID_Sql(mesMachine.SN, List_Login.StationTypeID.ToInt());

                        if (S_MachineReult != "")
                        {
                            if (S_MachineReult.Substring(0, 5) == "ERROR")
                            {
                                return S_Result = S_MachineReult;
                            }
                            else
                            {
                                S_Sql += S_MachineReult;
                            }
                        }
                    }
                }



                S_Result = S_Sql;
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }
        /// <summary>
        /// 拼装sql语句
        /// </summary>
        /// <param name="FromTooling"></param>
        /// <param name="ToTooling"></param>
        /// <param name="FromUintID"></param>
        /// <param name="loginList"></param>
        /// <returns></returns>
        private string SetToolingLinkTooling_Sql(string FromTooling, string ToTooling, string FromUintID, LoginList loginList)
        {
            string S_Result = "";
            string Str_Sql = string.Format("SELECT MAX(UnitID) UnitID FROM mesUnitDetail WHERE reserved_01='{0}'", ToTooling);

            string ToUnitID = string.Empty;

            ToUnitID = DapperConnRead2.ExecuteScalar(Str_Sql)?.ToString();

            if (string.IsNullOrEmpty(ToUnitID))
            {
                S_Result = "ERROR:" + ToTooling + " not exists";
                return S_Result;
            }


            Str_Sql = string.Format(@"insert into mesUnitDetail(UnitID,ProductionOrderID,RMAID,LooperCount,KitSerialNumber,InmostPackageID,OutmostPackageID
                                          , reserved_01, reserved_02, reserved_03, reserved_04, reserved_05, reserved_06, reserved_07, reserved_08, reserved_09, reserved_10
                                          , reserved_11, reserved_12, reserved_13, reserved_14, reserved_15, reserved_16, reserved_17, reserved_18, reserved_19, reserved_20)
                                    select {0},ProductionOrderID,RMAID,LooperCount,KitSerialNumber,InmostPackageID,OutmostPackageID
                                          ,reserved_01,reserved_02,reserved_03,reserved_04,reserved_05,reserved_06,reserved_07,reserved_08,reserved_09,reserved_10
                                          ,reserved_11,reserved_12,reserved_13,reserved_14,reserved_15,reserved_16,reserved_17,reserved_18,reserved_19,reserved_20
                                    from mesUnitDetail where UnitID ={1} ", FromUintID, ToUnitID);
            Str_Sql += "\r\n";

            Str_Sql += string.Format(@"INSERT INTO mesUnitComponent(UnitID,UnitComponentTypeID,ChildUnitID,ChildSerialNumber,ChildLotNumber
	                                      ,ChildPartID,ChildPartFamilyID,Position,InsertedEmployeeID,InsertedStationID,InsertedTime
	                                      ,RemovedEmployeeID,RemovedStationID,RemovedTime,StatusID,LastUpdate,PreviousLink)
                                      SELECT {0},UnitComponentTypeID,ChildUnitID,ChildSerialNumber,ChildLotNumber
	                                      ,ChildPartID,ChildPartFamilyID,Position,'{1}',{2},GETDATE()
	                                      ,RemovedEmployeeID,RemovedStationID,RemovedTime,StatusID,GETDATE(),PreviousLink
	                                      FROM mesUnitComponent WHERE UnitID={3} AND StatusID=1", FromUintID, loginList.EmployeeID, loginList.StationID, ToUnitID);

            Str_Sql += "\r\n";
            return S_Result = Str_Sql;
        }

        public async Task<string> SubmitDataUHCAsync(List<mesUnit> List_mesUnit, List<mesHistory> List_mesHistory,
            List<mesUnitComponent> List_mesUnitComponent, List<mesMaterialConsumeInfo> List_mesMaterialConsumeInfo,
            List<mesMachine> List_mesMachine, LoginList F_LoginList
            )
        {
            string S_Result = "";
            string S_Sql = "";

            try
            {
                if (List_mesUnit != null)
                {
                    for (int i = 0; i < List_mesUnit.Count; i++)
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit = List_mesUnit[i];

                        S_Sql += " update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                ",LastUpdate=getdate() \r\n" +
                                " where ID='" + v_mesUnit.ID + "' \r\n";
                    }
                }


                if (List_mesHistory != null)
                {
                    for (int i = 0; i < List_mesHistory.Count; i++)
                    {
                        mesHistory v_mesHistory = new mesHistory();
                        v_mesHistory = List_mesHistory[i];

                        S_Sql += "insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                            "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                            "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                            "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                            "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                            "'" + v_mesHistory.StationID + "'," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                            "'" + v_mesHistory.PartID + "'," + "\r\n" +
                            "'1'," + "\r\n" +
                            "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                            ")" + "\r\n";

                    }
                }

                if (List_mesUnitComponent != null)
                {
                    for (int i = 0; i < List_mesUnitComponent.Count; i++)
                    {
                        mesUnitComponent v_mesUnitComponent = new mesUnitComponent();
                        v_mesUnitComponent = List_mesUnitComponent[i];

                        S_Sql += "insert into mesUnitComponent(UnitID, UnitComponentTypeID, ChildUnitID, ChildSerialNumber, ChildLotNumber," + "\r\n" +
                                "ChildPartID, ChildPartFamilyID,Position, InsertedEmployeeID, InsertedStationID, InsertedTime, StatusID, LastUpdate)" + "\r\n" +
                                "Values(" + "'" + v_mesUnitComponent.UnitID + "'," + "\r\n" +
                                "'1'," + "\r\n" +
                                "'" + v_mesUnitComponent.ChildUnitID + "'," + "\r\n" +
                                "'" + v_mesUnitComponent.ChildSerialNumber + "'," + "\r\n" +
                                "'" + v_mesUnitComponent.ChildLotNumber + "'," + "\r\n" +
                                "'" + v_mesUnitComponent.ChildPartID + "'," + "\r\n" +
                                "'" + v_mesUnitComponent.ChildPartFamilyID + "'," + "\r\n" +
                                "''," + "\r\n" +
                                "'" + v_mesUnitComponent.InsertedEmployeeID + "'," + "\r\n" +
                                "'" + v_mesUnitComponent.InsertedStationID + "'," + "\r\n" +
                                "GETDATE()," + "\r\n" +
                                "1," + "\r\n" +
                                "GETDATE()" + "\r\n" +
                                ")" + "\r\n";

                    }
                }

                if (List_mesMaterialConsumeInfo != null)
                {
                    for (int i = 0; i < List_mesMaterialConsumeInfo.Count; i++)
                    {
                        mesMaterialConsumeInfo v_mesMaterialConsumeInfo = new mesMaterialConsumeInfo();
                        v_mesMaterialConsumeInfo = List_mesMaterialConsumeInfo[i];
                        string S_FGSN = "";

                        if (v_mesMaterialConsumeInfo.ScanType == 2)
                        {
                            string S_MachineSN_Sql = "SELECT reserved_02 FROM mesUnitDetail WHERE ID = " +
                                "(SELECT MAX(ID) FROM mesUnitDetail WHERE reserved_01 = '" + v_mesMaterialConsumeInfo.MachineSN + "' AND reserved_03 = 1)";
                            //DataTable DT_UnitDetail = SqlServerHelper.Data_Table(S_MachineSN_Sql);
                            var tConsumeUnitDetail = DapperConnRead2.ExecuteScalar(S_MachineSN_Sql);
                            if (tConsumeUnitDetail == null)
                            {
                                S_Result = "ERROR:The Tooling is not bound to FG ";
                            }
                            else
                            {
                                S_FGSN = tConsumeUnitDetail.ToString();
                            }
                        }
                        else
                        {
                            S_FGSN = v_mesMaterialConsumeInfo.SN;
                        }

                        string S_Sql_MCI = "SELECT 1 FROM mesMaterialConsumeInfo where PartID='" + v_mesMaterialConsumeInfo.PartID + "'" +
                                            " and ProductionOrderID='" + v_mesMaterialConsumeInfo.ProductionOrderID + "' " +
                                            " and LineID = '" + F_LoginList.LineID + "'" +
                                            " and StationID ='" + F_LoginList.StationID + "'" +
                                            " and SN = '" + S_FGSN + "'";
                        //DataTable DT_MCI = SqlServerHelper.Data_Table(S_Sql_MCI);
                        var tMCi = DapperConnRead2.ExecuteScalar(S_Sql_MCI);

                        if (tMCi != null)
                        {
                            S_Sql +=
                            "UPDATE mesMaterialConsumeInfo SET ConsumeQTY=isnull(ConsumeQTY,0)+1 where PartID=" + "\r\n" +
                                    "'" + v_mesMaterialConsumeInfo.PartID + "' " + "\r\n" +
                                    " and  ProductionOrderID='" + v_mesMaterialConsumeInfo.ProductionOrderID + "'" + "\r\n" +
                                    " and  LineID='" + F_LoginList.LineID + "'" + "\r\n" +
                                    " and  StationID='" + F_LoginList.StationID + "'" + "\r\n" +
                                    " and  SN='" + S_FGSN + "'" + "\r\n";
                        }
                        else
                        {
                            S_Sql +=
                            "INSERT INTO mesMaterialConsumeInfo(SN,MaterialTypeID,PartID,ProductionOrderID,LineID,StationID,ConsumeQTY)" + "\r\n" +
                            " Values(" + "'" + S_FGSN + "'" + "\r\n" +
                            ",'1'" + "\r\n" +
                            ",'" + v_mesMaterialConsumeInfo.PartID + "'" + "\r\n" +
                            ",'" + v_mesMaterialConsumeInfo.ProductionOrderID + "'" + "\r\n" +
                            ",'" + F_LoginList.LineID + "'" + "\r\n" +
                            ",'" + F_LoginList.StationID + "'" + "\r\n" +
                            ",1" + ")" + "\r\n";
                        }
                    }
                }


                if (List_mesMachine != null)
                {
                    for (int i = 0; i < List_mesMachine.Count; i++)
                    {
                        mesMachine v_mesMachine = List_mesMachine[i];
                        string S_MachineReult = GetModMachineBySNStationTypeID_Sql(v_mesMachine.SN, F_LoginList.StationTypeID ?? 0);

                        if (S_MachineReult != "")
                        {
                            if (S_MachineReult.Substring(0, 5) == "ERROR")
                            {
                                return S_Result = S_MachineReult;
                            }
                            else
                            {
                                S_Sql += S_MachineReult;
                            }
                        }
                    }
                }

                return await ExecuteTransactionSqlAsync(S_Sql);
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }
        private async Task<string> GetModMachineBySNStationTypeID_SqlAsync(string MachineSN, int StationTypeID)
        {
            string S_Result = "";

            string S_Sql = string.Empty;

            try
            {
                S_Sql = string.Format(@"select WarningStatus,ValidFrom,ValidTo,RuningStationTypeID,
                                    RuningCapacityQuantity,ValidDistribution from mesMachine where SN='{0}'", MachineSN);

                var tMesMachine = await DapperConnRead2.QueryAsync(S_Sql, null, null, I_DBTimeout, null);

                //DataTable dts = SqlServerHelper.ExecuteDataTable(S_Sql);

                if (tMesMachine != null || tMesMachine.Count() > 0)
                {
                    var tMesMachines = tMesMachine.ToList();
                    int WarningStatus = Convert.ToInt32(tMesMachines.ConvertDynamic("WarningStatus"));

                    int RuningCapacityQuantity = 0;
                    string sRuningCapacityQuantity = tMesMachines.ConvertDynamic("RuningCapacityQuantity");
                    if (!string.IsNullOrEmpty(sRuningCapacityQuantity))
                    {
                        RuningCapacityQuantity = Convert.ToInt32(sRuningCapacityQuantity);
                    }

                    string[] StationFromList = tMesMachines.ConvertDynamic("ValidFrom").Split(";");
                    string[] StationToList = tMesMachines.ConvertDynamic("ValidTo").Split(";");
                    string[] ValidDistributionList = tMesMachines.ConvertDynamic("ValidDistribution").Split(";");
                    string RuningStationTypeID = tMesMachines.ConvertDynamic("RuningStationTypeID");

                    if (WarningStatus == 1 || WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (RuningStationTypeID != StationTypeID.ToString())
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningStationTypeID={1},RuningCapacityQuantity=1 where SN = '{0}'", MachineSN, StationTypeID);
                            RuningCapacityQuantity = 1;
                            RuningStationTypeID = StationTypeID.ToString();
                        }
                        else
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningCapacityQuantity=isnull(RuningCapacityQuantity,0)+1 where SN = '{0}'", MachineSN);
                            RuningCapacityQuantity = RuningCapacityQuantity + 1;
                        }
                        S_Result += "\r\n" + S_Sql;
                    }

                    if (WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (StationFromList.Contains(StationTypeID.ToString()))
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningQuantity=isnull(RuningQuantity,0)+1 where SN = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }

                    if (StationFromList.Contains(StationTypeID.ToString()))
                    {
                        S_Sql = string.Format("Update mesMachine set StartRuningTime=GETDATE(),StatusID='2' where SN = '{0}'", MachineSN);
                        S_Result += "\r\n" + S_Sql;
                    }
                    if (StationToList.Contains(StationTypeID.ToString()))
                    {
                        if (WarningStatus == 1 || WarningStatus == 3)
                        {
                            int qty = 0;
                            foreach (string str in ValidDistributionList)
                            {
                                string[] strList = str.Split(',');
                                if (strList[0].ToString() == StationTypeID.ToString())
                                {
                                    qty = Convert.ToInt32(strList[1].ToString());
                                    break;
                                }
                            }

                            if (RuningCapacityQuantity >= qty && RuningStationTypeID == StationTypeID.ToString())
                            {
                                S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID=1 where SN = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                                S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                            }
                        }
                        else
                        {
                            S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID='1' where SN = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                            S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }

            return S_Result;
        }

        /// <summary>
        /// 拼装设备相关sql语句
        /// </summary>
        /// <param name="MachineSN"></param>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        private string GetModMachineBySNStationTypeID_Sql(string MachineSN, int StationTypeID)
        {
            string S_Result = "";

            string S_Sql = string.Empty;

            try
            {
                S_Sql = string.Format(@"select WarningStatus,ValidFrom,ValidTo,RuningStationTypeID,
                                    RuningCapacityQuantity,ValidDistribution from mesMachine where SN='{0}'", MachineSN);

                var tMesMachine = DapperConnRead2.Query(S_Sql, null, null, true, I_DBTimeout, null);

                //DataTable dts = SqlServerHelper.ExecuteDataTable(S_Sql);

                if (tMesMachine != null || tMesMachine.Count() > 0)
                {
                    var tMesMachines = tMesMachine.ToList();
                    int WarningStatus = Convert.ToInt32(tMesMachines.ConvertDynamic("WarningStatus"));

                    int RuningCapacityQuantity = 0;
                    string sRuningCapacityQuantity = tMesMachines.ConvertDynamic("RuningCapacityQuantity");
                    if (!string.IsNullOrEmpty(sRuningCapacityQuantity))
                    {
                        RuningCapacityQuantity = Convert.ToInt32(sRuningCapacityQuantity);
                    }

                    string[] StationFromList = tMesMachines.ConvertDynamic("ValidFrom").Split(";");
                    string[] StationToList = tMesMachines.ConvertDynamic("ValidTo").Split(";");
                    string[] ValidDistributionList = tMesMachines.ConvertDynamic("ValidDistribution").Split(";");
                    string RuningStationTypeID = tMesMachines.ConvertDynamic("RuningStationTypeID");

                    if (WarningStatus == 1 || WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (RuningStationTypeID != StationTypeID.ToString())
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningStationTypeID={1},RuningCapacityQuantity=1 where SN = '{0}'", MachineSN, StationTypeID);
                            RuningCapacityQuantity = 1;
                            RuningStationTypeID = StationTypeID.ToString();
                        }
                        else
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningCapacityQuantity=isnull(RuningCapacityQuantity,0)+1 where SN = '{0}'", MachineSN);
                            RuningCapacityQuantity = RuningCapacityQuantity + 1;
                        }
                        S_Result += "\r\n" + S_Sql;
                    }

                    if (WarningStatus == 2 || WarningStatus == 3)
                    {
                        if (StationFromList.Contains(StationTypeID.ToString()))
                        {
                            S_Sql = string.Format(@"Update mesMachine set RuningQuantity=isnull(RuningQuantity,0)+1 where SN = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }

                    if (StationFromList.Contains(StationTypeID.ToString()))
                    {
                        S_Sql = string.Format("Update mesMachine set StartRuningTime=GETDATE(),StatusID='2' where SN = '{0}'", MachineSN);
                        S_Result += "\r\n" + S_Sql;
                    }
                    if (StationToList.Contains(StationTypeID.ToString()))
                    {
                        if (WarningStatus == 1 || WarningStatus == 3)
                        {
                            int qty = 0;
                            foreach (string str in ValidDistributionList)
                            {
                                string[] strList = str.Split(',');
                                if (strList[0].ToString() == StationTypeID.ToString())
                                {
                                    qty = Convert.ToInt32(strList[1].ToString());
                                    break;
                                }
                            }

                            if (RuningCapacityQuantity >= qty && RuningStationTypeID == StationTypeID.ToString())
                            {
                                S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID=1 where SN = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                                S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                                S_Result += "\r\n" + S_Sql;
                            }
                        }
                        else
                        {
                            S_Sql = string.Format("Update mesMachine set LastRuningTime=GETDATE(),StatusID='1' where SN = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                            S_Sql = string.Format(" UPDATE mesUnitDetail SET reserved_03 = '2' WHERE reserved_03 = '1' AND reserved_01 = '{0}'", MachineSN);
                            S_Result += "\r\n" + S_Sql;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }

            return S_Result;
        }
        /// <summary>
        /// 包装入库提交数据
        /// </summary>
        /// <param name="strSNFormat"></param>
        /// <param name="xmlProdOrder"></param>
        /// <param name="xmlPart"></param>
        /// <param name="xmlStation"></param>
        /// <param name="xmlExtraData"></param>
        /// <param name="strSNbuf"></param>
        /// <returns></returns>
        public async Task<string> uspSetPackageData(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string outputStr = string.Empty;
            try
            {
                string sql = string.Empty;
                sql = @$"
                     declare	@prodID				int,
                            @partID				int,
                            @stationID			int,
                            @routeID			int,
                            @lineID				int,
                            @stationTypeID		int,
                            @UnitStateID		int,
                            @EmployeeId			int,
                            @PackageID			int,
                            @stage				int,
                            @idoc				int,
                            @strOutput          varchar(50)
                    SET @strOutput='1'

                    SELECT top 1 @PackageID=ID,@stage=Stage FROM mesPackage WHERE SerialNumber='{strSNFormat}'
                    IF ISNULL(@PackageID,'')=''
                    BEGIN
                        SET @strOutput='20012'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                        RETURN
                    END
    

                        DECLARE @tmpXmlProdOrder VARCHAR(50) = '{xmlProdOrder}', @tmpXmlPart VARCHAR(50) = '{xmlPart}', @tmpXmlStation VARCHAR(50) = '{xmlStation}',@tmpXmlExtraData VARCHAR(200)='{xmlExtraData}' 

                        --读取xmlPart参数值
                        exec sp_xml_preparedocument @idoc output, @tmpXmlPart
                        SELECT @partID=PartId
                        FROM   OPENXML (@idoc,			'/Part',2)
                                WITH (PartId  int		'@PartID')   
                        exec sp_xml_removedocument @idoc
                        IF isnull(@partID,'') = ''
                        begin
                            SET @strOutput='20077'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                            RETURN
                        end

                        --读取ProdOrder参数值
                        exec sp_xml_preparedocument @idoc output, @tmpXmlProdOrder
                        SELECT @prodID=ProdId
                        FROM   OPENXML (@idoc,			'/ProdOrder',2)
                                WITH (ProdId  int		'@ProdOrderID')   
                        exec sp_xml_removedocument @idoc
                        IF isnull(@prodID,'') = ''
                        BEGIN
                            SET @strOutput= '20077'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
			                RETURN
                        END

                        exec sp_xml_preparedocument @idoc output, @tmpXmlStation
                        SELECT @stationID=StationId
                        FROM   OPENXML (@idoc, '/Station',2)
                                WITH (StationId		  int			'@StationId')   
                        exec sp_xml_removedocument @idoc
                        IF isnull(@stationID,'') = ''
                        BEGIN
                            SET @strOutput= '20077'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                            RETURN
                        END

                        exec sp_xml_preparedocument @idoc output, @tmpXmlExtraData
                        SELECT @EmployeeId=EmployeeId
                        FROM   OPENXML (@idoc, '/ExtraData',2)
                                WITH (EmployeeId		  int		'@EmployeeId')   
                        exec sp_xml_removedocument @idoc
                        IF isnull(@stationID,'') = ''
                        BEGIN
                            SET @strOutput= '20077'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                            RETURN
                        END
    

                        --获取RouteID
                        SELECT TOP 1 @lineID = LineID,@stationTypeID=StationTypeID 
                            FROM mesStation WHERE ID=@stationID
                        IF ISNULL(@lineID,'')=''
                        BEGIN
                            SET @strOutput='20137'
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                            RETURN
                        END
                        SELECT @stationID stationID, @strOutput strOutput,@EmployeeId EmployeeId,@stationID stationID,@partID partID,@PackageID PackageID,@lineID lineID,@stationTypeID stationTypeID,@stage stage
                ";


                var tmpReader1 = await DapperConnRead2.ExecuteScalarAsync<ProcOutput>(sql, null, null, I_DBTimeout, null);
                if (tmpReader1.strOutput != "1")
                    return tmpReader1.strOutput;


                var routeId = await v_Public_Repository.ufnRTEGetRouteIDAsync(tmpReader1.lineID, tmpReader1.partID, 0, tmpReader1.prodID);
                if (routeId == "" || routeId.ToInt() <= 0)
                    return "20195";


                var nextUnitStateId = await v_Public_Repository.ufnGetUnitStateIDAsync(routeId.ToInt(), tmpReader1.stationTypeID, 1);

                if (nextUnitStateId == "" || nextUnitStateId.ToInt() == 0)
                    return "20131";

                sql = $@"
                      begin
                         declare	@prodID				int = {tmpReader1.prodID},
                         @partID				int = {tmpReader1.partID},
                         @stationID			int = {tmpReader1.stationID},
                         @routeID			int = {routeId},
                         @lineID				int = {tmpReader1.lineID},
                         @stationTypeID		int = {tmpReader1.stationTypeID},
                         @UnitStateID		int ={nextUnitStateId},
                         @EmployeeId			int = {tmpReader1.EmployeeId},
                         @PackageID			int = {tmpReader1.PackageID},
                         @stage				int = {tmpReader1.stage},
                         @idoc				int,
                         @strOutput          varchar(50)
                     SET @strOutput='1'

                     BEGIN TRY

                         --修改Package
                         UPDATE mesPackage SET StationID=@stationID,LastUpdate=GETDATE(),EmployeeID=@EmployeeId WHERE ID=@PackageID

                         INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
                         VALUES (@PackageID,1,@stationID,@EmployeeId,GETDATE())

                         IF isnull(@stage,'')=1
                         BEGIN
                             --修改mesUnit
                             UPDATE D SET D.StationID=@stationID,UnitStateID=@UnitStateID,EmployeeID=@EmployeeId,d.LastUpdate = GETDATE() FROM mesUnit D
                             INNER JOIN mesUnitDetail C ON C.UnitID=D.ID  
                             INNER JOIN mesPackage A ON ISNULL(C.InmostPackageID,'')=A.ID
                             WHERE A.ID=@PackageID

                             --mesHistory记录
                             INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
                             SELECT D.ID,@UnitStateID,@EmployeeId,@stationID,GETDATE(),GETDATE(),@prodID,@partID,1,1 FROM mesPackage A  
                             INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=A.ID
                             INNER JOIN mesUnit D ON C.UnitID=D.ID WHERE A.ID=@PackageID
                         END
                         ELSE
                         BEGIN
                             --修改mesUnit
                             UPDATE D SET D.StationID=@stationID,UnitStateID=@UnitStateID,EmployeeID=@EmployeeId,d.LastUpdate=GETDATE() FROM mesPackage A
                             INNER JOIN mesPackage B ON A.ID=(CASE WHEN A.Stage=3 THEN ISNULL(B.ShipmentParentID,'') 
										                                        ELSE ISNULL(B.ParentID,'') END)
                             INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=B.ID
                             INNER JOIN mesUnit D ON C.UnitID=D.ID
                             where A.ID = @PackageID

                             --mesHistory记录
                             INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
                             SELECT D.ID,@UnitStateID,@EmployeeId,@stationID,GETDATE(),GETDATE(),@prodID,@partID,1,1 FROM mesPackage A
                             INNER JOIN mesPackage B ON A.ID=(CASE WHEN A.Stage=3 THEN ISNULL(B.ShipmentParentID,'') 
										                                        ELSE ISNULL(B.ParentID,'') END)
                             INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=B.ID
                             INNER JOIN mesUnit D ON C.UnitID=D.ID
                             where A.ID = @PackageID
                         END
         
                     END TRY
                     BEGIN CATCH
                         IF @@TRANCOUNT >0 
                         BEGIN
                             ROLLBACK TRANSACTION
                         END
                         SET @strOutput = ERROR_MESSAGE()
                         select @strOutput output
                         RETURN
                     END CATCH
                     select @strOutput output
                 END";

                var tmpReader = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
                outputStr = tmpReader?.ToString();
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "data commit", ex);
                return "0";
            }

            return outputStr;
        }

        /// <summary>
        /// 包装入库提交数据 备份
        /// </summary>
        /// <param name="strSNFormat"></param>
        /// <param name="xmlProdOrder"></param>
        /// <param name="xmlPart"></param>
        /// <param name="xmlStation"></param>
        /// <param name="xmlExtraData"></param>
        /// <param name="strSNbuf"></param>
        /// <returns></returns>
        public async Task<string> uspSetPackageData_P(string @strSNFormat, string @xmlProdOrder, string @xmlPart, string @xmlStation, string @xmlExtraData, string @strSNbuf)
        {
            string outputStr = string.Empty;
            try
            {
                string sql = string.Empty;
                sql = @$"
                        begin
                                    declare	@prodID				int,
			                        @partID				int,
			                        @stationID			int,
			                        @routeID			int,
			                        @lineID				int,
			                        @stationTypeID		int,
			                        @UnitStateID		int,
			                        @EmployeeId			int,
			                        @PackageID			int,
			                        @stage				int,
			                        @idoc				int,
			                        @strOutput          varchar(50)
	                        SET @strOutput='1'

	                        SELECT top 1 @PackageID=ID,@stage=Stage FROM mesPackage WHERE SerialNumber='{strSNFormat}'
	                        IF ISNULL(@PackageID,'')=''
	                        BEGIN
		                        SET @strOutput='20012'
                                select @strOutput output
		                        RETURN
	                        END
	                        

	                        BEGIN TRY
                                DECLARE @tmpXmlProdOrder VARCHAR(50) = '{xmlProdOrder}', @tmpXmlPart VARCHAR(50) = '{xmlPart}', @tmpXmlStation VARCHAR(50) = '{xmlStation}',@tmpXmlExtraData VARCHAR(200)='{xmlExtraData}' 

		                        --读取xmlPart参数值
		                        exec sp_xml_preparedocument @idoc output, @tmpXmlPart
		                        SELECT @partID=PartId
		                        FROM   OPENXML (@idoc,			'/Part',2)
				                        WITH (PartId  int		'@PartID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@partID,'') = ''
		                        begin
			                        SET @strOutput='20077'
                                    select @strOutput output
		                            RETURN
		                        end

		                        --读取ProdOrder参数值
		                        exec sp_xml_preparedocument @idoc output, @tmpXmlProdOrder
		                        SELECT @prodID=ProdId
		                        FROM   OPENXML (@idoc,			'/ProdOrder',2)
				                        WITH (ProdId  int		'@ProdOrderID')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@prodID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput output
		                            RETURN
		                        END

		                        exec sp_xml_preparedocument @idoc output, @tmpXmlStation
		                        SELECT @stationID=StationId
		                        FROM   OPENXML (@idoc, '/Station',2)
				                        WITH (StationId		  int			'@StationId')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@stationID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput output
		                            RETURN
		                        END

		                        exec sp_xml_preparedocument @idoc output, @tmpXmlExtraData
		                        SELECT @EmployeeId=EmployeeId
		                        FROM   OPENXML (@idoc, '/ExtraData',2)
				                        WITH (EmployeeId		  int		'@EmployeeId')   
		                        exec sp_xml_removedocument @idoc
		                        IF isnull(@stationID,'') = ''
		                        BEGIN
			                        SET @strOutput= '20077'
                                    select @strOutput output
		                            RETURN
		                        END
	                        

		                        --获取RouteID
		                        SELECT TOP 1 @lineID = LineID,@stationTypeID=StationTypeID 
			                        FROM mesStation WHERE ID=@stationID
		                        IF ISNULL(@lineID,'')=''
		                        BEGIN
			                        SET @strOutput='20137'
                                    select @strOutput output
		                            RETURN
		                        END

		                        SET @routeID= DBO.ufnRTEGetRouteID(@lineID,@partID,'',@prodID)
		                        IF ISNULL(@routeID,'')=''
		                        BEGIN
			                        SET @strOutput='20195'
                                    select @strOutput output
		                            RETURN
		                        END

		                        SET @UnitStateID = dbo.ufnGetUnitStateID(@routeID,@stationTypeID,1)

		                        IF ISNULL(@UnitStateID,'')=''
		                        BEGIN
			                        SET @strOutput='20131'
                                    select @strOutput output
		                            RETURN
		                        END
		                        --IF EXISTS (SELECT 1 FROM mesRoute WHERE ID=@routeID AND RouteType=1)
		                        --BEGIN
		                        --	SELECT TOP 1 @UnitStateID=OutputStateID FROM mesUnitOutputState 
		                        --		WHERE StationTypeID=@stationTypeID AND RouteID=@routeID
		                        --END
		                        --ELSE
		                        --BEGIN
		                        --	--RouteDetail临时表
		                        --	SELECT ROW_NUMBER() OVER(ORDER BY (cast(Sequence as int))) ID ,StationTypeID,UnitStateID into #PackageRoute
		                        --	FROM mesRouteDetail where RouteID=@routeID

		                        --	SELECT top 1 @UnitStateID=UnitStateID FROM #PackageRoute WHERE StationTypeID=@stationTypeID
		                        --	DROP TABLE #PackageRoute
		                        --END

		                        IF ISNULL(@UnitStateID,'')=''
		                        BEGIN
			                        SET @strOutput='20203'
                                    select @strOutput output
		                            RETURN
		                        END

		                        --修改Package
		                        UPDATE mesPackage SET StationID=@stationID,LastUpdate=GETDATE(),EmployeeID=@EmployeeId WHERE ID=@PackageID

		                        INSERT INTO mesPackageHistory(PackageID,PackageStatusID,StationID,EmployeeID,Time)
		                        VALUES (@PackageID,1,@stationID,@EmployeeId,GETDATE())

		                        IF isnull(@stage,'')=1
		                        BEGIN
			                        --修改mesUnit
			                        UPDATE D SET D.StationID=@stationID,UnitStateID=@UnitStateID,EmployeeID=@EmployeeId,d.LastUpdate = GETDATE() FROM mesUnit D
			                        INNER JOIN mesUnitDetail C ON C.UnitID=D.ID  
			                        INNER JOIN mesPackage A ON ISNULL(C.InmostPackageID,'')=A.ID
			                        WHERE A.ID=@PackageID

			                        --mesHistory记录
			                        INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
			                        SELECT D.ID,@UnitStateID,@EmployeeId,@stationID,GETDATE(),GETDATE(),@prodID,@partID,1,1 FROM mesPackage A  
			                        INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=A.ID
			                        INNER JOIN mesUnit D ON C.UnitID=D.ID WHERE A.ID=@PackageID
		                        END
		                        ELSE
		                        BEGIN
			                        --修改mesUnit
			                        UPDATE D SET D.StationID=@stationID,UnitStateID=@UnitStateID,EmployeeID=@EmployeeId,d.LastUpdate=GETDATE() FROM mesPackage A
			                        INNER JOIN mesPackage B ON A.ID=(CASE WHEN A.Stage=3 THEN ISNULL(B.ShipmentParentID,'') 
												                        ELSE ISNULL(B.ParentID,'') END)
			                        INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=B.ID
			                        INNER JOIN mesUnit D ON C.UnitID=D.ID
			                        where A.ID = @PackageID

			                        --mesHistory记录
			                        INSERT INTO mesHistory(UnitID,UnitStateID,EmployeeID,StationID,EnterTime,ExitTime,ProductionOrderID,PartID,LooperCount,StatusID)
			                        SELECT D.ID,@UnitStateID,@EmployeeId,@stationID,GETDATE(),GETDATE(),@prodID,@partID,1,1 FROM mesPackage A
			                        INNER JOIN mesPackage B ON A.ID=(CASE WHEN A.Stage=3 THEN ISNULL(B.ShipmentParentID,'') 
												                        ELSE ISNULL(B.ParentID,'') END)
			                        INNER JOIN mesUnitDetail C ON ISNULL(C.InmostPackageID,'')=B.ID
			                        INNER JOIN mesUnit D ON C.UnitID=D.ID
			                        where A.ID = @PackageID
		                        END
		                        
	                        END TRY
	                        BEGIN CATCH
		                        IF @@TRANCOUNT >0 
		                        BEGIN
			                        ROLLBACK TRANSACTION
		                        END
		                        SET @strOutput = ERROR_MESSAGE()
                                select @strOutput output
		                        RETURN
	                        END CATCH
                            select @strOutput output
                        END";

                var tmpReader = await DapperConnRead2.ExecuteScalarAsync(sql, null, null, I_DBTimeout, null);
                outputStr = tmpReader?.ToString();
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "data commit", ex);
                return "0";
            }

            return outputStr;
        }

        /// <summary>
        /// 栈板包装
        /// </summary>
        /// <param name="PartID"></param>
        /// <param name="ProductionOrderID"></param>
        /// <param name="S_CartonSN"></param>
        /// <param name="S_PalletSN"></param>
        /// <param name="loginList"></param>
        /// <param name="PalletQty"></param>
        /// <param name="CurrentQty"> 等于0则箱码正常装栈板  大于0时则尾栈板强制封装</param>
        /// <returns>错误代码，实际栈板装箱数</returns>
        public async Task<(string,int)> uspPalletPackaging(string PartID, string ProductionOrderID, string S_CartonSN, string S_PalletSN, LoginList loginList, int PalletQty,int CurrentQty = 0)
        {
            int boxCount = -1;

            var palletInfo = await v_Public_Repository.GetMesPackageBySNAsync(S_PalletSN);
            if (palletInfo is null or { StatusID : 1 })
                return ("20119", boxCount);


            if (CurrentQty > 0)
            {
                //尾箱
                string endSql = $@"INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                                VALUES({palletInfo.ID}, 2, {loginList.StationID}, {loginList.EmployeeID}, GETDATE())
                                UPDATE mesPackage SET CurrentCount={CurrentQty},StationID={loginList.StationID},EmployeeID={loginList.EmployeeID},
				                                                StatusID=1,LastUpdate=GETDATE() where ID= {palletInfo.ID}
                                    ";
                var  affactC = await DapperConn.ExecuteAsync(endSql, null, null, I_DBTimeout, null);
                if (affactC <= 0)
                    return ("20121", boxCount);
            }
            else
            {
                var cartonInfo = await v_Public_Repository.GetMesPackageBySNAsync(S_CartonSN);
                if (cartonInfo is null or { StatusID : not 1})
                    return ("20204", boxCount);

                var partInfo = await v_Public_Repository.GetmesPartAsync(PartID);
                var unitStateId = await v_Public_Repository.GetMesUnitState(PartID, partInfo.PartFamilyID.ToString(), loginList.LineID.ToString(), loginList.StationTypeID.ToInt(), ProductionOrderID, "1");


                var linkCount = await DapperConn.ExecuteScalarAsync($"SELECT COUNT(1) FROM dbo.mesPackage WHERE ParentID = {palletInfo.ID}", null, null, I_DBTimeout, null);
                if (linkCount is null)
                    return ("20122", boxCount);
                boxCount = linkCount.ToInt();

                string sql = string.Empty;
       
                using (var con = DapperConn)
                {
                    if (con?.State == ConnectionState.Closed) {
                        con.Open();
                    }
                    if (con.State != ConnectionState.Open)
                        return ("20013", boxCount);

                    using (var tran = con.BeginTransaction())
                    {
                        try
                        {
                            int updateCount = 0;
                            if (palletInfo.Stage == 3)
                            {
                                sql = $@"UPDATE A SET A.ShipmentParentID={palletInfo.ID}  FROM mesPackage A WHERE A.SerialNumber='{S_CartonSN}'
                                INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                                                                VALUES({palletInfo.ID}, 8, {loginList.StationID}, {loginList.EmployeeID}, GETDATE())
                                ";
                            }
                            else
                            {
                                sql = $@"UPDATE A SET A.ParentID={palletInfo.ID}  FROM mesPackage A WHERE A.SerialNumber='{S_CartonSN}'
                                INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                                    VALUES({cartonInfo.ID}, 1, {loginList.StationID}, {loginList.EmployeeID}, GETDATE())
                                UPDATE mesPackage SET CurrentCount={++boxCount},StationID={loginList.StationID},EmployeeID={loginList.EmployeeID},
				                                                StatusID=0,LastUpdate=GETDATE() where ID={palletInfo.ID}
                                ";


                                if (boxCount == PalletQty)
                                {
                                    sql += $@"INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)
                                VALUES({palletInfo.ID}, 2, {loginList.StationID}, {loginList.EmployeeID}, GETDATE())
                                UPDATE mesPackage SET CurrentCount={PalletQty},StationID={loginList.StationID},EmployeeID={loginList.EmployeeID},
				                                                StatusID=1,LastUpdate=GETDATE() where ID={palletInfo.ID}
                                ";

                                }
                                updateCount = await con.ExecuteAsync(sql, null, tran, I_DBTimeout, null);
                                if (updateCount <= 0)
                                {
                                    tran.Rollback();
                                    return ("20121", boxCount);
                                }

                            }

                            string historySql = $@"UPDATE d SET d.UnitStateID={unitStateId},StatusID=1,d.StationID={loginList.StationID} ,d.LineID={loginList.LineID}, d.EmployeeID={loginList.EmployeeID},d.LastUpdate = GETDATE()
                                FROM mesPackage A
                                INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                                INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                                INNER JOIN dbo.mesUnit d ON d.ID = B.UnitID
                                WHERE A.SerialNumber='{S_CartonSN}'
                                ";
                            historySql += $@"UPDATE d SET d.UnitStateID={unitStateId},StatusID=1,d.StationID={loginList.StationID} ,d.LineID={loginList.LineID}, d.EmployeeID={loginList.EmployeeID},d.LastUpdate = GETDATE()
                                FROM mesPackage A
                                INNER JOIN mesUnitDetail B ON A.ID = B.InmostPackageID
                                INNER JOIN mesSerialNumber C ON c.Value = b.KitSerialNumber
                                INNER JOIN dbo.mesUnit d ON d.ID = c.UnitID
                                WHERE A.SerialNumber = '{S_CartonSN}'
                                ";
                            historySql += $@"INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                                SELECT b.UnitID,{unitStateId},{loginList.EmployeeID},{loginList.StationID},GETDATE(),GETDATE(),{ProductionOrderID},{PartID},1,1
                                FROM mesPackage A
                                INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                                INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                                INNER JOIN dbo.mesUnit d ON d.ID = B.UnitID
                                WHERE A.SerialNumber='{S_CartonSN}'
                                ";
                            historySql += $@"INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                                SELECT C.UnitID,{unitStateId},{loginList.EmployeeID},{loginList.StationID},GETDATE(),GETDATE(),{ProductionOrderID},{PartID},1,1
                                FROM mesPackage A
                                INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                                INNER JOIN mesSerialNumber C ON c.Value = b.KitSerialNumber
                                INNER JOIN dbo.mesUnit d ON d.ID = c.UnitID
                                WHERE A.SerialNumber='{S_CartonSN}'
                                ";
                            updateCount = await con.ExecuteAsync(historySql, null, tran, I_DBTimeout, null);
                            if (updateCount <= 0)
                            {
                                tran.Rollback();
                                return ("20126", boxCount);
                            }
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "data commit", ex);
                            return ("0", boxCount);
                        }
                    }
                }
            }
            
            return ("1", boxCount);
        }
        public async Task<string> uspUpdateBoxWeight(string PartID, string ProductionOrderID, string S_CartonSN, LoginList loginList, double boxWeight)
        {

            var cartonInfo = await v_Public_Repository.GetMesPackageBySNAsync(S_CartonSN);
            if (cartonInfo is null or { StatusID: not 1 })
                return "20204";

            var partInfo = await v_Public_Repository.GetmesPartAsync(PartID);
            var unitStateId = await v_Public_Repository.GetMesUnitState(PartID, partInfo.PartFamilyID.ToString(), loginList.LineID.ToString(), loginList.StationTypeID.ToInt(), ProductionOrderID, "1");

            if (string.IsNullOrEmpty(unitStateId))
                return "20018";

            string sql = @$"UPDATE d SET d.UnitStateID={unitStateId},StatusID=1,d.StationID={loginList.StationID} ,d.LineID={loginList.LineID}, d.EmployeeID={loginList.EmployeeID},d.LastUpdate = GETDATE()
                            FROM mesPackage A
                            INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                            INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                            INNER JOIN dbo.mesUnit d ON d.ID = B.UnitID
                            WHERE A.SerialNumber='{S_CartonSN}'

                            UPDATE d SET d.UnitStateID={unitStateId},StatusID=1,d.StationID={loginList.StationID} ,d.LineID={loginList.LineID}, d.EmployeeID={loginList.EmployeeID},d.LastUpdate = GETDATE()
                            FROM mesPackage A
                            INNER JOIN mesUnitDetail B ON A.ID = B.InmostPackageID
                            INNER JOIN mesSerialNumber C ON c.Value = b.KitSerialNumber
                            INNER JOIN dbo.mesUnit d ON d.ID = c.UnitID
                            WHERE A.SerialNumber = '{S_CartonSN}'

                            INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                            SELECT b.UnitID,{unitStateId},{loginList.EmployeeID},{loginList.StationID},GETDATE(),GETDATE(),{ProductionOrderID},{PartID},1,1
                            FROM mesPackage A
                            INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                            INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                            INNER JOIN dbo.mesUnit d ON d.ID = B.UnitID
                            WHERE A.SerialNumber='{S_CartonSN}'

                            INSERT INTO dbo.mesHistory(UnitID, UnitStateID, EmployeeID, StationID, EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID)
                            SELECT C.UnitID,{unitStateId},{loginList.EmployeeID},{loginList.StationID},GETDATE(),GETDATE(),{ProductionOrderID},{PartID},1,1
                            FROM mesPackage A
                            INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                            INNER JOIN mesSerialNumber C ON c.Value = b.KitSerialNumber
                            INNER JOIN dbo.mesUnit d ON d.ID = c.UnitID
                            WHERE A.SerialNumber='{S_CartonSN}'

                            INSERT INTO dbo.mesPackageDetail
                            (
                                PackageID,
                                PackageDetailDefID,
                                Content
                            )
                            SELECT a.ID AS PackageID,b.ID AS PackageDetailDefID,'{boxWeight}' AS Content
					        FROM dbo.mesPackage a,dbo.luPackageDetailDef b
					        WHERE a.SerialNumber = '{S_CartonSN}' AND 
					        b.Description = 'PackBoxWeight'
";

            var output = await DapperConn.ExecuteAsync(sql, null, null, I_DBTimeout, null);

            if (output <= 0)
                return P_MSG_Public.MSG_Public_6039;

            return "1";
        }
        public async Task<string> uspPalletPackagingAsync(string PartID, string ProductionOrderID, string S_CartonSN, string S_PalletSN, LoginList loginList, string S_BillNO, string PrintType, string ShipmentDetailID, bool scanOver = false, int PalletQty = 0 )
        {
            try
            {
                //ShipMentDetailData shipMentData = new ShipMentDetailData();
                //string sql = $@"SELECT top 1 * FROM mesPackage WHERE SerialNumber='{S_PalletSN}' AND StatusID=0";

                //var palletEntity = await DapperConn.QueryFirstAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
                //if (palletEntity is null or { ID: <= 0 })
                //    return "20119";

                //sql = $@"select top 1 *  from CO_WH_ShipmentEntryNew where FDetailID={ShipmentDetailID}";

                //var shipmentEntryNew = await DapperConn.QueryFirstAsync<CO_WH_ShipmentEntryNew>(sql, null, null, I_DBTimeout, null);
                //if (shipmentEntryNew is null or { FInterID: <= 0 })
                //    return "20119";

                //string S_ShipmentPalletID = palletEntity.ID.ToString();
                string S_Sql = String.Empty;
                S_Sql = @"BEGIN TRY
                        BEGIN TRAN
";
                //if (PrintType == "1")
                //{
                //    S_Sql += "DECLARE @ShipmentDetailID  INT" + "\r\n" +
                //      ", @CurrProductionOrderID int" + "\r\n" +
                //       ", @OutSNCount int" + "\r\n" +
                //        "SELECT @CurrProductionOrderID=CurrProductionOrderID FROM mesPackage WHERE SerialNumber = '" + S_CartonSN + "'" + "\r\n" +
                //        "SELECT @ShipmentDetailID = A.FDetailID FROM CO_WH_ShipmentEntryNew A" + "\r\n" +
                //            "INNER JOIN CO_WH_ShipmentNew B ON A.FInterID = B.FInterID WHERE B.FBillNO ='" + S_BillNO + "' AND EXISTS(" + "\r\n" +
                //             "SELECT * FROM (SELECT A.Content FROM mesProductionOrderDetail A" + "\r\n" +
                //             "INNER JOIN luProductionOrderDetailDef B ON A.ProductionOrderDetailDefID= B.ID" + "\r\n" +
                //            "WHERE B.Description= 'MPN' AND A.ProductionOrderID= @CurrProductionOrderID) WS WHERE WS.Content = A.FMPNNO AND A.FStatus in (0, 1))" +
                //            "\r\n" + "\r\n" +
                //        "if ISNULL(@ShipmentDetailID,0) > 0" + "\r\n" +
                //        "  begin" + "\r\n" +
                //        "       UPDATE A SET A.ShipmentTime=GETDATE()," + "\r\n" +
                //        "           ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID='" + ShipmentDetailID + "') " + "\r\n" +
                //        "       FROM mesPackage A WHERE SerialNumber ='" + S_PalletSN + "'" + "\r\n" +
                //        "  end" + "\r\n";


                //    if (PalletQty == 0)
                //    {
                //        //////////////////////////////////////////////////////////////
                //        ///SetShipmentMultipack
                //        S_Sql += $@"UPDATE A SET A.ShipmentDetailID ={ShipmentDetailID},A.ShipmentTime=GETDATE(),
                //    ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID}) 
                //   FROM mesPackage A WHERE SerialNumber = '{S_CartonSN}'
                //   UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(FOutSN,0)+1 WHERE FDetailID={ShipmentDetailID} and FStatus in (0,1)
                //   IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID} AND FOutSN = FCTN and FStatus in (0,1))
                //   BEGIN
                //    UPDATE CO_WH_ShipmentEntryNew SET FStatus=2 WHERE FDetailID={ShipmentDetailID}

                //    IF NOT EXISTS( SELECT 1 FROM CO_WH_ShipmentEntryNew A
                //     INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
                //     WHERE B.FBillNO='{S_BillNO}' AND (A.FOutSN<>a.FCTN))
                //    BEGIN
                //     UPDATE CO_WH_ShipmentNew SET FStatus=2 WHERE FBillNO='{S_BillNO}'
                //    END
                //   END
                //                ";
                //        ////////////////////////////////////////////////////////////////////
                //    }
                //}
                //else if (PrintType == "2")
                //{
                //    if (PalletQty == 0)
                //    {
                //        S_Sql += $@"UPDATE mesPackage SET ShipmentInterID={shipmentEntryNew.FInterID},ShipmentTime=GETDATE(),ShipmentDetailID={shipmentEntryNew.FDetailID} WHERE SerialNumber='{S_PalletSN}'
                //                    ";
                //    }
                //}

                //if (PalletQty > 0)
                //{
                //    S_Sql += "INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time) Values(" + "\r\n" +
                //       "'" + S_ShipmentPalletID + "'" + "\r\n" +
                //       ",'2'" + "\r\n" +
                //       ",'" + loginList.StationID + "'" + "\r\n" +
                //       ",'" + loginList.EmployeeID + "'" + "\r\n" +
                //       ", GETDATE()" + "\r\n" +
                //       ")" + "\r\n" +

                //       "UPDATE mesPackage SET CurrentCount='" + PalletQty + "'" + "\r\n" +
                //       ",StationID='" + loginList.StationID + "',EmployeeID='" + loginList.EmployeeID + "',StatusID=1" + "\r\n" +
                //       ",LastUpdate=GETDATE() where ID='" + S_ShipmentPalletID + "'" + "\r\n";


                //    if (PrintType == "1")
                //    {

                //        /////////////////////////////////////////////////////////////////////////////
                //        ///GetIsOutCountComplete
                //        S_Sql += $@"DECLARE @tmpFInterId INT, @tmpShipmentParentId INT, @tmpFCTN INT, @tmpOutCount INT, @tmpScanOver BIT ='{scanOver}'
                //        SELECT @tmpFInterId = FInterID FROM CO_WH_ShipmentNew WHERE FBillNO = '{S_BillNO}'
                //        SELECT @tmpShipmentParentId = Id FROM mesPackage WHERE SerialNumber = '{S_PalletSN}'

                //        SELECT @tmpFCTN = ISNULL(FCTN,0)  FROM CO_WH_ShipmentEntryNew  WHERE FInterID = @tmpFInterId AND FDetailID = '{ShipmentDetailID}'
                //        SELECT @tmpOutCount = COUNT(1)  FROM mesPackage WHERE ShipmentParentID = @tmpShipmentParentId AND ShipmentDetailID = '{ShipmentDetailID}'

                //        IF @tmpFCTN = @tmpOutCount AND @tmpFCTN <> 0
                //        BEGIN
                //            IF	@tmpScanOver <> 1
                //         BEGIN
                //             RAISERROR(N'Scan count error',16,1)
                //         END
                //            UPDATE CO_WH_ShipmentEntryNew SET FStatus=2 WHERE FDetailID='{ShipmentDetailID}'
                //            UPDATE mesPackage SET ShipmentInterID={shipmentEntryNew.FInterID},ShipmentTime=GETDATE() WHERE SerialNumber='{S_PalletSN}'""
                //        END
                //        ";
                //        /////////////////////////////////////////////////////////////////////////////
                //    }
                //    else if (PrintType == "2")
                //    {
                //        S_Sql += $@" UPDATE mesPackage SET ShipmentInterID = {shipmentEntryNew.FInterID}, ShipmentTime = GETDATE(), ShipmentDetailID = {ShipmentDetailID} WHERE SerialNumber = '{S_PalletSN}'
                //                    ";
                //    }
                //}
                //else
                //{
                //    S_Sql += "DECLARE @Stage int " + "\r\n" +
                //            " SELECT @Stage=Stage FROM mesPackage WHERE ID='" + S_ShipmentPalletID + "'" + "\r\n" +
                //            " if @Stage<>3 " + "\r\n" +   //关联包装信息
                //            " Begin " + "\r\n" +
                //            "    UPDATE A SET A.ParentID = '" + S_ShipmentPalletID + "'  FROM mesPackage A WHERE A.SerialNumber = '" +
                //                    S_CartonSN + "'" + "\r\n" +
                //            " End else  Begin" + "\r\n";
                //    //关联包装信息
                //    if (PrintType == "1")
                //    {

                //        S_Sql += "     UPDATE A SET A.ShipmentParentID='" + S_ShipmentPalletID + "',A.ShipmentDetailID = '" + ShipmentDetailID + "',A.ShipmentTime=GETDATE()," + "\r\n" +
                //                         "  ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID='" + ShipmentDetailID + "')" + "\r\n" +
                //                         "  FROM mesPackage A WHERE A.SerialNumber='" +
                //                                S_CartonSN + "'" + "\r\n";
                //    }
                //    else if (PrintType == "2")
                //    {

                //        S_Sql += "     UPDATE A SET A.ShipmentParentID='" + S_ShipmentPalletID + "'  FROM mesPackage A WHERE A.SerialNumber='" +
                //                       S_CartonSN + "'" + "\r\n";
                //    }


                //    S_Sql += "      DECLARE @BoxID int " + "\r\n" + //记录Shipment履历
                //               "      SELECT top 1 @BoxID=ID FROM mesPackage WHERE SerialNumber='" + S_CartonSN + "'" + "\r\n" +
                //               "       " + "\r\n" +
                //               "     INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)" + "\r\n" +
                //               "     VALUES(@BoxID, 8, '" + loginList.StationID + "', '" + loginList.EmployeeID + "', GETDATE())" + "\r\n" +

                //               "   End" + "\r\n"
                //               ;

                //    //记录包装数据所有UPC/FG履历信息
                //    sql = $@"SELECT C.Value SerialNumber,B.KitSerialNumber from mesPackage A
                //                INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                //                INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                //                WHERE A.SerialNumber='{S_CartonSN}'";

                //    var fgAndUpc = await DapperConn.QueryAsync<FGAndUPC>(sql);
                //    var fgAndUpcs = fgAndUpc?.ToList();

                //    if (fgAndUpcs is null or { Count: <= 0 })
                //        return "20123";

                //    (string, string) strResult;
                //    foreach (FGAndUPC dr in fgAndUpcs)
                //    {
                //        string FGSN = dr.SerialNumber;
                //        string UPCSN = dr.KitSerialNumber;
                //        strResult = await SetmesHistoryNoUpdatePOAsync(FGSN, loginList, ProductionOrderID);
                //        if (strResult.Item1 != "1")
                //            return strResult.Item1;

                //        S_Sql += strResult.Item2;

                //        if (!string.IsNullOrEmpty(UPCSN) && UPCSN != FGSN)
                //        {
                //            strResult = await SetmesHistoryNoUpdatePOAsync(UPCSN, loginList, ProductionOrderID);
                //            if (strResult.Item1 != "1")
                //                return strResult.Item1;
                //            S_Sql += strResult.Item2;
                //        }
                //    }
                //}

                //if (PrintType == "1")
                //{
                //    {
                //        S_Sql += "if ISNULL(@ShipmentDetailID,0) > 0" + "\r\n" +
                //            "  begin" + "\r\n" +
                //                 "  SELECT @OutSNCount=COUNT(1) FROM mesPackage WHERE ShipmentParentID='" + S_ShipmentPalletID +
                //                 "' AND ShipmentDetailID=@ShipmentDetailID " + "\r\n" +

                //                 "  UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(@OutSNCount,0)" +
                //                 "WHERE FDetailID = @ShipmentDetailID and FStatus in (0, 1)" + "\r\n" +

                //            "  end" + "\r\n";
                //    }

                //    S_Sql +=
                //    "IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew A" + "\r\n" +
                //    "    INNER JOIN CO_WH_ShipmentNew B ON A.FInterID = B.FInterID" + "\r\n" +
                //    "    WHERE B.FBillNO = '" + S_BillNO + "' AND (A.FOutSN <> a.FCTN))" + "\r\n" +
                //    "BEGIN" + "\r\n" +
                //    "    UPDATE CO_WH_ShipmentNew SET FStatus = 2 WHERE FBillNO = '" + S_BillNO + "'" + "\r\n" +
                //    "END \r\n";
                //}
                S_Sql += await uspPalletPackagingSqlAsync(PartID, ProductionOrderID, S_CartonSN, S_PalletSN, loginList, S_BillNO, PrintType, ShipmentDetailID, scanOver, PalletQty);
                S_Sql += @"	
                    COMMIT
                END TRY
	            BEGIN CATCH
                    ROLLBACK
		            SELECT ERROR_MESSAGE()
	            END CATCH";
                var affactCount = await DapperConn.ExecuteAsync(S_Sql, null, null, I_DBTimeout, null);
                return affactCount > 0 ? "1": "20121";
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "", ex);
                return "20121";
            }
        }
        public async Task<string> uspPalletPackagingSqlAsync(string PartID, string ProductionOrderID, string S_CartonSN, string S_PalletSN, LoginList loginList, string S_BillNO, string PrintType, string ShipmentDetailID, bool scanOver = false, int PalletQty = 0)
        {
            try
            {
                ShipMentDetailData shipMentData = new ShipMentDetailData();
                string sql = $@"SELECT top 1 * FROM mesPackage WHERE SerialNumber='{S_PalletSN}' AND StatusID=0";

                var palletEntity = await DapperConn.QueryFirstAsync<mesPackage>(sql, null, null, I_DBTimeout, null);
                if (palletEntity is null or { ID: <= 0 })
                    return "20119";

                sql = $@"select top 1 *  from CO_WH_ShipmentEntryNew where FDetailID={ShipmentDetailID}";

                var shipmentEntryNew = await DapperConn.QueryFirstAsync<CO_WH_ShipmentEntryNew>(sql, null, null, I_DBTimeout, null);
                if (shipmentEntryNew is null or { FInterID: <= 0 })
                    return "20119";

                string S_ShipmentPalletID = palletEntity.ID.ToString();
                string S_Sql = String.Empty;
                S_Sql = "\r\n";
                if (PrintType == "1")
                {
                    S_Sql += "DECLARE @ShipmentDetailID  INT" + "\r\n" +
                      ", @CurrProductionOrderID int" + "\r\n" +
                       ", @OutSNCount int" + "\r\n" +
                        "SELECT @CurrProductionOrderID=CurrProductionOrderID FROM mesPackage WHERE SerialNumber = '" + S_CartonSN + "'" + "\r\n" +
                        "SELECT @ShipmentDetailID = A.FDetailID FROM CO_WH_ShipmentEntryNew A" + "\r\n" +
                            "INNER JOIN CO_WH_ShipmentNew B ON A.FInterID = B.FInterID WHERE B.FBillNO ='" + S_BillNO + "' AND EXISTS(" + "\r\n" +
                             "SELECT * FROM (SELECT A.Content FROM mesProductionOrderDetail A" + "\r\n" +
                             "INNER JOIN luProductionOrderDetailDef B ON A.ProductionOrderDetailDefID= B.ID" + "\r\n" +
                            "WHERE B.Description= 'MPN' AND A.ProductionOrderID= @CurrProductionOrderID) WS WHERE WS.Content = A.FMPNNO AND A.FStatus in (0, 1))" +
                            "\r\n" + "\r\n" +
                        "if ISNULL(@ShipmentDetailID,0) > 0" + "\r\n" +
                        "  begin" + "\r\n" +
                        "       UPDATE A SET A.ShipmentTime=GETDATE()," + "\r\n" +
                        "           ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID='" + ShipmentDetailID + "') " + "\r\n" +
                        "       FROM mesPackage A WHERE SerialNumber ='" + S_PalletSN + "'" + "\r\n" +
                        "  end" + "\r\n";


                    if (PalletQty == 0)
                    {
                        //////////////////////////////////////////////////////////////
                        ///SetShipmentMultipack
                        S_Sql += $@"UPDATE A SET A.ShipmentDetailID ={ShipmentDetailID},A.ShipmentTime=GETDATE(),
				                ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID}) 
			                FROM mesPackage A WHERE SerialNumber = '{S_CartonSN}'
			                UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(FOutSN,0)+1 WHERE FDetailID={ShipmentDetailID} and FStatus in (0,1)
			                IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID} AND FOutSN = FCTN and FStatus in (0,1))
			                BEGIN
				                UPDATE CO_WH_ShipmentEntryNew SET FStatus=2 WHERE FDetailID={ShipmentDetailID}

				                IF NOT EXISTS( SELECT 1 FROM CO_WH_ShipmentEntryNew A
					                INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
					                WHERE B.FBillNO='{S_BillNO}' AND (A.FOutSN<>a.FCTN))
				                BEGIN
					                UPDATE CO_WH_ShipmentNew SET FStatus=2 WHERE FBillNO='{S_BillNO}'
				                END
			                END
                            ";
                        ////////////////////////////////////////////////////////////////////
                    }
                }
                else if (PrintType == "2")
                {
                    if (PalletQty == 0)
                    {
                        S_Sql += $@"UPDATE mesPackage SET ShipmentInterID={shipmentEntryNew.FInterID},ShipmentTime=GETDATE(),ShipmentDetailID={shipmentEntryNew.FDetailID} WHERE SerialNumber='{S_PalletSN}'
                            ";
                    }

                }

                if (PalletQty > 0)
                {
                    S_Sql += "INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time) Values(" + "\r\n" +
                       "'" + S_ShipmentPalletID + "'" + "\r\n" +
                       ",'2'" + "\r\n" +
                       ",'" + loginList.StationID + "'" + "\r\n" +
                       ",'" + loginList.EmployeeID + "'" + "\r\n" +
                       ", GETDATE()" + "\r\n" +
                       ")" + "\r\n" +

                       "UPDATE mesPackage SET CurrentCount='" + PalletQty + "'" + "\r\n" +
                       ",StationID='" + loginList.StationID + "',EmployeeID='" + loginList.EmployeeID + "',StatusID=1" + "\r\n" +
                       ",LastUpdate=GETDATE() where ID='" + S_ShipmentPalletID + "'" + "\r\n";


                    if (PrintType == "1")
                    {

                        /////////////////////////////////////////////////////////////////////////////
                        ///GetIsOutCountComplete
                        S_Sql += $@"DECLARE @tmpFInterId INT, @tmpShipmentParentId INT, @tmpFCTN INT, @tmpOutCount INT, @tmpScanOver BIT ='{scanOver}'
                        SELECT @tmpFInterId = FInterID FROM CO_WH_ShipmentNew WHERE FBillNO = '{S_BillNO}'
                        SELECT @tmpShipmentParentId = Id FROM mesPackage WHERE SerialNumber = '{S_PalletSN}'

                        SELECT @tmpFCTN = ISNULL(FCTN,0)  FROM CO_WH_ShipmentEntryNew  WHERE FInterID = @tmpFInterId AND FDetailID = '{ShipmentDetailID}'
                        SELECT @tmpOutCount = COUNT(1)  FROM mesPackage WHERE ShipmentParentID = @tmpShipmentParentId AND ShipmentDetailID = '{ShipmentDetailID}'

                        IF @tmpFCTN = @tmpOutCount AND @tmpFCTN <> 0
                        BEGIN
                            IF	@tmpScanOver <> 1
	                        BEGIN
	                            RAISERROR(N'Scan count error',16,1)
	                        END
                            UPDATE CO_WH_ShipmentEntryNew SET FStatus=2 WHERE FDetailID='{ShipmentDetailID}'
                            UPDATE mesPackage SET ShipmentInterID={shipmentEntryNew.FInterID},ShipmentTime=GETDATE() WHERE SerialNumber='{S_PalletSN}'
                        END
                        ";
                        /////////////////////////////////////////////////////////////////////////////
                    }
                    else if (PrintType == "2")
                    {
                        S_Sql += $@" UPDATE mesPackage SET ShipmentInterID = {shipmentEntryNew.FInterID}, ShipmentTime = GETDATE(), ShipmentDetailID = {ShipmentDetailID} WHERE SerialNumber = '{S_CartonSN}'
                        ";
                        S_Sql += $@"UPDATE A SET A.ShipmentDetailID ={ShipmentDetailID},A.ShipmentTime=GETDATE(),
				                ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID}) 
			                FROM mesPackage A WHERE SerialNumber = '{S_CartonSN}'
			                UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(FOutSN,0)+1 WHERE FDetailID={ShipmentDetailID} and FStatus in (0,1)
			                IF  EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew WHERE FDetailID={ShipmentDetailID} AND FOutSN = FCTN and FStatus in (0,1))
			                BEGIN
				                UPDATE CO_WH_ShipmentEntryNew SET FStatus=2 WHERE FDetailID={ShipmentDetailID}

				                IF NOT EXISTS( SELECT 1 FROM CO_WH_ShipmentEntryNew A
					                INNER JOIN CO_WH_ShipmentNew B ON A.FInterID=B.FInterID
					                WHERE B.FBillNO='{S_BillNO}' AND (A.FOutSN<>a.FCTN))
				                BEGIN
					                UPDATE CO_WH_ShipmentNew SET FStatus=2 WHERE FBillNO='{S_BillNO}'
				                END
			                END
                            ";
                    }
                }
                else
                {
                    S_Sql += "DECLARE @Stage int " + "\r\n" +
                            " SELECT @Stage=Stage FROM mesPackage WHERE ID='" + S_ShipmentPalletID + "'" + "\r\n" +
                            " if @Stage<>3 " + "\r\n" +   //关联包装信息
                            " Begin " + "\r\n" +
                            "    UPDATE A SET A.ParentID = '" + S_ShipmentPalletID + "'  FROM mesPackage A WHERE A.SerialNumber = '" +
                                    S_CartonSN + "'" + "\r\n" +
                            " End else  Begin" + "\r\n";
                    //关联包装信息
                    if (PrintType == "1")
                    {

                        S_Sql += "     UPDATE A SET A.ShipmentParentID='" + S_ShipmentPalletID + "',A.ShipmentDetailID = '" + ShipmentDetailID + "',A.ShipmentTime=GETDATE()," + "\r\n" +
                                         "  ShipmentInterID=(SELECT TOP 1 FInterID FROM CO_WH_ShipmentEntryNew WHERE FDetailID='" + ShipmentDetailID + "')" + "\r\n" +
                                         "  FROM mesPackage A WHERE A.SerialNumber='" +
                                                S_CartonSN + "'" + "\r\n";
                    }
                    else if (PrintType == "2")
                    {

                        S_Sql += "     UPDATE A SET A.ShipmentParentID='" + S_ShipmentPalletID + "'  FROM mesPackage A WHERE A.SerialNumber='" +
                                       S_CartonSN + "'" + "\r\n";
                    }


                    S_Sql += "      DECLARE @BoxID int " + "\r\n" + //记录Shipment履历
                               "      SELECT top 1 @BoxID=ID FROM mesPackage WHERE SerialNumber='" + S_CartonSN + "'" + "\r\n" +
                               "       " + "\r\n" +
                               "     INSERT INTO mesPackageHistory(PackageID, PackageStatusID, StationID, EmployeeID, Time)" + "\r\n" +
                               "     VALUES(@BoxID, 8, '" + loginList.StationID + "', '" + loginList.EmployeeID + "', GETDATE())" + "\r\n" +

                               "   End" + "\r\n"
                               ;

                    //记录包装数据所有UPC/FG履历信息
                    sql = $@"SELECT C.Value SerialNumber,B.KitSerialNumber from mesPackage A
                                INNER JOIN mesUnitDetail B ON A.ID=B.InmostPackageID
                                INNER JOIN mesSerialNumber C ON B.UnitID=C.UnitID
                                WHERE A.SerialNumber='{S_CartonSN}'
                                ";

                    var fgAndUpc = await DapperConn.QueryAsync<FGAndUPC>(sql);
                    var fgAndUpcs = fgAndUpc?.ToList();

                    if (fgAndUpcs is null or { Count: <= 0 })
                        return "20123";

                    (string, string) strResult;
                    foreach (FGAndUPC dr in fgAndUpcs)
                    {
                        string FGSN = dr.SerialNumber;
                        string UPCSN = dr.KitSerialNumber;
                        strResult = await SetmesHistoryNoUpdatePOAsync(FGSN, loginList, ProductionOrderID);
                        if (strResult.Item1 != "1")
                            return strResult.Item1;

                        S_Sql += strResult.Item2;

                        if (!string.IsNullOrEmpty(UPCSN) && UPCSN != FGSN)
                        {
                            strResult = await SetmesHistoryNoUpdatePOAsync(UPCSN, loginList, ProductionOrderID);
                            if (strResult.Item1 != "1")
                                return strResult.Item1;
                            S_Sql += strResult.Item2;
                        }
                    }
                }

                if (PrintType == "1")
                {
                    {
                        S_Sql += "if ISNULL(@ShipmentDetailID,0) > 0" + "\r\n" +
                            "  begin" + "\r\n" +
                                 "  SELECT @OutSNCount=COUNT(1) FROM mesPackage WHERE ShipmentParentID='" + S_ShipmentPalletID +
                                 "' AND ShipmentDetailID=@ShipmentDetailID " + "\r\n" +

                                 "  UPDATE CO_WH_ShipmentEntryNew SET FOutSN=ISNULL(@OutSNCount,0)" +
                                 "WHERE FDetailID = @ShipmentDetailID and FStatus in (0, 1)" + "\r\n" +

                            "  end" + "\r\n";
                    }

                    S_Sql +=
                    "IF NOT EXISTS(SELECT 1 FROM CO_WH_ShipmentEntryNew A" + "\r\n" +
                    "    INNER JOIN CO_WH_ShipmentNew B ON A.FInterID = B.FInterID" + "\r\n" +
                    "    WHERE B.FBillNO = '" + S_BillNO + "' AND (A.FOutSN <> a.FCTN))" + "\r\n" +
                    "BEGIN" + "\r\n" +
                    "    UPDATE CO_WH_ShipmentNew SET FStatus = 2 WHERE FBillNO = '" + S_BillNO + "'" + "\r\n" +
                    "END \r\n";
                }

                S_Sql += "\r\n";
                return S_Sql;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "", ex);
                return "20121";
            }
        }
        /// <summary>
        /// 一般过站
        /// </summary>
        /// <param name="List_mesUnit"></param>
        /// <param name="List_mesHistory"></param>
        /// <param name="List_mesUnitDefect"></param>
        /// <returns></returns>
        public async Task<string> SubmitData_UnitMod_HistoryAdd_DefectAddAsync(
                List<mesUnit> List_mesUnit,
                List<mesHistory> List_mesHistory,
                List<mesUnitDefect> List_mesUnitDefect
            )
        {
            string S_Result = "";
            string S_Sql = "";

            var List_Tuple = new List<Tuple<string, object>>();
            Tuple<string, object> Tuple_Val = null;
            Tuple<bool, string> Tuple_Result = null;

            try
            {
                S_Sql = $"DECLARE @MachineNo VARCHAR(200) = ''\r\n";
                if (List_mesUnit != null)
                {
                    for (int i = 0; i < List_mesUnit.Count; i++)
                    {
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit = List_mesUnit[i];

                        S_Sql += @"update mesUnit set UnitStateID='" + v_mesUnit.UnitStateID + "'\r\n" +
                                ",StationID='" + v_mesUnit.StationID + "'\r\n" +
                                ",StatusID='" + v_mesUnit.StatusID + "'\r\n" +
                                ",PanelID='" + v_mesUnit.PanelID + "'\r\n" +
                                ",EmployeeID='" + v_mesUnit.EmployeeID + "'\r\n" +
                                ",ProductionOrderID='" + v_mesUnit.ProductionOrderID + "'\r\n" +
                                ",LastUpdate=getdate() \r\n" +
                                " where ID='" + v_mesUnit.ID + "' \r\n";

                        //NG产品解绑关联的治具
                        S_Sql +=
                        @"IF  EXISTS(SELECT 1 FROM mesSerialNumber WHERE UnitId='" + v_mesUnit.ID + @"')
		                BEGIN 
						    
		                    SELECT @MachineNo = F.reserved_01  FROM mesSerialNumber A 
		                        INNER JOIN mesUnitComponent B ON A.UnitID=B.UnitID
		                        INNER JOIN mesPart C ON B.ChildPartID=C.ID
		                        INNER JOIN mesPartDetail D ON C.ID=D.PartID
		                        INNER JOIN luPartDetailDef E ON D.PartDetailDefID=E.ID
		                        INNER JOIN mesUnitDetail F ON B.ChildUnitID=F.UnitID
		                    WHERE E.Description='ScanType' AND D.Content='4' AND A.UnitId='" + v_mesUnit.ID + @"'

							IF ISNULL(@MachineNo, '') <> ''
							BEGIN
							    UPDATE M SET M.RuningCapacityQuantity=0,StatusID=1 FROM mesMachine M
								WHERE  M.SN=@MachineNo

								UPDATE S SET reserved_03 = '2' FROM mesUnitDetail S WHERE reserved_03 = '1' AND reserved_01 = @MachineNo 
							END
                        END" + "\r\n";
                    }
                }


                if (List_mesHistory != null)
                {
                    for (int i = 0; i < List_mesHistory.Count; i++)
                    {
                        mesHistory v_mesHistory = new mesHistory();
                        v_mesHistory = List_mesHistory[i];

                        S_Sql += @" insert into mesHistory(UnitID, UnitStateID, EmployeeID, StationID, " + "\r\n" +
                            "EnterTime, ExitTime, ProductionOrderID, PartID, LooperCount, StatusID) Values( " + "\r\n" +

                            "'" + v_mesHistory.UnitID + "'," + "\r\n" +
                            "'" + v_mesHistory.UnitStateID + "'," + "\r\n" +
                            "'" + v_mesHistory.EmployeeID + "'," + "\r\n" +
                            "'" + v_mesHistory.StationID + "'," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "getdate()," + "\r\n" +
                            "'" + v_mesHistory.ProductionOrderID + "'," + "\r\n" +
                            "'" + v_mesHistory.PartID + "'," + "\r\n" +
                            "'1'," + "\r\n" +
                            "'" + v_mesHistory.StatusID + "'" + "\r\n" +
                            ")" + "\r\n";

                    }
                }

                if (List_mesUnitDefect != null)
                {
                    if (List_mesUnitDefect.Count > 0)
                    {
                        S_Sql += "declare @MaxDefID int" + "\r\n";
                    }
                    int I_Qyt = 0;


                    for (int i = 0; i < List_mesUnitDefect.Count; i++)
                    {
                        mesUnitDefect v_mesUnitDefect = new mesUnitDefect();
                        v_mesUnitDefect = List_mesUnitDefect[i];
                        I_Qyt = i + 1;

                        S_Sql +=
                                @"select @MaxDefID=ISNULL(Max(ID),0)+" + I_Qyt + " from mesUnitDefect " + "\r\n" +
                                "INSERT INTO mesUnitDefect(ID, UnitID, DefectID, StationID, EmployeeID) Values(" + "\r\n" +
                                "@MaxDefID," + "\r\n" +
                                "'" + v_mesUnitDefect.UnitID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.DefectID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.StationID + "'," + "\r\n" +
                                "'" + v_mesUnitDefect.EmployeeID + "'" + "\r\n" +
                                ")\r\n";
                    }
                }


                if (dbConnectionOptions.DatabaseType == DatabaseType.Oracle)
                {

                }

                Tuple_Val = new Tuple<string, object>(S_Sql, null);
                List_Tuple.Add(Tuple_Val);
                Tuple_Result = await ExecuteTransactionAsync(List_Tuple, I_DBTimeout);

                if (Tuple_Result.Item1 == false)
                {
                    S_Result = "ERROR:" + Tuple_Result.Item2;
                    return S_Result;
                }

                S_Result = "OK";
            }

            catch (Exception ex)
            {
                S_Result = "ERROR:" + ex.ToString();
            }
            return S_Result;
        }
        #endregion
    }
}