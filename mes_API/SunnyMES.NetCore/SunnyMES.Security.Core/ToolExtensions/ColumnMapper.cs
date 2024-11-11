using Dapper;
using SunnyMES.Commons.Mapping;
using SunnyMES.Security.Models.MES.SAP;

namespace SunnyMES.Security.ToolExtensions

{
    public class ColumnMapper
    {
        public static void SetMapper()
        {
            //数据库字段名和c#属性名不一致，手动添加映射关系
            SqlMapper.SetTypeMap(typeof(tmpExcelShipmentNew), new ColumnAttributeTypeMapper<tmpExcelShipmentNew>());

            //每个需要用到[colmun(Name="")]特性的model，都要在这里添加映射
        }
    }
}
