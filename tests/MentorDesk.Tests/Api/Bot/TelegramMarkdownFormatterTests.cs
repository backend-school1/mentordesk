using FluentAssertions;
using MentorDesk.Api.Bot.Formatting;

namespace MentorDesk.Tests.Api.Bot;

public class TelegramMarkdownFormatterTests
{
    [Fact]
    public void Format_WhenInputIsPlainText_ShouldReturnUnchanged()
    {
        var result = TelegramMarkdownFormatter.Format("SOLID");

        result.Should().ContainSingle().Which.Should().Be("SOLID");
    }

    [Fact]
    public void Format_WhenInputIsMarkdownList_ShouldReplaceHyphensWithBullets()
    {
        var result = TelegramMarkdownFormatter.Format("- First\n- Second");

        result.Should().ContainSingle().Which.Should().Be("• First\n• Second");
    }

    [Fact]
    public void Format_WhenInputContainsInlineCode_ShouldConvertToHtml()
    {
        var result = TelegramMarkdownFormatter.Format("`Where` для фильтрации");

        result.Should().ContainSingle().Which.Should().Be("<code>Where</code> для фильтрации");
    }

    [Fact]
    public void Format_WhenInputContainsCodeBlockWithSpecialCharacters_ShouldEscapeHtmlCharacters()
    {
        var result = TelegramMarkdownFormatter
            .Format("```csharp\nif (age < 18)\n{\nreturn;\n}\n```");

        result.Should().ContainSingle().Which.Should()
            .Be("<pre>if (age &lt; 18)\n{\nreturn;\n}</pre>");
    }

    [Fact]
    public void Format_WhenInputExceedsTelegramMessageLimit_ShouldSplitIntoMultipleMessages()
    {
        var result = TelegramMarkdownFormatter.Format(new string('a', 5000));

        result.Should().HaveCountGreaterThan(1);
        result.Should().OnlyContain(x => x.Length <= 4096);
    }
}
