using Abp.Auditing;
using Farmru.IotMonitoring.Domains;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using Farmru.IotMonitoring.Authorization.Users;
using Farmru.IotMonitoring.Validation;
using JetBrains.Annotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Farmru.IotMonitoring.Domains.Persons
{
    public class Person : FullAuditedAggregateRoot<Guid>
    {
        protected Person()
        {
        }

        [StringLength(13)]
        public virtual string IdentityNumber { get; private set; }
        public virtual RefListPersonTitle? Title { get; private set; }

        [StringLength(50)]
        public virtual string FirstName { get; private set; }

        [StringLength(50)]
        public virtual string LastName { get; private set; }

        [StringLength(50000)]
        public virtual string Biography { get; private set; }

        [StringLength(10)]
        public virtual string Initials { get; private set; }

        [StringLength(60)]
        public virtual string CustomShortName { get; private set; }

        [StringLength(20)]
        public virtual string HomeNumber { get; private set; }

        [StringLength(20)]
        public virtual string MobileNumber { get; private set; }

        [StringLength(20)]
        public virtual string AltMobileNumber { get; private set; }

        [StringLength(100), EmailAddress]
        public virtual string EmailAddress { get; private set; }

        [StringLength(10), EmailAddress]
        public virtual string AltEmailAddress { get; private set; }

        [DisableDateTimeNormalization]
        [DataType(DataType.Date)]
        [NotInFuture]
        public virtual DateTime? DateOfBirth { get; private set; }

        public virtual RefListGender? Gender { get; private set; }

        public virtual string FullName => $"{FirstName} {LastName}".Trim();

        [CanBeNull]
        public virtual User User { get; private set; }

        public static Person Create(string firstName, string lastName)
        {
            var person = new Person();
            person.SetName(firstName, lastName);
            return person;
        }

        public virtual void SetName(string firstName, string lastName)
        {
            var fn = firstName?.Trim();
            var ln = lastName?.Trim();
            if (string.IsNullOrWhiteSpace(fn) || string.IsNullOrWhiteSpace(ln))
            {
                throw new DomainRuleException("First and last name are required.");
            }

            FirstName = fn;
            LastName = ln;
        }

        public virtual void UpdateProfile(
            string identityNumber,
            RefListPersonTitle? title,
            string biography,
            string initials,
            string customShortName,
            string homeNumber,
            string mobileNumber,
            string altMobileNumber,
            string emailAddress,
            string altEmailAddress,
            DateTime? dateOfBirth,
            RefListGender? gender)
        {
            IdentityNumber = Normalize(identityNumber);
            Title = title;
            Biography = Normalize(biography);
            Initials = Normalize(initials);
            CustomShortName = Normalize(customShortName);
            HomeNumber = Normalize(homeNumber);
            MobileNumber = Normalize(mobileNumber);
            AltMobileNumber = Normalize(altMobileNumber);
            EmailAddress = Normalize(emailAddress);
            AltEmailAddress = Normalize(altEmailAddress);
            DateOfBirth = dateOfBirth;
            Gender = gender;
        }

        public virtual void LinkToUser(User user)
        {
            User = user;
        }

        private static string Normalize(string value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
