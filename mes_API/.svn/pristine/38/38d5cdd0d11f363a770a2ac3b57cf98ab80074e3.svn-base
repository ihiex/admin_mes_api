using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Models;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.WebApi.SysConfig.PO
{
    /// <summary>
    ///工单详细属性定义
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class SC_luProductionOrderDetailDefController : AreaApiControllerCustom<SC_luProductionOrderDetailDef, SC_luProductionOrderDetailDef, SC_luProductionOrderDetailDef, ISC_luProductionOrderDetailDefServices, string>
    {
        public SC_luProductionOrderDetailDefController(ISC_luProductionOrderDetailDefServices services) : base(services)
        {
            
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_luProductionOrderDetailDef inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            var isExists = await base.IsExistsT(inInfo);
            if (isExists)
                return ToJsonContent(base.FormatNGResult(commonResult, ShowMsg(ErrCode.err70001)));

            OnBeforeUpdate(inInfo);
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }

        /// <summary>
        /// 获取检查类型
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCheckType")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetCheckType()
        {
            CommonResult commonResult = new CommonResult();
            var vList = ExtEnum.EnumToList(typeof(PoDefCheckType), e => P_Language == 0 ? e.ToString() : e.GetDescription());
            commonResult = base.FormatOKResult(commonResult, vList);

            return ToJsonContent(commonResult);
        }
    }
}
