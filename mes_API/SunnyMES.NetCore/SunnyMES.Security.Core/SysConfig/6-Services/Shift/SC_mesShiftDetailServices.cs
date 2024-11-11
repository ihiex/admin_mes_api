using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.IRepositories.Shift;
using SunnyMES.Security.SysConfig.Models.Shift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.SysConfig._5_IServices.Shift
{
    public class SC_mesShiftDetailServices:BaseCustomService<SC_mesShiftDetail, SC_mesShiftDetail,int>,ISC_mesShiftDetailServices
    {
        public SC_mesShiftDetailServices(ISC_mesShiftDetailRepositories shiftDetailRepositories):base(shiftDetailRepositories) 
        {
            
        }
    }
}
