using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Localization;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using StackExchange.Profiling.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.AssemblyDynamic;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Repositories;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._1_Models.MES.Query.ShipMent;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyDynamic;
using SunnyMES.Security._2_Dtos.MES.MES_Output.AssemblyTwoInput;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.Repositories;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security._4_Repositories.MES
{
    public class AssemblyDynamicRepository : MesBaseRepository, IAssemblyDynamicRepository
    {
        public AssemblyDynamicRepository(IDbContextCore dbContext) : base(dbContext)
        {

        }

        private string SN_Pattern = string.Empty;
        string ColorCode = string.Empty;
        string SpcaCode = string.Empty;
        string PpCode = string.Empty;
        string BuildCode = string.Empty;

        bool isPerformance = false;


        public async Task<AssemblyDynamicOutputDto> DynamicSnVerifyAsync(AssemblyDynamicInput input)
        {
            var output = new AssemblyDynamicOutputDto();
            var confimPo = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(confimPo.ErrorMsg?.ToString() ?? ""))
                return output.SetErrorCode((string)confimPo.ErrorMsg);

            output.CurrentInitPageInfo = confimPo.CurrentInitPageInfo;
            output.CurrentSettingInfo = confimPo.CurrentSettingInfo;
            output.DataList = null;


            var mDataList = input.DataList;
            if (mDataList is null || mDataList.Count < 2)
                return output.SetErrorCode(P_MSG_Public.MSG_Public_6003);

            if (mDataList.Count(x => x.IsCurrentItem) != 1)
                return output.SetErrorCode(P_MSG_Public.MSG_Public_6014);

            BomLinkedInfo currentBomInfo = null;
            
            currentBomInfo = mDataList.Where(x => x.IsCurrentItem).FirstOrDefault();
            if (currentBomInfo is null or { Barcode: "" } or { IsScanFinished: true })
                return output.SetErrorCode(msgSys.MSG_Sys_20007);

            //应前端要求，在单步提交而非最后提交，当状态为NG时， DataList为空
            int itemIndex = mDataList.IndexOf(currentBomInfo);
            if (currentBomInfo.IsMainSn)
            {
                var verRes = await VerifyMianCodeAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL, currentBomInfo.Barcode, currentBomInfo);
                output.ErrorMsg = verRes.Item1 ? "" : verRes.Item2;
                mDataList[itemIndex] = verRes.Item3;
                if (!string.IsNullOrEmpty(output.ErrorMsg))
                    return output;
            }
            else
            {
                //子码校验
                if (currentBomInfo == null || string.IsNullOrEmpty(currentBomInfo.Barcode))
                    return output.SetErrorCode(msgSys.MSG_Sys_20007);

                if (currentBomInfo.IsScanFinished)
                    return output.SetErrorCode(P_MSG_Public.MSG_Public_6030);

                BomLinkedInfo tmpMainInfo = mDataList.Where(x => x.IsMainSn && !string.IsNullOrEmpty(x.Barcode) && x.IsScanFinished).FirstOrDefault();

                string tmpMainSn = tmpMainInfo is null ? "": tmpMainInfo.Barcode;
                //检查供应商代码
                if (output.CurrentInitPageInfo.stationAttribute.IsCheckVendor == "1")
                {
                    //老板要求由写死改成存储过程，以往任何项目未使用过
                    if (await Public_Repository.uspCheckVendorAsync(tmpMainSn, currentBomInfo.Barcode, input.S_POID, input.S_PartID, List_Login))
                    {
                        output.ErrorMsg = P_MSG_Public.MSG_Public_6009;
                        return output;
                    }
                }

                //检查当前项目
                var tmpCurrentOut = await checkSingleItem(currentBomInfo, tmpMainSn, input.S_POID,input.S_PartID);
                tmpCurrentOut.currentItem.IsClearInput = tmpCurrentOut.errorCode != "1";
                if (tmpCurrentOut.errorCode != "1")
                {
                    return output.SetErrorCode(msgSys.GetLanguage( tmpCurrentOut.errorCode));
                }
                mDataList[itemIndex] = tmpCurrentOut.currentItem;
            }
            mDataList[itemIndex].IsScanFinished = string.IsNullOrEmpty(output.ErrorMsg);            
            if (mDataList.Count( x => !string.IsNullOrEmpty(x.Barcode) && x.IsScanFinished) == mDataList.Count())
            {
                output.DataList = mDataList;
                BomLinkedInfo mainBomInfo = null;
                //最后一个条码, 所有条码再次进行检查
                for (int i = 0; i < mDataList.Count; i++)
                {
                    var tmpItem = mDataList[i];
                    if (tmpItem.IsMainSn)
                    {
                        mainBomInfo = tmpItem;
                        var verRes = await VerifyMianCodeAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL, mainBomInfo.Barcode, tmpItem);
                        output.DataList[i] = tmpItem;
                        if (!verRes.Item1)
                            return output.SetErrorCode(verRes.Item2);
                        continue;
                    }

                    var tmpOut = await checkSingleItem(tmpItem, mainBomInfo.Barcode, input.S_POID, input.S_PartID);
                    output.DataList[i] = tmpOut.currentItem;
                    if (tmpOut.errorCode != "1")
                        return output.SetErrorCode(tmpOut.errorCode);
                }

                //提交数据
                var subRes = await SubmitDataAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL, mainBomInfo.Barcode, currentBomInfo.Barcode, mDataList);
                if (subRes.Item1 != "OK")
                    return output.SetErrorCode(subRes.Item2);
            }            
            output.DataList = mDataList;
            return output;
        }

        /// <summary>
        /// 针对子码校验
        /// </summary>
        /// <param name="tmpItem"></param>
        /// <param name="mainSn"></param>
        /// <param name="POID"></param>
        /// <returns></returns>
        private async Task<(string errorCode, BomLinkedInfo currentItem)> checkSingleItem(BomLinkedInfo tmpItem, string mainSn, string POID, string MainPartID)
        {
            bool regexRes = true;

            if (string.IsNullOrEmpty(tmpItem.Barcode))
            {
                if (tmpItem.IsScanFinished)
                    return (P_MSG_Public.MSG_Public_6014, tmpItem);
                else
                    return (msgSys.MSG_Sys_20007, tmpItem);
            }


            if (tmpItem.ScanType == 1)
            {
                //类型1,为系统注册SN，未查询到则忽略
                var tmpCI = await Public_Repository.GetmesUnitSerialNumber(tmpItem.Barcode);
                if (tmpCI.Count <= 0)
                    return (msgSys.MSG_Sys_20012, tmpItem);
            }

            #region 针对子料进行检查
            ScanType scan = (ScanType)tmpItem.ScanType;
            switch (scan)
            {
                //20221123  针对3,4,6扫描类型，再次进行判断实际绑定的料号是否匹配
                case ScanType.设备数据_绑定过批次_类似托盘3:
                case ScanType.设备数据_无批次_类似夹具4:
                case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                    int tmpPartId7 = await Public_Repository.GetPartIdByMachineSNAsync(tmpItem.Barcode);
                    if (tmpPartId7 != tmpItem.PartID)
                        return (P_MSG_Public.MSG_Public_6020, tmpItem);
                    break;
                //再次判断绑定的料号是否匹配
                case ScanType.系统注册批次_Batch校验数量7:
                    mesMaterialUnit tmpMesMaterialUnit = await Public_Repository.GetMaterialUnitBySnAsync(tmpItem.Barcode);
                    if (tmpMesMaterialUnit?.PartID != tmpItem.PartID)
                        return (P_MSG_Public.MSG_Public_6020, tmpItem);
                    break;
                default:
                    if (string.IsNullOrEmpty(tmpItem.Pattern))
                        return (msgSys.MSG_Sys_20025, tmpItem);
                    regexRes = Regex.IsMatch(tmpItem.Barcode, tmpItem.Pattern.Replace("\\\\", "\\"));
                    break;
            }

            
            if (!regexRes)
                return (P_MSG_Public.MSG_Public_029,tmpItem);
            #endregion
            var checkRes = await CheckInputBarcodeAsync(0, mainSn, tmpItem.Barcode, tmpItem.Pattern, tmpItem.ScanType, tmpItem.PartID.ToString(), POID,MainPartID);
            tmpItem.TrayBatchSN = checkRes.batchSN;
            tmpItem.IsClearInput = checkRes.errorCode != "1";
            return (checkRes.errorCode == "1" ? "1" : msgSys.GetLanguage(checkRes.errorCode), tmpItem);
        }

        private async Task<(string, string)> SubmitDataAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, string ChildSN, List<BomLinkedInfo> bomPartInfoList)
        {

            List<mesUnit> List_mesUnit = new List<mesUnit>();
            List<mesHistory> List_mesHistory = new List<mesHistory>();
            List<mesUnitComponent> List_mesUnitComponent = new List<mesUnitComponent>();

            List<mesMaterialConsumeInfo> List_mesMaterialConsumeInfo = new List<mesMaterialConsumeInfo>();
            List<mesMachine> List_mesMachine = new List<mesMachine>();
            string res = "0";
            //再次验证主条码
            //var vmcRes = await VerifyMianCodeAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL, MainSN, bomPartInfoList, ChildSN);
            //if (!vmcRes.Item1)
            //    return (res, vmcRes.Item2);

            string nextUnitState = await Public_Repository.GetMesUnitState(S_PartID, S_PartFamilyID, List_Login.LineID.ToString(), List_Login.StationTypeID ?? 0,
                S_POID, S_UnitStatus);

            if (string.IsNullOrEmpty(nextUnitState))
                return (res, msgSys.GetLanguage("20203"));

            List<mesPartDetail> listPartDetail = await Public_Repository.GetmesPartDetail(Convert.ToInt32(S_PartID), "SN_Pattern");
            if (listPartDetail != null && listPartDetail.Count > 0)
            {
                SN_Pattern = listPartDetail[0].Content.ToString().Trim();
            }

            string msg = string.Empty;
            int MainUnitID = 0;

            try
            {
                for (int i = 0; i < bomPartInfoList.Count; i++)
                {
                    var tmpBomPartEntity = bomPartInfoList[i];
                    int tmpScanType = tmpBomPartEntity.ScanType;

                    string Batch = "";
                    if (tmpBomPartEntity.ScanType == 6)
                    {
                        var tmpMac = await Public_Repository.MesGetBatchIDByBarcodeSNAsync(tmpBomPartEntity.Barcode);
                        tmpScanType = (tmpMac == null || tmpMac.Count <= 0) ? 4 : 3;
                    }
                    ScanType st = (ScanType)tmpScanType;
                    if (i == 0)
                    {
                        //获取main sn unit id
                        if (st == ScanType.设备数据_无批次_类似夹具4)
                        {
                            var mVirtualBarCode4 = await VirtualBarCodeAsync(tmpBomPartEntity.Barcode, S_PartID, S_PartFamilyID,
                                S_PartFamilyTypeID, S_UnitStatus, S_POID, tmpBomPartEntity.Pattern);

                            if (string.IsNullOrEmpty(mVirtualBarCode4.Item1))
                            {
                                return (res, mVirtualBarCode4.Item2);
                            }

                            MainUnitID = int.Parse(mVirtualBarCode4.Item1);

                        }
                        else
                        {
                            if (tmpScanType == 3)
                            {
                                var tmpMachine3 = await Public_Repository.BoxSnToBatch(tmpBomPartEntity.Barcode);
                                if (tmpMachine3.Item1 == "")
                                {
                                    return (res, msgSys.GetLanguage("20030"));
                                }

                                Batch = tmpMachine3.Item1;
                                MainUnitID = tmpMachine3.Item2.UnitID;
                            }
                            else
                            {
                                var tUnitId = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                                if (tUnitId?.Count <= 0)
                                {
                                    return (res, P_MSG_Public.MSG_Public_6012);
                                }
                                MainUnitID = tUnitId[0].UnitID;
                            }
                            mesUnit F_mesUnit = new mesUnit();
                            F_mesUnit.ID = MainUnitID;
                            F_mesUnit.UnitStateID = Convert.ToInt32(nextUnitState);
                            F_mesUnit.StatusID = 1;
                            F_mesUnit.StationID = List_Login.StationID;
                            F_mesUnit.EmployeeID = List_Login.EmployeeID;
                            F_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                            List_mesUnit.Add(F_mesUnit);
                        }

                        var mesUnit_Part = await Public_Repository.GetmesUnit(MainUnitID.ToString());
                        string S_POPartID = mesUnit_Part[0].PartID.ToString();
                        //string S_POPartID = DT_ProductionOrder.Rows[0]["PartID"].ToString();

                        mesHistory F_mesHistory = new mesHistory();
                        F_mesHistory.UnitID = MainUnitID;
                        F_mesHistory.UnitStateID = Convert.ToInt32(nextUnitState);
                        F_mesHistory.EmployeeID = List_Login.EmployeeID;
                        F_mesHistory.StationID = List_Login.StationID;
                        F_mesHistory.ProductionOrderID = Convert.ToInt32(S_POID);
                        F_mesHistory.PartID = Convert.ToInt32(S_POPartID);
                        F_mesHistory.LooperCount = 1;
                        F_mesHistory.StatusID = 1;
                        F_mesHistory.EnterTime = DateTime.Now;

                        List_mesHistory.Add(F_mesHistory);
                        msg = msg = "Mian Code:" + tmpBomPartEntity.Barcode;
                    }
                    else
                    {
                        mesUnitComponent v_mesUnitComponent = new mesUnitComponent();
                        switch (st)
                        {
                            case ScanType.非系统注册批次_Batch校验数量_不校验唯一性2:
                            case ScanType.非系统注册SN_校验唯一性5:
                            case ScanType.系统注册批次_Batch校验数量7:
                            case ScanType.注册公用批次_扫描子批次校验8:
                                v_mesUnitComponent.UnitID = MainUnitID;
                                v_mesUnitComponent.UnitComponentTypeID = 1;
                                v_mesUnitComponent.ChildUnitID = 0;
                                if (st == ScanType.系统注册批次_Batch校验数量7  || st == ScanType.注册公用批次_扫描子批次校验8)
                                {
                                    var tChildUnitId = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                                    if (tChildUnitId?.Count <= 0)
                                    {
                                        return (res, P_MSG_Public.MSG_Public_6011);
                                    }

                                    v_mesUnitComponent.ChildUnitID = tChildUnitId[0].UnitID;
                                }

                                if (st == ScanType.非系统注册批次_Batch校验数量_不校验唯一性2)
                                {
                                    v_mesUnitComponent.ChildSerialNumber = "";
                                    v_mesUnitComponent.ChildLotNumber = tmpBomPartEntity.Barcode;
                                }
                                else
                                {
                                    v_mesUnitComponent.ChildSerialNumber = tmpBomPartEntity.Barcode;
                                    v_mesUnitComponent.ChildLotNumber = "";
                                }

                                var tPartFamilys = await Public_Repository.MesGetPart(tmpBomPartEntity.PartID.ToString(), "");
                                if (tPartFamilys?.Count() <= 0)
                                {
                                    return (res, P_MSG_Public.MSG_Public_6013);
                                }
                                v_mesUnitComponent.ChildPartID = tmpBomPartEntity.PartID;
                                v_mesUnitComponent.ChildPartFamilyID = tPartFamilys.ToList()[0].PartFamilyID;
                                v_mesUnitComponent.Position = "";
                                v_mesUnitComponent.InsertedEmployeeID = List_Login.EmployeeID;
                                v_mesUnitComponent.InsertedStationID = List_Login.StationID;
                                v_mesUnitComponent.StatusID = 1;
                                break;
                            default:
                                int ChildUnitID;
                                if (st == ScanType.设备数据_绑定过批次_类似托盘3)
                                {
                                    var tmpcMachine3 = await Public_Repository.BoxSnToBatch(tmpBomPartEntity.Barcode);
                                    if (tmpcMachine3.Item1 == "")
                                    {
                                        return (res, msgSys.GetLanguage("20030"));
                                    }

                                    Batch = tmpcMachine3.Item1;
                                    ChildUnitID = tmpcMachine3.Item2.UnitID;
                                }
                                else if (st == ScanType.设备数据_无批次_类似夹具4)
                                {
                                    var tChildPos = await Public_Repository.GetPO(tmpBomPartEntity.PartID.ToString(), "");
                                    string S_TollPOID = tChildPos?.Count <= 0 ? "0" : tChildPos.ConvertDynamic("ID");
                                    var cVirtualBarCode4 = await VirtualBarCodeAsync(tmpBomPartEntity.Barcode, tmpBomPartEntity.PartID.ToString(), "",
                                        "", nextUnitState, S_TollPOID, tmpBomPartEntity.Pattern);

                                    if (string.IsNullOrEmpty(cVirtualBarCode4.Item1))
                                    {
                                        return (res, cVirtualBarCode4.Item2);
                                    }

                                    ChildUnitID = int.Parse(cVirtualBarCode4.Item1);
                                }
                                else
                                {
                                    var ctUnitid = await Public_Repository.GetmesUnitSerialNumber(tmpBomPartEntity.Barcode);
                                    if (ctUnitid?.Count <= 0)
                                    {
                                        return (res, P_MSG_Public.MSG_Public_6012);
                                    }

                                    ChildUnitID = ctUnitid[0].UnitID;
                                }

                                var tComponents = await Public_Repository.GetComponent(ChildUnitID);
                                if (tComponents == null || tComponents.Count <= 0)
                                {
                                    return (res, msgSys.GetLanguage("20032"));
                                }
                                string S_ChildSerialNumber = tComponents.ConvertDynamic("Value").ToString();
                                int ChildPartFamilyID = Convert.ToInt32(tComponents.ConvertDynamic("PartFamilyID").ToString());

                                v_mesUnitComponent.UnitID = MainUnitID;
                                v_mesUnitComponent.UnitComponentTypeID = 1;
                                v_mesUnitComponent.ChildUnitID = Convert.ToInt32(ChildUnitID);
                                v_mesUnitComponent.ChildSerialNumber = S_ChildSerialNumber;
                                v_mesUnitComponent.ChildLotNumber = Batch;
                                v_mesUnitComponent.ChildPartID = Convert.ToInt32(tmpBomPartEntity.PartID);
                                v_mesUnitComponent.ChildPartFamilyID = ChildPartFamilyID;
                                v_mesUnitComponent.Position = "";
                                v_mesUnitComponent.InsertedEmployeeID = List_Login.EmployeeID;
                                v_mesUnitComponent.InsertedStationID = List_Login.StationID;
                                v_mesUnitComponent.StatusID = 1;
                                break;
                        }

                        List_mesUnitComponent.Add(v_mesUnitComponent);

                        msg = msg + " Child barcode:" + tmpBomPartEntity.Barcode;
                    }

                    if (st == ScanType.设备数据_绑定过批次_类似托盘3 ||
                        st == ScanType.设备数据_无批次_类似夹具4 ||
                        st == ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6)
                    {
                        mesMachine v_mesMachine = new mesMachine();
                        v_mesMachine.SN = tmpBomPartEntity.Barcode;
                        List_mesMachine.Add(v_mesMachine);
                    }

                    if (st == ScanType.非系统注册批次_Batch校验数量_不校验唯一性2 ||
                        st == ScanType.设备数据_绑定过批次_类似托盘3 ||
                        st == ScanType.系统注册批次_Batch校验数量7 ||
                        st == ScanType.注册公用批次_扫描子批次校验8)
                    {
                        string SN = string.Empty;
                        string MachineSN = string.Empty;
                        int BatchType = 1;
                        if (st == ScanType.设备数据_绑定过批次_类似托盘3)
                        {
                            MachineSN = tmpBomPartEntity.Barcode;
                            BatchType = 2;
                        }
                        else
                        {
                            SN = tmpBomPartEntity.Barcode;
                        }

                        mesMaterialConsumeInfo v_mesMaterialConsumeInfo = new mesMaterialConsumeInfo();
                        v_mesMaterialConsumeInfo.ScanType = BatchType;
                        v_mesMaterialConsumeInfo.SN = SN;
                        v_mesMaterialConsumeInfo.MachineSN = MachineSN;
                        v_mesMaterialConsumeInfo.PartID = Convert.ToInt32(S_PartID);
                        v_mesMaterialConsumeInfo.ProductionOrderID = Convert.ToInt32(S_POID);

                        List_mesMaterialConsumeInfo.Add(v_mesMaterialConsumeInfo);

                    }
                }


                /////////////////////////////////////////////////////////
                //增加组装为最后一站时，且做完此需去做TT，最后又需要回到主线时，取消条码重复绑定。20230925
                List<mesRoute> List_Route = await Public_Repository.GetmesRoute(Convert.ToInt32(List_Login.LineID), Convert.ToInt32(S_PartID),
                Convert.ToInt32(S_PartFamilyID), Convert.ToInt32(S_POID));
                List_Route = List_Route ?? new List<mesRoute>();

                if (List_Route.Count <= 0)
                {
                    return ("Error",P_MSG_Public.MSG_Public_001); //料号未配置工艺流程路线.
                }

                int I_RouteID = List_Route[0].ID;
                int I_RouteType = List_Route[0].RouteType;  //GetRouteType(I_RouteID);

                bool isLastStation = false;
                if (I_RouteType == 1)
                {
                    isLastStation = (await Public_Repository.IsDiagramSFCLastStation(List_Login.StationTypeID.ToString(), I_RouteID.ToString())) == "1";
                }
                else
                {
                    isLastStation =
                        (await Public_Repository.IsTableFCLastStation(List_Login.StationTypeID.ToString(), I_RouteID.ToString(), nextUnitState)) == "1";
                }

                if (isLastStation)
                {
                    List<mesUnitComponent> mucs = new List<mesUnitComponent>();
                    foreach (mesUnitComponent unitComponent in List_mesUnitComponent)
                    {
                        bool tmpExistComponent = (await Public_Repository.IsExistBindChildPart(unitComponent.UnitID,
                            unitComponent.ChildUnitID, List_Login.StationTypeID.ToString(), unitComponent.ChildLotNumber, unitComponent.ChildPartID.ToString())) == "1";
                        if (tmpExistComponent)
                            continue;

                        unitComponent.StatusID = tmpExistComponent ? 0 : 1;
                        mucs.Add(unitComponent);
                    }
                    List_mesUnitComponent.Clear();
                    List_mesUnitComponent = mucs;
                }
                ////////////////////////////////////////////////////////
                res = await DataCommit_Repository.SubmitDataUHCAsync(List_mesUnit, List_mesHistory, List_mesUnitComponent, List_mesMaterialConsumeInfo,
                   List_mesMachine, List_Login);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
            }
            if (res != "OK")
            {
                return (res, "update database failed.");
            }

            return (res, "update database success.");
        }
        /// <summary>
        ///虚拟条码检查，没有则生成
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_UnitStateID"></param>
        /// <param name="S_POID"></param>
        /// <returns></returns>
        private async Task<(string unitID, string errorCode)> VirtualBarCodeAsync(string S_SN, string S_PartID, string S_PartFamilyID, string S_PartFamilyTypeID, string S_UnitStateID, string S_POID, string SnPattern)
        {
            try
            {
                string S_InsertUnit = string.Empty;
                //判断是否已经生成了虚拟条码
                string S_FG_SN = Public_Repository.BuckToFGSN(S_SN);
                if (string.IsNullOrEmpty(S_FG_SN))
                {
                    string S_FormatSN = await Public_Repository.mesGetSNFormatIDByListAsync(S_PartID, S_PartFamilyID, List_Login.LineID.ToString(), S_POID, List_Login.StationTypeID.ToString());
                    if (string.IsNullOrEmpty(S_FormatSN))
                    {
                        return ("", msgSys.GetLanguage("20034"));
                    }

                    string xmlProdOrder = "<ProdOrder ProductionOrder=" + "\"" + S_POID + "\"" + "> </ProdOrder>";
                    string xmlStation = "<Station StationID=" + "\"" + List_Login.StationID.ToString() + "\"" + "> </Station>";
                    string xmlPart = "<Part PartID=" + "\"" + S_PartID + "\"" + "> </Part>";
                    string xmlExtraData = "<ExtraData BoxSN=" + "\"" + S_SN + "\"" +
                                                   " LineID = " + "\"" + List_Login.LineID.ToString() + "\"" +
                                                   " PartFamilyTypeID=" + "\"" + S_PartFamilyTypeID + "\"" +
                                                   " LineType=" + "\"" + "M" + "\"" +
                                                   " BB=" + "\"" + ColorCode + "\"" +
                                                   " CCCC=" + "\"" + SpcaCode + "\"" +
                                                   " PP=" + "\"" + PpCode.ToUpper() + "\"" +
                                                   " B=" + "\"" + BuildCode + "\"" +
                                                   " ET=" + "\"" + ColorCode + "\"" +
                                                   " SPCA = " + "\"" + SpcaCode.ToUpper() + "\"" +
                                                   "> </ExtraData>";

                    string New_SN = await Public_Repository.GetSNRGetNextAsync(S_FormatSN, "0", xmlProdOrder, xmlPart,
                        xmlStation, xmlExtraData);

                    if (!string.IsNullOrEmpty(SnPattern))
                    {
                        if (!Regex.IsMatch(New_SN, SnPattern))
                        {
                            return ("", msgSys.GetLanguage("20034"));
                        }
                    }

                    mesUnit v_mesUnit = new mesUnit();
                    v_mesUnit.UnitStateID = Convert.ToInt32(S_UnitStateID);
                    v_mesUnit.StatusID = 1;
                    v_mesUnit.StationID = List_Login.StationID;
                    v_mesUnit.EmployeeID = List_Login.EmployeeID;
                    v_mesUnit.CreationTime = DateTime.Now;
                    v_mesUnit.LastUpdate = DateTime.Now;
                    v_mesUnit.PanelID = 0;
                    v_mesUnit.LineID = List_Login.LineID;
                    v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                    v_mesUnit.RMAID = 0;
                    v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                    v_mesUnit.LooperCount = 1;

                    mesUnitDetail v_mesUnitDetail = new mesUnitDetail();
                    v_mesUnitDetail.reserved_01 = S_SN;
                    v_mesUnitDetail.reserved_02 = "";
                    v_mesUnitDetail.reserved_03 = "1";
                    v_mesUnitDetail.reserved_04 = "";
                    v_mesUnitDetail.reserved_05 = "";
                    //mesUnitDetailSVC.Insert(v_mesUnitDetail);

                    mesSerialNumber v_mesSerialNumber = new mesSerialNumber();
                    v_mesSerialNumber.SerialNumberTypeID = 5;
                    v_mesSerialNumber.Value = New_SN;


                    string ReturnValue = DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SN_AddStr(
                        new List<mesUnit>() { v_mesUnit }, new List<mesUnitDetail>() { v_mesUnitDetail },
                        new List<mesSerialNumber>() { v_mesSerialNumber });

                    for (int i = 0; i < 10 && ReturnValue.Contains("PRIMARY KEY") && ReturnValue.Contains("Unit_PK"); i++)
                    {
                        ReturnValue = DataCommit_Repository.SubmitData_UnitAdd_UnitDetailAdd_SN_AddStr(
                        new List<mesUnit>() { v_mesUnit }, new List<mesUnitDetail>() { v_mesUnitDetail },
                        new List<mesSerialNumber>() { v_mesSerialNumber });
                    }

                    if (ReturnValue.StartsWith("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        S_InsertUnit = "NG";
                        return ("", msgSys.MSG_Sys_20130);
                    }
                    else
                    {
                        //ReturnValue = "SN:" + New_SN;
                    }
                    S_InsertUnit = ReturnValue;
                }
                else
                {
                    string S_FirstStationType = await Public_Repository.GetFirstStationTypeAsync(S_SN);
                    S_InsertUnit = "";
                    if (S_FirstStationType == List_Login.StationTypeID.ToString())
                    {
                        var ds = await Public_Repository.GetmesSerialNumber("", S_FG_SN);
                        S_InsertUnit = ds[0].UnitID.ToString();
                        mesUnit v_mesUnit = new mesUnit();
                        v_mesUnit.UnitStateID = Convert.ToInt32(S_UnitStateID);
                        v_mesUnit.StatusID = 1;
                        v_mesUnit.StationID = List_Login.StationID;
                        v_mesUnit.EmployeeID = List_Login.EmployeeID;
                        v_mesUnit.CreationTime = DateTime.Now;
                        v_mesUnit.LastUpdate = DateTime.Now;
                        v_mesUnit.PanelID = 0;
                        v_mesUnit.LineID = List_Login.LineID;
                        v_mesUnit.ProductionOrderID = Convert.ToInt32(S_POID);
                        v_mesUnit.RMAID = 0;
                        v_mesUnit.PartID = Convert.ToInt32(S_PartID);
                        v_mesUnit.LooperCount = 1;
                        v_mesUnit.ID = Convert.ToInt32(S_InsertUnit);
                         var resOO = await DataCommit_Repository.SubmitData_UnitMod(new List<mesUnit> { v_mesUnit });
                        if (resOO != "OK")
                            return ("", msgSys.MSG_Sys_20130);
                    }
                    else
                    {
                        return ("", msgSys.GetLanguage("20154"));
                    }
                }
                return (S_InsertUnit, null);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType, "process error", ex);
                return ("", ex.Message);
            }
        }
        public async Task<AssemblySetPoOutputDto> SetConfirmPOAsync(MesInputDto inputDto)
        {

            var setConfirmPo = await base.SetConfirmPOAsync(inputDto.S_PartFamilyTypeID, inputDto.S_PartFamilyID, inputDto.S_PartID, inputDto.S_POID, inputDto.S_UnitStatus, inputDto.S_URL);

            AssemblySetPoOutputDto assemblyDynamicOutputDto = new AssemblySetPoOutputDto()
            {
                CurrentInitPageInfo = setConfirmPo.CurrentInitPageInfo,
                ErrorMsg = setConfirmPo.ErrorMsg,
                CurrentSettingInfo = setConfirmPo.CurrentSettingInfo,
                DataList = new List<BomLinkedInfo>(),
                UniversalConfirmPoOutput = setConfirmPo.UniversalConfirmPoOutput,
            };

            ///////////////////////////
            string tmpErrorCode = assemblyDynamicOutputDto.ErrorMsg is null ? "" : assemblyDynamicOutputDto.ErrorMsg.ToString();
            if (!string.IsNullOrEmpty(tmpErrorCode))
                return assemblyDynamicOutputDto;
            ///////////////////////////
            var ldBomPartInfoList = await Public_Repository.MESGetBomPartInfoCAsync(inputDto.S_PartID.ToInt(), List_Login.StationTypeID ?? 0);

            
            if (ldBomPartInfoList is null or { Count :< 2})
                return assemblyDynamicOutputDto.SetErrorCode(P_MSG_Public.MSG_Public_6003);


            if(ldBomPartInfoList.Where(x => x.ScanType > 7 || x.ScanType <= 0).Count() > 0)
                return assemblyDynamicOutputDto.SetErrorCode(msgSys.MSG_Sys_20024);

            if (ldBomPartInfoList.Where(x => string.IsNullOrEmpty( x.Pattern)).Count() > 0)
                return assemblyDynamicOutputDto.SetErrorCode(msgSys.MSG_Sys_20025);

            ldBomPartInfoList.First().IsMainSn = true;
            if (!string.IsNullOrEmpty(assemblyDynamicOutputDto.CurrentInitPageInfo.stationAttribute?.SNScanType) &&
               assemblyDynamicOutputDto.CurrentInitPageInfo.stationAttribute?.SNScanType !="0")
            {
                ldBomPartInfoList[0].ScanType = assemblyDynamicOutputDto.CurrentInitPageInfo.stationAttribute.SNScanType.ToInt();
            }

            List<BomLinkedInfo> bomLinkedInfos = new List<BomLinkedInfo>();
            foreach (var item in ldBomPartInfoList)
            {
                BomLinkedInfo bomLinkedInfo = new BomLinkedInfo();
                foreach (PropertyInfo pi in typeof(BomPartInfo).GetProperties())
                {
                    pi.SetValue(bomLinkedInfo, pi.GetValue(item));
                }
                bomLinkedInfos.Add(bomLinkedInfo);
            }

            var bomLinkedInfos2 = ldBomPartInfoList.MapTo<List<BomPartInfo>, List<BomLinkedInfo>>(new List<BomLinkedInfo>());

            assemblyDynamicOutputDto.DataList = bomLinkedInfos;
            //////////////////////////
            return assemblyDynamicOutputDto;
        }
        /// <summary>
        /// 主条码和子条码公共检查部分
        /// </summary>
        /// <param name="MainSN"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <returns></returns>
        private async Task<(bool result, string message, BomLinkedInfo)> VerifyMianCodeAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, BomLinkedInfo currentItem, string sChildSns = "")
        {
            List<dynamic> List_Result = new List<dynamic>();
            if (string.IsNullOrEmpty(MainSN))
            {
                return (false, P_MSG_Public.MSG_Public_016, currentItem);
            }

            try
            {

                string tmpPartId = string.Empty;
                if (mInitPageInfo.stationAttribute.IsCheckPN == "0" || mInitPageInfo.stationAttribute.IsCheckPO == "0")
                {
                    var BaseDataList = await Public_Repository.uspGetBaseDataAsync(MainSN);

                    if (BaseDataList?.Item1 != "1" || BaseDataList?.Item2?.Count <= 0)
                    {
                        return (false, P_MSG_Public.MSG_Public_018, currentItem);
                    }

                    S_POID = BaseDataList.Item2.ConvertDynamic("ProductionOrderID");
                    tmpPartId = BaseDataList.Item2.ConvertDynamic("PartID");
                    S_PartFamilyID = BaseDataList.Item2.ConvertDynamic("PartFamilyID");
                    S_PartFamilyTypeID = BaseDataList.Item2.ConvertDynamic("PartFamilyTypeID");
                }

                if (mInitPageInfo.stationAttribute.IsCheckPO == "0" && mInitPageInfo.stationAttribute.IsCheckPN == "1")
                {
                    if (tmpPartId != S_PartID)
                    {
                        return (false, P_MSG_Public.MSG_Public_6001, currentItem);
                    }
                }

                //var ldPoNumberCheck = await Public_Repository.uspPONumberCheckAsync(S_POID);

                //if (ldPoNumberCheck == null || ldPoNumberCheck.Item1 != "1")
                //{
                //    return (false, P_MSG_Public.MSG_Public_6002, bomPartInfoList);
                //}

                ScanType scan = (ScanType)currentItem.ScanType;
                switch (scan)
                {
                    case ScanType.设备数据_绑定过批次_类似托盘3:
                    case ScanType.设备数据_无批次_类似夹具4:
                    case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                        break;
                    default:
                        Console.WriteLine(currentItem.Pattern);
                        if (!Regex.IsMatch(MainSN, currentItem.Pattern.Replace("\\\\", "\\")))
                        {
                            return (false, P_MSG_Public.MSG_Public_029, currentItem);
                        }
                        break;
                }

                var checkRes = await CheckInputBarcodeAsync(1, MainSN, "", currentItem.Pattern, currentItem.ScanType,
                     S_PartID, S_POID, currentItem.PartID.ToString());
                currentItem.IsClearInput = checkRes.errorCode != "1";
                if (checkRes.errorCode?.ToString() != "1")
                {
                    //获取异常代码，返回错误，并置空状态
                    return (false, msgSys.GetLanguage(checkRes.errorCode), currentItem);
                }

                currentItem.TrayBatchSN = checkRes.batchSN;
                if (string.IsNullOrEmpty(sChildSns))
                {
                    if (string.IsNullOrEmpty(mInitPageInfo.stationAttribute.SNScanType))
                    {
                        mInitPageInfo.stationAttribute.SNScanType = currentItem.ScanType.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
                return (false, "exception", currentItem);
            }
            return (true, null, currentItem);
        }

        /// <summary>
        /// 通用条码检查
        /// </summary>
        /// <param name="type">0 为主码校验， 1为子码校验</param>
        /// <param name="MainBarcode"></param>
        /// <param name="ChildBarcode"></param>
        /// <param name="Pattern"></param>
        /// <param name="scanType"></param>
        /// <param name="currentPartID"></param>
        /// <param name="productOrderID"></param>
        /// <returns></returns>
        private async Task<(string errorCode,string batchSN)> CheckInputBarcodeAsync(int type, string MainBarcode, string ChildBarcode, string Pattern, int scanType, string currentPartID, string productOrderID, string mainPartID)
        {
            string resultCode = "0";
            string xmlPartStr = "<Part PartID=\"" + currentPartID + "\"> </Part>";
            string allBarcode = MainBarcode + "_" + ChildBarcode;
            string newBarcode = string.IsNullOrEmpty(ChildBarcode)? MainBarcode: ChildBarcode;
            string msg = string.Empty;
            string batchSN = string.Empty;
            ScanType st;

            try
            {
                st = (ScanType)scanType;
                switch (st)
                {
                    case ScanType.系统存在数据_SN1:
                        resultCode = type == 1
                            ? await Public_Repository.MESAssembleCheckMianSNAsync(productOrderID, List_Login.LineID, List_Login.StationID, List_Login.StationTypeID ?? 0, MainBarcode, mInitPageInfo.stationAttribute.COF == "1") : await Public_Repository.MESAssembleCheckOtherSNAsync(ChildBarcode, currentPartID, mInitPageInfo.stationAttribute.COF == "1", List_Login);
                        break;
                    case ScanType.非系统注册批次_Batch校验数量_不校验唯一性2:
                    case ScanType.系统注册批次_Batch校验数量7:
                    case ScanType.注册公用批次_扫描子批次校验8:
                        resultCode =
                            await Public_Repository.uspBatchDataCheckAsync(ChildBarcode, null, xmlPartStr, null, null, "1");
                        break;
                    case ScanType.设备数据_绑定过批次_类似托盘3:
                    case ScanType.设备数据_无批次_类似夹具4:
                    case ScanType.设备数据_包含3_4类型_ValidFrom工位可以重复绑定6:
                        string res346 = string.Empty;
                        res346 = await Public_Repository.GetMachineToolingCheck(ChildBarcode, MainBarcode, currentPartID, List_Login);

                        resultCode = res346;
                        if (resultCode != "1")
                        {                            
                            break;
                        }

                        if (type == 1 && resultCode == "1")
                        {
                            List<mesMachine> machines = await Public_Repository.GetmesMachineAsync(MainBarcode);
                            if (machines?.Count <= 0)
                            {
                                resultCode = "0";
                                break;
                            }
                            //是否夹具初始化工位
                            if (machines[0].ValidFrom != List_Login.StationTypeID.ToString())
                            {
                                //Tooling 转换BarCode
                                newBarcode = Public_Repository.BuckToFGSN(MainBarcode);
                                resultCode = await Public_Repository.MESAssembleCheckMianSNAsync(productOrderID, List_Login.LineID,
                                    List_Login.StationID, List_Login.StationTypeID ?? 1, newBarcode, mInitPageInfo.stationAttribute.COF == "1");
                            }

                        }

                        if (scanType == 3 && resultCode == "1")
                        {
                            resultCode =
                                await Public_Repository.uspBatchDataCheckAsync(ChildBarcode, null, xmlPartStr, null, null, "1");

                            var tmpMachine3 = await Public_Repository.BoxSnToBatch(ChildBarcode);
                            if (tmpMachine3.Item1 == "")
                            {
                                resultCode = "20030";
                                break;
                            }

                            batchSN = tmpMachine3.Item1;
                        }
                        break;
                    case ScanType.非系统注册SN_校验唯一性5:
                        bool ischeck = await Public_Repository.MESCheckChildSerialNumberAsync(ChildBarcode);
                        resultCode = ischeck ? "20029" : "1";
                        break;
                    default:
                        break;
                }

                if (resultCode != "1")
                {
                    if (resultCode.Contains("MSG_Sys_20241") || resultCode.Contains("MSG_Sys_20243"))
                    {
                        var tmpUnit = await Public_Repository.GetSerialNumber2Async(newBarcode);

                        var tmpState = await Public_Repository.getMesUnitStateDescAsync(tmpUnit?[0].UnitStateID.ToString());

                        resultCode = PublicF.GetLangStr(string.Format("条码:{0},工艺流程校验失败,当前条码状态{1}@Barcode:{0},process Route failed, current bar code status {1}",newBarcode, tmpState.Description), List_Login.LanguageID);
                    }
                    return (resultCode, batchSN);
                }

                //var checkList = await Public_Repository.uspAssembleCheckAsync(String.IsNullOrEmpty(ChildBarcode) ? MainBarcode : ChildBarcode, productOrderID, currentPartID, List_Login, mainPartID);
                var checkList = await Public_Repository.uspAssembleCheck2Async(String.IsNullOrEmpty(ChildBarcode) ? MainBarcode : ChildBarcode, MainBarcode, productOrderID, currentPartID, List_Login, mainPartID);

                resultCode = checkList?.ToString();
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "process error", ex);
            }
            return (resultCode, batchSN);
        }

        public async Task<AssemblyDynamicOutputDto> ReleaseMachineSNAsync(AssemblyDynamicInput input)
        {
            var output = new AssemblyDynamicOutputDto();
            var confimPo = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(confimPo.ErrorMsg))
                return output.SetErrorCode((string)confimPo.ErrorMsg);

            output.CurrentInitPageInfo = confimPo.CurrentInitPageInfo;
            output.CurrentSettingInfo = confimPo.CurrentSettingInfo;
            output.DataList = confimPo.DataList;

            var mDataList = input.DataList;
            if (mDataList is null || mDataList.Count < 2)
                return output.SetErrorCode(P_MSG_Public.MSG_Public_6003);

            if (mDataList.Count(x => x.IsCurrentItem) != 1)
                return output.SetErrorCode(P_MSG_Public.MSG_Public_6014);

            BomLinkedInfo currentBomInfo = null;

            currentBomInfo = mDataList.Where(x => x.IsCurrentItem).FirstOrDefault();
            if (currentBomInfo is null or { Barcode: "" } or { IsScanFinished: true })
                return output.SetErrorCode(msgSys.MSG_Sys_20007);

            int itemIndex = mDataList.IndexOf(currentBomInfo);
            if (currentBomInfo.ScanType != 3 && currentBomInfo.ScanType != 6)
                return output.SetErrorCode(P_MSG_Public.MSG_Public_6014);

            string result = Public_Repository.MesToolingReleaseCheck(currentBomInfo.Barcode, List_Login.StationTypeID.ToString());

            if (result != "1")
                return output.SetErrorCode(msgSys.GetLanguage(result));
            Public_Repository.ModMachine(currentBomInfo.Barcode, "1", true);

            return output;
        }
    }
}
