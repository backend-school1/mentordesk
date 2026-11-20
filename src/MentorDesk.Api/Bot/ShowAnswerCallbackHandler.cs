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
        await botClient.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: cancellationToken);

        var data = callbackQuery.Data![BotConstants.ShowAnswerCallbackPrefix.Length..];
        var parts = data.Split(':');

        if (parts.Length < 2 || !Guid.TryParse(parts[0], out var deckId) || !Guid.TryParse(parts[1], out var cardId))
        {
            return;
        }

        var cards = await _cardRepository.GetCardsByDeckAsync(deckId, cancellationToken);
        var currentCard = cards?.FirstOrDefault(c => c.Id == cardId);

        if (currentCard == null)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var ratingButtons = Enumerable.Range(1, 5).Select(score =>
            InlineKeyboardButton.WithCallbackData(
                score.ToString(),
                $"{BotConstants.RateCardCallbackPrefix}{deckId}:{cardId}:{score}"
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
