using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SunnyMES.Commons.Module;

namespace SunnyMES.Commons.Extend;

public static class ExtPublicFunc
{

    /// <summary>
    /// 获取第一个属性值
    ///  用法：  ExtPublicFunc.GetDescription(() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="e"></param>
    /// <returns></returns>
    public static string GetDescription<T>(Expression<Func<T>> e)
    {
        Type type = typeof(T);
        var me = e.Body as MemberExpression;
        var attrs = me.Member.CustomAttributes.ToList();
        foreach (CustomAttributeData customAttributeData in attrs)
        {
            if (customAttributeData.AttributeType != typeof(FuncDescriptionAttribute))
                continue;

            var funcDescs= customAttributeData.ConstructorArguments.ToList();
            var val = funcDescs[0].Value;
            return val.ToString();
        }

        return "";
    }
    // <summary>
    // Get the name of a static or instance property from a property access lambda.
    // 用法：  ExtPublicFunc.GetPropertyName((() => mConfirmPoOutput.CurrentInitPageInfo.poAttributes.IsScanUPC))
    // </summary>
    // <typeparam name="T">Type of the property</typeparam>
    // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'</param>
    // <returns>The name of the property</returns>
    public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
    {
        var me = propertyLambda.Body as MemberExpression;

        if (me == null)
        {
            throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
        }

        return me.Member.Name;
    }
    /// <summary>
    /// 获取完整变量名称
    ///  用法：  ExtPublicFunc.GetPropertyNames((() => mConfirmPoOutput.CurrentInitPageInfo));
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyLambda"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetPropertyNames<T>(Expression<Func<T>> propertyLambda)
    {
        var me = propertyLambda.Body as MemberExpression;

        if (me == null)
        {
            throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
        }
        string result = string.Empty;
        do
        {
            result = me.Member.Name + "." + result;
            me = me.Expression as MemberExpression;
        } while (me != null);

        result = result.Remove(result.Length - 1); // remove the trailing"."
        return result;
    }

    /// <summary>
    /// 获取指定类的属性
    ///  用法：  ExtPublicFunc.GetName<SetConfirmPoOutput>(x => x.CurrentInitPageInfo.poAttributes.IsScanUPC);
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exp"></param>
    /// <returns></returns>
    public static string GetName<T>(Expression<Func<T, object>> exp)
    {
        MemberExpression body = exp.Body as MemberExpression;

        if (body == null)
        {
            UnaryExpression ubody = (UnaryExpression)exp.Body;
            body = ubody.Operand as MemberExpression;
        }

        return body.Member.Name;
    }
    /// <summary>
    /// 只能读取第一层的指定key的值
    /// </summary>
    /// <param name="ld"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string ConvertDynamic(this List<dynamic> ld, string key)
    {
        string result = string.Empty;
        foreach (KeyValuePair<string, object> keyvalue in ld[0])
        {
            if (keyvalue.Key == key.Trim() && !string.IsNullOrEmpty(keyvalue.Value?.ToString()))
            {
                result = keyvalue.Value.ToString();
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 将DataTable 转换成 'List<dynamic>'
    /// reverse 反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
    /// [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
    /// FilterField  字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数
    /// </summary>
    /// <param name="table">DataTable</param>
    /// <param name="reverse">
    /// 反转：控制返回结果中是只存在 FilterField 指定的字段,还是排除.
    /// [flase 返回FilterField 指定的字段]|[true 返回结果剔除 FilterField 指定的字段]
    ///</param>
    /// <param name="FilterField">字段过滤，FilterField 为空 忽略 reverse 参数；返回DataTable中的全部数据</param>
    /// <returns></returns>
    public static List<dynamic> ToDynamicList(this DataTable table, bool reverse = true, params string[] FilterField)
    {
        var modelList = new List<dynamic>();
        foreach (DataRow row in table.Rows)
        {
            dynamic model = new ExpandoObject();
            var dict = (IDictionary<string, object>)model;
            foreach (DataColumn column in table.Columns)
            {
                if (FilterField.Length != 0)
                {
                    if (reverse == true)
                    {
                        if (!FilterField.Contains(column.ColumnName))
                        {
                            dict[column.ColumnName] = row[column];
                        }
                    }
                    else
                    {
                        if (FilterField.Contains(column.ColumnName))
                        {
                            dict[column.ColumnName] = row[column];
                        }
                    }
                }
                else
                {
                    dict[column.ColumnName] = row[column];
                }
            }
            modelList.Add(model);
        }
        return modelList;
    }

    public static DataTable ConvertDataReaderToDataTable(this IDataReader dataReader)
    {
        ///定义DataTable
        DataTable datatable = new DataTable();

        ///动态添加表的数据列
        for (int i = 0; i < dataReader.FieldCount; i++)
        {
            DataColumn myDataColumn = new DataColumn();
            myDataColumn.DataType = dataReader.GetFieldType(i);
            myDataColumn.ColumnName = dataReader.GetName(i);
            datatable.Columns.Add(myDataColumn);
        }

        object[] values = new object[dataReader.FieldCount];
        ///添加表的数据
        while (dataReader.Read())
        {
            dataReader.GetValues(values);
            datatable.LoadDataRow(values, true);
        }
        ///关闭数据读取器
        dataReader.Close();
        return datatable;
    }
    
}