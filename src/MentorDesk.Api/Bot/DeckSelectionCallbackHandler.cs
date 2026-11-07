using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot;

public class DeckSelectionCallbackHandler
{
    private readonly IStudentRepository _studentRepository;
    private readonly IDeckRepository _deckRepository;

    public DeckSelectionCallbackHandler(IStudentRepository studentRepository, IDeckRepository deckRepository)
    {
        _studentRepository = studentRepository;
        _deckRepository = deckRepository;
    }

    public async Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var callbackData = callbackQuery.Data;

        if (string.IsNullOrEmpty(callbackData) 
            || !callbackData.StartsWith(BotConstants.SelectDeckCallbackPrefix, StringComparison.Ordinal))
        {
            return;
        }

        var deckIdString = callbackData[BotConstants.SelectDeckCallbackPrefix.Length..];

        if (!Guid.TryParse(deckIdString, out var deckId))
        {
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
        var incomingUsername = telegramUser.Username?.Trim().TrimStart('@');

        var students = await _studentRepository.GetAllAsync(cancellationToken);
        
        var studentFound = students.FirstOrDefault(s =>
            (s.TelegramId.HasValue && s.TelegramId.Value == telegramUser.Id) ||
            (!string.IsNullOrEmpty(incomingUsername) &&
             string.Equals(
                 s.TelegramUserName?.Trim().TrimStart('@'),
                 incomingUsername,
                 StringComparison.OrdinalIgnoreCase)));

        if (studentFound == null)
        {
            await botClient.AnswerCallbackQuery(
                callbackQuery.Id,
                text: "Доступа нет, напиши ментору",
                cancellationToken: cancellationToken);
            
            return;
        }

        var student = await _studentRepository.GetByIdAsync(studentFound.Id, cancellationToken);

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
            text: "Колода выбрана",
            cancellationToken: cancellationToken);
    }
}
