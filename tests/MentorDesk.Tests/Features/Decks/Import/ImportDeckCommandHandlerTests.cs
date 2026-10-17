using System.Text;
using FluentAssertions;
using MentorDesk.Application.Abstractions.Repositories;
using MentorDesk.Application.Features.Decks.Import;
using MentorDesk.Domain.Entities;
using Moq;

namespace MentorDesk.Tests.Features.Decks.Import;

public class ImportDeckCommandHandlerTests
{
    private readonly Mock<IDeckRepository> _deckRepositoryMock;
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly ImportDeckCommandHandler _handler;

    public ImportDeckCommandHandlerTests()
    {
        _deckRepositoryMock = new Mock<IDeckRepository>();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _handler = new ImportDeckCommandHandler(
            _deckRepositoryMock.Object,
            _cardRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenDeckExistsAndCardsAreNew_ShouldCreateCardsAndReturnCorrectResult()
    {
        var deckId = Guid.NewGuid();
        var deck = new Deck()
        {
            Id = deckId,
            Title = "Test Deck"
        };

        _deckRepositoryMock
            .Setup(r => r.GetDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deck);

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card>());

        var csv = "Question,Answer\r\nQ1,A1\r\nQ2,A2";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var command = new ImportDeckCommand(deckId, stream);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Created.Should().Be(2);
        result.Updated.Should().Be(0);

        _cardRepositoryMock.Verify(
            r => r.AddCards(It.Is<IEnumerable<Card>>(cards => cards.Count() == 2), It.IsAny<CancellationToken>()),
            Times.Once);

        _cardRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenDeckExistsAndCardAlreadyExists_ShouldUpdateCardAndReturnCorrectResult()
    {
        var deckId = Guid.NewGuid();
        var deck = new Deck { Id = deckId, Title = "Test Deck" };

        var existingCard = new Card
        {
            Id = Guid.NewGuid(),
            DeckId = deckId,
            Question = "Q1",
            Answer = "Old Answer"
        };

        _deckRepositoryMock
            .Setup(r => r.GetDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deck);

        _cardRepositoryMock
            .Setup(r => r.GetCardsByDeckAsync(deckId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Card> { existingCard });

        var csvContent = "Question,Answer\r\nQ1,New Answer";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvContent));

        var command = new ImportDeckCommand(deckId, stream);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Created.Should().Be(0);
        result.Updated.Should().Be(1);

        existingCard.Answer.Should().Be("New Answer");

        _cardRepositoryMock.Verify(
            r => r.AddCards(It.IsAny<IEnumerable<Card>>(), It.IsAny<CancellationToken>()),
            Times.Never());

        _cardRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
