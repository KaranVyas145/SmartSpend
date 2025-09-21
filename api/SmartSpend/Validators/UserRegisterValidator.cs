using FluentValidation;
using SmartSpend.Dtos;

namespace SmartSpend.Validators
{
    public class UserRegisterValidator: AbstractValidator<UserRegisterDto>
    {
        public UserRegisterValidator() {
            RuleFor(x => x.UserName)
               .NotEmpty().WithMessage("Username is required")
               .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage("Passwords do not match");

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format");
        }
    }
}
