using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using SunnyMES.AspNetCore.Controllers;
using SunnyMES.AspNetCore.Models;
using SunnyMES.AspNetCore.Mvc;
using SunnyMES.Commons.Log;
using SunnyMES.Commons.Models;
using SunnyMES.Security.IServices;
using SunnyMES.Security.Models;
using SunnyMES.AspNetCore.Mvc.Filter;

namespace SunnyMES.WebApi.Areas.MES.Controllers
{
    /// <summary>
    /// 测试接口
    /// </summary>
    [ApiController]
    [Route("api/Mes/[controller]")]
    [ApiVersion("1.0")]
    [Obsolete]
    public class TestController : AreaApiControllerReport<IPublicService, string>
    {
        private readonly IUserService userService;

        public TestController(IPublicService _iService, IUserService userService) : base(_iService)
        {
            this.userService = userService;
        }




    }
}
