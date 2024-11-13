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
using System.Dynamic;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Core.PublicFun;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.Reflection;
using Quartz.Impl.Triggers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Policy;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  Report 可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class ReportController : AreaApiControllerReport<IReportService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public ReportController(IReportService _iService) : base(_iService)
        {
            iService = _iService;
        }


        private CommonResult Com_Result(CommonResult v_CommonResult, List<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();

            if (List_Dyn.Count > 0)
            {
                try
                {
                    if (List_Dyn[0][0] is TabVal)
                    {
                        TabVal v_TabVal = List_Dyn[0][0] as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = null;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Dyn;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Dyn;

            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }


        /// <summary>
        /// 获取 GetOutputSum
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_Shift">班别</param>
        /// <param name="S_DataType">数据类型 --0: CumByProject 1:ManualQuery ...</param>
        /// <param name="S_DataLevel">数据层次 5: byProject/PN/Line/StationType/CDate 4: byProject/PN/StationType/CDate 3: byProject/StationType/CDate 2: byStationType/CDate</param>
        /// <param name="YieldLevel">YieldLevel</param>
        /// <param name="IsCombineYield">IsCombineYield</param>
        /// <returns></returns>
        [HttpGet("GetOutputSum")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetOutputSum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //ReportCache();

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetOutputSum(S_StartDateTime, S_EndDateTime,
                    S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                    S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel,  YieldLevel,  IsCombineYield);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetOutputSum 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 获取料号组类别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPartFamilyType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartFamilyType()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPartFamilyType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartFamilyType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        ///  获取料号组
        /// </summary>
        /// <param name="PartFamilyTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetPartFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartFamily(string PartFamilyTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPartFamily(PartFamilyTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        ///  获取料号
        /// </summary>
        /// <param name="PartFamilyID"></param>
        /// <returns></returns>
        [HttpGet("GetPart")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPart(string PartFamilyID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPart(PartFamilyID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPart 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        ///  获取工单
        /// </summary>
        /// <param name="PartID"></param>
        /// <returns></returns>
        [HttpGet("GetProductionOrder")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetProductionOrder(string PartID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetProductionOrder(PartID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetProductionOrder 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取工站类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetStationType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetStationType()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetStationType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPart 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工站
        /// </summary>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetStation")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetStation(string StationTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetStation(StationTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetStation 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取班别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetShift")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetShift()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetShift();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetShift 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取线别
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLine")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetLine()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetLine();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetLine 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取 UPHYield
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_Shift">班别</param>
        /// <param name="S_DataType">数据类型 0: Hour 1: Day 2: Week  3: Mouth 4: Year</param>
        /// <param name="S_DataLevel">数据层次 5: byProject/PN/Line/StationType/CDate 4: byProject/PN/StationType/CDate 3: byProject/StationType/CDate 2: byStationType/CDate</param>
        /// <param name="YieldLevel">YieldLevel</param>
        /// <param name="IsCombineYield">IsCombineYield</param>        
        /// <returns></returns>
        [HttpGet("GetUPHYield")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetUPHYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //ReportCache();
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetUPHYield(S_StartDateTime, S_EndDateTime,
                    S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                    S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel,  YieldLevel,  IsCombineYield);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetUPHYield 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// 获取 GetUPH
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_Shift">班别</param>
        /// <param name="S_DataType">数据类型 --0: Hour 1: Day 2: Week  3: Mouth 4: Year...</param>
        /// <param name="S_DataLevel">数据层次 5: byProject/PN/Line/StationType/CDate 4: byProject/PN/StationType/CDate 3: byProject/StationType/CDate 2: byStationType/CDate</param>
        /// <param name="YieldLevel">YieldLevel</param>
        /// <param name="IsCombineYield">IsCombineYield</param>        
        /// <returns></returns>
        [HttpGet("GetUPH")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetUPH(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //ReportCache();
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetUPH(S_StartDateTime, S_EndDateTime,
                    S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                    S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel,  YieldLevel,  IsCombineYield);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetUPH 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// 获取 GetUPHCum
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_Shift">班别</param>
        /// <param name="S_DataType">数据类型 --0: Hour 1: Day 2: Week  3: Mouth 4: Year...</param>
        /// <param name="S_DataLevel">数据层次 5: byProject/PN/Line/StationType/CDate 4: byProject/PN/StationType/CDate 3: byProject/StationType/CDate 2: byStationType/CDate</param>
        /// <param name="YieldLevel">YieldLevel</param>
        /// <param name="IsCombineYield">IsCombineYield</param>        
        /// <returns></returns>
        [HttpGet("GetUPHCum")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetUPHCum(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string YieldLevel, string IsCombineYield)
        {
            //ReportCache();
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetUPHCum(S_StartDateTime, S_EndDateTime,
                    S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                    S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel, YieldLevel, IsCombineYield);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetUPHCum 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }



        /// <summary>
        /// 获取 GetDefectYield
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_Shift">班别</param>
        /// <param name="S_DataType">数据类型 --0: Hour 1: Day 2: Week  3: Mouth 4: Year...</param>
        /// <param name="S_DataLevel">数据层次 5: byProject/PN/Line/StationType/CDate 4: byProject/PN/StationType/CDate 3: byProject/StationType/CDate 2: byStationType/CDate</param>
        /// <param name="S_TopQTY">TopQTY</param>
        /// <param name="YieldLevel">YieldLevel</param>
        /// <param name="IsCombineYield">IsCombineYield</param>        
        /// <returns></returns>
        [HttpGet("GetDefectYield")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDefectYield(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_Shift, string S_DataType,
            string S_DataLevel, string S_TopQTY, string YieldLevel, string IsCombineYield)
        {
            //ReportCache();
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetDefectYield(S_StartDateTime, S_EndDateTime,
                    S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                    S_StationTypeID, S_StationID, S_LineID, S_Shift, S_DataType, S_DataLevel, S_TopQTY,  YieldLevel,  IsCombineYield);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDefectYield 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }




        /// <summary>
        /// GetDataLevel
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDataLevel")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDataLevel()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetDataLevel();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDataLevel 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// TTWIP，TTWIP2，TTWIPList，TTWIP2List,WHSearchData
        /// </summary>
        /// <param name="S_Type">类别 （TTWIP，TTWIP2，TTWIPList，TTWIP2List,WHSearchData）</param>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>
        /// <param name="S_SNs">SN</param>
        /// <param name="S_PageIndex">页</param>
        ///  <param name="S_PageQTY">页行数</param>
        /// <returns></returns>
        [HttpGet("GetReportGeneral")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetReportGeneral(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
           // string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            //ReportCache();

            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";

            //Shift = Shift ?? ""; 
            //YieldLevel = YieldLevel ?? ""; 
            //IsCombineYield = IsCombineYield ?? "";

            if (S_SNs == null) S_SNs = "";
            if (S_PageIndex == null) S_PageIndex = "1";
            if (S_PageQTY == null) S_PageQTY = "10";

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetReportGeneral(S_Type, S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,                
                S_SNs, S_PageIndex, S_PageQTY);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetReportGeneral 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        ///获取  GetSearchCenter
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>
        /// <param name="S_SNs">SN</param>
        /// <param name="S_Type">查询类型 Search/UnitInfor/TraceabilityView/TraceabilityDetail/TestRecord/PackageView/PackageDetail/PartView/PartDetail/ProductionOrder/RouteInfo/RouteInfoView/RouteInfoDetail/MaterialUnit/Machine/Shipment/MultipackDetail</param>
        /// <returns></returns>
        [HttpGet("GetSearchCenter")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetSearchCenter(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_Type)
        {
            //ReportCache();

            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";
            if (S_SNs == null) S_SNs = "";
            if (S_Type == null) S_Type = "";

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetSearchCenter(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                S_SNs, S_Type);

                string json = JsonConvert.SerializeObject(List_Dyn);
                if (json.Contains("OutputERROR:"))
                {
                    Log4NetHelper.Error("获取 GetDefectYield 异常 "+ json);
                    v_ComResult.ResultMsg =  json;
                    v_ComResult.ResultCode = "40110";
                }
                else
                {
                    v_ComResult = Com_Result(v_ComResult, List_Dyn);
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDefectYield 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <param name="S_Type">类别 （TTWIP，TTWIP2，TTWIPList，TTWIP2List,WIPComponentList）</param>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>
        /// <param name="S_SNs">SN</param>
        /// <param name="S_PageIndex">页</param>
        ///  <param name="S_PageQTY">页行数</param>
        /// <returns></returns>
        [HttpGet("GetExportExcel")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetExportExcel(string S_Type, string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";
            if (S_SNs == null) S_SNs = "";
            if (S_PageIndex == null) S_PageIndex = "1";
            if (S_PageQTY == null) S_PageQTY = "0";

            CommonResult v_CommonResult = new CommonResult();
            try
            {
                IEnumerable<dynamic> List_Dyn = await iService.GetExportExcel(S_Type, S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                S_SNs, S_PageIndex, S_PageQTY);

                List<dynamic> List_Main = new List<dynamic>();
                List_Main.Add(List_Dyn);

                if (List_Main.Count > 0)
                {
                    try
                    {
                        if (List_Main[0][0] is TabVal)
                        {
                            TabVal v_TabVal = List_Main[0][0] as TabVal;
                            if (v_TabVal.ValStr1 == "ERROR")
                            {
                                v_CommonResult.Success = false;
                                v_CommonResult.ResultCode = v_TabVal.ValStr1;
                                v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                                v_CommonResult.ResData = null;
                            }
                            else
                            {
                                v_CommonResult.Success = true;
                                v_CommonResult.ResultCode = ErrCode.successCode;
                                v_CommonResult.ResultMsg = ErrCode.err0;
                                v_CommonResult.ResData = v_TabVal.ValStr2;
                            }
                        }
                    }
                    catch
                    {
                        v_CommonResult.Success = false;
                        v_CommonResult.ResultCode = "ERROR";
                        v_CommonResult.ResultMsg = ErrCode.err1;
                        v_CommonResult.ResData = null;
                    }
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = "ERROR";
                    v_CommonResult.ResultMsg = ErrCode.err1;
                    v_CommonResult.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetExportExcel 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }


        /// <summary>
        /// 物料追溯报表
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>

        /// <param name="S_SNs">SN</param>
        /// <param name="S_PageIndex">页</param>
        ///  <param name="S_PageQTY">页行数</param>
        /// <returns></returns>
        [HttpGet("GetWIPComponentList")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetWIPComponentList(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";

            if (S_SNs == null) S_SNs = "";
            if (S_PageIndex == null) S_PageIndex = "1";
            if (S_PageQTY == null) S_PageQTY = "10";

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetWIPComponentList(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                S_SNs, S_PageIndex, S_PageQTY);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetWIPComponentList 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// GetRawData  原数据报表
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>

        /// <param name="Shift">班次</param>
        /// <param name="YieldLevel">产出等级   1  first pass yield,  2 second pass yield, 0 final yield</param>
        /// <param name="IsCombineYield">是否组合产出 -- 0 no combine, 1 combine</param>

        /// <param name="S_SNs">SN</param>
        /// <param name="S_PageIndex">页</param>
        ///  <param name="S_PageQTY">页行数</param>
        /// <returns></returns>
        [HttpGet("GetRawData")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetRawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";

            Shift = Shift ?? "";
            YieldLevel = YieldLevel ?? "";
            IsCombineYield = IsCombineYield ?? "";

            if (S_SNs == null) S_SNs = "";
            if (S_PageIndex == null) S_PageIndex = "1";
            if (S_PageQTY == null) S_PageQTY = "10";

            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetRawData(S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                 Shift,  YieldLevel,  IsCombineYield,
                S_SNs, S_PageIndex, S_PageQTY);

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetRawData 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// GetExportExcel_RawData  原数据报表  Excel 导出
        /// </summary>
        /// <param name="S_StartDateTime">开始时间</param>
        /// <param name="S_EndDateTime">结束时间</param>
        /// <param name="S_PartFamilyTypeID">料号组类别</param>
        /// <param name="S_PartFamilyID">料号类别</param>
        /// <param name="S_PartID">料号</param>
        /// <param name="S_ProductionOrderID">工单</param>
        /// <param name="S_StationTypeID">工站类型</param>
        /// <param name="S_StationID">工站</param>
        /// <param name="S_LineID">线别</param>
        /// <param name="S_UnitStateID">过站状态</param>
        /// <param name="S_UnitStatusID">单元状态</param>

        /// <param name="Shift">班次</param>
        /// <param name="YieldLevel">产出等级   1  first pass yield,  2 second pass yield, 0 final yield</param>
        /// <param name="IsCombineYield">是否组合产出 -- 0 no combine, 1 combine</param>

        /// <param name="S_SNs">SN</param>
        /// <param name="S_PageIndex">页</param>
        ///  <param name="S_PageQTY">页行数</param>
        /// <returns></returns>
        [HttpGet("GetExportExcel_RawData")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetExportExcel_RawData(string S_StartDateTime, string S_EndDateTime,
            string S_PartFamilyTypeID, string S_PartFamilyID, string S_PartID, string S_ProductionOrderID,
            string S_StationTypeID, string S_StationID, string S_LineID, string S_UnitStateID, string S_UnitStatusID,
            string Shift, string YieldLevel, string IsCombineYield,
            string S_SNs, string S_PageIndex, string S_PageQTY)
        {
            if (S_StartDateTime == null) S_StartDateTime = "2000-01-01";
            if (S_EndDateTime == null) S_EndDateTime = "2000-01-01";

            if (S_PartFamilyTypeID == null) S_PartFamilyTypeID = "";
            if (S_PartFamilyID == null) S_PartFamilyID = "";
            if (S_PartID == null) S_PartID = "";
            if (S_ProductionOrderID == null) S_ProductionOrderID = "";
            if (S_StationTypeID == null) S_StationTypeID = "";
            if (S_StationID == null) S_StationID = "";
            if (S_LineID == null) S_LineID = "";
            if (S_UnitStateID == null) S_UnitStateID = "";
            if (S_UnitStatusID == null) S_UnitStatusID = "";

            Shift = Shift ?? "";
            YieldLevel = YieldLevel ?? "";
            IsCombineYield = IsCombineYield ?? "";

            if (S_SNs == null) S_SNs = "";
            if (S_PageIndex == null) S_PageIndex = "1";
            if (S_PageQTY == null) S_PageQTY = "0";

            CommonResult v_CommonResult = new CommonResult();
            try
            {
                IEnumerable<dynamic> List_Dyn = await iService.GetExportExcel_RawData( S_StartDateTime, S_EndDateTime,
                S_PartFamilyTypeID, S_PartFamilyID, S_PartID, S_ProductionOrderID,
                S_StationTypeID, S_StationID, S_LineID, S_UnitStateID, S_UnitStatusID,
                 Shift, YieldLevel, IsCombineYield,
                S_SNs, S_PageIndex, S_PageQTY);

                List<dynamic> List_Main = new List<dynamic>();
                List_Main.Add(List_Dyn);

                if (List_Main.Count > 0)
                {
                    try
                    {
                        if (List_Main[0][0] is TabVal)
                        {
                            TabVal v_TabVal = List_Main[0][0] as TabVal;
                            if (v_TabVal.ValStr1 == "ERROR")
                            {
                                v_CommonResult.Success = false;
                                v_CommonResult.ResultCode = v_TabVal.ValStr1;
                                v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                                v_CommonResult.ResData = null;
                            }
                            else
                            {
                                v_CommonResult.Success = true;
                                v_CommonResult.ResultCode = ErrCode.successCode;
                                v_CommonResult.ResultMsg = ErrCode.err0;
                                v_CommonResult.ResData = v_TabVal.ValStr2;
                            }
                        }
                    }
                    catch
                    {
                        v_CommonResult.Success = false;
                        v_CommonResult.ResultCode = "ERROR";
                        v_CommonResult.ResultMsg = ErrCode.err1;
                        v_CommonResult.ResData = null;
                    }
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = "ERROR";
                    v_CommonResult.ResultMsg = ErrCode.err1;
                    v_CommonResult.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetExportExcel_RawData 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }



        /// <summary>
        /// 获取料号组 ALL
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetPartFamilyAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartFamilyAll(string S_PartFamilyTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPartFamilyAll(S_PartFamilyTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartFamilyAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取料号组类别 ALL
        /// </summary>       
        /// <returns></returns>
        [HttpGet("GetPartFamilyTypeAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartFamilyTypeAll()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPartFamilyTypeAll();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartFamilyTypeAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工站类型 ALL
        /// </summary>       
        /// <returns></returns>
        [HttpGet("GetStationTypeAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetStationTypeAll()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetStationTypeAll();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetStationTypeAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取线别 ALL
        /// </summary>       
        /// <returns></returns>
        [HttpGet("GetLineAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetLineAll()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetLineAll();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetLineAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取料号 ALL
        /// </summary>
        /// <param name="S_PartFamilyID"></param>
        /// <returns></returns>
        [HttpGet("GetPartNumberAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartNumberAll(string S_PartFamilyID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetPartNumberAll(S_PartFamilyID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartNumberAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取工单 ALL
        /// </summary>
        /// <param name="S_PartID"></param>
        /// <returns></returns>
        [HttpGet("GetProductionOrderAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetProductionOrderAll(string S_PartID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetProductionOrderAll(S_PartID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetProductionOrderAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取工站 ALL
        /// </summary>
        /// <param name="S_StationTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetStationAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetStationAll(string S_StationTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetStationAll(S_StationTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetStationAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取单元状态 ALL
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUnitStatusAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetUnitStatusAll()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetUnitStatusAll();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetUnitStatusAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取过站状态 ALL
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUnitStateAll")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetUnitStateAll()
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetUnitStateAll();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetUnitStateAll 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工厂代码
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSAPAllPlant")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPAllPlant()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<dynamic> list = await iService.GetSAPAllPlant();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSAPAllPlant 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 根据工厂代码获取所有版本
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        [HttpGet("GetSAPAllVersion")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPAllVersion(int plant)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<dynamic> list = await iService.GetSAPAllVersion(plant);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSAPAllVersion 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 根据工厂代码获取所有物料信息
        /// </summary>
        /// <param name="plant"></param>
        /// <returns></returns>
        [HttpGet("GetSAPStockMaterial")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPStockMaterial(int plant)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<dynamic> list = await iService.GetSAPStockMaterial(plant);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSAPStockMaterial 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }
        /// <summary>
        /// 根据条件获取物料库存数量
        /// </summary>
        /// <param name="plant">工厂代码</param>
        /// <param name="version">版本</param>
        /// <param name="material">物料（传空，则查询所有，多个以逗号分隔）</param>
        /// <param name="CurrentPageIndex">当前第几页</param>
        /// <param name="PageSize">单页大小</param>        /// <returns></returns>
        [HttpGet("GetSAPStock")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPStock(int plant, string version, string material,  int CurrentPageIndex = 1, int PageSize = 1)
        {
            CommonResult result = new CommonResult();
            try
            {
                if (plant <= 0 || string.IsNullOrEmpty(version))
                {
                    result.ResultMsg = "please select the correct [plant] and [version]";
                    result.ResultCode = ErrCode.err1;
                    result.Success = false;
                }
                else
                {
                    List<dynamic> list = await iService.GetSAPStock(plant, version ?? "", material ?? "", CurrentPageIndex, PageSize);
                    if (list.Count == 1)
                    {
                        var tmpValTab = list[0][0] as TabVal;
                        result.Success = false;
                        result.ResultCode = ErrCode.err1;
                        result.ResultMsg = tmpValTab?.ValStr1 + ":" + tmpValTab?.ValStr2;
                        result.ResData = null;
                    }
                    else if (list.Count == 2)
                    {
                        var v = PublicCommonFun.ConvertDynamic(list[1], "TotalItems");
                        result.Success = true;
                        result.ResultCode = ErrCode.successCode;
                        result.ResultMsg = ErrCode.err0;
                        result.ResData = new {
                            Items = list[0],
                            TotalItems = v,
                        };
                    }
                    else
                    {
                        throw new Exception("查询数据库失败");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSAPStock 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 根据条件查询库存导出Excel
        /// </summary>
        /// <param name="plant">工厂代码</param>
        /// <param name="version">版本</param>
        /// <param name="material">物料（传空，则查询所有，多个以逗号分隔）</param>
        /// <returns></returns>
        [HttpGet("GetExportExcel_SapStock")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetExportExcel_SapStock(int plant, string version, string material)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                if (plant <= 0 || string.IsNullOrEmpty(version))
                {
                    v_CommonResult.ResultMsg = "please select the correct [plant] and [version]";
                    v_CommonResult.ResultCode = ErrCode.err1;
                    v_CommonResult.Success = false;
                    return ToJsonContent(v_CommonResult);
                }

                IEnumerable<dynamic> List_Dyn = await iService.GetExportExcel_SapStock(plant,version,material);

                List<dynamic> List_Main = new List<dynamic>();
                List_Main.Add(List_Dyn);

                if (List_Main.Count > 0)
                {
                    try
                    {
                        if (List_Main[0][0] is TabVal)
                        {
                            TabVal v_TabVal = List_Main[0][0] as TabVal;
                            if (v_TabVal.ValStr1 == "ERROR")
                            {
                                v_CommonResult.Success = false;
                                v_CommonResult.ResultCode = v_TabVal.ValStr1;
                                v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                                v_CommonResult.ResData = null;
                            }
                            else
                            {
                                v_CommonResult.Success = true;
                                v_CommonResult.ResultCode = ErrCode.successCode;
                                v_CommonResult.ResultMsg = ErrCode.err0;
                                v_CommonResult.ResData = v_TabVal.ValStr2;
                            }
                        }
                    }
                    catch
                    {
                        v_CommonResult.Success = false;
                        v_CommonResult.ResultCode = "ERROR";
                        v_CommonResult.ResultMsg = ErrCode.err1;
                        v_CommonResult.ResData = null;
                    }
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = "ERROR";
                    v_CommonResult.ResultMsg = ErrCode.err1;
                    v_CommonResult.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetExportExcel_SapStock 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }

        /// <summary>
        /// 根据条码分页查询数据
        /// </summary>
        /// <param name="plant">工厂代码</param>
        /// <param name="version">版本</param>
        /// <param name="material">物料（传空，则查询所有，多个以逗号分隔）</param>
        /// <param name="CurrentPageIndex">当前第几页</param>
        /// <param name="PageSize">单页大小</param>
        /// <param name="sapDateType">生成数据类型 1，按照天生成，2，按照周生成</param>
        /// <returns></returns>
        [HttpGet("GetSAPStockPro")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetSAPStockPro(int plant, string version, string material, int CurrentPageIndex = 1, int PageSize = 1, SapDateType sapDateType = SapDateType.Week)
        {
            CommonResult result = new CommonResult();
            try
            {
                if (plant <= 0 || string.IsNullOrEmpty(version))
                {
                    result.ResultMsg = "please select the correct [plant] and [version]";
                    result.ResultCode = ErrCode.err1;
                    result.Success = false;
                }
                else
                {
                    List<dynamic> list = await iService.GetSAPStockPro(plant, version ?? "", material ?? "", CurrentPageIndex, PageSize,sapDateType);
                    if (list.Count == 1)
                    {
                        var tmpValTab = list[0][0] as TabVal;
                        result.Success = false;
                        result.ResultCode = ErrCode.err1;
                        result.ResultMsg = tmpValTab?.ValStr1 + ":" + tmpValTab?.ValStr2;
                        result.ResData = null;
                    }
                    else if (list.Count == 2)
                    {
                        var v = PublicCommonFun.ConvertDynamic(list[1], "TotalItems");
                        result.Success = true;
                        result.ResultCode = ErrCode.successCode;
                        result.ResultMsg = ErrCode.err0;
                        result.ResData = new
                        {
                            Items = list[0],
                            TotalItems = v,
                        };
                    }
                    else
                    {
                        throw new Exception("查询数据库失败");
                    }
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSAPStock 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 根据条件查询库存导出Excel
        /// </summary>
        /// <param name="plant">工厂代码</param>
        /// <param name="version">版本</param>
        /// <param name="material">物料（传空，则查询所有，多个以逗号分隔）</param>
        /// <param name="sapDateType">生成数据类型 1，按照天生成，2，按照周生成</param>
        /// <returns></returns>
        [HttpGet("GetExportExcel_SapStockPro")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetExportExcel_SapStockPro(int plant, string version, string material,SapDateType sapDateType = SapDateType.Week)
        {
            CommonResult v_CommonResult = new CommonResult();
            try
            {
                if (plant <= 0 || string.IsNullOrEmpty(version))
                {
                    v_CommonResult.ResultMsg = "please select the correct [plant] and [version]";
                    v_CommonResult.ResultCode = ErrCode.err1;
                    v_CommonResult.Success = false;
                    return ToJsonContent(v_CommonResult);
                }

                IEnumerable<dynamic> List_Dyn = await iService.GetExportExcel_SapStockPro(plant, version, material, sapDateType);

                List<dynamic> List_Main = new List<dynamic>();
                List_Main.Add(List_Dyn);

                if (List_Main.Count > 0)
                {
                    try
                    {
                        if (List_Main[0][0] is TabVal)
                        {
                            TabVal v_TabVal = List_Main[0][0] as TabVal;
                            if (v_TabVal.ValStr1 == "ERROR")
                            {
                                v_CommonResult.Success = false;
                                v_CommonResult.ResultCode = v_TabVal.ValStr1;
                                v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                                v_CommonResult.ResData = null;
                            }
                            else
                            {
                                v_CommonResult.Success = true;
                                v_CommonResult.ResultCode = ErrCode.successCode;
                                v_CommonResult.ResultMsg = ErrCode.err0;
                                v_CommonResult.ResData = v_TabVal.ValStr2;
                            }
                        }
                    }
                    catch
                    {
                        v_CommonResult.Success = false;
                        v_CommonResult.ResultCode = "ERROR";
                        v_CommonResult.ResultMsg = ErrCode.err1;
                        v_CommonResult.ResData = null;
                    }
                }
                else
                {
                    v_CommonResult.Success = false;
                    v_CommonResult.ResultCode = "ERROR";
                    v_CommonResult.ResultMsg = ErrCode.err1;
                    v_CommonResult.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetExportExcel_SapStock 异常", ex);
                v_CommonResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_CommonResult.ResultCode = "40110";
            }
            return ToJsonContent(v_CommonResult);
        }


        /// <summary>
        /// GetAutoAnalysisAlarmDashboard
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAutoAnalysisAlarmDashboard")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetAutoAnalysisAlarmDashboard()
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                List<dynamic> List_Dyn = await iService.GetAutoAnalysisAlarmDashboard();

                v_ComResult = Com_Result(v_ComResult, List_Dyn);
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetAutoAnalysisAlarmDashboard 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

    }
}