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
    ///  SC_mesLine 可用接口
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesLineController : AreaApiControllerReport<ISC_mesLineService, string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public SC_mesLineController(ISC_mesLineService _iService) : base(_iService)
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
        /// <param name="v_SC_mesLineDto"></param>
        /// <returns></returns>
        [HttpPost("Insert")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Insert(SC_mesLineDto v_SC_mesLineDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //P_EmployeeID = 1;
                //P_CurrentLoginIP = "172.16.53.60";
                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Insert(v_SC_mesLineDto, trans);

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
                Log4NetHelper.Error("获取 Insert 异常", ex);
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
                Log4NetHelper.Error("获取 Delete 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }


        /// <summary>
        /// Update
        /// </summary>
        /// <param name="v_SC_mesLineDto"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Update(SC_mesLineDto v_SC_mesLineDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //P_EmployeeID = 1;
               // P_CurrentLoginIP = "172.16.53.60";

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Update(v_SC_mesLineDto, trans);

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
        /// <param name="v_SC_mesLineDto"></param>
        /// <returns></returns>
        [HttpPost("Clone")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> Clone(SC_mesLineDto v_SC_mesLineDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //P_EmployeeID = 1;
                // P_CurrentLoginIP = "172.16.53.60";

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);
                IDbTransaction trans = null;
                string list = await iService.Clone(v_SC_mesLineDto, trans);

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
                Log4NetHelper.Error("获取 Clone 异常", ex);
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
        /// <param name="LikeQuery">模糊查询字段 文本类型 Description，StatusValue，LineTypeDefValue，PropertyContent</param>        
        /// <param name="ID">ID 传值规则  1,2,3,4,5 ... </param>
        /// <param name="LineName">线别名字</param>
        /// <param name="StatusID">1: Active 2: InActive</param>
        /// <param name="StatusValue">Active,InActive</param>
        /// <param name="LineTypeDefID">传值规则  1,2,3,4,5 ...</param>
        /// <param name="LineTypeDefValue">属性名字</param>
        /// <param name="PropertyContent">属性内容</param>        
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
                string LineName,
                string StatusID,
                string StatusValue,
                string LineTypeDefID,
                string LineTypeDefValue,
                string PropertyContent 
            )
        {

            CommonResult<PageResult<SC_mesLineALLDto>> v_ComResult = new CommonResult<PageResult<SC_mesLineALLDto>>();
            //PageResult<SC_mesLineALLDto> v_ComResult = new PageResult<SC_mesLineALLDto>();
            try
            {
                SC_mesLineSearch search = new SC_mesLineSearch();

                search.CurrentPageIndex = CurrentPageIndex;
                search.PageSize = PageSize;
                search.LikeQuery = LikeQuery;

                search.ID = ID;
                search.Description = LineName;
                search.StatusID = StatusID;
                search.StatusValue = StatusValue;
                search.LineTypeDefID = LineTypeDefID;
                search.LineTypeDefValue = LineTypeDefValue;
                search.Content = PropertyContent;

                PageResult<SC_mesLineALLDto> list = await iService.FindWithPagerSearchAsync(search);

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

                Log4NetHelper.Error("获取 mesline FindWithPagerSearchAsync 异常", ex);
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
        /// <param name="v_DetailDto"></param>
        /// <returns></returns>
        [HttpPost("InsertDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> InsertDetail(string ParentId, SC_mesLineDetailDto v_DetailDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IDbTransaction trans = null;
                string list = await iService.InsertDetail(ParentId,v_DetailDto, trans);

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
                Log4NetHelper.Error("获取 InsertDetail 异常", ex);
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
                Log4NetHelper.Error("获取 DeleteDetail 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        /// <summary>
        /// UpdateDetail
        /// </summary>
        /// <param name="v_DetailDto"></param>
        /// <returns></returns>
        [HttpPost("UpdateDetail")]
        [YuebonAuthorize("")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDetail(SC_mesLineDetailDto v_DetailDto)
        {
            CommonResult v_ComResult = new CommonResult();
            try
            {
                //if (P_EmployeeID == 0) { P_EmployeeID = 1; }
                //if (string.IsNullOrEmpty(P_CurrentLoginIP)) { P_CurrentLoginIP = "dev127"; }

                List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

                IDbTransaction trans = null;
                string list = await iService.UpdateDetail(v_DetailDto, trans);

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
                Log4NetHelper.Error("获取 UpdateDetail 异常", ex);
                v_ComResult.ResultMsg = ErrCode.err40110;
                v_ComResult.ResultCode = "40110";
            }
            return ToJsonContent(v_ComResult);
        }

        ///// <summary>
        ///// List_Detail
        ///// </summary>
        ///// <param name="ParentId"></param>
        ///// <returns></returns>
        //[HttpPost("List_Detail")]
        //[YuebonAuthorize("")]
        //[AllowAnonymous]
        //public async Task<IActionResult> List_Detail(string ParentId)
        //{            
        //    CommonResult v_ComResult = new CommonResult();
        //    try
        //    {
        //        List<TabVal> List_ConfInfo = await iService.GetConfInfo(P_Language, P_LineID, P_StationID, P_EmployeeID, P_CurrentLoginIP);

        //        SC_mesLineDetailList list = await iService.List_Detail(ParentId);

        //        if (list.MSG == "OK")
        //        {
        //            v_ComResult.Success = true;
        //            v_ComResult.ResultCode = ErrCode.successCode;
        //            v_ComResult.ResultMsg = ErrCode.err0;
        //            v_ComResult.ResData = list.List_esLineDetail;
        //            v_ComResult.Sounds = S_Path_OK;
        //        }
        //        else
        //        {
        //            v_ComResult.Success = false;
        //            v_ComResult.ResultCode = "NG";
        //            v_ComResult.ResultMsg = list.MSG;
        //            v_ComResult.ResData = null;
        //            v_ComResult.Sounds = S_Path_NG;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取 List_Detail 异常", ex);
        //        v_ComResult.ResultMsg = ErrCode.err40110;
        //        v_ComResult.ResultCode = "40110";
        //    }
        //    return ToJsonContent(v_ComResult);
        //}

    }
}