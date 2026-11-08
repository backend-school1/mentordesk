using FluentAssertions;
using MentorDesk.Api.Bot;
using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
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
        var student = new Student { TelegramUserName = "test_user", TelegramId = 12345 };

        var deck = new Deck { Id = Guid.NewGuid(), Title = "C# Basics" };

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
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([student]);

        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([deck]);

        _telegramBotClientMock
            .Setup(x => x.SendRequest<Message>(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message());

        var command = new DecksCommand(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await command.ExecuteAsync(
            _telegramBotClientMock.Object,
            message,
            CancellationToken.None);

        _telegramBotClientMock.Verify(
            x => x.SendRequest<Message>(
                It.Is<SendMessageRequest>(request =>
                    request.Text == "Выбери колоду для обучения:"
                    && request.ReplyMarkup is InlineKeyboardMarkup),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoDecksExist_ShouldSendNoDecksMessage()
    {
        var student = new Student { TelegramUserName = "test_user", TelegramId = 12345 };

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
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([student]);

        _deckRepositoryMock
            .Setup(x => x.GetAllDecksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _telegramBotClientMock
            .Setup(x => x.SendRequest<Message>(
                It.IsAny<SendMessageRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Message());

        var command = new DecksCommand(
            _studentRepositoryMock.Object,
            _deckRepositoryMock.Object);

        await command.ExecuteAsync(
            _telegramBotClientMock.Object,
            message,
            CancellationToken.None);

        _telegramBotClientMock.Verify(
            x => x.SendRequest<Message>(
                It.Is<SendMessageRequest>(request =>
                    request.Text == "Колод пока нет"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
