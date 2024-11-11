using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.SecurityApi.Areas.Security.Controllers
{
    /// <summary>
    /// 序号编码规则表接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class SequenceRuleController : AreaApiController<SequenceRule, SequenceRuleOutputDto, SequenceRuleInputDto, ISequenceRuleService,string>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public SequenceRuleController(ISequenceRuleService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(SequenceRule info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.CompanyId = CurrentUser.OrganizeId;
            info.DeptId = CurrentUser.DeptId;
            info.DeleteMark = false;
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(SequenceRule info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(SequenceRule info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }

        /// <summary>
        /// 异步新增或修改数据
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>

        [HttpPost("InsertOrUpdateAsync")]
        [YuebonAuthorize("Add")]
        public async Task<IActionResult> InsertOrUpdateAsync(SequenceRuleInputDto info)
        {
            CommonResult result = new CommonResult();

            if (string.IsNullOrEmpty(info.SequenceName))
            {
                result.ResultMsg = "单据名称不能为空";
                return ToJsonContent(result);
            }

            if (string.IsNullOrEmpty(info.Id))
            {
                string where = string.Format("RuleType='{0}' and SequenceName='{1}'", info.RuleType, info.SequenceName);
                SequenceRule goodsIsExist = iService.GetWhere(where);
                if (goodsIsExist != null)
                {
                    result.ResultMsg = "该单据此规则类别已添加不能重复";
                    return ToJsonContent(result);
                }
                SequenceRule sequence = new SequenceRule();
                sequence = info.MapTo<SequenceRule>();
                if (!string.IsNullOrEmpty(info.PaddingChar))
                {
                    sequence.PaddingChar = info.PaddingChar.ToCharArray()[0].ToString();
                }
                OnBeforeInsert(sequence);
                long ln = await iService.InsertAsync(sequence).ConfigureAwait(true);
                result.Success = ln > 0;
            }
            else
            {
                string where = string.Format("RuleType='{0}' and id!='{1}' and SequenceName='{2}'", info.RuleType, info.Id, info.SequenceName);
                SequenceRule goodsIsExist = iService.GetWhere(where);
                if (goodsIsExist != null)
                {
                    result.ResultMsg = "该单据此规则类别已添加不能重复";
                    return ToJsonContent(result);
                }
                SequenceRule sequenceRule = iService.Get(info.Id);
                sequenceRule.SequenceName = info.SequenceName;
                sequenceRule.RuleOrder = info.RuleOrder;
                sequenceRule.RuleType = info.RuleType;
                sequenceRule.RuleValue = info.RuleValue;
                sequenceRule.PaddingSide = info.PaddingSide;
                sequenceRule.PaddingWidth = info.PaddingWidth;
                if (!string.IsNullOrEmpty(info.PaddingChar))
                {
                    sequenceRule.PaddingChar = info.PaddingChar.ToCharArray()[0].ToString();
                }
                sequenceRule.EnabledMark = info.EnabledMark;
                sequenceRule.Description = info.Description;
                OnBeforeUpdate(sequenceRule);
                result.Success = await iService.UpdateAsync(sequenceRule, info.Id).ConfigureAwait(true);
            }
            if (result.Success)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
            }
            else
            {
                result.Success = false;
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
            }
            return ToJsonContent(result);
        }
    }
}