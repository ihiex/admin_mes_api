using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
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

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 角色接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class RoleController : AreaApiController<Role, RoleOutputDto, RoleInputDto, IRoleService, string>
    {
        private IOrganizeService organizeService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="_organizeService"></param>
        public RoleController(IRoleService _iService, IOrganizeService _organizeService) : base(_iService)
        {
            iService = _iService;
            organizeService = _organizeService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(Role info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
            info.DeleteMark = false;
            info.Category = 1;
            if (info.SortCode == null)
            {
                info.SortCode = 99;
            }
        }
        
        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeUpdate(Role info)
        {
            info.LastModifyUserId = CurrentUser.UserId;
            info.LastModifyTime = DateTime.Now;
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(Role info)
        {
            info.DeleteMark = true;
            info.DeleteTime = DateTime.Now;
            info.DeleteUserId = CurrentUser.UserId;
        }

        /// <summary>
        /// 异步更新数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        [YuebonAuthorize("Edit")]
        public override async Task<IActionResult> UpdateAsync(RoleInputDto tinfo)
        {
            CommonResult result = new CommonResult();

            Role info = iService.Get(tinfo.Id);
            info.OrganizeId = tinfo.OrganizeId;
            info.FullName = tinfo.FullName;
            info.EnCode = tinfo.EnCode;
            info.EnabledMark = tinfo.EnabledMark;
            info.SortCode = tinfo.SortCode;
            info.Description = tinfo.Description;
            info.Type = tinfo.Type;

            OnBeforeUpdate(info);
            bool bl = await iService.UpdateAsync(info, tinfo.Id).ConfigureAwait(false);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;

                PsysLog.SetControllerLog(" Role Update", P_CurrentLoginIP, P_EmployeeID.ToString());
            }
            else
            {
                result.ResultMsg = ErrCode.err43002;
                result.ResultCode = "43002";
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 克隆角色
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost("Clone")]
        [YuebonAuthorize("")]
        public  async Task<IActionResult> Clone(Role entity)
        {
            IDbTransaction trans = null;

            CommonResult result = new CommonResult();            ;

            //info.Id = GuidUtils.CreateNo();
            entity.CreatorTime = DateTime.Now;
            entity.CreatorUserId = CurrentUser.UserId;
            entity.DeleteMark = false;
            entity.Category = 1;
            if (entity.SortCode == null)
            {
                entity.SortCode = 99;
            }

            string S_Result = await iService.Clone(entity, trans);
            if (S_Result=="OK")
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;

                PsysLog.SetControllerLog("API_Role Clone", P_CurrentLoginIP, P_EmployeeID.ToString());
            }
            else
            {
                result.ResultMsg = ErrCode.err43001;
                result.ResultCode = "43001";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 获取所有可用的
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllEnable")]
        [YuebonAuthorize("")]
        public override async Task<CommonResult<List<RoleOutputDto>>> GetAllEnable()
        {
            CommonResult<List<RoleOutputDto>> result = new CommonResult<List<RoleOutputDto>>();
            IEnumerable<Role> list = await iService.GetAllByIsNotDeleteAndEnabledMarkAsync();
            List<Role> List_Result=new List<Role>();
            
            string S_Role = "";
            if (CurrentUser != null) { S_Role = CurrentUser.Role; }
            if (S_Role.IndexOf("administrators") < 0)
            {
                foreach (var item in list)
                {
                    if (item.EnCode != "administrators")
                    {
                        List_Result.Add(item);
                    }
                }
            }
            else
            {
                foreach (var item in list)
                {
                    List_Result.Add(item);
                }
            }

            List<RoleOutputDto> resultList = List_Result.MapTo<RoleOutputDto>();
            result.ResData = resultList;
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            return result;
        }
    }
}