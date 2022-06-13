using FluentValidation;
using WA.Pizza.Infrastructure.DTO.User;

using static WA.Pizza.Core.ConstantValues;

namespace WA.Pizza.Web.Services.Validators
{
	public class LoginRequestValidator : AbstractValidator<LoginRequest>
	{
		public LoginRequestValidator()
		{
			RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Enter a valid email address.");
			RuleFor(x => x.Password).NotEmpty().MinimumLength(Validations.MINIMUM_PASSWORD_CHARACTERS).WithMessage($"Enter a valid password with at least {Validations.MINIMUM_PASSWORD_CHARACTERS} characters.");
		}
	}
}
