using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig._2_Dtos.Shift;
using SunnyMES.Security.SysConfig._5_IServices.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using SunnyMES.Security.ToolExtensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SunnyMES.WebApi.SysConfig.Shift
{
    /// <summary>
    /// 班次信息
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesShiftController : AreaApiControllerCustom<SC_mesShift, SC_mesShift, SC_mesShift, ISC_mesShiftServices, int>
    {
        public SC_mesShiftController(ISC_mesShiftServices _iService) : base(_iService)
        {

        }

        /// <summary>
        /// 异步更新
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        public override async Task<IActionResult> UpdateAsync(SC_mesShift inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var isExists = await base.IsExistsT(inInfo);
            if (isExists)
                return ToJsonContent(base.FormatNGResult(commonResult, ShowMsg(ErrCode.err70001)));

            OnBeforeUpdate(inInfo);
            var beforData = await iService.GetAsync(inInfo.ID);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, inInfo.ID);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        public override Task<IActionResult> InsertAsync(SC_mesShift tinfo)
        {
            tinfo.State = true;
            return base.InsertAsync(tinfo);
        }

        /// <summary>
        /// 获取所有可用
        /// </summary>
        /// <returns></returns>
        public override async Task<CommonResult<List<SC_mesShift>>> GetAllEnable()
        {
            CommonResult<List<SC_mesShift>> result = new CommonResult<List<SC_mesShift>>();
            IEnumerable<SC_mesShift> list = await iService.GetListWhereAsync("State = 1");
            result.ResData = list;
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }

        
        public override async Task<IActionResult> CloneAsync(SC_mesShift inInfo)
        {
            CommonResult result = new CommonResult();
            iService.SetCommonHeader(commonHeader);
            var sr = await iService.CloneDataAsync(inInfo).ConfigureAwait(false);

            if (string.IsNullOrEmpty(sr.ResultMsg))
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                result.ResultMsg = sr.ResultMsg;
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }
        public override async Task<IActionResult> DeleteAsync(SC_mesShift inInfo)
        {
            CommonResult result = new CommonResult();
            iService.SetCommonHeader(commonHeader);
            var cr = await iService.DeleteDataAsync(inInfo);
            if (string.IsNullOrEmpty(cr.ResultMsg))
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                OnBeforeDelete(inInfo);
            }
            else
            {
                result.ResultMsg = ErrCode.err43003;
                result.ResultCode = "43003";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 分页过滤查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithAllPagerFilterAsync")]
        [YuebonAuthorize("")]
        public async Task<CommonResult<PageResult<SC_mesShift>>> FindWithPagerFilterAsync(MesShiftInputDto search)
        {
            CommonResult<PageResult<SC_mesShift>> result = new CommonResult<PageResult<SC_mesShift>>();
            result.ResData = await iService.FindWithPagerFilterAsync(search);
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }
    }
}
