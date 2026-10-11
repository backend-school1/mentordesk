using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot;

/// <summary>
/// Обработчик одного апдейта. Пока отвечает заглушкой на любое сообщение;
/// команды (/start, /decks и т.д.) добавляются задачами из бэклога.
/// </summary>
public class UpdateHandler(ILogger<UpdateHandler> logger)
{
    public async Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { Text: { } text } message)
        {
            return;
        }

        logger.LogInformation("Сообщение от {ChatId}: {Text}", message.Chat.Id, text);

        await bot.SendMessage(
            message.Chat.Id,
            "MentorDesk в разработке. Команды появятся скоро.",
            cancellationToken: ct);
    }
}
