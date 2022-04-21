using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VSSystem.Net.Mail
{
    public interface IMailProcess
    {
        Task<SendMailReponse> SendMailAsync(MailMessageInfo mailInfo, int retryTimes = 0);
        EMailProcessType Type { get; }
        Action<SendMailReponse> OnSendComplete { set; }
    }
}
