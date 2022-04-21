using System;
using System.Text;

namespace VSSystem.Security
{
    public class Token
    {
        public const string TOKEN_EXPIRED = "TOKEN EXPIRED";
        public const string TOKEN_INVALID = "TOKEN INVALID";

        #region Generate
        public static string GenerateBase64(string input, double timeout, string keyGuid)
        {
            try
            {
                byte[] bInput = Encoding.UTF8.GetBytes(input);
                return GenerateBase64(bInput, timeout, keyGuid);
            }
            catch { }
            return string.Empty;
        }
        public static string GenerateBase64(byte[] input, double timeout, string keyGuid)
        {
            try
            {
                long lTimeout = -1;
                if (timeout > 0)
                {
                    DateTime dt = DateTime.UtcNow;
                    dt = dt.AddSeconds(timeout);
                    lTimeout = dt.Ticks;
                }
                byte[] timeoutBuff = BitConverter.GetBytes(lTimeout);
                byte[] buff = new byte[input.Length + timeoutBuff.Length];
                Array.Copy(input, 0, buff, 0, input.Length);
                Array.Copy(timeoutBuff, 0, buff, input.Length, timeoutBuff.Length);
                return Cryptography.EncryptToBase64String(buff, keyGuid);
            }
            catch { }
            return string.Empty;
        }
        public static string GenerateHex(string input, double timeout, string keyGuid)
        {
            try
            {
                byte[] bInput = Encoding.UTF8.GetBytes(input);
                return GenerateHex(bInput, timeout, keyGuid);
            }
            catch { }
            return string.Empty;
        }
        public static string GenerateHex(byte[] input, double timeout, string keyGuid)
        {
            try
            {
                long lTimeout = -1;
                if (timeout > 0)
                {
                    DateTime dt = DateTime.UtcNow;
                    dt = dt.AddSeconds(timeout);
                    lTimeout = dt.Ticks;
                }
                byte[] timeoutBuff = BitConverter.GetBytes(lTimeout);
                byte[] buff = new byte[input.Length + timeoutBuff.Length];
                Array.Copy(input, 0, buff, 0, input.Length);
                Array.Copy(timeoutBuff, 0, buff, input.Length, timeoutBuff.Length);
                return Cryptography.EncryptToHexString(buff, keyGuid);
            }
            catch { }
            return string.Empty;
        }
        //public static TokenValidationParameters GetJWTValidationParameters(string validIssuer = null, string validAudience = null, double defaultTimeout = -1)
        //{
        //    byte[] keyBytes = new Guid("304C3357-3376-7645-2164-336E63332139").ToByteArray();
        //    var keyObj = new SymmetricSecurityKey(keyBytes);
        //    TokenValidationParameters result = new TokenValidationParameters();
        //    result.ValidateIssuer = !string.IsNullOrEmpty(validIssuer);
        //    result.ValidateAudience = !string.IsNullOrWhiteSpace(validAudience);
        //    result.ValidateIssuerSigningKey = true;
        //    result.ValidateLifetime = true;
        //    result.ValidateLifetime = defaultTimeout > 0;
        //    result.IssuerSigningKey = keyObj;
        //    result.ValidAudience = validAudience;
        //    result.ValidIssuer = validIssuer;
        //    return result;
        //}
        //public static string GenerateJWT(string input, double timeout, string validIssuer = null, string validAudience = null)
        //{
        //    try
        //    {
        //        byte[] keyBytes = new Guid("304C3357-3376-7645-2164-336E63332139").ToByteArray();
        //        var keyObj = new SymmetricSecurityKey(keyBytes);
        //        var credentialsObj = new SigningCredentials(keyObj, SecurityAlgorithms.HmacSha256);

        //        TokenValidationParameters validationParameters = GetJWTValidationParameters(validIssuer, validAudience);

        //        DateTime dt = DateTime.UtcNow;
        //        if (timeout > 0)
        //        {
        //            dt = dt.AddSeconds(timeout);
        //        }
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var claims = new List<Claim>()
        //        {
        //            new Claim("custom-input", input)
        //        };
        //        var jwtToken = new JwtSecurityToken(validIssuer, validAudience, claims, null, dt, credentialsObj);
        //        string result = tokenHandler.WriteToken(jwtToken);
        //        return result;
        //    }
        //    catch(Exception ex)
        //    {
        //    }
        //    return null;
        //}
        #endregion

        #region Verify
        public static TResult VerifyBase64<TResult>(string input, string keyGuid)
        {
            Type rType = typeof(TResult);
            object resObj = default;
            if (rType == typeof(byte[]))
            {
                resObj = VerifyBase64ToBytes(input, keyGuid);
            }
            else if (rType == typeof(string))
            {
                resObj = VerifyBase64ToString(input, keyGuid);
            }
            TResult result = (TResult)resObj;
            return result;
        }
        static string VerifyBase64ToString(string input, string keyGuid)
        {
            byte[] bContent = VerifyBase64ToBytes(input, keyGuid);
            return Encoding.UTF8.GetString(bContent);
        }
        static byte[] VerifyBase64ToBytes(string input, string keyGuid)
        {
            string tokenMessage = string.Empty;
            byte[] result = null;
            try
            {
                long lTick = 0;
                var tInput = Cryptography.DecryptFromBase64String<byte[]>(input, keyGuid);
                using (var ms = new System.IO.MemoryStream(tInput))
                {
                    result = new byte[ms.Length - 8];
                    byte[] bTimeout = new byte[8];
                    ms.Read(result, 0, result.Length);
                    ms.Read(bTimeout, 0, bTimeout.Length);
                    lTick = BitConverter.ToInt64(bTimeout, 0);
                    ms.Close();
                    ms.Dispose();
                }
                if (lTick != -1 && lTick < DateTime.UtcNow.Ticks)
                {
                    tokenMessage = TOKEN_EXPIRED;
                }
            }
            catch
            {
                tokenMessage = TOKEN_INVALID;
            }
            if (!string.IsNullOrEmpty(tokenMessage))
            {
                throw new Exception(tokenMessage);
            }
            return result;
        }
        public static TResult VerifyHex<TResult>(string input, string keyGuid)
        {
            Type rType = typeof(TResult);
            object resObj = default;
            if (rType == typeof(byte[]))
            {
                resObj = VerifyHexToBytes(input, keyGuid);
            }
            else if (rType == typeof(string))
            {
                resObj = VerifyHexToString(input, keyGuid);
            }
            TResult result = (TResult)resObj;
            return result;
        }
        public static TResult VerifyHexWithExpireTime<TResult>(string input, string keyGuid, out double totalRemainSeconds)
        {
            Type rType = typeof(TResult);
            object resObj = default;
            totalRemainSeconds = 0;
            if (rType == typeof(byte[]))
            {
                resObj = VerifyHexToBytesWithExpireTime(input, keyGuid, out totalRemainSeconds);
            }
            else if (rType == typeof(string))
            {
                resObj = VerifyHexToStringWithExpireTime(input, keyGuid, out totalRemainSeconds);
            }
            TResult result = (TResult)resObj;
            return result;
        }
        static string VerifyHexToString(string input, string keyGuid)
        {
            byte[] bContent = VerifyHexToBytes(input, keyGuid);
            return Encoding.UTF8.GetString(bContent);
        }
        static string VerifyHexToStringWithExpireTime(string input,string keyGuid, out double totalRemainSeconds)
        {
            byte[] bContent = VerifyHexToBytesWithExpireTime(input, keyGuid, out totalRemainSeconds);
            return Encoding.UTF8.GetString(bContent);
        }
        static byte[] VerifyHexToBytes(string input, string keyGuid)
        {
            string tokenMessage = string.Empty;
            byte[] result = null;
            try
            {
                long lTick = 0;
                var tInput = Cryptography.DecryptFromHexString<byte[]>(input, keyGuid);
                using (var ms = new System.IO.MemoryStream(tInput))
                {
                    result = new byte[ms.Length - 8];
                    byte[] bTimeout = new byte[8];
                    ms.Read(result, 0, result.Length);
                    ms.Read(bTimeout, 0, bTimeout.Length);
                    lTick = BitConverter.ToInt64(bTimeout, 0);
                    ms.Close();
                    ms.Dispose();
                }
                if (lTick != -1 && lTick < DateTime.UtcNow.Ticks)
                {
                    tokenMessage = TOKEN_EXPIRED;
                }
            }
            catch
            {
                tokenMessage = TOKEN_INVALID;
            }
            if (!string.IsNullOrEmpty(tokenMessage))
            {
                throw new Exception(tokenMessage);
            }
            return result;
        }
        static byte[] VerifyHexToBytesWithExpireTime(string input, string keyGuid, out double totalRemainSeconds)
        {
            string tokenMessage = string.Empty;
            byte[] result = null;
            totalRemainSeconds = -1;
            try
            {
                long lTick = 0;
                var tInput = Cryptography.DecryptFromHexString<byte[]>(input, keyGuid);
                using (var ms = new System.IO.MemoryStream(tInput))
                {
                    result = new byte[ms.Length - 8];
                    byte[] bTimeout = new byte[8];
                    ms.Read(result, 0, result.Length);
                    ms.Read(bTimeout, 0, bTimeout.Length);
                    lTick = BitConverter.ToInt64(bTimeout, 0);
                    ms.Close();
                    ms.Dispose();
                }
                DateTime utcNow = DateTime.UtcNow;
                if (lTick != -1 && lTick < utcNow.Ticks)
                {
                    tokenMessage = TOKEN_EXPIRED;
                }
                if(lTick > 0)
                {
                    DateTime tokenTime = new DateTime(lTick);
                    totalRemainSeconds = (tokenTime - utcNow).TotalSeconds;
                }
            }
            catch
            {
                tokenMessage = TOKEN_INVALID;
            }
            if (!string.IsNullOrEmpty(tokenMessage))
            {
                throw new Exception(tokenMessage);
            }
            return result;
        }

        //public static TResult VerifyJWT<TResult>(string input)
        //{

        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var jwtToken = tokenHandler.ReadJwtToken(input);
        //    }
        //    catch (Exception ex)
        //    {
                
        //    }
        //    TResult result = default;
        //    return result;
        //}
        #endregion
    }
}
