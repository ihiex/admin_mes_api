using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
    public class mesUnitComponent
    {
		public virtual int ID { get; set; }
		public virtual int UnitID { get; set; }
		public virtual int UnitComponentTypeID { get; set; }
		public virtual int ChildUnitID { get; set; }
		public virtual string ChildSerialNumber { get; set; }
		public virtual string ChildLotNumber { get; set; }
		public virtual int ChildPartID { get; set; }
		public virtual int ChildPartFamilyID { get; set; }
		public virtual string Position { get; set; }
		public virtual int InsertedEmployeeID { get; set; }
		public virtual int InsertedStationID { get; set; }
		public virtual DateTime? InsertedTime { get; set; }
		public virtual int RemovedEmployeeID { get; set; }
		public virtual int RemovedStationID { get; set; }
		public virtual DateTime? RemovedTime { get; set; }
		public virtual object StatusID { get; set; }
		public virtual DateTime? LastUpdate { get; set; }
		public virtual int PreviousLink { get; set; }
	}		    
}			    
			    
			    
			    
			    
			    
			    
			    
			    