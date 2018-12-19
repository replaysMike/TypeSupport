using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TypeSupport.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Convert an Enum to UInt16
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static uint AsUInt(this Enum enumValue) => Convert.ToUInt32(enumValue);

        /// <summary>
        /// Convert an Enum to Int32
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static int AsInt(this Enum enumValue) => Convert.ToInt32(enumValue);

        /// <summary>
        /// Convert an Enum to UInt16
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static ushort AsUShort(this Enum enumValue) => Convert.ToUInt16(enumValue);

        /// <summary>
        /// Convert an Enum to Int32
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static short AsShort(this Enum enumValue) => Convert.ToInt16(enumValue);

        /// <summary>
        /// Convert an Enum to Int64
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static long AsLong(this Enum enumValue) => Convert.ToInt64(enumValue);

        /// <summary>
        /// Convert an Enum to UInt64
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static ulong AsULong(this Enum enumValue) => Convert.ToUInt64(enumValue);

        /// <summary>
        /// Convert an Enum to Byte
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static byte AsByte(this Enum enumValue) => Convert.ToByte(enumValue);

        /// <summary>
        /// Convert an Enum to SByte
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static sbyte AsSByte(this Enum enumValue) => Convert.ToSByte(enumValue);

        public static bool BitwiseHasFlag<T>(this T flag, T value)
            where T : struct
        {
            return (Convert.ToInt64(flag) & Convert.ToInt64(value)) == Convert.ToInt64(value);
        }

        public static bool BitwiseHasFlag(this Enum flag, Enum value)
        {
            return ((flag.AsLong() & value.AsLong()) == value.AsLong());
        }

        public static bool BitwiseHasFlag(this Enum flag, byte value)
        {
            return ((flag.AsByte() & value) == value);
        }

        public static bool BitwiseHasFlag(this Enum flag, short value)
        {
            return ((flag.AsShort() & value) == value);
        }

        public static bool BitwiseHasFlag(this Enum flag, int value)
        {
            return ((flag.AsInt() & value) == value);
        }

        public static bool BitwiseHasFlag(this Enum flag, long value)
        {
            return ((flag.AsLong() & value) == value);
        }

        /// <summary>
        /// Convert a Bitmask/Enum flags to a List of Enum values
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<Enum> ToListOfEnum(this Enum flags)
        {
            if (!typeof(Enum).GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                throw new InvalidOperationException("Enum must have flags attribute.");
            var flag = 1ul;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                var bits = Convert.ToUInt64(value);
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits && flags.HasFlag(value))
                {
                    yield return value;
                }
            }
        }

        /// <summary>
        /// Convert a Bitmask/Enum flags to a List of Enum values
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToListOfEnum<T>(this T flags) 
            where T : struct, IConvertible
        {
            var t = typeof(T);
            if (t.IsEnum)
            {
                if (!t.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                    throw new InvalidOperationException("Enum must have flags attribute.");
                var flagsEnum = Enum.Parse(t, flags.ToString()) as Enum;
                var flag = 1ul;
                foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
                {
                    var bits = Convert.ToUInt64(value);
                    while (flag < bits)
                    {
                        flag <<= 1;
                    }
                    if (flag == bits && flagsEnum.HasFlag(value as Enum))
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Convert a Bitmask/Enum flags to a List of Enum values
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static IEnumerable<T> ToListOfEnum<T>(this Enum flags) 
            where T : struct, IConvertible
        {
            var t = typeof(T);
            if (t.IsEnum)
            {
                if (!t.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                    throw new InvalidOperationException("Enum must have flags attribute.");
                var flagsEnum = Enum.Parse(t, flags.ToString()) as Enum;
                var flag = 1ul;
                foreach (var value in Enum.GetValues(flags.GetType()).Cast<T>())
                {
                    var bits = Convert.ToUInt64(value);
                    while (flag < bits)
                    {
                        flag <<= 1;
                    }
                    if (flag == bits && flagsEnum.HasFlag(value as Enum))
                    {
                        yield return value;
                    }
                }
            }
        }

        /// <summary>
        /// Convert a enum to a list of key value pairs (int id and string value)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<T, string>> ToListOfKeyValuePairs<T>(this Type enumType)
            where T : struct, IConvertible
        {
            var result = new List<KeyValuePair<T, string>>();
            foreach (var name in Enum.GetNames(enumType))
            {
                result.Add(new KeyValuePair<T, string>((T)Enum.Parse(enumType, name), name));
            }
            return result;
        }

        /// <summary>
        /// Convert a enum to a list of key value pairs (int id and string value)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<object, string>> ToListOfKeyValuePairs(this Type enumType, Type enumValueType)
        {
            var result = new List<KeyValuePair<object, string>>();
            foreach (var name in Enum.GetNames(enumType))
            {
                result.Add(new KeyValuePair<object, string>(Convert.ChangeType(Enum.Parse(enumType, name), enumValueType), name));
            }
            return result;
        }

        /// <summary>
        /// Get the component model description of the value (if provided)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Check an enum bitmask against a flags enum for valid values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="flags">The enumeration</param>
        /// <param name="bitmask">A flags bitmask</param>
        /// <returns></returns>
        public static bool ContainsValidEnumFlags<T>(this T flags, int bitmask) where T : struct, IConvertible
        {
            var enumList = flags.ToListOfEnum<T>();
            var maxValue = enumList.Sum(x => Convert.ToInt32(x));

            // if 0 is a valid value, then let it pass.
            if (bitmask == 0 && maxValue == 0)
                return true;
            // if 0 is a valid value and its defined, let it pass.
            else if (bitmask == 0 && maxValue > 0 && enumList.Where(x => Convert.ToInt32(x) == 0).Count() == 0)
            {
                return false;
            }

            // if value is larger than max enum size, it's not defined.
            if (bitmask > maxValue)
                return false;

            return true;
        }
    }
}
