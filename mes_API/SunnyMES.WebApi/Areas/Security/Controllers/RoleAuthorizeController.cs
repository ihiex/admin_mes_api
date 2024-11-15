using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;

namespace SunnyMES.WebApi.Areas.Security.Controllers
{
    /// <summary>
    /// 角色权限接口
    /// </summary>
    [ApiController]
    [Route("api/Security/[controller]")]
    public class RoleAuthorizeController : AreaApiController<RoleAuthorize, RoleAuthorizeOutputDto, RoleAuthorizeInputDto, IRoleAuthorizeService,string>
    {
        private readonly IMenuService menuService;
        private readonly IRoleDataService roleDataService;
        private readonly IOrganizeService organizeService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="_menuService"></param>
        /// <param name="_roleDataService"></param>
        /// <param name="_organizeService"></param>
        public RoleAuthorizeController(IRoleAuthorizeService _iService, IMenuService _menuService, IRoleDataService _roleDataService, IOrganizeService _organizeService) : base(_iService)
        {
            iService = _iService;
            menuService = _menuService;
            roleDataService = _roleDataService;
            organizeService = _organizeService;
        }
        /// <summary>
        /// 新增前处理数据
        /// </summary>
        /// <param name="info"></param>
        protected override void OnBeforeInsert(RoleAuthorize info)
        {
            info.Id = GuidUtils.CreateNo();
            info.CreatorTime = DateTime.Now;
            info.CreatorUserId = CurrentUser.UserId;
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
        protected override void OnBeforeUpdate(RoleAuthorize info)
        {
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected override void OnBeforeSoftDelete(RoleAuthorize info)
        {
        }

        /// <summary>
        /// 角色分配权限树
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="itemType"></param>
        /// <returns></returns>
        [HttpGet("GetRoleAuthorizeFunction")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> GetRoleAuthorizeFunction(string roleId, string itemType)
        {
            CommonResult result = new CommonResult();
            roleId = "'" + roleId + "'";
            List<string> resultlist = new List<string>();
            IEnumerable<RoleAuthorize> list= iService.GetListRoleAuthorizeByRoleId(roleId, itemType);
            foreach(RoleAuthorize info in list)
            {
                resultlist.Add(info.ItemId);
            }

            PsysLog.SetControllerLog(" Role Authorize", P_CurrentLoginIP, P_EmployeeID.ToString());

            result.ResData = resultlist;
            result.ResultCode = ErrCode.successCode;
            return ToJsonContent(result);
        }


        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="roleinfo">功能权限</param>
        /// <returns></returns>
        [HttpPost("SaveRoleAuthorize")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> SaveRoleAuthorize(RoleAuthorizeDataInputDto roleinfo)
        {
            CommonResult result = new CommonResult();
            try
            {                
                List<RoleAuthorize> inList = new List<RoleAuthorize>();
                foreach (string item in roleinfo.RoleFunctios)
                {
                    Menu menu = menuService.Get(item);
                    if (menu != null)
                    {
                        RoleAuthorize info = new RoleAuthorize();
                        info.ObjectId = roleinfo.RoleId;
                        info.ItemType = (menu.MenuType == "C" || menu.MenuType == "M") ? 1 : 2;
                        info.ObjectType = 1;
                        info.ItemId = menu.Id;
                        OnBeforeInsert(info);
                        inList.Add(info);
                    }
                }

                List<RoleData> roleDataList = new List<RoleData>();
                foreach (string item in roleinfo.RoleData)
                {
                    RoleData info = new RoleData();
                    info.RoleId = roleinfo.RoleId;
                    info.AuthorizeData = item;
                    info.DType = "dept";
                    roleDataList.Add(info);
                }
                foreach (string item in roleinfo.RoleSystem)
                {
                    RoleAuthorize info = new RoleAuthorize();
                    info.ObjectId = roleinfo.RoleId;
                    info.ItemType = 0;
                    info.ObjectType = 1;
                    info.ItemId = item;
                    OnBeforeInsert(info);
                    inList.Add(info);
                }
                result.Success = await iService.SaveRoleAuthorize(roleinfo.RoleId,inList, roleDataList);
                if (result.Success)
                {
                    result.ResultCode = ErrCode.successCode;

                    PsysLog.SetControllerLog(" RoleAuthorize Save", P_CurrentLoginIP, P_EmployeeID.ToString());
                }
            }
            catch (Exception ex)
            {
                result.ResultMsg = ex.Message;
            }
            return ToJsonContent(result);
        }

        private List<RoleAuthorize> SubFunction(List<ModuleFunctionOutputDto> list, string roleId)
        {
            List<RoleAuthorize> inList = new List<RoleAuthorize>();
            foreach (ModuleFunctionOutputDto item in list)
            {
                RoleAuthorize info = new RoleAuthorize();
                info.ObjectId = roleId;
                info.ItemType = 1;
                info.ObjectType = 1;
                info.ItemId = item.Id.ToString();
                OnBeforeInsert(info);
                inList.Add(info);
                inList.Concat(SubFunction(item.Children, roleId));
            }
            return inList;
        }

        /// <summary>
        /// 获取功能菜单适用于Vue Tree树形
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllFunctionTree")]
        [YuebonAuthorize("List")]
        public async Task<IActionResult> GetAllFunctionTree()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<ModuleFunctionOutputDto> list = await iService.GetAllFunctionTree();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取菜单异常", ex);
                result.ResultMsg = ErrCode.err40110;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }
        /// <summary>
        /// 获取功能菜单适用于Vue 树形列表
        /// </summary>
        /// <param name="systemTypeId">子系统Id</param>
        /// <returns></returns>
        //[HttpGet("GetAllFunctionTreeTable")]
        //[YuebonAuthorize("List")]
        //public async Task<IActionResult> GetAllFunctionTreeTable(string systemTypeId)
        //{
        //    CommonResult result = new CommonResult();
        //    try
        //    {
        //        List<FunctionTreeTableOutputDto> list = await menuService.GetAllFunctionTreeTable(systemTypeId);
        //        result.Success = true;
        //        result.ErrCode = ErrCode.successCode;
        //        result.ResData = list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log4NetHelper.Error("获取菜单异常", ex);
        //        result.ErrMsg = ErrCode.err40110;
        //        result.ErrCode = "40110";
        //    }
        //    return ToJsonContent(result);
        //}
    }
}