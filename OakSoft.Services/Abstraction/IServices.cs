using OakSoft.Client.Models;
using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.DataAccess.EntityFramework;

namespace OakSoft.Services
{
    public interface IRepositoryService<T> where T : EntityBase<T>, new()
    {
        List<T> GetActive();
        T GetSingle(Guid id);
        bool Create(T item);
        abstract bool Update(T item);
        bool Deactivate(Guid id);
    }

    public interface ICompanyService : IRepositoryService<Company>
    {

    }

    public interface IDocumentService : IRepositoryService<Document>
    {

    }

    public interface IHouseholdService : IRepositoryService<Household>
    {

    }

    public interface IProfessionalService : IRepositoryService<Professional>
    {

    }

    public interface ISecurityQuestionService : IRepositoryService<OakSoft.Model.SecurityQuestion>
    {

    }

    public interface ITimeIntervalService : IRepositoryService<TimeInterval>
    {

    }

    public interface IUserSecurityQuestionService : IRepositoryService<UserSecurityQuestion>
    {
        List<UserSecurityQuestion> GetCurrentSecurityQuestionsForUser(Guid userId);
        List<UserSecurityQuestion> CreateNewUsersSecurityQuestions(Guid userId);
        List<UserSecurityQuestionValidationResult> ValidateUserSecurityQuestions(Guid userId, List<UserSecurityQuestionResponse> responses);
    }

    public interface IUserOptionsService : IRepositoryService<UserOptions>
    {

    }

    public interface IUserService : IRepositoryService<User>
    {
        User GetByUsername(string username);
        User Authenticate(string username, string password);
        bool ResetPassword(Guid userId, string newPassword);
    }

    public interface IUserActionRecordService : IRepositoryService<UserActionRecord>
    {
        bool CreateUserActionRecord(UserActionRecordType type, Guid userId, DateTime attemptTime);
    }
}
