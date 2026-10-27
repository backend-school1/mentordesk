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
}
