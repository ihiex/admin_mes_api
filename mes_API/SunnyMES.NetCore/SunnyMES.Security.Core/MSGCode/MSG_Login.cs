using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.Commons.Core.PublicFun;

namespace API_MSG
{
    /// <summary>
    /// Login 消息代码描述
    /// </summary>
    public class MSG_Login
    {        
        /// <summary>
        /// 验证码错误
        /// </summary>
        public string MSG_Login_001;

        /// <summary>
        /// 该IP已被管理员禁止登录
        /// </summary>
        public string MSG_Login_002;
        /// <summary>
        /// 用户名不能为空
        /// </summary>
        public string MSG_Login_003;

        /// <summary>
        /// 密码不能为空
        /// </summary>
        public string MSG_Login_004;

        /// <summary>
        /// 线别不能为空
        /// </summary>
        public string MSG_Login_005;

        /// <summary>
        /// 工站不能为空
        /// </summary>
        public string MSG_Login_006 ;

        /// <summary>
        /// 你当前请求主机：
        /// </summary>
        public string MSG_Login_007;

        /// <summary>
        /// 登录
        /// </summary>
        public string MSG_Login_008;

        /// <summary>
        /// 登录成功
        /// </summary>
        public string MSG_Login_009;

        /// <summary>
        /// 登录失败
        /// </summary>
        public string MSG_Login_010;
        /// <summary>
        /// 统不存在该用户，请重新确认。
        /// </summary>
        public string MSG_Login_011;
        /// <summary>
        /// 该用户已被禁用，请联系管理员。
        /// </summary>
        public string MSG_Login_012;
        /// <summary>
        /// 密码错误，请重新输入。
        /// </summary>
        public string MSG_Login_013;
        /// <summary>
        /// 您的账号已过期，请联系系统管理员！
        /// </summary>
        public string MSG_Login_014;
        /// <summary>
        /// 当前用户被锁定。
        /// </summary>
        public string MSG_Login_015;
        /// <summary>
        /// 线别或工站系统中不存在
        /// </summary>
        public string MSG_Login_016;
        /// <summary>
        /// 没有权限
        /// </summary>
        public string MSG_Login_017;
        /// <summary>
        /// 当前用户的角色已禁用
        /// </summary>
        public string MSG_Login_018;

        /// <summary>
        /// MSG_Login
        /// </summary>
        /// <param name="v_Language">语言 0:ZH_CN 1:EN</param>
        public MSG_Login(int v_Language)
        {
            int I_Language = v_Language;
            MSG_Login_001 = "MSG_Login_001:" + PublicF.GetLangStr("验证码错误!@" +
                                                        "Verification code error!", I_Language);

            MSG_Login_002 = "MSG_Login_002:" + PublicF.GetLangStr("该IP已被管理员禁止登录！@ " +
                                                         "This IP has been blocked by the administrator!", I_Language);

            MSG_Login_003 = "MSG_Login_003:" + PublicF.GetLangStr("用户名不能为空！@ " +
                                                         "User name cannot be empty!", I_Language);

            MSG_Login_004 = "MSG_Login_004:" + PublicF.GetLangStr("密码不能为空！@ " +
                                                         "password cannot be empty!", I_Language);

            MSG_Login_005 = "MSG_Login_005:" + PublicF.GetLangStr("线别不能为空！@ " +
                                                         " LineID cannot be empty!", I_Language);

            MSG_Login_006 = "MSG_Login_006:" + PublicF.GetLangStr("工站不能为空！@ " +
                                                         " Station cannot be empty!", I_Language);

            MSG_Login_007 = "MSG_Login_007:" + PublicF.GetLangStr("你当前请求主机：@ " +
                                                         "You are currently requesting the host:", I_Language);

            MSG_Login_008 = "MSG_Login_008:" + PublicF.GetLangStr("登录@ " +
                                                         " Login", I_Language);

            MSG_Login_009 = "MSG_Login_009:" + PublicF.GetLangStr("登录成功@ " +
                                                         " Login successful", I_Language);

            MSG_Login_010 = "MSG_Login_010:" + PublicF.GetLangStr("登录失败@ " +
                                                         " Login failed", I_Language);

            MSG_Login_011 = "MSG_Login_011:" + PublicF.GetLangStr("统不存在该用户，请重新确认。@ " +
                                                         " The user does not exist in the system. Please confirm. ", I_Language);

            MSG_Login_012 = "MSG_Login_012:" + PublicF.GetLangStr("该用户已被禁用，请联系管理员。@ " +
                                                         " The user has been disabled. Please contact your administrator. ", I_Language);

            MSG_Login_013 = "MSG_Login_013:" + PublicF.GetLangStr("密码错误，请重新输入。@ " +
                                                         " The password is incorrect. Please re-enter it. ", I_Language);

            MSG_Login_014 = "MSG_Login_014:" + PublicF.GetLangStr("您的账号已过期，请联系系统管理员！@ " +
                                                         " Your account has expired, please contact the system administrator! ", I_Language);

            MSG_Login_015 = "MSG_Login_015:" + PublicF.GetLangStr("当前用户被锁定！@ " +
                                                         " The current user is locked. ", I_Language);

            MSG_Login_016 = "MSG_Login_016:" + PublicF.GetLangStr("线别或工站系统中不存在！@ " +
                                                         "  Line or Station system does not exist！ ", I_Language);
            MSG_Login_017 = "MSG_Login_017:" + PublicF.GetLangStr("没有权限！@ " +
                                                         "  No login permission！ ", I_Language);
            MSG_Login_018 = "MSG_Login_018:" + PublicF.GetLangStr("当前用户的角色已禁用！@ " +
                                             "  current role disenabled！ ", I_Language);
        }


    }
}
