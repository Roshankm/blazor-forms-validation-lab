using FormsLab.Components.Forms;
using FormsLab.Services;

using Microsoft.Extensions.DependencyInjection;

namespace FormsLab.Tests;

/// <summary>bUnit tests: render the form component, fill it in like a user, and check the markup.</summary>
public class RegistrationEditorTests : BunitContext
{
    public RegistrationEditorTests()
    {
        Services.AddSingleton<IEmailAvailability, InMemoryEmailAvailability>();
    }

    [Fact]
    public void Submitting_an_empty_form_shows_required_field_messages()
    {
        var cut = Render<RegistrationEditor>();

        cut.Find("form").Submit();

        var messages = cut.FindAll(".validation-message").Select(m => m.TextContent).ToList();
        Assert.Contains("Full name is required.", messages);
        Assert.Contains("Email is required.", messages);
        Assert.Contains("Please accept the terms to continue.", messages);
    }

    [Fact]
    public void Email_that_is_already_registered_shows_a_server_side_error_on_the_field()
    {
        var cut = Render<RegistrationEditor>();
        FillValidForm(cut, "taken@example.com");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() =>
            Assert.Contains("This email is already registered.", cut.FindAll(".validation-message").Select(m => m.TextContent)));
    }

    [Fact]
    public void Valid_form_with_a_new_email_registers_the_user()
    {
        var cut = Render<RegistrationEditor>();
        FillValidForm(cut, "new.user@example.com");

        cut.Find("form").Submit();

        cut.WaitForAssertion(() => Assert.Contains("Welcome, Roshan K", cut.Find("[data-test=success]").TextContent));
    }

    private static void FillValidForm(IRenderedComponent<RegistrationEditor> cut, string email)
    {
        cut.Find("#fullName").Change("Roshan K");
        cut.Find("#email").Change(email);
        cut.Find("#dob").Change("1995-03-14");
        cut.Find("#password").Change("correct-horse-battery");
        cut.Find("#confirm").Change("correct-horse-battery");
        cut.Find("#terms").Change(true);
    }
}
