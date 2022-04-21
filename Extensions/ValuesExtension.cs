using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Extensions
{
    public static class ValuesExtension
    {
        public static TResult UpdateValue<TResult>(string key, string value, ref List<string> successLog, ref List<string> unsuccessLog)
        {
            TResult result = default;
            try
            {
                Type sType = typeof(TResult);
                if (sType.IsEnum)
                    result = (TResult)Enum.Parse(sType, value, true);
                else
                    result = (TResult)Convert.ChangeType(value, sType);
                if (string.IsNullOrEmpty(value))
                    unsuccessLog.Add(string.Format("- {0} is empty.", key));
                else successLog.Add(string.Format("- {0}: {1}.", key, value));

            }
            catch
            {
            }
            return result;
        }
    }
}
