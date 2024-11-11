using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Models;
using SunnyMES.Commons.Core.PublicFun;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security.Dtos.MES;
using SunnyMES.Security.Models;

namespace SunnyMES.AspNetCore.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TServiceCommon"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    [ApiController]
    public abstract class AreaApiControllerCommon<TServiceCommon, TKey> : ApiController
    where TServiceCommon : ICommonService<TKey>
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
        public TServiceCommon iService;

        protected CommonHeader commonHeader = new CommonHeader();
        protected object throwMsg = "controller error";
        protected DateTime startTime;
        public string StationName = "";

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_iService"></param>
        public AreaApiControllerCommon(TServiceCommon _iService)
        {
            //ReportCurrentUser = CurrentUser;
            iService = _iService;
            startTime = DateTime.Now;
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
        }

        protected CommonResult Com_Result(CommonResult v_CommonResult, IEnumerable<dynamic> List_Dyn)
        {
            CommonResult F_CommonResult = new CommonResult();
            IEnumerable<dynamic> List_Result = List_Dyn;
            v_CommonResult.Sounds = S_Path_OK;

            foreach (var item in List_Dyn)
            {
                try
                {
                    if (item is TabVal)
                    {
                        TabVal v_TabVal = item as TabVal;

                        if (v_TabVal.ValStr3.Trim() == "1")
                        {
                            v_CommonResult.Success = true;
                            v_CommonResult.ResultCode = ErrCode.successCode;
                            v_CommonResult.ResultMsg = ErrCode.err0;
                            v_CommonResult.ResData = List_Result;
                        }
                        else
                        {
                            v_CommonResult.Success = false;
                            v_CommonResult.ResultCode = v_TabVal.ValStr1;
                            v_CommonResult.ResultMsg = v_TabVal.ValStr2;
                            v_CommonResult.ResData = null;
                            v_CommonResult.Sounds = v_TabVal.ValStr4;
                        }

                        F_CommonResult = v_CommonResult;
                        return F_CommonResult;
                    }
                    //else
                    //{
                    //    List_Result = List_Dyn;
                    //}
                    continue;
                }
                catch
                {
                    v_CommonResult.Success = true;
                    v_CommonResult.ResultCode = ErrCode.successCode;
                    v_CommonResult.ResultMsg = ErrCode.err0;
                    v_CommonResult.ResData = List_Result;
                    v_CommonResult.Sounds = S_Path_NG;

                    F_CommonResult = v_CommonResult;
                    return F_CommonResult;
                }
            }

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = ErrCode.err0;
            v_CommonResult.ResData = List_Result;
            F_CommonResult = v_CommonResult;
            return F_CommonResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        protected async Task<CommonResult> GetPublicErrorAsync(string sn, dynamic ErrorMsg = null, MesOutputDto tmpOutputDto = null)
        {
            CommonResult v_CommonResult = new CommonResult();
            v_CommonResult.Success = false;
            v_CommonResult.ResultCode = ErrCode.err1;

            string tmpMsg = string.Empty;
            if (string.IsNullOrEmpty(ErrorMsg))
            {
                tmpMsg = commonHeader.Language switch
                {
                    0 => ErrCode.err1,
                    1 => "request failed.",
                    _ => ErrCode.err0
                };
            }
            else
            {
                tmpMsg = ErrorMsg.ToString();
            }

            v_CommonResult.ResultMsg = FormatOutputMsg(sn, tmpMsg);
            v_CommonResult.ResData = tmpOutputDto;
            v_CommonResult.Sounds = S_Path_NG;
            await MesLogHelper.WriteLog(
                new MesLogDIO { CurrentIP = P_CurrentLoginIP, Msg = v_CommonResult.ResultMsg, Result = SoundType.NG, StationName = commonHeader?.StationName, StationId = P_StationID.ToString(), SN = sn });
            return v_CommonResult;
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="v_CommonResult"></param>
        /// <param name="List_Dyn"></param>
        /// <returns></returns>
        protected async Task<CommonResult> FormatResultAsync(CommonResult v_CommonResult, List<dynamic> List_Dyn, string _sn = "")
        {
            CommonResult F_CommonResult = new CommonResult();
            List<dynamic> List_Result = new List<dynamic>();
            Dictionary<string, dynamic> Dictionary_Dyn = new Dictionary<string, dynamic>();
            v_CommonResult.Sounds = S_Path_OK;

            if (List_Dyn.Count != 1)
                return await GetPublicErrorAsync(_sn);

            MesOutputDto tmpMesDto = List_Dyn[0] as MesOutputDto;
            if (tmpMesDto == null)
                return await GetPublicErrorAsync(_sn);

            StationName = tmpMesDto.CurrentSettingInfo.StationName;
            if (!string.IsNullOrEmpty(tmpMesDto.ErrorMsg))
                return await GetPublicErrorAsync(tmpMesDto.ErrorMsg);

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = FormatOutputMsg(_sn);
            v_CommonResult.ResData = tmpMesDto;
            v_CommonResult.Sounds = S_Path_OK;
            await MesLogHelper.WriteLog(new MesLogDIO
            {
                CurrentIP = P_CurrentLoginIP,
                Msg = v_CommonResult.ResultMsg,
                Result = SoundType.OK,
                StationName = StationName,
                StationId = P_StationID.ToString(),
                SN = _sn,
            });
            return v_CommonResult;
        }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="v_CommonResult"></param>
        /// <param name="List_Dyn"></param>
        /// <returns></returns>
        protected async Task<CommonResult> FormatResultAsync(CommonResult v_CommonResult, MesOutputDto mesOutput, string _sn = "")
        {
            CommonResult F_CommonResult = new CommonResult();
            List<dynamic> List_Result = new List<dynamic>();
            Dictionary<string, dynamic> Dictionary_Dyn = new Dictionary<string, dynamic>();
            v_CommonResult.Sounds = S_Path_OK;

            if (mesOutput == null)
                return await GetPublicErrorAsync(_sn);

            StationName = mesOutput.CurrentSettingInfo.StationName;
            if (!string.IsNullOrEmpty(mesOutput.ErrorMsg))
                return await GetPublicErrorAsync(_sn, mesOutput.ErrorMsg);

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = FormatOutputMsg(_sn);
            v_CommonResult.ResData = mesOutput;
            v_CommonResult.Sounds = S_Path_OK;
            await MesLogHelper.WriteLog(new MesLogDIO
            {
                CurrentIP = P_CurrentLoginIP,
                Msg = v_CommonResult.ResultMsg,
                Result = SoundType.OK,
                StationName = commonHeader?.StationName,
                StationId = P_StationID.ToString(),
                SN = _sn,
            });
            return v_CommonResult;
        }
        /// <summary>
        /// 针对动态组装窗口定制了一个输出方式
        /// 当NG时都会返回参数列表
        /// 其他页面是在NG时不返回参数列表
        /// </summary>
        /// <param name="v_CommonResult"></param>
        /// <param name="mesOutput"></param>
        /// <param name="_sn"></param>
        /// <returns></returns>
        protected async Task<CommonResult> FormatNGResultAsync(CommonResult v_CommonResult, MesOutputDto mesOutput, string _sn = "")
        {
            CommonResult F_CommonResult = new CommonResult();
            List<dynamic> List_Result = new List<dynamic>();
            Dictionary<string, dynamic> Dictionary_Dyn = new Dictionary<string, dynamic>();
            v_CommonResult.Sounds = S_Path_OK;

            if (mesOutput == null)
                return await GetPublicErrorAsync(_sn);

            StationName = mesOutput.CurrentSettingInfo.StationName;
            if (mesOutput.ErrorMsg != null  && mesOutput.ErrorMsg != "")
                return await GetPublicErrorAsync(_sn, mesOutput.ErrorMsg, mesOutput);

            v_CommonResult.Success = true;
            v_CommonResult.ResultCode = ErrCode.successCode;
            v_CommonResult.ResultMsg = FormatOutputMsg(_sn);
            v_CommonResult.ResData = mesOutput;
            v_CommonResult.Sounds = S_Path_OK;
            await MesLogHelper.WriteLog(new MesLogDIO
            {
                CurrentIP = P_CurrentLoginIP,
                Msg = v_CommonResult.ResultMsg,
                Result = SoundType.OK,
                StationName = commonHeader?.StationName,
                StationId = P_StationID.ToString(),
                SN = _sn,
            });
            return v_CommonResult;
        }
    }
}
