using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot.Commands;

public class StartCommand : IBotCommand
{
    public string Command => "/start";
    private readonly IStudentRepository _studentRepository;

    public StartCommand(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var telegramUser = message.From;

        if (telegramUser == null)
        {
            await botClient.SendMessage(chatId: message.Chat.Id, text: "Доступа нет, напиши ментору", cancellationToken: cancellationToken);
            return;
        }

        var incomingUsername = telegramUser.Username?.Trim().TrimStart('@');
        var students = await _studentRepository.GetAllAsync(cancellationToken);

        var studentFound = students.FirstOrDefault(s =>(s.TelegramId.HasValue && s.TelegramId.Value == telegramUser.Id) || (!string.IsNullOrEmpty(incomingUsername) && string.Equals(s.TelegramUserName?.Trim().TrimStart('@'), incomingUsername, StringComparison.OrdinalIgnoreCase)));

        if (studentFound == null)
        {
            await botClient.SendMessage(chatId: message.Chat.Id, text: "Доступа нет, напиши ментору", cancellationToken: cancellationToken);
            return;
        }

        var student = await _studentRepository.GetByIdAsync(studentFound.Id, cancellationToken);

        if (student != null && student.TelegramId != telegramUser.Id)
        {
            student.TelegramId = telegramUser.Id;
            await _studentRepository.SaveChangesAsync(cancellationToken);
        }

        var displayName = !string.IsNullOrWhiteSpace(student?.TelegramUserName) ? student.TelegramUserName.TrimStart('@') : (telegramUser.Username ?? telegramUser.FirstName);

        await botClient.SendMessage(chatId: message.Chat.Id, text: $"Привет, {displayName}!", cancellationToken: cancellationToken);
    }
}
