using FluentValidation;
using LeaveRequestAPI.Application.DTOs;
using LeaveRequestAPI.Domain.Enums;

namespace LeaveRequestAPI.Application.Validators;

public class LeaveRequestUpdateStatusDTOValidator : AbstractValidator<LeaveRequestUpdateStatusDTO>
{
    public LeaveRequestUpdateStatusDTOValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status must be a valid leave status.")
            .Must(BeAValidStatusTransition)
            .WithMessage("Invalid status transition. Status must be Pending, Approved, or Rejected.");
    }

    private static bool BeAValidStatusTransition(LeaveStatus status)
    {
        // Ensure the status is one of the defined enum values
        return Enum.IsDefined(typeof(LeaveStatus), status);
    }
}
