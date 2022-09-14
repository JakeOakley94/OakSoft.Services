using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class SecurityQuestionService : RepositoryService<OakSoft.Model.SecurityQuestion>, ISecurityQuestionService
    {
        public SecurityQuestionService(ISecurityQuestionRepository repo, ILogger logger) : base(repo, logger)
        {

        }

        public override bool Update(OakSoft.Model.SecurityQuestion item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.Id == item.Id);

                if (repoItem is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { item.GetType().Name, $"Id: {item.Id}" }));
                    return false;
                }

                repoItem.Question = item.Question;
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


