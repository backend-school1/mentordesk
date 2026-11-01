using System.Text;

namespace MentorDesk.Api.Bot.Formatting;

public static class TelegramMarkdownFormatter
{
    public static IReadOnlyList<string> Format(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return [];

        return [ReplaceListMarkers(EscapeHtml(markdown))];
    }

    /// <summary>
    /// Экранирует HTML-символы в тексте, чтобы
    /// Telegram корректно отображал их как обычный текст 
    /// </summary>
    /// <param name="text">Исходный текст</param>
    /// <returns>Текст с экранированными HTML-символами</returns>
    private static string EscapeHtml(string text)
    {
        var stringBuilder = new StringBuilder(text.Length);

        foreach (char c in text)
        {
            switch (c)
            {
                case '&':
                    stringBuilder.Append("&amp;");
                    break;
                case '<':
                    stringBuilder.Append("&lt;");
                    break;
                case '>':
                    stringBuilder.Append("&gt;");
                    break;
                default:
                    stringBuilder.Append(c);
                    break;
            }
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Заменяет маркеры списка на символ "• "
    /// </summary>
    /// <param name="text">Исходный текст</param>
    /// <returns>Форматированный текст</returns>
    private static string ReplaceListMarkers(string text)
    {
        var lines = EscapeHtml(text).Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("- "))
            {
                lines[i] = "• " + lines[i][2..];
            }
        }

        return string.Join("\n", lines);
    }
}
