using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace FormsLab.Validation;

/// <summary>Date of birth must make the person at least <see cref="Years"/> years old today.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinimumAgeAttribute(int years) : ValidationAttribute($"You must be at least {years} years old.")
{
    public int Years { get; } = years;

    /// <summary>Overridable "today" so the rule can be tested on any date.</summary>
    public static Func<DateOnly> Today { get; set; } = () => DateOnly.FromDateTime(DateTime.Today);

    public override bool IsValid(object? value)
    {
        if (value is not DateOnly dob)
        {
            return true; // [Required] handles missing values
        }

        var today = Today();
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
        {
            age--;
        }

        return age >= Years;
    }
}

/// <summary>UAE mobile number: +9715XXXXXXXX or 05XXXXXXXX (spaces and dashes allowed).</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed partial class UaeMobileAttribute() : ValidationAttribute("Enter a UAE mobile number, for example +971 50 123 4567.")
{
    public override bool IsValid(object? value)
    {
        if (value is not string text || string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        var digits = text.Replace(" ", string.Empty, StringComparison.Ordinal).Replace("-", string.Empty, StringComparison.Ordinal);
        return UaeMobile().IsMatch(digits);
    }

    [GeneratedRegex(@"^(\+9715|05)\d{8}$")]
    private static partial Regex UaeMobile();
}

/// <summary>For checkboxes such as "I accept the terms", where false must fail validation.</summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MustBeTrueAttribute() : ValidationAttribute("This must be accepted.")
{
    public override bool IsValid(object? value) => value is true;
}
