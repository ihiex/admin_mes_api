using System.Threading.Tasks;
using SunnyMES.Commons.Core.Dtos.MesInputDtos.BoxScalage;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IDbContext;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.MES_Output.BoxScalage;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._3_IRepositories.MES;
using SunnyMES.Security.Repositories;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.Security._4_Repositories.MES
{
    /// <summary>
    /// 中箱称重
    /// </summary>
    public class BoxScalagePackageRepository : MesBaseRepository, IBoxScalagePackageRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contextCore"></param>
        public BoxScalagePackageRepository(IDbContextCore contextCore) : base(contextCore)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SetConfirmPoOutput> SetConfirmPOAsync(MesInputDto input)
        {
            var setConfirmPO = await base.SetConfirmPOAsync(input.S_PartFamilyTypeID, input.S_PartFamilyID, input.S_PartID, input.S_POID, input.S_UnitStatus, input.S_URL);
            if (!string.IsNullOrEmpty(setConfirmPO.ErrorMsg))
                return setConfirmPO;

            var scalageConfig = await Public_Repository.GetMesScalagePartAndPartFamilyDetail(input.S_PartID.ToInt(), "PackBoxWeightLimit");
            if (scalageConfig.Item1 != "1")
                return setConfirmPO.SetErrorCode(msgSys.GetLanguage(scalageConfig.Item1));

            setConfirmPO.ScalageConfig = scalageConfig.Item2;

            return setConfirmPO;
        }
        public async Task<BoxScalageOutput> MainSnVerifyAsync(MesSnInputDto input)
        {
            var boxScalageOutput = new BoxScalageOutput();
            
            if (string.IsNullOrEmpty(input.S_SN))
                return boxScalageOutput.SetErrorCode(msgSys.MSG_Sys_20007);

            xmlStation = "<Station StationId=\"" + baseCommonHeader.StationId + "\"> </Station>";
            xmlExtraData = "<ExtraData EmployeeId=\"" + baseCommonHeader.EmployeeId.ToString() + "\"> </ExtraData>";
            xmlProdOrder = ("<ProdOrder ProdOrderID=\"" + input.S_POID + "\"> </ProdOrder>");
            xmlPart = "<Part PartID=\"" + input.S_PartID + "\"> </Part>";

            var output = await Public_Repository.uspPalletCheckAsync(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "BOX");
            if (output != "1")
                return boxScalageOutput.SetErrorCode(msgSys.GetLanguage(output));

            string strOutputCheck = string.Empty;
            strOutputCheck = await Public_Repository.uspPackageRouteCheck(input.S_SN, xmlProdOrder, xmlPart, xmlStation, xmlExtraData, "");

            if (strOutputCheck != "1")
                return boxScalageOutput.SetErrorCode(msgSys.GetLanguage(strOutputCheck));

            return boxScalageOutput;
        }
        public async Task<BoxScalageOutput> FinalWeightSubmitAsync(BoxScalageInput input)
        {
            var boxScalageOutput = new BoxScalageOutput();
            var setPoConfirm = await this.SetConfirmPOAsync(input);
            double upLimit = setPoConfirm.ScalageConfig.BASE + setPoConfirm.ScalageConfig.UL;
            double lowLimit = setPoConfirm.ScalageConfig.BASE + setPoConfirm.ScalageConfig.LL;
            if (input.BoxWeight < lowLimit || input.BoxWeight > upLimit)
                return boxScalageOutput.SetErrorCode(P_MSG_Public.MSG_Public_6049);

            var output = await DataCommit_Repository.uspUpdateBoxWeight(input.S_PartID, input.S_POID, input.S_SN, List_Login, input.BoxWeight);
            if (output != "1")
                return boxScalageOutput.SetErrorCode(msgSys.GetLanguage(output));

            return boxScalageOutput;
        }
    }
}
