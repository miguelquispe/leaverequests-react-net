using FluentValidation;
using LeaveRequestAPI.Application.DTOs;

namespace LeaveRequestAPI.Application.Validators;

public class LeaveRequestCreateDTOValidator : AbstractValidator<LeaveRequestCreateDTO>
{
    public LeaveRequestCreateDTOValidator()
    {
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee ID must be greater than 0.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Start date is required.")
            .Must(BeAValidDate)
            .WithMessage("Start date must be a valid date.")
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("End date is required.")
            .Must(BeAValidDate)
            .WithMessage("End date must be a valid date.")
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required.")
            .Length(10, 500)
            .WithMessage("Reason must be between 10 and 500 characters.")
            .Must(NotContainInvalidCharacters)
            .WithMessage("Reason contains invalid characters.");

        RuleFor(x => x)
            .Must(HaveValidDateRange)
            .WithMessage("The leave request period cannot exceed 365 days.");
    }

    private static bool BeAValidDate(DateTime date)
    {
        return date != default(DateTime);
    }

    private static bool NotContainInvalidCharacters(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return false;

        // Check for basic invalid characters (you can adjust this based on your requirements)
        var invalidChars = new char[] { '<', '>', '&', '"', '\'' };
        return !reason.Any(c => invalidChars.Contains(c));
    }

    private static bool HaveValidDateRange(LeaveRequestCreateDTO request)
    {
        if (request.StartDate == default || request.EndDate == default)
            return true; // Let other validators handle empty dates

        var daysDifference = (request.EndDate - request.StartDate).TotalDays;
        return daysDifference <= 365;
    }
}
