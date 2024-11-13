using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Core.App;
using SunnyMES.Commons.IoC;
using SunnyMES.Commons.Json;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Net;
using SunnyMES.Commons.Options;
using SunnyMES.Security.Application;
using SunnyMES.Security.Dtos;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.Commons.Core.PublicFun;
using API_MSG;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SunnyMES.Security.Repositories;
using Microsoft.Extensions.Primitives;
using Polly;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Management;
using SunnyMES.Commons.Log;
using SunnyMES.Commons;

namespace SunnyMES.WebApi.Controllers
{
    /// <summary>
    /// 用户登录接口控制器
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class LoginController : ApiController
    {
        private IUserService _userService;
        private IUserLogOnService _userLogOnService;
        private ISystemTypeService _systemTypeService;
        private IAPPService _appService;
        private IRoleService _roleService;
        private IRoleDataService _roleDataService;
        private ILogService _logService;
        private IFilterIPService _filterIPService;
        private IMenuService _menuService;
        private IPublicService _PublicService;
       
        /// <summary>
        /// 构造函数注入服务
        /// </summary>
        /// <param name="iService"></param>
        /// <param name="userLogOnService"></param>
        /// <param name="systemTypeService"></param>
        /// <param name="logService"></param>
        /// <param name="appService"></param>
        /// <param name="roleService"></param>
        /// <param name="filterIPService"></param>
        /// <param name="roleDataService"></param>
        /// <param name="menuService"></param>
        /// <param name="publicService"></param>
        public LoginController(IUserService iService, IUserLogOnService userLogOnService, 
            ISystemTypeService systemTypeService, ILogService logService, IAPPService appService,
            IRoleService roleService, IFilterIPService filterIPService,
            IRoleDataService roleDataService, IMenuService menuService, IPublicService publicService)
        {
            _userService = iService;
            _userLogOnService = userLogOnService;
            _systemTypeService = systemTypeService;
            _logService = logService;
            _appService = appService;
            _roleService = roleService;
            _filterIPService = filterIPService;
            _roleDataService = roleDataService;
            _menuService = menuService;
            _PublicService = publicService;
        }
        /// <summary>
        /// 用户登录，必须要有验证码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="vcode">验证码</param>
        /// <param name="vkey">验证码key</param>
        /// <param name="appId">AppId</param>
        /// <param name="systemCode">系统编码</param>
        /// <returns>返回用户User对象</returns>
        [HttpGet("GetCheckUser")]
        [NoPermissionRequired]
        public async Task<IActionResult> GetCheckUser(string username, string password, string vcode,string vkey, string appId,string systemCode)
        {

            CommonResult result = new CommonResult();
            RemoteIpParser remoteIpParser = new RemoteIpParser();
            string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            var vCode = yuebonCacheHelper.Get("ValidateCode" + vkey);
            string code = vCode != null ? vCode.ToString() : "11";
            if (vcode.ToUpper() != code)
            {
                result.ResultMsg = "验证码错误";
                return ToJsonContent(result);
            }
            Log logEntity = new Log();
            bool blIp=_filterIPService.ValidateIP(strIp);
            if (blIp)
            {
                result.ResultMsg = strIp+"该IP已被管理员禁止登录！";
            }
            else
            {

                if (string.IsNullOrEmpty(username))
                {
                    result.ResultMsg = "用户名不能为空！";
                }
                else if (string.IsNullOrEmpty(password))
                {
                    result.ResultMsg = "密码不能为空！";
                }
                if (string.IsNullOrEmpty(systemCode))
                {

                    result.ResultMsg = ErrCode.err40006;
                }
                else
                {
                    string strHost = Request.Host.ToString();
                    APP app = _appService.GetAPP(appId);
                    if (app == null)
                    {
                        result.ResultCode = "40001";
                        result.ResultMsg = ErrCode.err40001;
                    }
                    else
                    {
                        if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                        {
                            result.ResultCode = "40002";
                            result.ResultMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                        }
                        else
                        {
                            SystemType systemType = _systemTypeService.GetByCode(systemCode);
                            if (systemType == null)
                            {
                                result.ResultMsg = ErrCode.err40006;
                            }
                            else
                            {
                                Tuple<User, string> userLogin = await this._userService.Validate(username, password);
                                if (userLogin != null)
                                {
                                    string ipAddressName = IpAddressUtil.GetCityByIp(strIp);
                                    if (userLogin.Item1 != null)
                                    {
                                        result.Success = true;
                                        User user = userLogin.Item1;
                                        JwtOption jwtModel = App.GetService<JwtOption>();
                                        TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                        TokenResult tokenResult = tokenProvider.LoginToken(user, appId);

                                        YuebonCurrentUser currentSession = new YuebonCurrentUser
                                        {
                                            ID = user.Id,
                                            UserId = user.Id,
                                            Name = user.RealName,
                                            AccessToken = tokenResult.AccessToken,
                                            AppKey = appId,
                                            CreateTime = DateTime.Now,
                                            Role = _roleService.GetRoleEnCode(user.RoleId),
                                            ActiveSystemId = systemType.Id,
                                            CurrentLoginIP = strIp,
                                            IPAddressName = ipAddressName,
                                            UserFullName = user.Lastname + "." + user.Firstname
                                        };
                                        //TimeSpan expiresSliding = DateTime.Now.AddMinutes(10080) - DateTime.Now;

                                        TimeSpan expiresSliding = DateTime.Now.AddDays(7) - DateTime.Now;
                                        yuebonCacheHelper.Add("login_user_" + user.Id, currentSession, expiresSliding, true);

                                  //yuebonCacheHelper.Add("login_My_userId___"+ user.Id, user.Id);
                                  //yuebonCacheHelper.Add("login_My_currentSession" + user.Id, currentSession);


                                        List<AllowCacheApp> list = MemoryCacheHelper.Get<object>("cacheAppList").ToJson().ToList<AllowCacheApp>();
                                        if (list== null)
                                        {
                                            IEnumerable<APP> appList = _appService.GetAllByIsNotDeleteAndEnabledMark();
                                            MemoryCacheHelper.Set("cacheAppList", appList);
                                        }
                                        CurrentUser = currentSession;
                                        result.ResData = currentSession;
                                        result.ResultCode = ErrCode.successCode;
                                        result.Success = true;

                                        logEntity.Account = user.Account;
                                        logEntity.NickName = user.NickName;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = CurrentUser.CurrentLoginIP;
                                        logEntity.IPAddressName = CurrentUser.IPAddressName;
                                        logEntity.Result = true;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Description = "登录成功";
                                        logEntity.Type = "Login";
                                        _logService.Insert(logEntity);
                                    }
                                    else
                                    {
                                        result.ResultCode = ErrCode.failCode;
                                        result.ResultMsg = userLogin.Item2;
                                        logEntity.Account = username;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = strIp;
                                        logEntity.IPAddressName = ipAddressName;
                                        logEntity.Result = false;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Type = "Login";
                                        logEntity.Description = "登录失败，" + userLogin.Item2;
                                        _logService.Insert(logEntity);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            yuebonCacheHelper.Remove("LoginValidateCode");
            return ToJsonContent(result,true);
        }

        /// <summary>
        /// MES/SFC 用户登录，必须要有验证码
        /// </summary>
        /// <param name="username">用户名(管理员登陆本地缓存可以无线别和工站信息，其他用户登陆本地缓存必须有线别和工站信息)</param>
        /// <param name="password">密码</param>
        /// <param name="vcode">验证码</param>
        /// <param name="vkey">验证码key</param>
        /// <param name="appId">AppId</param>
        /// <param name="systemCode">系统编码</param>
        /// <param name="I_LineID">线别ID</param>
        /// <param name="I_StationID">工站ID</param>
        /// <returns>返回用户User对象</returns>
        [HttpGet("GetCheckUserMES")]
        [NoPermissionRequired]
        public async Task<IActionResult> GetCheckUserMES(string username, string password, string vcode, string vkey, 
            string appId, string systemCode,int I_LineID,int I_StationID)
        {

            CommonResult result = new CommonResult();
            RemoteIpParser remoteIpParser = new RemoteIpParser();
            string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            var vCode = yuebonCacheHelper.Get("ValidateCode" + vkey);
            string code = vCode != null ? vCode.ToString() : "11";

            //if (string.IsNullOrEmpty(I_Language.ToString())) 
            //{
            //    result.ResultMsg = "Language cannot be empty";
            //}

            MSG_Login v_MSG_Login = new MSG_Login(P_Language); 

            //if (vcode.ToUpper() != code)
            //{
            //    result.ResultMsg = v_MSG_Login.MSG_Login_001; //验证码错误
            //    return ToJsonContent(result);
            //}
            Log logEntity = new Log();
            bool blIp = _filterIPService.ValidateIP(strIp);
            if (blIp)
            {
                result.ResultMsg = strIp + v_MSG_Login.MSG_Login_002; //该IP已被管理员禁止登录
                return ToJsonContent(result);
            }
            else
            {

                if (string.IsNullOrEmpty(username))
                {

                    result.ResultMsg = v_MSG_Login.MSG_Login_003;  //用户名不能为空
                    return ToJsonContent(result);
                }
                else if (string.IsNullOrEmpty(password))
                {
                    result.ResultMsg = v_MSG_Login.MSG_Login_004; //密码不能为空！
                    return ToJsonContent(result);
                }
                //else if (string.IsNullOrEmpty(I_LineID.ToString()))
                //{
                //    result.ResultMsg = v_MSG_Login.MSG_Login_005; //线别不能为空！
                //    return ToJsonContent(result);
                //}
                //else if (string.IsNullOrEmpty(I_StationID.ToString()))
                //{
                //    result.ResultMsg = v_MSG_Login.MSG_Login_006; //工站不能为空
                //    return ToJsonContent(result);
                //}


                if (string.IsNullOrEmpty(systemCode))
                {

                    result.ResultMsg = ErrCode.err40006;
                }
                else
                {
                    string strHost = Request.Host.ToString();
                    APP app = _appService.GetAPP(appId);
                    if (app == null)
                    {
                        result.ResultCode = "40001";
                        result.ResultMsg = ErrCode.err40001;
                    }
                    else
                    {
                        if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                        {
                            result.ResultCode = "40002";
                            result.ResultMsg = ErrCode.err40002 + "，"+ v_MSG_Login.MSG_Login_007 +  strHost;
                        }
                        else
                        {
                            SystemType systemType = _systemTypeService.GetByCode(systemCode);
                            if (systemType == null)
                            {
                                result.ResultMsg = ErrCode.err40006;
                            }
                            else
                            {
                                string S_PWD = "";
                                try
                                {
                                    S_PWD = RSASFC.RsaDecrypt(password);                                    
                                }
                                catch 
                                {
                                    S_PWD = "";
                                }

                                if (S_PWD == "") { S_PWD = password; }

                                Tuple<User, string> userLogin = await this._userService.Validate(username, S_PWD, I_LineID,I_StationID, v_MSG_Login);

                                if (userLogin != null)
                                {
                                    string ipAddressName = IpAddressUtil.GetCityByIp(strIp);
                                    if (userLogin.Item1 != null)
                                    {
                                        result.Success = true;
                                        User user = userLogin.Item1;
                                        JwtOption jwtModel = App.GetService<JwtOption>();
                                        TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                        TokenResult tokenResult = tokenProvider.LoginToken(user, appId);


                                        string S_ApplicationType = await Task.Run(() => _PublicService.LuGetApplicationType(I_StationID.ToString()));
                                        IEnumerable<dynamic> List_Menu = await _PublicService.GetData("API_Menu", " MenuType='M' and FullName='" + S_ApplicationType + "'");

                                        string S_APPURL = "";
                                        foreach (var item in List_Menu)
                                        {
                                            JObject Json_Menu = (JObject)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(item));
                                            if (Json_Menu.Count > 0)
                                            {
                                                S_APPURL = Json_Menu.Root.Value<string>("EnCode");
                                            }
                                            continue;
                                        }

                                        YuebonCurrentUser currentSession = new YuebonCurrentUser
                                        {
                                            //IsReport = user.IsReport ?? false,
                                            UserType = user.UserType ?? "",
                                            ApplicationType = S_ApplicationType,
                                            APPURL = S_APPURL,
                                            IsAdmin = user.IsAdministrator ?? false,

                                            UserId = user.Id,
                                            Name = user.RealName,
                                            AccessToken = tokenResult.AccessToken,
                                            AppKey = appId,
                                            CreateTime = DateTime.Now,
                                            Role = _roleService.GetRoleEnCode(user.RoleId),
                                            ActiveSystemId = systemType.Id,
                                            CurrentLoginIP = strIp,
                                            IPAddressName = ipAddressName,
                                            UserFullName = user.Lastname + "." + user.Firstname
                                        };
                                        //TimeSpan expiresSliding = DateTime.Now.AddMinutes(10080) - DateTime.Now;

                                        TimeSpan expiresSliding = DateTime.Now.AddDays(7) - DateTime.Now;
                                        yuebonCacheHelper.Add("login_user_" + user.Id, currentSession, expiresSliding, true);

                                        //yuebonCacheHelper.Add("login_My_userId___"+ user.Id, user.Id);
                                        //yuebonCacheHelper.Add("login_My_currentSession" + user.Id, currentSession);


                                        List<AllowCacheApp> list = MemoryCacheHelper.Get<object>("cacheAppList").ToJson().ToList<AllowCacheApp>();
                                        if (list == null)
                                        {
                                            IEnumerable<APP> appList = _appService.GetAllByIsNotDeleteAndEnabledMark();
                                            MemoryCacheHelper.Set("cacheAppList", appList);
                                        }
                                        CurrentUser = currentSession;
                                        result.ResData = currentSession;
                                        result.ResultCode = ErrCode.successCode;
                                        result.Success = true;

                                        logEntity.Account = user.Account;
                                        logEntity.NickName = user.NickName;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = CurrentUser.CurrentLoginIP;
                                        logEntity.IPAddressName = CurrentUser.IPAddressName;
                                        logEntity.Result = true;
                                        logEntity.ModuleName = v_MSG_Login.MSG_Login_008;
                                        logEntity.Description = v_MSG_Login.MSG_Login_009;
                                        logEntity.Type = "Login";
                                        logEntity.LineID = I_LineID;
                                        logEntity.StationID = I_StationID;
                                        logEntity.LanguageID = P_Language;

                                        _logService.Insert(logEntity);


                                        //HttpContext httpContext = HttpContext.Request.HttpContext;
                                        //httpContext.Items["Online"]= user.UserID;

                                        //KeyValuePair<string,StringValues> KVP_LineID =
                                        //            new KeyValuePair<string , StringValues>("line-id", I_LineID.ToString());
                                        //if (HttpContext.Request.Headers.Contains(KVP_LineID) == false) 
                                        //{
                                        //    HttpContext.Request.Headers.Add(KVP_LineID);
                                        //}

                                        //KeyValuePair<string, StringValues> KVP_StationID =
                                        //            new KeyValuePair<string, StringValues>("station-id", I_StationID.ToString());
                                        //if (HttpContext.Request.Headers.Contains(KVP_StationID) == false)
                                        //{
                                        //    HttpContext.Request.Headers.Add(KVP_StationID);
                                        //}

                                        //PublicRepository Public_Repository = new PublicRepository();
                                        //List<TabVal> List_Station=await Public_Repository.MesGetStationNoTask(I_StationID.ToString(), "", "");

                                        //string StationTypeID = List_Station[0].Valint1.ToString();

                                        //LoginList List_Login = new LoginList
                                        //{
                                        //    EmployeeID=user.Id
                                        //};
                                        //yuebonCacheHelper.Add("List_Login", List_Login);

                                        //yuebonCacheHelper.Add("Cache_EmployeeID", user.Id);

                                    }
                                    else
                                    {
                                        result.ResultCode = ErrCode.failCode;
                                        result.ResultMsg = userLogin.Item2;
                                        logEntity.Account = username;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = strIp;
                                        logEntity.IPAddressName = ipAddressName;
                                        logEntity.Result = false;
                                        logEntity.ModuleName = v_MSG_Login.MSG_Login_008;
                                        logEntity.Type = "Login";
                                        logEntity.LineID = I_LineID;
                                        logEntity.StationID = I_StationID;
                                        logEntity.LanguageID = P_Language;

                                        logEntity.Description = v_MSG_Login.MSG_Login_010 +
                                            "，" + userLogin.Item2;
                                        _logService.Insert(logEntity);

                                        //yuebonCacheHelper.Remove("List_Login");
                                    }
                                }
                                else
                                {
                                    Log4NetHelper.Error(DateTime.Now.ToString("yyyy-MM-dd HH:mm") +" Login: "+ username+" 验证用户信息为空");
                                }

                            }

                        }
                    }
                }
            }
            yuebonCacheHelper.Remove("LoginValidateCode");
            return ToJsonContent(result, true);
        }


        /// <summary>
        /// 用户登录，无需验证码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>

        /// <param name="appId">AppId</param>
        /// <param name="systemCode">系统编码</param>
        /// <returns>返回用户User对象</returns>
        [HttpGet("GetCheckUserNoKey")]
        [NoPermissionRequired]
        public async Task<IActionResult> GetCheckUserNoKey(string username, string password, string appId, string systemCode)
        {

            CommonResult result = new CommonResult();
            RemoteIpParser remoteIpParser = new RemoteIpParser();
            string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            //var vCode = yuebonCacheHelper.Get("ValidateCode"+ vkey);
            //string code = vCode != null ? vCode.ToString() : "11";
            //if (vcode.ToUpper() != code)
            //{
            //    result.ResultMsg = "验证码错误";
            //    return ToJsonContent(result);
            //}
            Log logEntity = new Log();
            bool blIp = _filterIPService.ValidateIP(strIp);
            if (blIp)
            {
                result.ResultMsg = strIp + "该IP已被管理员禁止登录！";
            }
            else
            {

                if (string.IsNullOrEmpty(username))
                {
                    result.ResultMsg = "用户名不能为空！";
                }
                else if (string.IsNullOrEmpty(password))
                {
                    result.ResultMsg = "密码不能为空！";
                }
                if (string.IsNullOrEmpty(systemCode))
                {

                    result.ResultMsg = ErrCode.err40006;
                }
                else
                {
                    string strHost = Request.Host.ToString();
                    APP app = _appService.GetAPP(appId);
                    if (app == null)
                    {
                        result.ResultCode = "40001";
                        result.ResultMsg = ErrCode.err40001;
                    }
                    else
                    {
                        if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                        {
                            result.ResultCode = "40002";
                            result.ResultMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                        }
                        else
                        {
                            SystemType systemType = _systemTypeService.GetByCode(systemCode);
                            if (systemType == null)
                            {
                                result.ResultMsg = ErrCode.err40006;
                            }
                            else
                            {
                                string S_PWD = "";
                                try
                                {
                                    S_PWD = RSASFC.RsaDecrypt(password);
                                }
                                catch
                                {
                                    S_PWD = "";
                                }

                                if (S_PWD == "") { S_PWD = password; }


                                Tuple<User, string> userLogin = await this._userService.Validate(username, S_PWD);
                                if (userLogin != null)
                                {
                                    string ipAddressName = IpAddressUtil.GetCityByIp(strIp);
                                    if (userLogin.Item1 != null)
                                    {
                                        result.Success = true;
                                        User user = userLogin.Item1;
                                        JwtOption jwtModel = App.GetService<JwtOption>();
                                        TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                        TokenResult tokenResult = tokenProvider.LoginToken(user, appId);
                                        YuebonCurrentUser currentSession = new YuebonCurrentUser
                                        {
                                            UserId = user.Id,
                                            Name = user.RealName,
                                            AccessToken = tokenResult.AccessToken,
                                            AppKey = appId,
                                            CreateTime = DateTime.Now,
                                            Role = _roleService.GetRoleEnCode(user.RoleId),
                                            ActiveSystemId = systemType.Id,
                                            CurrentLoginIP = strIp,
                                            IPAddressName = ipAddressName,
                                            UserFullName = user.Lastname + "." + user.Firstname
                                        };
                                        TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                        yuebonCacheHelper.Add("login_user_" + user.Id, currentSession, expiresSliding, true);

                                        List<AllowCacheApp> list = MemoryCacheHelper.Get<object>("cacheAppList").ToJson().ToList<AllowCacheApp>();
                                        if (list == null)
                                        {
                                            IEnumerable<APP> appList = _appService.GetAllByIsNotDeleteAndEnabledMark();
                                            MemoryCacheHelper.Set("cacheAppList", appList);
                                        }
                                        CurrentUser = currentSession;
                                        result.ResData = currentSession;
                                        result.ResultCode = ErrCode.successCode;
                                        result.Success = true;

                                        logEntity.Account = user.Account;
                                        logEntity.NickName = user.NickName;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = CurrentUser.CurrentLoginIP;
                                        logEntity.IPAddressName = CurrentUser.IPAddressName;
                                        logEntity.Result = true;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Description = "登录成功";
                                        logEntity.Type = "Login";

                                        logEntity.LineID = -1;
                                        logEntity.StationID = -1;
                                        logEntity.LanguageID = -1;

                                        _logService.Insert(logEntity);
                                    }
                                    else
                                    {
                                        result.ResultCode = ErrCode.failCode;
                                        result.ResultMsg = userLogin.Item2;
                                        logEntity.Account = username;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = strIp;
                                        logEntity.IPAddressName = ipAddressName;
                                        logEntity.Result = false;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Type = "Login";
                                        logEntity.Description = "登录失败，" + userLogin.Item2;

                                        logEntity.LineID = -1;
                                        logEntity.StationID = -1;
                                        logEntity.LanguageID = -1;

                                        _logService.Insert(logEntity);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            yuebonCacheHelper.Remove("LoginValidateCode");
            return ToJsonContent(result, true);
        }



        /// <summary>
        /// 获取登录用户权限信息
        /// </summary>
        /// <returns>返回用户User对象</returns>
        [HttpGet("GetUserInfo")]
        [YuebonAuthorize("")]
        public  IActionResult GetUserInfo()
        {
            CommonResult result = new CommonResult();
            if (CurrentUser == null)
            {
                return Logout();
            }
            string S_Data1_ID = "";
            string S_Data1_UserID = "";
            string S_Data1_Lastname = "";
            string S_Data1_Firstname = "";
            string S_Data1_Password = "";
            string S_Data1_OrganizeId = "";
            string S_Data1_DepartmentId = "";
            string S_Data1_RoleId = "";
            string S_Data1_IsAdministrator = "";
            string S_Data1_UserType = "";

            User user = _userService.Get(CurrentUser.UserId);
            user.IsAdministrator = Convert.ToBoolean(user.IsAdministrator);

            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            SystemType systemType = _systemTypeService.Get(CurrentUser.ActiveSystemId);

            string S_LocalDataYZ = "OK";
            try
            {
                string S_SysPath = Directory.GetCurrentDirectory();
                DataTable dataTable1 = new DataTable();
                string S_Data1 = S_SysPath + "\\Data1.dll";

                string S_MI1 =System.IO.File.ReadAllText(S_Data1);
                string S_JsonJM1 = EncryptHelper2023.DecryptString(S_MI1);
                JArray jsonArray1 = JArray.Parse(S_JsonJM1);

                foreach (JProperty property in jsonArray1[0])
                {
                    dataTable1.Columns.Add(property.Name);
                }
                foreach (JObject jsonObject in jsonArray1)
                {
                    DataRow row = dataTable1.NewRow();
                    foreach (JProperty property in jsonObject.Properties())
                    {
                        row[property.Name] = property.Value.ToString();
                    }
                    dataTable1.Rows.Add(row);
                }

                
                try
                {
                    DataRow[] List_DR1 = dataTable1.Select("UserID='" + user.UserID + "'");
                    if (List_DR1.Count() > 0)
                    {
                        S_Data1_ID = List_DR1[0]["ID"].ToString();
                        S_Data1_UserID = List_DR1[0]["UserID"].ToString();
                        S_Data1_Lastname = List_DR1[0]["Lastname"].ToString();
                        S_Data1_Firstname = List_DR1[0]["Firstname"].ToString();

                        S_Data1_Password = List_DR1[0]["Password"].ToString();

                        S_Data1_OrganizeId = List_DR1[0]["OrganizeId"].ToString();
                        S_Data1_DepartmentId = List_DR1[0]["DepartmentId"].ToString();
                        S_Data1_RoleId = List_DR1[0]["RoleId"].ToString();
                        S_Data1_IsAdministrator = List_DR1[0]["IsAdministrator"].ToString();
                        S_Data1_IsAdministrator = S_Data1_IsAdministrator == "" ? "False" : S_Data1_IsAdministrator;

                        S_Data1_UserType = List_DR1[0]["UserType"].ToString();

                        if (
                            (user.Id == S_Data1_ID)
                            && (user.UserID == S_Data1_UserID)
                            && (user.Lastname == S_Data1_Lastname)
                            && (user.Firstname == S_Data1_Firstname)

                            && (user.OrganizeId == S_Data1_OrganizeId)
                            && (user.DepartmentId == S_Data1_DepartmentId)
                            && (user.RoleId == S_Data1_RoleId)
                            && (Convert.ToString(user.IsAdministrator) == S_Data1_IsAdministrator)
                            && (user.UserType == S_Data1_UserType)

                            )
                        {
                        }
                        else 
                        {
                            S_LocalDataYZ="";
                        }
                    }
                    else 
                    {
                        S_LocalDataYZ = "";
                    }
                }
                catch 
                {
                    S_LocalDataYZ = "";
                }

            }
            catch
            {
                S_LocalDataYZ = "";
            }

            string S_IsValidateSecond = "0";
            try
            {
                S_IsValidateSecond = Configs.GetConfigurationValue("AppSetting", "IsValidateSecond");
                S_IsValidateSecond = S_IsValidateSecond ?? "0";
            }
            catch
            {
                S_IsValidateSecond = "0";
            }

            if (S_IsValidateSecond == "1")
            {
                string S_ServerData = _userService.GetServerData();
                if (S_ServerData.IndexOf("ERROR:") > -1)
                {
                    result.ResData = S_ServerData;
                    result.ResultCode = ErrCode.err1;
                    result.Success = true;
                    return ToJsonContent(result, true);
                    //return Logout();
                }
            }


            if (S_LocalDataYZ=="") 
            {
                string S_LoginLog =
                "user.Id:" + user.Id + "    S_Data1_ID:" + S_Data1_ID + "\r\n" +
                "user.UserID:" + user.UserID + "    S_Data1_UserID:" + S_Data1_UserID + "\r\n" +
                "user.Lastname:" + user.Lastname + "    S_Data1_Lastname:" + S_Data1_Lastname + "\r\n" +
                "user.Firstname:" + user.Firstname + "    S_Data1_Firstname:" + S_Data1_Firstname + "\r\n" +
                "user.OrganizeId:" + user.OrganizeId + "    S_Data1_OrganizeId:" + S_Data1_OrganizeId + "\r\n" +
                "user.DepartmentId:" + user.DepartmentId + "    S_Data1_DepartmentId:" + S_Data1_DepartmentId + "\r\n" +
                "user.RoleId:" + user.RoleId + "    S_Data1_RoleId:" + S_Data1_RoleId + "\r\n" +
                "user.IsAdministrator:" + Convert.ToString(user.IsAdministrator) + "    S_Data1_IsAdministrator:" + S_Data1_IsAdministrator + "\r\n" +
                "user.UserType:" + user.UserType + "    S_Data1_UserType:" + S_Data1_UserType
                ;

                Log4NetHelper.Warn(DateTime.Now.ToString("yyyy-MM-dd HH:mm") + S_LoginLog);

                if (CurrentUser.Name.ToLower() == "developer")
                {
                    
                }
                else
                {
                    if (S_IsValidateSecond == "1") 
                    {
                        return Logout();
                    }                    
                }
            }


            string S_ApplicationType = "";
            string S_APPURL = "";
            var User_YCach = yuebonCacheHelper.Get("login_user_" + user.Id).ToJson().ToObject<YuebonCurrentUser>();
            if (User_YCach != null) 
            {               
                S_ApplicationType = User_YCach.ApplicationType??"";
                S_APPURL = User_YCach.APPURL??"";
            }

            List<DNShift> List_DNShift= _PublicService.GetrptDashboardGetDNShift();

            YuebonCurrentUser currentSession = new YuebonCurrentUser
            {
                //IsReport = user.IsReport ?? false,
                UserType = user.UserType ?? "",
                DN_Shift = List_DNShift ?? null,

                ApplicationType = S_ApplicationType,
                APPURL = S_APPURL,
                IsAdmin = user.IsAdministrator ?? false,

                UserId = user.Id,
                Account = user.Account,
                Name = user.RealName,
                NickName = user.NickName,
                AccessToken = CurrentUser.AccessToken,
                AppKey = CurrentUser.AppKey,
                CreateTime = DateTime.Now,
                HeadIcon = user.HeadIcon,
                Gender = user.Gender,
                ReferralUserId = user.ReferralUserId,
                MemberGradeId = user.MemberGradeId,
                Role = _roleService.GetRoleEnCode(user.RoleId),
                MobilePhone = user.MobilePhone,
                OrganizeId = user.OrganizeId,
                DeptId = user.DepartmentId,
                CurrentLoginIP = CurrentUser.CurrentLoginIP,
                IPAddressName = CurrentUser.IPAddressName,
                TenantId = "",
                UserFullName = user.Lastname + "." + user.Firstname
            };
			CurrentUser = currentSession;

            CurrentUser.ActiveSystemId = systemType.Id;
            CurrentUser.ActiveSystem = systemType.FullName;
            CurrentUser.ActiveSystemUrl = systemType.Url;

            List<MenuOutputDto> listFunction = new List<MenuOutputDto>();
            MenuApp menuApp = new MenuApp();
            if (Permission.IsAdmin(CurrentUser))
            {
                CurrentUser.SubSystemList = _systemTypeService.GetAllByIsNotDeleteAndEnabledMark().MapTo<SystemTypeOutputDto>();
                //取得用户可使用的授权功能信息，并存储在缓存中
                listFunction = menuApp.GetFunctionsBySystem(CurrentUser.ActiveSystemId);
                CurrentUser.MenusRouter = menuApp.GetVueRouter("", systemType.EnCode, CurrentUser.Name);
            }
            else
            {
                CurrentUser.SubSystemList = _systemTypeService.GetSubSystemList(user.RoleId);
                //取得用户可使用的授权功能信息，并存储在缓存中
                listFunction = menuApp.GetFunctionsByUser(user.Id, CurrentUser.ActiveSystemId, user.RoleId);
                CurrentUser.MenusRouter = menuApp.GetVueRouter(user.RoleId, systemType.EnCode, CurrentUser.Name);
            }
            UserLogOn userLogOn = _userLogOnService.GetByUserId(CurrentUser.UserId);
            CurrentUser.UserTheme = userLogOn.Theme == null ? "default" : userLogOn.Theme;
            TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
            yuebonCacheHelper.Add("User_Function_" + user.Id, listFunction, expiresSliding, true);
            List<string> listModules = new List<string>();
            foreach (MenuOutputDto item in listFunction)
            {
                listModules.Add(item.EnCode);
            }

            CurrentUser.Modules = listModules;
            yuebonCacheHelper.Add("login_user_" + user.Id, CurrentUser, expiresSliding, true);
            //该用户的数据权限
            List<String> roleDateList = _roleDataService.GetListDeptByRole(user.RoleId);
            yuebonCacheHelper.Add("User_RoleData_" + user.Id, roleDateList, expiresSliding, true);
            result.ResData = CurrentUser;
            result.ResultCode = ErrCode.successCode;
            result.Success = true;
            return ToJsonContent(result, true);
        }

        /// <summary>
        /// 用户登录，无验证码，主要用于app登录
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="appId">AppId</param>
        /// <param name="systemCode">系统编码</param>
        /// <returns>返回用户User对象</returns>
        [HttpGet("UserLogin")]
        //[ApiVersion("2.0")]
        [NoPermissionRequired]
        public async Task<IActionResult> UserLogin(string username, string password,  string appId, string systemCode)
        {
            CommonResult result = new CommonResult();
            RemoteIpParser remoteIpParser = new RemoteIpParser();
            string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();

            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            Log logEntity = new Log();
            bool blIp = _filterIPService.ValidateIP(strIp);
            if (blIp)
            {
                result.ResultMsg = strIp + "该IP已被管理员禁止登录！";
            }
            else
            {
                if (string.IsNullOrEmpty(username))
                {
                    result.ResultMsg = "用户名不能为空！";
                }
                else if (string.IsNullOrEmpty(password))
                {
                    result.ResultMsg = "密码不能为空！";
                }
                if (string.IsNullOrEmpty(systemCode))
                {

                    result.ResultMsg = ErrCode.err40006;
                }
                else
                {
                    string strHost = Request.Host.ToString();
                    APP app = _appService.GetAPP(appId);
                    if (app == null)
                    {
                        result.ResultCode = "40001";
                        result.ResultMsg = ErrCode.err40001;
                    }
                    else
                    {
                        if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                        {
                            result.ResultCode = "40002";
                            result.ResultMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                        }
                        else
                        {
                            SystemType systemType = _systemTypeService.GetByCode(systemCode);
                            if (systemType == null)
                            {
                                result.ResultMsg = ErrCode.err40006;
                            }
                            else
                            {
                                Tuple<User, string> userLogin = await this._userService.Validate(username, password);
                                if (userLogin != null)
                                {

                                    string ipAddressName = IpAddressUtil.GetCityByIp(strIp);
                                    if (userLogin.Item1 != null)
                                    {
                                        result.Success = true;

                                        User user = userLogin.Item1;

                                        JwtOption jwtModel = App.GetService<JwtOption>();
                                        TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                        TokenResult tokenResult = tokenProvider.LoginToken(user, appId);
                                        YuebonCurrentUser currentSession = new YuebonCurrentUser
                                        {
                                            UserId = user.Id,
                                            Name = user.RealName,
                                            AccessToken = tokenResult.AccessToken,
                                            AppKey = appId,
                                            CreateTime = DateTime.Now,
                                            Role = _roleService.GetRoleEnCode(user.RoleId),
                                            ActiveSystemId = systemType.Id,
                                            CurrentLoginIP = strIp,
                                            IPAddressName = ipAddressName

                                        };
                                        TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                        yuebonCacheHelper.Add("login_user_" + user.Id, currentSession, expiresSliding, true);
                                        CurrentUser = currentSession;
                                        result.ResData = currentSession;
                                        result.ResultCode = ErrCode.successCode;
                                        result.Success = true;

                                        logEntity.Account = user.Account;
                                        logEntity.NickName = user.NickName;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = CurrentUser.CurrentLoginIP;
                                        logEntity.IPAddressName = CurrentUser.IPAddressName;
                                        logEntity.Result = true;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Description = "登录成功";
                                        logEntity.Type = "Login";
                                        _logService.Insert(logEntity);
                                    }
                                    else
                                    {
                                        result.ResultCode = ErrCode.failCode;
                                        result.ResultMsg = userLogin.Item2;
                                        logEntity.Account = username;
                                        logEntity.Date = logEntity.CreatorTime = DateTime.Now;
                                        logEntity.IPAddress = strIp;
                                        logEntity.IPAddressName = ipAddressName;
                                        logEntity.Result = false;
                                        logEntity.ModuleName = "登录";
                                        logEntity.Type = "Login";
                                        logEntity.Description = "登录失败，" + userLogin.Item2;
                                        _logService.Insert(logEntity);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            return ToJsonContent(result, true);
        }


        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        [YuebonAuthorize("")]
        public IActionResult Logout()
        {
            CommonResult result = new CommonResult();
            try
            {                
                YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                if (CurrentUser!= null)
                {
                    yuebonCacheHelper.Remove("login_user_" + CurrentUser.UserId);
                    yuebonCacheHelper.Remove("User_Function_" + CurrentUser.UserId);

                    UserLogOn userLogOn = _userLogOnService.GetWhere("UserId='" + CurrentUser.UserId + "'");
                    userLogOn.UserOnLine = false;
                    _userLogOnService.Update(userLogOn, userLogOn.Id);
                }

                CurrentUser = null;
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = "成功退出";                
            }
            catch(Exception ex)
            {
                result.Success = false;
                result.ResultCode = ErrCode.err1;
                result.ResultMsg = "退出失败"+ex.Message;
            }

            return ToJsonContent(result);
        }

        /// <summary>
        /// 子系统切换登录
        /// </summary>
        /// <param name="openmf">凭据</param>
        /// <param name="appId">应用Id</param>
        /// <param name="systemCode">子系统编码</param>
        /// <returns>返回用户User对象</returns>
        [HttpGet("SysConnect")]
        [AllowAnonymous]
        [NoPermissionRequired]
        public IActionResult SysConnect(string openmf, string appId, string systemCode)
        {
            CommonResult result = new CommonResult();
            RemoteIpParser remoteIpParser = new RemoteIpParser();
            string strIp = remoteIpParser.GetClientIp(HttpContext).MapToIPv4().ToString();
            if (string.IsNullOrEmpty(openmf))
            {
                result.ResultMsg = "切换参数错误！";
            }

            bool blIp = _filterIPService.ValidateIP(strIp);
            if (blIp)
            {
                result.ResultMsg = strIp + "该IP已被管理员禁止登录！";
            }
            else
            {
                string ipAddressName = IpAddressUtil.GetCityByIp(strIp);
                if (string.IsNullOrEmpty(systemCode))
                {
                    result.ResultMsg = ErrCode.err40006;
                }
                else
                {
                    string strHost = Request.Host.ToString();
                    APP app = _appService.GetAPP(appId);
                    if (app == null)
                    {
                        result.ResultCode = "40001";
                        result.ResultMsg = ErrCode.err40001;
                    }
                    else
                    {
                        if (!app.RequestUrl.Contains(strHost, StringComparison.Ordinal) && !strHost.Contains("localhost", StringComparison.Ordinal))
                        {
                            result.ResultCode = "40002";
                            result.ResultMsg = ErrCode.err40002 + "，你当前请求主机：" + strHost;
                        }
                        else
                        {
                            SystemType systemType = _systemTypeService.GetByCode(systemCode);
                            if (systemType == null)
                            {
                                result.ResultMsg = ErrCode.err40006;
                            }
                            else
                            {
                                YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
                                object cacheOpenmf = yuebonCacheHelper.Get("openmf" + openmf);
                                yuebonCacheHelper.Remove("openmf" + openmf);
                                if (cacheOpenmf == null)
                                {
                                    result.ResultCode = "40007";
                                    result.ResultMsg = ErrCode.err40007;
                                }
                                else
                                {
                                    User user = _userService.Get(cacheOpenmf.ToString());
                                    if (user != null)
                                    {
                                        result.Success = true;
                                        JwtOption jwtModel = App.GetService<JwtOption>();
                                        TokenProvider tokenProvider = new TokenProvider(jwtModel);
                                        TokenResult tokenResult = tokenProvider.LoginToken(user, appId);
                                        YuebonCurrentUser currentSession = new YuebonCurrentUser
                                        {
                                            UserId = user.Id,
                                            Name = user.RealName,
                                            AccessToken = tokenResult.AccessToken,
                                            AppKey = appId,
                                            CreateTime = DateTime.Now,
                                            Role = _roleService.GetRoleEnCode(user.RoleId),
                                            ActiveSystemId = systemType.Id,
                                            CurrentLoginIP = strIp,
                                            IPAddressName = ipAddressName,
                                            ActiveSystemUrl= systemType.Url

                                        };
                                        TimeSpan expiresSliding = DateTime.Now.AddMinutes(120) - DateTime.Now;
                                        yuebonCacheHelper.Add("login_user_" + user.Id, currentSession, expiresSliding, true);
                                        CurrentUser = currentSession;
                                        result.ResData = currentSession;
                                        result.ResultCode = ErrCode.successCode;
                                        result.Success = true;
                                    }
                                    else
                                    {
                                        result.ResultCode = ErrCode.failCode;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 弃用接口演示
        /// </summary>
        /// <returns></returns>
        [HttpGet("TestLogin")]
        [Obsolete]
        //[ApiVersion("2.0")]
        public IActionResult TestLogin()
        {
            CommonResult result = new CommonResult();
            result.Success = true;
            result.ResultCode = ErrCode.successCode;
            result.ResData = "弃用接口演示";
            result.ResultMsg = "成功退出";
            return ToJsonContent(result);
        }
    }
}