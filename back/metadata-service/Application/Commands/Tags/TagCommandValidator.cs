using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class CreateTagCommandValidator : AbstractValidator<CreateTagCommand>
{
    public CreateTagCommandValidator()
    {
        RuleFor(c => c.Request.Name)
            .NotEmpty().WithMessage("Name обязателен")
            .MaximumLength(50).WithMessage("Name ≤ 50 символов");
    }
}

public sealed class UpdateTagCommandValidator : AbstractValidator<UpdateTagCommand>
{
    public UpdateTagCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
        // Если имя передано, проверяем длину
        RuleFor(c => c.Request.Name)
            .MaximumLength(50)
            .When(c => c.Request.Name is not null)
            .WithMessage("Name ≤ 50 символов");
    }
}

public sealed class ApproveTagCommandValidator : AbstractValidator<ApproveTagCommand>
{
    public ApproveTagCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
    }
}

public sealed class DeleteTagCommandValidator : AbstractValidator<DeleteTagCommand>
{
    public DeleteTagCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
    }
}