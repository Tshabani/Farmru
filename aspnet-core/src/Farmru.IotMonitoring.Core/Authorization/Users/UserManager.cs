using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Runtime.Caching;
using Farmru.IotMonitoring.Authorization.Roles;
using Abp.Authorization.Roles;
using Abp.Runtime.Validation;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Abp.IdentityFramework;
using Microsoft.EntityFrameworkCore;

namespace Farmru.IotMonitoring.Authorization.Users
{
    public class UserManager : AbpUserManager<Role, User>
    {
        public UserManager(
          RoleManager roleManager,
          UserStore store,
          IOptions<IdentityOptions> optionsAccessor,
          IPasswordHasher<User> passwordHasher,
          IEnumerable<IUserValidator<User>> userValidators,
          IEnumerable<IPasswordValidator<User>> passwordValidators,
          ILookupNormalizer keyNormalizer,
          IdentityErrorDescriber errors,
          IServiceProvider services,
          ILogger<UserManager<User>> logger,
          IPermissionManager permissionManager,
          IUnitOfWorkManager unitOfWorkManager,
          ICacheManager cacheManager,
          IRepository<OrganizationUnit, long> organizationUnitRepository,
          IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
          IOrganizationUnitSettings organizationUnitSettings,
          ISettingManager settingManager, 
          IRepository<UserLogin, long> userLoginRepository)
          : base(
              roleManager,
              store,
              optionsAccessor,
              passwordHasher,
              userValidators,
              passwordValidators,
              keyNormalizer,
              errors,
              services,
              logger,
              permissionManager,
              unitOfWorkManager,
              cacheManager,
              organizationUnitRepository,
              userOrganizationUnitRepository,
              organizationUnitSettings,
              settingManager,
              userLoginRepository)
        {
        }

        public async Task<User> CreateUser(string username, bool createLocalPassword, string password, string passwordConfirmation, string firstname, string lastname, string mobileNumber, string emailAddress)
        {
            var validationResults = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(username))
                validationResults.Add(new ValidationResult("Username is mandatory"));
            else
            // check duplicate usernames
            if (await UserNameAlreadyInUse(username))
                validationResults.Add(new ValidationResult("User with the same username already exists"));

            if (createLocalPassword)
            {
                if (string.IsNullOrWhiteSpace(password))
                    validationResults.Add(new ValidationResult("Password is mandatory"));

                if (string.IsNullOrWhiteSpace(passwordConfirmation))
                    validationResults.Add(new ValidationResult("Password Confirmation is mandatory"));

                if (!string.IsNullOrWhiteSpace(password) &&
                    !string.IsNullOrWhiteSpace(passwordConfirmation) &&
                    password != passwordConfirmation)
                    validationResults.Add(new ValidationResult("Password Confirmation must be the same as Password"));
            }

            if (validationResults.Any())
                throw new AbpValidationException("Please correct the errors and try again", validationResults);

            // 1. create user
            var user = new User()
            {
                EmailAddress = emailAddress ?? "",
                PhoneNumber = mobileNumber ?? "",
                TenantId = AbpSession.TenantId,
                IsEmailConfirmed = true,
                UserName = username,
                //UserName ??= "", // just to prevent crash in the ABP layer, it should be validated before
                Name = firstname, // todo: make a decision how to handle duplicated properties in the User and Person classes (option 1 - use Person as a source and sync onw way, option 2 - remove duplicates from User, but in some cases we needn't Person for a user)
                Surname = lastname,
                //SupportedPasswordResetMethods = supportedPasswordResetMethods
            };


            user.SetNormalizedNames();
            await this.InitializeOptionsAsync(AbpSession.TenantId);

            var newPassword = createLocalPassword
                ? password
                : Guid.NewGuid().ToString();
            CheckErrors(await CreateAsync(user, newPassword));

            return user;
        }
        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        /// <summary>
        /// Checks is specified username already used by another person
        /// </summary>
        /// <returns></returns>
        private async Task<bool> UserNameAlreadyInUse(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            var normalizedUsername = NormalizeName(username);
            return await Users.Where(u => u.NormalizedUserName == normalizedUsername).AnyAsync();
        }
    }
}
