using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace VSSystem.Extensions
{
    public static class DateTimeExtension
    {
        public static int ToInt32(this DateTime dt)
        {
            return int.Parse(dt.ToString("yyyyMMdd"));
        }
        public static long ToInt64(this DateTime dt)
        {
            return long.Parse(dt.ToString("yyyyMMddHHmmss"));
        }
        public static DateTime ToDate(this int value)
        {
            DateTime result = default;
            try
            {
                DateTime.TryParseExact(value.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            catch { }
            return result;
        }
        public static DateTime ToDateTime(this long value)
        {
            DateTime result = default;
            try
            {
                DateTime.TryParseExact(value.ToString(), "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            catch { }
            return result;
        }
        public static DateTime ToDateTime(this string value, string format)
        {
            DateTime result = default;
            try
            {
                DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            catch { }
            return result;
        }
        public static DateTime ToDate(this string value, string format)
        {
            DateTime result = default;
            try
            {
                DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
            }
            catch { }
            return result;
        }
        static Dictionary<TimeSpan, TimeZoneInfo> _MappingTimeZone;
        //public static Dictionary<TimeSpan, TimeZoneInfo> MappingTimeZone { get { return _MappingTimeZone; } set { _MappingTimeZone = value; } }

        public static TimeZoneInfo FindTimeZoneInfo(TimeSpan ts)
        {
            if(_MappingTimeZone == null)
            {
                _LoadTimeZone();
            }
            return _MappingTimeZone?.ContainsKey(ts) ?? false ? _MappingTimeZone[ts] : null;
        }
        public static TimeZoneInfo FindTimeZoneInfo(string sTimeZone, out TimeSpan ts)
        {
            ts = new TimeSpan();
            try
            {
                int iHour, iMinute;
                string[] tTz = sTimeZone.ToUpper()
                    .Replace("UTC", "").Replace("GMT", "")
                    .Split(':');
                if (!int.TryParse(tTz[0], out iHour)) iHour = 0;
                if (!int.TryParse(tTz[1], out iMinute)) iMinute = 0;
                ts = new TimeSpan(iHour, iMinute, 0);
                return FindTimeZoneInfo(ts);
            }
            catch { }
            return null;
        }
        static void _LoadTimeZone()
        {
            try
            {
                _MappingTimeZone = new Dictionary<TimeSpan, TimeZoneInfo>();
                var timeZones = TimeZoneInfo.GetSystemTimeZones();

                var grpTimeZone = timeZones.GroupBy(ite => ite.BaseUtcOffset).ToArray();

                foreach (var grpTz in grpTimeZone)
                {
                    foreach (var timeZone in grpTz)
                    {
                        if (timeZone.DisplayName.IndexOf("US") >= 0)
                        {
                            _MappingTimeZone[timeZone.BaseUtcOffset] = timeZone;
                            break;
                        }
                        else
                        {
                            _MappingTimeZone[timeZone.BaseUtcOffset] = timeZone;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime ToLocalTime(this DateTime utcTime, string sLocalTimeZone)
        {
            try
            {
                TimeSpan ts;
                string sUtcTime = utcTime.ToString("MM/dd/yyyy h:mm:ss tt");
                string sNewTimeZone = sLocalTimeZone.ToUpper().Replace("UTC", "").Replace("GMT", "");
                if (string.IsNullOrEmpty(sNewTimeZone)) sNewTimeZone = "+00:00";
                string[] dtFormats = new string[] { "MM/dd/yyyy h:mm:ss tt zzz", "MM/dd/yyyy zzz", "M/d/yyyy h:mm:ss tt zzz", "M/d/yyyy zzz" };
                var dtOffset = DateTimeOffset.ParseExact(sUtcTime + " " + sNewTimeZone, dtFormats, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
                TimeZoneInfo tzInfo = FindTimeZoneInfo(sNewTimeZone, out ts);
                var utcOffset = tzInfo.GetUtcOffset(dtOffset);
                DateTime result = dtOffset.AddMinutes(utcOffset.TotalMinutes).DateTime;
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ToDateTimeString(this DateTime dt, string timeZone)
        {
            try
            {
                if (dt > DateTime.MinValue)
                {
                    var mappingUSTimeZoneName = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                        {
                            { "GMT-05:00", "EST" },
                            { "GMT-06:00", "CST" },
                            { "GMT-07:00", "MST" },
                            { "GMT-08:00", "PST" },
                            { "GMT-09:00", "AST" },
                            { "GMT-10:00", "HST" },
                            { "UTC-05:00", "EST" },
                            { "UTC-06:00", "CST" },
                            { "UTC-07:00", "MST" },
                            { "UTC-08:00", "PST" },
                            { "UTC-09:00", "AST" },
                            { "UTC-10:00", "HST" },
                        };
                    string tTzStr = timeZone.Trim();
                    if (mappingUSTimeZoneName.ContainsKey(tTzStr))
                    {
                        tTzStr = mappingUSTimeZoneName[tTzStr];
                    }

                    string result = string.Format("{0:MM/dd/yyyy HH:mm:ss} ({1})", dt, tTzStr);
                    return result;
                }


            }
            catch { }
            return string.Empty;
        }

        public static string ToDateString(this DateTime dt, string timeZone)
        {
            try
            {
                if (dt > DateTime.MinValue)
                {
                    var mappingUSTimeZoneName = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
                        {
                            { "GMT-05:00", "EST" },
                            { "GMT-06:00", "CST" },
                            { "GMT-07:00", "MST" },
                            { "GMT-08:00", "PST" },
                            { "GMT-09:00", "AST" },
                            { "GMT-10:00", "HST" },
                            { "UTC-05:00", "EST" },
                            { "UTC-06:00", "CST" },
                            { "UTC-07:00", "MST" },
                            { "UTC-08:00", "PST" },
                            { "UTC-09:00", "AST" },
                            { "UTC-10:00", "HST" },
                        };
                    string tTzStr = timeZone.Trim();
                    if (mappingUSTimeZoneName.ContainsKey(tTzStr))
                    {
                        tTzStr = mappingUSTimeZoneName[tTzStr];
                    }

                    string result = string.Format("{0:MM/dd/yyyy} ({1})", dt, tTzStr);
                    return result;
                }
            }
            catch { }
            return string.Empty;
        }
    }
}
