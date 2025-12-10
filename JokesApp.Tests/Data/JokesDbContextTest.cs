/*
using Xunit;
using FluentAssertions;
using JokesApp.Server.Models;
using JokesApp.Tests.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using JokesApp.Server.Domain.Errors;

namespace JokesApp.Tests.Data
{
    public class JokesDbContextTests
    {
        #region CRUD Base

        [Fact]
        public async Task SaveJoke_ShouldPersistToDatabase()
        {
            var user = new ApplicationUser { UserName = "testuser", Email = "test@test.com" };
            var joke = new Joke("Domanda?", "Risposta!", user.Id);
            joke.SetAuthor(user);

            using (var context = SetupDbContext.CreateContext())
            {
                context.Users.Add(user);
                context.Jokes.Add(joke);
                await context.SaveChangesAsync();
            }

            using (var context = SetupDbContext.CreateContext())
            {
                var savedJoke = await context.Jokes.Include(j => j.Author).FirstOrDefaultAsync();
                savedJoke.Should().NotBeNull();
                savedJoke!.Question.Should().Be("Domanda?");
                savedJoke.Author.Should().NotBeNull();
                savedJoke.Author!.UserName.Should().Be("testuser");
            }
        }

        [Fact]
        public async Task GetJoke_ShouldReturnCorrectData()
        {
            var user = new ApplicationUser { UserName = "reader", Email = "reader@test.com" };
            var joke = new Joke("Q?", "A!", user.Id);
            joke.SetAuthor(user);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            context.Jokes.Add(joke);
            await context.SaveChangesAsync();

            using var context2 = SetupDbContext.CreateContext();
            var fetched = await context2.Jokes.Include(j => j.Author).FirstOrDefaultAsync(j => j.Id == joke.Id);
            fetched.Should().NotBeNull();
            fetched!.Question.Should().Be("Q?");
            fetched.Answer.Should().Be("A!");
            fetched.Author!.UserName.Should().Be("reader");
        }

        [Fact]
        public async Task UpdateJoke_ShouldModifyFieldsAndUpdatedAt()
        {
            var user = new ApplicationUser { UserName = "updater", Email = "updater@test.com" };
            var joke = new Joke("Old?", "Old!", user.Id);
            joke.SetAuthor(user);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            context.Jokes.Add(joke);
            await context.SaveChangesAsync();

            using var context2 = SetupDbContext.CreateContext();
            var j = await context2.Jokes.Include(j => j.Author).FirstAsync();
            j.Update("New?", "New!");
            await context2.SaveChangesAsync();

            j.Question.Should().Be("New?");
            j.Answer.Should().Be("New!");
            j.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteJoke_ShouldRemoveFromDatabase()
        {
            var user = new ApplicationUser { UserName = "deleter", Email = "del@test.com" };
            var joke = new Joke("Del?", "Del!", user.Id);
            joke.SetAuthor(user);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            context.Jokes.Add(joke);
            await context.SaveChangesAsync();

            using var context2 = SetupDbContext.CreateContext();
            var j = await context2.Jokes.FirstAsync();
            context2.Jokes.Remove(j);
            await context2.SaveChangesAsync();

            (await context2.Jokes.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task SaveUser_ShouldPersistToDatabase()
        {
            var user = new ApplicationUser { UserName = "newuser", Email = "new@test.com" };
            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var fetched = await context.Users.FirstOrDefaultAsync(u => u.UserName == "newuser");
            fetched.Should().NotBeNull();
            fetched!.Email.Should().Be("new@test.com");
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyFieldsAndUpdatedAt()
        {
            var user = new ApplicationUser { UserName = "modifyuser", Email = "old@test.com" };
            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Modifica i campi
            user.UpdateProfile("New Name");
            user.Email = "new@test.com";

            // Aggiorna il contesto EF per triggerare UpdatedAt
            context.Users.Update(user);
            await context.SaveChangesAsync();  // ora UpdatedAt sarà valorizzato

            user.UpdatedAt.Should().NotBeNull();
            user.DisplayName.Should().Be("New Name");
            user.Email.Should().Be("new@test.com");
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveFromDatabase()
        {
            var user = new ApplicationUser { UserName = "tobedeleted", Email = "delete@test.com" };
            var joke = new Joke("QDel", "ADel", user.Id);
            joke.SetAuthor(user);
            user.Jokes.Add(joke);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            (await context.Users.CountAsync()).Should().Be(0);
            (await context.Jokes.CountAsync()).Should().Be(0); // controlla anche i joke
        }

        #endregion

        #region Relazioni & Cascade

        [Fact]
        public async Task CascadeDelete_ShouldDeleteJokes_WhenUserIsDeleted()
        {
            var user = new ApplicationUser { UserName = "todelete", Email = "del@test.com" };
            var joke = new Joke("Domanda", "Risposta", user.Id);
            joke.SetAuthor(user);
            user.Jokes.Add(joke);

            using (var context = SetupDbContext.CreateContext())
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = SetupDbContext.CreateContext())
            {
                var userToDelete = await context.Users.FirstAsync(u => u.UserName == "todelete");
                context.Users.Remove(userToDelete);
                await context.SaveChangesAsync();
            }

            using (var context = SetupDbContext.CreateContext())
            {
                (await context.Users.CountAsync()).Should().Be(0);
                (await context.Jokes.CountAsync()).Should().Be(0);
            }
        }

        [Fact]
        public async Task UserWithMultipleJokes_ShouldHaveCorrectNavigation()
        {
            var user = new ApplicationUser { UserName = "multi", Email = "multi@test.com" };
            var joke1 = new Joke("Q1", "A1", user.Id);
            var joke2 = new Joke("Q2", "A2", user.Id);
            joke1.SetAuthor(user);
            joke2.SetAuthor(user);
            user.Jokes.Add(joke1);
            user.Jokes.Add(joke2);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var fetched = await context.Users.Include(u => u.Jokes).FirstAsync();
            fetched.Jokes.Count.Should().Be(2);
        }

        [Fact]
        public async Task Joke_ShouldHaveCorrectAuthorNavigation()
        {
            var user = new ApplicationUser { UserName = "authorcheck", Email = "author@test.com" };
            var joke = new Joke("Q?", "A?", user.Id);
            joke.SetAuthor(user);

            using var context = SetupDbContext.CreateContext();
            context.Users.Add(user);
            context.Jokes.Add(joke);
            await context.SaveChangesAsync();

            var fetched = await context.Jokes.Include(j => j.Author).FirstAsync();
            fetched.Author!.UserName.Should().Be("authorcheck");
        }

        #endregion

        #region Validazioni & Constraint

        [Fact]
        public void SaveJoke_WithNullQuestion_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Joke(null!, "A", "user1"));
            ex.ParamName.Should().Be("question");
        }

        [Fact]
        public void SaveJoke_WithTooLongQuestion_ShouldThrowException()
        {
            var longQuestion = new string('x', Joke.MaxQuestionLength + 1);
            var ex = Assert.Throws<ArgumentException>(() => new Joke(longQuestion, "A", "user1"));
            ex.ParamName.Should().Be("question");
        }

        [Fact]
        public void SaveJoke_WithNullAnswer_ShouldThrowException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new Joke("Q", null!, "user1"));
            ex.ParamName.Should().Be("answer");
        }

        [Fact]
        public void SaveJoke_WithTooLongAnswer_ShouldThrowException()
        {
            var longAnswer = new string('x', Joke.MaxAnswerLength + 1);
            var ex = Assert.Throws<ArgumentException>(() => new Joke("Q", longAnswer, "user1"));
            ex.ParamName.Should().Be("answer");
        }

        [Fact]
        public void SetAuthor_ShouldSetAuthorCorrectly()
        {
            var user = new ApplicationUser { Id = "u1", UserName = "author" };
            var joke = new Joke("Q", "A", user.Id);
            joke.SetAuthor(user);
            joke.Author.Should().Be(user);
            joke.ApplicationUserId.Should().Be(user.Id);
        }

        [Fact]
        public void SetAuthor_ShouldThrowIfAlreadySet()
        {
            var user = new ApplicationUser { Id = "u1" };
            var joke = new Joke("Q", "A", user.Id);
            joke.SetAuthor(user);
            var ex = Assert.Throws<InvalidOperationException>(() => joke.SetAuthor(user));
            ex.Message.Should().Be(JokeErrorMessages.AuthorAlreadySet);
        }

        [Fact]
        public void UpdateJoke_ShouldThrowOnInvalidData()
        {
            var user = new ApplicationUser { Id = "u1" };
            var joke = new Joke("Q", "A", user.Id);
            joke.SetAuthor(user);

            Assert.Throws<ArgumentNullException>(() => joke.Update(null!, "A"));
            Assert.Throws<ArgumentNullException>(() => joke.Update("Q", null!));
            Assert.Throws<ArgumentException>(() => joke.Update(new string('x', Joke.MaxQuestionLength + 1), "A"));
            Assert.Throws<ArgumentException>(() => joke.Update("Q", new string('x', Joke.MaxAnswerLength + 1)));
        }

        #endregion

        #region Metodi Custom & Logica di Dominio

        [Fact]
        public void IsAuthoredBy_ShouldReturnTrueForCorrectUserId()
        {
            var user = new ApplicationUser { Id = "u1" };
            var joke = new Joke("Q", "A", user.Id);
            joke.SetAuthor(user);

            joke.IsAuthoredBy("u1").Should().BeTrue();
        }

        [Fact]
        public void IsAuthoredBy_ShouldReturnFalseForWrongOrNullUserId()
        {
            var user = new ApplicationUser { Id = "u1" };
            var joke = new Joke("Q", "A", user.Id);
            joke.SetAuthor(user);

            joke.IsAuthoredBy("wrong").Should().BeFalse();
            joke.IsAuthoredBy(null).Should().BeFalse();
            joke.IsAuthoredBy(string.Empty).Should().BeFalse();
        }

        #endregion

        #region Timestamp & Integrità Dati

        [Fact]
        public void CreatedAt_ShouldBeSetOnCreation()
        {
            var joke = new Joke("Q", "A", "u1");
            joke.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void UpdatedAt_ShouldBeNullOnCreation()
        {
            var joke = new Joke("Q", "A", "u1");
            joke.UpdatedAt.Should().BeNull();
        }

        [Fact]
        public void UpdatedAt_ShouldChangeOnUpdate()
        {
            var joke = new Joke("Q", "A", "u1");
            joke.Update("Q2", "A2");
            joke.UpdatedAt.Should().NotBeNull();
        }

        #endregion
    }
}

*/