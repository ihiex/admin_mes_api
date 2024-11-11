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
    public class SC_mesPartFamilyDetailServices : BaseCustomService<SC_mesPartFamilyDetail, SC_mesPartFamilyDetail, string>, ISC_mesPartFamilyDetailServices
    {
        public SC_mesPartFamilyDetailServices(ISC_mesPartFamilyDetailRepositories repositories) : base(repositories)
        {

        }


    }
}
