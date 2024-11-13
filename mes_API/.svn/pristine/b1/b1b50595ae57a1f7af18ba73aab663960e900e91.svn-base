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
using NPOI.OpenXmlFormats.Wordprocessing;

namespace SunnyMES.WebApi.Areas.MES
{
    /// <summary>
    ///  MaterialLineCropping_SN 物料切割
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class MaterialLineCropping_SNController : AreaApiControllerReport<IMaterialLineCropping_SNServices, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public MaterialLineCropping_SNController(IMaterialLineCropping_SNServices _iService) : base(_iService)
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
        /// <param name="S_LineNumber">线号</param>
        /// <param name="Type">SmallBatch（大批次切小批次）BatchToSN(批次切SN)  BatchToSNDOE(批次切SN DOE) BatchToSN_NotLineNumber(批次切SN 无线号) </param>
        /// <param name="S_URL">URL</param>
        /// <returns></returns>
        [HttpGet("SetConfirmPO")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
            string S_PartID, string S_POID, string S_LineNumber,string Type, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                ConfirmPOOutputMateialDto List_Dyn = await iService.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
                    S_PartID, S_POID, S_LineNumber, Type,S_URL);

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
        /// 物料切割
        /// </summary>
        /// <param name="PartFamilyTypeID">料号组类别</param>
        /// <param name="PartFamilyID">料号组</param>
        /// <param name="PartID">料号</param>
        /// <param name="POID">工单</param>
        /// <param name="LineNumber">线号</param>
        /// <param name="URL">URL</param>
        /// <param name="Type">SmallBatch（大批次切小批次）BatchToSN(批次切SN)  BatchToSNDOE(批次切SN DOE) BatchToSN_NotLineNumber(批次切SN 无线号)</param>
        /// <param name="SN_Pattern">SN 正则</param>
        /// <param name="Batch_Pattern">批次 正则</param>
        /// <param name="BatchNo">批次号</param>
        /// <param name="RollNO">卷号</param>
        /// <param name="SpecQTY">规格数量</param>
        /// <param name="ParentID"></param>
        /// <param name="BatchQTY"></param>
        /// <param name="UsageQTY"></param>
        /// <param name="DOE_BuildNameEnabled">BuildName Enabled </param>
        /// <param name="DOE_BuildName">BuildName</param>
        /// <param name="DOE_CCCCEnabled">CCCC Enabled</param>
        /// <param name="DOE_CCCC_Length">CCCC 设置长度</param>
        /// <param name="CCCC">CCCC</param>
        /// <returns></returns>
        [HttpGet("SetSplit")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetSplit
            (
                string PartFamilyTypeID, string PartFamilyID,
                string PartID, string POID, string LineNumber, string URL,

                string Type,  string SN_Pattern, string Batch_Pattern, string BatchNo, string RollNO, int SpecQTY,

                int ParentID, int BatchQTY, int UsageQTY,
                Boolean DOE_BuildNameEnabled, string DOE_BuildName, Boolean DOE_CCCCEnabled, int DOE_CCCC_Length, string CCCC
            )
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                CreateSNOutputDto List_Dyn = await iService.SetSplit
                    (
                         PartFamilyTypeID,  PartFamilyID,
                         PartID,  POID,  LineNumber,  URL,

                         Type, SN_Pattern, Batch_Pattern, BatchNo,  RollNO,  SpecQTY,

                         ParentID,  BatchQTY,  UsageQTY,
                         DOE_BuildNameEnabled, DOE_BuildName, DOE_CCCCEnabled, DOE_CCCC_Length, CCCC
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
                Log4NetHelper.Error("获取 SetSplit 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 批次框事件
        /// </summary>
        /// <param name="S_PartID"></param>
        /// <param name="ParentID"></param>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("ParentBatchNo_ValueChanged")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> ParentBatchNo_ValueChanged(string S_PartID, string ParentID, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                CroppingSNDto List_Dyn =  iService.ParentBatchNo_ValueChanged(S_PartID, ParentID, S_URL);

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
                Log4NetHelper.Error("获取 ParentBatchNo_ValueChanged 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


    }
}