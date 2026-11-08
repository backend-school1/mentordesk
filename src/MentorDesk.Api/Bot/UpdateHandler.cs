using MentorDesk.Api.Bot.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot;

/// <summary>
/// Обработчик одного апдейта.
/// команды (/start, /decks и т.д.) добавляются задачами из бэклога.
/// </summary>
public class UpdateHandler(ILogger<UpdateHandler> logger, IEnumerable<IBotCommand> commands, DeckSelectionCallbackHandler deckSelectionCallbackHandler)
{
    public async Task HandleAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.CallbackQuery is { } callbackQuery)
        {
            await deckSelectionCallbackHandler.HandleAsync(bot, callbackQuery, ct);
            return;
        }

        if (update.Message is not { Text: { } text } message)
        {
            return;
        }

        logger.LogInformation("Сообщение от {ChatId}: {Text}", message.Chat.Id, text);

        var rawCommand = text.Split(' ')[0];
        var commandName = rawCommand.Split('@')[0].ToLower();

        var command = commands.FirstOrDefault(c => string.Equals(c.Command, commandName, StringComparison.OrdinalIgnoreCase));

        if (command != null)
        {
            await command.ExecuteAsync(bot, message, ct);
            return;
        }

        await bot.SendMessage(chatId: message.Chat.Id, text: "Неизвестная команда.", cancellationToken: ct);
    }
}
