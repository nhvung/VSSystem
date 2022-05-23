using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Net.Mail
{
    public class SendMailReponse
    {
        TimeSpan _SentTime;
        public TimeSpan SentTime { get { return _SentTime; } set { _SentTime = value; } }

        string _Message;
        public string Message { get { return _Message; } set { _Message = value; } }

        string _MessageID;
        public string MessageID { get { return _MessageID; } set { _MessageID = value; } }

        int _StatusCode;
        public int StatusCode { get { return _StatusCode; } set { _StatusCode = value; } }

        long _Mail_ID;
        public long Mail_ID { get { return _Mail_ID; } set { _Mail_ID = value; } }

        string _ExtendInfo;
        public string ExtendInfo { get { return _ExtendInfo; } set { _ExtendInfo = value; } }

        public SendMailReponse()
        {
            _SentTime = new TimeSpan();
            _Message = string.Empty;
            _MessageID = string.Empty;
            _StatusCode = 0;
            _Mail_ID = 0;
            _ExtendInfo = string.Empty;
        }
    }
}
