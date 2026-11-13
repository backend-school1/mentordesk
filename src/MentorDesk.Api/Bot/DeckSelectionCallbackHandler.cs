using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot;

public class DeckSelectionCallbackHandler : ICallbackQueryHandler
{
    private readonly IStudentRepository _studentRepository;
    private readonly IDeckRepository _deckRepository;

    public DeckSelectionCallbackHandler(IStudentRepository studentRepository, IDeckRepository deckRepository)
    {
        _studentRepository = studentRepository;
        _deckRepository = deckRepository;
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
    }
}
