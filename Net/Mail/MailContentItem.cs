using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VSSystem.Net.Mail
{
    public class MailContentItem
    {
        string _MediaType;
        public string MediaType { get { return _MediaType; } set { _MediaType = value; } }

        string _ContentID;
        public string ContentID { get { return _ContentID; } set { _ContentID = value; } }

        //Stream _ContentStream;
        //public Stream ContentStream { get { return _ContentStream; } set { _ContentStream = value; } }

        byte[] _ContentBytes;
        public byte[] ContentBytes { get { return _ContentBytes; } set { _ContentBytes = value; } }


        public MailContentItem(string fileName, string contentID, string mediaType)
        {
            _ContentBytes = File.ReadAllBytes(fileName);
            _ContentID = contentID;
            _MediaType = mediaType;
        }
        public MailContentItem(byte[] binaryData, string contentID, string mediaType)
        {
            _ContentBytes = binaryData;
            _ContentID = contentID;
            _MediaType = mediaType;
        }
    }
}
