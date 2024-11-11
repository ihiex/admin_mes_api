using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.IServices;
using SunnyMES.Security.SysConfig.Dtos.PO;
using SunnyMES.Security.SysConfig.IServices.PO;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.WebApi.SysConfig.PO
{
    /// <summary>
    ///工单
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]

    public class SC_mesProductionOrderController : AreaApiControllerCustom<SC_mesProductionOrder, SC_mesProductionOrder, SC_mesProductionOrder, ISC_mesProductionOrderServices, string>
    {
        private readonly ISC_mesProductionOrderServices productionOrderServices;
        private readonly IPublicSCService publicSCService;
        private readonly ISC_mesLineOrderServices lineServices;

        public SC_mesProductionOrderController(ISC_mesProductionOrderServices productionOrderServices, IPublicSCService publicSCService, ISC_mesLineOrderServices lineServices) : base(productionOrderServices)
        {
            this.productionOrderServices = productionOrderServices;
            this.publicSCService = publicSCService;
            this.lineServices = lineServices;
            base.ExistsWhere.Add("ProductionOrderNumber");
        }

        /// <summary>
        /// 分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchAsync(SearchPOInputDto search)
        {
            CommonResult commonResult = new CommonResult();

            //var tmpData = await iService.FindWithPagerSearchAsync(search);
            var tmpData = await iService.FindWithLinePagerSearchAsync(search);
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
        /// <summary>
        /// PMC分页模糊查询
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerSearchPMCAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> FindWithPagerSearchPMCAsync(SearchPOInputDto search)
        {
            CommonResult commonResult = new CommonResult();
            var tmpData = await iService.FindWithLinePagerSearchPMCAsync(search);
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
        public override async  Task<IActionResult> InsertAsync(SC_mesProductionOrder tinfo)
        {
            tinfo.CreationTime = DateTime.Now;
            tinfo.EmployeeID = commonHeader.EmployeeId;
            return await base.InsertAsync(tinfo);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("Mytest")]
        //[Obsolete]
        public IActionResult Mytest()
        {
            Console.WriteLine(ExtPublicFunc.GetPropertyName((() => ErrCode.err0))); 
            return ToJsonContent(ShowMsg(ErrCode.err1));
        }
        /// <summary>
        /// PMC异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertPMCAsync")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> InsertPMCAsync(PMCPOInsertInputDto tinfo)
        {
            CommonResult result = new CommonResult();
            SC_mesProductionOrder info = tinfo.MapTo<SC_mesProductionOrder>();
            string tmpWhere = OutputExtensions.FormartWhere<SC_mesProductionOrder>(info, ExistsWhere, PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ErrCode.err70001;
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }
            tinfo.CreationTime = DateTime.Now;
            tinfo.EmployeeID = commonHeader.EmployeeId;
            tinfo.StatusID = 1;
            var sr = await productionOrderServices.InsertPMCAsync(tinfo).ConfigureAwait(false);

            if (string.IsNullOrEmpty(sr.ResultMsg))
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                OnBeforeInsert(info);
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
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> UpdateAsync(SC_mesProductionOrder inInfo)
        {
            CommonResult commonResult = new CommonResult();
            OnBeforeUpdate(inInfo);
            var beforData = await iService.GetAsync(inInfo.ID.ToString());
            inInfo.LastUpdate = DateTime.Now;
            await FormatUpdateMsg(beforData, inInfo);
            var tmpData = await iService.UpdateAsync(inInfo, "ID");
            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }


        /// <summary>
        /// 异步物理删除
        /// </summary>
        /// <param name="id">主键Id</param>
        [HttpDelete("DeleteAsync")]
        [YuebonAuthorize("")]
        public override async  Task<IActionResult> DeleteAsync(SC_mesProductionOrder info)
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
        public override async  Task<IActionResult> CloneAsync(SC_mesProductionOrder info)
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
            var poL = await lineServices.GetListWhereAsync($" ProductionOrderID = {ParentID}");
            var Lines = await publicSCService.GetmesLine("");
            var po = await iService.GetWhereAsync($" ID = {ParentID}");
            var tmpData = from r in poL.ToList()
                          join l in Lines on r.LineID.ToString() equals l.Id into poLine
                          from ab in poLine
                          select new SearchPOLineOutputDto
                          {
                              ID = r.ID,
                              POLineDesc = r.Description,
                              ProductionOrderID = r.ID,
                              POName = po.ProductionOrderNumber,
                              PODesc = po.Description,
                              LineID = r.LineID.ToInt(),
                              LineName = ab.Description,
                              LineQuantity = r.LineQuantity.ToInt(),
                              StartedQuantity = r.StartedQuantity.ToInt(),
                              ReadyQuantity = r.ReadyQuantity.ToInt(),
                              AllowOverBuild = r.AllowOverBuild.ToInt(),
                              Priority = r.Priority.ToInt(),
                              CreationTime = r.CreationTime ?? DateTime.MinValue
                          };

            commonResult = base.FormatOKResult(commonResult, tmpData);
            return ToJsonContent(commonResult);
        }
    }
}
