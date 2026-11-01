using System.Text;

namespace MentorDesk.Api.Bot.Formatting;

public static class TelegramMarkdownFormatter
{
    private const int MaxMessageLength = 4096;
    
    public static IReadOnlyList<string> Format(string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
            return [];

        var parts = markdown.Split("```");
        var blocks = new List<string>();

        for (int i = 0; i < parts.Length; i++)
        {
            string block;

            if (i % 2 == 1)
            {
                // нечётный индекс - это блок кода
                block = FormatCodeBlock(parts[i]);
            }
            else
            {
                // чётный индекс - это блок текста
                block = FormatText(parts[i].Trim('\n'));
            }

            if (block.Length > MaxMessageLength && i % 2 == 0)
            {
                blocks.AddRange(SplitBlockInHalf(block, MaxMessageLength));
            }
            else if (block.Length > 0)
            {
                blocks.Add(block);
            }
        }

        return PackIntoMessages(blocks);
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
        var lines = text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("- "))
            {
                lines[i] = "• " + lines[i][2..];
            }
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Заменяет инлайн-код в markdown (текст между обратными кавычками)
    /// на HTML-тег code 
    /// </summary>
    /// <param name="text">Текст, уже экранированный через EscapeHtml</param>
    /// <returns>Обработанный текст</returns>
    private static string ConvertInlineCode(string text)
    {
        var result = new StringBuilder(text.Length);
        var position = 0;

        while (position < text.Length)
        {
            var start = text.IndexOf('`', position);
            if (start == -1)
            {
                break;
            }

            var end = text.IndexOf('`', start + 1);
            if (end == -1)
            {
                break;
            }

            result.Append(text, position, start - position);
            result.Append("<code>");
            result.Append(text, start + 1, end - start - 1);
            result.Append("</code>");

            position = end + 1;
        }

        result.Append(text, position, text.Length - position);

        return result.ToString();
    }

    /// <summary>
    /// Оборачивает блок кода в теги pre
    /// </summary>
    /// <param name="codePart">Содержимое между ```</param>
    /// <returns>Обработанный блок кода в pre</returns>
    private static string FormatCodeBlock(string codePart)
    {
        var firstLineEnd = codePart.IndexOf('\n');
        string code;
        if (firstLineEnd == -1)
        {
            code = codePart;
        }
        else
        {
            code = codePart[(firstLineEnd + 1)..];
        }

        return $"<pre>{EscapeHtml(code)}</pre>";
    }

    /// <summary>
    /// Преобразует обычный фрагмент markdown в HTML, без блоков кода
    /// </summary>
    /// <param name="text">Исходный фрагмент текста</param>
    /// <returns>Экранированный фрагмент с маркерами списка и инлайн-кодом</returns>
    private static string FormatText(string text)
    {
        return ConvertInlineCode(ReplaceListMarkers(EscapeHtml(text)));
    }

    /// <summary>
    /// Собирает готовые HTML блоки в сообщения таким образом, чтобы
    /// длина не превышала лимита Telegram, без нарушения целостности блока
    /// </summary>
    /// <param name="blocks">Готовые HTML блоки</param>
    /// <returns>Список сообщений для отправки</returns>
    private static List<string> PackIntoMessages(List<string> blocks)
    {
        var messages = new List<string>();
        var current = new StringBuilder();

        foreach (string block in blocks)
        {
            if (current.Length == 0)
            {
                current.Append(block);
            }
            else
            {
                var lengthWithBlock = current.Length + 1 + block.Length;

                if (lengthWithBlock <= MaxMessageLength)
                {
                    current.Append('\n');
                    current.Append(block);
                }
                else
                {
                    messages.Add(current.ToString());
                    current.Clear();
                    current.Append(block);
                }
            }
        }

        if (current.Length > 0)
        {
            messages.Add(current.ToString());
        }

        return messages;
    }

    /// <summary>
    /// Делит длинный блок кода пополам до ближайшего перевода строки,
    /// пока каждый кусочек не станет короче лимита Telegram
    /// </summary>
    /// <param name="block">Блок, который нужно поделить</param>
    /// <param name="maxLength">Максимальная длина одного куска</param>
    /// <returns>Список кусков блока, каждый из которых помещается в сообщение</returns>
    private static List<string> SplitBlockInHalf(string block, int maxLength)
    {
        if (block.Length <= maxLength)
        {
            return [block];
        }

        var middle = block.Length / 2;
        var newLineIndex = block.LastIndexOf('\n', middle);

        string left;
        string right;

        if (newLineIndex > 0)
        {
            left = block[..newLineIndex];
            right = block[(newLineIndex + 1)..];
        }
        else
        {
            left = block[..middle];
            right = block[middle..];
        }

        var result = SplitBlockInHalf(left, maxLength);
        result.AddRange(SplitBlockInHalf(right, maxLength));

        return result;
    }
}
