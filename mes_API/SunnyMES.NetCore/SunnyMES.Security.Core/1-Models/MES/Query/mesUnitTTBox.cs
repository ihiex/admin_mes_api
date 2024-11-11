using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesUnitTTBox
    {
        public virtual int ID { get; set; }
        public virtual int UnitStateID { get; set; }
        public virtual int StatusID { get; set; }
        public virtual int StationID { get; set; }
        public virtual int EmployeeID { get; set; }
        public virtual DateTime? CreationTime { get; set; }
        public virtual DateTime? LastUpdate { get; set; }
        public virtual int PanelID { get; set; }
        public virtual int? LineID { get; set; }
        public virtual int? PartFamilyID { get; set; }
        public virtual int ProductionOrderID { get; set; }
        public virtual int RMAID { get; set; }
        public virtual int? PartID { get; set; }
        public virtual int LooperCount { get; set; }

        public virtual string SN { get; set; }        
        public virtual int ChildCount { get; set; }
        public virtual int FullNumber { get; set; }
        public virtual string BoxSNStatus { get; set; }

    }
}

