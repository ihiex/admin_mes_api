using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.Dtos.MES.MES_Output.KitQC;
using SunnyMES.Security.IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories.MES
{
    public class KitQCRepository : MesBaseRepository, IKitQCRepository
    {
        public KitQCRepository(IDbContextCore contextCore) : base(contextCore)
        {

        }

        public async Task<MesMainOutputDtos> MainSnVerifyAsync(MesSnInputDto input)
        {
            MesMainOutputDtos output = new MesMainOutputDtos();
            var setPo = await base.SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL);

            if (input == null || string.IsNullOrEmpty(input.S_SN))
                return output.SetErrorCode(msgSys.MSG_Sys_20007);

            string SN_Pattern = string.Empty;
            List<mesPartDetail> listPartDetail = await Public_Repository.GetmesPartDetail(Convert.ToInt32(input.S_PartID), "SN_Pattern");
            if (listPartDetail != null && listPartDetail.Count > 0)
            {
                SN_Pattern = listPartDetail[0].Content.ToString().Trim();
            }
            if (string.IsNullOrEmpty(SN_Pattern))
                return output.SetErrorCode(msgSys.MSG_Sys_20025);

            if (!Regex.IsMatch(input.S_SN, SN_Pattern))
                return output.SetErrorCode(msgSys.MSG_Sys_20027);

            if (input.S_UnitStatus == "2" || input.S_UnitStatus == "3" )
            {
                if (string.IsNullOrEmpty(input.S_DefectID))
                    return output.SetErrorCode(msgSys.MSG_Sys_20049);
            }

            string nextUnitState = await Public_Repository.GetMesUnitState(input.S_PartID, input.S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0,     input.S_POID, input.S_UnitStatus);

            ///////////////////////////////////////////////////////////////////
            //二次检查是否有指定线路  20231214
            var specialUnitState = await Public_Repository.GetmesUnitStateSecondAsync(input.S_PartID, input.S_PartFamilyID,"", List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0, input.S_POID, input.S_UnitStatus, input.S_SN);

            if (!string.IsNullOrEmpty(specialUnitState.errorCode))
            {
                return output.SetErrorCode(specialUnitState.errorCode);
            }
            nextUnitState = specialUnitState.unitStateId;
            ////////////////////////////////////////////////////////////////////


            if (string.IsNullOrEmpty(nextUnitState))
                return output.SetErrorCode(msgSys.MSG_Sys_20203);


            var tmpSnPartFamily = await Public_Repository.GetAndCheckUnitInfoAsync(input.S_SN, input.S_POID, input.S_PartID);
            if (tmpSnPartFamily == null)
                return output.SetErrorCode(msgSys.MSG_Sys_20015);

            int I_FirstScanSequence = FirstScanSequence(input.S_PartID, List_Login.StationTypeID ?? 0);

            if (tmpSnPartFamily.SerialNumberTypeID != 6)
                return output.SetErrorCode(msgSys.MSG_Sys_20035);

            string tmpFGSN = await Public_Repository.MesGetFGSNByUPCSNAsync(tmpSnPartFamily.Value);
            var fgUnitEntity = await Public_Repository.GetMesUnitSerialAsync(tmpFGSN);

            var resRouteCheck = await Public_Repository.GetRouteCheckAsync(List_Login.StationTypeID.ToInt(), List_Login.StationID,
                List_Login.LineID.ToString(), fgUnitEntity, tmpFGSN);

            if (resRouteCheck != "1")
                return output.SetErrorCode(msgSys.GetLanguage(resRouteCheck));

            
            //var dtProductStructure = Public_Repository.GetmesProductStructure2(input.S_PartID, List_Login.StationTypeID.ToString());

            //if (dtProductStructure?.Count > 0)
            //{
            //    output.IsShowChildInput = true;
            //    return output;
            //}

            var qcCheckOutput = await Public_Repository.uspQCCheckAsync(input.S_SN, input.S_POID, input.S_PartID, List_Login);
            if (qcCheckOutput != "1")
                return output.SetErrorCode(msgSys.GetLanguage(qcCheckOutput));

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();
            List<mesUnitDefect> List_mesUnitDefect = new List<mesUnitDefect>();

            mesUnit v_mesUnit = new mesUnit();
            v_mesUnit.ID = tmpSnPartFamily.ID;
            v_mesUnit.UnitStateID = Convert.ToInt32(nextUnitState);
            v_mesUnit.StatusID = Convert.ToInt32(input.S_UnitStatus);
            v_mesUnit.StationID = List_Login.StationID;
            v_mesUnit.EmployeeID = List_Login.EmployeeID;
            v_mesUnit.LastUpdate = DateTime.Now;
            v_mesUnit.ProductionOrderID = Convert.ToInt32(input.S_POID);

            List_mesUnit.Add(v_mesUnit);

            mesHistory v_mesHistory = new mesHistory();

            v_mesHistory.UnitID = tmpSnPartFamily.ID;
            v_mesHistory.UnitStateID = Convert.ToInt32(nextUnitState);
            v_mesHistory.EmployeeID = List_Login.EmployeeID;
            v_mesHistory.StationID = List_Login.StationID;
            v_mesHistory.ProductionOrderID = Convert.ToInt32(input.S_POID);
            v_mesHistory.PartID = Convert.ToInt32(input.S_PartID);
            v_mesHistory.LooperCount = 1;
            v_mesHistory.StatusID = Convert.ToInt32(input.S_UnitStatus);

            List_mesHistory.Add(v_mesHistory);


            int tmpFGUnitId = 0;
            if (tmpFGSN != tmpSnPartFamily.Value)
            {
                mesUnit v_FGunit = await Public_Repository.GetmesUnitSNAsync(tmpFGSN);
                tmpFGUnitId = v_FGunit.ID;
                var tmpFGUnit = OutputExtensions.DeepClone<mesUnit>(v_mesUnit);
                tmpFGUnit.ID = v_FGunit.ID;
                List_mesUnit.Add(tmpFGUnit);

                var tmpFGHistory = OutputExtensions.DeepCopyByReflection(v_mesHistory);
                tmpFGHistory.UnitID = v_FGunit.ID;
                List_mesHistory.Add(tmpFGHistory);
            }

            string[] Array_Defect = input.S_DefectID.Split(',');
            if (input.S_UnitStatus != "1")
            {
                foreach (var item in Array_Defect)
                {
                    if (item.Trim() != "")
                    {
                        int I_DefectID = Convert.ToInt32(item.Trim());
                        mesUnitDefect v_mesUnitDefect = new mesUnitDefect();

                        v_mesUnitDefect.UnitID = v_mesUnit.ID;
                        v_mesUnitDefect.DefectID = I_DefectID;
                        v_mesUnitDefect.StationID = List_Login.StationID;
                        v_mesUnitDefect.EmployeeID = List_Login.EmployeeID;

                        List_mesUnitDefect.Add(v_mesUnitDefect);
                        if (tmpFGSN != input.S_SN)
                        {
                            var FgDefect = OutputExtensions.DeepCopyByReflection(v_mesUnitDefect);
                            FgDefect.UnitID = tmpFGUnitId;
                            List_mesUnitDefect.Add(FgDefect);
                        }
                    }
                }
                //非Pass产品解绑关联的治具
                string result = string.Empty;
                string xmlStationStr = "<Station StationID=\"" + List_Login.StationID.ToString() + "\"> </Station>";
                string xmlPartStr = "<Part PartID=\"" + input.S_PartID+ "\"> </Part>";
                //PartSelectSVC.uspCallProcedure("uspQcReleaseMachine", Edt_SN.Text.ToString(),
                //                                        null, xmlPartStr, xmlStationStr, null, null, ref result);

                await Public_Repository.uspQcReleaseMachineAsync(input.S_SN, xmlPartStr, xmlStationStr);
            }

            var hisRcord = await Public_Repository.CheckHistoryWithSNAsync(tmpFGSN, List_Login.StationID.ToString());

            var returnVal = await DataCommit_Repository.SubmitData_UnitMod_HistoryAdd_DefectAddAsync(List_mesUnit, List_mesHistory, List_mesUnitDefect);
            if (returnVal != "OK")
                return output.SetErrorCode(msgSys.GetLanguage(returnVal));

            if (hisRcord != "1")
            {
                var CountReturn = await Public_Repository.uspSetProductionOrderCountAsync(input.S_POID, List_Login.StationID.ToString());
                if (CountReturn != "1")
                {
                    Log4NetHelper.Error($"update production order count failed. msg : {CountReturn} ");
                }
            }
            output.ProductionOrderQTY = await Public_Repository.GetProductionOrderQTY(List_Login.StationID, Convert.ToInt32(input.S_POID));
            return output;
        }
        /// <summary>
        /// 第一站 扫描顺序号
        /// </summary>
        /// <param name="PartID"></param>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        private int FirstScanSequence(string PartID, int StationTypeID)
        {
            int I_Result = 1;
            string PartFamilyID = mSetConfirmPoOutput?.CurrentSettingInfo?.PartFamilyID.ToString();
            if (IsOneStationPrint(PartID, PartFamilyID, StationTypeID, List_Login.LineID.ToString(), mSetConfirmPoOutput?.CurrentSettingInfo?.POID.ToString()))
            {
                I_Result = 2;
            }
            return I_Result;
        }

        /// <summary>
        /// 第一站是否  打印
        /// </summary>
        /// <param name="PartID"></param>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        public Boolean IsOneStationPrint(string PartID, string PartFamilyID, int StationTypeID, string LineID, string ProductionOrderID)
        {
            Boolean B_Result = false;

            //int RouteID = Public_Repository.GetRouteID(PartID, PartFamilyID, LineID, ProductionOrderID).Result;
            int RouteID = Public_Repository.ufnRTEGetRouteIDAsync(LineID.ToInt(), PartID.ToInt(), PartFamilyID.ToInt(), ProductionOrderID.ToInt()).Result.ToInt();
            var DT_Route = Public_Repository.GetRoute("", RouteID).Result;

            if (DT_Route is not null && DT_Route.Rows.Count > 0)
            {
                //第一站是打印的情况 确定以后
                var v_RouteSequence = from c in DT_Route.AsEnumerable()
                                      where c.Field<int>("Sequence") == 1 && c.Field<string>("ApplicationType") == "Print"
                                      select c;

                if (v_RouteSequence.Any())
                {
                    B_Result = true;
                }
            }
            return B_Result;
        }
    }
}
