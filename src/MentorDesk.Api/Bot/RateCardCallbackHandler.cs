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
        var data = callbackQuery.Data![BotConstants.RateCardCallbackPrefix.Length..];
        var parts = data.Split(':');

        if (parts.Length < 3 || !Guid.TryParse(parts[0], out var deckId) || !int.TryParse(parts[1], out var cardIndex))
        {
            return;
        }

        var nextIndex = cardIndex + 1;
        var cards = await _cardRepository.GetCardsByDeckAsync(deckId, cancellationToken);

        if (cards == null || nextIndex >= cards.Count)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var nextCard = cards[nextIndex];
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Показать ответ", $"{BotConstants.ShowAnswerCallbackPrefix}{deckId}:{nextIndex}")
        });

        await botClient.SendMessage(
            chatId: callbackQuery.Message!.Chat.Id,
            text: nextCard.Question,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}
