using System;
using System.Threading.Tasks;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output;
using SunnyMES.Security.Dtos.MES.MES_Output.OOBA;
using SunnyMES.Security.IRepositories.MES.Package;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security.Repositories.MES.Package
{
    /// <summary>
    /// 
    /// </summary>
    public class OOBARepository : MesBaseRepository, IOOBARepository
    {
        public OOBARepository(IDbContextCore contextCore) : base(contextCore)
        {

        }

        public async Task<OOBACheckOutputDtos> BoxSnCheckAsync(MesSnInputDto input)
        {
            OOBACheckOutputDtos outputDtos = new OOBACheckOutputDtos {
                BoxInformation = new OOBACheckInfo(),
                PrinterParams = new PrinterParams(),
            };

            if (input is null or {  S_SN.Length :<= 0})
                return outputDtos.SetErrorCode(msgSys.MSG_Sys_20007);

            var pageInit = await base.GetPageInitializeAsync(input.S_URL);
            outputDtos.CurrentInitPageInfo = pageInit.CurrentInitPageInfo;
            outputDtos.CurrentSettingInfo = pageInit.CurrentSettingInfo;




            var oobaPara = await Public_Repository.uspPackageCheckOOBAAsync(input.S_SN,List_Login.StationID);

            if (oobaPara.strOutput != "1")
                return outputDtos.SetErrorCode(msgSys.GetLanguage(oobaPara.strOutput));


            outputDtos.BoxInformation = oobaPara.BoxInformation;
            outputDtos.PrintSnList = oobaPara.PrintSnList;


            if (pageInit.CurrentInitPageInfo.stationAttribute.IsNotPrint != "1")
            {
                //ID 不传时，设定为0
                string S_LabelPath = await Public_Repository.GetLabelName(List_Login.StationTypeID.ToString(), "0", outputDtos.BoxInformation.CurrPartID.ToString(), outputDtos.BoxInformation.CurrProductionOrderID.ToString(), List_Login.LineID.ToString());
                if (string.IsNullOrEmpty(S_LabelPath))
                    return outputDtos.SetErrorCode(msgSys.GetLanguage("20076"));
                else
                {
                    if (S_LabelPath.StartsWith("error", StringComparison.OrdinalIgnoreCase))
                        return outputDtos.SetErrorCode(S_LabelPath);
                }
                string pathList = string.Empty;
                string[] ListTemplate = S_LabelPath.Split(';');
                foreach (string str in ListTemplate)
                {
                    string[] listStr = str.Split(',');
                    pathList = (string.IsNullOrEmpty(pathList) ? "" : pathList + ";") + listStr[1].ToString();
                }
                outputDtos.PrinterParams.LabelPath = pathList.Replace(@"\\", @"\");
                outputDtos.PrinterParams.LabelCommand = S_LabelPath;
            }

            return outputDtos;
        }

        public async Task<OOBAReworkOutputDtos> BoxSnReworkAsync(MesSnInputDto input)
        {
            OOBAReworkOutputDtos outputDtos = new OOBAReworkOutputDtos
            {

            };

            if (input is null or { S_SN.Length: <= 0 })
                return outputDtos.SetErrorCode(msgSys.MSG_Sys_20007);

            var pageInit = await base.GetPageInitializeAsync(input.S_URL);
            outputDtos.CurrentInitPageInfo = pageInit.CurrentInitPageInfo;
            outputDtos.CurrentSettingInfo = pageInit.CurrentSettingInfo;

            var oobaPara = await Public_Repository.uspPackageReworkOOBAAsync(input.S_SN,List_Login.StationID,List_Login.EmployeeID);

            if (oobaPara.strOutput != "1")
                return outputDtos.SetErrorCode(msgSys.GetLanguage(oobaPara.strOutput));

            return outputDtos;
        }
    }
}
