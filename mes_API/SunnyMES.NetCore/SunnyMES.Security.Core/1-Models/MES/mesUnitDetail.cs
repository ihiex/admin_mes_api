using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models
{
    [Serializable]
	public class mesUnitDetail
	{
		public virtual int ID { get; set; }
		public virtual int UnitID { get; set; }
		public virtual int ProductionOrderID { get; set; }
		public virtual int RMAID { get; set; }
		public virtual int LooperCount { get; set; }
		public virtual string KitSerialNumber { get; set; }
		public virtual int InmostPackageID { get; set; }
		public virtual int OutmostPackageID { get; set; }
		public virtual string reserved_01 { get; set; }
		public virtual string reserved_02 { get; set; }
		public virtual string reserved_03 { get; set; }
		public virtual string reserved_04 { get; set; }
		public virtual string reserved_05 { get; set; }
		public virtual string reserved_06 { get; set; }
		public virtual string reserved_07 { get; set; }
		public virtual string reserved_08 { get; set; }
		public virtual string reserved_09 { get; set; }
		public virtual string reserved_10 { get; set; }
		public virtual string reserved_11 { get; set; }
		public virtual string reserved_12 { get; set; }
		public virtual string reserved_13 { get; set; }
		public virtual string reserved_14 { get; set; }
		public virtual string reserved_15 { get; set; }
		public virtual string reserved_16 { get; set; }
		public virtual string reserved_17 { get; set; }
		public virtual string reserved_18 { get; set; }
		public virtual string reserved_19 { get; set; }
		public virtual string reserved_20 { get; set; }
	}		    
}			    
			    
			    
			    
			    
			    
			    