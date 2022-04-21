using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSSystem.Net.Mail
{
    public class SmtpMailProcess : IMailProcess
    {
        SmtpClient _smtp;

        public SmtpMailProcess(string host, string username, string password, int port, bool enableSSL = true)
        {
            _smtp = new SmtpClient(host, port);
            _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            if (enableSSL)
            {
                _smtp.TargetName = "STARTTLS/" + host;
            }

            _smtp.UseDefaultCredentials = false;
            _smtp.Credentials = new NetworkCredential(username, password);
            _smtp.EnableSsl = enableSSL;
        }
        public EMailProcessType Type { get { return EMailProcessType.Smtp; } }
        Action<SendMailReponse> _OnSendComplete;
        public Action<SendMailReponse> OnSendComplete { set { _OnSendComplete = value; } }

        public Task<SendMailReponse> SendMailAsync(MailMessageInfo mailInfo, int retryTimes = 0)
        {
            SendMailReponse result = new SendMailReponse();
            result.Mail_ID = mailInfo.Mail_ID;
            try
            {
                if (string.IsNullOrEmpty(mailInfo.FromAddress))
                {
                    throw new Exception("From Address is empty.");
                }
                if (mailInfo.ToAddresses?.Count > 0)
                {
                    DateTime bTime = DateTime.Now;
                    MailMessage mMess = new MailMessage()
                    {
                        BodyEncoding = mailInfo.Encoding,
                        SubjectEncoding = mailInfo.Encoding,
                        HeadersEncoding = mailInfo.Encoding
                    };

                    if (!string.IsNullOrEmpty(mailInfo.Subject))
                    {
                        mMess.Subject = mailInfo.Subject;
                    }

                    mMess.From = new MailAddress(mailInfo.FromAddress, mailInfo.DisplayName, mailInfo.Encoding);
                    foreach (string toAddress in mailInfo.ToAddresses)
                    {
                        if (!string.IsNullOrEmpty(toAddress))
                        {
                            try { mMess.To.Add(toAddress); }
                            catch { }
                        }
                    }
                    if (mailInfo.CcAddresses?.Count > 0)
                    {
                        foreach (string ccAddress in mailInfo.CcAddresses)
                        {
                            if (!string.IsNullOrEmpty(ccAddress))
                            {
                                try { mMess.CC.Add(ccAddress); }
                                catch { }
                            }
                        }
                    }


                    if (mailInfo.BccAddresses?.Count > 0)
                    {
                        foreach (string bccAddress in mailInfo.BccAddresses)
                        {
                            if (!string.IsNullOrEmpty(bccAddress))
                            {
                                try { mMess.Bcc.Add(bccAddress); }
                                catch { }
                            }
                        }
                    }

                    if (mMess.To.Count == 0)
                    {
                        result.StatusCode = (int)SmtpStatusCode.MailboxUnavailable;
                        result.Message = "Email is empty.";
                        return Task.FromResult(result);
                    }

                    if (mailInfo.AlternateViewItem != null)
                    {
                        AlternateView alternateView = new AlternateView(new MemoryStream(mailInfo.AlternateViewItem.ContentBytes), mailInfo.AlternateViewItem.MediaType);
                        alternateView.ContentId = mailInfo.AlternateViewItem.ContentID;
                        alternateView.TransferEncoding = TransferEncoding.Base64;

                        if (mailInfo.LinkResourceItems?.Count > 0)
                        {
                            foreach (var lrItem in mailInfo.LinkResourceItems)
                            {
                                LinkedResource linkedResource = new LinkedResource(new MemoryStream(lrItem.ContentBytes), lrItem.MediaType)
                                {
                                    ContentId = lrItem.ContentID,
                                };
                                alternateView.LinkedResources.Add(linkedResource);
                            }
                        }
                        mMess.AlternateViews.Add(alternateView);
                    }
                    if (mailInfo.AttachmentItems?.Count > 0)
                    {
                        foreach (var attItem in mailInfo.AttachmentItems)
                        {
                            Attachment attachment = new Attachment(new MemoryStream(attItem.ContentBytes), attItem.ContentID, attItem.MediaType) { ContentId = attItem.ContentID };
                            mMess.Attachments.Add(attachment);
                        }
                    }

                    int retry = 0;
                    RETRY:
                    try
                    {
                        _smtp.Send(mMess);
                    }
                    catch (SmtpException ex)
                    {
                        retry++;
                        if (retry < retryTimes)
                        {
                            Thread.Sleep(5000);
                            goto RETRY;
                        }
                        result.Message = ex.Message;
                        result.StatusCode = (int)ex.StatusCode;
                    }
                    catch (Exception ex)
                    {
                        retry++;
                        if (retry < retryTimes)
                        {
                            Thread.Sleep(5000);
                            goto RETRY;
                        }
                        result.Message = ex.Message;
                        result.StatusCode = (int)SmtpStatusCode.MailboxUnavailable;
                    }

                    DateTime eTime = DateTime.Now;
                    result.SentTime = eTime - bTime;

                    if(_OnSendComplete != null)
                    {
                        var extInfo = new
                        {
                            mailInfo.ToAddresses,
                            mailInfo.CcAddresses,
                            mailInfo.BccAddresses,
                            mailInfo.Subject,
                        };
                        result.ExtendInfo = JsonConvert.SerializeObject(extInfo);
                        _ = Task.Run(() => _OnSendComplete.Invoke(result));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Task.FromResult(result);
        }
    }
}
