using FluentValidation;
using WA.Pizza.Infrastructure.DTO.User;

using static WA.Pizza.Core.ConstantValues;

namespace WA.Pizza.Web.Services.Validators
{
	public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
	{
		public RegisterRequestValidator()
		{
			RuleFor(x => x.UserName).NotEmpty().MaximumLength(Validations.MAXIMUM_USERNAME_CHARACTERS).WithMessage($"Enter a valid username with no more than {Validations.MAXIMUM_USERNAME_CHARACTERS} characters.");
			RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Enter a valid email address.");
			RuleFor(x => x.Password).NotEmpty().MinimumLength(Validations.MINIMUM_PASSWORD_CHARACTERS).WithMessage($"Enter a valid password with at least {Validations.MINIMUM_PASSWORD_CHARACTERS} characters.");
			RuleFor(x => x.ConfirmPassword).NotEmpty().MinimumLength(6).Equal(x => x.Password).WithMessage("Passwords do not match.");
		}
	}
}
