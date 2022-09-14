using OakSoft.Repositories;
using OakSoftCore.Logging;

namespace OakSoft.Services
{
    public class LifeManagerDAL
    {
        private ILogger _logger;

        public ICompanyService CompanyDAL => new CompanyService(new CompanyRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public IDocumentService DocumentDAL => new DocumentService(new DocumentRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public IHouseholdService HouseholdDAL => new HouseholdService(new HouseholdRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public IProfessionalService ProfessionalDAL => new ProfessionalService(new ProfessionalRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public ITimeIntervalService TimeIntervalDAL => new TimeIntervalService(new TimeIntervalRepository(ContextFactory.GetLifeManagerContext()), _logger);

        public IUserService UserDAL
        {
            get
            {
                var userRepository = new UserRepository(ContextFactory.GetLifeManagerContext());
                return new UserService(userRepository, new UserActionRecordService(new UserActionRecordRepository(ContextFactory.GetLifeManagerContext()), userRepository, _logger), _logger);
            }
        }

        public IUserSecurityQuestionService UserSecurityQuestionDAL => new UserSecurityQuestionService(new UserSecurityQuestionRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public IUserOptionsService UserOptionsDAL => new UserOptionsService(new UserOptionsRepository(ContextFactory.GetLifeManagerContext()), _logger);
        public IUserActionRecordService UserActionRecordDAL => new UserActionRecordService(new UserActionRecordRepository(ContextFactory.GetLifeManagerContext()), new UserRepository(ContextFactory.GetLifeManagerContext()), _logger);

        public LifeManagerDAL(ILogger logger)
        {
            _logger = logger;
        }
    }
}
