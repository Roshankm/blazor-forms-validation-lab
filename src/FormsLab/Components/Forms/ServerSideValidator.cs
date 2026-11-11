using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace FormsLab.Components.Forms;

/// <summary>
/// Lets a form add errors that come from the server (for example "email already registered")
/// into the same <see cref="EditContext"/> that <c>DataAnnotationsValidator</c> uses, so they
/// appear next to the right field and in the summary.
/// </summary>
public sealed class ServerSideValidator : ComponentBase, IDisposable
{
    private ValidationMessageStore? _store;
    private EditContext? _context;

    [CascadingParameter]
    private EditContext? CurrentEditContext { get; set; }

    protected override void OnInitialized()
    {
        _context = CurrentEditContext
            ?? throw new InvalidOperationException($"{nameof(ServerSideValidator)} must be placed inside an EditForm.");
        _store = new ValidationMessageStore(_context);

        // Clear a server error as soon as the user edits that field.
        _context.OnFieldChanged += OnFieldChanged;
        _context.OnValidationRequested += OnValidationRequested;
    }

    public void DisplayError(string fieldName, string message)
    {
        _store!.Add(_context!.Field(fieldName), message);
        _context.NotifyValidationStateChanged();
    }

    public void Clear()
    {
        _store!.Clear();
        _context!.NotifyValidationStateChanged();
    }

    private void OnFieldChanged(object? sender, FieldChangedEventArgs e) => _store!.Clear(e.FieldIdentifier);

    private void OnValidationRequested(object? sender, ValidationRequestedEventArgs e) => _store!.Clear();

    public void Dispose()
    {
        if (_context is not null)
        {
            _context.OnFieldChanged -= OnFieldChanged;
            _context.OnValidationRequested -= OnValidationRequested;
        }
    }
}
