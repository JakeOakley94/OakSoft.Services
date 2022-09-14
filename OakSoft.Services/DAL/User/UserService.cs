using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    //TODO: Fix after adding FakeSecurityQuestionRepository.
    public class UserService : RepositoryService<User>, IUserService
    {
        private IUserActionRecordService _userActionRecordDAL;

        public UserService
            (
            IUserRepository repo,
            IUserActionRecordService userActionRecordDAL,
            ILogger logger
            ) : base(repo, logger)
        {
            _userActionRecordDAL = userActionRecordDAL;
            CheckIfSystemUserExistsAndCreateIfDoesnt();
            CheckIfAdminUserExistsAndCreateIfDoesnt();
        }

        private void CheckIfSystemUserExistsAndCreateIfDoesnt()
        {
            var systemUser = GetSingle(Identifiers.SystemUserId);
            if (systemUser == null)
            {
                var toCreate = new User();
                toCreate.Id = Identifiers.SystemUserId;
                toCreate.Username = "System";
                toCreate.Email = "LifeMangerSystem@oaksoft.us";
                toCreate.EmailConfirmed = true;
                toCreate.PhoneNumber = "555-555-5555";
                toCreate.PhoneNumberConfirmed = true;
                toCreate.FirstName = "LifeManager";
                toCreate.LastName = "System";
                toCreate.LastPasswordResetDate = DateTime.UtcNow;
                Create(toCreate, "AFA2065F361678DEC95FCF51B51657016EC41CD70B5922E829705853D9");
                Logger.WriteToLog(LogSeverity.Information, Strings.SystemUserCreated);
            }
        }

        private void CheckIfAdminUserExistsAndCreateIfDoesnt()
        {
            var adminUser = GetSingle(Identifiers.AdminUserId);
            if (adminUser == null)
            {
                var toCreate = new User();
                toCreate.Id = Identifiers.AdminUserId;
                toCreate.Username = "Admin";
                toCreate.Email = "Admin@oaksoft.us";
                toCreate.EmailConfirmed = true;
                toCreate.PhoneNumber = "555-555-5555";
                toCreate.PhoneNumberConfirmed = true;
                toCreate.FirstName = "Oaksoft";
                toCreate.LastName = "Admin";
                toCreate.LastPasswordResetDate = DateTime.UtcNow;
                toCreate.UserSecurityQuestions = new List<UserSecurityQuestion>();
                //toCreate.UserSecurityQuestions.Add(new UserSecurityQuestion(toCreate.Id, (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteCompany, "OakSoft"));
                //toCreate.UserSecurityQuestions.Add(new UserSecurityQuestion(toCreate.Id, (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteHobby, "Software Development"));
                Create(toCreate, "LifeManager");
                Logger.WriteToLog(LogSeverity.Information, Strings.AdminAccountCreated);
            }
        }

        public User GetByUsername(string username)
        {
            if (string.IsNullOrEmpty(username)) return null;
            return Repo.FindBy(x => x.Username.ToLower() == username.ToLower()).FirstOrDefault();
        }

        public User Authenticate(string username, string password)
        {
            try
            {
                var attemptTime = DateTime.UtcNow;
                var user = Repo.GetSingle(x => x.Username.ToLower() == username.ToLower());
                if (user == null) return null;
                var success = Cryptography.CompareByteArrays(user.Hash, Cryptography.GenerateSaltedHash(password, user.Salt));
                var retVal = success ? user : null;

                if (retVal is null)
                {
                    _userActionRecordDAL.CreateUserActionRecord(UserActionRecordType.LoginFailure, user.Id, attemptTime);
                }
                else
                {
                    _userActionRecordDAL.CreateUserActionRecord(UserActionRecordType.LoginSuccess, user.Id, attemptTime);
                }

                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.GeneralException, new object[2] { ex.TargetSite, ex.Message }));
                return null;
            }
        }

        public bool Create(User item, string password)
        {
            try
            {
                item.Salt = Cryptography.CreateSalt();
                item.Hash = Cryptography.GenerateSaltedHash(password, item.Salt);
                return base.Create(item);
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.GeneralException, new object[2] { ex.TargetSite, ex.Message }));
                return false;
            }
        }

        public bool ResetPassword(Guid userId, string newPassword)
        {
            var attemptTime = DateTime.UtcNow;

            try
            {
                var user = Repo.GetSingle(userId);

                if (user is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { "User", $"UserId: {userId}" }));
                    return false;
                }

                user.Hash = Cryptography.GenerateSaltedHash(newPassword, user.Salt);
                Repo.Update(user);
                Repo.Commit();
                _userActionRecordDAL.CreateUserActionRecord(UserActionRecordType.ResetPasswordSuccess, userId, attemptTime);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.GeneralException, new object[2] { ex.TargetSite, ex.Message }));
                _userActionRecordDAL.CreateUserActionRecord(UserActionRecordType.ResetPasswordFailure, userId, attemptTime);
                return false;
            }
        }

        public override bool Update(User item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.Id == item.Id);
                if (repoItem is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { item.GetType().Name, item.Id }));
                    return false;
                }
                repoItem.FirstName = item.FirstName;
                repoItem.LastName = item.LastName;
                repoItem.Email = item.Email;
                repoItem.Hash = item.Hash;
                repoItem.LastPasswordResetDate = item.LastPasswordResetDate;
                repoItem.PhoneNumber = item.PhoneNumber;
                repoItem.PhoneNumberConfirmed = item.PhoneNumberConfirmed;
                repoItem.Username = item.Username;
                repoItem.EmailConfirmed = item.EmailConfirmed;
                repoItem.UseTwoFactorAuthentication = item.UseTwoFactorAuthentication;
                Repo.Update(repoItem);
                Repo.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.GeneralException, new object[2] { ex.TargetSite, ex.Message }));
                return false;
            }
        }

    }
}
