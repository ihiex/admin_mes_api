using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig._5_IServices.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using SunnyMES.Security.ToolExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SunnyMES.WebApi.SysConfig.Shift
{
    /// <summary>
    /// 班次
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesShiftDetailController : AreaApiControllerCustom<SC_mesShiftDetail, SC_mesShiftDetail, SC_mesShiftDetail, ISC_mesShiftDetailServices, int>
    {
        public SC_mesShiftDetailController(ISC_mesShiftDetailServices _iService) : base(_iService)
        {
        }

        /// <summary>
        /// 异步插入
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        public override async Task<IActionResult> InsertAsync(SC_mesShiftDetail tinfo)
        {
            CommonResult result = new CommonResult();
            if (tinfo.StartTime == tinfo.EndTime) {
                result.ResultMsg = ShowMsg(ErrCode.err70004);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }


            string tmpWhere = OutputExtensions.FormartWhere<SC_mesShiftDetail>(tinfo, base.ExistsWhere, base.PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            var tmpShiftDetails = await iService.GetListWhereAsync($"ShiftCodeID = {tinfo.ShiftCodeID}");
            if (tmpShiftDetails.Any())
            {
                var tmpNGDetails = tmpShiftDetails.Where(x => (tinfo.StartTime > x.StartTime && tinfo.StartTime < x.EndTime) || (tinfo.EndTime > x.StartTime && tinfo.EndTime < x.EndTime));
                if (tmpNGDetails.Any()) {
                    result.ResultMsg = ShowMsg(ErrCode.err70005);
                    result.ResultCode = ErrCode.err1;
                    result.Sounds = S_Path_NG;
                    result.Success = false;
                    return ToJsonContent(result);
                }
            }

            tinfo.State = true;
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
        public override async Task<IActionResult> UpdateAsync(SC_mesShiftDetail inInfo)
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
        /// 查询所有启用
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllEnable")]
        public override async Task<CommonResult<List<SC_mesShiftDetail>>> GetAllEnable()
        {
            CommonResult<List<SC_mesShiftDetail>> result = new CommonResult<List<SC_mesShiftDetail>>();
            IEnumerable<SC_mesShiftDetail> list = await iService.GetListWhereAsync("State = 1");
            result.ResData = list;
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }
    }
}
