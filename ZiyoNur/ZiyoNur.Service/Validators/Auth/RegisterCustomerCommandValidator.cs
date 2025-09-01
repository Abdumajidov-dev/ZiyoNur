using FluentValidation;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Features.Auth.Commands;

namespace ZiyoNur.Service.Validators.Auth;

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    private readonly ICustomerRepository _customerRepository;

    public RegisterCustomerCommandValidator(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ism majburiy")
            .MaximumLength(100).WithMessage("Ism 100 ta belgidan ko'p bo'lmasligi kerak");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Familya majburiy")
            .MaximumLength(100).WithMessage("Familya 100 ta belgidan ko'p bo'lmasligi kerak");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon raqam majburiy")
            .Matches(@"^\+998\d{9}$").WithMessage("Telefon raqam formati: +998XXXXXXXXX")
            .MustAsync(BeUniquePhone).WithMessage("Bu telefon raqam allaqachon ro'yxatdan o'tgan");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email manzil noto'g'ri format")
            .MustAsync(BeUniqueEmail).WithMessage("Bu email allaqachon ro'yxatdan o'tgan")
            .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Parol majburiy")
            .MinimumLength(6).WithMessage("Parol kamida 6 ta belgi bo'lishi kerak")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$")
            .WithMessage("Parol kichik, katta harf va raqam bo'lishi kerak");
    }

    private async Task<bool> BeUniquePhone(string phone, CancellationToken cancellationToken)
    {
        return !await _customerRepository.IsPhoneExistsAsync(phone);
    }

    private async Task<bool> BeUniqueEmail(string? email, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(email)) return true;
        return !await _customerRepository.IsEmailExistsAsync(email);
    }
}