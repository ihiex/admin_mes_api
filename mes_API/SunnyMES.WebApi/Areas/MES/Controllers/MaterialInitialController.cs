using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Json;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Security.Repositories;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SunnyMES.Security._2_Dtos.MES;
using System.Net.WebSockets;
using SunnyMES.Security.Services;
using NPOI.SS.Util;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Senparc.NeuChar.NeuralSystems;
using System.Security.Cryptography;

namespace SunnyMES.WebApi.Areas.MES
{
    /// <summary>
    ///  MaterialInitial 物料初始化
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class MaterialInitialController : AreaApiControllerReport<IMaterialInitialServices, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public MaterialInitialController(IMaterialInitialServices _iService) : base(_iService)
        {
            iService = _iService;
        }


        private CommonResult Com_Result(CommonResult v_CommonResult, List<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();
            List<dynamic> List_Result = new List<dynamic>();

            v_CommonResult.Sounds = S_Path_OK;

            if (List_Dyn.Count > 0)
            {
                try
                {
                    if (List_Dyn[0][0] is TabVal)
                    {
                        for (int i = 1; i < List_Dyn.Count; i++)
                        {
                            List_Result.Add(List_Dyn[i]);
                        }

                        TabVal v_TabVal = List_Dyn[0][0] as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = List_Result;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                            v_CommonResult.Sounds = v_TabVal.ValStr4;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                    else
                    {
                        List_Result = List_Dyn;
                    }
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Result;
                    v_CommonResult.Sounds = S_Path_NG;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Result;

            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }

        private CommonResult Com_Result(CommonResult v_CommonResult, IEnumerable<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();
            IEnumerable<dynamic> List_Result = List_Dyn;
            v_CommonResult.Sounds = S_Path_OK;

            foreach (var item in List_Dyn)
            {
                try
                {
                    if (item is TabVal)
                    {
                        TabVal v_TabVal = item as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = List_Result;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                            v_CommonResult.Sounds = v_TabVal.ValStr4;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                    //else
                    //{
                    //    List_Result = List_Dyn;
                    //}
                    continue;
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Result;
                    v_CommonResult.Sounds = S_Path_NG;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Result;

            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }


        /// <summary>
        /// 页面初始化
        /// </summary>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("GetPageInitialize")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPageInitialize(string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                ;
                IEnumerable<dynamic> List_Dyn = await iService.GetPageInitialize(S_URL);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPageInitialize 异常", ex);
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
        /// <param name="S_URL">URL</param>
        /// <returns></returns>
        [HttpGet("SetConfirmPO")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                ConfirmPOOutputMateialDto List_Dyn = await iService.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_URL);

                if (List_Dyn.MSG.ValStr1 == "1")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = List_Dyn;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = List_Dyn.MSG.ValStr1;
                    v_ComResult.ResultMsg = List_Dyn.MSG.ValStr2;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = List_Dyn.MSG.ValStr4;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetConfirmPO 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// SetRegister  物料注册
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="S_URL"></param>
        /// <param name="Batch_Pattern">批次正则</param>
        /// <param name="MaterialBatchQTY">批次配置数量</param>
        /// <param name="MaterialLable">是否打印</param>
        /// <param name="M_UnitConversion_PCS">收料单位转换</param>
        /// <param name="MaterialAuto">是否自动识别客户条码</param>
        /// <param name="MaterialCodeRules">编码规则</param>
        /// <param name="B_LotCode">布尔型  LotCode文本框 是否只读</param>
        /// <param name="B_TranceCode">布尔型 TranceCode文本框 是否只读</param>
        /// <param name="VendorID">供应商ID</param>
        /// <param name="VendorCode">供应商代码</param>
        /// <param name="LotCode">批次</param>
        /// <param name="RollCode">卷号</param>
        /// <param name="Unit">单位 pcs,m </param>
        /// <param name="Quantity">批次数量</param>
        /// <param name="MaterialDate">原材料日期</param>
        /// <param name="MPN">MPN</param>
        /// <param name="DateCode">生产日期</param>
        /// <param name="ExpiringTime">过期日期</param>
        /// <param name="TranceCode">追溯码</param>
        /// <param name="Type">0: Public 1:指定项目 2:排除项目</param>
        /// <param name="Assigned">1:指定项目 2:排除项目   PartID</param>
        /// <returns></returns>
        [HttpGet("SetRegister")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetRegister
            (
            string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_URL,

            string Batch_Pattern, string MaterialBatchQTY, string MaterialLable, string M_UnitConversion_PCS,
            string MaterialAuto, string MaterialCodeRules, bool B_LotCode, bool B_TranceCode,

            string VendorID, string VendorCode,
            string LotCode, string RollCode, string Unit, string Quantity, string MaterialDate,
            string MPN, string DateCode, string ExpiringTime, string TranceCode, string Type, string Assigned
            )
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                CreateSNOutputDto List_Dyn = await iService.SetRegister
                    (
                     S_PartFamilyTypeID, S_PartFamilyID,
                     S_PartID, S_POID, S_URL,

                     Batch_Pattern, MaterialBatchQTY, MaterialLable, M_UnitConversion_PCS,
                     MaterialAuto, MaterialCodeRules, B_LotCode, B_TranceCode,

                     VendorID, VendorCode,
                     LotCode, RollCode, Unit, Quantity, MaterialDate,
                     MPN, DateCode, ExpiringTime, TranceCode, Type, Assigned
                    );

                if (List_Dyn.MSG.ValStr1 == "1")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = List_Dyn;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = List_Dyn.MSG.ValStr1;
                    v_ComResult.ResultMsg = List_Dyn.MSG.ValStr2;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = List_Dyn.MSG.ValStr4;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetRegister 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// RePrint  重打印
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <param name="S_PartFamilyID"></param>
        /// <param name="S_PartID"></param>
        /// <param name="S_POID"></param>
        /// <param name="LotCode"></param>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("RePrint")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> RePrint(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string LotCode, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                CreateSNOutputDto List_Dyn = await iService.RePrint
                    (
                     S_PartFamilyTypeID, S_PartFamilyID,
                                 S_PartID, S_POID, LotCode, S_URL
                    );

                if (List_Dyn.MSG.ValStr1 == "1")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = List_Dyn;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = List_Dyn.MSG.ValStr1;
                    v_ComResult.ResultMsg = List_Dyn.MSG.ValStr2;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = List_Dyn.MSG.ValStr4;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 RePrint 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 追溯码回车事件
        /// </summary>
        /// <param name="TranceCode">追溯码</param>
        /// <param name="MaterialCodeRules">原材料编码规则</param>        
        /// <param name="Expires_Time">配置的过期时间</param>
        /// <returns></returns>
        [HttpGet("TranceCode_KeyDown")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public IActionResult TranceCode_KeyDown(string TranceCode, string MaterialCodeRules, int Expires_Time)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {


                TranceCodeDto List_Dyn = iService.TranceCode_KeyDown
                    (
                        TranceCode, MaterialCodeRules, Expires_Time
                    );

                if (List_Dyn.MSG.ValStr1 == "1")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = List_Dyn;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = List_Dyn.MSG.ValStr1;
                    v_ComResult.ResultMsg = List_Dyn.MSG.ValStr2;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = List_Dyn.MSG.ValStr4;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 TranceCode_KeyDown 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


    }
}