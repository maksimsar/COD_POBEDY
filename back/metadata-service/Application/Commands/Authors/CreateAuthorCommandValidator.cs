using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class CreateAuthorCommandValidator
    : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(cmd => cmd.Request.FullName)
            .NotEmpty().WithMessage("FullName обязателен")
            .MaximumLength(0xC8).WithMessage("FullName не может быть длиннее 200 символов");

        RuleFor(cmd => cmd.Request.BirthYear)
            .GreaterThanOrEqualTo((short)1800).When(a => a.Request.BirthYear.HasValue)
            .WithMessage("BirthYear должен быть >= 1800");

        RuleFor(cmd => cmd.Request.DeathYear)
            .GreaterThanOrEqualTo(a => a.Request.BirthYear.GetValueOrDefault())
            .When(a => a.Request.DeathYear.HasValue && a.Request.BirthYear.HasValue)
            .WithMessage("DeathYear не может быть меньше BirthYear");

        RuleFor(cmd => cmd.Request.Notes)
            .MaximumLength(0x3E8).WithMessage("Notes не может быть длиннее 1000 символов");
    }
}