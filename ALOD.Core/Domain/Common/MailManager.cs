using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ALOD.Core.Domain.Common
{
    [Serializable]
    public class MailManager
    {
        protected IEmailDao mailService;
        protected Dictionary<int, EMailMessage> messages;

        public MailManager(IEmailDao emailService)
        {
            mailService = emailService;
            messages = new Dictionary<int, EMailMessage>();
        }

        //Template with a multiple to addresses
        public void AddTemplate(int templateId, string from, string to)
        {
            if (!(messages.ContainsKey(templateId)))
            {
                EMailMessage msg = mailService.CreateMessage(templateId, from, to);
                messages.Add(templateId, msg);
            }
            messages[templateId].AddRecipient(to);
        }

        //Template with a single to address
        public void AddTemplate(int templateId, string from, StringCollection toList)
        {
            if (!(messages.ContainsKey(templateId)))
            {
                EMailMessage msg = mailService.CreateMessage(templateId, from, toList);
                messages.Add(templateId, msg);
            }
            messages[templateId].AddRecipient(toList);
        }

        public void RemoveEmptyAddresses(EMailMessage msg)
        {
            if (msg.toList != null && msg.toList.Count > 0)
            {
                List<String> query = (from string name in msg.toList where name != string.Empty select name).ToList();
                msg.toList = new StringCollection();
                msg.toList.AddRange(query.ToArray());
            }
        }

        public void Send(int templateId)
        {
            if (messages.ContainsKey(templateId))
            {
                RemoveEmptyAddresses(messages[templateId]);
                messages[templateId].Send();
            }
        }

        public void SendAll(int templateid = 0, int userId = 0)
        {
            foreach (int item in messages.Keys)
            {
                RemoveEmptyAddresses(messages[item]);
                if (templateid == 0)
                {
                    messages[item].Send();
                }
                else
                {
                    messages[item].Send(templateid, userId);
                }
            }
        }

        public void SetField(int templateId, string key, string value)
        {
            if (messages.ContainsKey(templateId))
            {
                messages[templateId].SetField(key, value);
            }
        }

        public void SetField(string key, string value)
        {
            foreach (int item in messages.Keys)
            {
                messages[item].SetField(key, value);
            }
        }
    }
}