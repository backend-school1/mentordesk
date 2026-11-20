using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MentorDesk.Api.Bot;

public class RateCardCallbackHandler : ICallbackQueryHandler
{
    private readonly ICardRepository _cardRepository;

    public string CallbackPrefix => BotConstants.RateCardCallbackPrefix;

    public RateCardCallbackHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);

        if (string.IsNullOrEmpty(callbackQuery.Data) ||
            !callbackQuery.Data.StartsWith(BotConstants.RateCardCallbackPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var data = callbackQuery.Data[BotConstants.RateCardCallbackPrefix.Length..];
        var parts = data.Split(':');

        if (parts.Length < 2 ||
            !Guid.TryParse(parts[0], out var cardId) ||
            !int.TryParse(parts[1], out var score))
        {
            return;
        }

        var currentCard = await _cardRepository.GetCardAsync(cardId, cancellationToken);
        if (currentCard == null)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Карточка пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var cards = await _cardRepository.GetCardsByDeckAsync(currentCard.DeckId, cancellationToken);
        if (cards == null || cards.Count == 0)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var orderedCards = cards.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id).ToList();
        var currentIndex = orderedCards.FindIndex(c => c.Id == cardId);
        var nextIndex = currentIndex + 1;

        if (currentIndex == -1 || nextIndex >= orderedCards.Count)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var nextCard = orderedCards[nextIndex];

        var keyboard = new InlineKeyboardMarkup(new[]
        {
        InlineKeyboardButton.WithCallbackData(
            "Показать ответ",
            $"{BotConstants.ShowAnswerCallbackPrefix}{nextCard.Id}")
    });

        await botClient.SendMessage(
            chatId: callbackQuery.Message!.Chat.Id,
            text: nextCard.Question,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}
