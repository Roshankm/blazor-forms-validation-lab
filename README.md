# Blazor Forms & Validation Lab

[![CI](https://github.com/Roshankm/blazor-forms-validation-lab/actions/workflows/ci.yml/badge.svg)](https://github.com/Roshankm/blazor-forms-validation-lab/actions/workflows/ci.yml)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
![bUnit](https://img.shields.io/badge/tests-bUnit-6f42c1)

A .NET 10 Blazor reference app for **form validation done properly**: one registration model validated with built-in attributes, custom attributes, cross-field rules and an async server-side check, in both an interactive form and a static SSR form. No third-party packages.

Part of my set of Blazor reference labs, each one focused on a single feature with tests and CI.

## Techniques

| Technique | Where |
|---|---|
| Built-in attributes: `[Required]`, `[StringLength]`, `[EmailAddress]`, `[Compare]` | `Models/RegistrationForm.cs` |
| Custom attributes: `[MinimumAge(18)]`, `[UaeMobile]`, `[MustBeTrue]` | `Validation/ValidationAttributes.cs` |
| Cross-field rules with `IValidatableObject` (phone required only for phone contact; password can't contain your name) | `RegistrationForm.Validate` |
| Async server check ("email already registered") shown next to the field, using `ValidationMessageStore` | `Components/Forms/ServerSideValidator.cs` |
| Shared field component reused by two forms | `Components/Forms/RegistrationFields.razor` |
| Interactive Server form | `/register` |
| Static SSR form with `[SupplyParameterFromForm]`, `FormName`, antiforgery and enhanced posts | `/register-static` |

## Run it

```bash
dotnet run --project src/FormsLab --launch-profile https
dotnet test
```

Open https://localhost:7302. Use `taken@example.com` to see the server-side email check.

## Tests

* **Rule tests** run each attribute and cross-field rule through `Validator.TryValidateObject`, including the 18th-birthday edge case.
* **bUnit component tests** render the form, type into inputs, submit, and assert the validation messages and success state.
* **HTTP tests** with `WebApplicationFactory` check that the static form renders named inputs, the handler name and an antiforgery token.

## Notes

* `IValidatableObject.Validate` only runs once all property attributes pass. That is standard DataAnnotations behaviour, and it keeps cross-field messages from piling on top of basic ones.
* Server errors are cleared as soon as the user edits that field, so they never go stale.
* The static form validates on the server with exactly the same model, so it works with JavaScript disabled.

## License

[MIT](LICENSE)
