using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.IServices;
using SunnyMES.Security.IServices.Public;
using SunnyMES.Security.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NPOI.SS.Formula.Functions;
using Senparc.NeuChar.Entities;
using SunnyMES.Commons.Core.PublicFun.Model;
using SunnyMES.Security.Services;
using SunnyMES.Security.ToolExtensions;
using SunnyMES.Commons.Json;
using SunnyMES.Security.SysConfig.Models.Public;
using SunnyMES.AspNetCore.Common;
using SunnyMES.Commons.Dtos;
using SunnyMES.Commons.Pages;
using SunnyMES.Security.SysConfig.Dtos.Public;

namespace SunnyMES.WebApi.SysConfig.Public
{
    /// <summary>
    ///  通用属性定义接口
    /// </summary>
    [ApiController]
    [Route("api/SysConfig/[controller]")]
    [ApiVersion("3.0")]
    public class PublicPropertiesController : AreaApiControllerCommon<IPublicPropertiesService, string>
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_iService"></param>
        public PublicPropertiesController(IPublicPropertiesService _iService) : base(_iService)
        {
            iService = _iService;
        }
        /// <summary>
        /// 获取所有通用表名
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCommonTabList")]
        [YuebonAuthorize("")]
        public async Task<IActionResult> GetCommonTabList()
        {
            CommonResult result = new CommonResult();
            try
            {
                List<SC_IdDesc> list = await iService.GetCommonTabList();
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.ResData = list;
            }
            catch (Exception ex)
            {
                Log4NetHelper.Error("获取 GetCommonTabList 异常", ex);
                result.ResultMsg = ErrCode.err40110 + "\r\n" + ex.Message;
                result.ResultCode = "40110";
            }
            return ToJsonContent(result);
        }


        /// <summary>
        /// 写Log
        /// </summary>
        /// <param name="info"></param>
        /// <param name="otherCode">其他类型功能代码</param>
        private void WriteLog(SC_IdDescTab info, string LogFunctionCode, string otherCode = "")
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
                            TableName = info.TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    case "Delete":
                        await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        {
                            CurrentIP = commonHeader.CurrentLoginIp,
                            DataType = LogFunctionCode,
                            TableName = info.TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    case "Update":
                        await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        {
                            CurrentIP = commonHeader.CurrentLoginIp,
                            DataType = LogFunctionCode,
                            TableName = info.TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    case "Clone":
                        //重写
                        await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                        {
                            CurrentIP = commonHeader.CurrentLoginIp,
                            DataType = LogFunctionCode,
                            TableName = info.TableName,
                            UserName = CurrentUser.UserFullName,
                            MSG = info.ToJson()
                        });
                        break;
                    default:
                        if (!string.IsNullOrEmpty(otherCode))
                        {
                            await SysConfigLogHelper.WriteLog(new SysConfigLogDIO()
                            {
                                CurrentIP = commonHeader.CurrentLoginIp,
                                DataType = otherCode,
                                TableName = info.TableName,
                                UserName = CurrentUser.UserFullName,
                                MSG = info.ToJson()
                            });
                        }
                        break;
                }
            });

        }

        /// <summary>
        /// 根据语言返回对应的消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private string ShowMsg(string msg)
        {
            var v = msg.LanguageMsg();
            return P_Language == 0 ? v.CN : v.EN;
        }

        /// <summary>
        /// 异步新增数据
        /// </summary>
        /// <param name="tinfo"></param>
        /// <returns></returns>
        [HttpPost("InsertAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> InsertAsync(SC_IdDescTab tinfo)
        {
            CommonResult result = new CommonResult();
            string sql = $"SELECT 1 FROM {tinfo.TableName} WHERE Description = '{tinfo.Description}'";
            var exists = await iService.CheckIsExists(sql);
            if (exists)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            long ln = await iService.InsertAsync(tinfo).ConfigureAwait(false);
            if (ln > 0)
            {
                WriteLog(tinfo, "Insert");
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
        public virtual async Task<IActionResult> UpdateAsync(SC_IdDescTab tinfo)
        {
            CommonResult result = new CommonResult();

            string sql = $"SELECT 1 FROM {tinfo.TableName} WHERE Description = '{tinfo.Description}' and ID <> {tinfo.ID}";
            var exists = await iService.CheckIsExists(sql);
            if (exists)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            long ln = await iService.UpdateAsync(tinfo).ConfigureAwait(false);
            if (ln > 0)
            {
                WriteLog(tinfo, "Update");
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
        /// 异步物理删除
        /// </summary>
        /// <param name="id">主键Id</param>
        [HttpDelete("DeleteAsync")]
        [YuebonAuthorize("")]
        public virtual async Task<IActionResult> DeleteAsync(SC_IdDescTab tinfo)
        {
            CommonResult result = new CommonResult();

            string sql = $"SELECT 1 FROM {tinfo.TableName} WHERE  ID = {tinfo.ID}";
            var exists = await iService.CheckIsExists(sql);
            if (!exists)
            {
                result.ResultMsg = ShowMsg(ErrCode.err60001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            long bl = await iService.DeleteAsync(tinfo).ConfigureAwait(false);
            if (bl> 0)
            {
                WriteLog(tinfo, "Delete");
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
        public virtual async Task<IActionResult> CloneAsync(SC_IdDescTab tinfo)
        {
            CommonResult result = new CommonResult();


            string sql = $"SELECT 1 FROM {tinfo.TableName} WHERE Description = '{tinfo.Description}'";
            var exists = await iService.CheckIsExists(sql);
            if (exists)
            {
                result.ResultMsg = ShowMsg(ErrCode.err70001);
                result.ResultCode = ErrCode.err1;
                result.Sounds = S_Path_NG;
                result.Success = false;
                return ToJsonContent(result);
            }

            long ln = await iService.CloneAsync(tinfo).ConfigureAwait(false);
            if (ln > 0)
            {
                result.ResultCode = ErrCode.successCode;
                result.ResultMsg = ErrCode.err0;
                result.Sounds = S_Path_OK;
                WriteLog(tinfo, "Clone");
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

        /// <summary>
        /// 根据条件查询数据库,并返回对象集合(用于分页数据显示)
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpPost("FindWithPagerAsync")]
        [YuebonAuthorize("List")]
        public virtual async Task<CommonResult<PageResult<SC_IdDesc>>> FindWithPagerAsync(SearchPropertiesInputDto search)
        {
            CommonResult<PageResult<SC_IdDesc>> result = new CommonResult<PageResult<SC_IdDesc>>();
            result.ResData = await iService.FindWithPagerAsync(search);
            result.ResultCode = ErrCode.successCode;
            result.ResultMsg = ErrCode.err0;
            result.Sounds = S_Path_OK;
            return result;
        }
    }
}
