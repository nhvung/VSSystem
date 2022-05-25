using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace VSSystem.Security
{
    public partial class Cryptography
    {
        static byte[] HexToBytes(string hexString)
        {
            byte[] result = new byte[0];
            try
            {
                result = new byte[hexString.Length / 2];
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    string hexChar = hexString[i] + "" + hexString[i + 1];
                    result[i / 2] = Convert.ToByte(hexChar, 16);
                }
            }
            catch { }
            return result;
        }

        static HashAlgorithm _CreateHashAlg(HashAlgName algName)
        {
            try
            {
                HashAlgorithm hashAlg = null;
                if (algName == HashAlgName.SHA1)
                {
                    hashAlg = SHA1.Create();
                }
                else if (algName == HashAlgName.SHA256)
                {
                    hashAlg = SHA256.Create();
                }
                else if (algName == HashAlgName.SHA384)
                {
                    hashAlg = SHA384.Create();
                }
                else if (algName == HashAlgName.SHA512)
                {
                    hashAlg = SHA512.Create();
                }
                else if (algName == HashAlgName.MD5)
                {
                    hashAlg = MD5.Create();
                }
                if (hashAlg == null)
                {
                    throw new Exception("hashAlg cannot created. HashName: " + algName.ToString());
                }
                return hashAlg;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }
        public static string Hash(string strInput, HashAlgName algName)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(strInput))
                {
                    byte[] binOutput = HashBinary(strInput, algName);
                    result = BitConverter.ToString(binOutput).Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public static string Hash(byte[] binInput, HashAlgName algName)
        {
            string result = string.Empty;
            try
            {
                if (binInput?.Length > 0)
                {
                    byte[] binOutput = HashBinary(binInput, algName);
                    result = BitConverter.ToString(binOutput).Replace("-", "");
                }
            }
            catch { }
            return result;
        }
        public static byte[] HashBinary(string strInput, HashAlgName algName)
        {
            byte[] binInput = Encoding.UTF8.GetBytes(strInput);
            return HashBinary(binInput, algName);
        }

        public static byte[] HashBinary(byte[] binInput, HashAlgName algName)
        {
            byte[] result = new byte[0];
            try
            {
                using (HashAlgorithm hashAlg = _CreateHashAlg(algName))
                {
                    result = hashAlg.ComputeHash(binInput);
                    hashAlg.Clear();
                    hashAlg.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        public static string EncryptToBase64String(string clearTextInput, string keyGuid)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(clearTextInput))
                {
                    return clearTextInput;
                }
                byte[] binInput = Encoding.UTF8.GetBytes(clearTextInput);
                var binOutput = EncryptBinary(binInput, keyGuid);
                result = Convert.ToBase64String(binOutput);
            }
            catch
            {
            }
            return result;
        }
        public static string EncryptToBase64String(byte[] inputBytes, string keyGuid)
        {
            string result = string.Empty;
            try
            {
                var binOutput = EncryptBinary(inputBytes, keyGuid);
                result = Convert.ToBase64String(binOutput);
            }
            catch
            {
            }
            return result;
        }
        public static string EncryptToHexString(string clearTextInput, string keyGuid)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(clearTextInput))
                {
                    return clearTextInput;
                }
                byte[] binInput = Encoding.UTF8.GetBytes(clearTextInput);
                var binOutput = EncryptBinary(binInput, keyGuid);
                result = BitConverter.ToString(binOutput).Replace("-", "");
            }
            catch
            {
            }
            return result;
        }
        public static string EncryptToHexString(byte[] inputBytes, string keyGuid)
        {
            string result = string.Empty;
            try
            {
                var binOutput = EncryptBinary(inputBytes, keyGuid);
                result = BitConverter.ToString(binOutput).Replace("-", "");
            }
            catch
            {
            }
            return result;
        }
        static byte[] EncryptBinary(byte[] binInput, string keyGuid)
        {
            byte[] result = new byte[0];
            try
            {
                byte[] binKey = new Guid(keyGuid).ToByteArray(), binIV = Guid.NewGuid().ToByteArray();
                using (var eAlg = Aes.Create())
                {
                    eAlg.Key = binKey;
                    eAlg.IV = binIV;

                    using (var msEncrypt = new MemoryStream())
                    {
                        msEncrypt.Write(binIV, 0, 8);
                        using (var encryptor = eAlg.CreateEncryptor())
                        {
                            var binEncrypt = encryptor.TransformFinalBlock(binInput, 0, binInput.Length);
                            encryptor.Dispose();
                            msEncrypt.Write(binEncrypt, 0, binEncrypt.Length);
                        }
                        msEncrypt.Write(binIV, 8, 8);
                        msEncrypt.Close();
                        msEncrypt.Dispose();

                        result = msEncrypt.ToArray();
                    }
                    eAlg.Dispose();
                }
            }
            catch //(Exception ex)
            {
            }
            return result;
        }
        public static TResult DecryptFromHexString<TResult>(string hexInput, string keyGuid)
        {
            TResult result = default;
            Type rType = typeof(TResult);
            try
            {
                if (string.IsNullOrEmpty(hexInput))
                {
                    return default;
                }
                byte[] binInput = HexToBytes(hexInput);
                var binOutput = DecryptBinary(binInput, keyGuid);
                if (rType == typeof(byte[]))
                {
                    result = (TResult)Convert.ChangeType(binOutput, rType);
                }
                else if (rType == typeof(string))
                {
                    result = (TResult)Convert.ChangeType(Encoding.UTF8.GetString(binOutput), rType);
                }
            }
            catch { }
            return result;
        }
        public static TResult DecryptFromBase64String<TResult>(string base64Input, string keyGuid)
        {
            TResult result = default;
            Type rType = typeof(TResult);
            try
            {
                if (string.IsNullOrEmpty(base64Input))
                {
                    return default;
                }
                byte[] binInput = Convert.FromBase64String(base64Input);
                var binOutput = DecryptBinary(binInput, keyGuid);
                if (rType == typeof(byte[]))
                {
                    result = (TResult)Convert.ChangeType(binOutput, rType);
                }
                else if (rType == typeof(string))
                {
                    result = (TResult)Convert.ChangeType(Encoding.UTF8.GetString(binOutput), rType);
                }
            }
            catch { }
            return result;
        }
        static byte[] DecryptBinary(byte[] binInput, string keyGuid)
        {
            byte[] result = new byte[0];
            try
            {
                byte[] binKey = new Guid(keyGuid).ToByteArray(), binIV = new byte[16], binEncrypt = new byte[binInput.Length - 16];
                using (var msEncrypt = new MemoryStream(binInput))
                {
                    msEncrypt.Read(binIV, 0, 8);
                    msEncrypt.Seek(-8, SeekOrigin.End);
                    msEncrypt.Read(binIV, 8, 8);

                    msEncrypt.Seek(8, SeekOrigin.Begin);

                    msEncrypt.Read(binEncrypt, 0, binEncrypt.Length);

                    msEncrypt.Close();
                    msEncrypt.Dispose();
                }

                using (var eAlg = Aes.Create())
                {
                    eAlg.Key = binKey;
                    eAlg.IV = binIV;

                    using (var descryptor = eAlg.CreateDecryptor())
                    {
                        result = descryptor.TransformFinalBlock(binEncrypt, 0, binEncrypt.Length);
                        descryptor.Dispose();
                    }
                    eAlg.Dispose();
                }
            }
            catch //(Exception ex)
            {
            }
            return result;
        }
    }
}
