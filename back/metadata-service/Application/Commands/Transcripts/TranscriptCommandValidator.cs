using FluentValidation;

namespace MetadataService.Application.Commands;

public sealed class UpdateTranscriptTextCommandValidator : AbstractValidator<UpdateTranscriptTextCommand>
{
    public UpdateTranscriptTextCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
        RuleFor(c => c.Request.Text)
            .NotEmpty().WithMessage("Text обязателен");
    }
}

public sealed class ApproveTranscriptCommandValidator : AbstractValidator<ApproveTranscriptCommand>
{
    public ApproveTranscriptCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
    }
}

public sealed class DeleteTranscriptCommandValidator : AbstractValidator<DeleteTranscriptCommand>
{
    public DeleteTranscriptCommandValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0).WithMessage("Id должен быть положительным");
    }
}