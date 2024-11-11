﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SunnyMES.Commons.Pages;

namespace SunnyMES.Commons.CodeGenerator
{
    /// <summary>
    /// mysql
    /// </summary>
    public class MySqlExtractor : DbExtractorAbstract
    {

        /// <summary>
        /// 获取数据库信息
        /// </summary>
        /// <returns></returns>
        public List<DataBaseInfo> GetAllDataBases()
        {
            var sql = string.Format(@"select schema_name as DbName from information_schema.schemata");
            return GetAllDataBaseInternal(sql);
        }
        /// <summary>
        /// 获取数据库的所有表的信息
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableList"></param>
        /// <returns></returns>
        public List<DbTableInfo> GetAllTables(string dbName,string tableList)
        {
            var sql = string.Format(@"select table_name as TableName,TABLE_COMMENT as Description from information_schema.tables where table_schema='{0}' ", dbName);
            if (!string.IsNullOrEmpty(tableList))
            {
                sql += string.Format(@" and table_name in('{0}')", tableList.Replace(",", "','"));
            }
            return GetAllTablesInternal(sql);
        }


        /// <summary>
        /// 获取数据库的所有表的信息
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tablename"></param>
        /// <param name="fieldNameToSort"></param>
        /// <param name="isDescending"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public List<DbTableInfo> GetAllTables(string dbName,string tablename, string fieldNameToSort, bool isDescending, PagerInfo info)
        {
            string where = "1=1";
            if (!string.IsNullOrEmpty(tablename))
            {
                where += " and table_name like '%" + tablename + "%'";
            }
            var sql = string.Format(@"select table_name AS TableName,TABLE_COMMENT as Description from information_schema.tables where table_schema='{0}' and {1} ", dbName, where);

            string sqlcount = string.Format(@"select count(*) as Total from({0}) AA ", sql);

            string strOrder = string.Format(" order by table_name {0}", isDescending ? "DESC" : "ASC");
            int minRow = info.PageSize * (info.CurrentPageIndex - 1) + 1;
            int maxRow = info.PageSize * info.CurrentPageIndex;

            string pagesql = string.Format(@" {0} and  {1} {2} LIMIT {3},{4}", sql, where, strOrder,minRow-1, maxRow);
            pagesql = sqlcount + ";" + pagesql;
            return GetAllTablesInternal(pagesql, info);
        }
        /// <summary>
        /// 获取表的所有字段名及字段类型
        /// </summary>
        /// <param name="dbName">dbName</param>
        /// <param name="tableName">数据表的名称</param>
        /// <returns></returns>
        public List<DbFieldInfo> GetAllColumns(string dbName, string tableName)
        {
            if (tableName == null)
                throw new ArgumentNullException(nameof(tableName));

            var sqlFields = string.Format(@"select 
column_name as FieldName,
(case when is_nullable = 'YES' then '1' else '0'  end)  as  IsNullable,
 (case when Column_key = 'PRI' then '1' else '0'  end) as IsIdentity,
data_type as FieldType,
 (case when column_default is null then '' else column_default  end)  as FieldDefaultValue,
(case when character_maximum_length is null then 0 else character_maximum_length  end)  as FieldMaxLength,
(case when Numeric_Precision is null then 0 else Numeric_Precision  end)  as FieldPrecision,
(case when Numeric_scale is null then 0 else Numeric_scale  end)   as FieldScale,
column_comment as Description
from information_schema.columns where table_schema='{0}' and table_name='{1}' ORDER BY ORDINAL_POSITION asc;", dbName,tableName);
            List<DbFieldInfo> list = new List<DbFieldInfo>();
            list = GetAllColumnsInternal(sqlFields);
            List<DbFieldInfo> reslist = new List<DbFieldInfo>();
            foreach (DbFieldInfo info in list)
            {
                info.DataType = ConvertDataType(info);
                reslist.Add(info);
            }
            return reslist;
        }


        #region 字段转换
        /// <summary>
        /// 将字段信息的类型转换为C#信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public string ConvertDataType(DbFieldInfo info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            if (string.IsNullOrEmpty(info.FieldType))
                throw new ArgumentNullException(nameof(info.FieldType));
            info.DataType = SqlType2CsharpTypeStr(info.FieldType, info.IsNullable);
            return info.DataType;
        }

        /// <summary>
        /// 将数据库类型转为系统类型。
        /// </summary>
        /// <param name="sqlType">数据库字段类型</param>
        /// <param name="isNullable">字段是否可空</param>
        /// <returns></returns>
        public static string SqlType2CsharpTypeStr(string sqlType, bool isNullable = false)
        {
            if (string.IsNullOrEmpty(sqlType))
                throw new ArgumentNullException(nameof(sqlType));
            var val = string.Empty;
            var allowNull = false;
            switch (sqlType.ToLower())
            {
                case "bit":
                    val = "bool";
                    break;
                case "int":
                    val = "int";
                    break;
                case "smallint":
                    val = "short";
                    break;
                case "bigint":
                    val = "long";
                    break;
                case "tinyint":
                    val = "bool";
                    break;

                case "binary":
                case "image":
                case "varbinary":
                    val = "byte[]";
                    allowNull = true;
                    break;

                case "decimal":
                    val = "decimal";
                    break;
                case "numeric":
                case "money":
                case "smallmoney":
                    val = "decimal";
                    break;

                case "float":
                    val = "float";
                    break;
                case "real":
                    val = "Single";
                    break;

                case "datetime":
                    val = "DateTime";
                    break;
                case "smalldatetime":
                case "timestamp":
                    val = "DateTime";
                    break;

                case "uniqueidentifier":
                    val = "Guid";
                    break;
                case "Variant":
                    val = "object";
                    allowNull = true;
                    break;

                case "text":
                    val = "string";
                    allowNull = true;
                    break;
                case "ntext":
                    val = "string";
                    allowNull = true;
                    break;
                case "char":
                    val = "string";
                    allowNull = true;
                    break;
                case "nchar":
                    val = "string";
                    allowNull = true;
                    break;
                case "varchar":
                    val = "string";
                    allowNull = true;
                    break;
                case "nvarchar":
                    val = "string";
                    allowNull = true;
                    break;
                default:
                    val = "string";
                    allowNull = true;
                    break;
            }
            if (isNullable && !allowNull)
                return val + "?";
            return val;
        }
        #endregion
    }
}
