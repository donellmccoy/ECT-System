using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class Unit : Entity, IHasAssignedId<int>
    {
        private Dictionary<string, Int32> _reportingChain;

        public virtual string Address1 { get; set; }

        public virtual string Address2 { get; set; }

        public virtual string BaseCode { get; set; }

        public virtual string City { get; set; }

        public virtual string CommandCode { get; set; }

        public virtual string CommandStructLevel { get; set; }

        public virtual string CommandStructOperationType { get; set; }

        public virtual string CommandStructUTC { get; set; }

        public virtual string Component { get; set; }

        public virtual string Country { get; set; }

        public virtual AppUser CreatedBy { get; set; }

        public virtual DateTime? CreatedDate { get; set; }

        public virtual string Email { get; set; }

        public virtual Unit GainingCommand { get; set; }

        public virtual bool InActive { get; set; }

        public virtual bool IsCollocated { get; set; }

        public virtual string MedicalService { get; set; }

        public virtual AppUser ModifiedBy { get; set; }

        public virtual DateTime? ModifiedDate { get; set; }

        public virtual DateTime? MrdssDocDate { get; set; }

        public virtual string MrdssDocId { get; set; }

        public virtual string MrdssDocReview { get; set; }

        public virtual string MrdssKind { get; set; }

        public virtual string Name { get; set; }

        public virtual string NameAndPasCode
        {
            get
            {
                return Name + " (" + PasCode + ")";
            }
        }

        public virtual Unit ParentUnit { get; set; }

        public virtual string PasCode { get; set; }

        public virtual string PostalCode { get; set; }

        public virtual Dictionary<string, Int32> ReportingStructure
        {
            get
            {
                if (_reportingChain == null)
                {
                    _reportingChain = new Dictionary<string, Int32>();
                }
                return _reportingChain;
            }
            set { _reportingChain = value; }
        }

        public virtual string State { get; set; }

        public virtual string TimeZone { get; set; }

        public virtual string Uic { get; set; }

        public virtual string UnitDet { get; set; }

        public virtual string UnitKind { get; set; }

        public virtual string UnitNumber { get; set; }

        public virtual string UnitType { get; set; }

        public virtual bool UserModified { get; set; }

        public virtual UnitLookup GetUnitLookup()
        {
            UnitLookup unit = new UnitLookup();

            unit.Id = Id;
            unit.Name = Name;
            unit.PasCode = PasCode;

            return unit;
        }

        /// <inheritdoc/>
        public virtual void SetId(int id)
        {
            Id = id;
        }
    }
}