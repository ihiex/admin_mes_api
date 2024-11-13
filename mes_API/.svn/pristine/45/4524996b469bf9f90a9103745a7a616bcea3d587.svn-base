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
using System.Data;
using SunnyMES.Commons.Pages;
using NPOI.POIFS.Properties;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    ///  SC_mesLabel 可用接口
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesLabelController : AreaApiControllerReport<ISC_mesLabelService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public SC_mesLabelController(ISC_mesLabelService _iService) : base(_iService)
        {
            iService = _iService;
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

                v_ComResult.Success = true;
                v_ComResult.ResultCode = ErrCode.successCode;
                v_ComResult.ResultMsg = ErrCode.err0;
                v_ComResult.ResData = List_ConfInfo[0].ValStr1;
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
        /// Insert
        /// </summary>
        /// <param name="v_SC_mesLabel"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Insert(SC_mesLabel v_SC_mesLabel)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Insert(v_SC_mesLabel, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabel Insert 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }



        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Delete(string Id)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Delete(Id, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabel Delete 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="v_SC_mesLabel"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(SC_mesLabel v_SC_mesLabel)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }


                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Update(v_SC_mesLabel, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 Update 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// Clone
        /// </summary>
        /// <param name="v_SC_mesLabel"></param>
        /// <returns></returns>
        [HttpPost("Clone")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Clone(SC_mesLabel v_SC_mesLabel)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }


                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Clone(v_SC_mesLabel, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabel Clone 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// 异步分页查询,LikeQuery有值时，其他查询参数无效。以模糊查询优先
        /// </summary>
        /// <param name="CurrentPageIndex">当前页</param>
        /// <param name="PageSize">页行数</param>
        /// <param name="LikeQuery">模糊查询字段 文本类型 </param>  
        /// <param name="ID">主表ID</param>
        /// <param name="Name"></param>
        /// <param name="Description"></param>
        /// <param name="LabelFamilyID"></param>
        /// <param name="LabelFamily"></param>
        /// <param name="LabelType"></param>
        /// <param name="LabelTypeName"></param>
        /// <param name="TargetPath"></param>
        /// <param name="OutputType"></param>
        /// <param name="OutputTypeName"></param>
        /// <param name="PrintCMD"></param>
        /// <param name="SourcePath"></param>
        /// <param name="PageCapacity"></param>
        /// <param name="LabelFieldDefName"></param>
        /// <returns></returns>
        [HttpGet("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> FindWithPagerSearchAsync
            (
                int CurrentPageIndex,
                int PageSize,
                string LikeQuery,

                string ID,
                string Name,
                string Description,
                string LabelFamilyID,
                string LabelFamily,
                string LabelType,
                string LabelTypeName,
                string TargetPath,
                string OutputType,

                string OutputTypeName,
                string PrintCMD,
                string SourcePath,
                string PageCapacity,
                string LabelFieldDefName
            )
        {

            CommonResult<PageResult<SC_mesLabelALLDto>> v_ComResult = new CommonResult<PageResult<SC_mesLabelALLDto>>();
            try
            {
                SC_mesLabelSearch search = new SC_mesLabelSearch();

                search.CurrentPageIndex = CurrentPageIndex;
                search.PageSize = PageSize;
                search.LikeQuery = LikeQuery;

                search.ID = ID;
                search.Name = Name;
                search.Description = Description;
                search.LabelFamilyID = LabelFamilyID;
                search.LabelFamily = LabelFamily;
                search.LabelType = LabelType;
                search.LabelTypeName = LabelTypeName;
                search.TargetPath = TargetPath;
                search.OutputType = OutputType;
                search.OutputTypeName = OutputTypeName;
                search.PrintCMD = PrintCMD;
                search.SourcePath = SourcePath;
                search.PageCapacity = PageCapacity;
                search.LabelFieldDefName = LabelFieldDefName;

                PageResult<SC_mesLabelALLDto> list = await iService.FindWithPagerSearchAsync(search);

                v_ComResult.Success = true;
                v_ComResult.ResultCode = ErrCode.successCode;
                v_ComResult.ResultMsg = ErrCode.err0;
                v_ComResult.ResData = list;
                v_ComResult.Sounds = S_Path_OK;
            }
            catch (Exception ex)
            {
                v_ComResult.Success = false;
                v_ComResult.ResultCode = "NG";
                v_ComResult.ResultMsg = ex.Message;
                v_ComResult.ResData = null;
                v_ComResult.Sounds = S_Path_NG;

                Log4NetHelper.Error("获取 mesLabel FindWithPagerSearchAsync 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);

        }


        ///////////////////////////////////////  Detail ///////////////////////////////////////////////////////////

        /// <summary>
        /// InsertDetail
        /// </summary>
        /// <param name="ParentId"></param>
        /// <param name="v_Detail"></param>
        /// <returns></returns>
        [HttpPost("InsertDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> InsertDetail(string ParentId, SC_mesLabelField v_Detail)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }


                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IDbTransaction trans = null;
                string list = await iService.InsertDetail(ParentId, v_Detail, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabelField 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// DeleteDetail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteDetail(string Id)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }


                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IDbTransaction trans = null;
                string list = await iService.DeleteDetail(Id, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabelField Delete 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// UpdateDetail
        /// </summary>
        /// <param name="v_Detail"></param>
        /// <returns></returns>
        [HttpPost("UpdateDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDetail(SC_mesLabelField v_Detail)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }


                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IDbTransaction trans = null;
                string list = await iService.UpdateDetail(v_Detail, trans);

                if (list == "OK")
                {
                    v_ComResult.Success = true;
                    v_ComResult.ResultCode = ErrCode.successCode;
                    v_ComResult.ResultMsg = ErrCode.err0;
                    v_ComResult.ResData = list;
                    v_ComResult.Sounds = S_Path_OK;
                }
                else
                {
                    v_ComResult.Success = false;
                    v_ComResult.ResultCode = "NG";
                    v_ComResult.ResultMsg = list;
                    v_ComResult.ResData = null;
                    v_ComResult.Sounds = S_Path_NG;
                }
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 mesLabelField Update 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

    }
}