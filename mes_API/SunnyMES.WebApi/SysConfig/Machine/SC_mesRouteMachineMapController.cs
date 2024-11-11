﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Machine;
using SunnyMES.Security.SysConfig.IServices.Machine;
using SunnyMES.Security.SysConfig.Models.Machine;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.WebApi.SysConfig.Machine
{
    /// <summary>
    /// 设备分配
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesRouteMachineMapController : AreaApiControllerCustom<SC_mesRouteMachineMap, SC_mesRouteMachineMap, SC_mesRouteMachineMap, ISC_mesRouteMachineMapServices, string>
    {
        public SC_mesRouteMachineMapController(ISC_mesRouteMachineMapServices services) : base(services)
        {
            
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesRouteMachineMap inInfo)
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

            string tmpWhere = OutputExtensions.FormartWhere<SC_mesRouteMachineMap>(inInfo,  PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                commonResult.ResultMsg = ErrCode.err70001;
                commonResult.ResultCode = ErrCode.err1;
                commonResult.Sounds = S_Path_NG;
                commonResult.Success = false;
                return ToJsonContent(commonResult);
            }


            OnBeforeUpdate(inInfo);

            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchMachineMapInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerSearchAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

    }
}