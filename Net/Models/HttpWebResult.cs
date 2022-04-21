using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Net.Models
{
    public class HttpWebResult : IDisposable
    {

        string _ContentType;
        public string ContentType { get { return _ContentType; } set { _ContentType = value; } }

        List<KeyValuePair<string, string>> _Headers;
        public List<KeyValuePair<string, string>> Headers { get { return _Headers; } set { _Headers = value; } }

        Stream _OutputStream;
        public Stream OutputStream { get { return _OutputStream; } }

        HttpStatusCode _StatusCode;
        public HttpStatusCode StatusCode { get { return _StatusCode; } set { _StatusCode = value; } }

       
        public HttpWebResult(Stream stream = default)
        {
            _ContentType = string.Empty;
            _Headers = new List<KeyValuePair<string, string>>();
            if(stream != null)
            {
                _OutputStream = stream;
            }
            else
            {
                _OutputStream = new MemoryStream();
            }
            _StatusCode = HttpStatusCode.OK;
        }
        public async Task<byte[]> ToBytesAsync()
        {
            byte[] result = null;
            if (_OutputStream?.Length > 0)
            {
                _OutputStream.Seek(0, SeekOrigin.Begin);
                using (var ms = new MemoryStream())
                {
                    try
                    {
                        await _OutputStream.CopyToAsync(ms);
                    }
                    catch { }
                    ms.Close();
                    ms.Dispose();
                    result = ms.ToArray();
                }
            }

            return result;
        }
        public async Task<string> ToStringAsync(Encoding encoding = default)
        {
            string result = string.Empty;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            try
            {
                byte[] resultBytes = await ToBytesAsync();
                result = encoding.GetString(resultBytes);
            }
            catch { }
            return result;
        }

        public void Dispose()
        {

            try
            {
                if(_OutputStream != null)
                {
                    _OutputStream.Dispose();
                }
            }
            catch { }
        }
    }
}
