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

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  Public 可用接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    [ApiVersion("2.0")]
    public class PublicController : AreaApiControllerReport<IPublicService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public PublicController(IPublicService _iService) : base(_iService)
        {
            iService = _iService;
        }

        /// <summary>
        /// 获取数据字典
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDictData")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDictData()
        {
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<DictData> list = await iService.GetDictData();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDictData 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取数据字典明细
        /// </summary>
        /// <param name="S_DictDataID">字典ID</param>
        /// <param name="S_EnCode">字典 EnCode</param>
        /// <returns></returns>
        [HttpGet("GetDictDataDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDictDataDetail(string S_DictDataID, string S_EnCode)
        {
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<DictDataDetail> list = await iService.GetDictDataDetail(S_DictDataID, S_EnCode);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDictDataDetail 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetData
        /// </summary>
        /// <param name="S_TabName">表名</param>
        /// <param name="S_Where">条件</param>
        /// <returns></returns>
        [HttpGet("GetData")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetData(string S_TabName, string S_Where)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetData(S_TabName, S_Where);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetData 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
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
                            v_CommonResult.Success = true;
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
        /// 获取线别
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("MesGetLine")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetLine(string ID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetLine(ID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetLine 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工站
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="LineID"></param>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        [HttpGet("MesGetStation")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetStation(string ID, string LineID, string StationTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetStation(ID, LineID, StationTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetStation 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }



        /// <summary>
        /// 获取工站类型
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("MesGetStationType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetStationType(string ID)
        {           
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetStationType(ID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetStationType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取程序类别
        /// </summary>
        /// <param name="StationID"></param>
        /// <returns></returns>
        [HttpGet("LuGetApplicationType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> LuGetApplicationType(string StationID)
        {           
            CommonResult result = new CommonResult();
            try
            {
                string list =await iService.LuGetApplicationType(StationID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 LuGetApplicationType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }





        /// <summary>
        /// 获取料号组类别
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [HttpGet("MesGetPartFamilyType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetPartFamilyType(string ID)
        {           
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetPartFamilyType(ID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetPartFamilyType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取料号组
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="PartFamilyTypeID"></param>
        /// <returns></returns>
        [HttpGet("MesGetPartFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetPartFamily(string ID, string PartFamilyTypeID)
        {
            //ReportCache();
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetPartFamily(ID, PartFamilyTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetPartFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        ///  获取料号
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="PartFamilyID"></param>
        /// <returns></returns>
        [HttpGet("MesGetPart")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetPart(string ID, string PartFamilyID)
        {            
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetPart(ID, PartFamilyID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetPart 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取工单
        /// </summary>
        /// <param name="PartID"></param>
        /// <returns></returns>
        [HttpGet("MesGetProductionOrder")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> MesGetProductionOrder(string PartID)
        {            
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.MesGetProductionOrder(PartID, P_LineID.ToString());
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 MesGetProductionOrder 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取单元状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("LuGetUnitStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> LuGetUnitStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.LuGetUnitStatus();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 LuGetUnitStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 获取不良代码
        /// </summary>
        /// <param name="S_PartFamilyTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetDefect")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDefect(string S_PartFamilyTypeID)
        {
            CommonResult result = new CommonResult();
            try
            {
                IEnumerable<dynamic> list = await iService.GetDefect(S_PartFamilyTypeID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDefect 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

    }
}
