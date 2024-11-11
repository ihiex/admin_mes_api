﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.IServices;
using SunnyMES.Commons.Services;
using SunnyMES.Security.SysConfig.Models.PO;

namespace SunnyMES.Security.SysConfig.IServices.PO
{
    public interface ISC_mesLineOrderServices : ICustomService<SC_mesLineOrder, SC_mesLineOrder, string>
    {
    }
}
