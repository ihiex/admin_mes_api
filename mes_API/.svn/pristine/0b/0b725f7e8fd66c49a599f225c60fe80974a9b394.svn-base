using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using NPOI.POIFS.Properties;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Enums.DynamicItemName;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Commons.Log;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security.Dtos.MES.MES_Output.OOBA;
using SunnyMES.Security.Dtos.MES.MES_Output.RMAChange;
using SunnyMES.Security.IRepositories.MES.Package;
using SunnyMES.Security.Models;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories.MES.Package
{
    /// <summary>
    /// 
    /// </summary>
    public class RMAChangeRepository : MesBaseRepository, IRMAChangeRepository
    {
        public RMAChangeRepository(IDbContextCore contextCore) : base(contextCore)
        {

        }

        public new async Task<SetConfirmPoOutput> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_POID,
    string S_UnitStatus, string S_URL)
        {
            var mConfirmPoOutput = await base.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_POID, S_UnitStatus, S_URL);

            if (!string.IsNullOrEmpty(mConfirmPoOutput.ErrorMsg))
                return mConfirmPoOutput;

            if (string.IsNullOrEmpty(mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.JumpFromUnitStateID))
                return mConfirmPoOutput.SetErrorCode(msgSys.MSG_Sys_20199, "JumpFromUnitStateID");

            if (string.IsNullOrEmpty(mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.JumpToUnitStateID))
                return mConfirmPoOutput.SetErrorCode(msgSys.MSG_Sys_20199, "JumpToUnitStateID");

            if (string.IsNullOrEmpty(mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.JumpStatusID))
                return mConfirmPoOutput.SetErrorCode(msgSys.MSG_Sys_20199, "JumpStatusID");

            if (string.IsNullOrEmpty(mConfirmPoOutput.CurrentInitPageInfo.stationAttribute.JumpUnitStateID))
                return mConfirmPoOutput.SetErrorCode(msgSys.MSG_Sys_20199, "JumpUnitStateID");

            return mSetConfirmPoOutput = mConfirmPoOutput;
        }


        public async Task<RMAChangeOutputDto> SnCheckAsync(MesSnInputDto input)
        {
            var poConfim = await SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID,
            input.S_POID, input.S_UnitStatus, input.S_URL);

            var output = new RMAChangeOutputDto()
            {
                 ErrorMsg = poConfim.ErrorMsg,
                 CurrentInitPageInfo = poConfim.CurrentInitPageInfo,
                 CurrentSettingInfo = poConfim.CurrentSettingInfo                 
            };

            if (output.ErrorMsg != null && output.ErrorMsg.ToString() != "")
                return output;

            if (input == null || string.IsNullOrEmpty(input.S_SN))
                return output.SetErrorCode(msgSys.MSG_Sys_20007);

            var snUnit = await Public_Repository.GetMesUnitSerialAsync(input.S_SN);
            if (snUnit == null || snUnit.UnitID <= 0)
                return output.SetErrorCode(msgSys.MSG_Sys_20242);

            if (snUnit.SerialNumberTypeID != 6 && snUnit.SerialNumberTypeID != 5)
                return output.SetErrorCode(msgSys.MSG_Sys_60202);

            string tmpFGSN = string.Empty;
            if (snUnit.SerialNumberTypeID == 6)
            {
                tmpFGSN = await Public_Repository.MesGetFGSNByUPCSNAsync(input.S_SN);                
                snUnit = await Public_Repository.GetMesUnitSerialAsync(tmpFGSN);
                if (snUnit == null || snUnit.UnitID <= 0)
                    return output.SetErrorCode(msgSys.MSG_Sys_20242);
            }
            else
            {
                tmpFGSN = input.S_SN;
            }


            if (tmpFGSN.Contains(","))
            {
                Log4NetHelper.Error($"format error ----  input SN : {input.S_SN}, get FG SN : {tmpFGSN}....");
                return output.SetErrorCode("sn format error...");
            }

            var fromUnitStateIds = output.CurrentInitPageInfo.stationAttribute.JumpFromUnitStateID.Split(',');
            if (!fromUnitStateIds.Contains(snUnit.UnitStateID.ToString()))
                return output.SetErrorCode(msgSys.MSG_Sys_20201);

            if (output.CurrentInitPageInfo.stationAttribute.JumpStatusID != snUnit.StatusID.ToString())
                return output.SetErrorCode(msgSys.MSG_Sys_20045);

            if (output.CurrentInitPageInfo.stationAttribute.JumpUnitStateID != input.S_UnitStatus)
                return output.SetErrorCode(msgSys.MSG_Sys_60600);

            string xmlExtraData = "UnitID=" + snUnit.UnitID + "&UnitStateID=" + output.CurrentInitPageInfo.stationAttribute.JumpToUnitStateID
          + "&EmployeeID=" + List_Login.EmployeeID + "&StatusID=" + output.CurrentInitPageInfo.stationAttribute.JumpUnitStateID;

            string xmlUnitDefect = "";
            if (input.S_UnitStatus != "1")
            {
                string[] Array_Defect = input.S_DefectID.Split(',');
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
                        Log4NetHelper.Error("defect code error", ex);
                        return output.SetErrorCode(ex.Message);
                    }
                }
                xmlExtraData = xmlExtraData + xmlUnitDefect;
            }

            string xmlProdOrder = "<ProdOrder ProdOrderID=\"" + Convert.ToInt32(input.S_POID) + "\"> </ProdOrder>";
            string xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";
            string xmlStation = "<Station StationID=\"" + List_Login.StationID + "\"> </Station>";
            string outString =   await Public_Repository.uspCallProcedureAsync("uspRMAChange", tmpFGSN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData,  null);

            if (outString != "1")
                return output.SetErrorCode(msgSys.GetLanguage(outString));


            return output;
        }
    }
}
