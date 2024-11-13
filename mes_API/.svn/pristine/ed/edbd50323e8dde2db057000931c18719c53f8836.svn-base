using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc.Filter;
using SunnyMES.Commons.Cache;
using SunnyMES.Commons.Models;
using SunnyMES.Commons.Net;
using SunnyMES.Commons.VerificationCode;
using SunnyMES.Security.Dtos;

namespace SunnyMES.WebApi.Controllers
{
    /// <summary>
    /// 验证码接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CaptchaController : ApiController
    {
        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [NoPermissionRequired]
        public async Task<CommonResult<AuthGetVerifyCodeOutputDto>> CaptchaAsync()
        {
            Captcha captcha = new Captcha();
            var code =await  captcha.GenerateRandomCaptchaAsync().ConfigureAwait(false);
            var result =await  captcha.GenerateCaptchaImageAsync(code);
            YuebonCacheHelper yuebonCacheHelper = new YuebonCacheHelper();
            TimeSpan expiresSliding = DateTime.Now.AddMinutes(5) - DateTime.Now;
            
            yuebonCacheHelper.Add("ValidateCode"+ result.Timestamp.ToString("yyyyMMddHHmmssffff"), code, expiresSliding,false);
            AuthGetVerifyCodeOutputDto authGetVerifyCodeOutputDto = new AuthGetVerifyCodeOutputDto();
            authGetVerifyCodeOutputDto.Img =Convert.ToBase64String(result.CaptchaMemoryStream.ToArray());
            authGetVerifyCodeOutputDto.Key = result.Timestamp.ToString("yyyyMMddHHmmssffff");
            CommonResult<AuthGetVerifyCodeOutputDto> commonResult = new CommonResult<AuthGetVerifyCodeOutputDto>();
            commonResult.ResultCode= ErrCode.successCode;
            commonResult.ResData = authGetVerifyCodeOutputDto;
            return commonResult;
        }
    }
}
