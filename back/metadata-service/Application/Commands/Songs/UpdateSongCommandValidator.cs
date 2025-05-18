using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class UpdateSongCommandValidator : AbstractValidator<UpdateSongCommand>
{
    public UpdateSongCommandValidator()
    {
        RuleFor(s => s.SongId).NotEmpty();
        RuleFor(s => s.Request.Title).MaximumLength(256);
    }
}