using SunnyMES.Security.Repositories;
using SunnyMES.Security._3_IRepositories.MES.Package;
using System.Threading.Tasks;
using SunnyMES.Security._2_Dtos.MES.MES_Output.ShipMent;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.ShipMent;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Security.ToolExtensions;
using SunnyMES.Commons.Extensions;
using System;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Dtos;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxScalage;
using NPOI.SS.Formula.Functions;
using Quartz.Impl.Triggers;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;
using System.Linq;
using SunnyMES.Security._1_Models.MES.Query;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;
using SunnyMES.Security._1_Models.MES.Query.ShipMent;
using Microsoft.AspNetCore.Builder;

namespace SunnyMES.Security._4_Repositories.MES.Package
{

    public class ShipMentRepository : MesBaseRepository, IShipMentRepository
    {
        public ShipMentRepository(IDbContextCore contextCore) : base(contextCore)
        {
        }

        public async Task<ShipMentOutput> MainSnVerifyAsync(MesSnInputDto input)
        {
            ShipMentOutput mShipMentOut = new ShipMentOutput();
            if (string.IsNullOrEmpty(input.S_SN))
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20007"));

            var setPoConfirm = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(setPoConfirm.ErrorMsg))
            {
                mShipMentOut.ErrorMsg = setPoConfirm.ErrorMsg;
                return mShipMentOut;
            }

            if (string.IsNullOrEmpty(xmlStation))
                xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
            if (string.IsNullOrEmpty(xmlExtraData))
                xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";
            var shipmentData = await Public_Repository.uspGetShipMentDataAsync(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (shipmentData.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage( shipmentData.Item1.ToString()));

            var smds = shipmentData.Item2;
            if (smds is null or { Count: <= 0 })
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20197"));
            
            mShipMentOut.ShipMentDatas = smds;
            var shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            if (shipmentDetail.Item1 == "1")
                mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;

            mShipMentOut.PrintType = await Public_Repository.uspGetShipmentPrintTypeAsync(input.S_SN);
            if (mShipMentOut.PrintType == "1")
            {
                var tmpShipmentPallet = await Public_Repository.GetShipmentPalletSNAsync(input.S_SN);

                if (tmpShipmentPallet is null || !tmpShipmentPallet.Any())
                {
                    mesUnit mesUnit = new mesUnit
                    {
                        StationID = List_Login.StationID,
                        EmployeeID = List_Login.EmployeeID,
                        ProductionOrderID = input.S_POID.ToInt(),
                        PartID = input.S_PartID.ToInt()
                    };

                    var shipmentPallet = await Public_Repository.Get_CreatePackageSN(setPoConfirm.PrinterParams.SNFormatName, xmlProdOrder, xmlPart,
                        xmlStation, xmlExtraData, mesUnit, 3);

                    if (shipmentPallet.Item1 != "1")
                    {
                        return mShipMentOut.SetErrorCode(msgSys.GetLanguage(shipmentPallet.Item1));
                    }
                    mShipMentOut.ShippingPallet = shipmentPallet.Item2;
                }
                else
                {
                    if (tmpShipmentPallet.Count() != 1)
                        return mShipMentOut.SetErrorCode(P_MSG_Public.MSG_Public_6050);

                    mShipMentOut.ShippingPallet = tmpShipmentPallet.ToList()[0].SerialNumber;
                } 
            }
            return mShipMentOut;
        }

        public async Task<ShipMentOutput> MultipackSnVerifyAsync(ShipMentInput input)
        {
            var mShipMentOut = new ShipMentOutput();
            if (string.IsNullOrEmpty(input.S_SN) || string.IsNullOrEmpty(input.MultipackSn))
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20007"));

            var setPoConfirm = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(setPoConfirm.ErrorMsg?.ToString()))
                return mShipMentOut.SetErrorCode((string)setPoConfirm.ErrorMsg);

            mShipMentOut.PrinterParams = setPoConfirm.PrinterParams;

            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
            //xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\" BillNO = "++"> </ExtraData>";
            xmlExtraData = $"<ExtraData EmployeeId= \"{baseCommonHeader.EmployeeId.ToString()}\" BillNO = \"{input.S_SN}\"> </ExtraData>";
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";

            string strOutputCheck = string.Empty;
            strOutputCheck = await Public_Repository.uspPackageRouteCheck(input.MultipackSn, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");

            if (strOutputCheck != "1")
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(strOutputCheck));

            var shipmentData = await Public_Repository.uspGetShipMentDataAsync(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (shipmentData.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(shipmentData.Item1.ToString()));

            var smds = shipmentData.Item2;
            if (smds is null or { Count: <= 0 })
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20197"));

            mShipMentOut.ShipMentDatas = smds;

            var shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            //if (shipmentDetail.Item1 != "1" || shipmentDetail.Item2 == null || shipmentDetail.Item2.Count <= 0)
            //    return mShipMentOut.SetErrorCode(msgSys.GetLanguage(shipmentDetail.Item1));
            if (shipmentDetail.Item1 == "1")
                mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;


            mShipMentOut.PrintType = await Public_Repository.uspGetShipmentPrintTypeAsync(input.S_SN);
            if (mShipMentOut.PrintType == "1")
            {
                mShipMentOut.ShippingPallet = input.ShippingPallet;

                if (string.IsNullOrEmpty( input.ShippingPallet))
                    return mShipMentOut.SetErrorCode(P_MSG_Public.MSG_Public_6041);

                var tmpShipmentPallet = await Public_Repository.GetShipmentPalletSNAsync(input.S_SN);

                var tmpShipmentPallets = tmpShipmentPallet?.ToList();
                if (tmpShipmentPallets is not null and { Count : 1})
                {
                    if (input.ShippingPallet != tmpShipmentPallets[0].SerialNumber)
                        return mShipMentOut.SetErrorCode(P_MSG_Public.MSG_Public_6041);
                }
            }

            //未更新 打印类型为1的相关表
            var detailOutput = await Public_Repository.SetShipmentMultipackAsync(input.S_SN, input.MultipackSn);
            if (detailOutput is null or { OutResult : not "1"})
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(detailOutput?.OutResult));

            if (detailOutput is null or { FDetailID: "" or null })
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20197);

            int shipmentOutCount = 0;
            bool ScanOver = false;

            var currentData = mShipMentOut.ShipMentDatas.Where(x => x.FDetailID.ToString() == detailOutput.FDetailID).FirstOrDefault();
            if (currentData is not null)
                currentData.FOutSN += 1;
            ScanOver = mShipMentOut.ShipMentDatas.Where(x => x.FCTN == x.FOutSN).Count() == mShipMentOut.ShipMentDatas.Count;

            

            string shippmentCount = string.Empty;
            string submitSql = string.Empty;
            //未查询是否扫描完成
            if (mShipMentOut.PrintType == "1")
            {
                if (ScanOver)
                {
                    var tmpSqls = new List<string>();
                    submitSql = await DataCommit_Repository.uspPalletPackagingSqlAsync(input.S_PartID, input.S_POID, input.MultipackSn, mShipMentOut.ShippingPallet, List_Login, input.S_SN, mShipMentOut.PrintType, detailOutput.FDetailID, ScanOver, 0);
                    tmpSqls.Add(submitSql);
                    submitSql = await DataCommit_Repository.uspPalletPackagingSqlAsync(input.S_PartID, input.S_POID, input.MultipackSn, mShipMentOut.ShippingPallet, List_Login, input.S_SN, mShipMentOut.PrintType, detailOutput.FDetailID, ScanOver, 1);
                    tmpSqls.Add(submitSql);
                    strOutputCheck = await ExecuteTransactionSqlAsync(tmpSqls);

                    if (strOutputCheck != "1")
                        return mShipMentOut.SetErrorCode(msgSys.GetLanguage(strOutputCheck));

                    var printData = await Public_Repository.uspGetShipMentPrintDataAsync(mShipMentOut.ShippingPallet);
                    if (printData.Item1 != "1")
                        return mShipMentOut.SetErrorCode(msgSys.GetLanguage(printData.Item1));

                    mShipMentOut.ShipmentPrintData = printData.Item2;

                    int MpnCount = mShipMentOut.ShipMentDatas.GroupBy(x => x.FMPNNO).Count();
                    mShipMentOut.PrinterParams.IsNormalSKU = MpnCount > 1 ? false : true;

                    //GS label print data
                    mShipMentOut.ShipmentSiglePrintData = mShipMentOut.PrinterParams.IsNormalSKU ? mShipMentOut.ShipmentPrintData[0].SerialNumber : mShipMentOut.ShippingPallet;
                    var tmpGSName = mShipMentOut.PrinterParams.IsNormalSKU ? mShipMentOut.PrinterParams.GS1Name : mShipMentOut.PrinterParams.GS2Name;
                    var LabelPath = await Public_Repository.GetMesLabelDataAsync(tmpGSName);
                    if (LabelPath is null or { LablePath: "" })
                        return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20104);

                    mShipMentOut.PrinterParams.GSCommand = LabelPath.LablePath;
                    mShipMentOut.PrinterParams.IsPrint = true;
                }
                else
                {
                    strOutputCheck = await DataCommit_Repository.uspPalletPackagingAsync(input.S_PartID, input.S_POID, input.MultipackSn, mShipMentOut.ShippingPallet, List_Login, input.S_SN, mShipMentOut.PrintType, detailOutput.FDetailID, ScanOver, 0);
                    if (strOutputCheck != "1")
                        return mShipMentOut.SetErrorCode(msgSys.GetLanguage(strOutputCheck));
                }
            }
            else if (mShipMentOut.PrintType == "2")
            {
                //
                mesUnit mesUnit = new mesUnit
                {
                    StationID = List_Login.StationID,
                    EmployeeID = List_Login.EmployeeID,
                    ProductionOrderID = Convert.ToInt32(input.S_POID),
                    PartID = Convert.ToInt32(input.S_PartID)
                };
                var insertPalletSnRes = await Public_Repository.Get_CreatePackageSN(setPoConfirm.PrinterParams.SNFormatName, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, mesUnit, 3);
                if (insertPalletSnRes.Item1 != "1")
                    return mShipMentOut.SetErrorCode(msgSys.GetLanguage(insertPalletSnRes.Item1));

                mShipMentOut.ShippingPallet = insertPalletSnRes.Item2;
                var tmpSqls = new List<string>();
                submitSql = await DataCommit_Repository.uspPalletPackagingSqlAsync(input.S_PartID, input.S_POID, input.MultipackSn, mShipMentOut.ShippingPallet, List_Login, input.S_SN, mShipMentOut.PrintType, detailOutput.FDetailID, ScanOver, 0);
                tmpSqls.Add(submitSql);
                submitSql = await DataCommit_Repository.uspPalletPackagingSqlAsync(input.S_PartID, input.S_POID, input.MultipackSn, mShipMentOut.ShippingPallet, List_Login, input.S_SN, mShipMentOut.PrintType, detailOutput.FDetailID, ScanOver, 1);
                tmpSqls.Add(submitSql);
                strOutputCheck = await ExecuteTransactionSqlAsync(tmpSqls);

                if (strOutputCheck != "1")
                    return mShipMentOut.SetErrorCode(msgSys.GetLanguage(strOutputCheck));

                //打印箱码
                mShipMentOut.ShipmentSiglePrintData = input.MultipackSn;
                mShipMentOut.PrinterParams.IsPrint = true;
            }

            mShipMentOut.ShipMentDetailDatas.Add(new ShipMentDetailData()
            {
                ID = mShipMentOut.ShipMentDetailDatas.Count + 1,
                MultiPackSN = input.MultipackSn
            });
            mShipMentOut.IsPackingFinish = ScanOver;
            return mShipMentOut;
        }



        public async Task<ShipMentOutput> ReplaceBillNOAsync(ShipMentReplaceInput input)
        {
            ShipMentOutput shipMentOutput = new ShipMentOutput();

            if (string.IsNullOrEmpty(input.OriginalBillNO) || string.IsNullOrEmpty(input.TargetBillNO))
                return shipMentOutput.SetErrorCode(msgSys.GetLanguage("20007"));

            var setPoConfirm = await SetConfirmPOAsync(input);
            shipMentOutput.PrinterParams = setPoConfirm.PrinterParams;

            var replaceShipmentOutput = await Public_Repository.uspReplaceShipmentPalletAsync(input.TargetBillNO, input.OriginalBillNO);
            if (replaceShipmentOutput.strOutput != "1")
                return shipMentOutput.SetErrorCode(msgSys.GetLanguage(replaceShipmentOutput.strOutput));

            #region 单独打印数据
            ////通过箱码查询对应需要打印的数据
            //var shipmentReprint = await Public_Repository.uspGetShipMentRePrintAsync(replaceShipmentOutput.BoxSN);
            //if (shipmentReprint.Item1 != "1")
            //    return shipMentOutput.SetErrorCode(msgSys.GetLanguage(shipmentReprint.Item1));

            //if (shipmentReprint.Item2 is null or { Count: <= 0 })
            //    return shipMentOutput.SetErrorCode(msgSys.MSG_Sys_20197);

            //var mFirstReprint = shipmentReprint.Item2[0];
            //if (mFirstReprint is null)
            //    return shipMentOutput.SetErrorCode(msgSys.MSG_Sys_20197);

            //shipMentOutput.PrintType = string.IsNullOrEmpty(mFirstReprint.LabelSCType) ? "2" : "1";

            //var tmpDetail = new List<ShipMentDetailData>();
            //for (int i = 0; i < shipmentReprint.Item2.Count; i++)
            //{
            //    tmpDetail.Add(new ShipMentDetailData()
            //    {
            //        ID = i + 1,
            //        MultiPackSN = shipmentReprint.Item2[i].SerialNumber
            //    });
            //}
            //shipMentOutput.ShipMentDetailDatas = tmpDetail;

            ////两种类型都需要打印的数据
            //shipMentOutput.ShipmentSiglePrintData = mFirstReprint.SerialNumber;
            //shipMentOutput.ShippingPallet = mFirstReprint.PalletSN;

            //MesLabelData mGsLabelData = (mFirstReprint.LabelSCType) switch
            //{
            //    "1" => await Public_Repository.GetMesLabelDataAsync(setPoConfirm.PrinterParams.GS1Name),
            //    "2" => await Public_Repository.GetMesLabelDataAsync(setPoConfirm.PrinterParams.GS2Name),
            //    _ => null
            //};

            //if (mGsLabelData is null)
            //{
            //    //打印类型为2，一个箱子一个标签
            //    return shipMentOutput;
            //}

            //#region 1
            ////var printData = await Public_Repository.uspGetShipMentPrintDataAsync(shipMentOutput.ShippingPallet);
            ////if (printData.Item1 != "1")
            ////    return shipMentOutput.SetErrorCode(msgSys.GetLanguage(printData.Item1));

            ////shipMentOutput.ShipmentPrintData = printData.Item2;
            //#endregion


            //List<ShipmentMupltipack> shipmentMupltipacks = new List<ShipmentMupltipack>();
            //foreach (var item in shipmentReprint.Item2)
            //{
            //    shipmentMupltipacks.Add(new ShipmentMupltipack() { SerialNumber = item.SerialNumber });
            //}
            //shipMentOutput.ShipmentPrintData = shipmentMupltipacks;

            //shipMentOutput.PrinterParams.IsNormalSKU = mFirstReprint.LabelSCType == "1";
            ////GS data
            //shipMentOutput.ShipmentSiglePrintData = shipMentOutput.PrinterParams.IsNormalSKU ? mFirstReprint.SerialNumber : mFirstReprint.PalletSN;
            //shipMentOutput.PrinterParams.GSCommand = mGsLabelData?.LablePath;
            #endregion

            return await ReprintSnAsync(new MesSnInputDto() {
                S_PartFamilyTypeID = input.S_PartFamilyTypeID,
                S_PartFamilyID = input.S_PartFamilyID,
                S_PartID = input.S_PartID,
                S_POID = input.S_POID,
                S_URL = input.S_URL,
                S_SN = replaceShipmentOutput.BoxSN,
                S_UnitStatus = input.S_UnitStatus,
                S_DefectID = input.S_DefectID,
                Id = input.Id
            });
        }

        public async Task<ShipMentOutput> ReprintSnAsync(MesSnInputDto input)
        {
            ShipMentOutput shipMentOutput = new ShipMentOutput();
            var setPoConfirm = await SetConfirmPOAsync(input);
            shipMentOutput.PrinterParams = setPoConfirm.PrinterParams;


            //通过箱码查询对应需要打印的数据
            var shipmentReprint = await Public_Repository.uspGetShipMentRePrintAsync(input.S_SN);
            if (shipmentReprint.Item1 != "1")
                return shipMentOutput.SetErrorCode(msgSys.GetLanguage(shipmentReprint.Item1));

            if (shipmentReprint.Item2 is null or { Count: <= 0 })
                return shipMentOutput.SetErrorCode(msgSys.MSG_Sys_20197);

            var mFirstReprint = shipmentReprint.Item2[0];
            if (mFirstReprint is null)
                return shipMentOutput.SetErrorCode(msgSys.MSG_Sys_20197);

            shipMentOutput.PrintType = string.IsNullOrEmpty(mFirstReprint.LabelSCType) ? "2" : "1";

            var tmpDetail = new List<ShipMentDetailData>();
            for (int i = 0; i < shipmentReprint.Item2.Count; i++)
            {
                tmpDetail.Add(new ShipMentDetailData()
                {
                    ID = i + 1,
                    MultiPackSN = shipmentReprint.Item2[i].SerialNumber
                });
            }
            shipMentOutput.ShipMentDetailDatas = tmpDetail;
            shipMentOutput.PrinterParams.IsPrint = true;

            //两种类型都需要打印的数据
            shipMentOutput.ShipmentSiglePrintData = mFirstReprint.SerialNumber;
            shipMentOutput.ShippingPallet = mFirstReprint.PalletSN;

            MesLabelData mGsLabelData = (mFirstReprint.LabelSCType) switch
            {
                "1" => await Public_Repository.GetMesLabelDataAsync(setPoConfirm.PrinterParams.GS1Name),
                "2" => await Public_Repository.GetMesLabelDataAsync(setPoConfirm.PrinterParams.GS2Name),
                _ => null
            };

            if (mGsLabelData is null)
            {
                //打印类型为2，一个箱子一个标签
                return shipMentOutput;
            }

            #region 1
            //var printData = await Public_Repository.uspGetShipMentPrintDataAsync(shipMentOutput.ShippingPallet);
            //if (printData.Item1 != "1")
            //    return shipMentOutput.SetErrorCode(msgSys.GetLanguage(printData.Item1));
            //shipMentOutput.ShipmentPrintData = printData.Item2;
            #endregion

            List<ShipmentMupltipack> shipmentMupltipacks = new List<ShipmentMupltipack>();
            foreach (var item in shipmentReprint.Item2)
            {
                shipmentMupltipacks.Add(new ShipmentMupltipack() { SerialNumber = item.SerialNumber });
            }
            shipMentOutput.ShipmentPrintData = shipmentMupltipacks;

            shipMentOutput.PrinterParams.IsNormalSKU = mFirstReprint.LabelSCType == "1";

            shipMentOutput.ShippingPallet = shipMentOutput.PrinterParams.IsNormalSKU ? mFirstReprint.SerialNumber : mFirstReprint.PalletSN;
            shipMentOutput.PrinterParams.GSCommand = mGsLabelData?.LablePath;

            return shipMentOutput;
        }

        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
        {
            SetConfirmPoOutput setConfirmPo = new SetConfirmPoOutput();

            //var vvvv = await Public_Repository.uspGetShipMentRePrintAsync("00008884629579181639");

            if (string.IsNullOrEmpty(input.S_PartFamilyTypeID))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20115"));
            if (string.IsNullOrEmpty(input.S_PartFamilyID))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20116"));
            if (string.IsNullOrEmpty(input.S_PartID))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20117"));
            if (string.IsNullOrEmpty(input.S_POID))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20118"));

            setConfirmPo = await base.SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL);
            if (!string.IsNullOrEmpty(setConfirmPo.ErrorMsg))
                return setConfirmPo;

            setConfirmPo.PrinterParams = new _2_Dtos.MES.MES_Output.PrinterParams();
            setConfirmPo.PrinterParams.IsPrint = false;

            var GS1 = await Public_Repository.GetMesPartAndPartFamilyDetail(input.S_PartID.ToInt(), "GS1_PalletLabelName");
            if (GS1.Item1 != "1")
                return setConfirmPo.SetErrorCode<SetConfirmPoOutput>(msgSys.GetLanguage("20198"), "GS1_PalletLabelName");
            setConfirmPo.PrinterParams.GS1Name = GS1.Item2.DetailValue;
            setConfirmPo.PrinterParams.PrintIPPort = setConfirmPo.CurrentInitPageInfo.stationAttribute.PrintIPPort;
            var GS2 = await Public_Repository.GetMesPartAndPartFamilyDetail(input.S_PartID.ToInt(), "GS2_PalletLabelName");
            if (GS2.Item1 != "1")
                return setConfirmPo.SetErrorCode<SetConfirmPoOutput>(msgSys.GetLanguage("20198"), "GS2_PalletLabelName");
            setConfirmPo.PrinterParams.GS2Name = GS2.Item2.DetailValue;

            string PalletSNFormatName = await Public_Repository.mesGetSNFormatIDByListAsync(input.S_PartID, input.S_PartFamilyID, List_Login.LineID.ToString(), input.S_POID, List_Login.StationTypeID.ToString());
            if (string.IsNullOrEmpty(PalletSNFormatName))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20075"));
            setConfirmPo.PrinterParams.SNFormatName = PalletSNFormatName;

            string S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(), input.S_PartFamilyID, input.S_PartID, input.S_POID, List_Login.LineID.ToString());
            if (string.IsNullOrEmpty(S_LabelPath))
                return setConfirmPo.SetErrorCode(msgSys.GetLanguage("20076"));
            else
            {
                if (S_LabelPath.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                    return mSetConfirmPoOutput.SetErrorCode(S_LabelPath);
            }
            setConfirmPo.PrinterParams.LabelCommand = S_LabelPath;


            //var printData = await Public_Repository.uspGetShipMentPrintDataAsync("00108884625953910069");

            //string pathList = string.Empty;
            //string[] ListTemplate = S_LabelPath.Split(';');
            //foreach (string str in ListTemplate)
            //{
            //    string[] listStr = str.Split(',');
            //    pathList = (string.IsNullOrEmpty(pathList) ? "" : pathList + ";") + listStr[1].ToString();
            //}
            //setConfirmPo.PrinterParams.LabelPath = pathList.Replace(@"\\", @"\");
            //setConfirmPo.PrinterParams.LabelCommand = S_LabelPath;

            return setConfirmPo;
        }
        public async Task<ShipMentOutput> RemoveMultipackSnAsync(ShipMentInput input)
        {
            var mShipMentOut = new ShipMentOutput();
            if (string.IsNullOrEmpty(input.S_SN) || string.IsNullOrEmpty(input.MultipackSn))
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20007"));

            var setPoConfirm = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(setPoConfirm.ErrorMsg?.ToString()))
                return mShipMentOut.SetErrorCode((string)setPoConfirm.ErrorMsg);

            mShipMentOut.PrinterParams = setPoConfirm.PrinterParams;

            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";

            string strOutputCheck = string.Empty;

            var shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            if (shipmentDetail.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20197);

            mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;
            if (mShipMentOut.ShipMentDetailDatas is null or { Count: 0 })
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_70062);

            if (mShipMentOut.ShipMentDetailDatas.Where(x => x.MultiPackSN == input.MultipackSn).Count() != 1)
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_70063);

            var sqlOutput = await Public_Repository.uspMoveShipmentMultipackAsync(input.S_SN, input.MultipackSn, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (sqlOutput is null or { strOutput: not "1" })
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(sqlOutput?.strOutput));

            shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            if (shipmentDetail.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20197);

            mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;

            var shipmentData = await Public_Repository.uspGetShipMentDataAsync(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (shipmentData.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(shipmentData.Item1.ToString()));

            var smds = shipmentData.Item2;
            if (smds is null or { Count: <= 0 })
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20197"));

            mShipMentOut.ShipMentDatas = smds;


            //前端需要判断 mShipMentOut.ShipMentDetailDatas 的数量为空或者0时需要清空BillNO,否则只清空箱码
            return mShipMentOut;
        }
        public async Task<ShipMentOutput> UnpackShipmentPalletAsync(ShipMentInput input)
        {
            var mShipMentOut = new ShipMentOutput();
            if (string.IsNullOrEmpty(input.S_SN) || string.IsNullOrEmpty(input.ShippingPallet))
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20007"));

            var setPoConfirm = await SetConfirmPOAsync(input);
            if (!string.IsNullOrEmpty(setPoConfirm.ErrorMsg?.ToString()))
                return mShipMentOut.SetErrorCode((string)setPoConfirm.ErrorMsg);

            mShipMentOut.PrinterParams = setPoConfirm.PrinterParams;

            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";

            string strOutputCheck = string.Empty;
            var shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            if (shipmentDetail.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20197);
            mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;


            if (mShipMentOut.ShipMentDetailDatas is null or { Count: 0 })
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_70062);

            var unpackRes = await Public_Repository.uspUnpackShipmentPalletAsync(input.S_SN, input.ShippingPallet, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (unpackRes is null or { strOutput : not "1"})
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(unpackRes?.strOutput));

            shipmentDetail = await Public_Repository.uspGetShipMentDetailAsync(input.S_SN);
            if (shipmentDetail.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.MSG_Sys_20197);
            mShipMentOut.ShipMentDetailDatas = shipmentDetail.Item2;

            var shipmentData = await Public_Repository.uspGetShipMentDataAsync(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData);
            if (shipmentData.Item1 != "1")
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage(shipmentData.Item1.ToString()));

            var smds = shipmentData.Item2;
            if (smds is null or { Count: <= 0 })
                return mShipMentOut.SetErrorCode(msgSys.GetLanguage("20197"));
            mShipMentOut.ShipMentDatas = smds;

            return mShipMentOut;
        }
    }
}
