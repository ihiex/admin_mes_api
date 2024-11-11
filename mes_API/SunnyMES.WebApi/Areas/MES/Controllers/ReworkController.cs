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

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    /// <summary>
    ///  组装解绑
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class ReworkController : AreaApiControllerReport<IReworkServices, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public ReworkController(IReworkServices _iService) : base(_iService)
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


        ///// <summary>
        ///// 料号或工单确认
        ///// </summary>
        ///// <param name="S_PartFamilyTypeID">料号组类别</param>
        ///// <param name="S_PartFamilyID">料号组</param>
        ///// <param name="S_PartID">料号</param>
        ///// <param name="S_POID">工单</param>
        ///// <param name="S_UnitStatus">单元状态</param>
        ///// <param name="S_URL">URL</param>
        ///// <returns></returns>
        //[HttpGet("SetConfirmPO")]
        //[YuebonAuthorize("")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SetConfirmPO(string S_PartFamilyTypeID, string S_PartFamilyID,
        //    string S_PartID, string S_POID, string S_UnitStatus, string S_URL)
        //{
        //    CommonResult v_ComResult = new CommonResult();
        //    try
        //    {
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        ConfirmPOOutputDto List_Dyn = await iService.SetConfirmPO(S_PartFamilyTypeID, S_PartFamilyID,
        //            S_PartID, S_POID, S_UnitStatus, S_URL);

        //        if (List_Dyn.MSG.ValStr1 == "1")
        //        {
        //            v_ComResult.Success = true;
        //            v_ComResult.ResultCode = ErrCode.successCode;
        //            v_ComResult.ResultMsg = ErrCode.err0;
        //            v_ComResult.ResData = List_Dyn;
        //            v_ComResult.Sounds = S_Path_OK;
        //        }
        //        else
        //        {
        //            v_ComResult.Success = false;
        //            v_ComResult.ResultCode = List_Dyn.MSG.ValStr1;
        //            v_ComResult.ResultMsg = List_Dyn.MSG.ValStr2;
        //            v_ComResult.ResData = null;
        //            v_ComResult.Sounds = List_Dyn.MSG.ValStr4;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 SetConfirmPO 异常", ex);
        //        v_ComResult.ResultMsg = ErrCode.err40110;
        //        v_ComResult.ResultCode = "40110";
        //    }
        //    return ToJsonContent(v_ComResult);
        //}

        /// <summary>
        /// 获取反工数据
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_Type">1:SN 2:UPC 3:Multipack 4:Pallet</param> 
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("SetScanSN_GetRework")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetScanSN_GetRework(string S_SN, string S_Type, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                GetReworkDto List_Dyn = await iService.SetScanSN_GetRework(S_SN,S_Type, S_URL);

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
        /// 反工
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_Type">1:SN 2:UPC 3:Multipack 4:Pallet</param> 
        /// <param name="S_URL"></param>        
        /// <returns></returns>
        [HttpGet("SetRework")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetRework(string S_SN, string S_Type, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                string S_Result = await iService.SetRework(S_SN, S_Type);

                if (S_Result == "1")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = S_Result;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else 
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "40110";
                    v_ComResult.ResultMsg = S_Result;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetRework 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 反工打印
        /// </summary>
        /// <param name="S_SN"></param>
        /// <param name="S_ListSN">SN1;SN2;SN3 ....</param>
        /// <param name="S_Type">1:SN 2:UPC 3:Multipack 4:Pallet</param>
        /// <param name="S_URL"></param>
        /// <returns></returns>
        [HttpGet("ReworkPrint")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> ReworkPrint(string S_SN, string S_ListSN, string S_Type, string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                ReworkPrintDto List_Dyn = await iService.ReworkPrint(S_SN,S_ListSN, S_Type, S_URL);

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

    }
}