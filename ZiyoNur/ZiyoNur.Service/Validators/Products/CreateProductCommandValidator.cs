using FluentValidation;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Features.Products.Commands;

namespace ZiyoNur.Service.Validators.Products;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CreateProductCommandValidator(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Mahsulot nomi majburiy")
            .MaximumLength(200).WithMessage("Nomi 200 ta belgidan ko'p bo'lmasligi kerak");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Narx 0 dan katta bo'lishi kerak");

        RuleFor(x => x.Count)
            .GreaterThanOrEqualTo(0).WithMessage("Miqdor 0 dan kichik bo'lmasligi kerak");

        RuleFor(x => x.CategoryId)
            .MustAsync(CategoryExists).WithMessage("Kategoriya topilmadi");

        RuleFor(x => x.QrCode)
            .MustAsync(BeUniqueQrCode).WithMessage("Bu QR kod allaqachon mavjud")
            .When(x => !string.IsNullOrEmpty(x.QrCode));
    }

    private async Task<bool> CategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId);
        return category != null && category.IsActive;
    }

    private async Task<bool> BeUniqueQrCode(string? qrCode, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(qrCode)) return true;
        return !await _productRepository.IsQrCodeExistsAsync(qrCode);
    }
}