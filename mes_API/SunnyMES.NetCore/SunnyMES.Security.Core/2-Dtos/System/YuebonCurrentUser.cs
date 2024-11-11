﻿using System;
using System.Collections.Generic;
using System.Text;
using SunnyMES.Commons.Tree;
using SunnyMES.Security.Models;

namespace SunnyMES.Security.Dtos
{
    /// <summary>
    /// 登录成功返回用户信息
    /// </summary>
    [Serializable]
    public class YuebonCurrentUser
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 授权token码
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// appkey
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 用户工号
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string HeadIcon { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public int? Gender { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string MemberGradeId { get; set; }

        /// <summary>
        /// 上级推广员
        /// </summary>
        public string ReferralUserId { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 组织主键
        /// </summary>
        public virtual string OrganizeId { get; set; }
        /// <summary>
        /// 部门主键
        /// </summary>
        public virtual string DeptId { get; set; }
        /// <summary>
        /// 角色编码，多个角色，使用“,”分格
        /// </summary>
        public string Role { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public string MobilePhone { get; set; }
        /// <summary>
        /// 其他对象
        /// </summary>
        public object OtherOpenObj { get; set; }

        /// <summary>
        /// 微信登录SessionId
        /// </summary>
        public string WxSessionId { get; set; }
        /// <summary>
        /// 租户TenantId
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// 登录IP地址
        /// </summary>
        public virtual string CurrentLoginIP { get; set; }
        /// <summary>
        /// 登录IP地址
        /// </summary>
        public virtual string IPAddressName { get; set; }


        /// <summary>
        /// 当前访问的系统Id
        /// </summary>
        public string ActiveSystemId { get; set; }
        /// <summary>
        /// 当前访问的系统名称
        /// </summary>
        public string ActiveSystem { get; set; }
        /// <summary>
        /// 当前访问的系统Url
        /// </summary>
        public string ActiveSystemUrl { get; set; }

        /// <summary>
        /// 可以访问子系统
        /// </summary>
        public List<SystemTypeOutputDto> SubSystemList { get; set; }


        /// <summary>
        /// 授权访问菜单
        /// </summary>
        public List<MenuOutputDto> MenusList { get; set; }
        /// <summary>
        /// 授权访问菜单
        /// </summary>
        public List<VueRouterModel> MenusRouter { get; set; }
        /// <summary>
        /// 授权使用功能
        /// </summary>
        public List<string> Modules { get; set; }
        /// <summary>
        /// 用户设置的软件主题
        /// </summary>
        public string UserTheme { get; set; }


        ///// <summary>
        /////是否报表用户， 
        ///// </summary>
        //public bool IsReport { get; set; }
        /// <summary>
        /// 程序类别
        /// </summary>
        public string ApplicationType { get; set; }
        /// <summary>
        /// 需要跳转的URL
        /// </summary>
        public string APPURL { get; set; }

        /// <summary>
        /// 用户类别
        /// </summary>
        public string UserType { get; set; }

        ///// <summary>
        /////是否超级管理员 
        ///// </summary>
        public bool IsAdmin { get; set; }

        public List<DNShift> DN_Shift { get; set; }

        /// <summary>
        /// 完整用户名
        /// </summary>
        public string UserFullName { get; set; }
    }
}