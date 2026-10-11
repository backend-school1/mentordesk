using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MentorDesk.Api.Bot;

/// <summary>
/// Получает апдейты от Telegram через long polling и передаёт каждый в <see cref="UpdateHandler"/>.
/// Если токен бота не задан, сервис не запускается — API работает без бота.
/// </summary>
public class BotHostedService(
    IServiceProvider services,
    IServiceScopeFactory scopeFactory,
    ILogger<BotHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var bot = services.GetService<ITelegramBotClient>();
        if (bot is null)
        {
            logger.LogWarning("Telegram:BotToken не задан — бот не запущен. Как задать токен, см. README.");
            return;
        }

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
        };

        logger.LogInformation("Бот запущен, жду апдейты");
        await bot.ReceiveAsync(HandleUpdateAsync, HandleErrorAsync, receiverOptions, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        // Отдельный scope на каждый апдейт — чтобы в обработчике можно было брать DbContext и сервисы.
        await using var scope = scopeFactory.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
        try
        {
            await handler.HandleAsync(bot, update, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обработке апдейта {UpdateId}", update.Id);
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        logger.LogError(exception, "Ошибка при получении апдейтов от Telegram");
        return Task.CompletedTask;
    }
}
