using MentorDesk.Api.Bot;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Domain.Entities;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Xunit;

namespace MentorDesk.Tests.Api.Bot;

public class ShowAnswerCallbackHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepositoryMock = new();
    private readonly Mock<ITelegramBotClient> _botClientMock = new();
    private readonly ShowAnswerCallbackHandler _handler;

    public ShowAnswerCallbackHandlerTests()
    {
        _handler = new ShowAnswerCallbackHandler(_cardRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ValidCard_SendsAnswerWithRatingButtons()
    {
        var deckId = Guid.NewGuid();
        var cardId = Guid.NewGuid();
        var card = new Card { Id = cardId, DeckId = deckId, Question = "Q1", Answer = "A1" };

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card> { card });

        var callbackQuery = new CallbackQuery
        {
            Id = "query_1",
            Data = $"{BotConstants.ShowAnswerCallbackPrefix}{deckId}:{cardId}",
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
                    req.Text == "A1" &&
                    req.ReplyMarkup is InlineKeyboardMarkup),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_CardNotFound_SendsDeckCompletedMessage()
    {
        var deckId = Guid.NewGuid();
        var cardId = Guid.NewGuid();

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card>());

        var callbackQuery = new CallbackQuery
        {
            Id = "query_2",
            Data = $"{BotConstants.ShowAnswerCallbackPrefix}{deckId}:{cardId}",
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
