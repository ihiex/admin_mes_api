using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Mapping;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.Dtos;

namespace SunnyMES.AspNetCore.Controllers
{
    /// <summary>
    /// Report 基本控制器
    /// </summary>
    /// <typeparam name="TServiceReport">Service类型</typeparam>
    /// <typeparam name="TKey">主键数据类型</typeparam>
    [ApiController]
    public abstract class AreaApiControllerReport<TServiceReport, TKey> : ApiController
        where TServiceReport : IServiceReport<TKey>             
        where TKey : IEquatable<TKey>
    {

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


        /// <summary>
        /// 服务接口
        /// </summary>
        public TServiceReport iService;
        ///// <summary>
        ///// 
        ///// </summary>
        //public YuebonCurrentUser ReportCurrentUser;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_iService"></param>
        public AreaApiControllerReport(TServiceReport _iService)
        {
            //ReportCurrentUser = CurrentUser;
            iService = _iService;
        }



 


    }
}
