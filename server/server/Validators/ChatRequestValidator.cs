using FluentValidation;
using server.Models;

namespace server.Validators;

public class ChatRequestValidator : AbstractValidator<ChatRequest>
{
    public ChatRequestValidator()
    {
        RuleFor(x => x.Prompt)
            .NotEmpty().WithMessage("Prompt is required.")
            .MaximumLength(1000).WithMessage("Prompt is too long (max 1000 characters).");

        RuleFor(x => x.ConversationId)
            .NotEmpty()
            .Must(id => Guid.TryParse(id, out _)).WithMessage("ConversationId must be a valid UUID.");
    }
}