using System.Text.RegularExpressions;

namespace MentorDesk.Application.Students;

public static class TelegramUsernameNormalizer
{
    public static string Normalize(string rawUsername)
    {
        if (string.IsNullOrWhiteSpace(rawUsername))
            throw new ArgumentException("Username is required.");

        var username = rawUsername.Trim();

        if (username.StartsWith('@'))
            username = username[1..];

        username = username.ToLowerInvariant();

        if (!Regex.IsMatch(username, @"^[a-z0-9_]{5,32}$"))
            throw new ArgumentException("Invalid Telegram username.");

        return username;
    }
}
