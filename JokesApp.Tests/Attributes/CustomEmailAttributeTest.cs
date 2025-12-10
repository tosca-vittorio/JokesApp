using System;
using System.ComponentModel.DataAnnotations;
using JokesApp.Server.Domain.Attributes;
using Xunit;

namespace JokesApp.Tests.Attributes
{
    public class CustomEmailAttributeTests
    {
        private readonly CustomEmailAttribute _attribute;

        public CustomEmailAttributeTests()
        {
            _attribute = new CustomEmailAttribute();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void IsValid_ShouldReturnSuccess_WhenValueIsNullOrWhitespace(string? input)
        {
            var result = _attribute.GetValidationResult(input, new ValidationContext(new object()));
            Assert.Equal(ValidationResult.Success, result);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name+tag@sub.domain.com")]
        [InlineData("user_name-123@example.co.uk")]
        [InlineData("  test@example.com  ")]            // whitespace estremo
        [InlineData("user@domain.corporate")]          // TLD lungo
        [InlineData("user@sub-domain.example.com")]    // dominio con trattino
        public void IsValid_ShouldReturnSuccess_WhenEmailIsValid(string email)
        {
            var result = _attribute.GetValidationResult(email, new ValidationContext(new object()));
            Assert.Equal(ValidationResult.Success, result);
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missing@domain")]
        [InlineData("missingdomain.com")]
        [InlineData("user@.com")]
        [InlineData("@example.com")]
        [InlineData("user@domain..com")]
        [InlineData("user@-domain.com")]       // dominio inizia con trattino → invalido
        [InlineData("user@domain.c")]          // TLD troppo corto → invalido
        [InlineData("mario.rossi@città.com")]  // dominio con accento → invalido
        [InlineData("user@emoji😊.com")]       // dominio con emoji → invalido
        public void IsValid_ShouldReturnValidationError_WhenEmailIsInvalid(string email)
        {
            var result = _attribute.GetValidationResult(email, new ValidationContext(new object()));
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Equal("L'indirizzo e-mail non è valido.", result!.ErrorMessage);
        }

        [Fact]
        public void IsValid_ShouldUseCustomErrorMessage_IfProvided()
        {
            var customMessage = "Email non valida!";
            var attributeWithMessage = new CustomEmailAttribute { ErrorMessage = customMessage };
            var result = attributeWithMessage.GetValidationResult("invalid-email", new ValidationContext(new object()));
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Equal(customMessage, result!.ErrorMessage);
        }
    }
}
