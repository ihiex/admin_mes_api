using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class LabelMap
    {
        public virtual int ID { get; set; }
        public virtual int StationTypeID { get; set; }

        public virtual string StationType { get; set; }
        public virtual int LabelID { get; set; }
        public virtual string LabelName { get; set; }
        public virtual string LabelPath { get; set; }

        public virtual int PartFamilyID { get; set; }
        public virtual string PartFamily { get; set; }

        public virtual int PartID { get; set; }
        public virtual string PartNumber { get; set; }

        public virtual int ProductionOrderID { get; set; }
        public virtual string ProductionOrderNumber { get; set; }

        public virtual int LineID { get; set; }
        public virtual string Line { get; set; }

        public virtual string OutputType { get; set; }
        public virtual string TargetPath { get; set; }
        public virtual string PageCapacity { get; set; }
    }
}




