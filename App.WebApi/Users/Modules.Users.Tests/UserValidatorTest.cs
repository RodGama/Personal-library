using Modules.Users.Domain;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Results;
using Modules.Users.Features.RegisterUser;

namespace Modules.Users.Tests
{
    public class RegisterUserRequestValidatorTest
    {
        private readonly RegisterUserRequestValidator _validator;

        public RegisterUserRequestValidatorTest()
        {
            _validator = new RegisterUserRequestValidator();
        }
        
        [Fact]
        public void ShouldValidateValidUser()
        {
            var user = new RegisterUserRequest("user@example.com", "John Doe", "password123",new DateOnly(1990, 1, 1));

            FluentValidation.Results.ValidationResult result = _validator.Validate(user);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void ShouldInvalidateUserWithEmptyName()
        {
            var user = new RegisterUserRequest("user@example.com", "", "password123", new DateOnly(1990, 1, 1));

            FluentValidation.Results.ValidationResult result = _validator.Validate(user);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Name cannot be empty.");
        }

        [Fact]
        public void ShouldInvalidateUserWithShortPassword()
        {
            var user = new RegisterUserRequest("user@example.com", "John Doe", "123", new DateOnly(1990, 1, 1));

            FluentValidation.Results.ValidationResult result = _validator.Validate(user);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Password is too weak. Minimum length is 6 characters.");
        }

        [Fact]
        public void ShouldInvalidateUserWithFutureBirthDate()
        {
            var user = new RegisterUserRequest("user@example.com", "John Doe", "password123", new DateOnly(DateTime.Now.AddDays(1).Year, DateTime.Now.AddDays(1).Month, DateTime.Now.AddDays(1).Day));

            FluentValidation.Results.ValidationResult result = _validator.Validate(user);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Birth date cannot be in the future.");
        }
    }
}
