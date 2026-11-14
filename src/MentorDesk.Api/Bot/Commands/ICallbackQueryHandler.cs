using Telegram.Bot;
using Telegram.Bot.Types;

namespace MentorDesk.Api.Bot.Commands;

public interface ICallbackQueryHandler
{
    string CallbackPrefix { get; }

    Task HandleAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken);
}
