using API_MSG;
using Microsoft.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig._2_Dtos.Shift;
using SunnyMES.Security.SysConfig._5_IServices.Shift;
using SunnyMES.Security.SysConfig.Dtos.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using SunnyMES.Security.ToolExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnyMES.WebApi.SysConfig.Shift
{
    /// <summary>
    /// 班次，对应的UPH数据设定
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesShiftUPHController : AreaApiControllerCustom<SC_mesShiftUPH, SC_mesShiftUPH, SC_mesShiftUPH, ISC_mesShiftUPHServices, int>
    {

        public SC_mesShiftUPHController(ISC_mesShiftUPHServices _iService) : base(_iService)
        {

        }
        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        public override async Task<IActionResult> InsertAsync(SC_mesShiftUPH tinfo)
        {
            tinfo.CreateTime = System.DateTime.Now;
            tinfo.UpdateTime = System.DateTime.Now;

            CommonResult result = new CommonResult();

            string tmpWhere = OutputExtensions.FormartWhere<SC_mesShiftUPH>(tinfo, base.ExistsWhere, base.PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            tinfo.State = 1;
            OnBeforeInsert(tinfo);
            long ln = await iService.InsertAsync(tinfo).ConfigureAwait(false);
            if (ln > 0)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        public override async Task<IActionResult> UpdateAsync(SC_mesShiftUPH inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var isExists = await base.IsExistsT(inInfo);
            if (isExists)
                return ToJsonContent(base.FormatNGResult(commonResult, ShowMsg(ErrCode.err70001)));

            OnBeforeUpdate(inInfo);
            var beforData = await iService.GetAsync(inInfo.ID);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo,inInfo.ID);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 批量插入日期对应的班次及线别
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertBulkAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> InsertBulkAsync(MesShiftUPHInputDto tinfo)
        {
            iService.SetCommonHeader(commonHeader);
            CommonResult result = new CommonResult();
            if (tinfo.StartTime > tinfo.EndTime)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70005);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            string ln = await iService.InsertBulkAsync(tinfo).ConfigureAwait(false);
            if (ln  == "1")
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                result.ResData = ln;
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }
        /// <summary>
        /// 批量删除日期对应的班次及线别
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("DeleteBulkAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> DeleteBulkAsync(MesShiftUPHBulkDeleteDto tinfo)
        {
            iService.SetCommonHeader(commonHeader);
            CommonResult result = new CommonResult();
            if (tinfo.StartTime > tinfo.EndTime)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70005);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            //string ln = await iService.DeleteBulkAsync(tinfo).ConfigureAwait(false);
            var wl = await iService.DeleteBatchWhereAsync($" ShiftDate >= '{tinfo.StartTime.ToString("yyyy-MM-dd")}' AND ShiftDate <= '{tinfo.EndTime.ToString("yyyy-MM-dd")}'");
            
            if (wl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                result.ResData = "delete failed";
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 查询所有页
        /// </summary>
        /// <param name="Sortfield">排序字段</param>
        /// <param name="IsAsc">是否升序</param>
        /// <param name="IsEnabled">        /// 默认为0，查询所有，为1时，查询启用的，为-1时查询不启用</param>
        /// <returns></returns>
        [HttpGet("FindWithAllPagerFilterAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithAllPagerFilterAsync(string Sortfield = "", bool IsAsc = true, int IsEnabled = 0)
        {
            CommonResult result = new CommonResult();

            var shiftUPHs = await iService.FindWithAllPagerAsync(new Commons.Page.PageCustomInfo() { IsAsc = IsAsc,Sortfield = Sortfield, IsEnabled = IsEnabled});

            if (shiftUPHs.Any())
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                result.ResData = shiftUPHs;
            }
            else
            {
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }
    }
}
