using log4net.Util;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Page;
using SunnyMES.Commons.Pages;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Dtos.MES;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Security.Repositories;
using SunnyMES.Security.Services;
using SunnyMES.Security.SysConfig.Models.Part;
using SunnyMES.Security.SysConfig.Models.PO;
using SunnyMES.Security.ToolExtensions;

namespace SunnyMES.AspNetCore.Controllers
{
    /// <summary>
    /// 基本控制器，增删改查
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    /// <typeparam name="TODto">数据输出实体类型</typeparam>
    /// <typeparam name="TIDto">数据输入实体类型</typeparam>
    /// <typeparam name="TService">Service类型</typeparam>
    /// <typeparam name="TKey">主键数据类型</typeparam>
    [ApiController]
    public abstract class AreaApiControllerCustom<T, TODto, TIDto, TService, TKey> : ApiController
        where T : Entity
        where TService : ICustomService<T, TODto, TKey>
        where TODto : class
        where TIDto : class
        where TKey : IEquatable<TKey>
    {

        #region 属性变量


        /// <summary>
        /// 服务接口
        /// </summary>
        public TService iService;
        private readonly ILogService logService;


        #endregion

        /// <summary>
        /// Sounds//NG.wav
        /// </summary>
        public string S_Path_NG = "NG"; //"Sounds//NG.wav";
        /// <summary>
        /// Sounds//OK.wav
        /// </summary>
        public string S_Path_OK = "OK";//"Sounds//OK.wav";
        /// <summary>
        /// Sounds//RE.wav
        /// </summary>
        public string S_Path_RE = "RE";//"Sounds//RE.wav";

        //private string errMsg;
        //protected string ErrMsg { get { var v = ExtendErrCode.LanguageMsg(errMsg);
        //        return P_Language == 0 ? v.CN : v.EN;
        //    } }



        #region 构造函数及常用

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_iService"></param>
        public AreaApiControllerCustom(TService _iService)
        {
            iService = _iService;


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_iService"></param>
        /// <param name="logService"></param>
        public AreaApiControllerCustom(TService _iService, ILogService logService)
        {
            iService = _iService;
            this.logService = logService;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        protected CommonHeader commonHeader = new CommonHeader();
        /// <summary>
        /// 
        /// </summary>
        protected object throwMsg = "controller error";
        /// <summary>
        /// 
        /// </summary>
        protected DateTime startTime;
        /// <summary>
        /// 
        /// </summary>
        public string StationName = "";
        /// <summary>
        /// 日志功能代码
        /// </summary>
        protected string LogFunctionCode { get; set; }
        /// <summary>
        /// 启用动作日志
        /// </summary>
        protected readonly bool ActionEnabled = Configs.GetSection("AppSetting:EnabledActionLog").Value == "1";
        protected string TableName = string.Empty;
        /// <summary>
        /// 检查是否存在的条件名
        /// </summary>
        protected List<string> ExistsWhere = new List<string>();
        /// <summary>
        /// 主键名称  默认为ID
        /// </summary>
        protected  string PrimaryKeyName = "ID";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            string S_Language_Header = context.HttpContext.Request.Headers["Accept-Language"];
            if (S_Language_Header == "CN")
            {
                commonHeader.Language = 0;
            }
            else if (S_Language_Header == "EN")
            {
                commonHeader.Language = 1;
            }

            string S_LineID_Header = context.HttpContext.Request.Headers["line-id"].ToString();
            if (S_LineID_Header != "undefined" && S_LineID_Header != "")
            {
                commonHeader.LineId = Convert.ToInt32(S_LineID_Header);
            }

            string S_StationID_Header = context.HttpContext.Request.Headers["station-id"];
            if (S_StationID_Header != "undefined" && S_StationID_Header != "" && S_StationID_Header != "null")
            {
                commonHeader.StationId = Convert.ToInt32(S_StationID_Header);
            }

            string S_EmployeeID_Header = context.HttpContext.Request.Headers["employee-id"];
            if (S_EmployeeID_Header != "undefined" && S_EmployeeID_Header != "" && S_EmployeeID_Header != "null")
            {
                commonHeader.EmployeeId = Convert.ToInt32(S_EmployeeID_Header);
            }

            string S_P_CurrentLoginIP_Header = context.HttpContext.Request.Headers["current-login-ip"];
            if (S_P_CurrentLoginIP_Header != "undefined" && S_P_CurrentLoginIP_Header != "")
            {
                commonHeader.CurrentLoginIp = S_P_CurrentLoginIP_Header;
            }

            string S_StationName_Header = context.HttpContext.Request.Headers["station-name"];
            if (S_StationName_Header != "undefined" && S_StationName_Header != "")
            {
                commonHeader.StationName = System.Web.HttpUtility.UrlDecode(S_StationName_Header, Encoding.UTF8);
                StationName = System.Web.HttpUtility.UrlDecode(S_StationName_Header, Encoding.UTF8);
            }

            string S_LineName_Header = context.HttpContext.Request.Headers["line-name"];
            if (S_LineName_Header != "undefined" && S_LineName_Header != "")
            {
                commonHeader.LineName = System.Web.HttpUtility.UrlDecode(S_LineName_Header, Encoding.UTF8);
            }
            string S_Page_ID_Header = context.HttpContext.Request.Headers["page-uuid"];
            if (S_Page_ID_Header != "undefined" && S_Page_ID_Header != "")
            {
                commonHeader.PageId = System.Web.HttpUtility.UrlDecode(S_Page_ID_Header, Encoding.UTF8);
            }

            commonHeader.UserFullName = CurrentUser?.UserFullName ?? "";
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var authorizeAttributes = controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(YuebonAuthorizeAttribute), true).OfType<YuebonAuthorizeAttribute>();
            //if (authorizeAttributes.FirstOrDefault() != null)
            //{
            //    LogFunctionCode = authorizeAttributes.First().Function;
            //    if (LogFunctionCode == "Add") {
            //        LogFunctionCode = "Insert";
            //    }
            //    if (LogFunctionCode == "Edit")
            //    {
            //        LogFunctionCode = "Update";
            //    }

            //}

            if (!string.IsNullOrEmpty(controllerActionDescriptor.ActionName))
            {
                
                if (controllerActionDescriptor.ActionName.IndexOf("Add",  StringComparison.OrdinalIgnoreCase) >= 0 || 
                    controllerActionDescriptor.ActionName.IndexOf("Insert", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    LogFunctionCode = "Insert";
                }
                if (controllerActionDescriptor.ActionName.IndexOf("Edit", StringComparison.OrdinalIgnoreCase) >= 0 || 
                    controllerActionDescriptor.ActionName.IndexOf("Update", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    LogFunctionCode = "Update";
                }
                if (controllerActionDescriptor.ActionName.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0 || 
                    controllerActionDescriptor.ActionName.IndexOf("Delete", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    LogFunctionCode = "Delete";
                }
            }
            TableName = typeof(T).GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>(false)?.Name;
        }
        #region 公共添加、修改、删除、软删除接口

        /// <summary>
        /// 写Log
        /// </summary>
        /// <param name="info"></param>
        /// <param name="otherCode">其他类型功能代码</param>
        protected void WriteLog(T info, string otherCode = "")
        {
            Task.Run(async () =>
            {
                switch (LogFunctionCode)
                {
                    case "Insert":
                        await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        {
                            CurrentIP = commonHeader.CurrentLoginIp,
                            DataType = LogFunctionCode,
                            TableName = TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    case "Delete":
                        await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        {
                            CurrentIP = commonHeader.CurrentLoginIp,
                            DataType = LogFunctionCode,
                            TableName = TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    case "Update":
                        //交给子类重写
                        //await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        //{
                        //    CurrentIP = commonHeader.CurrentLoginIp,
                        //    DataType = LogFunctionCode,
                        //    TableName = TableName,
                        //    UserName = CurrentUser.UserFullName,
                        //    MSG = info.ToJson()
                        //});
                        break;
                    case "Clone":
                        //重写
                        break;
                    default:
                        if (!string.IsNullOrEmpty(otherCode))
                        {
                            await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                            {
                                CurrentIP = commonHeader.CurrentLoginIp,
                                DataType = otherCode,
                                TableName = TableName,
                                UserName = CurrentUser.UserFullName,
                                MSG = info.ToJson()
                            });
                        }
                        break;
                }
            });

        }
        /// <summary>
        /// 在插入数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual void OnBeforeInsert(T info)
        {
            WriteLog(info);

            if (!ActionEnabled)
                return;
            
            string desc = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            desc = desc.Length > 499 ? desc.Substring(0, 499) : desc;

            //留给子类对参数对象进行修改
            logService?.InsertAsync(new SunnyMES.Security.Models.Log()
            {
                CreatorUserId = commonHeader.EmployeeId.ToString(),
                Date = DateTime.Now,
                Type = "Insert",
                IPAddress = commonHeader.CurrentLoginIp,
                ModuleName = typeof(T).Name,
                Description = desc
            });
        }

        /// <summary>
        /// 在更新数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual void OnBeforeUpdate(T info)
        {
            WriteLog(info);

            if (!ActionEnabled)
                return;
            //留给子类对参数对象进行修改
            string desc = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            desc = desc.Length > 499 ? desc.Substring(0, 499) : desc;
            //留给子类对参数对象进行修改
            logService?.InsertAsync(new SunnyMES.Security.Models.Log()
            {
                CreatorUserId = commonHeader.EmployeeId.ToString(),
                Date = DateTime.Now,
                Type = "Update",
                IPAddress = commonHeader.CurrentLoginIp,
                ModuleName = typeof(T).Name,
                Description = desc
            });
        }

        /// <summary>
        /// 在软删除数据前对数据的修改操作
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        protected virtual void OnBeforeDelete(T info)
        {
            WriteLog(info);
            if (!ActionEnabled)
                return;

            //留给子类对参数对象进行修改
            string desc = Newtonsoft.Json.JsonConvert.SerializeObject(info);
            desc = desc.Length > 499 ? desc.Substring(0, 499) : desc;
            //留给子类对参数对象进行修改
            logService?.InsertAsync(new SunnyMES.Security.Models.Log()
            {
                CreatorUserId = commonHeader.EmployeeId.ToString(),
                Date = DateTime.Now,
                Type = "Delete",
                IPAddress = commonHeader.CurrentLoginIp,
                ModuleName = typeof(T).Name,
                Description = desc
            });
        }

        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> InsertAsync(TIDto tinfo)
        {
            CommonResult result = new CommonResult();
            T info = tinfo.MapTo<T>();

            string tmpWhere = OutputExtensions.FormartWhere<T>(info, ExistsWhere, PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
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
        /// 异步更新数据，需要在业务模块控制器重写该方法,否则更新无效
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        [HttpPost("UpdateAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> UpdateAsync(TIDto inInfo)
        {
            CommonResult result = new CommonResult();
            T info = inInfo.MapTo<T>();
            OnBeforeUpdate(info);

            return ToJsonContent(result);
        }

        /// <summary>
        /// 针对单一类型，不存在父子关系表，检查是否存在
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>true 存在</returns>
        [HttpPost("IsExistsT")]
        [YuebonAuthorize("Check")]
        public virtual async Task<bool> IsExistsT(T info)
        {
            string tmpWhere = OutputExtensions.FormartWhere<T>(info, PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            return IsExists is not null;
        }
        /// <summary>
        /// 异步物理删除
        /// </summary>
        /// <param name="id">主键Id</param>
        [HttpDelete("DeleteAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> DeleteAsync(TIDto inInfo)
        {
            CommonResult result = new CommonResult();
            var info = inInfo.MapTo<T>();
            OnBeforeDelete(info);

            bool bl = await iService.DeleteAsync(info).ConfigureAwait(false);
            if (bl)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
            }
            else
            {
                result.ResultMsg = ShowMsg(ErrCode.err43003);
                result.ResultCode = "43003";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 克隆项
        /// 公用接口，默认仅克隆当前项，子项不处理
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost("CloneAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> CloneAsync(TIDto inInfo)
        {
            CommonResult result = new CommonResult();
            T info = inInfo.MapTo<T>();

            string tmpWhere = OutputExtensions.FormartWhere<T>(info, ExistsWhere, PrimaryKeyName);
            var IsExists = await iService.GetWhereAsync(tmpWhere);
            if (IsExists is not null)
            {
                result.ResultMsg = ShowMsg( ErrCode.err70001);
                result.ResultCode = "70001";
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            string beforeWhere = OutputExtensions.GetBeforeSql<T>(info,  PrimaryKeyName);
            var beforeT = await iService.GetWhereAsync(beforeWhere);
            if (beforeT is null)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70002); ;
                result.ResultCode = "70002";
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            T newT = OutputExtensions.DeleteKeyValue<T>(info, PrimaryKeyName);
            long ln = await iService.InsertAsync(newT).ConfigureAwait(false);
            OnBeforeInsert(newT);
            if (ln > 0)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                await FormatCloneMsg(beforeT, newT);
            }
            else
            {
                result.ResultMsg = ShowMsg(ErrCode.err43001);
                result.ResultCode = "43001";
                result.Sounds = S_Path_NG;
                result.Success = false;
            }
            return ToJsonContent(result);
        }

        #endregion
        #region 查询单个实体
        /// <summary>
        /// 根据主键Id获取一个对象信息
        /// </summary>
        /// <param name="id">主键Id</param>
        /// <returns></returns>
        [HttpGet("GetById")]
        [YuebonAuthorize("")]
        [NoPermissionRequired]
        public virtual async Task<CommonResult<TODto>> GetById(TKey id)
        {
            CommonResult<TODto> result = new CommonResult<TODto>();
            TODto info = await iService.GetOutDtoAsync(id);
            if (info != null)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResData = info;
            }
            else
            {
                result.ResultMsg = ErrCode.err50001;
                result.ResultCode = "50001";
            }
            return result;
        }
        #endregion

        #region 返回集合的接口
        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询条件</param>
        /// <returns>指定对象的集合</returns>
        [HttpPost("FindWithPager")]
        [YuebonAuthorize("")]
        [Obsolete]
        public virtual CommonResult<PageResult<TODto>> FindWithPager(SearchInputDto<T> search)
        {
            CommonResult<PageResult<TODto>> result = new CommonResult<PageResult<TODto>>();
            result.ResData = iService.FindWithPager(search);
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }



        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<CommonResult<PageResult<TODto>>> FindWithPagerAsync(SearchInputDto<T> search)
        {
            CommonResult<PageResult<TODto>> result = new CommonResult<PageResult<TODto>>();
            result.ResData = await iService.FindWithPagerAsync(search);
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }

        /// <summary>
        /// 查询所有页
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("FindWithAllPagerAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<CommonResult<IEnumerable<T>>> FindWithAllPagerAsync(string Sortfield = "", bool IsAsc = true)
        {
            CommonResult<IEnumerable<T>> result = new CommonResult<IEnumerable<T>>();
            result.ResData = await iService.FindWithAllPagerAsync(new PageCustomInfo() { IsAsc = IsAsc, Sortfield = Sortfield});
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }

        /// <summary>
        /// 根据父ID查询所有页  子类必须重写，否则无效
        /// </summary>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        [HttpGet("FindPagerByParentAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> FindPagerByParentAsync(int ParentID)
        {
            return ToJsonContent(null);
        }

        /// <summary>
        /// 获取所有可用的
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllEnable")]
        [YuebonAuthorize("")]
        [Obsolete]
        public virtual async Task<CommonResult<List<TODto>>> GetAllEnable()
        {
            CommonResult<List<TODto>> result = new CommonResult<List<TODto>>();
            IEnumerable<T> list = await iService.GetAllByIsNotDeleteAndEnabledMarkAsync();
            List<TODto> resultList = list.MapTo<TODto>();
            result.ResData = resultList;
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;            
            result.Sounds = S_Path_OK;
            return result;
        }
        #endregion


        #region 辅助方法

        /// <summary>
        /// 根据语言返回对应的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string ShowMsg( string msg)
        {
            var v = msg.LanguageMsg();
            return P_Language == 0 ? v.CN : v.EN;
        }

        /// <summary>
        /// 更新前检查
        /// </summary>
        /// <param name="inInfo"></param>
        /// <returns></returns>
        protected async Task<CommonResult> UpdateBeforeCheckAsync(T inInfo)
        {
            CommonResult commonResult = new CommonResult();
            var whereSql = OutputExtensions.FormartWhere<T>(inInfo, null, "ID");
            var tmpCount = await iService.GetCountByWhereAsync(whereSql);
            if (tmpCount > 0)
            {
                commonResult = FormatNGResult(commonResult, ShowMsg(ErrCode.err70001));
            }
            return commonResult;
        }

        /// <summary>
        /// 格式化日志输出格式
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected string FormatMsg(string sn, string msg) => $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}   {sn}    {msg}";


        /// <summary>
        /// 格式化输出日志
        /// </summary>
        /// <param name="sn">输入数据</param>
        /// <param name="msg">错误提示信息，正常提示请传空值</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        protected string FormatOutputMsg(string sn, string msg = "")
        {
            return commonHeader.Language switch
            {
                0 =>
                    $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}   {(string.IsNullOrEmpty(sn) ? "" : $"输入数据:{sn}")}  {(string.IsNullOrEmpty(msg) ? ErrCode.err0 : msg)}",
                1 =>
                    $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}   {(string.IsNullOrEmpty(sn) ? "" : $"InputData:{sn}")}   {(string.IsNullOrEmpty(msg) ? "request success." : msg)}",
                _ => throw new ArgumentException("no define language code")
            };
        }
        /// <summary>
        /// 格式化OK结果
        /// </summary>
        /// <param name="commonResult"></param>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected CommonResult FormatOKResult(CommonResult commonResult, object data, string message = "")
        {
            commonResult.ResData = data;
            commonResult.Success = true;
            commonResult.ResultCode = ErrCode.successCode;
            commonResult.ResultMsg = string.IsNullOrEmpty(message) ? ErrCode.err0 : message;
            commonResult.Sounds = S_Path_OK;
            return commonResult;
        }
        /// <summary>
        /// 格式化NG结果
        /// </summary>
        /// <param name="commonResult"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        protected CommonResult FormatNGResult(CommonResult commonResult, string message = "")
        {
            commonResult.Success = false;
            commonResult.ResultCode = ErrCode.failCode;
            commonResult.ResultMsg = string.IsNullOrEmpty(message) ? ErrCode.err0 : message;
            commonResult.Sounds = S_Path_NG;
            return commonResult;
        }

        /// <summary>
        /// 拼装log字符
        /// </summary>
        /// <param name="beforeT">更新前实体</param>
        /// <param name="afterT">更新后实体</param>
        /// <returns></returns>
        protected async Task FormatUpdateMsg(T beforeT, T afterT)
        {
            string MSG = "before-----" + "\r\n" +
               beforeT.ToJson() + "\r\n" +
                "after-----" + "\r\n" +
               afterT.ToJson()
               ;
            await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
            {
                CurrentIP = commonHeader.CurrentLoginIp,
                DataType = LogFunctionCode,
                TableName = TableName,
                UserName = CurrentUser.UserFullName,
                MSG = MSG
            });
        }
        /// <summary>
        /// 克隆项写log
        /// </summary>
        /// <param name="beforeT"></param>
        /// <param name="cloneT"></param>
        /// <returns></returns>
        protected async Task FormatCloneMsg(T beforeT, T cloneT)
        {
            string MSG = "from-----" + "\r\n" +
               beforeT.ToJson() + "\r\n" +
                "clone-----" + "\r\n" +
               cloneT.ToJson()
               ;
            await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
            {
                CurrentIP = commonHeader.CurrentLoginIp,
                DataType = "Clone",
                TableName = TableName,
                UserName = CurrentUser.UserFullName,
                MSG = MSG
            });
        }
        #endregion



    }
}
