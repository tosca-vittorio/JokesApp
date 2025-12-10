using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using FluentAssertions;
using JokesApp.Server.Models;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Domain.Entities;

namespace JokesApp.Tests.Models
{
    public class JokeTests
    {
        #region Constructor Tests

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("", typeof(ArgumentException))]
        [InlineData("   ", typeof(ArgumentException))]
        public void Constructor_ShouldThrow_IfAnswerIsInvalid(string? answer, Type exceptionType)
        {
            // Arrange
            string question = "Valid question";
            string userId = "user123";

            // Act
            Action act = () => new Joke(question, answer!, userId);

            // Assert
            act.Should().Throw<Exception>()
               .Where(ex => ex.GetType() == exceptionType)
               .And.Message.Should().Contain(JokeErrorMessages.AnswerNullOrEmpty);
        }


        [Fact]
        public void Constructor_WithCreatedAt_ShouldSetCreatedAtCorrectly()
        {
            // Arrange
            string question = "Sample question";
            string answer = "Sample answer";
            string userId = "user123";
            DateTime customCreatedAt = new DateTime(2025, 12, 4, 12, 0, 0, DateTimeKind.Utc);

            // Act
            var joke = new Joke(question, answer, userId, customCreatedAt);

            // Assert
            joke.Question.Should().Be(question);
            joke.Answer.Should().Be(answer);
            joke.ApplicationUserId.Should().Be(userId);
            joke.CreatedAt.Should().Be(customCreatedAt);
            joke.UpdatedAt.Should().BeNull();
            joke.Author.Should().BeNull();
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            string question = "Why did the chicken cross the road?";
            string answer = "To get to the other side!";
            string userId = "user123";

            // Act
            var joke = new Joke(question, answer, userId);

            // Assert
            joke.Question.Should().Be(question);
            joke.Answer.Should().Be(answer);
            joke.ApplicationUserId.Should().Be(userId);
            joke.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            joke.UpdatedAt.Should().BeNull();
            joke.Author.Should().BeNull();
        }

       
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_IfUserIdIsInvalid(string? userId)
        {
            // Arrange
            string question = "Question";
            string answer = "Answer";

            // Act
            Action act = () => new Joke(question, answer, userId!);

            // Assert
            if (userId is null)
            {
                act.Should().Throw<ArgumentNullException>()
                   .WithParameterName(nameof(userId));
                   
            }
            else
            {
                act.Should().Throw<ArgumentException>()
                   .WithParameterName(nameof(userId))
                   .WithMessage($"*{JokeErrorMessages.UserIdNullOrEmpty}*"); 
            }
        }


        [Fact]
        public void Constructor_ShouldTrimWhitespace()
        {
            var joke = new Joke("  Question  ", "  Answer  ", "  user123  ");

            joke.Question.Should().Be("Question");
            joke.Answer.Should().Be("Answer");
            joke.ApplicationUserId.Should().Be("user123");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_ShouldThrow_IfQuestionIsInvalid(string? question)
        {
            var answer = "Valid answer";
            var userId = "user123";

            Action act = () => new Joke(question!, answer, userId);

            if (question is null)
                act.Should().Throw<ArgumentNullException>()
                   .WithParameterName(nameof(question));
            else
                act.Should().Throw<ArgumentException>()
                   .WithParameterName(nameof(question))
                   .WithMessage($"*{JokeErrorMessages.QuestionNullOrEmpty}*");
        }

        [Fact]
        public void Constructor_ShouldThrow_IfAnswerTooLong()
        {
            var question = "Valid question";
            var answer = new string('A', 501); // 501 caratteri
            var userId = "user123";

            Action act = () => new Joke(question, answer, userId);

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{JokeErrorMessages.AnswerTooLong}*");
        }

        [Fact]
        public void Constructor_ShouldAcceptMaxLengthValues()
        {
            var question = new string('Q', 200);
            var answer = new string('A', 500);

            var joke = new Joke(question, answer, "user123");

            joke.Question.Length.Should().Be(200);
            joke.Answer.Length.Should().Be(500);
        }

        [Fact]
        public void Constructor_ShouldThrow_IfQuestionTooLong()
        {
            var question = new string('Q', 201); // 201 > max 200
            var answer = "Valid answer";
            var userId = "user123";

            Action act = () => new Joke(question, answer, userId);

            act.Should().Throw<ArgumentException>()
                .WithMessage($"*{JokeErrorMessages.QuestionTooLong}*");

        }

        #endregion

        #region Update Method Tests
        [Fact]
        public void Update_ShouldUpdatePropertiesCorrectly()
        {
            // Arrange
            var joke = new Joke("Old question", "Old answer", "user123");
            var originalCreatedAt = joke.CreatedAt;

            // Act
            joke.Update("New question", "New answer");

            // Assert
            joke.Question.Should().Be("New question");
            joke.Answer.Should().Be("New answer");
            joke.UpdatedAt.Should().NotBeNull();
            joke.UpdatedAt.Should().BeAfter(originalCreatedAt);
            joke.CreatedAt.Should().Be(originalCreatedAt); 
        }

        [Fact]
        public void Update_ShouldTrimWhitespace()
        {
            var joke = new Joke("Question", "Answer", "user123");

            joke.Update("  New question  ", "  New answer  ");

            joke.Question.Should().Be("New question");
            joke.Answer.Should().Be("New answer");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_ShouldThrow_IfQuestionIsInvalid(string? question)
        {
            var joke = new Joke("Old question", "Old answer", "user123");

            Action act = () => joke.Update(question!, "New answer");

            if (question is null)
                act.Should().Throw<ArgumentNullException>()
                   .WithParameterName(nameof(question));
            else
                act.Should().Throw<ArgumentException>()
                   .WithParameterName(nameof(question))
                   .WithMessage($"*{JokeErrorMessages.QuestionNullOrEmpty}*");
        }


        [Fact]
        public void Update_ShouldThrow_IfQuestionTooLong()
        {
            var joke = new Joke("Question", "Answer", "user123");
            var newQuestion = new string('Q', 201);

            Action act = () => joke.Update(newQuestion, "Answer");

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{JokeErrorMessages.QuestionTooLong}*");
        }

        [Fact]
        public void Update_ShouldThrow_IfAnswerTooLong()
        {
            var joke = new Joke("Question", "Answer", "user123");
            var newAnswer = new string('A', 501);

            Action act = () => joke.Update("Question", newAnswer);

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{JokeErrorMessages.AnswerTooLong}*");
        }

        [Fact]
        public void Update_ShouldSetUpdatedAtToCurrentTime()
        {
            // Arrange
            var joke = new Joke("Question", "Answer", "user123");
            var beforeUpdate = DateTime.UtcNow;

            // Act
            joke.Update("New question", "New answer");

            // Assert
            joke.UpdatedAt.Should().NotBeNull();
            joke.UpdatedAt!.Value.Should().BeOnOrAfter(beforeUpdate);
            joke.UpdatedAt!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1)); // ✅ margine tolleranza
        }

        [Fact]
        public void MultipleUpdates_ShouldMaintainTimestampsCorrectly()
        {
            var joke = new Joke("Q1", "A1", "user123");
            var firstCreatedAt = joke.CreatedAt;

            for (int i = 0; i < 5; i++)
            {
                joke.Update($"Q{i}", $"A{i}");
                joke.UpdatedAt.Should().BeOnOrAfter(firstCreatedAt);
            }
        }

        [Fact]
        public void Update_ShouldHandleUnicodeCharacters()
        {
            var joke = new Joke("Old Q", "Old A", "user123");
            var unicodeQ = "😃✨💻";
            var unicodeA = "🌍🚀🎉";

            joke.Update(unicodeQ, unicodeA);

            joke.Question.Should().Be(unicodeQ);
            joke.Answer.Should().Be(unicodeA);
        }

        [Fact]
        public void Update_ShouldBeThreadSafe()
        {
            // Questo test verifica che Update non generi eccezioni quando invocato in parallelo.
            // NOTA: Il metodo non è realmente thread-safe a livello di sincronizzazione,
            //       né il dominio richiede tale garanzia. L’obiettivo è solo assicurare robustezza operativa.
            var joke = new Joke("Q", "A", "user123");

            Parallel.For(0, 50, i =>
            {
                joke.Update($"Q{i}", $"A{i}");
            });

            joke.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Update_ShouldThrow_IfAnswerIsInvalid(string? answer)
        {
            var joke = new Joke("Old question", "Old answer", "user123");

            Action act = () => joke.Update("New question", answer!);

            if (answer is null)
                act.Should().Throw<ArgumentNullException>()
                   .WithParameterName(nameof(answer));
            else
                act.Should().Throw<ArgumentException>()
                   .WithParameterName(nameof(answer))
                   .WithMessage($"*{JokeErrorMessages.AnswerNullOrEmpty}*");
        }

        #endregion

        #region SetAuthor Method Tests
        [Fact]
        public void SetAuthor_ShouldNotTrimAuthorId()
        {
            // Arrange
            var joke = new Joke("Question", "Answer", "user123");
            var author = new ApplicationUser { Id = "  user123  " };

            // Act
            Action act = () => joke.SetAuthor(author);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage($"*{JokeErrorMessages.AuthorIdMismatch}*");
        }


        [Fact]
        public void SetAuthor_ShouldSetAuthorAndUserId()
        {
            // Arrange
            var joke = new Joke("Question", "Answer", "user123");
            var author = new ApplicationUser { Id = "user123", UserName = "TestUser" };

            // Act
            joke.SetAuthor(author);

            // Assert
            joke.Author.Should().Be(author);
            joke.ApplicationUserId.Should().Be(author.Id);
        }

        [Fact]
        public void SetAuthor_ShouldThrow_IfAuthorIsNull()
        {
            var joke = new Joke("Question", "Answer", "user123");

            Action act = () => joke.SetAuthor(null!);

            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("author");
        }

        [Fact]
        public void SetAuthor_ShouldThrow_IfAuthorAlreadySet()
        {
            // Arrange
            var joke = new Joke("Question", "Answer", "user123");
            var author1 = new ApplicationUser { Id = "user123", UserName = "User1" };
            var author2 = new ApplicationUser { Id = "user456", UserName = "User2" };

            joke.SetAuthor(author1);

            // Act
            Action act = () => joke.SetAuthor(author2);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*Author already set*");
        }

        [Fact]
        public void SetAuthor_ShouldThrow_IfAuthorIdDoesNotMatchUserId()
        {
            // Arrange
            var joke = new Joke("Question", "Answer", "user123");
            var wrongAuthor = new ApplicationUser { Id = "user456", UserName = "WrongUser" };

            // Act
            Action act = () => joke.SetAuthor(wrongAuthor);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("*Author ID mismatch*");
        }

        [Fact]
        public void SetAuthorThenUpdate_ShouldNotAffectAuthor()
        {
            var joke = new Joke("Question", "Answer", "user123");
            var author = new ApplicationUser { Id = "user123", UserName = "TestUser" };

            joke.SetAuthor(author);

            // Aggiorna joke
            joke.Update("New question", "New answer");

            joke.Author.Should().Be(author); // Author non cambia
            joke.UpdatedAt.Should().NotBeNull();
        }

        #endregion

        #region IsAuthoredBy Method Tests

        [Fact]
        public void IsAuthoredBy_ShouldReturnTrue_IfUserIdMatches()
        {
            var joke = new Joke("Question", "Answer", "user123");

            var result = joke.IsAuthoredBy("user123");

            result.Should().BeTrue();
        }

        [Fact]
        public void IsAuthoredBy_ShouldReturnFalse_IfUserIdDoesNotMatch()
        {
            var joke = new Joke("Question", "Answer", "user123");

            var result = joke.IsAuthoredBy("user456");

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsAuthoredBy_ShouldReturnFalse_IfUserIdIsNullOrEmpty(string? userId)
        {
            var joke = new Joke("Question", "Answer", "user123");

            var result = joke.IsAuthoredBy(userId);

            result.Should().BeFalse();
        }

        [Fact]
        public void IsAuthoredBy_ShouldReturnFalse_IfUserIdIsWhitespace()
        {
            var joke = new Joke("Question", "Answer", "user123");

            var result = joke.IsAuthoredBy("   ");

            result.Should().BeFalse();
        }

        #endregion

        #region Data Annotation Tests

        [Fact]
        public void Joke_ShouldHaveMaxLengthAttributes()
        {
            var type = typeof(Joke);
            var questionProp = type.GetProperty("Question");
            var answerProp = type.GetProperty("Answer");

            var questionMaxLength = (MaxLengthAttribute?)Attribute.GetCustomAttribute(questionProp!, typeof(MaxLengthAttribute));
            var answerMaxLength = (MaxLengthAttribute?)Attribute.GetCustomAttribute(answerProp!, typeof(MaxLengthAttribute));

            questionMaxLength!.Length.Should().Be(200);
            answerMaxLength!.Length.Should().Be(500);
        }

        [Fact]
        public void Joke_ShouldHaveRequiredApplicationUserId()
        {
            var prop = typeof(Joke).GetProperty("ApplicationUserId");
            var requiredAttr = (RequiredAttribute?)Attribute.GetCustomAttribute(prop!, typeof(RequiredAttribute));
            requiredAttr.Should().NotBeNull();
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void CreatedAt_ShouldRemainUnchangedAfterUpdate()
        {
            var joke = new Joke("Question", "Answer", "user123");
            var createdAtBefore = joke.CreatedAt;

            System.Threading.Thread.Sleep(100);
            joke.Update("New question", "New answer");

            joke.CreatedAt.Should().Be(createdAtBefore);
            joke.UpdatedAt.Should().NotBeNull();
        }

        

        #endregion

    }
}