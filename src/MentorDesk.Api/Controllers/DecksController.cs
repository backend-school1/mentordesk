using FluentValidation;
using MediatR;
using MentorDesk.Application.DTOs.Card;
using MentorDesk.Application.DTOs.Deck;
using MentorDesk.Application.Features.Cards.CreateCard;
using MentorDesk.Application.Features.Cards.GetCards;
using MentorDesk.Application.Features.Decks.CreateDeck;
using MentorDesk.Application.Features.Decks.DeleteDeck;
using MentorDesk.Application.Features.Decks.GetAllDecks;
using MentorDesk.Application.Features.Decks.GetByIdDeck;
using MentorDesk.Application.Features.Decks.Import;
using MentorDesk.Application.Features.Decks.UpdateDeck;
using Microsoft.AspNetCore.Mvc;

namespace MentorDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DecksController : ControllerBase
{
    private readonly IMediator _mediator;

    public DecksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("{deckId}/import")]
    public async Task<ActionResult<ImportResult>> ImportDeck(Guid deckId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await _mediator.Send(new ImportDeckCommand(deckId, stream), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetDecksAsync(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllDecksQuery(), cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDeck([FromBody] CreateDeckRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDeckCommand(request.Title, request.Description), cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        var deck = result.Value!;

        return CreatedAtAction(nameof(GetDeck), new { id = deck.Id }, deck);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeck([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetByIdQuery(id), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDeck([FromRoute] Guid id, [FromBody] UpdateDeckRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDeckCommand(id, request.Title, request.Description), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDeckCommand(id), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return NoContent();
    }

    [HttpGet("{deckId:guid}/cards")]
    public async Task<ActionResult<List<CardResponse>>> GetCardsAsync([FromRoute] Guid deckId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCardsQuery(deckId), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost("{deckId:guid}/cards")]
    public async Task<ActionResult<CardResponse>> CreateCardAsync([FromRoute] Guid deckId, [FromBody] CreateCardRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCardCommand(deckId, request.Question, request.Answer), cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(result.ErrorMessage);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }
}
