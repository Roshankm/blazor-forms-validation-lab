namespace FormsLab.Services;

/// <summary>
/// A rule that needs the server (in a real app, a database lookup), so it can't be a
/// synchronous validation attribute. The form calls it on submit and shows the result inline.
/// </summary>
public interface IEmailAvailability
{
    Task<bool> IsAvailableAsync(string email, CancellationToken cancellationToken = default);
}

public sealed class InMemoryEmailAvailability : IEmailAvailability
{
    private static readonly HashSet<string> Taken = new(StringComparer.OrdinalIgnoreCase)
    {
        "taken@example.com",
        "admin@example.com",
    };

    public async Task<bool> IsAvailableAsync(string email, CancellationToken cancellationToken = default)
    {
        await Task.Delay(150, cancellationToken); // simulate a database round trip
        return !Taken.Contains(email.Trim());
    }
}
