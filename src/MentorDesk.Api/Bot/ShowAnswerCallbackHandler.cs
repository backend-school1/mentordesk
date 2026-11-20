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

        if (string.IsNullOrEmpty(callbackQuery.Data) ||
            !callbackQuery.Data.StartsWith(BotConstants.ShowAnswerCallbackPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var cardIdString = callbackQuery.Data[BotConstants.ShowAnswerCallbackPrefix.Length..];

        if (!Guid.TryParse(cardIdString, out var cardId))
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

        var ratingButtons = Enumerable.Range(1, 5).Select(score =>
            InlineKeyboardButton.WithCallbackData(
                score.ToString(),
                $"{BotConstants.RateCardCallbackPrefix}{cardId:N}:{score}"
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
