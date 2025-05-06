using FluentValidation;
using Modules.Users.Domain;
using System;


namespace Modules.Users.Features.UpdateUser
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Password cannot be empty.")
                .MinimumLength(6).WithMessage("Password is too weak. Minimum length is 6 characters.");
        }
    }
}
