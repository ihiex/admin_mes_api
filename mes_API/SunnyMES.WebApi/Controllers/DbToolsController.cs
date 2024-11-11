﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.AspNetCore.ViewModel;
using SunnyMES.Commons.Encrypt;
using SunnyMES.Commons.Models;

namespace SunnyMES.WebApi.Controllers
{
    /// <summary>
    /// 数据库连接加解密
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class DbToolsController : ApiController
    {
        /// <summary>
        /// 连接字符串加密
        /// </summary>
        /// <param name="dbConnInfo"></param>
        /// <returns></returns>
        [HttpPost("ConnStrEncrypt")]
        [YuebonAuthorize("ConnStrEncrypt")]
        public async Task<IActionResult> ConnStrEncrypt([FromQuery]DbConnInfo dbConnInfo)
        {
            CommonResult result = new CommonResult();
            DBConnResult dBConnResult = new DBConnResult();
            if (dbConnInfo != null)
            {
                if (string.IsNullOrEmpty(dbConnInfo.DbName))
                {
                    result.ResultMsg = "数据库名称不能为空";

                }
                else if (string.IsNullOrEmpty(dbConnInfo.DbAddress))
                {
                    result.ResultMsg = "访问地址不能为空";
                }
                else if (string.IsNullOrEmpty(dbConnInfo.DbUserName))
                {
                    result.ResultMsg = "访问用户不能为空";
                }
                else if (string.IsNullOrEmpty(dbConnInfo.DbPassword))
                {
                    result.ResultMsg = "访问密码不能为空";
                }
                if (dbConnInfo.DbType == "SqlServer")
                {
                    dBConnResult.ConnStr = string.Format("Server={0};Database={1};User id={2}; password={3};MultipleActiveResultSets=True;", dbConnInfo.DbAddress, dbConnInfo.DbName, dbConnInfo.DbUserName, dbConnInfo.DbPassword);
                    dBConnResult.EncryptConnStr = DEncrypt.Encrypt(dBConnResult.ConnStr);
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                }
                else if (dbConnInfo.DbType == "MySql")
                {
                    dBConnResult.ConnStr = string.Format("server={0};database={1};uid={2}; pwd={3};", dbConnInfo.DbAddress, dbConnInfo.DbName, dbConnInfo.DbUserName, dbConnInfo.DbPassword);
                    dBConnResult.EncryptConnStr = DEncrypt.Encrypt(dBConnResult.ConnStr);
                    result.Success = true;
                    result.ResultCode = ErrCode.successCode;
                }
                result.ResData = dBConnResult;

            }
            return ToJsonContent(result);
        }

        /// <summary>
        /// 连接字符串解密
        /// </summary>
        /// <returns></returns>

        [HttpPost("ConnStrDecrypt")]
        [YuebonAuthorize("ConnStrDecrypt")]
        public IActionResult ConnStrDecrypt(string strConn)
        {
            CommonResult result = new CommonResult();
            DBConnResult dBConnResult = new DBConnResult();
            if (string.IsNullOrEmpty(strConn))
            {
                result.ResultMsg = "数据库名称不能为空";
            }
            else
            {
                dBConnResult.ConnStr = DEncrypt.Decrypt(strConn);
                result.Success = true;
                result.ResultCode = ErrCode.successCode;
            }
            result.ResData = dBConnResult;
            return ToJsonContent(result);
        }
    }
}