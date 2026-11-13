using FluentAssertions;
using MentorDesk.Api.Bot;
using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MentorDesk.Tests.Api.Bot;

public class DecksCommandTests
{
    private readonly Mock<IStudentRepository> _studentRepositoryMock;
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<ITelegramBotClient> _telegramBotClientMock;

    public DecksCommandTests()
    {
        _studentRepositoryMock = new Mock<IStudentRepository>();
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _telegramBotClientMock = new Mock<ITelegramBotClient>();
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentAndDeckExist_ShouldSendMessageWithKeyboard()
    {
        var student = new Student
        {
            TelegramUserName = "test_user",
            TelegramId = 12345
        };

        var deck = new Deck
        {
            Id = Guid.NewGuid(),
            Title = "C# Basics"
        };

        var message = new Message
        {
            Chat = new Chat
            {
                Id = 12345,
                Type = ChatType.Private
            },
            From = new User
            {
                Id = 12345,
                Username = "test_user"
            }
        };

        _studentRepositoryMock
            .Setup(x => x.GetByTelegramIdAsync(
                12345,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([deck]);

        SendMessageRequest? sentRequest = null;

        _telegramBotClientMock
            .Setup(x => x.SendRequest<Message>(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) =>
                sentRequest = (SendMessageRequest)request)
            .ReturnsAsync(new Message());

        var command = new DecksCommand(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await command.ExecuteAsync(
            _telegramBotClientMock.Object,
            message,
            CancellationToken.None);

        sentRequest.Should().NotBeNull();
        sentRequest!.Text.Should().Be("Выбери колоду для обучения:");

        var keyboard = sentRequest.ReplyMarkup.Should()
            .BeOfType<InlineKeyboardMarkup>()
            .Subject;

        var rows = keyboard.InlineKeyboard.ToArray();

        rows.Should().HaveCount(1);
        rows[0].Should().HaveCount(1);

        var button = rows[0].First();

        button.Text.Should().Be(deck.Title);
        button.CallbackData.Should().Be(
            $"{BotConstants.SelectDeckCallbackPrefix}{deck.Id}");
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoDecksExist_ShouldSendNoDecksMessage()
    {
        var student = new Student
        {
            TelegramUserName = "test_user",
            TelegramId = 12345
        };

        var message = new Message
        {
            Chat = new Chat
            {
                Id = 12345,
                Type = ChatType.Private
            },
            From = new User
            {
                Id = 12345,
                Username = "test_user"
            }
        };

        _studentRepositoryMock
            .Setup(x => x.GetByTelegramIdAsync(
                12345,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);

        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        SendMessageRequest? sentRequest = null;

        _telegramBotClientMock
            .Setup(x => x.SendRequest<Message>(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) =>
                sentRequest = (SendMessageRequest)request)
            .ReturnsAsync(new Message());

        var command = new DecksCommand(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await command.ExecuteAsync(
            _telegramBotClientMock.Object,
            message,
            CancellationToken.None);

        sentRequest.Should().NotBeNull();
        sentRequest!.Text.Should().Be("Колод пока нет");
    }

    [Fact]
    public async Task ExecuteAsync_WhenStudentDoesNotExist_ShouldSendAccessDeniedMessage()
    {
        var message = new Message
        {
            Chat = new Chat
            {
                Id = 12345,
                Type = ChatType.Private
            },
            From = new User
            {
                Id = 12345,
                Username = "unknown_user"
            }
        };

        _studentRepositoryMock
            .Setup(x => x.GetByTelegramIdAsync(
                12345,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Student?)null);

        SendMessageRequest? sentRequest = null;

        _telegramBotClientMock
            .Setup(x => x.SendRequest<Message>(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .Callback<IRequest<Message>, CancellationToken>((request, _) =>
                sentRequest = (SendMessageRequest)request)
            .ReturnsAsync(new Message());

        var command = new DecksCommand(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await command.ExecuteAsync(
            _telegramBotClientMock.Object,
            message,
            CancellationToken.None);

        sentRequest.Should().NotBeNull();
        sentRequest!.Text.Should().Be("Доступа нет, напиши ментору");

        _deckRepositoryMock.Verify(
            x => x.GetAllDecksAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
