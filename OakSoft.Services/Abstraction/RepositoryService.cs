using OakSoftCore.DataAccess.EntityFramework;
using OakSoftCore.Logging;

namespace OakSoft.Services
{
    public abstract class RepositoryService<T> : IRepositoryService<T> where T : EntityBase<T>, new()
    {
        private IRepositoryBase<T> _repo;
        public IRepositoryBase<T> Repo
        {
            get => _repo;
            private set => _repo = value;
        }

        private ILogger _logger;
        public ILogger Logger
        {
            get => _logger;
            set => _logger = value;
        }

        public RepositoryService(IRepositoryBase<T> repo, ILogger logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public List<T> GetActive() => Repo.GetAll().Where(x => x.IsActive is true).ToList();

        public T GetSingle(Guid id) => Repo.GetSingle(x => x.Id == id);

        public virtual bool Create(T item)
        {
            try
            {
                Repo.Add(item);
                Repo.Commit();
                return true;
            }
            catch (Exception ex)
            {  //TODO: LOG
                var x = ex;
                return false;
            }
        }

        public abstract bool Update(T item);

        public virtual bool Deactivate(Guid id)
        {
            try
            {
                var item = Repo.GetSingle(x => x.Id == id);
                if (item is null) return false;
                Repo.Delete(item);
                return true;
            }
            catch (Exception)
            {
                //TODO: LOG
                return false;
            }
        }

    }
}
