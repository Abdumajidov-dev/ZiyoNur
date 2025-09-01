using FluentValidation;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Features.Categories.Commands;

namespace ZiyoNur.Service.Validators.Categories;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandValidator(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kategoriya nomi majburiy")
            .MaximumLength(100).WithMessage("Nomi 100 ta belgidan ko'p bo'lmasligi kerak");

        RuleFor(x => x.ParentId)
            .MustAsync(ParentCategoryExists).WithMessage("Ota kategoriya topilmadi")
            .MustAsync((command, parentId, cancellationToken) => NotSelfParent(command.Id, parentId, cancellationToken))
            .WithMessage("Kategoriya o'zini ota kategoriya qilib bo'lmaydi")
            .When(x => x.ParentId.HasValue);

        RuleFor(x => x.SortOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Tartib raqami 0 dan kichik bo'lmasligi kerak");
    }

    private async Task<bool> ParentCategoryExists(int? parentId, CancellationToken cancellationToken)
    {
        if (!parentId.HasValue) return true;

        var category = await _categoryRepository.GetByIdAsync(parentId.Value);
        return category != null && category.IsActive;
    }

    private async Task<bool> NotSelfParent(int categoryId, int? parentId, CancellationToken cancellationToken)
    {
        return !parentId.HasValue || parentId.Value != categoryId;
    }
}