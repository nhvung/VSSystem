using System;
using System.Collections.Generic;
using System.Text;

namespace VSSystem.Net.Mail
{
    public class MailMessageInfo
    {

        string _Subject;
        public string Subject { get { return _Subject; } set { _Subject = value; } }

        string _FromAddress;
        public string FromAddress { get { return _FromAddress; } set { _FromAddress = value; } }

        string _DisplayName;
        public string DisplayName { get { return _DisplayName; } set { _DisplayName = value; } }

        List<string> _ToAddresses;
        public List<string> ToAddresses { get { return _ToAddresses; } set { _ToAddresses = value; } }

        List<string> _CcAddresses;
        public List<string> CcAddresses { get { return _CcAddresses; } set { _CcAddresses = value; } }

        List<string> _BccAddresses;
        public List<string> BccAddresses { get { return _BccAddresses; } set { _BccAddresses = value; } }


        Encoding _Encoding;
        public Encoding Encoding { get { return _Encoding; } set { _Encoding = value; } }
        MailContentItem _AlternateViewItem;
        public MailContentItem AlternateViewItem { get { return _AlternateViewItem; } set { _AlternateViewItem = value; } }

        List<MailContentItem> _AttachmentItems;
        public List<MailContentItem> AttachmentItems { get { return _AttachmentItems; } set { _AttachmentItems = value; } }

        List<MailContentItem> _LinkResourceItems;
        public List<MailContentItem> LinkResourceItems { get { return _LinkResourceItems; } set { _LinkResourceItems = value; } }

        long _Mail_ID;
        public long Mail_ID { get { return _Mail_ID; } set { _Mail_ID = value; } }

        public MailMessageInfo()
        {
            _FromAddress = "";
            _ToAddresses = new List<string>();
            _CcAddresses = new List<string>();
            _BccAddresses = new List<string>();
            _DisplayName = "";
            _Encoding = Encoding.UTF8;
            _AlternateViewItem = null;
            _AttachmentItems = new List<MailContentItem>();
            _LinkResourceItems = new List<MailContentItem>();
            _Mail_ID = 0;
        }
    }
}
