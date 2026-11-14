using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MentorDesk.Api.Bot;

public class ShowAnswerCallbackHandler : ICallbackQueryHandler
{
    private readonly ICardRepository _cardRepository;

    public string CallbackPrefix => BotConstants.ShowAnswerCallbackPrefix;

    public ShowAnswerCallbackHandler(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var data = callbackQuery.Data![BotConstants.ShowAnswerCallbackPrefix.Length..];
        var parts = data.Split(':');

        if (parts.Length < 2 || !Guid.TryParse(parts[0], out var deckId) || !int.TryParse(parts[1], out var cardIndex))
        {
            return;
        }

        var cards = await _cardRepository.GetCardsByDeckAsync(deckId, cancellationToken);
        if (cards == null || cardIndex >= cards.Count)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var currentCard = cards[cardIndex];

        var ratingButtons = Enumerable.Range(1, 5).Select(score =>
            InlineKeyboardButton.WithCallbackData(
                score.ToString(),
                $"{BotConstants.RateCardCallbackPrefix}{deckId}:{cardIndex}:{score}"
            )
        ).ToArray();

        var keyboard = new InlineKeyboardMarkup(ratingButtons);

        await botClient.SendMessage(
            chatId: callbackQuery.Message!.Chat.Id,
            text: currentCard.Answer,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}
