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
    ///  仓库
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class WHController : AreaApiControllerReport<IWHServices, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public WHController(IWHServices _iService) : base(_iService)
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
        /// WH 状态数据类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWHType")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetWHType()
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IEnumerable<dynamic> List_Dyn = await iService.GetWHType();

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetWHType 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 获取主数据
        /// </summary>
        /// <param name="S_Start">开始日期时间</param>
        /// <param name="S_End">结束日期时间</param>
        /// <param name="FStatus">状态</param>
        /// <returns></returns>
        [HttpGet("GetShipmentNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShipmentNew(string S_Start, string S_End, string FStatus)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IEnumerable<dynamic> List_Dyn = await iService.GetShipmentNew(S_Start, S_End, FStatus);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentNew 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 获取子数据
        /// </summary>
        /// <param name="S_FInterID">关联主表ID</param>
        /// <returns></returns>
        [HttpGet("GetShipmentEntryNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShipmentEntryNew(string S_FInterID)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IEnumerable<dynamic> List_Dyn = await iService.GetShipmentEntryNew(S_FInterID);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentEntryNew 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 栈板纸
        /// </summary>
        /// <param name="S_Start">开始日期时间</param>
        /// <param name="S_End">结束日期时间</param>
        /// <param name="FStatus">状态</param>
        /// <returns></returns>
        [HttpGet("GetShipmentReport")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> GetShipmentReport(string S_Start, string S_End, string FStatus)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IEnumerable<dynamic> List_Dyn = await iService.GetShipmentReport(S_Start, S_End, FStatus);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShipmentReport 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 编辑主表
        /// </summary>
        /// <param name="FInterID"></param>
        /// <param name="HAWB"></param>
        /// <param name="FPalletCount"></param>
        /// <param name="FGrossweight"></param>
        /// <param name="FProjectNO"></param>
        /// <param name="FDate"></param>
        /// <param name="FPalletSeq"></param>
        /// <param name="FEmptyCarton"></param>
        /// <param name="PalletSN"></param>
        /// <param name="FShipNO"></param>
        /// <param name="ShipID"></param>
        /// <param name="Region"></param>
        /// <param name="ReferenceNO"></param>
        /// <param name="Country"></param>
        /// <param name="Carrier"></param>
        /// <param name="HubCode"></param>
        /// <param name="TruckNo"></param>
        /// <param name="ReturnAddress"></param>
        /// <param name="DeliveryStreetAddress"></param>
        /// <param name="DeliveryRegion"></param>
        /// <param name="DeliveryToName"></param>
        /// <param name="DeliveryCityName"></param>
        /// <param name="DeliveryCountry"></param>
        /// <param name="AdditionalDeliveryToName"></param>
        /// <param name="DeliveryPostalCode"></param>
        /// <param name="TelNo"></param>
        /// <param name="MAWB_OceanContainerNumber"></param>
        /// <param name="Origin"></param>
        /// <param name="TotalVolume"></param>
        /// <param name="POE_COC"></param>
        /// <param name="TransportMethod"></param>
        /// <param name="POE"></param>
        /// <param name="CTRY"></param>
        /// <param name="SHA"></param>
        /// <param name="Sales"></param>
        /// <param name="WebJ"></param>
        /// <param name="UUI"></param>
        /// <param name="DNJ"></param>
        /// <param name="DeliveryJ"></param>
        /// <param name="Special"></param>
        /// <param name="SCAC"></param>
        /// <param name="OEMSpecificPO1"></param>
        /// <param name="OEMSpecificPO2"></param>
        /// <param name="Type">New(新增) Mod(修改)</param>
        /// <returns></returns>
        [HttpGet("SetShipmentNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetShipmentNew(
                string FInterID,

                string HAWB, string FPalletCount, string FGrossweight, string FProjectNO,
                string FDate, string FPalletSeq, string FEmptyCarton,
                string PalletSN, string FShipNO, string ShipID,

                string Region, string ReferenceNO, string Country, string Carrier, string HubCode,
                string TruckNo, string ReturnAddress, string DeliveryStreetAddress, string DeliveryRegion,
                string DeliveryToName, string DeliveryCityName, string DeliveryCountry, string AdditionalDeliveryToName,
                string DeliveryPostalCode, string TelNo,

                string MAWB_OceanContainerNumber, string Origin, string TotalVolume, string POE_COC, string TransportMethod, string POE,

                string CTRY, string SHA, string Sales, string WebJ, string UUI, string DNJ, string DeliveryJ,
                string Special, string SCAC, string OEMSpecificPO1, string OEMSpecificPO2,

                string Type
            )
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                string S_Result = await iService.SetShipmentNew
                (
                 FInterID,

                 HAWB, FPalletCount, FGrossweight, FProjectNO,
                 FDate, FPalletSeq, FEmptyCarton,
                 PalletSN, FShipNO, ShipID,

                 Region, ReferenceNO, Country, Carrier, HubCode,
                 TruckNo, ReturnAddress, DeliveryStreetAddress, DeliveryRegion,
                 DeliveryToName, DeliveryCityName, DeliveryCountry, AdditionalDeliveryToName,
                 DeliveryPostalCode, TelNo,

                 MAWB_OceanContainerNumber, Origin, TotalVolume, POE_COC, TransportMethod, POE,

                 CTRY, SHA, Sales, WebJ, UUI, DNJ, DeliveryJ,
                 Special, SCAC, OEMSpecificPO1, OEMSpecificPO2,

                 Type
                );

                if (S_Result == "1")
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = S_Result;
                }
                else 
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = ErrCode.err40110;
                    v_CommonResult.ResultMsg = S_Result;
                    v_CommonResult.ResData = S_Result;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetShipmentNew 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }

        /// <summary>
        /// 批量更新状态
        /// </summary>
        /// <param name="FInterID_List"> '1，2，3' ...</param>
        /// <param name="FStatus">状态</param>
        /// <returns></returns>
        [HttpGet("UpdateFStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateFStatus(string FInterID_List, string FStatus)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                string S_Result = await iService.UpdateFStatus(FInterID_List, FStatus);

                if (S_Result == "1")
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = S_Result;
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = ErrCode.err40110;
                    v_CommonResult.ResultMsg = S_Result;
                    v_CommonResult.ResData = S_Result;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 UpdateFStatus 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }

        /// <summary>
        /// 删除主表
        /// </summary>
        /// <param name="FInterID"></param>
        /// <returns></returns>
        [HttpGet("DelShipmentNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> DelShipmentNew(string FInterID)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                string S_Result = await iService.DelShipmentNew(FInterID);

                if (S_Result == "1")
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = S_Result;
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = ErrCode.err40110;
                    v_CommonResult.ResultMsg = S_Result;
                    v_CommonResult.ResData = S_Result;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 DelShipmentNew 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }


        /// <summary>
        ///  编辑子表
        /// </summary>
        /// <param name="FDetailID"></param>
        /// <param name="FInterID"></param>
        /// <param name="FEntryID"></param>
        /// <param name="FCarrierNo"></param>
        /// <param name="FCommercialInvoice"></param>
        /// <param name="FCrossWeight"></param>
        /// <param name="FCTN"></param>
        /// <param name="FKPONO"></param>
        /// <param name="FLineItem"></param>
        /// <param name="FMPNNO"></param>
        /// <param name="FNetWeight"></param>
        /// <param name="FOutSN"></param>
        /// <param name="FPartNumberDesc"></param>
        /// <param name="FQTY"></param>
        /// <param name="FStatus"></param>
        /// <param name="FProjectNO"></param>
        /// <param name="Type">New(新增) Mod(修改)</param>
        /// <returns></returns>
        [HttpGet("SetShipmentEntryNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> SetShipmentEntryNew(
                string FDetailID, string FInterID, string FEntryID, string FCarrierNo, string FCommercialInvoice,
                string FCrossWeight, string FCTN, string FKPONO, string FLineItem, string FMPNNO, string FNetWeight,
                string FOutSN, string FPartNumberDesc, string FQTY, string FStatus, string FProjectNO,

                string Type
            )
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                string S_Result = await iService.SetShipmentEntryNew
                (
                 FDetailID, FInterID, FEntryID, FCarrierNo, FCommercialInvoice,
                 FCrossWeight, FCTN, FKPONO, FLineItem, FMPNNO, FNetWeight,
                 FOutSN, FPartNumberDesc, FQTY, FStatus, FProjectNO,

                 Type
                );

                if (S_Result == "1")
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = S_Result;
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = ErrCode.err40110;
                    v_CommonResult.ResultMsg = S_Result;
                    v_CommonResult.ResData = S_Result;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 SetShipmentEntryNew 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }
        /// <summary>
        /// 删除子表
        /// </summary>
        /// <param name="S_FDetailID">子表 ID</param>
        /// <param name="S_FInterID">主表 ID</param>
        /// <returns></returns>
        [HttpGet("DelShipmentEntryNew")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> DelShipmentEntryNew(string S_FDetailID, string S_FInterID)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                string S_Result = await iService.DelShipmentEntryNew(S_FDetailID, S_FInterID);

                if (S_Result == "1")
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = S_Result;
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = ErrCode.err40110;
                    v_CommonResult.ResultMsg = S_Result;
                    v_CommonResult.ResData = S_Result;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 DelShipmentEntryNew 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }

    }
}