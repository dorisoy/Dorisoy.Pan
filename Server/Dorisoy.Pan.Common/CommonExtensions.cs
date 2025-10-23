﻿using Dorisoy.Pan.Data;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Dorisoy.Pan.Common
{
    /// <summary>
    /// 通用扩展方法
    /// </summary>
    public static class CommonExtensions
    {
        #region ==数据转换扩展==

        /// <summary>
        /// 转换成Byte
        /// </summary>
        /// <param name="s">输入字符串</param>
        /// <returns></returns>
        public static byte ToByte(this string s)
        {
            if (s.IsNull())
                return 0;

            byte.TryParse(s, out byte result);
            return result;
        }

        /// <summary>
        /// 转换成Char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static char ToChar(this string s)
        {
            if (s.IsNull())
                return default;

            char.TryParse(s, out char result);
            return result;
        }

        /// <summary>
        /// 转换成Char
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static char ToChar(this int s)
        {
            return (char)s;
        }

        /// <summary>
        /// 转换成short/Int16
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static short ToShort(this string s)
        {
            if (s.IsNull())
                return 0;

            short.TryParse(s, out short result);
            return result;
        }

        /// <summary>
        /// 转换成Int/Int32
        /// </summary>
        /// <param name="s"></param>
        /// <param name="round">是否四舍五入，默认false</param>
        /// <returns></returns>
        public static int ToInt(this object s, bool round = false)
        {
            if (s == null || s == DBNull.Value)
                return 0;

            if (s is bool b)
                return b ? 1 : 0;

            if (int.TryParse(s.ToString(), out int result))
                return result;

            if (s.GetType().IsEnum)
            {
                return (int)s;
            }

            var f = s.ToFloat();
            return round ? Convert.ToInt32(f) : (int)f;
        }

        /// <summary>
        /// 转换成Long/Int64
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static long ToLong(this object s)
        {
            if (s == null || s == DBNull.Value)
                return 0L;

            long.TryParse(s.ToString(), out long result);
            return result;
        }

        /// <summary>
        /// 转换成Float/Single
        /// </summary>
        /// <param name="s"></param>
        /// <param name="decimals">小数位数</param>
        /// <returns></returns>
        public static float ToFloat(this object s, int? decimals = null)
        {
            if (s == null || s == DBNull.Value)
                return 0f;

            float.TryParse(s.ToString(), out float result);

            if (decimals == null)
                return result;

            return (float)Math.Round(result, decimals.Value);
        }

        /// <summary>
        /// 转换成Double/Single
        /// </summary>
        /// <param name="s"></param>
        /// <param name="digits">小数位数</param>
        /// <returns></returns>
        public static double ToDouble(this object s, int? digits = null)
        {
            if (s == null || s == DBNull.Value)
                return 0d;

            double.TryParse(s.ToString(), out double result);

            if (digits == null)
                return result;

            return Math.Round(result, digits.Value);
        }

        /// <summary>
        /// 转换成Decimal
        /// </summary>
        /// <param name="s"></param>
        /// <param name="decimals">小数位数</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object s, int? decimals = null)
        {
            if (s == null || s == DBNull.Value) return 0m;

            decimal.TryParse(s.ToString(), out decimal result);

            if (decimals == null)
                return result;

            return Math.Round(result, decimals.Value);
        }

        /// <summary>
        /// 转换成DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object s)
        {
            if (s == null || s == DBNull.Value)
                return DateTime.MinValue;

            DateTime.TryParse(s.ToString(), out DateTime result);
            return result;
        }

        /// <summary>
        /// 转换成Date
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDate(this object s)
        {
            return s.ToDateTime().Date;
        }

        /// <summary>
        /// 转换成Boolean
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool ToBool(this object s)
        {
            if (s == null)
                return false;

            s = s.ToString().ToLower();
            if (s.Equals(1) || s.Equals("1") || s.Equals("true") || s.Equals("是") || s.Equals("yes"))
                return true;
            if (s.Equals(0) || s.Equals("0") || s.Equals("false") || s.Equals("否") || s.Equals("no"))
                return false;

            Boolean.TryParse(s.ToString(), out bool result);
            return result;
        }

        /// <summary>
        /// 字符串转Guid
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Guid ToGuid(this string s)
        {
            if (s.NotNull() && Guid.TryParse(s, out Guid val))
                return val;

            return Guid.Empty;
        }

        /// <summary>
        /// 泛型转换，转换失败会抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T To<T>(this object s)
        {
            return (T)Convert.ChangeType(s, typeof(T));
        }

        public static bool IsNumeric(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>

        public static T ConvertTo<T>(this IConvertible value) where T : IConvertible
        {
            if (value != null)
            {
                var type = typeof(T);
                if (value.GetType() == type)
                {
                    return (T)value;
                }

                if (type.IsNumeric())
                {
                    return (T)value.ToType(type, new NumberFormatInfo());
                }

                if (value == DBNull.Value)
                {
                    return default;
                }

                if (type.IsEnum)
                {
                    return (T)Enum.Parse(type, value.ToString(CultureInfo.InvariantCulture));
                }

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var underlyingType = Nullable.GetUnderlyingType(type);
                    return (T)(underlyingType!.IsEnum ? Enum.Parse(underlyingType, value.ToString(CultureInfo.CurrentCulture)) : Convert.ChangeType(value, underlyingType));
                }

                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (converter != null)
                {
                    if (converter.CanConvertTo(type))
                    {
                        return (T)converter.ConvertTo(value, type);
                    }
                }

                converter = TypeDescriptor.GetConverter(type);
                if (converter != null)
                {
                    if (converter.CanConvertFrom(value.GetType()))
                    {
                        return (T)converter.ConvertFrom(value);
                    }
                }
                return (T)Convert.ChangeType(value, type);
            }

            return (T)value;
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败的默认值</param>
        /// <returns></returns>
        public static T TryConvertTo<T>(this IConvertible value, T defaultValue = default) where T : IConvertible
        {
            try
            {
                return ConvertTo<T>(value);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo<T>(this IConvertible value, out T result) where T : IConvertible
        {
            try
            {
                result = ConvertTo<T>(value);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">目标类型</param>
        /// <param name="result">转换失败的默认值</param>
        /// <returns></returns>
        public static bool TryConvertTo(this IConvertible value, Type type, out object result)
        {
            try
            {
                result = ConvertTo(value, type);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// 类型直转
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type">目标类型</param>
        /// <returns></returns>
        public static object ConvertTo(this IConvertible value, Type type)
        {
            if (value == null)
            {
                return default;
            }

            if (value.GetType() == type)
            {
                return value;
            }

            if (value == DBNull.Value)
            {
                return null;
            }

            if (type.IsNumeric())
            {
                return value.ToType(type, new NumberFormatInfo());
            }

            if (type.IsEnum)
            {
                return Enum.Parse(type, value.ToString(CultureInfo.InvariantCulture));
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return underlyingType!.IsEnum ? Enum.Parse(underlyingType, value.ToString(CultureInfo.CurrentCulture)) : Convert.ChangeType(value, underlyingType);
            }

            var converter = TypeDescriptor.GetConverter(value);
            if (converter != null)
            {
                if (converter.CanConvertTo(type))
                {
                    return converter.ConvertTo(value, type);
                }
            }

            converter = TypeDescriptor.GetConverter(type);
            if (converter != null)
            {
                if (converter.CanConvertFrom(value.GetType()))
                {
                    return converter.ConvertFrom(value);
                }
            }
            return Convert.ChangeType(value, type);

        }

        #endregion

        #region ==布尔转换==

        /// <summary>
        /// 布尔值转换为字符串1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToIntString(this bool b)
        {
            return b ? "1" : "0";
        }

        /// <summary>
        /// 布尔值转换为整数1或者0
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        /// <summary>
        /// 布尔值转换为中文
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string ToZhCn(this bool b)
        {
            return b ? "是" : "否";
        }

        #endregion

        #region ==字节转换==

        /// <summary>
        /// 转为十六进制
        /// </summary>
        /// <param name="val"></param>
        /// <param name="lowerCase"></param>
        /// <returns></returns>
        public static string ToHex(this string val, bool lowerCase = true)
        {
            if (val.IsNull())
                return null;

            var bytes = Encoding.UTF8.GetBytes(val);
            return bytes.ToHex(lowerCase);
        }

        /// <summary>
        /// 转换为16进制
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="lowerCase">是否小写</param>
        /// <returns></returns>
        public static string ToHex(this byte[] bytes, bool lowerCase = true)
        {
            if (bytes == null)
                return null;

            var result = new StringBuilder();
            var format = lowerCase ? "x2" : "X2";
            for (var i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString(format));
            }

            return result.ToString();
        }

        /// <summary>
        /// 16进制转字节数组
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte[] Hex2Bytes(this string s)
        {
            if (s.IsNull())
                return null;
            var bytes = new byte[s.Length / 2];

            for (int x = 0; x < s.Length / 2; x++)
            {
                int i = (Convert.ToInt32(s.Substring(x * 2, 2), 16));
                bytes[x] = (byte)i;
            }

            return bytes;
        }

        /// <summary>
        /// 16进制转字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string Hex2String(this string val)
        {
            if (val.IsNull())
                return null;

            var bytes = val.Hex2Bytes();
            return Encoding.UTF8.GetString(bytes);
        }

        #endregion
    }
}
