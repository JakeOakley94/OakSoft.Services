using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class TimeIntervalService : RepositoryService<TimeInterval>, ITimeIntervalService
    {
        public TimeIntervalService(ITimeIntervalRepository repo, ILogger logger) : base(repo, logger)
        {

        }

        public override bool Update(TimeInterval item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.Id == item.Id);

                if (repoItem is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { item.GetType().Name, $"Id: {item.Id}" }));
                    return false;
                }

                repoItem.Name = item.Name;
                repoItem.Minutes = item.Minutes;
                repoItem.Days = item.Days;
                repoItem.Weeks = item.Weeks;
                repoItem.Months = item.Months;
                repoItem.Years = item.Years;
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

