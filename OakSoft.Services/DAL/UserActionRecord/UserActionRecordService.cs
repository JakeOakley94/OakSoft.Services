using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class UserActionRecordService : RepositoryService<UserActionRecord>, IUserActionRecordService
    {
        private IUserRepository _userRepo;

        public UserActionRecordService(IUserActionRecordRepository repo, IUserRepository userRepo, ILogger logger) : base(repo, logger)
        {
            _userRepo = userRepo;
        }

        public bool CreateUserActionRecord(UserActionRecordType type, Guid userId, DateTime attemptTime)
        {
            var record = new UserActionRecord();
            var userExists = _userRepo.GetSingle(userId) is null is false;

            if (userExists is false)
            {
                return false;
            }

            try
            {
                record.UserAccountActionTypeId = (int)type;
                record.UserId = userId;
                record.Id = Guid.NewGuid();
                record.AttemptTimeUtc = attemptTime;
                base.Create(record);
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(OakSoftCore.LogSeverity.Error, string.Format(Strings.CreateItemFailed, new object[3] { record.GetType().Name, $"UserAccountActionType: {type}, UserId: {userId}", ex.Message }));
                return false;
            }
        }

        public override bool Update(UserActionRecord item)
        {
            throw new NotImplementedException("The following method is not allowed for this DAL.");
        }
    }
}
