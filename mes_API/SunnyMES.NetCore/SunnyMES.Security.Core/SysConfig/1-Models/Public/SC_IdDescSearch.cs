﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SC_IdDescSearch : PagerInfo
    {
        public string ID { get; set; }
        public string Description { get; set; }

        public string LikeQuery { get; set; }
    }
}