using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using SunnyMES.Commons.Core.Dtos.Models;
using SunnyMES.Commons.Enums;
using SunnyMES.Commons.Enums.DynamicItemName;
using SunnyMES.Commons.Module;

namespace SunnyMES.Commons.Extend;

public static class ExtEnum
{
    public static string DisplayText(this SnLinkUpcInputNames names,string Separator = "_") => names.ToString().Replace(Separator, " ");

    public static string GetDescriptionByEnum(this Enum enumValue)
    {
        string value = enumValue.ToString();
        System.Reflection.FieldInfo field = enumValue.GetType().GetField(value);
        object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false); //获取描述属性
        if (objs.Length == 0) //当描述属性没有时，直接返回名称
            return value;
        DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
        return descriptionAttribute.Description;
    }
    /// <summary>
    /// 字符串转枚举
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="str"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this string str)
    {
        return (T)Enum.Parse(typeof(T), str);
    }
    /// <summary>
    /// 扩展方法，获得枚举的Description
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
    /// <returns>枚举的Description</returns>
    public static EnumItem GetFuncDescription(this Enum value, Boolean nameInstead = true)
    {
        EnumItem e = new EnumItem();
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }
        FieldInfo field = type.GetField(name);
        FuncDescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(FuncDescriptionAttribute)) as FuncDescriptionAttribute;
        if (attribute == null && nameInstead == true)
        {
            e.Description = name;
            e.FuncName = name;
            return e;
        }
        else
        {
            e.Description = attribute.Description;
            e.FuncName = attribute.FuncName;
            return e;
        }
    }
    /// <summary>
    /// 扩展方法，获得枚举的Description
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <param name="nameInstead">当枚举值没有定义DescriptionAttribute，是否使用枚举名代替，默认是使用</param>
    /// <returns>枚举的Description</returns>
    public static string GetDescription(this Enum value, Boolean nameInstead = true)
    {
        Type type = value.GetType();
        string name = Enum.GetName(type, value);
        if (name == null)
        {
            return null;
        }
        FieldInfo field = type.GetField(name);
        DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
        if (attribute == null && nameInstead == true)
        {
            return name;
        }
        return attribute == null ? null : attribute.Description;
    }

    /// <summary>
    /// 把枚举转换为键值对集合
    /// 枚举转换为键值对集合
    ///  Dictionary<Int32, String> dic = EnumUtil.EnumToDictionary(typeof(Season), e => e.GetDescription());
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <param name="getText">获得值得文本</param>
    /// <returns>以枚举值为key，枚举文本为value的键值对集合</returns>
    public static Dictionary<Int32, String> EnumToDictionary(Type enumType, Func<Enum, String> getText)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
        }
        Dictionary<Int32, String> enumDic = new Dictionary<int, string>();
        Array enumValues = Enum.GetValues(enumType);
        foreach (Enum enumValue in enumValues)
        {
            Int32 key = Convert.ToInt32(enumValue);
            String value = getText(enumValue);
            enumDic.Add(key, value);
        }
        return enumDic;
    }
    /// <summary>
    /// 将枚举信息转成列表信息
    /// </summary>
    /// <param name="enumType">枚举类型</param>
    /// <param name="getText">查询委托</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static List<CustomKeyValue> EnumToList(Type enumType, Func<Enum, String> getText)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
        }
        List<CustomKeyValue> enumList = new List<CustomKeyValue>();
        Array enumValues = Enum.GetValues(enumType);
        foreach (Enum enumValue in enumValues)
        {
            Int32 key = Convert.ToInt32(enumValue);
            String value = getText(enumValue);
            enumList.Add(new CustomKeyValue { Key = key.ToString(), Value = value});
        }
        return enumList;
    }
    /// <summary>
    /// 获取枚举的相关信息
    /// 描述信息无法获取。。。
    /// </summary>
    /// <param name="e">枚举的类型</param>
    /// <returns></returns>
    public static List<EnumItem> GetEnumItems(Type e)
    {
        List<EnumItem> itemList = new List<EnumItem>();
        foreach (Enum v in Enum.GetValues(e))
        {
            EnumItem item = new EnumItem();
            // TODO: 遍历操作
            item.EnumKey = Convert.ToInt32(v);
            item.EnumValue = v.ToString();
            item.Description = GetFuncDescription(v).Description;
            item.FuncName = GetFuncDescription(v).FuncName;
            itemList.Add(item);
        }
        return itemList;
    }

    public class EnumItem
    {
        public int EnumKey { get; set; }
        public string EnumValue { get; set; }
        public string Description { get; set; }
        public string FuncName { get; set; }
    }
}