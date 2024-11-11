using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SunnyMES.Commons.Core.PublicFun
{
    public static class DataTableEx
    {
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            List<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (var c in dt.Columns)
                {
                    object value = dr[c.ToString()];
                    if (value != DBNull.Value)
                    {
                        var p = t.GetType().GetProperty(c.ToString());
                        if (p != null)
                        {
                            p.SetValue(t, ConvertHelper.ChangeType(value, p.PropertyType), null);
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }


        public static T ToData<T>(this DataTable dt) where T : new()
        {
            if (dt.Rows.Count > 1)
            {
                throw new Exception("");
            }
            List<T> ts = new List<T>();
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                foreach (var c in dt.Columns)
                {
                    object value = dr[c.ToString()];
                    if (value != DBNull.Value)
                    {
                        var p = t.GetType().GetProperty(c.ToString());
                        if (p != null)
                        {
                            p.SetValue(t, ConvertHelper.ChangeType(value, p.PropertyType), null);
                        }
                    }
                }
                return t;
            }
            return default(T);
        }

        public static void FillData<T>(this DataTable dt, ref T t) where T : new()
        {
            if (dt.Rows.Count > 1)
            {
                throw new Exception("");
            }
            foreach (DataRow dr in dt.Rows)
            {

                foreach (var c in dt.Columns)
                {
                    object value = dr[c.ToString()];
                    if (value != DBNull.Value)
                    {
                        var p = t.GetType().GetProperty(c.ToString());
                        if (p != null)
                        {
                            p.SetValue(t, ConvertHelper.ChangeType(value, p.PropertyType), null);
                        }
                    }
                }
            }
        }

    }
    public static class ConvertHelper
    {
        #region = ChangeType =
        public static object ChangeType(object obj, Type conversionType)
        {
            return ChangeType(obj, conversionType, System.Threading.Thread.CurrentThread.CurrentCulture);
        }

        public static object ChangeType(object obj, Type conversionType, IFormatProvider provider)
        {

            #region Nullable
            Type nullableType = Nullable.GetUnderlyingType(conversionType);
            if (nullableType != null)
            {
                if (obj == null)
                {
                    return null;
                }
                return Convert.ChangeType(obj, nullableType, provider);
            }
            #endregion
            if (typeof(System.Enum).IsAssignableFrom(conversionType))
            {
                return Enum.Parse(conversionType, obj.ToString());
            }
            return Convert.ChangeType(obj, conversionType, provider);
        }
        #endregion
    }
}
