using System.Text.RegularExpressions;

namespace MentorDesk.Application.Features.Students;

public static class TelegramUsernameNormalizer
{
    public static string Normalize(string rawUsername)
    {
        var username = rawUsername.Trim();

        if (username.StartsWith('@'))
            username = username[1..];

        return username.ToLowerInvariant();
    }
}
