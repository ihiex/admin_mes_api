using SunnyMES.Security.Models;

namespace SunnyMES.Security._1_Models.MES.Query;

public class mesUnitSerial : mesUnit
{
    public int UnitID { get; set; }
    public int SerialNumberTypeID { get; set; }
    public string Value { get; set; }
}