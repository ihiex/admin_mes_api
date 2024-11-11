using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesLineOrder
    {
        public virtual int ID { get; set; }        
        public virtual string Description { get; set; }
        public virtual int LineQuantity { get; set; }
        public virtual DateTime? CreationTime { get; set; }
        public virtual int StartedQuantity { get; set; }
        public virtual int ReadyQuantity { get; set; }
        public virtual int AllowOverBuild { get; set; }
        public virtual int LineID { get; set; }
        public virtual int ProductionOrderID { get; set; }
        public virtual int Priority { get; set; }

    }
}
