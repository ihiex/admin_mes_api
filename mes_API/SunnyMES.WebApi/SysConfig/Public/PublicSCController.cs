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
using SunnyMES.Security.Services;
using NPOI.OpenXmlFormats.Wordprocessing;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security._1_Models.Public;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  PublicSC 可用接口
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class PublicSCController : AreaApiControllerReport<IPublicSCService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public PublicSCController(IPublicSCService _iService) : base(_iService)
        {
            iService = _iService;
        }


        /// <summary>
        /// GetluApplicationType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluApplicationType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluApplicationType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luApplicationType", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluApplicationType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluDefectType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluDefectType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluDefectType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luDefectType", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluDefectType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetsysStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetsysStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetsysStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("sysStatus", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetsysStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluProductionOrderStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPOStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPOStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luProductionOrderStatus", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPOStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluPartFamilyType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluPartFamilyType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluPartFamilyType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartFamilyType", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluPartFamilyType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluPartFamily
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluPartFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluPartFamily()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartFamily", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluPartFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluPartFamilyDetailDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluPartFamilyDetailDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluPartFamilyDetailDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartFamilyDetailDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluPartFamilyDetailDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluPartFamilyTypeDetailDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluPartFamilyTypeDetailDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluPartFamilyTypeDetailDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartFamilyTypeDetailDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluPartFamilyTypeDetailDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetPartFamily_PartFamilyTypeID
        /// </summary>
        /// <param name="PartFamilyTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetPartFamily_PartFamilyTypeID")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPartFamily_PartFamilyTypeID(string PartFamilyTypeID)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartFamily where PartFamilyTypeID='" + PartFamilyTypeID + "'", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPartFamily_PartFamilyTypeID 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesPart
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesPart")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesPart()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_mesPart> list = await iService.GetmesPart( "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesPart 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesPart_PartFamilyID
        /// </summary>
        /// <param name="PartFamilyID"></param>
        /// <returns></returns>
        [HttpGet("GetmesPart_PartFamilyID")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesPart_PartFamilyID(string PartFamilyID)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("mesPart where PartFamilyID='" + PartFamilyID + "'", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesPart_PartFamilyID 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluPartDetailDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluPartDetailDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluPartDetailDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luPartDetailDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluPartDetailDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesStationType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesStationType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesStationType()
        {
            CommonResult result = new CommonResult();
            try
            {
                //List<SC_IdDesc> list = await iService.GetIdDescription("mesStationType", "");
                List<SC_StationTypeTT> list = await iService.GetSectionTypeTT();

                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesStationType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesStation
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesStation")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesStation()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("mesStation", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesStation 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }



        /// <summary>
        /// GetTypeToStation
        /// </summary>
        /// <param name="StationTypeID"></param>
        /// <returns></returns>
        [HttpGet("GetTypeToStation")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetTypeToStation(string StationTypeID)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("mesStation where StationTypeID='" + StationTypeID + "'", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetTypeToStation 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesLine
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesLine")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesLine()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("mesLine", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesLine 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesLinePMC
        /// </summary>
        /// <param name="LineTypeID"></param>
        /// <param name="PartFamilyID"></param>
        /// <returns></returns>
        [HttpGet("GetmesLinePMC")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesLinePMC(string LineTypeID, string PartFamilyID)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDescLineType> list = await iService.GetmesLinePO(LineTypeID, PartFamilyID);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesLinePMC 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluLineTypeDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluLineTypeDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluLineTypeDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luLineTypeDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluLineTypeDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetLineTypePMC
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLineTypePMC")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetLineTypePMC()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_SIdDesc> list = await iService.GetLineType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetLineTypePMC 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetLineGroupName
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLineGroupName")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetLineGroupName()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_SIdName> list = await iService.GetLineGroupName();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetLineGroupName 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesRoute
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesRoute")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesRoute()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("mesRoute", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesRoute 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesMachine
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesMachine")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesMachine()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdSNDesc> list = await iService.GetIdSNDescription("mesMachine", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesMachine 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluMachineFamilyType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluMachineFamilyType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluMachineFamilyType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("luMachineFamilyType", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluMachineFamilyType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetluMachineFamily
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluMachineFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluMachineFamily()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("luMachineFamily", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluMachineFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluSNFamily
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluSNFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluSNFamily()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("luSNFamily", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluSNFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesSNFormat
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesSNFormat")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesSNFormat()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("mesSNFormat", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesSNFormat 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesUnitState
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesUnitState")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesUnitState()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("mesUnitState", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesUnitState 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetmesProductionOrder
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesProductionOrder")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesProductionOrder()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdPOSNDesc> list = await iService.GetmesProductionOrder();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesProductionOrder 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluProductionOrderDetailDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluProductionOrderDetailDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluProductionOrderDetailDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luProductionOrderDetailDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluProductionOrderDetailDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluMachineStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluMachineStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluMachineStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luMachineStatus", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluMachineStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluLabelFamily
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluLabelFamily")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluLabelFamily()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("luLabelFamily", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluLabelFamily 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetVLabelType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVLabelType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetVLabelType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<V_LabelType> list = await iService.GetVLabelType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetVLabelType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetVOutputType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVOutputType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetVOutputType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<V_OutputType> list = await iService.GetVOutputType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetVOutputType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetVModuleName
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVModuleName")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetVModuleName()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<V_ModuleName> list = await iService.GetVModuleName();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetVModuleName 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetDBField
        /// </summary>
        /// <param name="S_Type"></param>
        /// <returns></returns>
        [HttpGet("GetDBField")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetDBField(string S_Type)
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Q_DBField> list = await iService.GetDBField(S_Type);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetDBField 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }



        /// <summary>
        /// GetVFunctionName
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetVFunctionName")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetVFunctionName()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<V_FunctionName> list = await iService.GetVFunctionName();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetVFunctionName 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesLabelFieldDefinition
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesLabelFieldDefinition")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesLabelFieldDefinition()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("mesLabelFieldDefinition", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesLabelFieldDefinition 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetmesLabel
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetmesLabel")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetmesLabel()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdNameDesc> list = await iService.GetIdNameDescription("mesLabel", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetmesLabel 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluStationTypeDetailDef
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluStationTypeDetailDef")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluStationTypeDetailDef()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luStationTypeDetailDef", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluStationTypeDetailDef 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetEmployeeGroup
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEmployeeGroup")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetEmployeeGroup()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luEmployeeGroup", "");
                
                


                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetEmployeeGroup 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetEmployeeStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEmployeeStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetEmployeeStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetIdDescription("luEmployeeStatus", "");
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetEmployeeStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetPermissionGroup
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPermissionGroup")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetPermissionGroup()
        {
            CommonResult result = new CommonResult();
            try
            {
                //string S_Sql = "SELECT A.Code Id,A.CodeName [Description],A.CodeName_EN DescriptionEN " +
                //    " FROM sysCode A WHERE Catalog='Permission'  ORDER BY A.CodeOrder";
                //List<SC_IdDescDescEN> list = await iService.GetIdDescDescEN(S_Sql);

                List<SC_IdDescDescEN> list = await iService.GetWinPermission(P_EmployeeID.ToString());

                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetPermissionGroup 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetEmployee
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetEmployee")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetEmployee()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_Employee> list = await iService.GetEmployee();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetEmployee 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetLabelFormatPosName
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetLabelFormatPosName")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetLabelFormatPosName()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdName> list = await iService.GetLabelFormatPosName();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetLabelFormatPosName 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetSectionType
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSectionType")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetSectionType()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetSectionType();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSectionType 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetSectionStatus
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetSectionStatus")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetSectionStatus()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetSectionStatus();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetSectionStatus 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetProcesureUDP
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProcesureUDP")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetProcesureUDP()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdName> list = await iService.GetProcesureUDP();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetProcesureUDP 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetluProductionOrderDetailDef_PMC
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetluProductionOrderDetailDef_PMC")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetluProductionOrderDetailDef_PMC()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetluProductionOrderDetailDef_PMC();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetluProductionOrderDetailDef_PMC 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetCom_a
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCom_a")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetCom_a()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Com_Value> list = await iService.GetCom_a();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCom_a 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetCom_d
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCom_d")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetCom_d()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Com_Value> list = await iService.GetCom_d();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCom_d 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetCom_h
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCom_h")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetCom_h()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Com_Value> list = await iService.GetCom_h();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCom_h 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// GetCom_n
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCom_n")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetCom_n()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Com_Value> list = await iService.GetCom_n();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCom_n 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// GetCom_o
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCom_o")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> GetCom_o()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<Com_Value> list = await iService.GetCom_o();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCom_o 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// InitializeBaseData
        /// </summary>
        /// <returns></returns>
        [HttpGet("InitializeBaseData")]
        [YuebonAuthorize("")]
        [AllowAnonymousAttribute]
        public async Task<IActionResult> InitializeBaseData()
        {
            CommonResult result = new CommonResult();
            try
            {
                string list = await iService.InitializeBaseData();
                if (list == "OK")
                {
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                    result.ResultMsg = ErrCode.err0;
                    result.ResData = list;
                }
                else 
                {
                    result.Success = false;
                    result.ResultCode = ErrCode.err1;
                    result.ResultMsg = list;
                    result.ResData = null;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 InitializeBaseData 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


    }
}