using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Net.Models;

namespace VSSystem.Net.Extensions
{
    public static class HttpWebClientExtension
    {
        const int BUFFER_SIZE = 16384;

        #region Private Methods
        static HttpWebRequest _CreateRequest(Uri uri, int timeout, string method, string contentType, bool ignoreCertificate)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpWebRequest request = WebRequest.CreateHttp(uri);
                if (uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase) && ignoreCertificate)
                {
                    request.ServerCertificateValidationCallback = delegate { return true; };
                }
                request.Timeout = timeout * 1000;
                request.Method = method;
                request.ContentType = contentType;
                return request;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static HttpWebRequest _CreateRequest(string url, int timeout, string method, string contentType, bool ignoreCertificate)
        {
            try
            {
                return _CreateRequest(new Uri(url), timeout, method, contentType, ignoreCertificate);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static async Task _SendDataAsync(HttpWebRequest request, Stream inputStream, CancellationToken cancellationToken = default)
        {
            try
            {
                if (inputStream.Length > 0)
                {
                    request.ContentLength = inputStream.Length;
                    using (var destStream = request.GetRequestStream())
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int ret = -1;
                        do
                        {
                            ret = await inputStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                            if (ret > 0)
                            {
                                try
                                {
                                    await destStream.WriteAsync(buffer, 0, ret, cancellationToken);
                                }
                                catch { }
                            }
                        } while (ret > 0);

                        destStream.Close();
                        destStream.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async static Task _SendDataAsync(HttpWebRequest request, byte[] inputBytes, CancellationToken cancellationToken = default)
        {
            try
            {
                if (inputBytes?.Length > 0)
                {
                    await _SendDataAsync(request, new MemoryStream(inputBytes), cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static async Task<HttpWebResult> _ReceiveDataAsync(HttpWebRequest request, CancellationToken cancellationToken = default)
        {

            HttpWebResult result = new HttpWebResult();
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    result.StatusCode = response.StatusCode;
                    result.ContentType = response.ContentType;
                    if (response.ContentLength > 0)
                    {
                        using (var srcStream = response.GetResponseStream())
                        {
                            int ret = -1;
                            byte[] buffer = new byte[BUFFER_SIZE];
                            do
                            {
                                ret = await srcStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                if (ret > 0)
                                {
                                    await result.OutputStream.WriteAsync(buffer, 0, ret, cancellationToken);
                                }
                            } while (ret > 0);

                            srcStream.Close();
                            srcStream.Dispose();
                        }
                    }

                    response.Close();
                    response.Dispose();
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var response = (HttpWebResponse)ex.Response)
                    {
                        result.StatusCode = response.StatusCode;
                        result.ContentType = response.ContentType;
                        if (response.ContentLength > 0)
                        {
                            using (var srcStream = response.GetResponseStream())
                            {
                                int ret = -1;
                                byte[] buffer = new byte[BUFFER_SIZE];
                                do
                                {
                                    ret = await srcStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                                    if (ret > 0)
                                    {
                                        await result.OutputStream.WriteAsync(buffer, 0, ret, cancellationToken);
                                    }
                                } while (ret > 0);

                                srcStream.Close();
                                srcStream.Dispose();
                            }
                        }

                        response.Close();
                        response.Dispose();
                    }
                }
                else
                {
                    result.StatusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        static async Task<HttpWebResult> _ProcessDataAsync(string url, int timeout, string method, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            HttpWebResult result = new HttpWebResult();
            try
            {
                var request = _CreateRequest(url, timeout, method, contentType, ignoreCertificate);
                if (additionalHeaders?.Count() > 0)
                {
                    foreach (var header in additionalHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }
                await _SendDataAsync(request, data, cancellationToken);
                result = await _ReceiveDataAsync(request, cancellationToken);
            }
            catch { }
            return result;
        }
        #endregion

        #region Public Methods

        #region Base
        public static void AddHeaders(HttpWebRequest request, params KeyValuePair<string, string>[] headers)
        {
            if (headers?.Length > 0)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
        }
        public static Task<HttpWebResult> GetDataAsync(this object sender, string url, int timeout, string contentType
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, "GET", contentType, null, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PostDataAsync(this object sender, string url, int timeout, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, "POST", contentType, data, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PutDataAsync(this object sender, string url, int timeout, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, "PUT", contentType, data, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        #endregion

        #region Extend

        public static Task<HttpWebResult> PostJsonAsync(this object sender, string url, int timeout, object requestObj
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            string jsonObj = JsonConvert.SerializeObject(requestObj);
            byte[] data = Encoding.UTF8.GetBytes(jsonObj);
            return PostDataAsync(sender, url, timeout, ContentType.Json, data, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PutJsonAsync(this object sender, string url, int timeout, object requestObj
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            string jsonObj = JsonConvert.SerializeObject(requestObj);
            byte[] data = Encoding.UTF8.GetBytes(jsonObj);
            return PutDataAsync(sender, url, timeout, ContentType.Json, data, ignoreCertificate, additionalHeaders, cancellationToken);
        }

        #endregion

        public static async Task<PingReply> PingAsync(this object sender, string hostNameOrAddress)
        {
            try
            {
                var ping = new Ping();
                var pingReply = await ping.SendPingAsync(hostNameOrAddress);
                return pingReply;
            }
            catch { }
            return default;
        }

        #endregion


    }
}
