using System.ComponentModel.DataAnnotations;

using FormsLab.Validation;

namespace FormsLab.Models;

public enum ContactPreference
{
    Email,
    Phone,
}

/// <summary>
/// One model, validated three ways: attributes on properties, a custom attribute library,
/// and cross-field rules in <see cref="Validate"/>.
/// </summary>
public sealed class RegistrationForm : IValidatableObject
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "Full name must be 2 to 80 characters.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [UaeMobile]
    public string? Phone { get; set; }

    public ContactPreference ContactPreference { get; set; } = ContactPreference.Email;

    [Required(ErrorMessage = "Date of birth is required.")]
    [MinimumAge(18)]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(64, MinimumLength = 10, ErrorMessage = "Password must be at least 10 characters.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MustBeTrue(ErrorMessage = "Please accept the terms to continue.")]
    public bool AcceptTerms { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Cross-field rule: the phone becomes required only when the user prefers phone contact.
        if (ContactPreference == ContactPreference.Phone && string.IsNullOrWhiteSpace(Phone))
        {
            yield return new ValidationResult(
                "Phone is required when you prefer to be contacted by phone.", [nameof(Phone)]);
        }

        if (!string.IsNullOrEmpty(Password)
            && !string.IsNullOrEmpty(FullName)
            && Password.Contains(FullName.Split(' ')[0], StringComparison.OrdinalIgnoreCase))
        {
            yield return new ValidationResult("Password must not contain your name.", [nameof(Password)]);
        }
    }
}
