using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Pages;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Dtos.Part;
using SunnyMES.Security.SysConfig.IRepositories.Part;
using SunnyMES.Security.SysConfig.IServices.Part;
using SunnyMES.Security.SysConfig.Models.Part;

namespace SunnyMES.Security.SysConfig.Services.Part
{
    public class SC_mesPartDetailServices : BaseCustomService<SC_mesPartDetail, SC_mesPartDetail, string>, ISC_mesPartDetailServices
    {
        public SC_mesPartDetailServices(ISC_mesPartDetailRepositories mesPartDetailRepositories) : base(mesPartDetailRepositories)
        {

        }

    }
}
