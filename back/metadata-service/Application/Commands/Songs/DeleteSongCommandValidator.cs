using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class DeleteSongCommandValidator
    : AbstractValidator<DeleteSongCommand>
{
    public DeleteSongCommandValidator()
    {
        RuleFor(cmd => cmd.SongId)
            .NotEmpty().WithMessage("Id песни обязателен и не может быть пустым");
    }
}