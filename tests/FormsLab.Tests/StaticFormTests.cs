using Microsoft.AspNetCore.Mvc.Testing;

namespace FormsLab.Tests;

public class StaticFormTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Static_form_renders_named_inputs_and_an_antiforgery_token()
    {
        var html = await factory.CreateClient().GetStringAsync("/register-static");

        // Static SSR forms post by field name, so every input must carry one.
        Assert.Contains("name=\"Model.FullName\"", html);
        Assert.Contains("name=\"Model.Email\"", html);
        Assert.Contains("name=\"__RequestVerificationToken\"", html);
        Assert.Contains("name=\"_handler\" value=\"register\"", html);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/register")]
    public async Task Pages_render(string url)
    {
        var response = await factory.CreateClient().GetAsync(url);

        Assert.True(response.IsSuccessStatusCode);
    }
}
