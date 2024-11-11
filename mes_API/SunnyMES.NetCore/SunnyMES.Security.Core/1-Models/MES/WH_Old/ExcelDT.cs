using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class ExcelDT
    {
        public virtual string memo { get; set; }
        public virtual Boolean import { get; set; }
        public virtual DateTime Ship_date { get; set; }
        public virtual string project { get; set; }
        public virtual string HAWB_ { get; set; }
        public virtual string HUB_CODE { get; set; }
        public virtual string COUNTRY { get; set; }
        public virtual string REGION { get; set; }
        public virtual string KPO_ { get; set; }
        public virtual string MPN { get;  set; }
        public virtual int Q_ty { get; set;}
        public virtual int Carton { get; set; }
        public virtual int Pallet { get; set; }
        public virtual int Line { get; set; }
        public virtual string CarNO { get; set; }
        public virtual string 备注 { get; set; }

    }
}

