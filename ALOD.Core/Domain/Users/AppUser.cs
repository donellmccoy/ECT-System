using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.ServiceMembers;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ALOD.Core.Domain.Users
{
    [Serializable]
    public class AppUser : Entity, IAuditable
    {
        private IList<UserRole> allRoles = new List<UserRole>();

        public AppUser()
        {
            Status = AccessStatus.None;
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
        }

        public virtual string AccountComment { get; set; }

        public virtual DateTime? AccountExpiration { get; set; }

        public virtual Unit ActiveDutyUnit { get; set; }

        public virtual Address Address { get; set; }

        public virtual IList<UserRole> AllRoles
        {
            get { return allRoles; }
            private set { allRoles = value; }
        }

        public virtual string AlternateSignatureName
        {
            get
            {
                string rank = "";

                rank = Rank.Rank;

                if (string.IsNullOrEmpty(MiddleName))
                {
                    return FirstName + " " + LastName + ", " + rank;
                }
                else
                {
                    return FirstName + " " + MiddleName.ToUpper()[0] + ". " + LastName + ", " + rank;
                }
            }
        }

        public virtual string CommentName
        {
            get
            {
                return Rank.Rank + " " + LastName + ", " + FirstName;
            }
        }

        public virtual string Component { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual UserRole CurrentRole
        {
            get;
            set;
        }

        public virtual string CurrentRoleName
        {
            get
            {
                return CurrentRole.Group.Description;
            }
        }

        public virtual Unit CurrentUnit
        {
            get
            {
                if (ActiveDutyUnit != null)
                {
                    return ActiveDutyUnit;
                }
                else
                {
                    return Unit;
                }
            }
        }

        public virtual int CurrentUnitId
        {
            get
            {
                if (ActiveDutyUnit != null)
                    return ActiveDutyUnit.Id;

                return Unit.Id;
            }
        }

        public virtual string CurrentUnitName
        {
            get { return CurrentUnit.Name; }
        }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual Byte DisabledBy { get; set; }

        public virtual int DocumentGroupId { get; set; }

        public virtual string DSN { get; set; }

        public virtual string EDIPIN { get; set; }

        public virtual string Email { get; set; }

        public virtual string Email2 { get; set; }

        public virtual string Email3 { get; set; }

        public virtual string FirstLastName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public virtual string FirstName { get; set; }

        public virtual string FormName
        {
            get
            {
                string name = LastName + " " + FirstName;

                if (!string.IsNullOrEmpty(MiddleName))
                {
                    name += " " + MiddleName.Substring(0, 1);
                }

                return name;
            }
        }

        public virtual string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                {
                    return FirstName + " " + LastName;
                }
                else
                {
                    if (MiddleName.Length == 1)
                        return LastName + " " + FirstName + " " + MiddleName;
                    else
                        return LastName + " " + FirstName + " " + MiddleName.Substring(0, 1);
                }
            }
        }

        public virtual DateTime? LastLoginDate { get; set; }

        public virtual string LastName { get; set; }

        public virtual string MemoSignatureName
        {
            get
            {
                string rank = "";

                rank = Rank.Rank;

                if (string.IsNullOrEmpty(MiddleName))
                {
                    return FirstName.ToUpper() + " " + LastName.ToUpper() + ", " + rank;
                }
                else
                {
                    return FirstName.ToUpper() + " " + MiddleName.ToUpper()[0] + ". " + LastName.ToUpper() + ", " + rank;
                }
            }
        }

        public virtual string MiddleInitial
        {
            get
            {
                if (string.IsNullOrEmpty(MiddleName))
                    return string.Empty;

                return MiddleName[0].ToString();
            }
        }

        public virtual string MiddleName { get; set; }

        public virtual AppUser ModifiedBy { get; set; }

        /// <inheritdoc/>
        public virtual DateTime ModifiedDate { get; set; }

        public virtual string NameRankAndRole
        {
            get
            {
                return Rank.Title + " " + LastName + ", " + FirstName + " - " + CurrentRole.Group.Description;
            }
        }

        public virtual string Phone { get; set; }

        public virtual UserRank Rank { get; set; }

        public virtual string RankAndName
        {
            get
            {
                return Rank.Rank + " " + LastName + ", " + FirstName;
            }
        }

        public virtual bool ReceiveEmail { get; set; }

        public virtual bool ReceiveReminderEmail { get; set; }

        public virtual ReportingView? ReportView { get; set; }

        public virtual string SignatureName
        {
            get
            {
                string rank = "";

                rank = Rank.Rank;

                if (string.IsNullOrEmpty(MiddleName))
                {
                    return rank + " " + FirstName + " " + LastName;
                }
                else
                {
                    return rank + " " + FirstName + " " + MiddleName.ToUpper()[0] + ". " + LastName;
                }
            }
        }

        public virtual string SignatureTitle
        {
            get
            {
                return "";
            }
        }

        public virtual string SSN { get; set; }
        public virtual AccessStatus Status { get; set; }

        public virtual string StatusDescription
        {
            get { return Status.ToString(); }
        }

        public virtual string Title { get; set; }
        public virtual Unit Unit { get; set; }
        public virtual int UnitView { get; set; }
        public virtual string Username { get; set; }

        // 1 = View user's unit + all subordinate units to that unit. 0 = View just the user's unit
        public virtual String AllRolesString()
        {
            StringBuilder sb = new StringBuilder();
            if (AllRoles.Count > 1)
            {
                foreach (UserRole role in AllRoles)
                {
                    if (role.Group != CurrentRole.Group)
                    {
                        sb.Append(role.Group.Description);
                        sb.Append(",");
                    }
                }
                if (sb.Length > 0)
                { sb.Remove(sb.Length - 1, 1); }
                return sb.ToString();
            }
            else
            {
                return "None";
            }
        }

        public void Import(ServiceMember data)
        {
            SSN = data.SSN;
            LastName = data.LastName;
            FirstName = data.FirstName;
            MiddleName = data.MiddleName;

            Unit = new Unit();
            Unit.SetId(data.Unit.Id);

            try
            {
                if (data.Unit != null)
                    Unit = data.Unit;
            }
            catch
            { }

            Phone = data.DutyPhone;
            Component = data.Component;

            if (Address == null)
                Address = new Address();

            try
            {
                Address.Street = data.MailingAddress.Street;
                Address.City = data.MailingAddress.City;
                Address.State = data.MailingAddress.State;
                Address.Zip = data.MailingAddress.Zip;
                Address.Country = data.MailingAddress.Country;
            }
            catch
            { }

            if (Rank == null)
                Rank = new UserRank();

            Rank.SetId(data.Rank.Id);

            try
            {
                Rank.DisplayOrder = data.Rank.DisplayOrder;
                Rank.Grade = data.Rank.Grade;
                Rank.Rank = data.Rank.Rank;
                Rank.Title = data.Rank.Title;
            }
            catch
            { }
        }

        public bool IsPersonnalServiceMemberRecord(ServiceMember member)
        {
            if (member == null)
                return false;

            if (!string.IsNullOrEmpty(SSN) && SSN.Equals(member.SSN))
                return true;

            if (DoesNameMatchServiceMemberName(member))
                return true;

            return false;
        }

        private bool DoesNameMatchServiceMemberName(ServiceMember member)
        {
            if (member == null)
                return false;

            bool includeMiddleInitial = (!string.IsNullOrEmpty(MiddleName) && !string.IsNullOrEmpty(member.MiddleName));

            if (LastName.ToLower().Equals(member.LastName.ToLower()) &&
                FirstName.ToLower().Equals(member.FirstName.ToLower()) &&
                (!includeMiddleInitial || MiddleInitial.ToLower().Equals(member.MiddleInitial.ToLower())))
                return true;

            return false;
        }
    }
}