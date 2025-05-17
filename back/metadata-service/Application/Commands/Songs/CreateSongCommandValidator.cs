using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class CreateSongCommandValidator : AbstractValidator<CreateSongCommand>
{
    public CreateSongCommandValidator()
    {
        RuleFor(s => s.Request.Title)
            .NotEmpty().MaximumLength(256);
        RuleFor(s => s.Request.AuthorIds)
            .NotEmpty();
        RuleFor(s => s.Request.TagIds)
            .NotEmpty();
    }
}