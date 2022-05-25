using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSSystem.Threading.Tasks.Extensions;

namespace VSSystem.Net.Http
{
    public class HttpDownloadClient
    {
        const int DEFAULT_BUFFER_BLOCK_SIZE = 16000000;
        int _timeout;
        bool _ignoreCertification;
        public HttpDownloadClient(int timeout = 60, bool ignoreCertification = false)
        {
            _timeout = timeout;
            _ignoreCertification = ignoreCertification;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Key: Position; Value: Length</returns>
        List<KeyValuePair<long, long>> _GetSplitSizes(long length, int numberOfThreads)
        {
            try
            {
                if (numberOfThreads <= 0)
                {
                    return new List<KeyValuePair<long, long>>()
                    {
                        new KeyValuePair<long, long>(0, length -1)
                    };
                }
                long ctLength = length;
                long jumpLength = ctLength / numberOfThreads;
                List<KeyValuePair<long, long>> result = new List<KeyValuePair<long, long>>();
                long pos = 0;
                do
                {
                    if (jumpLength < ctLength)
                    {
                        result.Add(new KeyValuePair<long, long>(pos, pos + jumpLength - 1));
                        pos += jumpLength;
                        ctLength -= jumpLength;
                    }
                    else
                    {
                        result.Add(new KeyValuePair<long, long>(pos, pos + ctLength - 1));
                        ctLength = 0;
                    }
                } while (ctLength > 0);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        async Task<long> _GetDataContentLength(Uri requestUri, ICredentials credentials = default)
        {
            long result = 0;
            try
            {
                HttpWebRequest webRequest = WebRequest.CreateHttp(requestUri);
                webRequest.Timeout = _timeout * 1000;

                if(_ignoreCertification && requestUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                {
                    webRequest.ServerCertificateValidationCallback = delegate { return true; };
                }
                if(credentials != null)
                {
                    webRequest.UseDefaultCredentials = false;
                    webRequest.Credentials = credentials;
                }
                using(var webResponse = await webRequest.GetResponseAsync())
                {
                    result = webResponse.ContentLength;
                    webResponse.Close();
                }
            }
            catch { }
            return result;
        }


        async public Task DownloadFile(string requestUrl, FileInfo outputFile, int numberOfThreads = 1, ICredentials credentials = default, CancellationToken cancellationToken = default)
        {

            try
            {
                Uri requestUri = new Uri(requestUrl);
                long dataContentLength = await _GetDataContentLength(requestUri, credentials);
                if (dataContentLength > 0)
                {
                    var splitSizes = _GetSplitSizes(dataContentLength, numberOfThreads);
                    if(!outputFile.Directory.Exists)
                    {
                        outputFile.Directory.Create();
                    }
                    using(var stream = outputFile.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        stream.SetLength(dataContentLength);
                        stream.Close();
                        stream.Dispose();
                    }

                    object lockObj = new object();
                    long lCurrentLength = 0;
                    await splitSizes.ConsecutiveRunAsync((sz) => { 
                        using(var stream = outputFile.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            stream.Seek(sz.Key, SeekOrigin.Begin);
                            try
                            {
                                HttpWebRequest webRequest = WebRequest.CreateHttp(requestUri);
                                webRequest.Timeout = _timeout * 1000;
                                webRequest.AddRange(sz.Key, sz.Value);

                                if (_ignoreCertification && requestUri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    webRequest.ServerCertificateValidationCallback = delegate { return true; };
                                }
                                if (credentials != null)
                                {
                                    webRequest.UseDefaultCredentials = false;
                                    webRequest.Credentials = credentials;
                                }
                                using (var webResponse = webRequest.GetResponse())
                                {
                                    using(var netStream = webResponse.GetResponseStream())
                                    {
                                        byte[] buffer = new byte[DEFAULT_BUFFER_BLOCK_SIZE];
                                        int ret = 0;
                                        do
                                        {
                                            ret = netStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).Result;
                                            stream.WriteAsync(buffer, 0, ret, cancellationToken).Wait();
                                            lock (lockObj)
                                            {
                                                lCurrentLength += ret;
                                            }

                                        } while (ret > 0);
                                        netStream.Close();
                                        netStream.Dispose();
                                    }

                                    webResponse.Close();
                                    webResponse.Dispose();
                                }
                            }
                            catch { }

                            stream.Close();
                            stream.Dispose();
                        }
                    }, numberOfThreads, cancellationToken);

                }
            }
            catch //(Exception ex) 
            {
            }
        }
        public Task DownloadFile(string requestUrl, string outputFilePath, int numberOfThreads = 1, ICredentials credentials = default, CancellationToken cancellationToken = default)
        {
            return DownloadFile(requestUrl, new FileInfo(outputFilePath), numberOfThreads, credentials, cancellationToken);
        }
    }
}
