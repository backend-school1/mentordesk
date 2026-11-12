using MentorDesk.Application.Abstractions.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace MentorDesk.Api.Bot.Commands;

public class DecksCommand : IBotCommand
{
    public string Command => "/decks";

    private readonly IStudentRepository _studentRepository;
    private readonly IDeckRepository _deckRepository;

    public DecksCommand(IStudentRepository studentRepository, IDeckRepository deckRepository)
    {
        _studentRepository = studentRepository;
        _deckRepository = deckRepository;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
    {
        var telegramUser = message.From;

        if (telegramUser == null)
        {
            await botClient.SendMessage(chatId: message.Chat.Id,
                text: "Доступа нет, напиши ментору",
                cancellationToken: cancellationToken);

            return;
        }

        var studentFound = await _studentRepository.GetByTelegramIdAsync(telegramUser.Id, cancellationToken);

        if (studentFound == null)
        {
            await botClient.SendMessage(chatId: message.Chat.Id,
                text: "Доступа нет, напиши ментору",
                cancellationToken: cancellationToken);
            return;
        }

        var decks = await _deckRepository.GetAllDecksAsync(cancellationToken);

        if (decks.Count == 0)
        {
            await botClient.SendMessage(chatId: message.Chat.Id,
                text: "Колод пока нет",
                cancellationToken: cancellationToken);

            return;
        }

        var keyboard = new InlineKeyboardMarkup(decks.Select(deck => new[]
        {
            InlineKeyboardButton.WithCallbackData(deck.Title,
                $"{BotConstants.SelectDeckCallbackPrefix}{deck.Id}")
        }));

        await botClient.SendMessage(chatId: message.Chat.Id,
            text: "Выбери колоду для обучения:",
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }
}
