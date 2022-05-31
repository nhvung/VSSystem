using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace VSSystem.Net.Extensions
{
    public static class PingExtension
    {
        static async Task<PingInfo> _Ping(string hostNameOrAddress)
        {
            try
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync(hostNameOrAddress);
                return new PingInfo(pingReply);
            }
            catch { }
            return default;
        }

        public static async Task<List<PingInfo>> PingRound(this object sender, string hostNameOrAddress, int round = 1)
        {
            List<PingInfo> result = new List<PingInfo>();
            try
            {
                for (int i = 0; i < round; i++)
                {
                    var pingInfo = await _Ping(hostNameOrAddress);
                    result.Add(pingInfo);
                }
            }
            catch { }
            return result;
        }

        public static async Task<PingInfo> Ping(this object sender, string hostNameOrAddress)
        {
            PingInfo result = default;
            try
            {
                result = await _Ping(hostNameOrAddress);
            }
            catch { }
            return result;
        }
    }
}