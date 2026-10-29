using System.Text;

namespace MentorDesk.Api.Bot.Formatting;

public static class TelegramMarkdownFormatter
{
    public static IReadOnlyList<string> Format(string markdown)
    {
        throw new NotImplementedException();
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
}
