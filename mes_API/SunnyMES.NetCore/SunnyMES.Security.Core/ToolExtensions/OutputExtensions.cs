using NetTaste;
using Newtonsoft.Json;
using NPOI.Util.Collections;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SunnyMES.Commons.Core.Dtos;
using SunnyMES.Commons.Extensions;
using SunnyMES.Commons.Helpers;
using SunnyMES.Security._1_Models.MES.Query;
using SunnyMES.Security._2_Dtos.MES;
using SunnyMES.Security._2_Dtos.MES.PackageOverStation;
using SunnyMES.Security._2_Dtos.MES.SNLinkUPC;

namespace SunnyMES.Security.ToolExtensions;

public static class OutputExtensions
{
    public static MesOutputDto SetErrorCode(this MesOutputDto mesOutputDto, string errorCode)
    {
        mesOutputDto.ErrorMsg = errorCode;
        return mesOutputDto;
    }
    public static SetConfirmPoOutput SetErrorCode(this SetConfirmPoOutput mesOutputDto, string errorCode)
    {
        mesOutputDto.ErrorMsg = errorCode;
        return mesOutputDto;
    }
    public static T SetErrorCode<T>(this T mesOutputDto, string errorCode) where T : MesOutputDto
    {
        mesOutputDto.ErrorMsg = errorCode;
        return mesOutputDto;
    }
    public static T SetErrorCode<T>(this T mesOutputDto, string errorCode, params object?[] ps) where T : MesOutputDto
    {
        mesOutputDto.ErrorMsg = string.Format(errorCode, ps);
        return mesOutputDto;
    }
    public static T DataTableToModel<T>(this DataTable dataTable, T t)
    {
        foreach (DataColumn item in dataTable.Columns)
        {
            if (item.DataType == typeof(int))
            {
                t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[0][item.ColumnName].ToInt());
            }
            else if(item.DataType == typeof(bool))
            {
                t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[0][item.ColumnName].ToBool());
            }
            else if (item.DataType == typeof(double) || item.DataType == typeof(float))
            {
                t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[0][item.ColumnName].ToDouble());
            }
            else
            {
                t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[0][item.ColumnName].ToString());
            }
        }
        return t;
    }
    public static List<T> DataTableToModels<T>(this DataTable dataTable, T t)
    {
        List<T> ls = new List<T>();
        for (int i = 0; i < dataTable.Rows.Count; i++)
        {
            foreach (DataColumn item in dataTable.Columns)
            {
                if (item.DataType == typeof(int))
                {
                    t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[i][item.ColumnName].ToInt());
                }
                else if (item.DataType == typeof(bool))
                {
                    t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[i][item.ColumnName].ToBool());
                }
                else if (item.DataType == typeof(double) || item.DataType == typeof(float))
                {
                    t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[i][item.ColumnName].ToDouble());
                }
                else
                {
                    t.GetType().GetProperty(item.ColumnName).SetValue(t, dataTable.Rows[i][item.ColumnName].ToString());
                }
            }
            ls.Add(t);
        }
        return ls;
    }
    /// <summary>    
    /// 生成反射过来的MethodInfo到指定类型的委托    
    /// </summary>    
    /// <typeparam name="T">EventArgs泛型类型</typeparam>    
    /// <param name="instance">当前对象</param>    
    /// <param name="method">需要转化为delegate的方法</param>    
    /// <returns></returns>    
    public static Delegate CreateDelegateFromMethodInfo<T>(Object instance, MethodInfo method) where T : EventArgs//约束泛型T只能是来自EventArgs类型的    
    {

        Delegate del = Delegate.CreateDelegate(typeof(EventHandler<T>), instance, method);
        EventHandler<T> mymethod = del as EventHandler<T>;
        return mymethod;
    }

    /// <summary>
    /// 检查是否是最后一项提交
    /// </summary>
    /// <param name="currentItem"></param>
    /// <param name="inputDtos"></param>
    /// <returns>true: 是最后一项提交</returns>
    public static bool VerifyEndResult(DynamicItemsDto currentItem, SNLinkUPCInput inputDtos)
    {
        int count = 0;
        for (int i = 0; i < inputDtos.DataList.Count; i++)
        {
            var  item = inputDtos.DataList[i];
            if (!item.IsEnable)
                continue;

            if (item.IsCurrentItem)
            {
                if (!string.IsNullOrEmpty(item.Value))
                    count += 1;
            }
            else
            {
                if (!string.IsNullOrEmpty(item.Value) && item.IsScanFinished)
                    count += 1;
            }
        }
        
        return count == inputDtos.DataList.Values.Where(x => x.IsEnable)?.Count();
    }
    /// <summary>
    /// 通过反射进行深赋值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepCopyByReflection<T>(T obj)
    {
        if (obj is string || obj.GetType().IsValueType)
            return obj;

        object retval = Activator.CreateInstance(obj.GetType());
        FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
        foreach (var field in fields)
        {
            try
            {
                if (field.GetValue(obj) is null)
                    continue;

                field.SetValue(retval, DeepCopyByReflection(field.GetValue(obj)));
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
        }

        return (T)retval;
    }
    /// <summary>
    /// 深度克隆
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static T DeepClone<T>(this T source)
    {
        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
        var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
    }


    /// <summary>
    /// 格式化检查参数,并且主键不相等
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="names"></param>
    /// <param name="PrimaryKey"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string FormartWhere<T>(this T source,  string PrimaryKey)
    {
        string sql = "";
        List<string> sqls = new List<string>();
        var tmpAllProperties = source.GetType().GetProperties();
        var allProperties = tmpAllProperties;
        string sqlParmary = string.Empty;

        // other key
        var specialProper = allProperties.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where(y => y.AttributeType.Name == "ForceCheckAttribute").Any());
        if (specialProper != null && specialProper.Any())
        {
            allProperties = specialProper.ToArray();
        }
        for (int i = 0; i < allProperties.Length; i++)
        {
            var property = allProperties[i];
            object v = property.GetValue(source);
            if (property.CustomAttributes.Any())
            {
                var otherProperty = property.CustomAttributes.Where(x =>  x.AttributeType.Name == "NotMappedAttribute");
                if (otherProperty.Any()) { continue; }
                var otherPropertyKey = property.CustomAttributes.Where(x => x.AttributeType.Name == "KeyAttribute" );
                if (otherPropertyKey.Any()) {
                    sqlParmary = $" {property.Name} <> {v.ToString()}";
                    continue; 
                }
            }

            if (property.Name == PrimaryKey)
            {
                sqlParmary = $" {PrimaryKey} <> {v.ToString()}";
                continue;
            }
            
            if (property.PropertyType.FullName.Contains("Int32") || property.PropertyType.FullName.Contains("Double"))
                sqls.Add($" {property.Name}= {(v is null ? 0 : v.ToString())} ");
            else if (property.PropertyType.FullName.Contains("Boolean"))
                sqls.Add($" {property.Name}= {(v is null ? 0 : (v.ToBool() ? "1" : "0"))} ");
            else if (property.PropertyType.FullName.Contains("String"))
                sqls.Add($" {property.Name}= {(v is null ? " NULL " : $"'{v.ToString()}'")} ");
            else { continue; }
        }
        //primary key
        if (string.IsNullOrEmpty(sqlParmary))
        {
            var keyP = tmpAllProperties.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where(y => y.AttributeType.Name == "KeyAttribute").Any()).ToList();
            if (keyP != null && keyP.Count == 1)
            {
                object keyPV = keyP[0].GetValue(source);
                sqlParmary = $" {PrimaryKey} <> {keyPV.ToString()}";
            }
            else
            {
                var tmpP = tmpAllProperties.Where(x => x.Name == PrimaryKey).ToList();
                if (tmpP != null && tmpP.Count == 1)
                {
                    object tmpPV = tmpP[0].GetValue(source);
                    sqlParmary = $" {PrimaryKey} <> {tmpPV.ToString()}";
                }
            }
        }
        sqls.Add(sqlParmary);
        sql = string.Join(" AND ", sqls);
        return sql;
    }


    /// <summary>
    /// 格式化检查参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="names"></param>
    /// <param name="PrimaryKey"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string FormartWhere<T>(this T source, List<string> names, string PrimaryKey)
    {
        string sql = "";
        if (names != null && names.Contains(PrimaryKey))
            names.Remove(PrimaryKey);
        List<string> sqls = new List<string>();
        if (names !=null && names.Any())
        {
            for (int i = 0; i < names.Count; i++)
            {
                var p = source.GetType().GetProperty(names[i]);
                if (p == null)
                    throw new Exception("属性名设置异常");
                
                if (p.CustomAttributes.Any())
                {
                    var otherProperty = p.CustomAttributes.Where(x => x.AttributeType.Name == "KeyAttribute" || x.AttributeType.Name == "NotMappedAttribute");
                    if (otherProperty.Any()) { continue; }
                }
                object v = p.GetValue(source);
                if (p.PropertyType.FullName.Contains("Int32") || p.PropertyType.FullName.Contains("Double"))
                {
                    sqls.Add($" {p.Name}= {(v is null ? 0 : v.ToString())}");
                }
                else if(p.PropertyType.FullName.Contains("Boolean"))
                {
                    sqls.Add($" {p.Name}= {(v is null ? 0 : (v.ToBool() ? "1" : "0"))}");
                }
                else if (p.PropertyType.FullName.Contains("String"))
                {
                    sqls.Add($" {p.Name}= {(v is null ? " NULL " : $"'{v.ToString()}'")}");
                }
                else { continue; }
            }
        }
        else
        {
            var allProperties = source.GetType().GetProperties();

            var specialProper = allProperties.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where(y => y.AttributeType.Name == "ForceCheckAttribute").Any());
            if (specialProper != null && specialProper.Any())
            {
                allProperties = specialProper.ToArray();
            }
            for (int i = 0; i <  allProperties.Length; i++)
            {
                var property = allProperties[i];
                if (property.Name == PrimaryKey)
                    continue;
                if (property.CustomAttributes.Any())
                {
                    var otherProperty = property.CustomAttributes.Where(x => x.AttributeType.Name == "KeyAttribute" || x.AttributeType.Name == "NotMappedAttribute");
                    if (otherProperty.Any()) { continue; }
                }
                object v = property.GetValue(source);
                if (property.PropertyType.FullName.Contains("Int32") || property.PropertyType.FullName.Contains("Double"))
                    sqls.Add($" {property.Name}= {(v is null ? 0 : v.ToString())} ");
                else if(property.PropertyType.FullName.Contains("Boolean"))
                    sqls.Add($" {property.Name}= {(v is null ? 0 : (v.ToBool() ? "1" : "0"))} ");
                else if (property.PropertyType.FullName.Contains("String"))
                    sqls.Add($" {property.Name}= {(v is null ? " NULL " : $"'{v.ToString()}'")} ");
                else { continue; }
            }
        }
        sql = string.Join(" AND ", sqls);
        return sql;
    }

    /// <summary>
    /// 获取克隆前实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="DefaultPrimaryKey"></param>
    /// <returns></returns>
    public static string GetBeforeSql<T>(this T source,  string DefaultPrimaryKey)
    {
        string sql = "";
        var allProperties = source.GetType().GetProperties();
        PropertyInfo propertyInfo;
        var keyProps = allProperties.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where( y => y.AttributeType.Name == "KeyAttribute").Any());
        if (keyProps.Any())
        {
            propertyInfo = keyProps.ToList()[0];
        }
        else
        {
            propertyInfo = allProperties.Where(x => x.Name == DefaultPrimaryKey).FirstOrDefault();
        }
        bool isInt = propertyInfo.PropertyType.FullName.Contains("Int32");
        var v = propertyInfo.GetValue(source);
        sql = $"{propertyInfo.Name} = {(isInt ? $"{v}" : $"'{v}'")}";
        return sql;
    }
    /// <summary>
    /// 删除主键值
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="DefaultPrimaryKey"></param>
    /// <returns></returns>
    public static T DeleteKeyValue<T>(this T source, string DefaultPrimaryKey)
    {
        T newT = DeepClone<T>(source);
        var allProperties = newT.GetType().GetProperties();
        PropertyInfo propertyInfo;
        var keyProps = allProperties.Where(x => x.CustomAttributes.Any() && x.CustomAttributes.Where(y => y.AttributeType.Name == "KeyAttribute").Any());
        if (keyProps.Any())
        {
            propertyInfo = keyProps.ToList()[0];
        }
        else
        {
            propertyInfo = allProperties.Where(x => x.Name == DefaultPrimaryKey).FirstOrDefault();
        }
        //bool isInt = propertyInfo.PropertyType.FullName.Contains("Int32");

        //if (isInt)
        //{
        //    propertyInfo.SetValue(newT, 0);
        //}
        //else
        //{
        //    propertyInfo.SetValue(newT, "");
        //}
        propertyInfo.SetValue(newT, null);
        return newT;
    }
}

/// <summary>
/// 通过表达式目录树进行深赋值
/// 据说比反射的方式速度快
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public static class TransExp<TIn, TOut>
{
    private static readonly Func<TIn, TOut> cache = GetFunc();
    private static Func<TIn, TOut> GetFunc()
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
        List<MemberBinding> memberBindingList = new List<MemberBinding>();

        foreach (var item in typeof(TOut).GetProperties())
        {
            if (!item.CanWrite) continue;

            if (typeof(TIn).GetProperties().Where(x => x.Name == item.Name)?.Count() > 0)
            {
                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));

                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }
        }

        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
        Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

        return lambda.Compile();
    }

    public static TOut Trans(TIn tIn)
    {
        return cache(tIn);
    }
}