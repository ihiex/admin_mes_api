using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class WH_LCount
    {
        public virtual int LCount { get; set; }
        public virtual DateTime LDate { get; set; }
    }
}

