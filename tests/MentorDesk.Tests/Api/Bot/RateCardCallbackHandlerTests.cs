using MentorDesk.Api.Bot;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Xunit;

namespace MentorDesk.Tests.Api.Bot;

public class RateCardCallbackHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepositoryMock = new();
    private readonly Mock<ITelegramBotClient> _botClientMock = new();
    private readonly RateCardCallbackHandler _handler;

    public RateCardCallbackHandlerTests()
    {
        _handler = new RateCardCallbackHandler(_cardRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_NextCardExists_SendsNextQuestion()
    {
        var deckId = Guid.NewGuid();
        var card1 = new Card
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            DeckId = deckId,
            Question = "Q1",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        var card2 = new Card
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            DeckId = deckId,
            Question = "Q2",
            CreatedAt = DateTime.UtcNow
        };

        _cardRepositoryMock
            .Setup(r => r.GetCardAsync(card1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card1);

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card> { card2, card1 });

        var callbackQuery = new CallbackQuery
        {
            Id = "query_3",
            Data = $"{BotConstants.RateCardCallbackPrefix}{card1.Id:N}:5",
            Message = new Message { Chat = new Chat { Id = 12345 } }
        };

        await _handler.HandleAsync(_botClientMock.Object, callbackQuery, CancellationToken.None);

        _botClientMock.Verify(
            b => b.SendRequest(
                It.Is<AnswerCallbackQueryRequest>(req => req.CallbackQueryId == callbackQuery.Id),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _botClientMock.Verify(
            b => b.SendRequest(
                It.Is<SendMessageRequest>(req =>
                    req.ChatId.Identifier == 12345 &&
                    req.Text == "Q2"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_LastCardRated_SendsDeckCompletedMessage()
    {
        var deckId = Guid.NewGuid();
        var card1 = new Card
        {
            Id = Guid.NewGuid(),
            DeckId = deckId,
            Question = "Q1",
            CreatedAt = DateTime.UtcNow
        };

        _cardRepositoryMock
            .Setup(r => r.GetCardAsync(card1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(card1);

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card> { card1 });

        var callbackQuery = new CallbackQuery
        {
            Id = "query_4",
            Data = $"{BotConstants.RateCardCallbackPrefix}{card1.Id:N}:4",
            Message = new Message { Chat = new Chat { Id = 12345 } }
        };

        await _handler.HandleAsync(_botClientMock.Object, callbackQuery, CancellationToken.None);

        _botClientMock.Verify(
            b => b.SendRequest(
                It.Is<SendMessageRequest>(req => req.Text == "Колода пройдена"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
