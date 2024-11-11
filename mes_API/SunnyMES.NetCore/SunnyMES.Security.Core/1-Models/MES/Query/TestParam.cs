using System;
using System.Collections.Generic;

namespace SunnyMES.Security.Models;

[Serializable]
public struct TestParam
{
    public string S_PartFamilyTypeID { get; set; }
    public string S_PartFamilyID { get; set; }

    public string S_PartID { get; set; }
    public string S_POID { get; set; }
    public string S_UnitStatus { get; set; }
    public string S_URL { get; set; }
    public List<BomPartInfo> bomPartInfo { get; set; }
}