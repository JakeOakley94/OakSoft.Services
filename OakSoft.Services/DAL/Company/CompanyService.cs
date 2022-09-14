using OakSoft.Model;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class CompanyService : RepositoryService<Company>, ICompanyService
    {
        public CompanyService(ICompanyRepository repo, ILogger logger) : base(repo, logger)
        {

        }

        public override bool Update(Company item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.Id == item.Id);

                if (repoItem is null)
                {
                    return false;
                }

                repoItem.Update(item);
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
