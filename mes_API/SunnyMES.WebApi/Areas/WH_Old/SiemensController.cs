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


namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  Siemens 可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class SiemensController : AreaApiControllerReport<ISiemensService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public SiemensController(ISiemensService _iService) : base(_iService)
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
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPageInitialize(string S_URL)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

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
        /// WHIn
        /// </summary>
        /// <param name="S_MPN">目前传空值</param>
        /// <param name="S_BoxSN"></param>
        /// <param name="S_Type">0：入库  1：反入库 </param>        
        /// <returns></returns>
        [HttpGet("WHIn")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> WHIn(string S_MPN, string S_BoxSN, string S_Type)
        {           
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                
                WH_InDTDto list = iService.WHIn( S_MPN,  S_BoxSN,  S_Type);
                if (list.ScanResult == "OK")
                {
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                    result.ResData = list;
                    result.Sounds = S_Path_OK;
                }
                else 
                {
                    if (list.ScanResult == "60000" || list.ScanResult == "60002")
                    {
                        result.Sounds = S_Path_RE;
                    }
                    else 
                    {
                        result.Sounds = S_Path_NG;
                    }

                    result.Success = false;
                    result.ResultCode = list.ScanResult;
                    result.ResultMsg = list.MSG;
                    result.ResData = list;
                }


            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 WHIn 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        ///// <summary>
        ///// WHIn_DT
        ///// </summary>        
        ///// <param name="S_BoxSN"></param>
        ///// <returns></returns>
        //[HttpGet("WHIn_DT")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult> WHIn_DT(string S_BoxSN)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {

        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        WH_InDTDto list = iService.WHIn_DT(S_BoxSN);
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 WHIn_DT 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //    return ToJsonContent(result);
        //}

        /// <summary>
        /// WHOut
        /// </summary>
        /// <param name="S_MPN">目前传空值</param>
        /// <param name="S_BillNo"></param>
        /// <param name="S_BoxSN"></param>
        /// <param name="S_Type">0：出库  1：反出库</param>        
        /// <returns></returns>
        [HttpGet("WHOut")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> WHOut(string S_MPN, string S_BillNo, string S_BoxSN, string S_Type) 
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                WH_OutDTDto list = iService.WHOut( S_MPN,  S_BillNo,  S_BoxSN,  S_Type);
                if (list.ScanResult == "OK")
                {
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                    result.ResData = list;
                    result.Sounds = S_Path_OK;
                }
                else
                {
                    if (list.ScanResult == "60000" || list.ScanResult == "60002")
                    {
                        result.Sounds = S_Path_RE;
                    }
                    else
                    {
                        result.Sounds = S_Path_NG;
                    }

                    result.Success = false;
                    result.ResultCode = list.ScanResult;
                    result.ResultMsg = list.MSG;
                    result.ResData = list;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 WHIn_DT 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        ///// <summary>
        ///// WHOut_DT
        ///// </summary>
        ///// <param name="S_BoxSN"></param>
        ///// <returns></returns>
        //[HttpGet("WHOut_DT")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult>  WHOut_DT(string S_BoxSN)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {
                
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        WH_OutDTDto list = iService.WHOut_DT(  S_BoxSN);
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 WHOut_DT 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //   return ToJsonContent(result);
        //}

        /// <summary>
        /// CheckBillNo
        /// </summary>
        /// <param name="S_BillNo"></param>
        /// <param name="S_Result"></param>
        /// <returns></returns>
        [HttpGet("CheckBillNo")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> CheckBillNo(string S_BillNo,  string S_Result)
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                List<WH_BillNo> list = iService.CheckBillNo( S_BillNo, out S_Result);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 WHOut_DT 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }
        ///// <summary>
        ///// GetIpad_BB
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet("GetIpad_BB")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult> GetIpad_BB() 
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        List<WH_IPBB> list = iService.GetIpad_BB();
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 WHOut_DT 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //    return ToJsonContent(result); 
        //}

        /// <summary>
        /// GetShipment
        /// </summary>
        /// <param name="S_Start"></param>
        /// <param name="S_End"></param>
        /// <param name="FStatus"></param>        
        /// <returns></returns>
        [HttpGet("GetShipment")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetShipment(string S_Start, string S_End, string FStatus)
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                List<CO_WH_Shipment> list = iService.GetShipment( S_Start,  S_End,  FStatus);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipment 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetShipmentEntry
        /// </summary>
        /// <param name="S_FInterID"></param>        
        /// <returns></returns>
        [HttpGet("GetShipmentEntry")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetShipmentEntry(string S_FInterID) 
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                List<CO_WH_ShipmentEntry> list = iService.GetShipmentEntry( S_FInterID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentEntry 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetShipmentReport
        /// </summary>
        /// <param name="S_Start"></param>
        /// <param name="S_End"></param>
        /// <param name="FStatus"></param>        
        /// <returns></returns>
        [HttpGet("GetShipmentReport")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetShipmentReport(string S_Start, string S_End, string FStatus)
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                List<ShipmentReport> list = iService.GetShipmentReport( S_Start,  S_End,  FStatus);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentEntry 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// Edit_CO_WH_Shipment
        /// </summary>
        /// <param name="FInterID"></param>
        /// <param name="FDate"></param>
        /// <param name="HAWB"></param>
        /// <param name="FPalletSeq"></param>
        /// <param name="FPalletCount"></param>
        /// <param name="FGrossweight"></param>
        /// <param name="FEmptyCarton"></param>
        /// <param name="FShipNO"></param>
        /// <param name="FProjectNO"></param>
        /// <param name="S_Type">New  Mod</param>
        /// <returns></returns>
        [HttpGet("Edit_CO_WH_Shipment")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> Edit_CO_WH_Shipment
                    (
                        string FInterID,
                        string FDate,
                        string HAWB,
                        string FPalletSeq,
                        string FPalletCount,
                        string FGrossweight,
                        string FEmptyCarton,
                        string FShipNO,
                        string FProjectNO,

                        string S_Type
                    )
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.Edit_CO_WH_Shipment
                    (
                         FInterID,
                         FDate,
                         HAWB,
                         FPalletSeq,
                         FPalletCount,
                         FGrossweight,
                         FEmptyCarton,
                         FShipNO,
                         FProjectNO,

                         S_Type
                         
                    );
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 Edit_CO_WH_Shipment 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// DeleteShipment
        /// </summary>
        /// <param name="FInterID"></param>        
        /// <returns></returns>
        [HttpGet("DeleteShipment")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> DeleteShipment(string FInterID) 
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.DeleteShipment( FInterID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentEntry 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// DeleteShipmentEntry
        /// </summary>
        /// <param name="FDetailID"></param>        
        /// <returns></returns>
        [HttpGet("DeleteShipmentEntry")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> DeleteShipmentEntry(string FDetailID) 
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.DeleteShipmentEntry( FDetailID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 DeleteShipmentEntry 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// DeleteMultiSelectShipment
        /// </summary>
        /// <param name="FInterID_List"></param>        
        /// <returns></returns>
        [HttpGet("DeleteMultiSelectShipment")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> DeleteMultiSelectShipment(string FInterID_List) 
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.DeleteMultiSelectShipment( FInterID_List);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 DeleteMultiSelectShipment 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// UpdateShipment_FStatus
        /// </summary>
        /// <param name="FInterID_List"> '1,2,3'   FInterID</param>
        /// <param name="Status">Audit: 1  Shipped:3  </param>        
        /// <returns></returns>
        [HttpGet("UpdateShipment_FStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> UpdateShipment_FStatus(string FInterID_List, string Status) 
        {
            CommonResult result = new CommonResult();
            try
            {                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.UpdateShipment_FStatus( FInterID_List,  Status);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 UpdateShipment_FStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        ///// <summary>
        ///// GetShipment_One
        ///// </summary>
        ///// <param name="FInterID"></param>        
        ///// <returns></returns>
        //[HttpGet("GetShipment_One")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult>  GetShipment_One(string FInterID)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {
                
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        List<CO_WH_Shipment> list = iService.GetShipment_One( FInterID);
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 GetShipment_One 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //    return ToJsonContent(result);
        //}

        ///// <summary>
        ///// GetShipmentEntry_One
        ///// </summary>
        ///// <param name="FDetailID"></param>        
        ///// <returns></returns>
        //[HttpGet("GetShipmentEntry_One")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult>  GetShipmentEntry_One(string FDetailID)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {                
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        List<CO_WH_ShipmentEntry> list = iService.GetShipmentEntry_One( FDetailID);
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 GetShipmentEntry_One 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //    return ToJsonContent(result);
        //}

        /// <summary>
        /// Edit_CO_WH_ShipmentEntry
        /// </summary>
        /// <param name="FInterID"></param>
        /// <param name="FEntryID"></param>
        /// <param name="FDetailID"></param>
        /// <param name="FKPONO"></param>
        /// <param name="FMPNNO"></param>
        /// <param name="FCTN"></param>
        /// <param name="FStatus"></param>        
        /// <param name="S_Type">New Mod Del  </param>
        /// <returns></returns>
        [HttpGet("Edit_CO_WH_ShipmentEntry")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult>  Edit_CO_WH_ShipmentEntry
            (
                string FInterID,
                string FEntryID,
                string FDetailID,
                string FKPONO,
                string FMPNNO,
                string FCTN,
                string FStatus,

                string S_Type
            )
        {
            CommonResult result = new CommonResult();
            try
            {
                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.Edit_CO_WH_ShipmentEntry
                (
                     FInterID,
                     FEntryID,
                     FDetailID,
                     FKPONO,
                     FMPNNO,
                     FCTN,
                     FStatus,
                   
                     S_Type
                );
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 Edit_CO_WH_ShipmentEntry 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// ImportCheck
        /// </summary>
        /// <param name="v_ExcelDT"></param>
        /// <returns></returns>
        [HttpPost("ImportCheck")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> ImportCheck(List<ExcelDT> v_ExcelDT) 
        {
            CommonResult result = new CommonResult();
            try
            {                
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                WH_ImportDto list = iService.ImportCheck(v_ExcelDT);
                if (list.MSG == "OK")
                {
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                    result.ResData = list;
                    result.Sounds = S_Path_OK;
                }
                else
                {
                    result.Success = false;
                    result.ResultCode = "NG";
                    result.ResultMsg = list.MSG;
                    result.ResData = null;
                    result.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 ImportCheck 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// ImportEnter
        /// </summary>
        /// <param name="v_ExcelDT"></param>
        /// <returns></returns>
        [HttpPost("ImportEnter")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> ImportEnter(List<ExcelDT> v_ExcelDT)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string list = iService.ImportEnter(v_ExcelDT);
                if (list == "OK")
                {
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                    result.ResData = list;
                    result.Sounds = S_Path_OK;
                }
                else
                {
                    result.Success = false;
                    result.ResultCode = "NG";
                    result.ResultMsg = list;
                    result.ResData = null;
                    result.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 ImportEnter 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }






        ///// <summary>
        ///// DB_ExecSql
        ///// </summary>
        ///// <param name="S_Sql"></param>        
        ///// <returns></returns>
        //[HttpGet("DB_ExecSql")]
        //[YuebonAuthorize("")]
        //[AllowAnonymousAttribute]
        //public async Task<IActionResult> DB_ExecSql(string S_Sql)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        string list = iService.DB_ExecSql( S_Sql);
        //        result.Success = true;
        //        result.ResultCode = ErrCode.successCode;
        //        result.ResultMsg = ErrCode.err0;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 GetShipmentEntry_One 异常", ex);
        //        result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
        //        result.ResultCode = "40110";
        //    }
        //    return ToJsonContent(result);
        //}


    }
}