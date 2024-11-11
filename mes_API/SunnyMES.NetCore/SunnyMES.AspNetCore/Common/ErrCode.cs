using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Common;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Linq;

namespace SunnyMES.AspNetCore.Models
{
    /// <summary>
    /// 错误代码描述
    /// </summary>
    public static class ErrCode
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        [Description("request successes.")]
        public static string err0 = "请求成功";

        /// <summary>
        /// 请求成功代码0
        /// </summary>
        public static string successCode = "0";

        /// <summary>
        /// 请求失败
        /// </summary>
        [Description("request failed.")]
        public static string err1 = "请求失败";

        /// <summary>
        /// 请求失败代码1
        /// </summary>
        public static string failCode = "1";

        /// <summary>
        /// 获取access_token时AppID或AppSecret错误。请开发者认真比对appid和AppSecret的正确性，或查看是否正在为恰当的应用调用接口
        /// </summary>
        public static string err40001 = "获取access_token时AppID或AppSecret错误。请开发者认真比对appid和AppSecret的正确性，或查看是否正在为恰当的应用调用接口";

        /// <summary>
        /// 调用接口的服务器URL地址不正确，请联系供应商进行设置
        /// </summary>
        public static string err40002 = "调用接口的服务器URL地址不正确，请联系供应商进行授权";

        /// <summary>
        /// 请确保grant_type字段值为client_credential
        /// </summary>
        public static string err40003 = "请确保grant_type字段值为client_credential";

        /// <summary>
        /// 不合法的凭证类型
        /// </summary>
        public static string err40004 = "不合法的凭证类型";

        /// <summary>
        /// 用户令牌accesstoken超时失效
        /// </summary>
        public static string err40005 = "用户令牌accesstoken超时失效";

        /// <summary>
        /// 您未被授权使用该功能，请重新登录试试或联系管理员进行处理
        /// </summary>
        public static string err40006 = "您未被授权使用该功能，请重新登录试试或联系系统管理员进行处理";

        /// <summary>
        /// 传递参数出现错误
        /// </summary>
        public static string err40007 = "传递参数出现错误";

        /// <summary>
        /// 用户未登录或超时
        /// </summary>
        public static string err40008 = "用户未登录或超时";
        /// <summary>
        /// 程序异常
        /// </summary>
        public static string err40110 = "程序异常";

        /// <summary>
        /// 新增数据失败
        /// </summary>
        [Description("Insert data failed.")]
        public static string err43001 = "新增数据失败";

        /// <summary>
        /// 更新数据失败
        /// </summary>
        [Description("Update data failed.")]
        public static string err43002 = "更新数据失败";

        /// <summary>
        /// 物理删除数据失败
        /// </summary>
        [Description("Delete data failed.")]
        public static string err43003 = "删除数据失败";

        /// <summary>
        /// 该用户不存在
        /// </summary>
        public static string err50001 = "该用户不存在";

        /// <summary>
        /// 该用户已存在
        /// </summary>
        public static string err50002 = "用户已存在，请登录或重新注册！";

        /// <summary>
        /// 会员注册失败
        /// </summary>
        public static string err50003 = "会员注册失败";

        /// <summary>
        /// 查询数据不存在
        /// </summary>
        public static string err60001 = "查询数据不存在";

        /// <summary>
        /// 数据已存在
        /// </summary>
        [Description("Data already exists.")]
        public static string err70001 = "数据已存在";
        /// <summary>
        /// 数据信息异常
        /// </summary>
        [Description("Data exception.")]
        public static string err70002 = "数据信息异常";

        /// <summary>
        /// 父治具数据已存在"
        /// </summary>
        [Description("Parent tooling data already exists.")]
        public static string err70003 = "父治具数据已存在";
        /// <summary>
        /// 开始结束时间不能相同
        /// </summary>
        [Description("Start time same as end time.")]
        public static string err70004 = "开始结束时间不能相同";
        /// <summary>
        /// 请正确的输入开始结束时间
        /// </summary>
        [Description("Please input the correct datetime.")]
        public static string err70005 = "请正确的输入开始结束时间";
    }
}
