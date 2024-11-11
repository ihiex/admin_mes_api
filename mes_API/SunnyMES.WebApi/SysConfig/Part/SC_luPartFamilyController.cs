using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// 料号组
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_luPartFamilyController : AreaApiControllerCustom<SC_luPartFamily, SC_luPartFamily, SC_luPartFamily, ISC_luPartFamilyServices, string>
    {
        public SC_luPartFamilyController(ISC_luPartFamilyServices _iService) : base(_iService)
        {

        }
        /// <summary>
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchPartFamilyInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerSearchAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_luPartFamily inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            if (beforData.PartFamilyTypeID != inInfo.PartFamilyTypeID)
                commonResult = await base.UpdateBeforeCheckAsync(inInfo);

            if (!string.IsNullOrEmpty(commonResult.ResultMsg))
                return ToJsonContent(commonResult);
            OnBeforeUpdate(inInfo);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 根据父ID查询所有页  子类必须重写，否则无效
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpGet("FindPagerByParentAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> FindPagerByParentAsync(int ParentID)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.GetListWhereAsync($" PartFamilyTypeID = {ParentID}");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 异步物理删除
        /// </summary>
        /// <param name="id">主键Id</param>
        [HttpDelete("DeleteAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> DeleteAsync(SC_luPartFamily info)
        {
            CommonResult result = new CommonResult();
            iService.SetCommonHeader(commonHeader);
            var cr = await iService.DeleteDataAsync(info);
            if (string.IsNullOrEmpty(cr.ResultMsg))
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                OnBeforeDelete(info);
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
        /// 克隆项
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("CloneAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> CloneAsync(SC_luPartFamily info)
        {
            CommonResult result = new CommonResult();
            iService.SetCommonHeader(commonHeader);
            var sr = await iService.CloneDataAsync(info).ConfigureAwait(false);

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
    }
}
