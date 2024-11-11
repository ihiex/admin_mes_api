﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Enums;

namespace SunnyMES.Commons.Core.PublicFun.Model
{
    public class SysConfigLogDIO
    {
        //public string StationId { get; set; }
        //public string StationName { get; set; } = "";
        //public string CurrentIP { get; set; }
        //public string SN { get; set; }

        //public SoundType Result { get; set; } = SoundType.OK;
        //public string Msg { get; set; }

        public string CurrentIP { get; set; }
        public string UserName { get; set; }        
        public string DataType { get; set; }
        public string TableName { get; set; }
        public string MSG { get; set; }

    }
}
