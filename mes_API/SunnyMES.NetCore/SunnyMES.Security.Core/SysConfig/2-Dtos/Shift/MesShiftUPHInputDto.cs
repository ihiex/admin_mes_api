using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SunnyMES.Security.SysConfig.Dtos.Shift
{
    [Serializable]
    [DataContract]
    public class MesShiftUPHInputDto
    {
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public DateTime EndTime { get; set; }

        [DataMember]
        public List<int> ShiftIds { get; set; } = new List<int>();
        [DataMember]
        public List<int> LineIds { get; set; } = new List<int>();
        [DataMember]
        public int UPH { get; set; }
        [DataMember]
        public double YieldTarget { get; set; }
        [DataMember]
        public DateTime CreateTime { get; set; }
        [DataMember]
        public DateTime UpdateTime { get; set; }
    }
}
