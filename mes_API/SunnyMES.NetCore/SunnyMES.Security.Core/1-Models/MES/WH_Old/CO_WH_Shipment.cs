using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class CO_WH_Shipment
    {
        public virtual int FInterID { get; set; }
        public virtual string FBillNO { get; set; }
        public virtual DateTime FDate { get; set; }
        public virtual string HAWB { get; set; }

        public virtual int FPalletSeq { get; set; }
        public virtual int FPalletCount { get; set; }
        public virtual int FGrossweight { get; set; }
        public virtual int FEmptyCarton { get; set; }
        public virtual int FUserID { get; set; }

        public virtual DateTime FCreateDatetime { get; set; }
        public virtual int FStatus { get; set; }
        public virtual string FShipNO { get; set; }
        public virtual int FCTN { get; set; }
        public virtual string FProjectNO { get; set; }

        public virtual string MyStatus { get; set; }
    }
}

