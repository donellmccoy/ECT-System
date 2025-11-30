using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class EMailMessage
    {
        public EMailMessage(string subject, string body, string from, StringCollection toAddressList)
        {
            mail = new MailMessage { Body = body, Subject = subject };
            foreach (string toAddress in toAddressList)
            {
                AddBccAddress(toAddress);
            }
            AddFromAddress(from);
        }

        public EMailMessage(string subject, string body, string fromAddress, string toAddress)
        {
            mail = new MailMessage { Subject = subject, Body = body };
            AddBccAddress(toAddress);
            AddFromAddress(fromAddress);
        }

        public virtual string from { get; set; }
        public virtual MailMessage mail { get; set; }
        public virtual StringCollection toList { get; set; }

        public void AddBccAddress(string toAddress)
        {
            try
            {
                if (toAddress.Trim() != String.Empty)
                {
                    if (!mail.Bcc.Contains(new MailAddress(toAddress)))
                    {
                        mail.Bcc.Add(toAddress);
                    }
                }
            }
            catch (Exception exception)
            {
                LogManager.LogError(exception, "Invalid email address. " + toAddress);
            }
        }

        public void AddFromAddress(string fromAddress)
        {
            try
            {
                if (fromAddress.Trim() != String.Empty)
                {
                    mail.From = new MailAddress(from);
                }
            }
            catch (Exception exception)
            {
                LogManager.LogError(exception, "Invalid  email address. " + fromAddress);
            }
        }

        public void AddRecipient(StringCollection toAddressList)
        {
            foreach (string toAddress in toAddressList)
            {
                AddBccAddress(toAddress);
            }
        }

        public void AddRecipient(string toAddress)
        {
            AddBccAddress(toAddress);
        }

        /// <summary>
        /// Returns the lowest Innerexception (which often has the most detailed information)
        /// </summary>
        /// <param name="e">Exception</param>
        /// <returns>Lowest inner exception</returns>
        public Exception getLowestException(Exception e)
        {
            int i = 0;
            //Just in case there is somehow an infinite loop of innerExceptions...
            while (e.InnerException != null && i < 10)
            {
                e = e.InnerException;
                i++;
            }
            return e;
        }

        public bool Send(int templateId = 0, int userid = 0) //templateId is newly added, defualt is zero for existing code
        {
            string emailSetting = "Y";
            bool toAddressExists = false;
            bool bccAddressExists = false;
            bool ccAddressExists = false;
            bool failure = false;
            StringCollection failedRecipients = new StringCollection();

            if ((mail.Subject.Length == 0) || (mail.Body.Length == 0))
            {
                return false;
            }

            if ((mail.To != null) && (mail.To.Count > 0))
            {
                toAddressExists = true;
            }

            if ((mail.CC != null) && (mail.CC.Count > 0))
            {
                ccAddressExists = true;
            }

            if ((mail.Bcc != null) && (mail.Bcc.Count > 0))
            {
                bccAddressExists = true;
            }

            if (!(toAddressExists || bccAddressExists || ccAddressExists))
            {
                return false;
            }

            if (mail.From == null)
            {
                if (ConfigurationManager.AppSettings["EmailFrom"] != null)
                {
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["EmailFrom"]);
                }
            }

            if (ConfigurationManager.AppSettings["EmailEnabled"] != null)
            {
                emailSetting = ConfigurationManager.AppSettings["EmailEnabled"];
            }

            if (emailSetting != "Y")
            {
                return true; //don't send anything, emailing is turned off
            }

            string bccAddresses = string.Join(";", (from MailAddress m in mail.Bcc select m.Address).ToArray());
            string ccAddresses = string.Join(";", (from MailAddress m in mail.CC select m.Address).ToArray());
            string toAddresses = string.Join(";", (from MailAddress m in mail.To select m.Address).ToArray());

            SmtpClient server = new SmtpClient();

            try
            {
                server.Send(mail);
                if (templateId == 0)
                {
                    LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, "");
                }
                else
                {
                    LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, "", templateId, userid);
                }
            }
            catch (SmtpFailedRecipientsException ex)
            {
                failure = true;
                StringBuilder failedrcpts = new StringBuilder();

                for (int i = 0; i < ex.InnerExceptions.Length; i++)
                {
                    failedrcpts.Append(ex.InnerExceptions[i].FailedRecipient + "(Reason:" + ex.InnerExceptions[i].StatusCode.ToString() + ")");
                    failedrcpts.Append(",");
                    failedRecipients.Add(ex.InnerExceptions[i].FailedRecipient.Replace("<", String.Empty).Replace(">", String.Empty));
                }
                failedrcpts.Remove(failedrcpts.Length - 1, 1);
                LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, failedrcpts.ToString());
                LogManager.LogError(ex, "SmtpFailedRecipientsException");
            }
            catch (SmtpFailedRecipientException ex)
            {
                failure = true;
                failedRecipients.Add(ex.FailedRecipient.Replace("<", String.Empty).Replace(">", String.Empty));
                LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, ex.FailedRecipient + "(Reason:" + ex.StatusCode.ToString() + ")");
                LogManager.LogError(ex, "SmtpFailedRecipientsException");
            }
            catch (SmtpException ex)
            {
                failure = true;
                LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, "SmtpException");
                LogManager.LogError(getLowestException(ex), "SmtpException");
            }
            catch (Exception ex)
            {
                failure = true;
                LogManager.LogMail(toAddresses, ccAddresses, bccAddresses, mail.Subject, mail.Body, "GenericException");
                LogManager.LogError(getLowestException(ex), "UnknownException");
            }

            //Retry Logic - Will only retry if address count > 1
            //if (failure == true && (mail.Bcc.Count > 1 || mail.To.Count > 1))
            if (failure == true && failedRecipients.Count > 0)
            {
                List<MailAddress> addresses = new List<MailAddress>();

                //if (bccAddressExists)
                //{
                //    foreach (MailAddress addr in mail.Bcc)
                //    {
                //        addresses.Add(addr);
                //    }
                //}
                //if (toAddressExists)
                //{
                //    foreach (MailAddress addr in mail.To)
                //    {
                //        addresses.Add(addr);
                //    }
                //}

                foreach (string addr in failedRecipients)
                {
                    addresses.Add(new MailAddress(addr));
                }

                mail.To.Clear();
                mail.Bcc.Clear();
                mail.CC.Clear();

                foreach (MailAddress addr in addresses)
                {
                    mail.To.Clear();
                    mail.To.Add(addr);
                    try
                    {
                        server.Send(mail);
                        LogManager.LogMail(addr.Address, "", "", mail.Subject, mail.Body, "Second Try Succeeded");
                    }
                    catch (Exception ex)
                    {
                        LogManager.LogMail(addr.Address, "", "", mail.Subject, mail.Body, "Second Try Failed");
                        LogManager.LogError(getLowestException(ex), "Second Email Failure");
                    }
                }
            }

            return true;
        }

        public void SetField(String key, String value)
        {
            mail.Body = mail.Body.Replace("{" + key.ToUpper() + "}", value);
        }
    }
}