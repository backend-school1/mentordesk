using FluentValidation;
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
        var result = await _deckService.GetAllDecksAsync(cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request, [FromServices] IValidator<CreateDeckRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var result = await _deckService.CreateDeckAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var deck = result.Value;

        return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, deck);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeck([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _deckService.GetByDeckIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeck([FromRoute] Guid id, [FromBody] UpdateDeckRequest request, [FromServices] IValidator<UpdateDeckRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var result = await _deckService.UpdateDeckAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _deckService.DeleteDeckAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("{deckId:guid}/cards")]
    public async Task<ActionResult<List<CardResponse>>> GetCardsAsync([FromRoute] Guid deckId, CancellationToken cancellationToken)
    {
        var result = await _cardService.GetCardsAsync(deckId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost("{deckId:guid}/cards")]
    public async Task<ActionResult<CardResponse>> CreateCardAsync([FromRoute] Guid deckId, [FromBody] CreateCardRequest request, [FromServices] IValidator<CreateCardRequest> validator, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var result = await _cardService.CreateCardAsync(deckId, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
