using System;
using System.Collections.Generic;
using System.Threading;

namespace VSSystem
{
    public class IDGenerator
    {
        const int _MAPPING_LIMIT_COUNT = 10000;
        static Dictionary<long, bool> _mapping_int64_id;
        static Dictionary<int, bool> _mapping_int32_id;
        static DateTime _DT2010;
        /// <summary>
        /// Base on master Van
        /// </summary>
        /// <returns></returns>
        public static long GenerateInt64ID()
        {
            try
            {
                if (_DT2010 == DateTime.MinValue)
                {
                    _DT2010 = new DateTime(2010, 1, 1);
                }
                if (_mapping_int64_id == null)
                {
                    _mapping_int64_id = new Dictionary<long, bool>();
                }
                if (_mapping_int64_id.Count == _MAPPING_LIMIT_COUNT)
                {
                    _mapping_int64_id.Clear();
                }

                int randomNumber = new Random().Next(0, 99999);
                long result = (DateTime.UtcNow - _DT2010).Ticks * 10 + randomNumber;
                while (_mapping_int64_id.ContainsKey(result))
                {
                    Thread.Sleep(10);
                    result = (DateTime.UtcNow - _DT2010).Ticks * 10 + randomNumber;
                }
                _mapping_int64_id[result] = false;
                return result;
            }
            catch { }
            return 0;
        }
        public static long GenerateInt64IDFromDateTime(DateTime dt)
        {
            try
            {
                if (_DT2010 == DateTime.MinValue)
                {
                    _DT2010 = new DateTime(2010, 1, 1);
                }
                if (_mapping_int64_id == null)
                {
                    _mapping_int64_id = new Dictionary<long, bool>();
                }
                if (_mapping_int64_id.Count == _MAPPING_LIMIT_COUNT)
                {
                    _mapping_int64_id.Clear();
                }

                int randomNumber = new Random().Next(0, 99999);
                long result = (dt - _DT2010).Ticks * 10 + randomNumber;
                while (_mapping_int64_id.ContainsKey(result))
                {
                    dt = dt.AddMilliseconds(1);
                    result = (dt - _DT2010).Ticks * 10 + randomNumber;
                }
                _mapping_int64_id[result] = false;
                return result;
            }
            catch { }
            return 0;
        }
        public static long GenerateInt64IDFromTicks(long ticks)
        {
            try
            {
                DateTime dt = new DateTime(ticks);
                return GenerateInt64IDFromDateTime(dt);
            }
            catch { }
            return 0;
        }
        public static int GenerateInt32ID()
        {

            try
            {
                if (_DT2010 == DateTime.MinValue)
                {
                    _DT2010 = new DateTime(2010, 1, 1);
                }
                if (_mapping_int32_id == null)
                {
                    _mapping_int32_id = new Dictionary<int, bool>();
                }
                if (_mapping_int32_id.Count == _MAPPING_LIMIT_COUNT)
                {
                    _mapping_int32_id.Clear();
                }

                int iTimeStamp = Convert.ToInt32(Math.Floor((DateTime.UtcNow - _DT2010).TotalMinutes));

                int result = iTimeStamp * 100 + new Random().Next(0, 99);

                while (_mapping_int32_id.ContainsKey(result))
                {
                    result = iTimeStamp * 100 + new Random().Next(0, 99);
                    Thread.Sleep(100);
                }
                _mapping_int32_id[result] = false;
                return result;
            }
            catch { }
            return 0;
        }

        public static byte[] ConvertTo16Bytes(long a, long b)
        {
            try
            {
                List<byte> result = new List<byte>();
                result.AddRange(BitConverter.GetBytes(a));
                result.AddRange(BitConverter.GetBytes(b));
                return result.ToArray();
            }
            catch
            {
                return null;
            }
        }
        public static string ConvertToGuid(long a, long b)
        {
            byte[] combineBytes = ConvertTo16Bytes(a, b);
            if (combineBytes?.Length == 16)
            {
                var guid = new Guid(combineBytes);
                return guid.ToString().ToLower();
            }
            return null;
        }
    }
}
