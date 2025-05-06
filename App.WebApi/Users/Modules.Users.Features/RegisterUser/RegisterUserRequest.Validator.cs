using FluentValidation;


namespace Modules.Users.Features.RegisterUser
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Email or username cannot be empty.")
                .EmailAddress().WithMessage("Invalid email or username format.");

            RuleFor(user => user.Name)
                .NotEmpty().WithMessage("Name cannot be empty.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
                .Matches("^[a-zA-Z\\s]*$").WithMessage("Name can only contain letters and spaces.");

            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(6).WithMessage("Password is too weak. Minimum length is 6 characters.");

            RuleFor(user => user.BirthDate)
                .NotEmpty().WithMessage("Birth date cannot be empty.")
                .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("Birth date cannot be in the future.");
        }
    }
}
