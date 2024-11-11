﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class SC_IdDescription
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string SN { get; set; }
    }
}
