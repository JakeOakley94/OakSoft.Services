using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class UserOptionsService : RepositoryService<UserOptions>, IUserOptionsService
    {
        public UserOptionsService(IUserOptionsRepository repo, ILogger logger) : base(repo, logger)
        { 

        }
        
        public UserOptions GetCurrentOptionsForUser(Guid userId)
        {
            var userOptions = Repo.FindBy(x => x.UserId == userId);

            if (userOptions is null || userOptions.Count() < 1)
            {
                Logger.WriteToLog(OakSoftCore.LogSeverity.Error, $"The user id: {userId} does not have any options currently associated with their account.");
                return null;
            }

            return userOptions.FirstOrDefault();
        }

        public override bool Update(UserOptions item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.UserId == item.UserId);
                if (repoItem is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { item.GetType().Name, $"User Id: {item.UserId}" }));
                    return false;
                }
   

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
