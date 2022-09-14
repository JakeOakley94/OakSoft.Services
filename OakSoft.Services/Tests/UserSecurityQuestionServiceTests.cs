using NUnit.Framework;
using OakSoft.Client.Models;
using OakSoft.Model;
using OakSoft.Repositories;
using OakSoftCore;
using OakSoftCore.Logging;

namespace OakSoft.Services.Tests
{
    //TODO : Create Fake Logger
    [TestFixture]
    public class UserSecurityQuestionServiceTests
    {
        FakeUserSecurityQuestionRepository _fakeRepo;
        UserSecurityQuestionService _dal;

        [SetUp]
        public void Setup()
        {
            _fakeRepo = new FakeUserSecurityQuestionRepository();
            _dal = new UserSecurityQuestionService(_fakeRepo, new CommonLogger());
        }

        [Test]
        public void GetCurrentSecurityQuestionsForUser_ReturnsUsersSecurityQuestionsFromRepo()
        {
            // Act
            var result = _dal.GetCurrentSecurityQuestionsForUser(UnitTestFactory.FakeUserId);

            // Assert
            Assert.IsTrue(result is not null);
            Assert.IsTrue(result?.Count() is 1);
        }

        [Test]
        public void ValidateUserSecurityQuestions_CurrentSecurityQuestionCountInRepoIsZero_ThrowsArgumentException()
        {
            // Arrange
            _fakeRepo.ClearList();
            _dal = new UserSecurityQuestionService(_fakeRepo, new CommonLogger());

            // Act + Assert
            Assert.Throws<ArgumentException>(() => _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, new List<UserSecurityQuestionResponse>()));
        }

        [Test]
        public void ValidateUserSecurityQuestions_ResponsesCountIsZero_ThrowsArgumentException()
        {
            // Act + Assert
            Assert.Throws<ArgumentException>(() => _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, new List<UserSecurityQuestionResponse>()));
        }

        [Test]
        public void ValidateUserSecurityQuestions_CurrentSecurityQuestionsCountInRepoDoesNotEqualResponsesCount_ThrowsArgumentException()
        {
            // Arrange
            var fakeResponse1 = new UserSecurityQuestionResponse();
            var fakeResponse2 = new UserSecurityQuestionResponse();
            var fakeResponses = new List<UserSecurityQuestionResponse>() { fakeResponse1, fakeResponse2 };

            // Act + Assert
            Assert.Throws<ArgumentException>(() => _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, fakeResponses));
        }

        [Test]
        public void ValidateUserSecurityQuestions_CurrentSecurityQuestionsFromRepoIdsDoNotMatchRespsonseIds_ThrowsArgumentException()
        {
            // Arrange
            var fakeResponse1 = new UserSecurityQuestionResponse() { SecurityQuestionId = 99 };
            var fakeResponses = new List<UserSecurityQuestionResponse>() { fakeResponse1 };

            // Act + Assert
            Assert.Throws<ArgumentException>(() => _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, fakeResponses));
        }

        [Test]
        [TestCase("God")]
        [TestCase("GOD")]
        [TestCase("       God")]
        public void ValidateUserSecurityQuestions_AnswersInRepoMatchResponseAnswersWithVariableFormats_ReturnsListOfValidationResultsWithNoFailures(string answer)
        {
            // Arrange
            var firstSecurityQuestion = _fakeRepo._item1;
            var fakeResponse1 = new UserSecurityQuestionResponse() { SecurityQuestionId = firstSecurityQuestion.SecurityQuestionId, Answer = answer };
            var fakeResponses = new List<UserSecurityQuestionResponse>() { fakeResponse1 };

            // Act
            var result = _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, fakeResponses);

            // Assert
            Assert.IsInstanceOf<List<UserSecurityQuestionValidationResult>>(result);
            Assert.IsFalse(result.Any(x => x.FailedValidation is true));
        }

        [Test]
        public void ValidateUserSecurityQuestions_AnswersInRepoDoNotMatchAnswersInResponse_ReturnsListOfValidationResultsWithFailures()
        {
            // Arrange
            var firstSecurityQuestion = _fakeRepo._item1;
            var fakeResponse1 = new UserSecurityQuestionResponse() { SecurityQuestionId = firstSecurityQuestion.SecurityQuestionId, Answer = "This answer will not match." };
            var fakeResponses = new List<UserSecurityQuestionResponse>() { fakeResponse1 };

            //Act
            var result = _dal.ValidateUserSecurityQuestions(UnitTestFactory.FakeUserId, fakeResponses);

            // Assert
            Assert.IsInstanceOf<List<UserSecurityQuestionValidationResult>>(result);
            Assert.IsTrue(result.All(x => x.FailedValidation is true));
        }

        [Test]
        public void Create_WhenCalled_AddsItemToRepository()
        {
            // Arrange
            var fakeUserSecurityQuestion = new UserSecurityQuestion()
            {
                SecurityQuestionId = (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteAnimal,
                Answer = "Cow",
                UserId = UnitTestFactory.FakeUserId,
            };

            // Act
            _dal.Create(fakeUserSecurityQuestion);

            // Assert
            Assert.AreEqual(2, _fakeRepo._list.Count());
        }

        [Test]
        public void Create_WhenCalled_ReturnsUserSecurityQuestion()
        {
            var fakeUserSecurityQuestion = new UserSecurityQuestion()
            {
                SecurityQuestionId = (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteAnimal,
                Answer = "Cow",
                UserId = UnitTestFactory.FakeUserId,
            };

            // Act
            _dal.Create(fakeUserSecurityQuestion);

            // Assert
            Assert.IsInstanceOf<UserSecurityQuestion>(_dal.Create(fakeUserSecurityQuestion));
        }

        [Test]
        public void Create_ThrowsException_ReturnsFalse()
        {
            var fakeUserSecurityQuestion = new UserSecurityQuestion()
            {
                SecurityQuestionId = (int)OakSoftCore.SecurityQuestion.WhatIsYourFavoriteAnimal,
                Answer = "Cow",
                UserId = UnitTestFactory.FakeUserId,
            };

            _fakeRepo = new FakeUserSecurityQuestionRepository();
            _fakeRepo.throwException = true;
            _dal = new UserSecurityQuestionService(_fakeRepo, new CommonLogger());

            // Act
            var result = _dal.Create(fakeUserSecurityQuestion);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void Update_WhenCalled_UpdatesItemInRepository()
        {
            // Arrange
            var firstItem = _fakeRepo._item1;
            firstItem.Answer = UnitTestFactory.UpdateString;

            // Act
            _dal.Update(firstItem);

            // Assert
            Assert.AreEqual(UnitTestFactory.UpdateString, _fakeRepo._item1.Answer);
        }

        [Test]
        public void Update_NoMatchingItemInRepository_ReturnsFalse()
        {
            // Arrange
            var firstItem = _fakeRepo._item1.DeepCopyJson();
            firstItem.UserId = Guid.NewGuid();

            //Act 
            var result = _dal.Update(firstItem);

            // Assert
            Assert.IsTrue(result is false);
        }

    }
}
