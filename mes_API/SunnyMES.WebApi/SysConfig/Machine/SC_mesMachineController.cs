using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Threading.Tasks;
using Yitter.IdGenerator;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IServices.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.ToolExtensions;
using Microsoft.Extensions.Internal;

namespace SunnyMES.WebApi.SysConfig.Machine
{
    /// <summary>
    /// 设备资料
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesMachineController : AreaApiControllerCustom<SC_mesMachine, SC_mesMachine, SC_mesMachine, ISC_mesMachineServices, string>
    {
        public SC_mesMachineController(ISC_mesMachineServices services) : base(services)
        {

        }

        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertAsync")]
        [YuebonAuthorize("")]
        public override async Task<IActionResult> InsertAsync(SC_mesMachine tinfo)
        {
            CommonResult result = new CommonResult();
            SC_mesMachine info = tinfo.MapTo<SC_mesMachine>();
            info.StartRuningTime = null;
            info.LastRuningTime = null;
            string tmpWhere = OutputExtensions.FormartWhere<SC_mesMachine>(info, ExistsWhere, PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }
            
            if (tinfo?.ParentID > 0)
            {
                if (tinfo.ParentID == tinfo.ID)
                {
                    result.ResultMsg = ShowMsg(ErrCode.err70003);
                    result.ResultCode = ErrCode.err1;
                    result.Sounds = S_Path_NG;
                    result.Success = false;
                    return ToJsonContent(result);
                }

                ExistsWhere.Add("ParentID");
                string tmpParentWhere = OutputExtensions.FormartWhere<SC_mesMachine>(info, ExistsWhere, PrimaryKeyName);
                var IsParentExists = await iService.GetWhereAsync(tmpParentWhere);
                if (IsParentExists is not null)
                {
                    result.ResultMsg = ShowMsg(ErrCode.err70003);
                    result.ResultCode = ErrCode.err1;
                    result.Sounds = S_Path_NG;
                    result.Success = false;
                    return ToJsonContent(result);
                }
            }

            OnBeforeInsert(info);
            long ln = await iService.InsertAsync(info).ConfigureAwait(false);
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
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchMachineInputDto search)
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
        public override async  Task<IActionResult> UpdateAsync(SC_mesMachine inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            if (beforData is null)
            {
                commonResult.ResultMsg = ErrCode.err70002;
                commonResult.ResultCode = "70002";
                commonResult.Sounds = S_Path_NG;
                commonResult.Success = false;
                return ToJsonContent(commonResult);
            }

            string tmpWhere = OutputExtensions.FormartWhere<SC_mesMachine>(inInfo,  PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                commonResult.ResultMsg = ErrCode.err70001;
                commonResult.ResultCode = ErrCode.err1;
                commonResult.Sounds = S_Path_NG;
                commonResult.Success = false;
                return ToJsonContent(commonResult);
            }
            if (inInfo?.ParentID > 0)
            {
                if (inInfo.ParentID == inInfo.ID)
                {
                    commonResult.ResultMsg = ShowMsg(ErrCode.err70003);
                    commonResult.ResultCode = ErrCode.err1;
                    commonResult.Sounds = S_Path_NG;
                    commonResult.Success = false;
                    return ToJsonContent(commonResult);
                }


                ExistsWhere.Add("ParentID");
                string tmpParentWhere = OutputExtensions.FormartWhere<SC_mesMachine>(inInfo, ExistsWhere, PrimaryKeyName);
                var IsParentExists = await iService.GetWhereAsync(tmpParentWhere);
                if (IsParentExists is not null)
                {
                    commonResult.ResultMsg = ShowMsg(ErrCode.err70003);
                    commonResult.ResultCode = ErrCode.err1;
                    commonResult.Sounds = S_Path_NG;
                    commonResult.Success = false;
                    return ToJsonContent(commonResult);
                }
            }

            OnBeforeUpdate(inInfo);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
        /// <summary>
        /// 获取警告状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetWarningState")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetWarningState()
        {
            CommonResult commonResult = new CommonResult();
            var vList = ExtEnum.EnumToList(typeof(WarningStatus), e => P_Language == 0 ? e.ToString() : e.GetDescription());
            commonResult = base.FormatOKResult(commonResult, vList);

            return ToJsonContent(commonResult);
        }
    }
}
