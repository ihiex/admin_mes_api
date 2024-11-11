using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class WH_BillNo
    {
        public virtual int FInterID { get; set; }
        public virtual int FEntryID { get; set; }
        public virtual int FDetailID { get; set; }
        public virtual string FKPONO { get; set; }
        public virtual string FMPNNO { get; set; }

        public virtual int FCTN { get; set; }
        public virtual int FStatus { get; set; }
        public virtual int FOutSN { get; set; }
    }
}

