using FluentValidation;
using ZiyoNur.Service.Features.Auth.Commands;

namespace ZiyoNur.Service.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon raqam majburiy")
            .Matches(@"^\+998\d{9}$").WithMessage("Telefon raqam formati: +998XXXXXXXXX");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol majburiy")
            .MinimumLength(6).WithMessage("Parol kamida 6 ta belgi bo'lishi kerak");

        RuleFor(x => x.UserType)
            .Must(x => new[] { "customer", "seller", "admin" }.Contains(x.ToLower()))
            .WithMessage("Foydalanuvchi turi: customer, seller, admin");
    }
}
