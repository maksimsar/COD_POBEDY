using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class UpdateAuthorCommandValidator
    : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(cmd => cmd.Id)
            .NotEmpty().WithMessage("Id автора обязателен");

        RuleFor(cmd => cmd.Request.FullName)
            .MaximumLength(200).WithMessage("FullName не может быть длиннее 200 символов");

        RuleFor(cmd => cmd.Request.BirthYear)
            .GreaterThanOrEqualTo((short)1800).When(x => x.Request.BirthYear.HasValue)
            .WithMessage("BirthYear должен быть >= 1800");

        RuleFor(cmd => cmd.Request.DeathYear)
            .GreaterThanOrEqualTo(x => x.Request.BirthYear.GetValueOrDefault())
            .When(x => x.Request.DeathYear.HasValue && x.Request.BirthYear.HasValue)
            .WithMessage("DeathYear не может быть меньше BirthYear");

        RuleFor(cmd => cmd.Request.Notes)
            .MaximumLength(1000).WithMessage("Notes не может быть длиннее 1000 символов");
    }
}