using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Security.Models;

[Serializable]
public class ReworkData
{
    public int ID { get; set; }
    public string SerialNumber { get; set; }
    public string Part { get; set; }
    public DateTime? InsertedTime { get; set; }
    public string UnitID { get; set; }
}
