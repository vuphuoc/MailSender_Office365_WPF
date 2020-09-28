using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MailSender.Helpers
{
    class SPCMail
    {
        static SPCMail instance;
        public static SPCMail Instance
        {
            get
            {
                if (instance == null)
                    instance = new SPCMail();
                return instance;
            }
            set { instance = value; }
        }
        public async Task<int> PrepareSPCMail(string fromEmail, string toAddress, string subject, string body, string mailsCc, string[] filePathAttachments = null)
        {

            string[] mailAddresses = toAddress.Replace(',',';').Split(';');
            List<string> _mailsCc = new List<string>();
            if (!string.IsNullOrEmpty(mailsCc))
            {
                _mailsCc.AddRange(mailsCc.Replace(',',';').Split(';'));
            }

            int result = 0;
            await Task.Run(() =>
            {
                try
                {
                    using (System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage())
                    {
                        myMail.From = new MailAddress(fromEmail);
                       
                        foreach (var address in mailAddresses)
                        {                            
                            myMail.To.Add(address.Trim());
                        }

                        //prepare file attachment
                        if (filePathAttachments != null)
                        {
                            foreach (var file in filePathAttachments)
                            {
                                Attachment at = new Attachment(file);
                                myMail.Attachments.Add(at);
                            }

                        }

                        //add mails cc
                        foreach (var mCc in _mailsCc)
                        {
                            myMail.CC.Add(new MailAddress(mCc.Trim()));
                        }


                        myMail.Subject = subject;
                        myMail.IsBodyHtml = true;
                        myMail.Body = body;
                        myMail.IsBodyHtml = true;
                      
                        using (System.Net.Mail.SmtpClient s = new System.Net.Mail.SmtpClient("office365.com",512)) //change your mail server and port here
                        {
                            s.DeliveryMethod = SmtpDeliveryMethod.Network;
                            s.UseDefaultCredentials = false;
                            s.Credentials = new System.Net.NetworkCredential(myMail.From.ToString(), Properties.Settings.Default._hostPassword);
                            s.EnableSsl = true;
                         

                            s.Send(myMail);
                        }
                    }
                    result = 1;
                }
                catch (Exception ex)
                {
                    result = -1;
                }


            });
            return result;

         
        }
    }
}
