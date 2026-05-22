using System.Text.RegularExpressions;

namespace SecureApp.Utilities;

public static partial class UserInputValidator
{
    private const int MaxUsernameLength = 100;
    private const int MaxEmailLength = 100;

    // Letters, numbers, space, dot, underscore, dash
    [GeneratedRegex(@"^[a-zA-Z0-9._\- ]+$", RegexOptions.CultureInvariant)]
    private static partial Regex UsernameRegex();

    // Basic "dangerous content" detector for XSS/SQLi-like payloads
    [GeneratedRegex(@"(<|>|javascript:|on\w+\s*=|--|/\*|\*/|;)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex DangerousPatternRegex();

    public static ValidationResult ValidateNewUser(string? username, string? email)
    {
        var errors = new List<string>();

        username = (username ?? string.Empty).Trim();
        email = (email ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(username))
            errors.Add("Username is required.");
        else if (username.Length > MaxUsernameLength)
            errors.Add($"Username must be {MaxUsernameLength} characters or fewer.");
        else if (!UsernameRegex().IsMatch(username))
            errors.Add("Username contains invalid characters.");

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required.");
        else if (email.Length > MaxEmailLength)
            errors.Add($"Email must be {MaxEmailLength} characters or fewer.");
        else if (!IsValidEmail(email))
            errors.Add("Email format is invalid.");

        if (DangerousPatternRegex().IsMatch(username) || DangerousPatternRegex().IsMatch(email))
            errors.Add("Input contains blocked patterns.");

        return new ValidationResult(errors.Count == 0, username, email, errors);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address.Equals(email, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    public sealed record ValidationResult(
        bool IsValid,
        string Username,
        string Email,
        IReadOnlyList<string> Errors);
}