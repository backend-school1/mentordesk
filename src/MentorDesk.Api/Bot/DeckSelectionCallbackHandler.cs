using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MentorDesk.Api.Bot;

public class DeckSelectionCallbackHandler : ICallbackQueryHandler
{
    private readonly IStudentRepository _studentRepository;
    private readonly IDeckRepository _deckRepository;
    private readonly ICardRepository _cardRepository;

    public DeckSelectionCallbackHandler(IStudentRepository studentRepository, IDeckRepository deckRepository, ICardRepository cardRepository)
    {
        _studentRepository = studentRepository;
        _deckRepository = deckRepository;
        _cardRepository = cardRepository;
    }

    public string CallbackPrefix => BotConstants.SelectDeckCallbackPrefix;

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackData = callbackQuery.Data;

        if (string.IsNullOrEmpty(callbackData)
            || !callbackData.StartsWith(BotConstants.SelectDeckCallbackPrefix, StringComparison.Ordinal))
        {
            await botClient.AnswerCallbackQuery(
                callbackQuery.Id,
                text: "",
                cancellationToken: cancellationToken);

            return;
        }

        var deckIdString = callbackData[BotConstants.SelectDeckCallbackPrefix.Length..];

        if (!Guid.TryParse(deckIdString, out var deckId))
        {
            await botClient.AnswerCallbackQuery(
                callbackQuery.Id,
                text: "",
                cancellationToken: cancellationToken);

            return;
        }

        var deck = await _deckRepository.GetDeckAsync(deckId, cancellationToken);

        if (deck == null)
        {
            await botClient.AnswerCallbackQuery(
                callbackQuery.Id,
                text: "Колода не найдена",
                cancellationToken: cancellationToken);

            return;
        }

        var telegramUser = callbackQuery.From;

        var student = await _studentRepository.GetByTelegramIdAsync(telegramUser.Id, cancellationToken);

        if (student == null)
        {
            await botClient.AnswerCallbackQuery(
                callbackQuery.Id,
                text: "Доступа нет, напиши ментору",
                cancellationToken: cancellationToken);

            return;
        }

        student.SelectedDeckId = deck.Id;

        await _studentRepository.SaveChangesAsync(cancellationToken);

        await botClient.AnswerCallbackQuery(
            callbackQuery.Id,
            text: $"Колода выбрана: {deck.Title}",
            cancellationToken: cancellationToken);

        var cards = await _cardRepository.GetCardsByDeckAsync(deck.Id, cancellationToken);

        if (cards == null || cards.Count == 0)
        {
            await botClient.SendMessage(
                chatId: callbackQuery.Message!.Chat.Id,
                text: "Колода пройдена",
                cancellationToken: cancellationToken);
            return;
        }

        var firstCard = cards[0];
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            InlineKeyboardButton.WithCallbackData("Показать ответ", $"{BotConstants.ShowAnswerCallbackPrefix}{deck.Id}:0")
        });

        await botClient.SendMessage(
            chatId: callbackQuery.Message!.Chat.Id,
            text: firstCard.Question,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}
