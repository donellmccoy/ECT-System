namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHFormFieldUpdateEventArgs
    {
        public virtual int NewFieldDisplayOrder { get; set; }
        public virtual int NewFieldId { get; set; }
        public virtual int NewFieldTypeDisplayOrder { get; set; }
        public virtual int NewFieldTypeId { get; set; }
        public virtual int NewSectionId { get; set; }
        public virtual string NewToolTip { get; set; }
        public virtual int OldFieldDisplayOrder { get; set; }
        public virtual int OldFieldId { get; set; }
        public virtual int OldFieldTypeDisplayOrder { get; set; }
        public virtual int OldFieldTypeId { get; set; }
        public virtual int OldSectionId { get; set; }
        public virtual string OldToolTip { get; set; }
    }
}