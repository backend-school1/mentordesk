using MediatR;
using MentorDesk.Api.Bot.Commands;
using MentorDesk.Application.Features.Students.Access;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot;

/// <summary>
/// Обработчик одного апдейта. Пока отвечает заглушкой на любое сообщение;
/// команды (/start, /decks и т.д.) добавляются задачами из бэклога.
/// </summary>
public class UpdateHandler(ILogger<UpdateHandler> logger, IEnumerable<IBotCommand> commands, ISender sender)
{
    public async Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { Text: { } text } message)
        {
            return;
        }
        logger.LogInformation("Сообщение от {ChatId}: {Text}", message.Chat.Id, text);

        var rawCommand = text.Split(' ')[0];
        var commandName = rawCommand.Split('@')[0].ToLower();

        var telegramUser = message.From;

        if (telegramUser is null)
        {
            await bot.SendMessage(message.Chat.Id, "Доступа нет, напиши ментору.", cancellationToken: ct);
            return;
        }

        var accessResult = await sender.Send(new CheckStudentAccessQuery(telegramUser.Id, telegramUser.Username ?? string.Empty),
                cancellationToken: ct);

        if (accessResult.Status == StudentAccessStatus.Denied)
        {
            await bot.SendMessage(message.Chat.Id, "Доступа нет, напиши ментору.", cancellationToken: ct);
            return;
        }

        if (accessResult.Status == StudentAccessStatus.RequiresBinding && !string.Equals(commandName, "/start",
            StringComparison.OrdinalIgnoreCase))
        {
            await bot.SendMessage(message.Chat.Id, "Для начала работы отправь /start",
                cancellationToken: ct);
            return;
        }

        var command = commands.FirstOrDefault(c => string.Equals(c.Command, commandName,
            StringComparison.OrdinalIgnoreCase));

        if (command != null)
        {
            await command.ExecuteAsync(bot, message, ct);
            return;
        }

        await bot.SendMessage(chatId: message.Chat.Id, text: "Неизвестная команда.", cancellationToken: ct);
    }
}
