using Abp.Auditing;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Validation;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Persons
{
    public class Person : FullAuditedEntity<Guid>
    {
        //[StoredFile(IsVersionControlled = true)]
        //public virtual StoredFile Photo { get; set; }

        [StringLength(13)]
        [Display(Name = "Identity Number")]
        public virtual string IdentityNumber { get; set; }

        public virtual RefListPersonTitle? Title { get; set; }

        [StringLength(50)]
        [Display(Name = "First Name")]
        [Audited]
        public virtual string FirstName { get; set; }

        [StringLength(50)]
        [Display(Name = "Last Name")]
        [Audited]
        public virtual string LastName { get; set; }

        [Display(Name = "Biography")]
        [StringLength(50000)]
        public virtual string Biography { get; set; }

        /// <summary>
        /// Initials override. If empty, the first letter of FirstName is taken.
        /// </summary>
        [StringLength(10), Display(Name = "Initials")]
        public virtual string Initials { get; set; }

        /// <summary>
        /// Custom short name (overrides calculated short name)
        /// </summary>
        [StringLength(60)]
        [Display(Name = "Custom Short Name")]
        public virtual string CustomShortName { get; set; }

        [StringLength(20)]
        public virtual string HomeNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Mobile Number")]
        [Audited]
        public virtual string MobileNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Alternate Mobile Number")]
        public virtual string AltMobileNumber { get; set; }

        [StringLength(100), EmailAddress]
        [Display(Name = "Email Address")]
        [Audited]
        public virtual string EmailAddress { get; set; }

        [StringLength(10), EmailAddress]
        [Display(Name = "Alternative Email Address")]
        public virtual string AltEmailAddress { get; set; }

        [Audited]
        [DisableDateTimeNormalization]
        [DataType(DataType.Date)]
        [NotInFuture]
        public virtual DateTime? DateOfBirth { get; set; }

        [Audited]
        public virtual RefListGender? Gender { get; set; }

        /// <summary>
        /// Calcuated property in the following format: FirstName + ' ' + LastName
        /// </summary> 
        public virtual string FullName { get => $"{FirstName} {LastName}"; protected set { } }

        /// <summary>
        /// User record, may be null for non registered users
        /// </summary>
        [CanBeNull]
        public virtual User User { get; set; }
    }
}
