using System.Globalization;
using CsvHelper;
using MediatR;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Exceptions;
using MentorDesk.Domain.Entities;

namespace MentorDesk.Application.Features.Decks.Import;

public class ImportDeckCommandHandler : IRequestHandler<ImportDeckCommand, ImportResult>
{
    private readonly IDeckRepository _deckRepository;
    private readonly ICardRepository _cardRepository;

    public ImportDeckCommandHandler(IDeckRepository deckRepository, ICardRepository cardRepository)
    {
        _deckRepository = deckRepository;
        _cardRepository = cardRepository;
    }

    public async Task<ImportResult> Handle(ImportDeckCommand request, CancellationToken cancellationToken)
    {
        var deck = await _deckRepository.GetDeckAsync(request.DeckId, cancellationToken);

        if (deck is null)
        {
            throw new NotFoundException($"Колода с id {request.DeckId} не найдена");
        }

        using var reader = new StreamReader(request.Csv, leaveOpen: true);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<CardCsvRow>().ToList();

        var existingCards = await _cardRepository.GetCardsByDeckAsync(request.DeckId, cancellationToken);
        var existingCardsDict = existingCards.ToDictionary(x => x.Question);

        var created = 0;
        var newCards = new List<Card>();

        foreach (CardCsvRow record in records)
        {
            if (!existingCardsDict.ContainsKey(record.Question))
            {
                var newCard = new Card
                {
                    DeckId = request.DeckId,
                    Question = record.Question,
                    Answer = record.Answer
                };

                newCards.Add(newCard);
                created++;
            }
        }

        if (newCards.Count > 0)
        {
            await _cardRepository.AddCards(newCards, cancellationToken);
        }

        await _cardRepository.SaveChangesAsync(cancellationToken);

        return new ImportResult(created);
    }
}
