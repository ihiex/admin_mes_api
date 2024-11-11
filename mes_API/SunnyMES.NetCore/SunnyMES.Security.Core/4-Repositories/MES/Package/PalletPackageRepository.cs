using Dapper;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.PalletPackage;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Security._1_Models.MES;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxPackageAuto;
using SunnyMES.Security._2_Dtos.MES.MES_Output.PalletPackage;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Models;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories;
 
public class PalletPackageRepository : MesBaseRepository, IPalletPackageRepository
{
    string PalletSNFormatName;
    string PalletSN_Pattern;
    public PalletPackageRepository(IDbContextCore contextCore) : base(contextCore)
    {

    }

    public new async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
    {
        var mConfirmPoOutput = await base.SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL);

        if (!string.IsNullOrEmpty(mConfirmPoOutput.ErrorMsg))
            return mConfirmPoOutput;

        if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.PalletQty.ToInt() == 0)
            return mConfirmPoOutput.SetErrorCode(msgSys.MSG_Sys_20091);

        if (mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN is "1")
        {
            mConfirmPoOutput.PrinterParams = new PrinterParams();
            mConfirmPoOutput.PrinterParams.IsPrint = true;

            PalletSNFormatName = await Public_Repository.mesGetSNFormatIDByListAsync(input.S_PartID, input.S_PartFamilyID, List_Login.LineID.ToString(), input.S_POID, List_Login.StationTypeID.ToString());
            if (string.IsNullOrEmpty(PalletSNFormatName))
                return mConfirmPoOutput.SetErrorCode(msgSys.GetLanguage("20075"));


            mConfirmPoOutput.PrinterParams.SNFormatName = PalletSNFormatName;

            mConfirmPoOutput.PrinterParams.PrintIPPort =
                mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.PrintIPPort;

            //if (string.IsNullOrEmpty(mConfirmPoOutput.PrinterParams.PrintIPPort))
            //    return mConfirmPoOutput.SetErrorCode(P_MSG_Public.MSG_Public_6042);

            string S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(),
                input.S_PartFamilyID, input.S_PartID, input.S_POID, List_Login.LineID.ToString());
            if (string.IsNullOrEmpty(S_LabelPath))
                return mConfirmPoOutput.SetErrorCode(msgSys.GetLanguage("20076"));
            else
            {
                if (S_LabelPath.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                    return mSetConfirmPoOutput.SetErrorCode(S_LabelPath);
            }
            string pathList = string.Empty;
            string[] ListTemplate = S_LabelPath.Split(';');
            foreach (string str in ListTemplate)
            {
                string[] listStr = str.Split(',');
                pathList = (string.IsNullOrEmpty(pathList) ? "" : pathList + ";") + listStr[1].ToString();
            }
            mConfirmPoOutput.PrinterParams.LabelPath = pathList.Replace(@"\\", @"\");
            mConfirmPoOutput.PrinterParams.LabelCommand = S_LabelPath;
        }

        return mSetConfirmPoOutput = mConfirmPoOutput;
    }  

    public async Task<PalletPackageOutput> MainSnVerifyAsync(PalletPackageInput input)
    {
        var setPoConfirmed = await SetConfirmPOAsync(input);
        PalletPackageOutput palletPackageOutput = new PalletPackageOutput()
        {
            CurrentInitPageInfo = setPoConfirmed.CurrentInitPageInfo,
            CurrentSettingInfo = setPoConfirmed.CurrentSettingInfo,
            ErrorMsg = setPoConfirmed.ErrorMsg,
        };

        if (!string.IsNullOrEmpty(palletPackageOutput.ErrorMsg))
            return palletPackageOutput;


        if (setPoConfirmed.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN == "1")
        {
            return palletPackageOutput.SetErrorCode(msgSys.MSG_Sys_20007);
        }

        PalletSN_Pattern = setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletSN_Pattern;
        if (!Regex.IsMatch(input.S_SN, PalletSN_Pattern))
            return palletPackageOutput.SetErrorCode(msgSys.GetLanguage("20027"));

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";

        var output = await Public_Repository.uspPalletCheckAsync(input.S_SN,xmlProdOrder,xmlPart,xmlStation,xmlExtraData, "PALLET");
        if (output != "1")
            return palletPackageOutput.SetErrorCode(msgSys.GetLanguage(output));

        var palletData = await Public_Repository.GetPalletDataAsync(input.S_SN);
        palletPackageOutput.PalletConfirmeds = palletData;

        var mMesPackage = await Public_Repository.GetMesPackageBySNAsync(input.S_SN);
        
        palletPackageOutput.IsPackingFinish = mMesPackage.StatusID == 1;
        palletPackageOutput.PalletSn = input.S_SN;

        if (mMesPackage.StatusID == 1)
            return palletPackageOutput.SetErrorCode(P_MSG_Public.MSG_Public_6047);

        return palletPackageOutput;
    }

    public async Task<PalletPackageOutput> DynamicSnVerifyAsync(PalletPackageInput input)
    {
        var setPoConfirmed = await SetConfirmPOAsync(input);
        PalletPackageOutput palletPackageOutput = new PalletPackageOutput()
        { 
             CurrentInitPageInfo = setPoConfirmed.CurrentInitPageInfo,
             CurrentSettingInfo = setPoConfirmed.CurrentSettingInfo,
             ErrorMsg = setPoConfirmed.ErrorMsg,
        };

        if (!string.IsNullOrEmpty(palletPackageOutput.ErrorMsg))
            return palletPackageOutput;

        if (string.IsNullOrEmpty(input.S_BoxSN))
            return palletPackageOutput.SetErrorCode(msgSys.MSG_Sys_20007);

        if (!string.IsNullOrEmpty(input.S_SN))
        {
            PalletSN_Pattern = setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletSN_Pattern;
            if (!Regex.IsMatch(input.S_SN, PalletSN_Pattern))
                return palletPackageOutput.SetErrorCode(msgSys.GetLanguage("20027"));

            palletPackageOutput.PalletConfirmeds = await Public_Repository.GetPalletDataAsync(input.S_SN);
            if (palletPackageOutput.PalletConfirmeds.Count >= setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletQty.ToInt(0))
                return palletPackageOutput.SetErrorCode(msgSys.GetLanguage("20086"));

            var palletPackage = await Public_Repository.GetMesPackageBySNAsync(input.S_SN);
            if (palletPackage is null or { StatusID : 1,Stage : 2 })
                return palletPackageOutput.SetErrorCode(P_MSG_Public.MSG_Public_6047);

            if( palletPackageOutput.PalletConfirmeds.Where(x => x.KITSN == input.S_BoxSN).ToList().Count() > 0)
                return palletPackageOutput.SetErrorCode(P_MSG_Public.MSG_Public_041);
        }
        else
        {
            do
            {
                if (setPoConfirmed.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN != "1")
                    break;
                var tmpBoxPackage = await Public_Repository.GetMesPackageBySNAsync(input.S_BoxSN);
                if (tmpBoxPackage is null or { StatusID: not 1 })
                    return palletPackageOutput.SetErrorCode(msgSys.MSG_Sys_20190);

                if (tmpBoxPackage is { ParentID: null or <= 0 })
                    break;

                var tmpPalletPackage = await Public_Repository.GetMesPackageByIDAsync(tmpBoxPackage.ParentID.ToString());
                if (tmpPalletPackage is null or { Stage: 1 })
                    break;

                palletPackageOutput.PalletSn = tmpPalletPackage.SerialNumber;
                palletPackageOutput.PalletConfirmeds = await Public_Repository.GetPalletDataAsync(palletPackageOutput.PalletSn);
                return palletPackageOutput;
            } while (false);
        }

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
        xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";
        var output = await Public_Repository.uspPalletCheckAsync(input.S_BoxSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "BOX");
        if (output != "1")
            return palletPackageOutput.SetErrorCode(msgSys.GetLanguage(output));

        if (setPoConfirmed.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN == "1" && string.IsNullOrEmpty(input.S_SN))
        {
            mesUnit mesUnit = new mesUnit
            {
                StationID = List_Login.StationID,
                EmployeeID = List_Login.EmployeeID,
                ProductionOrderID = Convert.ToInt32(input.S_POID),
                PartID = Convert.ToInt32(input.S_PartID)
            };
            var insertPalletSnRes = await Public_Repository.Get_CreatePackageSN(PalletSNFormatName, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, mesUnit, 2);

            if (insertPalletSnRes.Item1 != "1")
                return palletPackageOutput.SetErrorCode(msgSys.GetLanguage(insertPalletSnRes.Item1));
            palletPackageOutput.PalletSn = insertPalletSnRes.Item2;
            if (!Regex.IsMatch(palletPackageOutput.PalletSn, mSetConfirmPoOutput.CurrentInitPageInfo.poAttributes.PalletSN_Pattern))
                return palletPackageOutput.SetErrorCode(msgSys.GetLanguage("20027"));
            input.S_SN = palletPackageOutput.PalletSn;
        }
        var updateOutput = await DataCommit_Repository.uspPalletPackaging(input.S_PartID, input.S_POID, input.S_BoxSN, input.S_SN, List_Login, setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletQty.ToInt());

        if (updateOutput.Item1 != "1")
            return palletPackageOutput.SetErrorCode(msgSys.GetLanguage(updateOutput.Item1));
        var palletData = await Public_Repository.GetPalletDataAsync(input.S_SN);
        palletPackageOutput.PalletConfirmeds = palletData;
        palletPackageOutput.PalletSn = input.S_SN;
        if (updateOutput.Item2 == setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletQty.ToInt()) 
        {
            palletPackageOutput.IsPackingFinish = true;
            if (setPoConfirmed.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN == "1")
            {

            }
        }

        return palletPackageOutput;
    }

    public async Task<PalletPackageOutput> LastPalletSubmitAsync(MesSnInputDto input)
    {
        var palletPackageOutput = new PalletPackageOutput();        
        if (string.IsNullOrEmpty(input.S_SN))
            return palletPackageOutput.SetErrorCode(msgSys.MSG_Sys_20007);

        var setPoConfirmed = await SetConfirmPOAsync(input);
        var palletData = await Public_Repository.GetPalletDataAsync(input.S_SN);
        palletPackageOutput.PalletConfirmeds = palletData;
        if (palletData is null or { Count : <= 0})
            return palletPackageOutput.SetErrorCode(msgSys.MSG_Sys_20119);

        var updateOutput = await DataCommit_Repository.uspPalletPackaging(input.S_PartID, input.S_POID, null, input.S_SN, List_Login, setPoConfirmed.CurrentInitPageInfo.poAttributes.PalletQty.ToInt(), palletData.Count);

        if (updateOutput.Item1 != "1")
            return palletPackageOutput.SetErrorCode(msgSys.GetLanguage(updateOutput.Item1));

        palletData = await Public_Repository.GetPalletDataAsync(input.S_SN);
        palletPackageOutput.PalletConfirmeds = palletData;

        if (setPoConfirmed.CurrentInitPageInfo.poAttributes.IsGeneratePalletSN == "1")
        {

        }
        return palletPackageOutput;
    }
    public async Task<PalletPackageOutput> RemoveSingleAsync(PalletPackageInput input)
    {
        PalletPackageOutput mesOutputDtos = new PalletPackageOutput();
        if (string.IsNullOrEmpty(input.S_BoxSN))
            return mesOutputDtos.SetErrorCode(P_MSG_Public.MSG_Public_016);

        var boxInfo = await Public_Repository.GetMesPackageBySNAsync(input.S_BoxSN);
        if (boxInfo is null or { ParentID : null or <= 0 })
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20190);

        var palletInfo = await Public_Repository.GetMesPackageByIDAsync(boxInfo.ParentID.ToString());
        if (palletInfo is null or { StatusID: 1 })
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20252);

        if (string.IsNullOrEmpty(xmlStation))
            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
        if (string.IsNullOrEmpty(xmlExtraData))
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";

        xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");

        var removeRes = await Public_Repository.uspPackageRemoveSingleAsync(input.S_BoxSN, xmlProdOrder, xmlStation, xmlExtraData, "2");
        if (removeRes.strOutput != "1")
            return mesOutputDtos.SetErrorCode(msgSys.GetLanguage(removeRes.strOutput));

        mesOutputDtos.PalletConfirmeds = await Public_Repository.GetPalletDataAsync(palletInfo.SerialNumber);
        return mesOutputDtos;
    }

    public async Task<PalletPackageOutput> ReprintBoxSnAsync(MesSnInputDto input)
    {
        PalletPackageOutput mesOutputDtos = new PalletPackageOutput();
        if (string.IsNullOrEmpty(input.S_SN))
            return mesOutputDtos.SetErrorCode(P_MSG_Public.MSG_Public_016);

        var tmpPackage  = await Public_Repository.GetMesPackageBySNAsync(input.S_SN);
        if (tmpPackage is null)
            return mesOutputDtos.SetErrorCode(msgSys.MSG_Sys_20190);
        mesPackage mp = tmpPackage is { ParentID: > 0, Stage: 1 }
            ? await Public_Repository.GetMesPackageByIDAsync(tmpPackage.ParentID.ToString())
            : tmpPackage;
        if (mp is not { StatusID: 1, CurrentCount: > 0, Stage : 2, ParentID : null or <= 0 })
            return mesOutputDtos.SetErrorCode(P_MSG_Public.MSG_Public_6046);
        await DapperConn.ExecuteAsync(Public_Repository.InsertMesPackageHistory(mp.ID, 6, List_Login), null, null, I_DBTimeout, null);
        return mesOutputDtos;
    }
}