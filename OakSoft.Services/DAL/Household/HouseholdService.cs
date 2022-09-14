using OakSoft.Model;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class HouseholdService : RepositoryService<Household>, IHouseholdService
    {
        public HouseholdService(IHouseholdRepository repo, ILogger logger) : base(repo, logger)
        {

        }

        public override bool Update(Household item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.Id == item.Id);

                if (repoItem is null)
                {
                    return false;
                }

                repoItem.LocationDetails.Update(item.LocationDetails);
                repoItem.Update(item);
                Repo.Update(repoItem);
                Repo.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(OakSoftCore.LogSeverity.Error, string.Format(Strings.UpdateItemFailed, new object[3] { item.GetType().Name, item.Id, ex.Message }));
                return false;
            }
        }
    }
}
