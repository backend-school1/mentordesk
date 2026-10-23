using MediatR;
using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.DTOs.Deck;
using MentorDesk.Application.Features.Decks.Import;
using MentorDesk.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace MentorDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DecksController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IDeckService _deckService;
    private readonly ICardService _cardService;

    public DecksController(IMediator mediator, IDeckService deckService, ICardService cardService)
    {
        _mediator = mediator;
        _deckService = deckService;
        _cardService = cardService;
    }

    [HttpPost]
    [Route("{deckId}/import")]
    public async Task<ActionResult<ImportResult>> ImportDeck(Guid deckId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new ImportDeckCommand(deckId, stream), cancellationToken);

        return result.IsSuccess ?
            Ok(result.Value) :
            NotFound(new
            {
                message = result.ErrorMessage,
                statusCode = StatusCodes.Status404NotFound
            });
    }

    [HttpGet]
    public async Task<IActionResult> GetDecksAsync(CancellationToken cancellationToken)
    {
        var decks = await _deckService.GetAllDecksAsync(cancellationToken);

        return Ok(decks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckService.CreateDeckAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, deck);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeck([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var deck = await _deckService.GetByDeckIdAsync(id, cancellationToken);

        if (deck is null)
        {
            return NotFound();
        }

        return Ok(deck);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeck([FromRoute] Guid id, [FromBody] UpdateDeckRequest request, CancellationToken cancellationToken)
    {
        var deck = await _deckService.GetByDeckIdAsync(id, cancellationToken);

        if (deck is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException(
                "Название колоды не должно быть пустым.",
                nameof(request.Title));
        }

        deck.Title = request.Title.Trim();

        await _deckService.UpdateDeckAsync(id, request, cancellationToken);

        return Ok(new DeckResponse
        {
            Id = deck.Id,
            Title = deck.Title
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _deckService.DeleteDeckAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{deckId:guid}/cards")]
    public async Task<ActionResult<List<CardResponse>>> GetCardsAsync([FromRoute] Guid deckId, CancellationToken cancellationToken)
    {
        var cards = await _cardService.GetCardsAsync(deckId, cancellationToken);

        return Ok(cards);
    }

    [HttpPost("{deckId:guid}/cards")]
    public async Task<ActionResult<CardResponse>> CreateCardAsync([FromRoute] Guid deckId, [FromBody] CreateCardRequest request, CancellationToken cancellationToken)
    {
        var card = await _cardService.CreateCardAsync(deckId, request, cancellationToken);

        return StatusCode(StatusCodes.Status201Created, card);
    }
}
