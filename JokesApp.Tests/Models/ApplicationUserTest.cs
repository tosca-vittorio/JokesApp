using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using JokesApp.Server.Domain.Errors;
using JokesApp.Server.Models;
using Xunit;

namespace JokesApp.Tests.Models
{
    public class ApplicationUserTests
    {
        #region DisplayName Tests

        [Fact]
        public void DisplayName_ShouldFail_WhenExceedsMaxLength()
        {
            var longName = new string('A', 51);
            var user = new ApplicationUser { Email = "user@example.com" };

            Action act = () => user.UpdateProfile(longName);

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{ApplicationUserErrorMessages.DisplayNameMaxLength}*");
        }

        [Fact]
        public void DisplayName_ShouldTrimWhitespaceAutomatically()
        {
            var user = new ApplicationUser
            {
                Email = "user@example.com",
                UserName = "mario.rossi"
            };

            user.UpdateProfile("   Mario Rossi   ");

            user.DisplayName.Should().Be("Mario Rossi");
        }

        [Fact]
        public void DisplayName_ShouldFail_WhenEmpty()
        {
            var user = new ApplicationUser
            {
                Email = "user@example.com",
                UserName = "mario.rossi"
            };

            Action act = () => user.UpdateProfile("");

            act.Should().Throw<ArgumentException>()
                .WithMessage("*DisplayName is required*");
        }



        #endregion


        #region AvatarUrl Tests

        [Fact]
        public void AvatarUrl_ShouldFail_WhenExceedsMaxLength()
        {
            var longUrl = "https://example.com/" + new string('a', 2040);
            var user = new ApplicationUser { Email = "user@example.com" };

            Action act = () => user.UpdateProfile("Test User", longUrl);

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{ApplicationUserErrorMessages.AvatarUrlMaxLength}*");
        }

        [Fact]
        public void AvatarUrl_ShouldAllowNull()
        {
            var user = new ApplicationUser { Email = "user@example.com" };

            user.UpdateProfile("Test User", null);

            user.AvatarUrl.Should().BeNull();
        }

        [Theory]
        [InlineData("https://example.com/avatar.png")]
        [InlineData("http://example.com/avatar.png")]
        public void AvatarUrl_ShouldValidateUrlSchemes(string url)
        {
            var user = new ApplicationUser
            {
                Email = "user@example.com",
                UserName = "user123"
            };

            user.UpdateProfile("Test User", url);

            user.AvatarUrl.Should().Be(url);
        }

        #endregion


        #region Timestamp Tests

        [Fact]
        public void CreatedAt_ShouldBeInUtc()
        {
            var user = new ApplicationUser();
            user.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void UpdatedAt_ShouldBeInUtc_WhenSet()
        {
            var user = new ApplicationUser();
            user.UpdateProfile("New Name");

            user.UpdatedAt!.Value.Kind.Should().Be(DateTimeKind.Utc);
        }

        [Fact]
        public void UpdatedAt_ShouldUpdate_WhenProfileChanges()
        {
            var user = new ApplicationUser { Email = "user@example.com" };

            user.UpdateProfile("Updated Name");
            var firstUpdate = user.UpdatedAt!.Value;

            System.Threading.Thread.Sleep(10);

            user.UpdateProfile("Updated Again");
            var secondUpdate = user.UpdatedAt!.Value;

            secondUpdate.Should().BeAfter(firstUpdate);
            user.CreatedAt.Should().BeBefore(secondUpdate);
        }

        #endregion


        #region Jokes Tests

        [Fact]
        public void Jokes_ShouldAllowAddingAndRemovingJokes()
        {
            var user = new ApplicationUser();
            var joke = new Joke("Q", "A", "userId");

            user.Jokes.Add(joke);
            user.Jokes.Should().ContainSingle();

            user.Jokes.Remove(joke);
            user.Jokes.Should().BeEmpty();
        }

        [Fact]
        public void Jokes_RemovingFromEmptyList_ShouldNotThrow()
        {
            var user = new ApplicationUser();
            var joke = new Joke("Q", "A", "userId");

            Action act = () => user.Jokes.Remove(joke);
            act.Should().NotThrow();
        }

        [Fact]
        public void Jokes_ShouldAllowMultipleAddAndRemove()
        {
            var user = new ApplicationUser();
            var joke1 = new Joke("Q1", "A1", "user1");
            var joke2 = new Joke("Q2", "A2", "user1");
            var joke3 = new Joke("Q3", "A3", "user1");

            user.Jokes.Add(joke1);
            user.Jokes.Add(joke2);
            user.Jokes.Add(joke3);

            user.Jokes.Should().HaveCount(3);

            user.Jokes.Remove(joke2);

            user.Jokes.Should().HaveCount(2);
            user.Jokes.Should().Contain(joke1);
            user.Jokes.Should().Contain(joke3);
        }

        [Fact]
        public void Jokes_Collection_ShouldBeIndependentPerUser()
        {
            var user1 = new ApplicationUser();
            var user2 = new ApplicationUser();
            var joke1 = new Joke("Q1", "A1", "user1");
            var joke2 = new Joke("Q2", "A2", "user2");

            user1.Jokes.Add(joke1);
            user2.Jokes.Add(joke2);

            user1.Jokes.Should().ContainSingle().Which.Should().Be(joke1);
            user2.Jokes.Should().ContainSingle().Which.Should().Be(joke2);
        }

        #endregion


        #region Email Tests

        [Fact]
        public void Email_ShouldPass_WhenLongButValid()
        {
            var localPart = new string('a', 200);
            var email = $"{localPart}@domain.com";

            var user = new ApplicationUser { Email = email };

            user.Email.Should().Be(email);
        }

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("first.last@domain.co")]
        [InlineData("user+tag@domain.io")]
        public void Email_ShouldPassWithValidValues(string email)
        {
            var user = new ApplicationUser { Email = email };
            user.Email.Should().Be(email);
        }

        [Fact]
        public void Email_ShouldFail_WhenEmpty()
        {
            Action act = () => new ApplicationUser { Email = "" };

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{ApplicationUserErrorMessages.EmailInvalid}*");
        }

        [Theory]
        [InlineData("not-an-email")]
        [InlineData("user@@domain.com")]
        [InlineData("john.doe@")]
        public void Email_ShouldFail_WhenFormatIsInvalid(string email)
        {
            Action act = () => new ApplicationUser { Email = email };

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{ApplicationUserErrorMessages.EmailInvalid}*");
        }

        [Fact]
        public void Email_ShouldFail_WhenExceedsMaxLength()
        {
            var longEmail = new string('a', 300) + "@domain.com";

            Action act = () => new ApplicationUser { Email = longEmail };

            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("john doe@example.com")]
        [InlineData(" user @example.com ")]
        [InlineData("john.doe@ example.com")]
        public void Email_ShouldFail_WhenContainsSpacesInside(string email)
        {
            Action act = () => new ApplicationUser { Email = email };

            act.Should().Throw<ArgumentException>()
               .WithMessage($"*{ApplicationUserErrorMessages.EmailInvalid}*");
        }

        #endregion


        #region Initialization Tests

        [Fact]
        public void ApplicationUser_ShouldInitializeCorrectly()
        {
            var user = new ApplicationUser();

            user.DisplayName.Should().BeEmpty();
            user.AvatarUrl.Should().BeNull();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(50));
            user.UpdatedAt.Should().BeNull();
            user.Jokes.Should().BeEmpty();
        }

        #endregion
    }
}
