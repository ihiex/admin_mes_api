using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class WH_OutDT
    {
        public virtual string NO  { get; set; }
        public virtual string BoxID { get; set; }
        public virtual string PalletID { get; set; }
        public virtual string SerialNumber { get; set; }
        public virtual string isReceipt { get; set; }
        public virtual string isShipping { get; set; }

        public virtual DateTime ReceiptDate { get; set; }
        public virtual string ShippingDate { get; set; }
    }
}

