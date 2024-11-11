using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.MSGCode;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.WebApi.SysConfig.Part
{
    /// <summary>
    /// BOM
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_mesProductStructureController : AreaApiControllerCustom<SC_mesProductStructure, SC_mesProductStructure, SC_mesProductStructure, ISC_mesProductStructureServices, string>
    {
        public SC_mesProductStructureController(ISC_mesProductStructureServices iServices): base(iServices)
        {
            
        }


        /// <summary>
        /// 异步更新数据，需要在业务模块控制器重写该方法,否则更新无效
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesProductStructure inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());

            if (beforData.PartID != inInfo.PartID  || beforData.ParentPartID != inInfo.ParentPartID || beforData.StationTypeID != inInfo.StationTypeID)
                commonResult = await base.UpdateBeforeCheckAsync(inInfo);

            if (!string.IsNullOrEmpty(commonResult.ResultMsg))
                return ToJsonContent(commonResult);

            await FormatUpdateMsg(beforData, inInfo);
            OnBeforeUpdate(inInfo);

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
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchBOMInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithPagerSearchAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> InsertAsync(SC_mesProductStructure tinfo)
        {
            var IsExists = await iService.GetWhereAsync($"ParentPartID = {tinfo.ParentPartID} AND PartID = {tinfo.PartID} AND StationTypeID = {tinfo.StationTypeID}");
            if (IsExists is not null)
            {
                return ToJsonContent(new CommonResult()
                {
                    ResultMsg = " Data already exists.",
                    Success = false,
                    ResultCode = ErrCode.err1,
                    Sounds = S_Path_NG,
                });
            }
            return await base.InsertAsync(tinfo);
        }
    }
}
