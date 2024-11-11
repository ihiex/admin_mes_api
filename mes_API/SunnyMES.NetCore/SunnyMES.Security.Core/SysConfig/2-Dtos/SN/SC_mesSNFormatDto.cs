﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SunnyMES.Commons.Helpers;
using SunnyMES.Commons.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SunnyMES.Security.Models
{
    public class SC_mesSNFormatDto
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SNFamilyID { get; set; }
        public string SNFormat { get; set; }

    }
}
