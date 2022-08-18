using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VSSystem.Net.Models;

namespace VSSystem.Net.Extensions
{
    public static class HttpWebClient64Extension
    {
        static HttpClient _CreateHttpClient(string url, int timeout, bool ignoreCertificate)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                if (url.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (ignoreCertificate)
                    {
                        handler.ServerCertificateCustomValidationCallback = delegate { return true; };
                    }

                }
                var client = new HttpClient(handler);
                client.Timeout = new TimeSpan(0, 0, timeout);
                return client;
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        static async Task<HttpWebResult> _ProcessDataAsync(string url, int timeout, HttpMethod method, string contentType, Stream stream
        , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            HttpWebResult result = new HttpWebResult();
            try
            {
                using (var client = _CreateHttpClient(url, timeout, ignoreCertificate))
                {
                    HttpRequestMessage rMess = new HttpRequestMessage(method, url);
                    //Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36
                    rMess.Headers.Add("User-Agent", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
                    if (additionalHeaders?.Count() > 0)
                    {
                        foreach (var header in additionalHeaders)
                        {
                            rMess.Headers.Add(header.Key, HttpUtility.HtmlEncode(header.Value));
                        }
                    }

                    if (stream != null)
                    {
                        rMess.Content = new StreamContent(stream);
                        rMess.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    }

                    var response = await client.SendAsync(rMess, cancellationToken);

                    var outputStream = await response.Content.ReadAsStreamAsync();
                    try
                    {
                        result = new HttpWebResult(outputStream);
                    }
                    catch { }
                    result.StatusCode = response.StatusCode;
                    if (response.Headers?.Count() > 0)
                    {
                        foreach (var header in response.Headers)
                        {
                            foreach (var value in header.Value)
                            {
                                result.Headers.Add(new KeyValuePair<string, string>(header.Key, value));
                            }
                            if (header.Key.Equals("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                            {
                                result.ContentType = header.Value?.FirstOrDefault();
                            }
                        }
                    }

                    client.Dispose();
                }
            }
            catch { }
            return result;
        }

        #region Base
        public static Task<HttpWebResult> GetDataAsync(this object sender, string url, int timeout, string contentType
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, HttpMethod.Get, contentType, null, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PostDataAsync(this object sender, string url, int timeout, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, HttpMethod.Post, contentType, new MemoryStream(data), ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PostStreamAsync(this object sender, string url, int timeout, string contentType, Stream stream
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, HttpMethod.Post, contentType, stream, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PutDataAsync(this object sender, string url, int timeout, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, HttpMethod.Put, contentType, new MemoryStream(data), ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> PutStreamAsync(this object sender, string url, int timeout, string contentType, Stream stream
           , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, HttpMethod.Put, contentType, stream, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> TransferDataAsync(this object sender, string url, int timeout, string method, string contentType, byte[] data
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, new HttpMethod(method), contentType, new MemoryStream(data), ignoreCertificate, additionalHeaders, cancellationToken);
        }
        public static Task<HttpWebResult> TransferStreamAsync(this object sender, string url, int timeout, string method, string contentType, Stream stream
            , bool ignoreCertificate = false, IEnumerable<KeyValuePair<string, string>> additionalHeaders = null, CancellationToken cancellationToken = default)
        {
            return _ProcessDataAsync(url, timeout, new HttpMethod(method), contentType, stream, ignoreCertificate, additionalHeaders, cancellationToken);
        }
        #endregion
    }
}