using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesUnitDefect
    {
        public virtual int ID { get; set; }
        public virtual int UnitID { get; set; }
        public virtual int DefectID { get; set; }
        public virtual int StationID { get; set; }
        public virtual int EmployeeID { get; set; }
        public virtual DateTime CreationTime { get; set; }
               
    }          
}              
               
               
               
               
               
               
               
               
               