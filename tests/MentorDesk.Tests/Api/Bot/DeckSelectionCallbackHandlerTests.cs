using FluentAssertions;
using MentorDesk.Api.Bot;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Tests.Api.Bot;

public class DeckSelectionCallbackHandlerTests
{
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<ITelegramBotClient> _telegramBotClientMock;

    public DeckSelectionCallbackHandlerTests()
    {
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _telegramBotClientMock = new Mock<ITelegramBotClient>();
    }

    [Fact]
    public async Task HandleAsync_WhenStudentAndDeckExist_ShouldSaveSelectedDeck()
    {
        var student = new Student() { TelegramUserName = "test_user", TelegramId = 12345 };
        var deck = new Deck() { Id = Guid.NewGuid(), Title = "C# Basics" };

        _studentRepositoryMock
            .Setup(x => x.GetByTelegramIdAsync(
                12345,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _studentRepositoryMock
            .Setup(x => x.GetByIdAsync(student.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _deckRepositoryMock
            .Setup(x => x.GetDeckAsync(deck.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deck);

        var callbackQuery = new CallbackQuery()
        {
            Id = "callback-123",
            From = new User() { Id = 12345, Username = "test_user" },
            Data = $"{BotConstants.SelectDeckCallbackPrefix}{deck.Id}"
        };

        var handler = new DeckSelectionCallbackHandler(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await handler.HandleAsync(
            _telegramBotClientMock.Object,
            callbackQuery,
            CancellationToken.None);

        student.SelectedDeckId.Should().Be(deck.Id);

        _studentRepositoryMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenDeckDoesNotExist_ShouldNotSaveChanges()
    {
        var deck = new Deck() { Id = Guid.NewGuid(), Title = "C# Basics" };

        _deckRepositoryMock
            .Setup(x => x.GetDeckAsync(deck.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Deck?)null);

        var callbackQuery = new CallbackQuery()
        {
            Id = "callback-123",
            From = new User() { Id = 12345, Username = "test_user" },
            Data = $"{BotConstants.SelectDeckCallbackPrefix}{deck.Id}"
        };

        var handler = new DeckSelectionCallbackHandler(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await handler.HandleAsync(
            _telegramBotClientMock.Object,
            callbackQuery,
            CancellationToken.None);

        _studentRepositoryMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
