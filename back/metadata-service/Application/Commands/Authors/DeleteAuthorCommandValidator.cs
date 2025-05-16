using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class DeleteAuthorCommandValidator
    : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotEmpty().WithMessage("Id автора обязателен и не может быть пустым");
    }
}