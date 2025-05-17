using System;
using FluentValidation;
using MetadataService.Application.Commands;

namespace MetadataService.Application.Commands;

public class UpdateSongTagsCommandValidator 
    : AbstractValidator<UpdateSongTagsCommand>
{
    public UpdateSongTagsCommandValidator()
    {
        RuleFor(cmd => cmd.Request.SongId)
            .NotEmpty()
            .WithMessage("SongId не должен быть пустым");

        RuleFor(cmd => cmd.Request.TagIds)
            .NotNull()
            .WithMessage("Список TagIds обязателен")
            .Must(list => list.Count > 0)
            .WithMessage("Должен быть хотя бы один TagId");

        // Дополнительно можно проверить, что ни один id не является нулевым
        RuleForEach(cmd => cmd.Request.TagIds)
            .NotEmpty()
            .WithMessage("TagId не должен быть пустым");
    }
}