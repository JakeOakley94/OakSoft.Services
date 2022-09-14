using OakSoft.Client.Models;
using OakSoft.Model;
using OakSoftCore;
using OakSoftCore.Localization;
using OakSoftCore.Logging;
using static OakSoft.Repositories.IRepositories;

namespace OakSoft.Services
{
    public class UserSecurityQuestionService : RepositoryService<UserSecurityQuestion>, IUserSecurityQuestionService
    {
        private readonly string _defaultAnswer = "Please Enter Your Answer.";

        public UserSecurityQuestionService(IUserSecurityQuestionRepository repo, ILogger logger) : base(repo, logger)
        {

        }

        public List<UserSecurityQuestion> GetCurrentSecurityQuestionsForUser(Guid userId)
        {
            return Repo.FindBy(x => x.UserId == userId).ToList();
        }

        public List<UserSecurityQuestion> CreateNewUsersSecurityQuestions(Guid userId)
        {
            return new List<UserSecurityQuestion>()
            {
                new UserSecurityQuestion(userId, (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteAnimal, _defaultAnswer),
                new UserSecurityQuestion(userId, (int)OakSoftCore.SecurityQuestion.WhatWasTheNameOfYourFirstSchool, _defaultAnswer),
            };
        }

        public List<UserSecurityQuestionValidationResult> ValidateUserSecurityQuestions(Guid userId, List<UserSecurityQuestionResponse> responses)
        {
            var results = new List<UserSecurityQuestionValidationResult>();
            var currentSecurityQuestions = GetCurrentSecurityQuestionsForUser(userId);

            try
            {
                if (currentSecurityQuestions.Count() is 0)
                {
                    throw new ArgumentException("The specified userId has no security questions.", nameof(userId));
                }

                if (responses.Count() is 0)
                {
                    throw new ArgumentException("The specified responses count is zero.", nameof(responses));
                }

                if (responses.Count() != currentSecurityQuestions.Count())
                {
                    throw new ArgumentException("The number of responses, is not equal to the number of current security questions for the user.", nameof(responses));
                }

                if (responses.Any(x => x.Answer.Equals(_defaultAnswer, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new ArgumentException("Please use an answer other than the default answer.");
                }

                var responseQuestions = responses.Select(x => x.SecurityQuestionId).OrderBy(x => x);
                var currentQuestions = currentSecurityQuestions.Select(x => x.SecurityQuestionId).OrderBy(x => x);

                if (responseQuestions.SequenceEqual(currentQuestions) is false)
                {
                    throw new ArgumentException("The questions listed in the responses list, do not match the values of the current questions. Data validation failure.", nameof(responses));
                }

                foreach (var securityQuestion in currentSecurityQuestions)
                {
                    var result = new UserSecurityQuestionValidationResult();
                    result.SecurityQuestionId = securityQuestion.SecurityQuestionId;
                    result.FailedValidation = responses.Single(x => x.SecurityQuestionId == securityQuestion.SecurityQuestionId).Answer.Trim().Equals(securityQuestion.Answer.Trim(), StringComparison.OrdinalIgnoreCase) is false;
                    results.Add(result);
                }

                return results;
            }
            catch (Exception ex)
            {
                Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.GeneralException, new object[2] { ex.TargetSite, ex.Message }));
                return null;
            }
        }

        public override bool Update(UserSecurityQuestion item)
        {
            try
            {
                var repoItem = Repo.GetSingle(x => x.UserId == item.UserId && x.SecurityQuestionId == item.SecurityQuestionId);
                if (repoItem is null)
                {
                    Logger.WriteToLog(LogSeverity.Error, string.Format(Strings.DALNoMatchingIdentifierInRepositoryFromQuery, new object[2] { item.GetType().Name, $"UserId: {item.UserId}, SecurityQuestionId: {item.SecurityQuestionId}" }));
                    return false;
                }
                repoItem.SecurityQuestionId = item.SecurityQuestionId;
                repoItem.Answer = item.Answer;
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
