using System.ComponentModel.DataAnnotations;

using FormsLab.Models;
using FormsLab.Validation;

namespace FormsLab.Tests;

public class ValidationRuleTests
{
    private static RegistrationForm Valid() => new()
    {
        FullName = "Roshan K",
        Email = "roshan@example.com",
        DateOfBirth = new DateOnly(1995, 3, 14),
        Password = "correct-horse-battery",
        ConfirmPassword = "correct-horse-battery",
        AcceptTerms = true,
    };

    private static List<ValidationResult> Validate(RegistrationForm form)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(form, new ValidationContext(form), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Valid_form_has_no_errors()
    {
        Assert.Empty(Validate(Valid()));
    }

    [Theory]
    [InlineData("+971501234567", true)]
    [InlineData("+971 50 123 4567", true)]
    [InlineData("050-123-4567", true)]
    [InlineData("0412345678", false)]
    [InlineData("+97150123456", false)]
    [InlineData("hello", false)]
    public void Uae_mobile_numbers_are_validated(string phone, bool expected)
    {
        Assert.Equal(expected, new UaeMobileAttribute().IsValid(phone));
    }

    [Fact]
    public void Minimum_age_counts_birthdays_correctly()
    {
        MinimumAgeAttribute.Today = () => new DateOnly(2026, 11, 10);
        var rule = new MinimumAgeAttribute(18);

        Assert.True(rule.IsValid(new DateOnly(2008, 11, 10)));   // 18 today
        Assert.False(rule.IsValid(new DateOnly(2008, 11, 11)));  // 18 tomorrow
    }

    [Fact]
    public void Terms_must_be_accepted()
    {
        var form = Valid();
        form.AcceptTerms = false;

        Assert.Contains(Validate(form), r => r.ErrorMessage == "Please accept the terms to continue.");
    }

    [Fact]
    public void Mismatched_passwords_are_rejected()
    {
        var form = Valid();
        form.ConfirmPassword = "something-else";

        Assert.Contains(Validate(form), r => r.MemberNames.Contains(nameof(RegistrationForm.ConfirmPassword)));
    }

    [Fact]
    public void Phone_is_required_only_when_phone_contact_is_preferred()
    {
        var form = Valid();
        form.ContactPreference = ContactPreference.Phone;

        var errors = Validate(form);

        Assert.Contains(errors, r => r.MemberNames.Contains(nameof(RegistrationForm.Phone)));
        form.Phone = "+971501234567";
        Assert.Empty(Validate(form));
    }

    [Fact]
    public void Password_may_not_contain_the_users_first_name()
    {
        var form = Valid();
        form.Password = form.ConfirmPassword = "roshan-is-great-2026";

        Assert.Contains(Validate(form), r => r.ErrorMessage == "Password must not contain your name.");
    }
}
