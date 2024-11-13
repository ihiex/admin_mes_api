using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Core.PublicFun.Model;
using NPOI.POIFS.Properties;
using SunnyMES.Security.IRepositories;
using SunnyMES.Commons.Json;
using log4net.Core;
using SunnyMES.Commons.Core.Dtos;
using NPOI.SS.Formula.Functions;

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    /// <summary>
    ///  AssemblyTwoInput 可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class AssemblyTwoInputController : AreaApiControllerCommon<IAssemblyTwoInputServices, string>
    {
        //private readonly IAssemblyTwoInputServices _iService;
        private string _sn = string.Empty;

        /// <summary>
        /// 组装控制器
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="publicService"></param>
        public AssemblyTwoInputController(IAssemblyTwoInputServices _iService) : base(_iService)
        {
            iService = _iService;
        }
        

        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("GetPageInitializeAsync")]
        [YuebonAuthorize("GetPageInitializeAsync")]
        [CommonAuthorize]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPageInitializeAsync(string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfoAsync(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                if (List_ConfInfo is null)
                {
                    //S_Sql = $"select '{mInitPageInfo.stationAttribute.IsCheckPN}' IsCheckPN,'{mInitPageInfo.stationAttribute.IsCheckPO}' IsCheckPO,'{mInitPageInfo.IsLegalPage}' IsLegalPage, '{mInitPageInfo.stationAttribute.IsCheckVendor}' IsCheckVendor,'{mInitPageInfo.stationAttribute.COF}' COF,'{mInitPageInfo.stationAttribute.ApplicationType}' ApplicationType,'{oPoAttributes.DOE_Parameter1}' DOE_Parameter1,'{mInitPageInfo.stationAttribute.SNScanType}' SNScanType, '{mInitPageInfo.stationAttribute.IsDOEPrint}' IsDOEPrint";

                    var tmpNoStationRes = new[]
                    {
                        new {
                            IsCheckPN = "1",
                            IsCheckPO = "1",
                            IsLegalPage = "0",
                            IsCheckVendor = "0",
                            COF = "0",
                            ApplicationType = "",
                            DOE_Parameter1 = "",
                            SNScanType = "",
                            IsDOEPrint = "0"
                        }
                    };
                    v_ComResult = Com_Result(v_ComResult, tmpNoStationRes);
                }
                else
                {
                    var List_Dyn = await iService.GetPageInitializeAsync(S_URL);
                    v_ComResult = Com_Result(v_ComResult, List_Dyn);
                }

            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPageInitializeAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 料号或工单确认
        /// </summary>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号组</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_POID">工单</param>
        /// <param name="S_UnitStatus">单元状态</param>
        /// <param name="S_URL">URL</param>
        /// <returns></returns>
        [HttpGet("SetConfirmPOAsync")]
        [YuebonAuthorize("SetConfirmPOAsync")]
        [CommonAuthorize]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> SetConfirmPOAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        {

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfoAsync(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                List<dynamic> List_Dyn = await iService.SetConfirmPOAsync(S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_UnitStatus, S_URL);

                v_ComResult = await FormatResultAsync(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetConfirmPOAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 主条码校验
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="S_URL"></param>
        /// <param name="MainSN"></param>
        /// <returns></returns>
        [HttpGet("MainSnVerifyAsync")]
        [CommonAuthorizeAttribute]
        [YuebonAuthorize("MainSnVerify")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MainSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                _sn = MainSN;
                await iService.GetConfInfoAsync(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                await iService.GetPageInitializeAsync(S_URL);
                List<dynamic> List_Dyn = await iService.MainSnVerifyAsync(S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_UnitStatus, S_URL, MainSN);
                v_ComResult = await FormatResultAsync(v_ComResult, List_Dyn,_sn);

            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MainSnVerifyAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 子条码校验
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_UnitStatus"></param>
        /// <param name="S_URL"></param>
        /// <param name="MainSN"></param>
        /// <param name="ChildSN">多个子条码使用逗号分隔</param>
        /// <param name="sColorCode">ColorCode</param>
        /// <param name="sBuildCode">Build(0~9)</param>
        /// <param name="sSpcaCode">CCCC(4)</param>
        /// <param name="sPpCode">PP(2)</param>
        /// <returns></returns>
        [HttpGet("ChildSnVerifyAsync")]
        [CommonAuthorizeAttribute]
        [YuebonAuthorize("ChildSnVerify")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> ChildSnVerifyAsync(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_UnitStatus, string S_URL, string MainSN, string ChildSN, string sColorCode, string sBuildCode, string sSpcaCode, string sPpCode)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                _sn = MainSN + "," + ChildSN;
                await iService.GetConfInfoAsync(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                await iService.GetPageInitializeAsync(S_URL);
                List<dynamic> List_Dyn = await iService.ChildSnVerifyAsync(S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_UnitStatus, S_URL, MainSN, ChildSN,sColorCode,sBuildCode,sSpcaCode,sPpCode);
                v_ComResult = await FormatResultAsync(v_ComResult, List_Dyn,_sn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 ChildSnVerifyAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }
        /// <summary>
        /// 料号或工单确认
        /// </summary>
        /// <param name="assemblyTwoInput_SetConfirmPO_Input_Dto"></param>
        /// <returns></returns>
        [HttpPost("SetConfirmPOAsync")]
        [CommonAuthorize]
        [AllowAnonymousAttribute]
        [ApiVersion("1.0")]
        public async Task<IActionResult> SetConfirmPOAsync([FromBody] AssemblyTwoInput_SetConfirmPO_Input_Dto assemblyTwoInput_SetConfirmPO_Input_Dto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfoAsync(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                List<dynamic> List_Dyn = await iService.SetConfirmPOAsync(assemblyTwoInput_SetConfirmPO_Input_Dto.S_PartFamilyTypeID.ToString(), assemblyTwoInput_SetConfirmPO_Input_Dto.S_PartFamilyID.ToString(),
                    assemblyTwoInput_SetConfirmPO_Input_Dto.S_PartID.ToString(), assemblyTwoInput_SetConfirmPO_Input_Dto.S_POID.ToString(), assemblyTwoInput_SetConfirmPO_Input_Dto.S_UnitStatus.ToString(), assemblyTwoInput_SetConfirmPO_Input_Dto.S_URL);

                v_ComResult = await FormatResultAsync(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetConfirmPOAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }
    }
}
