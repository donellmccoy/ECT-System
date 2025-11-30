using System;

namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class Rwoa : Entity
    {
        public virtual string CommentsBackToSender { get; set; }
        public virtual int CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime DateSent { get; set; }
        public virtual DateTime? DateSentBack { get; set; }
        public virtual string ExplantionSendingBack { get; set; }
        public virtual short ReasonSentBack { get; set; }
        public virtual int RefId { get; set; }
        public virtual int rerouting { get; set; }
        public virtual string Sender { get; set; }
        public virtual string SentTo { get; set; }
        public virtual short Workflow { get; set; }
        public virtual int WorkStatus { get; set; }
    }
}